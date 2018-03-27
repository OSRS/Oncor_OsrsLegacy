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

namespace Osrs
{
    /// <summary>
    /// An indexed entity is one that is identifiable by an index value - similar to a primary key.
    /// The index is read-only and index matching is the responsibility of the implementor.
    /// 
    /// The indexed differs from an identifiable in that the index is not the identity of the item, merely and "index" to get the item.
    /// An index may be transient in that it can change between executions of a program or on an entity, such as the position within a collection.
    /// </summary>
    /// <typeparam name="I"></typeparam>
    public interface IIndexedItem<I>
        where I : IEquatable<I>
    {
        I Index
        {
            get;
        }
    }

    public interface IOrderableIndexedItem<I> : IIndexedItem<I>
        where I : IEquatable<I>, IComparable<I>
    { }

    public interface IKeyedItem<K>
        where K : IEquatable<K>
    {
        K Key
        {
            get;
        }
    }

    public interface IOrderableKeyedItem<K> : IKeyedItem<K>
        where K : IEquatable<K>, IComparable<K>
    { }

    public interface IValue<V>
    {
        V Value
        {
            get;
        }
    }

    public interface IMutableValue<V> : IValue<V>
    {
        new V Value
        {
            get;
            set;
        }
    }
}
