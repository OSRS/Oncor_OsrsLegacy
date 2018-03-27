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
using System;
using System.Collections.Generic;

namespace Osrs.WellKnown.Sites
{
    public interface ISiteProvider
    {
        bool CanGet();
        IEnumerable<Site> Get();
        IEnumerable<Site> Get(string name);
        IEnumerable<Site> Get(string name, StringComparison comparisonOption);

        Site Get(CompoundIdentity id);
        IEnumerable<Site> GetByOwner(CompoundIdentity owningOrgId);

        bool HasParents(Site site);
        bool HasChildren(Site site);
        IEnumerable<Site> GetParents(Site site);
        IEnumerable<Site> GetParents(CompoundIdentity site);
        IEnumerable<Site> GetChildren(Site parentSite);
        IEnumerable<Site> GetChildren(CompoundIdentity parentSite);
        bool RemoveParent(Site child, Site parent);
        bool AddParent(Site child, Site parent);
        bool RemoveParent(CompoundIdentity child, CompoundIdentity parent);
        bool AddParent(CompoundIdentity child, CompoundIdentity parent);
        bool AddParent(IEnumerable<Site> children, Site parent);
        bool RemoveParent(IEnumerable<Site> children, Site parent);
        bool AddParent(IEnumerable<CompoundIdentity> children, CompoundIdentity parent);
        bool RemoveParent(IEnumerable<CompoundIdentity> children, CompoundIdentity parent);

        bool Exists(CompoundIdentity id);
        bool Exists(string name);
        bool Exists(string name, StringComparison comparisonOption);

        bool CanUpdate();
        bool CanUpdate(Site site);
        bool Update(Site site);

        bool CanDelete();
        bool CanDelete(Site site);
        bool Delete(Site site);
        bool Delete(CompoundIdentity id);

        bool CanCreate();
        Site Create(CompoundIdentity owningOrgId, string name);
        Site Create(CompoundIdentity owningOrgId, string name, Site parent);
        Site Create(CompoundIdentity owningOrgId, string name, string description);
        Site Create(CompoundIdentity owningOrgId, string name, string description, Site parent);
    }
}
