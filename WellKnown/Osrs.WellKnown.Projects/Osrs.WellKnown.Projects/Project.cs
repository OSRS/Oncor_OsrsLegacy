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

using Osrs.Data;
using Osrs.Runtime;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Osrs.WellKnown.Projects
{
    public sealed class Project : INamedEntity<CompoundIdentity>, IDescribable, IEquatable<Project>
    {
        public CompoundIdentity Identity
        {
            get;
        }

        private CompoundIdentity principal;
        public CompoundIdentity PrincipalOrganization
        {
            get { return this.principal; }
            set
            {
                if (!value.IsNullOrEmpty())
                    this.principal = value;
            }
        }

        public ProjectAffiliates Affiliates
        {
            get;
        }

        public CompoundIdentity ParentId
        {
            get;
            set;
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

        public Project(CompoundIdentity id, string name, CompoundIdentity principalOrganizationId) : 
            this(id, name, principalOrganizationId, null, null)
        { }

        public Project(CompoundIdentity id, string name, CompoundIdentity principalOrganizationId, string description) : 
            this(id, name, principalOrganizationId, null, description)
        { }

        public Project(CompoundIdentity id, string name, CompoundIdentity principalOrganizationId, CompoundIdentity parentId, string description)
        {
            MethodContract.NotNullOrEmpty(id, nameof(id));
            MethodContract.NotNullOrEmpty(name, nameof(name));
            MethodContract.NotNullOrEmpty(principalOrganizationId, nameof(principalOrganizationId));
            this.Identity = id;
            this.name = name;
            this.principal = principalOrganizationId;
            this.Description = description;
            this.ParentId = parentId;
            this.Affiliates = new ProjectAffiliates(this.Identity);
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            return this.Equals(other as Project);
        }

        public bool Equals(Project other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }
    }

    public sealed class ProjectAffiliates : IEnumerable<CompoundIdentity>
    {
        private readonly CompoundIdentity projectId;
        private readonly HashSet<CompoundIdentity> members = new HashSet<CompoundIdentity>();

        public int Count
        {
            get { return this.members.Count; }
        }

        public bool Add(CompoundIdentity orgId)
        {
            if (!orgId.IsNullOrEmpty())
            {
                return this.members.Add(orgId);
            }
            return false;
        }

        public bool Remove(CompoundIdentity orgId)
        {
            if (!orgId.IsNullOrEmpty())
            {
                return this.members.Remove(orgId);
            }
            return false;
        }

        public bool Contains(CompoundIdentity orgId)
        {
            if (!orgId.IsNullOrEmpty())
                return this.members.Contains(orgId);
            return false;
        }

        internal ProjectAffiliates(CompoundIdentity projectId)
        {
            MethodContract.NotNullOrEmpty(projectId, nameof(projectId));
            this.projectId = projectId;
        }

        public IEnumerator<CompoundIdentity> GetEnumerator()
        {
            return this.members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.members.GetEnumerator();
        }
    }
}
