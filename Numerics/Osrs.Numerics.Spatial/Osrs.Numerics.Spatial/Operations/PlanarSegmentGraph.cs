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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial.Operations
{
    internal sealed class PlanarSegmentGraph<T> : IEnumerable<Node<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        private static readonly NodeComparer<T> comparer = new NodeComparer<T>();
        //internal readonly LeakyResizableArray<Point2<T>> Points = new LeakyResizableArray<Point2<T>>();
        internal readonly LeakyResizableArray<Node<T>> Nodes = new LeakyResizableArray<Node<T>>();
        internal readonly LeakyResizableArray<Edge<T>> Edges = new LeakyResizableArray<Edge<T>>();

        private bool isClosed = false;
        public bool IsClosed
        {
            get { return this.isClosed; }
        }

        public void Open()
        {
            this.isClosed = false;
        }

        internal void Add(LineSegment2<T> segment)
        {
            if (segment == null)
                return;
            //this.Points.Add(segment.Start);
            //this.Points.Add(segment.End);
            Node<T> n = new Node<T>(segment.Start, segment);
            Node<T> p = new Node<T>(segment.End, segment);
            this.Nodes.Add(n);
            this.Nodes.Add(p);

            Edge<T> e = new Edge<T>(n, p);

            this.Edges.Add(e);
        }

        internal void Add(LineSegment2<T> segment, LeakyResizableArray<Node<T>> nodes)
        {
            if (segment == null)
                return;
            //this.Points.Add(segment.Start);
            //this.Points.Add(segment.End);
            Node<T> n = new Node<T>(segment.Start, segment);
            Node<T> p = new Node<T>(segment.End, segment);
            this.Nodes.Add(n);
            this.Nodes.Add(p);
            nodes.Add(n);
            nodes.Add(p);

            Edge<T> e = new Edge<T>(n, p);

            this.Edges.Add(e);
        }

        internal void Add(LineSegment2<T> segment, LeakyResizableArray<Node<T>> nodes, LeakyResizableArray<Edge<T>> edges)
        {
            if (segment == null)
                return;
            //this.Points.Add(segment.Start);
            //this.Points.Add(segment.End);
            Node<T> n = new Node<T>(segment.Start, segment);
            Node<T> p = new Node<T>(segment.End, segment);
            this.Nodes.Add(n);
            this.Nodes.Add(p);
            nodes.Add(n);
            nodes.Add(p);

            Edge<T> e = new Edge<T>(n, p);

            this.Edges.Add(e);
            edges.Add(e);
        }

        internal void Add(Point2<T> start, Point2<T> end)
        {
            if (start == null || end == null || start.Equals(end))
                return;
            LineSegment2<T> seg = start.Factory.ConstructSegment(start, end);
            Node<T> n = new Node<T>(start, seg);
            Node<T> p = new Node<T>(end, seg);
            this.Nodes.Add(n);
            this.Nodes.Add(p);

            Edge<T> e = new Edge<T>(n, p);

            this.Edges.Add(e);
        }

        internal void Add(Point2<T> start, Point2<T> end, LeakyResizableArray<Node<T>> nodes)
        {
            if (start == null || end == null || start.Equals(end))
                return;
            LineSegment2<T> seg = start.Factory.ConstructSegment(start, end);
            Node<T> n = new Node<T>(start, seg);
            Node<T> p = new Node<T>(end, seg);
            this.Nodes.Add(n);
            this.Nodes.Add(p);
            nodes.Add(n);
            nodes.Add(p);

            Edge<T> e = new Edge<T>(n, p);

            this.Edges.Add(e);
        }

        internal void Add(Point2<T> start, Point2<T> end, LeakyResizableArray<Node<T>> nodes, LeakyResizableArray<Edge<T>> edges)
        {
            if (start == null || end == null || start.Equals(end))
                return;
            LineSegment2<T> seg = start.Factory.ConstructSegment(start, end);
            Node<T> n = new Node<T>(start, seg);
            Node<T> p = new Node<T>(end, seg);
            this.Nodes.Add(n);
            this.Nodes.Add(p);
            nodes.Add(n);
            nodes.Add(p);

            Edge<T> e = new Edge<T>(n, p);

            this.Edges.Add(e);
            edges.Add(e);
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
