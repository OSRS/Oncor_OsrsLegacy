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
    public abstract class AbstractNamedEntity : INamedEntity<Guid>
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (!string.IsNullOrEmpty(name))
                    this.name = value;
            }
        }

        private Guid id;
        public Guid Identity
        {
            get { return this.id; }
            private set
            {
                if (!Guid.Empty.Equals(value))
                    this.id = value;
            }
        }

        public string Description
        {
            get;
            set;
        }

        private AbstractNamedEntity()
        { }

        protected AbstractNamedEntity(Guid id)
        {
            this.id = id;
        }

        protected AbstractNamedEntity(Guid id, string name)
        {
            this.id = id;
            if (!string.IsNullOrEmpty(name))
                this.name = name;
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        public bool Equals(IIdentifiableEntity<Guid> other)
        {
            if (other == null)
                return false;
            return this.id.Equals(other.Identity);
        }
    }
}
