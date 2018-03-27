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
using Osrs.WellKnown.Organizations;
using System.Collections.Generic;

namespace Osrs.WellKnown.OrganizationHierarchies
{
    public abstract class OrganizationHierarchy
    {
        public CompoundIdentity Identity
        {
            get;
            private set;
        }

        private CompoundIdentity owningOrgId;
        public CompoundIdentity OwningOrgId
        {
            get { return this.owningOrgId; }
            protected set //has to allow for empty, but not null
            {
                if (value != null)
                    this.owningOrgId = value;
            }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            protected set
            {
                if (!string.IsNullOrEmpty(value))
                    this.name = value;
            }
        }

        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// This method updates the name/description back to persistent storage if changed in the instance.
        /// This method does NOT update the membership information - that happens in the hierarchy itself.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <returns></returns>
        public abstract bool UpdateInfo();

        public abstract bool CanUpdateInfo();
        public abstract bool CanGet();
        public abstract bool CanAdd();
        public abstract bool CanAdd(CompoundIdentity parentOrgId, CompoundIdentity childOrgId);
        public abstract bool CanMove();
        public abstract bool CanMove(CompoundIdentity orgId);
        public abstract bool CanRemove();
        public abstract bool CanRemove(CompoundIdentity parentOrgId, CompoundIdentity childOrgId);

        public IEnumerable<Organization> GetChildren(Organization org)
        {
            return GetChildren(org, 0);
        }
        public IEnumerable<Organization> GetChildren(Organization org, bool recurse)
        {
            if (recurse)
                return GetChildren(org, uint.MaxValue);
            return GetChildren(org, 0);
        }
        public abstract IEnumerable<Organization> GetChildren(Organization org, uint maxLevels);

        public IEnumerable<CompoundIdentity> GetChildrenIds(CompoundIdentity orgId)
        {
            return GetChildrenIds(orgId, 0);
        }
        public IEnumerable<CompoundIdentity> GetChildrenIds(CompoundIdentity orgId, bool recurse)
        {
            if (recurse)
                return GetChildrenIds(orgId, uint.MaxValue);
            return GetChildrenIds(orgId, 0);
        }
        public abstract IEnumerable<CompoundIdentity> GetChildrenIds(CompoundIdentity orgId, uint maxLevels);

        public abstract IEnumerable<CompoundIdentity> GetParentIds(CompoundIdentity orgId);

        public abstract IEnumerable<KeyValuePair<CompoundIdentity, CompoundIdentity>> GetAllPairs();

        public bool Add(Organization parent, Organization child)
        {
            if (parent != null && child != null)
                return Add(parent.Identity, child.Identity);
            return false;
        }
        public abstract bool Add(CompoundIdentity parentId, CompoundIdentity childId);
        public abstract bool Add(Organization parent, IEnumerable<Organization> child);
        public abstract bool Add(CompoundIdentity parentId, IEnumerable<CompoundIdentity> childId);

        public bool Move(Organization oldParent, Organization newParent, Organization child)
        {
            if (oldParent != null && child != null)
            {
                if (newParent != null)
                    return Move(oldParent.Identity, newParent.Identity, child.Identity);
                return Move(oldParent.Identity, null, child.Identity);
            }
            return false;
        }
        public abstract bool Move(CompoundIdentity oldParentId, CompoundIdentity newParentId, CompoundIdentity childId);
        public abstract bool Move(Organization oldParent, Organization newParent, IEnumerable<Organization> child);
        public abstract bool Move(CompoundIdentity oldParentId, CompoundIdentity newParentId, IEnumerable<CompoundIdentity> childId);

        public bool Remove(Organization parent, Organization child)
        {
            if (parent != null && child != null)
                return Remove(parent.Identity, child.Identity);
            return false;
        }
        public abstract bool Remove(CompoundIdentity parentId, CompoundIdentity childId);
        public abstract bool Remove(Organization parent, IEnumerable<Organization> child);
        public abstract bool Remove(CompoundIdentity parentId, IEnumerable<CompoundIdentity> childId);

        protected OrganizationHierarchy(CompoundIdentity identity, CompoundIdentity owningOrgId, string name, string description)
        {
            MethodContract.NotNullOrEmpty(identity, nameof(identity));
            MethodContract.NotNullOrEmpty(owningOrgId, nameof(owningOrgId));
            MethodContract.NotNullOrEmpty(name, nameof(name));
            this.Identity = identity;
            this.owningOrgId = owningOrgId;
            this.name = name;
            this.Description = description;
        }
    }

    public interface IOrganizationHierarchyProvider
    {
        bool CanGet();
        bool CanDelete();
        bool CanDelete(CompoundIdentity hierarchyId);
        bool CanCreate();

        bool Exists(string hierarchyName);
        bool Exists(CompoundIdentity hierarchyId);
        /// <summary>
        /// There is the single "reporting hierarchy" which cannot be deleted which represents the "official" organizational hierarchy for an organization.
        /// </summary>
        /// <returns></returns>
        OrganizationHierarchy GetReporting();
        OrganizationHierarchy Get(string hierarchyName);
        OrganizationHierarchy Get(CompoundIdentity hierarchyId);

        OrganizationHierarchy Create(string hierarchyName, Organization owningOrg);
        bool Delete(OrganizationHierarchy hierarchy);
        bool Delete(CompoundIdentity hierarchyId);
    }
}
