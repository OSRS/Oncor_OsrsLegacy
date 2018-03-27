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
    public sealed class ChangeDetectedItem<T> where T : IEquatable<T>
    {
        private bool isDirty = false;
        public bool IsDirty
        {
            get { return this.isDirty; }
        }

        private T item;
        public T Item
        {
            get { return this.item; }
            set
            {
                if (this.item == null)
                {
                    if (value != null)
                    {
                        this.item = value;
                        this.isDirty = true;
                    }
                }
                else
                {
                    if (!this.item.Equals(value))
                    {
                        this.item = value;
                        this.isDirty = true;
                    }
                }

            }
        }

        public ChangeDetectedItem()
        {
            this.Reset();
        }

        public ChangeDetectedItem(T item)
        {
            this.Reset(item);
        }

        public void Reset()
        {
            this.isDirty = false;
            this.item = default(T);
        }

        public void Reset(T item)
        {
            this.isDirty = false;
            this.item = item;
        }
    }
}
