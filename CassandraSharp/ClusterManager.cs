﻿namespace CassandraSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using CassandraSharp.Config;
    using CassandraSharp.EndpointStrategy;
    using CassandraSharp.Factory;
    using CassandraSharp.Pool;
    using CassandraSharp.Snitch;
    using CassandraSharp.Transport;
    using CassandraSharp.Utils;

    public class ClusterManager
    {
        private static CassandraSharpConfig _config;

        public static ICluster GetCluster(string name)
        {
            if (null == _config)
            {
                throw new InvalidOperationException("ClusterManager is not initialized");
            }

            if (null == name)
            {
                throw new ArgumentNullException("name");
            }

            ClusterConfig clusterConfig = GetClusterConfig(name);
            return GetCluster(clusterConfig);
        }

        public static ICluster GetCluster(ClusterConfig clusterConfig)
        {
            if (null == clusterConfig)
            {
                throw new ArgumentNullException("clusterConfig");
            }

            if (null == clusterConfig.Behavior)
            {
                throw new ArgumentNullException("clusterConfig.Behavior");
            }

            if (null == clusterConfig.Endpoints)
            {
                throw new ArgumentNullException("clusterConfig.Endpoints");
            }

            TransportConfig transportConfig = clusterConfig.Transport ?? new TransportConfig();

            // create endpoints
            ISnitch snitch = clusterConfig.Endpoints.Snitch.Create();
            IPAddress clientAddress = NetworkFinder.Find(Dns.GetHostName());
            IEnumerable<Endpoint> endpoints = GetEndpoints(clusterConfig.Endpoints, snitch, clientAddress);

            // create endpoint strategy
            IEndpointStrategy endpointsManager = clusterConfig.Endpoints.Create(endpoints);
            IPool<IConnection> pool = PoolType.Stack.Create(clusterConfig.Behavior.PoolSize);

            // create the cluster now
            ITransportFactory transportFactory = transportConfig.Create();
            return new Cluster(clusterConfig.Behavior, pool, transportFactory, endpointsManager);
        }

        private static IEnumerable<Endpoint> GetEndpoints(EndpointsConfig config, ISnitch snitch, IPAddress clientAddress)
        {
            List<Endpoint> endpoints = new List<Endpoint>();
            foreach (string server in config.Servers)
            {
                IPAddress serverAddress = NetworkFinder.Find(server);
                if (null != serverAddress)
                {
                    string datacenter = snitch.GetDataCenter(serverAddress);
                    int proximity = snitch.ComputeProximity(clientAddress, serverAddress);
                    Endpoint endpoint = new Endpoint(server, serverAddress, datacenter, proximity);
                    endpoints.Add(endpoint);
                }
            }

            return endpoints;
        }

        private static ClusterConfig GetClusterConfig(string name)
        {
            ClusterConfig clusterConfig = (from config in _config.Clusters
                                           where config.Name == name
                                           select config).FirstOrDefault();
            if (null == clusterConfig)
            {
                string msg = string.Format("Can't find cluster configuration '{0}'", name);
                throw new KeyNotFoundException(msg);
            }

            return clusterConfig;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Shutdown()
        {
            if (null == _config)
            {
                return;
            }

            _config = null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Configure(CassandraSharpConfig config)
        {
            if (null != _config)
            {
                throw new InvalidOperationException("ClusterManager is already initialized");
            }

            if (null == config)
            {
                throw new ArgumentNullException("config");
            }

            _config = config;
        }
    }
}