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
using Osrs.Runtime;
using System;

namespace Osrs.Data
{
    public interface IIdentity : IEquatable<IIdentity>
    { }

    public interface IIdentity<T> : IIdentity, IEquatable<IIdentity<T>>
        where T : IEquatable<T>
    {
        T Identity
        {
            get;
        }
    }

    public interface IIdentifiableEntity<PKType> : IEquatable<IIdentifiableEntity<PKType>> where PKType : IEquatable<PKType>
    {
        PKType Identity
        {
            get;
        }
    }

    public interface IIdentifiableFactory<T, V>
        where T : IIdentifiableEntity<V>
        where V : IEquatable<V>
    {
        T Create(V identity);
    }

    public interface INamed
    {
        string Name
        {
            get;
        }
    }

    public interface INamedEntity<PKType> : INamed, IIdentifiableEntity<PKType> where PKType : IEquatable<PKType>
    {
        new string Name
        {
            get;
            set;
        }
    }

    public interface IDescribable
    {
        string Description
        {
            get;
            set;
        }
    }
}
