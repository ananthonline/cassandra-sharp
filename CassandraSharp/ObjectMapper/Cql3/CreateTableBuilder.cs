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

namespace CassandraSharp.ObjectMapper.Cql3
{
    using System;
    using System.Text;
    using CassandraSharp.ObjectMapper.Dialect;
    using CassandraSharp.Utils;

    public class CreateTableBuilder : ICreateTableBuilder
    {
        public string Build()
        {
            Validate();

            StringBuilder sb = new StringBuilder();

            // create table columns
            sb.AppendFormat("create table {0} (", Table);
            for (int i = 0; i < Columns.Length; ++i)
            {
                string name = Columns[i];
                string type = ColumnTypes[i];
                sb.AppendFormat("{0} {1},", name, type);
            }

            string sep = "primary key (";
            foreach (string key in Keys)
            {
                sb.AppendFormat("{0}{1}", sep, key);
                sep = ",";
            }
            sb.Append("))");

            if (CompactStorage.HasValue && CompactStorage.Value)
            {
                sb.Append(" with compact storage");
            }

            return sb.ToString();
        }

        public string Table { get; set; }

        public string[] Columns { get; set; }

        public string[] ColumnTypes { get; set; }

        public string[] Keys { get; set; }

        public bool? CompactStorage { get; set; }

        private void Validate()
        {
            Columns.CheckArrayHasAtLeastOneElement("Columns");
            Table.CheckArgumentNotNull("Table");
            ColumnTypes.CheckArrayHasAtLeastOneElement("ColumnTypes");
            Keys.CheckArrayHasAtLeastOneElement("Keys");
            Columns.CheckArrayIsSameLengthAs(ColumnTypes, "Columns", "ColumnTypes");
        }
    }
}