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

using Osrs.Collections.Specialized;
using System;
using System.Collections.Generic;

namespace Osrs.Numerics.Spatial.Operations
{
    internal sealed class NodeFront<T> : IEnumerator<Node<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        private readonly LeakyResizableArray<Node<T>> masterNodes;

        private long curId;
        private Node<T> current;
        public Node<T> Current
        {
            get { return this.current; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.current; }
        }

        internal NodeFront(LeakyResizableArray<Node<T>> masterNodes)
        {
            if (masterNodes == null)
                throw new ArgumentNullException();
            this.masterNodes = masterNodes;
            this.Reset();
        }

        public void Dispose()
        { }

        public bool MoveNext()
        {
            this.curId++;
            if (this.curId < (long)this.masterNodes.Count)
            {
                this.current = this.masterNodes.Data[this.curId];
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this.curId = -1;
        }
    }
}
