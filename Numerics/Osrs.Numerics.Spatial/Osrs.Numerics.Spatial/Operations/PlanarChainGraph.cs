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
    internal sealed class PlanarChainGraph<T> : IEnumerable<Node<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        private static readonly NodeComparer<T> comparer = new NodeComparer<T>();
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

        public void Add(LineChain2<T> chain)
        {
            if (chain == null)
                return;
            this.AddImpl(chain, chain.Points, false);
        }

        public void Add(LineChain2<T> chain, LeakyResizableArray<Node<T>> nodes)
        {
            if (chain == null)
                return;
            this.AddImpl(chain, chain.Points, false, nodes);
        }

        public void Add(LineChain2<T> chain, LeakyResizableArray<Node<T>> nodes, LeakyResizableArray<Edge<T>> edges)
        {
            if (chain == null)
                return;
            this.AddImpl(chain, chain.Points, false, nodes, edges);
        }

        public void Add(Ring2<T> ring)
        {
            if (ring == null)
                return;
            this.AddImpl(ring, ring.Points, true);
        }

        public void Add(Ring2<T> ring, LeakyResizableArray<Node<T>> nodes)
        {
            if (ring == null)
                return;
            this.AddImpl(ring, ring.Points, true, nodes);
        }

        public void Add(Ring2<T> ring, LeakyResizableArray<Node<T>> nodes, LeakyResizableArray<Edge<T>> edges)
        {
            if (ring == null)
                return;
            this.AddImpl(ring, ring.Points, true, nodes, edges);
        }

        public void Add(Point2<T>[] chain, bool isRing)
        {
            if (chain == null || chain.Length < 3)
                return;
            List<Point2<T>> chainPoints = RemoveDups(chain, isRing);
            if (chainPoints.Count < 3)
                return;
            this.AddImpl(chainPoints, chainPoints, isRing);
        }

        public void Add(Point2<T>[] chain, bool isRing, LeakyResizableArray<Node<T>> nodes)
        {
            if (chain == null || chain.Length < 3)
                return;
            List<Point2<T>> chainPoints = RemoveDups(chain, isRing);
            if (chainPoints.Count < 3)
                return;
            this.AddImpl(chainPoints, chainPoints, isRing, nodes);
        }

        public void Add(Point2<T>[] chain, bool isRing, LeakyResizableArray<Node<T>> nodes, LeakyResizableArray<Edge<T>> edges)
        {
            if (chain == null || chain.Length < 3)
                return;
            List<Point2<T>> chainPoints = RemoveDups(chain, isRing);
            if (chainPoints.Count < 3)
                return;
            this.AddImpl(chainPoints, chainPoints, isRing, nodes, edges);
        }

        public void Add(IList<Point2<T>> chain, bool isRing)
        {
            if (chain == null || chain.Count < 3)
                return;
            List<Point2<T>> chainPoints = RemoveDups(chain, isRing);
            if (chainPoints.Count < 3)
                return;
            this.AddImpl(chainPoints, chainPoints, isRing);
        }

        public void Add(IList<Point2<T>> chain, bool isRing, LeakyResizableArray<Node<T>> nodes)
        {
            if (chain == null || chain.Count < 3)
                return;
            List<Point2<T>> chainPoints = RemoveDups(chain, isRing);
            if (chainPoints.Count < 3)
                return;
            this.AddImpl(chainPoints, chainPoints, isRing, nodes);
        }

        public void Add(IList<Point2<T>> chain, bool isRing, LeakyResizableArray<Node<T>> nodes, LeakyResizableArray<Edge<T>> edges)
        {
            if (chain == null || chain.Count < 3)
                return;
            List<Point2<T>> chainPoints = RemoveDups(chain, isRing);
            if (chainPoints.Count < 3)
                return;
            this.AddImpl(chainPoints, chainPoints, isRing, nodes, edges);
        }

        public void Add(IEnumerable<Point2<T>> chain, bool isRing)
        {
            if (chain == null)
                return;
            List<Point2<T>> chainPoints = RemoveDups(chain, isRing);
            if (chainPoints.Count < 3)
                return;
            this.AddImpl(chainPoints, chainPoints, isRing);
        }

        public void Add(IEnumerable<Point2<T>> chain, bool isRing, LeakyResizableArray<Node<T>> nodes)
        {
            if (chain == null)
                return;
            List<Point2<T>> chainPoints = RemoveDups(chain, isRing);
            if (chainPoints.Count < 3)
                return;
            this.AddImpl(chainPoints, chainPoints, isRing, nodes);
        }

        public void Add(IEnumerable<Point2<T>> chain, bool isRing, LeakyResizableArray<Node<T>> nodes, LeakyResizableArray<Edge<T>> edges)
        {
            if (chain == null)
                return;
            List<Point2<T>> chainPoints = RemoveDups(chain, isRing);
            if (chainPoints.Count < 3)
                return;
            this.AddImpl(chainPoints, chainPoints, isRing, nodes, edges);
        }

        private void AddImpl(object parentShape, IEnumerable<Point2<T>> chain, bool isRing)
        {
            int firstIndex = (int)this.Nodes.Count;
            int firstEdge = (int)this.Edges.Count;
            Node<T> n = null;
            Node<T> p = null;
            Edge<T> e = null;
            Edge<T> prev = null;
            foreach (Point2<T> pt in chain)
            {
                p = n;
                n = new Node<T>(pt, parentShape);
                if (p != null)
                {
                    prev = e;
                    e = new Edge<T>(p, n);
                    if (prev != null)
                    {
                        e.Previous = prev;
                        prev.Next = e;
                    }
                    this.Edges.Add(e);
                }
                this.Nodes.Add(n);
            }
            if (isRing) //wraparound
            {
                p = n; //last node added above
                n = this.Nodes.Data[firstIndex];
                prev = e; //last edge added above
                e = new Edge<T>(p, n);

                e.Previous = prev;
                prev.Next = e;

                //connect the wrap
                prev = e;
                e = this.Edges.Data[firstEdge];
                e.Previous = prev;
                prev.Next = e;

                this.Edges.Add(e);
            }
        }

        private void AddImpl(object parentShape, IEnumerable<Point2<T>> chain, bool isRing, LeakyResizableArray<Node<T>> nodes)
        {
            int firstIndex = (int)this.Nodes.Count;
            int firstEdge = (int)this.Edges.Count;
            Node<T> n = null;
            Node<T> p = null;
            Edge<T> e = null;
            Edge<T> prev = null;
            foreach (Point2<T> pt in chain)
            {
                p = n;
                n = new Node<T>(pt, parentShape);
                if (p != null)
                {
                    prev = e;
                    e = new Edge<T>(p, n);
                    if (prev != null)
                    {
                        e.Previous = prev;
                        prev.Next = e;
                    }
                    this.Edges.Add(e);
                }
                this.Nodes.Add(n);
                nodes.Add(n);
            }
            if (isRing) //wraparound
            {
                p = n; //last node added above
                n = this.Nodes.Data[firstIndex];
                prev = e; //last edge added above
                e = new Edge<T>(p, n);

                e.Previous = prev;
                prev.Next = e;

                //connect the wrap
                prev = e;
                e = this.Edges.Data[firstEdge];
                e.Previous = prev;
                prev.Next = e;

                this.Edges.Add(e);
            }
        }

        private void AddImpl(object parentShape, IEnumerable<Point2<T>> chain, bool isRing, LeakyResizableArray<Node<T>> nodes, LeakyResizableArray<Edge<T>> edges)
        {
            int firstIndex = (int)this.Nodes.Count;
            int firstEdge = (int)this.Edges.Count;
            Node<T> n = null;
            Node<T> p = null;
            Edge<T> e = null;
            Edge<T> prev = null;
            foreach (Point2<T> pt in chain)
            {
                p = n;
                n = new Node<T>(pt, parentShape);
                if (p != null)
                {
                    prev = e;
                    e = new Edge<T>(p, n);
                    if (prev != null)
                    {
                        e.Previous = prev;
                        prev.Next = e;
                    }
                    this.Edges.Add(e);
                    edges.Add(e);
                }
                this.Nodes.Add(n);
                nodes.Add(n);
            }
            if (isRing) //wraparound
            {
                p = n; //last node added above
                n = this.Nodes.Data[firstIndex];
                prev = e; //last edge added above
                e = new Edge<T>(p, n);

                e.Previous = prev;
                prev.Next = e;

                //connect the wrap
                prev = e;
                e = this.Edges.Data[firstEdge];
                e.Previous = prev;
                prev.Next = e;

                this.Edges.Add(e);
                edges.Add(e);
            }
        }

        //removes duplicated points in a chain of points - only adjacent duplicates are removed
        private List<Point2<T>> RemoveDups(IEnumerable<Point2<T>> chain, bool isRing)
        {
            Point2<T> prev = null;
            List<Point2<T>> tmp = new List<Point2<T>>();
            foreach (Point2<T> curPt in chain)
            {
                if (curPt == null)
                    continue;
                if (prev != null && curPt.Equals(prev))
                    continue; //dup
                prev = curPt;
                tmp.Add(curPt);
            }
            if (isRing)
            {
                if (tmp[0].Equals(tmp[tmp.Count - 1])) //start and end are dup on a ring
                    tmp.RemoveAt(tmp.Count - 1); //get rid of the last item
            }
            return tmp;
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
            this.Close();
            return new NodeFront<T>(this.Nodes);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new NodeFront<T>(this.Nodes);
        }
    }
}
