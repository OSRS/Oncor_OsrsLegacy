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
using System.Collections.Generic;
using System.Linq;

namespace Osrs.Collections.Specialized
{
    /// <summary>
    /// Warning, this class is similar to a List, but exposes the underlying array used for storage.
    /// Any reference taken on the inner array may be inconsistent with the LeakyResizableArray under several conditions:
    ///     The inner array is generally not "full" in that it allocates a larger array than is needed, so the actual x.Data.Length &lt= x.Count
    ///     When the inner array is "full", a call to Add() will result in a new array being created - any existing references to x.Data will be to the "old" array (and keep it's memory from being reclaimed)
    ///     When the inner array is not "full", a call to TrimExcess() will result in a new array being created - any existing references to x.Data will be to the "old" array (and keep it's memory from being reclaimed)
    ///     When a call to EnsureSize() is made, in a new array may be created - any existing references to x.Data will be to the "old" array (and keep it's memory from being reclaimed)
    /// Knowing these things, this class is quite useful to provide high-performance access to the underlying array when needed.
    /// This class should not be used in any circumstance other than special cases where this capability is needed.  You have been warned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class LeakyResizableArray<T>
    {
        private const int initSize = 25;
        public T[] Data;

        private int count;
        public ulong Count
        {
            get { return (ulong)this.count; }
        }

        public bool IsEmpty
        {
            get { return this.count == 0; }
        }

        public T this[ulong index]
        {
            get
            {
                if (index < (ulong)this.Data.Length)
                    return this.Data[(int)index];
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index < (ulong)this.Data.Length)
                    this.Data[(int)index] = value;
                throw new IndexOutOfRangeException();
            }
        }

        public LeakyResizableArray()
            : this(initSize)
        { }

        public LeakyResizableArray(T[] data)
        {
            if (data == null)
                throw new ArgumentNullException();
            this.Data = data;
            this.count = data.Length;
        }

        public LeakyResizableArray(int initialSize)
        {
            this.Data = new T[initialSize];
            this.count = 0;
        }

        public LeakyResizableArray(IList<T> data)
        {
            if (data == null)
                throw new ArgumentNullException();
            this.Data = data.ToArray();
            this.count = this.Data.Length;
        }

        public LeakyResizableArray(IEnumerable<T> data)
        {
            if (data == null)
                throw new ArgumentNullException();
            this.Data = data.ToArray();
            this.count = this.Data.Length;
        }

        private void Resize()
        {
            int newCount;
            if (this.count < 5000) //double in size up to 10,000, then increment by 2000
                newCount = this.Data.Length * 2;
            else
                newCount = this.Data.Length + 2000;

            T[] newData = new T[newCount];
            Array.Copy(this.Data, newData, (int)this.count);
            this.Data = newData;
        }

        public void EnsureSize(int addedItems)
        {
            int newCount = (int)this.count + addedItems;
            if (this.Data.Length < newCount)
            {
                T[] newData = new T[newCount];
                Array.Copy(this.Data, newData, (int)this.count);
                this.Data = newData;
            }
        }

        public void TrimExcess()
        {
            int ct = (int)this.count;
            if (this.Data.Length > ct)
            {
                T[] tmp = new T[this.count];
                Array.Copy(this.Data, tmp, ct);
                this.Data = tmp;
            }
        }

        public T[] ToArray()
        {
            int ct = (int)this.count;
            T[] tmp = new T[ct];
            Array.Copy(this.Data, tmp, ct);
            return tmp;
        }

        public void Add(T item)
        {
            if (this.count == this.Data.Length)
                this.Resize();
            this.Data[this.count] = item;
            this.count++;
        }

        public void Add(object item)
        {
            this.Add((T)item);
        }

        internal sealed class LeakyEnumerator
        {
            private T[] data;
            private int count;
            private int cur;

            private T current;
            public T Current
            {
                get { return this.current; }
            }

            internal LeakyEnumerator(T[] data, int count)
            {
                this.data = data;
                this.count = count;
                this.Reset();
            }

            public void Dispose()
            {
                this.data = null;
            }

            public bool MoveNext()
            {
                this.cur++;
                bool res = this.cur < this.count;
                if (res)
                    this.current = this.data[this.cur];
                return res;
            }

            public void Reset()
            {
                this.cur = -1;
            }

            public bool MovePrevious()
            {
                this.cur--;
                bool res = this.cur >= 0;
                if (res)
                    this.current = this.data[this.cur];
                else
                    this.cur = -1;
                return res;
            }
        }

        public void Truncate()
        {
            this.RemoveAt((ulong)(this.count - 1));
        }

        public void Remove()
        {
            this.RemoveAt(0);
        }

        public void RemoveAt(ulong index)
        {
            int ix = (int)index;
            if (ix > -1 && ix < this.count)
            {
                int max = this.count - 1;
                int cur;
                for (int i = ix; i < max; i++)
                {
                    cur = i + 1;
                    this.Data[i] = this.Data[cur];
                }
                this.count = max;
            }
        }

        public ulong IndexOf(T item)
        {
            return (ulong)Array.IndexOf(this.Data, item, 0, this.count);
        }

        public bool Contains(T item)
        {
            return this.Data.Contains(item);
        }

        public bool Contains(T item, out ulong index)
        {
            int ix = -1;
            if (!object.ReferenceEquals(null, item))
            {
                for (int i = 0; i < this.Data.Length; i++)
                {
                    if (item.Equals(this.Data[i]))
                    {
                        ix = i;
                        break;
                    }
                }
            }
            else //item is null
            {
                for (int i = 0; i < this.Data.Length; i++)
                {
                    if (this.Data[i] == null)
                    {
                        ix = i;
                        break;
                    }
                }
            }

            if (ix >= 0)
                index = (ulong)ix;
            else
                index = 0;
            return ix >= 0;
        }
    }
}
