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
using System;
using System.Collections.Generic;

namespace Osrs.Numerics.Spatial.Operations
{
    internal sealed class PlanarPointGraph<T> : IEnumerable<Node<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        private static readonly NodeComparer<T> comparer = new NodeComparer<T>();
        //internal readonly LeakyResizableArray<Point2<T>> Points = new LeakyResizableArray<Point2<T>>();
        internal readonly LeakyResizableArray<Node<T>> Nodes = new LeakyResizableArray<Node<T>>();

        private bool isClosed = false;
        public bool IsClosed
        {
            get { return this.isClosed; }
        }

        public void Open()
        {
            this.isClosed = false;
        }

        internal void Add(Point2<T> point)
        {
            if (point == null)
                return;
            Node<T> node = new Node<T>(point, point);
            this.Nodes.Add(node);
            //this.Points.Add(point);
        }

        internal void Add(Point2<T> point, LeakyResizableArray<Node<T>> nodes)
        {
            if (point == null)
                return;
            Node<T> node = new Node<T>(point, point);
            this.Nodes.Add(node);
            nodes.Add(node);
            //this.Points.Add(point);
        }

        public void Close()
        {
            if (this.isClosed)
                return;
            this.isClosed = true;
            Array.Sort<Node<T>>(this.Nodes.Data, 0, (int)this.Nodes.Count, comparer);
        }

        public IEnumerator<Node<T>> GetEnumerator()
        {
            return new NodeFront<T>(this.Nodes);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new NodeFront<T>(this.Nodes);
        }
    }
}