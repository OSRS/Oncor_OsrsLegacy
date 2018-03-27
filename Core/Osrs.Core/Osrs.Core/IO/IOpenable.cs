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
using System.Collections;
using System.Collections.Generic;

namespace Osrs.IO
{
    public interface IOpenable : IDisposable
    {
        bool IsOpen
        {
            get;
        }

        void Open();
        void Close();
    }

    public interface IBasicIO<TIndex, TData>
    {
        TData Read(TIndex id);
        bool Write(TIndex id, TData data);
        bool Exists(TIndex id);
    }

    public interface IFlushable : IDisposable
    {
        bool Flush();
    }

    public interface IFlushableIO<TIndex, TData> : IBasicIO<TIndex, TData>, IFlushable
    {
    }

    public interface IFileIO<TIndex, TData> : IBasicIO<TIndex, TData>, IFlushableIO<TIndex, TData>
    {
        void Close();
    }

    public interface IIndexMapper<TIn, TOut>
    {
        TOut AddressTo(TIn index);
        TIn AddressFrom(TOut index);
    }

    public interface IKeyValuePairIO<TKey, TValue> : IBasicIO<TKey, KeyValuePair<TKey, TValue>>
    {
        long Count
        {
            get;
        }
    }

    public interface IEnumerableIO : IEnumerable, IDisposable
    {
    }

    public interface IEnumerableIO<T> : IEnumerable<T>, IEnumerableIO, IDisposable
    {
    }

    public interface IEnumeratorIO : IEnumerator, IDisposable
    {
    }

    public interface IEnumeratorIO<T> : IEnumeratorIO, IEnumerator<T>, IDisposable
    {
    }

    public interface ICollectionIO : IEnumerableIO, ICollection, IDisposable
    {
        long CountLong
        {
            get;
        }
    }

    public interface ICollectionIO<T> : ICollectionIO, IEnumerableIO<T>, IDisposable
    {
    }

    public interface IListIO<T> : IList<T>, ICollectionIO<T>, IEnumerableIO<T>, IEnumerableIO, IDisposable
    {
    }

    public interface IDictionaryIO : IDictionary, IDisposable
    {
    }

    public interface IDictionaryIO<TKey, TValue> : ICollectionIO<KeyValuePair<TKey, TValue>>, IEnumerableIO<KeyValuePair<TKey, TValue>>, IDisposable
    {
        TValue this[TKey index]
        {
            get;
            set;
        }

        bool ContainsKey(TKey index);

        bool Add(TKey index, TValue data);

        bool Remove(TKey index);
    }

    public interface IIOProvider
    {
        object Read(long index);
        bool Write(long index, object item);
        bool Exists(long index);
        long NewIndex();
    }

    public interface IIOProvider<T> : IIOProvider
    {
        new T Read(long index);
        bool Write(long index, T item);
    }
}
