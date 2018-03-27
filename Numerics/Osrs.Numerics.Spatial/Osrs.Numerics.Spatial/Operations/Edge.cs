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

namespace Osrs.Numerics.Spatial.Operations
{
    internal sealed class NodeComparer<T> : IComparer<Node<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        internal NodeComparer()
        {
        }

        public int Compare(Node<T> x, Node<T> y)
        {
            return x.Point.CompareTo(y.Point);
        }
    }

    internal sealed class Edge<T> : IEquatable<Edge<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly Node<T> Start;
        public readonly Node<T> End;
        public Edge<T> Next;
        public Edge<T> Previous;

        public Edge(Node<T> start, Node<T> end)
        {
            this.Start = start;
            this.End = end;
            start.Attach(this);
            end.Attach(this);
        }

        public bool Equals(Edge<T> other)
        {
            if (object.ReferenceEquals(null, other))
                return false;
            return object.ReferenceEquals(this, other);
        }
    }
}
