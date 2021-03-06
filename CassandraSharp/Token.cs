// cassandra-sharp - a .NET client for Apache Cassandra
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

namespace CassandraSharp
{
    using System;
    using CassandraSharp.Utils;

    public class Token : IComparable<Token>,
                         IEquatable<Token>
    {
        private readonly byte[] _serializedNumber;

        public Token(byte[] serializedNumber)
        {
            serializedNumber.CheckArgumentNotNull("serializedNumber");
            _serializedNumber = serializedNumber;
        }

        public int CompareTo(Token other)
        {
            other.CheckArgumentNotNull("other");

            if (_serializedNumber.Length < other._serializedNumber.Length)
            {
                return -1;
            }

            if (_serializedNumber.Length > other._serializedNumber.Length)
            {
                return 1;
            }

            for (int i = 0; i < _serializedNumber.Length; ++i)
            {
                if (_serializedNumber[i] < other._serializedNumber[i])
                {
                    return -1;
                }

                if (_serializedNumber[i] > other._serializedNumber[i])
                {
                    return 1;
                }
            }

            return 0;
        }

        public bool Equals(Token other)
        {
            return 0 == CompareTo(other);
        }
    }
}