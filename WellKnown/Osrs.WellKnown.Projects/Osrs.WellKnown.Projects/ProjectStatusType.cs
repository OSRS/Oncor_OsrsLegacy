﻿//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Osrs.Data;
using Osrs.Runtime;
using System;

namespace Osrs.WellKnown.Projects
{
    public sealed class ProjectStatusType : INamedEntity<CompoundIdentity>, IDescribable, IEquatable<ProjectStatusType>
    {
        public CompoundIdentity Identity
        {
            get;
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    this.name = value;
            }
        }

        string INamed.Name
        {
            get
            {
                return this.name;
            }
        }

        public string Description
        {
            get;
            set;
        }

        public ProjectStatusType(CompoundIdentity id, string name) : this(id, name, null)
        { }

        public ProjectStatusType(CompoundIdentity id, string name, string description)
        {
            MethodContract.NotNullOrEmpty(id, nameof(id));
            MethodContract.NotNullOrEmpty(name, nameof(name));
            this.Identity = id;
            this.name = name;
            this.Description = description;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            return this.Equals(other as ProjectStatusType);
        }

        public bool Equals(ProjectStatusType other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }
    }
}
