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
using Osrs.Numerics.Spatial.Geometry;
using Osrs.Runtime;
using System;

namespace Osrs.Numerics.Spatial.Operations
{
    internal sealed class Node<T> : IComparable, IComparable<Node<T>>, IEquatable<Node<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly Point2<T> Point;
        public readonly LeakyResizableArray<Edge<T>> Edges = new LeakyResizableArray<Edge<T>>(2);
        public readonly object ParentShape;

        public Node(Point2<T> pt, object parentShape)
        {
            this.Point = pt;
            this.ParentShape = parentShape;
        }

        public void Attach(Edge<T> e)
        {
            this.Edges.Add(e);
        }

        public int CompareTo(Node<T> other)
        {
            return this.Point.CompareTo(other.Point);
        }

        public int CompareTo(object obj)
        {
            if (obj is Node<T>)
                return this.CompareTo(obj as Node<T>);
            throw new TypeMismatchException();
        }

        public bool Equals(Node<T> other)
        {
            if (object.ReferenceEquals(null, other))
                return false;
            return object.ReferenceEquals(this, other);
        }
    }
}
