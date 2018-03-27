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

namespace Osrs.Collections
{
    public interface ITypedItem
    {
        string Name
        {
            get;
        }
        Type DataType
        {
            get;
        }
        object ObjectData
        {
            get;
        }
    }

    public sealed class MutableTypedItem<T> : TypedItem<T>
    {
        public new T Item
        {
            set
            {
                base.Item = value;
            }
        }

        private MutableTypedItem()
            : base(null, default(T))
        {
        }

        public MutableTypedItem(string name, T item)
            : base(name, item)
        {
        }
    }

    public class TypedItem<T> : ITypedItem
    {
        private T item;
        public T Item
        {
            get { return this.item; }
            protected set { this.item = value; }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
        }

        public Type DataType
        {
            get { return typeof(T); }
        }

        public object ObjectData
        {
            get { return this.item; }
        }

        private TypedItem()
        {
        }

        public TypedItem(string name, T item)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");
            if (null == item)
                throw new ArgumentNullException("item");
            this.item = item;
            this.name = name;
        }
    }
}
