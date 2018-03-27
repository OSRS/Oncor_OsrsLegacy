//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Osrs.Data
{
    public interface IDbParameterBag : IEnumerable<DbParameter>
    {
        void Add(DbParameter parameter);

        DbParameter Get(string name);
    }

    public interface IDbParameterBag<T> : IDbParameterBag
        where T : DbParameter
    {
        void Add(T parameter);

        new T Get(string name);

        void AddInParam(string name, bool isNullable, DbType dbType);

        void AddOutParam(string name, bool isNullable, DbType dbType);

        void AddNullInParam(string name, DbType dbType);

        void Add(string name, bool isNullable, DbType dbType, ParameterDirection dir);
    }

    public sealed class DbParameterBag : IDbParameterBag
    {
        Dictionary<string, DbParameter> parameters = new Dictionary<string, DbParameter>();

        public DbParameterBag()
        { }

        public void Add(DbParameter parameter)
        {
            if (parameter == null)
                return;
            if (this.parameters.ContainsKey(parameter.ParameterName))
                return;
            this.parameters.Add(parameter.ParameterName, parameter);
        }

        public DbParameter Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            try
            {
                return this.parameters[name];
            }
            catch
            { }
            return null;
        }


        public IEnumerator<DbParameter> GetEnumerator()
        {
            return this.parameters.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.parameters.Values.GetEnumerator();
        }
    }
}
