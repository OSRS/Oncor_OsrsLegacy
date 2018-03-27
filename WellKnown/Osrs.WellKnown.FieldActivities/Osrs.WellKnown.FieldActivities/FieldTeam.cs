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

namespace Osrs.WellKnown.FieldActivities
{
    public sealed class FieldTeam : INamedEntity<CompoundIdentity>, IEquatable<FieldTeam>
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
                if (!string.IsNullOrEmpty(name))
                    this.name = value;
            }
        }

        string INamed.Name
        {
            get
            {
                return this.Name;
            }
        }

        public string Description
        {
            get;
            set;
        }

        public FieldTeamMembers Members
        {
            get;
        }

        public FieldTeam(CompoundIdentity id, string name):this(id, name, null)
        { }

        public FieldTeam(CompoundIdentity id, string name, string description)
        {
            MethodContract.NotNull(id, nameof(id));
            MethodContract.NotNullOrEmpty(name, nameof(name));

            this.Identity = id;
            this.name = name;
            this.Description = description;
            this.Members = new FieldTeamMembers();
        }

        public bool Equals(FieldTeam other)
        {
            if (other != null)
                return this.Identity.Equals(other.Identity);
            return false;
        }

        public bool Equals(IIdentifiableEntity<CompoundIdentity> other)
        {
            return this.Equals(other as FieldTeam);
        }
    }

    public sealed class FieldTeamMember : IEquatable<FieldTeamMember>
    {
        public CompoundIdentity PersonId
        {
            get;
        }

        private CompoundIdentity fieldTeamMemberRoleId;
        public CompoundIdentity FieldTeamMemberRoleId
        {
            get { return fieldTeamMemberRoleId; }
            set
            {
                if (!value.IsNullOrEmpty())
                    this.fieldTeamMemberRoleId = value;
            }
        }

        public FieldTeamMember(CompoundIdentity personId, CompoundIdentity fieldTeamMemberRoleId)
        {
            MethodContract.NotNull(personId, nameof(personId));
            MethodContract.NotNull(fieldTeamMemberRoleId, nameof(fieldTeamMemberRoleId));

            this.PersonId = personId;
            this.fieldTeamMemberRoleId = fieldTeamMemberRoleId;
        }

        public bool Equals(FieldTeamMember other)
        {
            if (other != null)
                return this.PersonId.Equals(other.PersonId);
            return false;
        }

        public bool Equals(CompoundIdentity personId)
        {
            if (!personId.IsNullOrEmpty())
                return this.PersonId.Equals(personId);
            return false;
        }
    }

    public sealed class FieldTeamMembers : IEnumerable<FieldTeamMember>
    {
        private List<FieldTeamMember> members = new List<FieldTeamMember>();

        public int Count
        {
            get { return this.members.Count; }
        }

        public bool Add(FieldTeamMember member)
        {
            if (member!=null)
            {
                if (!this.Contains(member))
                {
                    this.members.Add(member);
                    return true;
                }
            }
            return false;
        }

        public bool Remove(FieldTeamMember member)
        {
            if (member!=null)
            {
                for(int i=0;i<this.members.Count;i++)
                {
                    if (this.members[i].Equals(member))
                    {
                        this.members.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Remove(CompoundIdentity personId)
        {
            if (!personId.IsNullOrEmpty())
            {
                for (int i = 0; i < this.members.Count; i++)
                {
                    if (this.members[i].Equals(personId))
                    {
                        this.members.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Contains(FieldTeamMember member)
        {
            if (member != null)
            {
                foreach (FieldTeamMember cur in this.members)
                {
                    if (member.Equals(cur))
                        return true;
                }
            }
            return false;
        }

        public bool Contains(CompoundIdentity personId)
        {
            if (!personId.IsNullOrEmpty())
            {
                foreach (FieldTeamMember cur in this.members)
                {
                    if (cur.Equals(personId))
                        return true;
                }
            }
            return false;
        }

        public IEnumerator<FieldTeamMember> GetEnumerator()
        {
            return this.members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.members.GetEnumerator();
        }
    }
}
