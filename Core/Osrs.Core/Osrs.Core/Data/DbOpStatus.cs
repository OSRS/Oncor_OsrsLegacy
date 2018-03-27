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
using System;

namespace Osrs.Data
{
    public enum DbOpStatus
    {
        Success,
        FailedOpen,
        FailedClose,
        FailedExecute
    }

    public class DbOpResultStatus
    {
        private DbOpStatus status;
        private Exception e;
        private int count;
        private object data;

        public DbOpResultStatus()
        {
            this.status = DbOpStatus.Success;
        }
        public DbOpResultStatus(int count)
        {
            this.status = DbOpStatus.Success;
            this.count = count;
        }
        public DbOpResultStatus(object data)
        {
            this.status = DbOpStatus.Success;
            this.data = data;
        }
        public DbOpResultStatus(DbOpStatus status)
        {
            this.status = status;
        }
        public DbOpResultStatus(DbOpStatus status, int count)
        {
            this.status = status;
            this.count = count;
        }
        public DbOpResultStatus(DbOpStatus status, object data)
        {
            this.status = status;
            this.data = data;
        }
        public DbOpResultStatus(DbOpStatus status, int count, object data)
        {
            this.status = status;
            this.count = count;
            this.data = data;
        }

        public DbOpResultStatus(DbOpStatus status, Exception e)
        {
            this.status = status;
            this.e = e;
        }
        public DbOpResultStatus(DbOpStatus status, int count, Exception e)
        {
            this.status = status;
            this.count = count;
            this.e = e;
        }
        public DbOpResultStatus(DbOpStatus status, object data, Exception e)
        {
            this.status = status;
            this.data = data;
            this.e = e;
        }
        public DbOpResultStatus(DbOpStatus status, int count, object data, Exception e)
        {
            this.status = status;
            this.count = count;
            this.data = data;
            this.e = e;
        }

        public DbOpStatus Status
        {
            get { return this.status; }
        }

        public Exception E
        {
            get { return this.e; }
        }

        public int Count
        {
            get { return this.count; }
        }

        public object Data
        {
            get { return this.data; }
        }
    }
}
