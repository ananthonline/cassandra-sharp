﻿// cassandra-sharp - a .NET client for Apache Cassandra
// Copyright (c) 2011-2012 Pierre Chalamet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Samples.Async
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CassandraSharp;
    using CassandraSharp.CQLPoco;
    using CassandraSharp.Config;

    public class SchemaKeyspaces
    {
        public bool DurableWrites { get; set; }

        public string KeyspaceName { get; set; }

        // ReSharper disable InconsistentNaming
        public string strategy_Class { get; set; }

        // ReSharper restore InconsistentNaming

        // ReSharper disable InconsistentNaming
        public string strategy_options { get; set; }

        // ReSharper restore InconsistentNaming
    }

    public static class AsyncSample
    {
        public static void Run()
        {
            XmlConfigurator.Configure();
            using (ICluster cluster = ClusterManager.GetCluster("TestCassandra"))
            {
                const string cqlKeyspaces = "SELECT * from system.schema_keyspaces";

                var allTasks = new List<Task>();
                for (int i = 0; i < 100; ++i)
                {
                    var futRes = cluster.Execute<SchemaKeyspaces>(cqlKeyspaces).ContinueWith(t => DisplayKeyspace(t.Result));
                    allTasks.Add(futRes);
                }

                Task.WaitAll(allTasks.ToArray());
            }

            ClusterManager.Shutdown();
        }

        private static void DisplayKeyspace(IEnumerable<SchemaKeyspaces> result)
        {
            try
            {
                foreach (var resKeyspace in result)
                {
                    Console.WriteLine("DurableWrites={0} KeyspaceName={1} strategy_Class={2} strategy_options={3}",
                                      resKeyspace.DurableWrites, resKeyspace.KeyspaceName, resKeyspace.strategy_Class, resKeyspace.strategy_options);
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Command failed {0}", ex.Message);
            }
        }
    }
}