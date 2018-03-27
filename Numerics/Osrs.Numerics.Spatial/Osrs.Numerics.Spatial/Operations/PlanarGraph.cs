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
    internal enum GraphType
    {
        Point,
        Segment,
        Chain,
        Ring
    }

    internal sealed class PlanarGraph<T> : IEnumerable<Node<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        private static readonly NodeComparer<T> comparer = new NodeComparer<T>();
        //private readonly LeakyResizableArray<Point2<T>> masterPoints = new LeakyResizableArray<Point2<T>>();
        private readonly LeakyResizableArray<Node<T>> masterNodes = new LeakyResizableArray<Node<T>>();
        private readonly PlanarPointGraph<T> loosePoints;
        private readonly PlanarSegmentGraph<T> lineSegments;
        private readonly LeakyResizableArray<PlanarChainGraph<T>> chains = new LeakyResizableArray<PlanarChainGraph<T>>(4);
        private readonly LeakyResizableArray<PlanarChainGraph<T>> rings = new LeakyResizableArray<PlanarChainGraph<T>>(4);
        private readonly LeakyResizableArray<PlanarChainGraph<T>> polygons = new LeakyResizableArray<PlanarChainGraph<T>>(4);

        private bool isClosed = false;
        public bool IsClosed
        {
            get { return this.isClosed; }
        }

        public void Open()
        {
            this.isClosed = false;
        }

        public void Close()
        {
            if (this.isClosed)
                return;
            this.isClosed = true;
            Array.Sort<Node<T>>(this.masterNodes.Data, 0, (int)this.masterNodes.Count, comparer);
        }

        internal void AddPoint(Point2<T> point)
        {
            if (point == null)
                return;
            this.loosePoints.Add(point, this.masterNodes);
        }

        internal void AddSegment(LineSegment2<T> segment)
        {
            if (segment == null)
                return;
            this.lineSegments.Add(segment, this.masterNodes);
        }

        internal void AddSegment(Point2<T> start, Point2<T> end)
        {
            this.lineSegments.Add(start, end, this.masterNodes);
        }

        internal void AddChain(IList<Point2<T>> chain)
        {
            PlanarChainGraph<T> g = new PlanarChainGraph<T>();
            g.Add(chain, false, this.masterNodes);
            this.chains.Add(g);
        }

        internal void AddRing(IList<Point2<T>> chain)
        {
            PlanarChainGraph<T> g = new PlanarChainGraph<T>();
            g.Add(chain, true, this.masterNodes);
            this.rings.Add(g);
        }

        internal void AddPolygon(IList<Point2<T>> outerRing, IList<IList<Point2<T>>> innerRings)
        {
            if (outerRing == null || outerRing.Count < 3)
                return;
            PlanarChainGraph<T> g = new PlanarChainGraph<T>();
            g.Add(outerRing, true, this.masterNodes);

            foreach (IList<Point2<T>> ring in innerRings)
            {
                g.Add(ring, true);
            }

            this.polygons.Add(g);
        }

        public IEnumerator<Node<T>> GetEnumerator()
        {
            return new NodeFront<T>(this.masterNodes);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new NodeFront<T>(this.masterNodes);
        }

        internal PlanarGraph()
        {
            this.loosePoints = new PlanarPointGraph<T>();
            this.lineSegments = new PlanarSegmentGraph<T>();
        }
    }
}
