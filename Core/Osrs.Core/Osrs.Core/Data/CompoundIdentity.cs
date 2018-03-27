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
    /// <summary>
    /// Compound identities are identitfiers for an entity that additionally contain an identifier for the system which "owns" the record.
    /// Any entities that may need to be shared across systems should use an <code><![CDATA[ICompoundIdentity<S,K>]]></code> for its ID.
    /// </summary>
    /// <typeparam name="S">The type of the system identifier (usually Guid)</typeparam>
    /// <typeparam name="K">The type of the entity identifier (usually Guid or long)</typeparam>
    public interface ICompoundIdentity<S, K> : IIdentity<K>, IEquatable<ICompoundIdentity<S, K>>, IEmpty
        where S : IEquatable<S>
        where K : IEquatable<K>
    {
        S DataStoreIdentity
        {
            get;
        }
    }

    /// <summary>
    /// Compound identities are identitfiers for an entity that additionally contain an identifier for the system which "owns" the record.
    /// Any entities that may need to be shared across systems should use an <code><![CDATA[ICompoundIdentity<S,K>]]></code> for its ID.
    /// </summary>
    public sealed class CompoundIdentity : ICompoundIdentity<Guid, Guid>, IEquatable<CompoundIdentity>
    {
        private readonly Guid dataStoreIdentity;
        public Guid DataStoreIdentity
        {
            get { return this.dataStoreIdentity; }
        }

        private readonly Guid identity;
        public Guid Identity
        {
            get { return this.identity; }
        }

        public bool IsEmpty
        {
            get { return Guid.Empty.Equals(this.dataStoreIdentity) || Guid.Empty.Equals(this.identity); }
        }

        public CompoundIdentity(Guid systemIdentity, Guid identity)
        {
            this.dataStoreIdentity = systemIdentity;
            this.identity = identity;
        }

        public bool Equals(IIdentity other)
        {
            return this.Equals(other as CompoundIdentity);
        }

        public bool Equals(IIdentity<Guid> other)
        {
            return this.Equals(other as CompoundIdentity);
        }

        public bool Equals(ICompoundIdentity<Guid, Guid> other)
        {
            return this.Equals(other as CompoundIdentity);
        }

        public bool Equals(CompoundIdentity other)
        {
            if (object.ReferenceEquals(other, null))
                return false;
            return this.dataStoreIdentity.Equals(other.dataStoreIdentity) && this.identity.Equals(other.identity);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null))
                return false;
            return this.Equals(obj as CompoundIdentity);
        }

        public override int GetHashCode()
        {
            return this.identity.GetHashCode();
        }

        public static bool operator == (CompoundIdentity a, CompoundIdentity b)
        {
            if (object.ReferenceEquals(a, null))
                return object.ReferenceEquals(b, null);
            return a.Equals(b);
        }

        public static bool operator != (CompoundIdentity a, CompoundIdentity b)
        {
            return !(a == b);
        }
    }
}
