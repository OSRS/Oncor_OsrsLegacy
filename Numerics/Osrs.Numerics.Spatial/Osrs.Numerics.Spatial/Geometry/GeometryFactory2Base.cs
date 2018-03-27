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
using Osrs.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Osrs.Numerics.Spatial.Geometry
{
    internal static class GeometryFactory2Manager
    {
        internal static Dictionary<Type, object> items = new Dictionary<Type, object>();

        public static GeometryFactory2Base<T> Factory<T>() where T : IEquatable<T>, IComparable<T>
        {
            Type t = typeof(T);
            if (items.ContainsKey(t))
                return (items[t] as GeometryFactory2Base<T>);
            return null;
        }
    }

    public abstract class GeometryFactory2Base<T> : SubclassableSingletonBase<GeometryFactory2Base<T>>, IGeometryFactory<T>
        where T : IEquatable<T>, IComparable<T>
    {
        protected GeometryFactory2Base()
        {
            GeometryFactory2Manager.items[typeof(T)] = this;
        }

        public abstract Point2<T> ConstructPoint(Coordinates.Coordinate2<T> coord);
        public abstract Point2<T> ConstructPoint(T x, T y);

        public abstract PointBag2<T> ConstructPointBag(Point2<T>[] points);
        public abstract PointBag2<T> ConstructPointBag(IList<Point2<T>> points);
        public abstract PointBag2<T> ConstructPointBag(IEnumerable<Point2<T>> points);

        public abstract PointSet2<T> ConstructPointSet(Point2<T>[] points);
        public abstract PointSet2<T> ConstructPointSet(IList<Point2<T>> points);
        public abstract PointSet2<T> ConstructPointSet(IEnumerable<Point2<T>> points);

        public LineSegment2<T> ConstructSegment(Point2<T> start, Point2<T> end)
        {
            return new LineSegment2<T>(start, end);
        }

        //orientation is a point on the ray -- provides orientation/direction
        public Ray2<T> ConstructRay(Point2<T> start, Point2<T> orientation)
        {
            return new Ray2<T>(start, orientation);
        }

        //orientation is a point on the line
        public Line2<T> ConstructLine(Point2<T> anchor, Point2<T> orientation)
        {
            return new Line2<T>(anchor, orientation);
        }

        public abstract LineChain2<T> ConstructLineChain(Point2<T>[] points);
        public abstract LineChain2<T> ConstructLineChain(IList<Point2<T>> points);
        public abstract LineChain2<T> ConstructLineChain(IEnumerable<Point2<T>> points);

        //safe conversion
        public abstract LineChainSet2<T> ConstructLineSet(LineChain2<T>[] chains);

        public abstract LineChainSet2<T> ConstructLineSet(IList<LineChain2<T>> chains);
        public abstract LineChainSet2<T> ConstructLineSet(IEnumerable<LineChain2<T>> chains);
        public abstract LineChainSet2<T> ConstructLineSet(Point2<T>[][] chains);
        public abstract LineChainSet2<T> ConstructLineSet(IEnumerable<Point2<T>[]> chains);
        public abstract LineChainSet2<T> ConstructLineSet(IEnumerable<Point2<T>>[] chains);
        public abstract LineChainSet2<T> ConstructLineSet(IList<Point2<T>[]> chains);
        public abstract LineChainSet2<T> ConstructLineSet(IList<Point2<T>>[] chains);

        //safe conversion
        public LineChainBag2<T> ConstructLineBag(LineChain2<T>[] chains)
        {
            if (chains != null)
            {
                foreach (LineChain2<T> cur in chains)
                {
                    if (cur == null)
                        return null;
                }
                return new LineChainBag2<T>(chains);
            }
            return null;
        }

        //null guarding
        public LineChainBag2<T> ConstructLineBag(IList<LineChain2<T>> chains)
        {
            if (chains != null)
            {
                if (chains.Count>0)
                    return this.ConstructLineBag(chains.ToArray());
                return new LineChainBag2<T>(new LineChain2<T>[0]);
            }
            return null;
        }
        public LineChainBag2<T> ConstructLineBag(IEnumerable<LineChain2<T>> chains)
        {
            if (chains != null)
            {
                LeakyResizableArray<LineChain2<T>> chainSet = new LeakyResizableArray<LineChain2<T>>();
                foreach (LineChain2<T> ch in chains)
                {
                    if (ch == null)
                        return null;
                    chainSet.Add(ch);
                }
                chainSet.TrimExcess();
                return new LineChainBag2<T>(chainSet.Data);
            }
            return null;
        }

        public LineChainBag2<T> ConstructLineBag(Point2<T>[][] chains)
        {
            if (chains != null)
            {
                List<LineChain2<T>> constructed = new List<LineChain2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    LineChain2<T> chn = ConstructLineChain(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructLineBag(constructed);
            }
            return null;
        }
        public LineChainBag2<T> ConstructLineBag(IEnumerable<Point2<T>[]> chains)
        {
            if (chains != null)
            {
                List<LineChain2<T>> constructed = new List<LineChain2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    LineChain2<T> chn = ConstructLineChain(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructLineBag(constructed);
            }
            return null;
        }
        public LineChainBag2<T> ConstructLineBag(IEnumerable<Point2<T>>[] chains)
        {
            if (chains != null)
            {
                List<LineChain2<T>> constructed = new List<LineChain2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    LineChain2<T> chn = ConstructLineChain(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructLineBag(constructed);
            }
            return null;
        }
        public LineChainBag2<T> ConstructLineBag(IList<Point2<T>[]> chains)
        {
            if (chains != null)
            {
                List<LineChain2<T>> constructed = new List<LineChain2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    LineChain2<T> chn = ConstructLineChain(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructLineBag(constructed);
            }
            return null;
        }
        public LineChainBag2<T> ConstructLineBag(IList<Point2<T>>[] chains)
        {
            if (chains != null)
            {
                List<LineChain2<T>> constructed = new List<LineChain2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    LineChain2<T> chn = ConstructLineChain(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructLineBag(constructed);
            }
            return null;
        }

        public abstract Polyline2<T> ConstructPolyline(Point2<T>[] points);
        public abstract Polyline2<T> ConstructPolyline(IList<Point2<T>> points);
        public abstract Polyline2<T> ConstructPolyline(IEnumerable<Point2<T>> points);

        //safe conversion
        public abstract PolylineSet2<T> ConstructPolylineSet(Polyline2<T>[] chains);

        public abstract PolylineSet2<T> ConstructPolylineSet(IList<Polyline2<T>> chains);
        public abstract PolylineSet2<T> ConstructPolylineSet(IEnumerable<Polyline2<T>> chains);
        public abstract PolylineSet2<T> ConstructPolylineSet(Point2<T>[][] chains);
        public abstract PolylineSet2<T> ConstructPolylineSet(IEnumerable<Point2<T>[]> chains);
        public abstract PolylineSet2<T> ConstructPolylineSet(IEnumerable<Point2<T>>[] chains);
        public abstract PolylineSet2<T> ConstructPolylineSet(IList<Point2<T>[]> chains);
        public abstract PolylineSet2<T> ConstructPolylineSet(IList<Point2<T>>[] chains);

        //safe conversion
        public PolylineBag2<T> ConstructPolylineBag(Polyline2<T>[] chains)
        {
            if (chains != null)
            {
                foreach (Polyline2<T> cur in chains)
                {
                    if (cur == null)
                        return null;
                }
                return new PolylineBag2<T>(chains);
            }
            return null;
        }

        //null guarding
        public PolylineBag2<T> ConstructPolylineBag(IList<Polyline2<T>> chains)
        {
            if (chains != null)
            {
                if (chains.Count > 0)
                    return this.ConstructPolylineBag(chains.ToArray());
                return new PolylineBag2<T>(new Polyline2<T>[0]);
            }
            return null;
        }
        public PolylineBag2<T> ConstructPolylineBag(IEnumerable<Polyline2<T>> chains)
        {
            if (chains != null)
            {
                LeakyResizableArray<Polyline2<T>> chainSet = new LeakyResizableArray<Polyline2<T>>();
                foreach (Polyline2<T> ch in chains)
                {
                    if (ch == null)
                        return null;
                    chainSet.Add(ch);
                }
                chainSet.TrimExcess();
                return new PolylineBag2<T>(chainSet.Data);
            }
            return null;
        }

        public PolylineBag2<T> ConstructPolylineBag(Point2<T>[][] chains)
        {
            if (chains != null)
            {
                List<Polyline2<T>> constructed = new List<Polyline2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Polyline2<T> chn = ConstructPolyline(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructPolylineBag(constructed);
            }
            return null;
        }
        public PolylineBag2<T> ConstructPolylineBag(IEnumerable<Point2<T>[]> chains)
        {
            if (chains != null)
            {
                List<Polyline2<T>> constructed = new List<Polyline2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Polyline2<T> chn = ConstructPolyline(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructPolylineBag(constructed);
            }
            return null;
        }
        public PolylineBag2<T> ConstructPolylineBag(IEnumerable<Point2<T>>[] chains)
        {
            if (chains != null)
            {
                List<Polyline2<T>> constructed = new List<Polyline2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Polyline2<T> chn = ConstructPolyline(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructPolylineBag(constructed);
            }
            return null;
        }
        public PolylineBag2<T> ConstructPolylineBag(IList<Point2<T>[]> chains)
        {
            if (chains != null)
            {
                List<Polyline2<T>> constructed = new List<Polyline2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Polyline2<T> chn = ConstructPolyline(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructPolylineBag(constructed);
            }
            return null;
        }
        public PolylineBag2<T> ConstructPolylineBag(IList<Point2<T>>[] chains)
        {
            if (chains != null)
            {
                List<Polyline2<T>> constructed = new List<Polyline2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Polyline2<T> chn = ConstructPolyline(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructPolylineBag(constructed);
            }
            return null;
        }

        //potentially unsafe, so must be done by a geometry factory
        public abstract Ring2<T> CloseChain(LineChain2<T> chain);

        public abstract Ring2<T> ConstructRing(Point2<T>[] points);
        public abstract Ring2<T> ConstructRing(IList<Point2<T>> points);
        public abstract Ring2<T> ConstructRing(IEnumerable<Point2<T>> points);
        public Ring2<T> ConstructRing(Envelope2<T> bounds)
        {
            if (bounds!=null)
            {
                return new Ring2<T>(new Point2<T>[] {
                    new Point2<T>(bounds.MinX, bounds.MinY),
                    new Point2<T>(bounds.MaxX, bounds.MinY),
                    new Point2<T>(bounds.MaxX, bounds.MaxY),
                    new Point2<T>(bounds.MinX, bounds.MaxY),
                }, false, Orientation.Counterclockwise);
            }
            return null;
        }

        public abstract RingSet2<T> ConstructRingSet(Ring2<T>[] chains);
        public abstract RingSet2<T> ConstructRingSet(IList<Ring2<T>> chains);
        public abstract RingSet2<T> ConstructRingSet(IEnumerable<Ring2<T>> chains);
        public abstract RingSet2<T> ConstructRingSet(Point2<T>[][] chains);
        public abstract RingSet2<T> ConstructRingSet(IEnumerable<Point2<T>[]> chains);
        public abstract RingSet2<T> ConstructRingSet(IEnumerable<Point2<T>>[] chains);
        public abstract RingSet2<T> ConstructRingSet(IList<Point2<T>[]> chains);
        public abstract RingSet2<T> ConstructRingSet(IList<Point2<T>>[] chains);

        public RingBag2<T> ConstructRingBag(Ring2<T>[] chains)
        {
            if (chains!=null && chains.Length>0)
            {
                Ring2<T>[] Chains = new Ring2<T>[chains.Length];
                Array.Copy(chains, Chains, Chains.Length);

                foreach (Ring2<T> ch in Chains)
                {
                    if (ch == null)
                        return null; //can't have null chains
                }
                return new RingBag2<T>(Chains);
            }
            return null;
        }
        public RingBag2<T> ConstructRingBag(IList<Ring2<T>> chains)
        {
            if (chains != null && chains.Count > 0)
            {
                Ring2<T>[] Chains = new Ring2<T>[chains.Count];
                for (int i = 0; i < Chains.Length; i++)
                {
                    Ring2<T> ch = chains[i];
                    if (ch == null)
                        return null; //can't have null chains
                    Chains[i] = ch;
                }
                return new RingBag2<T>(Chains);
            }
            return null;
        }
        public RingBag2<T> ConstructRingBag(IEnumerable<Ring2<T>> chains)
        {
            if (chains != null)
            {
                LeakyResizableArray<Ring2<T>> chainSet = new LeakyResizableArray<Ring2<T>>();
                foreach (Ring2<T> ch in chains)
                {
                    if (ch == null)
                        return null;
                    chainSet.Add(ch);
                }
                chainSet.TrimExcess();
                Ring2<T>[] Chains = chainSet.Data;

                return new RingBag2<T>(Chains);
            }
            return null;
        }
        public RingBag2<T> ConstructRingBag(Point2<T>[][] chains)
        {
            if (chains != null)
            {
                List<Ring2<T>> constructed = new List<Ring2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Ring2<T> chn = ConstructRing(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructRingBag(constructed);
            }
            return null;
        }
        public RingBag2<T> ConstructRingBag(IEnumerable<Point2<T>[]> chains)
        {
            if (chains != null)
            {
                List<Ring2<T>> constructed = new List<Ring2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Ring2<T> chn = ConstructRing(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructRingBag(constructed);
            }
            return null;
        }
        public RingBag2<T> ConstructRingBag(IEnumerable<Point2<T>>[] chains)
        {
            if (chains != null)
            {
                List<Ring2<T>> constructed = new List<Ring2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Ring2<T> chn = ConstructRing(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructRingBag(constructed);
            }
            return null;
        }
        public RingBag2<T> ConstructRingBag(IList<Point2<T>[]> chains)
        {
            if (chains != null)
            {
                List<Ring2<T>> constructed = new List<Ring2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Ring2<T> chn = ConstructRing(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructRingBag(constructed);
            }
            return null;
        }
        public RingBag2<T> ConstructRingBag(IList<Point2<T>>[] chains)
        {
            if (chains != null)
            {
                List<Ring2<T>> constructed = new List<Ring2<T>>();
                foreach (Point2<T>[] ch in chains)
                {
                    if (ch == null)
                        return null;
                    Ring2<T> chn = ConstructRing(ch);
                    if (chn == null)
                        return null;
                    constructed.Add(chn);
                }
                return ConstructRingBag(constructed);
            }
            return null;
        }

        public Polygon2<T> ConstructPolygon(Envelope2<T> bounds)
        {
            if (bounds == null)
                return null;
            return ConstructRing(bounds);
        }
        public Polygon2<T> ConstructPolygon(Ring2<T> outerRing)
        {
            return outerRing; //implicit cast
        }
        public Polygon2<T> ConstructPolygon(Point2<T>[] outerRing, Ring2<T>[] innerRings)
        {
            if (outerRing != null && innerRings != null)
            {
                Ring2<T> outer = ConstructRing(outerRing);
                if (outer != null)
                {
                    RingSet2<T> inner = ConstructRingSet(innerRings);
                    if (inner != null)
                        return ConstructPolygon(outer, inner);
                }
            }
            return null;
        }
        public Polygon2<T> ConstructPolygon(Ring2<T> outerRing, Ring2<T>[] innerRings)
        {
            if (outerRing!=null && innerRings!=null)
            {
                RingSet2<T> rs = ConstructRingSet(innerRings);
                if (rs != null)
                    return ConstructPolygon(outerRing, rs);
            }
            return null;
        }
        public Polygon2<T> ConstructPolygon(Point2<T>[] outerRing, Point2<T>[][] innerRings)
        {
            if (outerRing != null && innerRings != null)
            {
                Ring2<T> outer = ConstructRing(outerRing);
                if (outer != null)
                {
                    RingSet2<T> inner = ConstructRingSet(innerRings);
                    if (inner != null)
                        return ConstructPolygon(outer, inner);
                }
            }
            return null;
        }
        public abstract Polygon2<T> ConstructPolygon(Ring2<T> outerRing, RingSet2<T> innerRings);

        public PolygonSet2<T> ConstructPolygonSet(IList<Polygon2<T>> parts)
        {
            if (parts!=null)
                return ConstructPolygonSet(parts.ToArray());
            return null;
        }
        public PolygonSet2<T> ConstructPolygonSet(IEnumerable<Polygon2<T>> parts)
        {
            if (parts!=null)
                return ConstructPolygonSet(parts.ToArray());
            return null;
        }
        public abstract PolygonSet2<T> ConstructPolygonSet(Polygon2<T>[] parts);

        public PolygonBag2<T> ConstructPolygonBag(Polygon2<T>[] parts)
        {
            if (parts!=null && parts.Length>0)
            {
                Polygon2<T>[] tmp = new Polygon2<T>[parts.Length];
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (parts[i] == null)
                        return null;
                    tmp[i] = parts[i];
                }
                return new PolygonBag2<T>(tmp);
            }
            return null;
        }
        public PolygonBag2<T> ConstructPolygonBag(IList<Polygon2<T>> parts)
        {
            if (parts != null && parts.Count > 0)
            {
                Polygon2<T>[] tmp = new Polygon2<T>[parts.Count];
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (parts[i] == null)
                        return null;
                    tmp[i] = parts[i];
                }
                return new PolygonBag2<T>(tmp);
            }
            return null;
        }
        public PolygonBag2<T> ConstructPolygonBag(IEnumerable<Polygon2<T>> parts)
        {
            if (parts != null)
            {
                List<Polygon2<T>> tmp = new List<Polygon2<T>>();
                foreach (Polygon2<T> cur in parts)
                {
                    if (cur == null)
                        return null;
                    tmp.Add(cur);
                }
                if (tmp.Count < 1)
                    return null;
                return new PolygonBag2<T>(tmp.ToArray());
            }
            return null;
        }

        public abstract Geometry2Set<T> ConstructGeometrySet(IEnumerable<IGeometry2<T>> shapes);
        public Geometry2Bag<T> ConstructGeometryBag(IEnumerable<IGeometry2<T>> shapes)
        {
            if (shapes!=null)
            {
                List<IGeometry2<T>> tmp = new List<IGeometry2<T>>();
                foreach(IGeometry2<T> cur in shapes)
                {
                    if (cur != null)
                        tmp.Add(cur);
                }
                if (tmp.Count > 0)
                    return new Geometry2Bag<T>(tmp.ToArray());
            }
            return null;
        }
    }
}
