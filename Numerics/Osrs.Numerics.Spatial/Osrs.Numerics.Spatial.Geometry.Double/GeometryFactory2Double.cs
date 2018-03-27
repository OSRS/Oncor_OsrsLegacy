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
using Osrs.Numerics.Spatial.Coordinates;
using Osrs.Numerics.Spatial.Operations;
using System.Linq;
using Osrs.Collections.Specialized;

namespace Osrs.Numerics.Spatial.Geometry
{
    public sealed class GeometryFactory2Double : GeometryFactory2Base<double>
    {
        public override Ring2<double> CloseChain(LineChain2<double> chain)
        {
            if (chain == null)
                return null;
            if (chain.Points.Length < 3)
                return null;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            graph.Add(chain.Points, true);
            if (PlanarGraphUtils.AnyIntersections(graph))
                return null;
            Orientation o = Orientation.Clockwise;
            if (chain.IsReversed)
            {
                if (PointUtilsDouble.IsCCW(chain.Points))
                    o = Orientation.Clockwise;
                else
                    o = Orientation.Counterclockwise;
            }
            else
            {
                if (PointUtilsDouble.IsCCW(chain.Points))
                    o = Orientation.Counterclockwise;
                else
                    o = Orientation.Counterclockwise;
            }
            return new Ring2<double>(chain.Points, chain.IsReversed, o);
        }

        public override LineChain2<double> ConstructLineChain(IList<Point2<double>> points)
        {
            if (points == null || points.Count < 2)
                return null;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            Point2<double>[] Coordinates = PointUtils.RemoveAdjacentDups<double>(points, false);
            if (Coordinates.Length < 2)
                return null;
            graph.Add(Coordinates, false);
            if (PlanarGraphUtils.AnyIntersections(graph))
                return null;
            return new LineChain2<double>(Coordinates);
        }

        public override LineChain2<double> ConstructLineChain(IEnumerable<Point2<double>> points)
        {
            if (points == null)
                return null;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            Point2<double>[] Coordinates = PointUtils.RemoveAdjacentDups<double>(points, false);
            if (Coordinates.Length < 2)
                return null;
            graph.Add(Coordinates, false);
            if (PlanarGraphUtils.AnyIntersections(graph))
                return null;
            return new LineChain2<double>(Coordinates);
        }

        public override LineChain2<double> ConstructLineChain(Point2<double>[] points)
        {
            if (points == null || points.Length < 2)
                return null;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            Point2<double>[] Coordinates = PointUtils.RemoveAdjacentDups<double>(points, false);
            if (Coordinates.Length < 2)
                return null;
            graph.Add(Coordinates, false);
            if (PlanarGraphUtils.AnyIntersections(graph))
                return null;
            return new LineChain2<double>(Coordinates);
        }

        public override LineChainSet2<double> ConstructLineSet(IEnumerable<Point2<double>[]> chains)
        {
            if (chains == null)
                return null;
            List<LineChain2<double>> constructed = new List<LineChain2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                LineChain2<double> chn = ConstructLineChain(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructLineSet(constructed);
        }

        public override LineChainSet2<double> ConstructLineSet(IList<LineChain2<double>> chains)
        {
            if (chains == null || chains.Count < 1)
                return null;
            LineChain2<double>[] Chains = chains.ToArray();

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            foreach (LineChain2<double> ch in chains)
            {
                if (ch == null)
                    return null; //can't have null chains
                graph.Add(ch);
            }
            if (PlanarGraphUtils.AnyIntersections(graph)) //there must be overlap
                return null;

            return new LineChainSet2<double>(Chains);
        }

        public override LineChainSet2<double> ConstructLineSet(Point2<double>[][] chains)
        {
            if (chains == null)
                return null;
            List<LineChain2<double>> constructed = new List<LineChain2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                LineChain2<double> chn = ConstructLineChain(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructLineSet(constructed);
        }

        public override LineChainSet2<double> ConstructLineSet(IEnumerable<LineChain2<double>> chains)
        {
            if (chains == null)
                return null;
            LineChain2<double>[] Chains = chains.ToArray();

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            foreach (LineChain2<double> ch in Chains)
            {
                if (ch == null)
                    return null; //can't have null chains
                graph.Add(ch);
            }
            if (PlanarGraphUtils.AnyIntersections(graph)) //there must be overlap
                return null;

            return new LineChainSet2<double>(Chains);
        }

        public override LineChainSet2<double> ConstructLineSet(IEnumerable<Point2<double>>[] chains)
        {
            if (chains == null)
                return null;
            List<LineChain2<double>> constructed = new List<LineChain2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                LineChain2<double> chn = ConstructLineChain(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructLineSet(constructed);
        }

        public override LineChainSet2<double> ConstructLineSet(IList<Point2<double>>[] chains)
        {
            if (chains == null)
                return null;
            List<LineChain2<double>> constructed = new List<LineChain2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                LineChain2<double> chn = ConstructLineChain(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructLineSet(constructed);
        }

        public override LineChainSet2<double> ConstructLineSet(IList<Point2<double>[]> chains)
        {
            if (chains == null)
                return null;
            List<LineChain2<double>> constructed = new List<LineChain2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                LineChain2<double> chn = ConstructLineChain(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructLineSet(constructed);
        }

        public override LineChainSet2<double> ConstructLineSet(LineChain2<double>[] chains)
        {
            if (chains == null || chains.Length < 1)
                return null;

            LineChain2<double>[] Chains = new LineChain2<double>[chains.Length];
            LineChain2<double> ch;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();

            for (int i = 0; i < Chains.Length; i++)
            {
                ch = chains[i];
                if (ch == null)
                    return null; //can't have null chains
                Chains[i] = ch;
                graph.Add(ch);
            }
            if (PlanarGraphUtils.AnyIntersections(graph)) //there must be overlap
                return null;

            return new LineChainSet2<double>(Chains);
        }

        public override Point2<double> ConstructPoint(Coordinate2<double> coord)
        {
            if (coord != null && !(double.IsNaN(coord.X) || double.IsNaN(coord.Y)))
                return new Point2<double>(coord.X, coord.Y);
            return null;
        }

        public override Point2<double> ConstructPoint(double x, double y)
        {
            if (double.IsNaN(x) || double.IsNaN(y))
                return null;
            return new Point2<double>(x, y);
        }

        public override PointBag2<double> ConstructPointBag(IEnumerable<Point2<double>> points)
        {
            if (points != null)
            {
                List<Point2<double>> tmp = new List<Point2<double>>();
                foreach (Point2<double> cur in points)
                {
                    if (cur != null)
                        tmp.Add(cur);
                }
                return new PointBag2<double>(tmp.ToArray());
            }
            return null;
        }

        public override PointBag2<double> ConstructPointBag(IList<Point2<double>> points)
        {
            if (points != null && points.Count>0)
            {
                bool sparse = false;
                IList<Point2<double>> tmp = points;
                foreach (Point2<double> cur in points)
                {
                    if (cur == null)
                    {
                        sparse = true;
                        break;
                    }
                }
                if (sparse)
                {
                    tmp = new List<Point2<double>>();
                    foreach (Point2<double> cur in points)
                    {
                        if (cur != null)
                        {
                            tmp.Add(cur);
                        }
                    }
                }
                return new PointBag2<double>(tmp.ToArray());
            }
            return null;
        }

        public override PointBag2<double> ConstructPointBag(Point2<double>[] points)
        {
            if (points!=null && points.Length>0)
            {
                bool sparse = false;
                foreach (Point2<double> cur in points)
                {
                    if (cur == null)
                    {
                        sparse = true;
                        break;
                    }
                }
                if (sparse)
                {
                    IList<Point2<double>> tmp = new List<Point2<double>>();
                    foreach (Point2<double> cur in points)
                    {
                        if (cur != null)
                        {
                            tmp.Add(cur);
                        }
                    }
                    return new PointBag2<double>(tmp.ToArray());
                }
                Point2<double>[] copy = new Point2<double>[points.Length];
                Array.Copy(points, copy, copy.Length);
                return new PointBag2<double>(copy);
            }
            return null;
        }

        public override PointSet2<double> ConstructPointSet(IEnumerable<Point2<double>> points)
        {
            if (points!=null)
            {
                Point2<double>[] unique = PointUtils.RemoveDups<double>(points);
                if (unique != null && unique.Length > 0)
                    return new PointSet2<double>(unique);
            }
            return null;
        }

        public override PointSet2<double> ConstructPointSet(IList<Point2<double>> points)
        {
            if (points != null)
            {
                Point2<double>[] unique = PointUtils.RemoveDups<double>(points);
                if (unique != null && unique.Length > 0)
                    return new PointSet2<double>(unique);
            }
            return null;
        }

        public override PointSet2<double> ConstructPointSet(Point2<double>[] points)
        {
            if (points != null)
            {
                Point2<double>[] unique = PointUtils.RemoveDups<double>(points);
                if (unique != null && unique.Length > 0)
                    return new PointSet2<double>(unique);
            }
            return null;
        }

        public override Polygon2<double> ConstructPolygon(Ring2<double> outerRing, RingSet2<double> innerRings)
        {
            if (outerRing == null)
                return null;
            if (innerRings == null)
                return new Polygon2<double>(outerRing);
            if (PlanarGraphUtils.ValidPolygon(outerRing, innerRings))
                return new Polygon2<double>(outerRing, innerRings);
            return null;
        }

        public override PolygonSet2<double> ConstructPolygonSet(Polygon2<double>[] parts)
        {
            if (parts == null)
                return null;
            Ring2<double>[] shells = new Ring2<double>[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                Polygon2<double> curA = parts[i];
                if (curA == null)
                    return null;
                shells[i] = curA.OuterRing;
            }
            if (PlanarGraphUtils.ValidRingSet(shells))
                return new PolygonSet2<double>(parts);
            return null;
        }

        public override Polyline2<double> ConstructPolyline(IEnumerable<Point2<double>> points)
        {
            if (points == null)
                return null;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            Point2<double>[] Coordinates = PointUtils.RemoveAdjacentDups<double>(points, false);
            if (Coordinates.Length < 2)
                return null;
            graph.Add(Coordinates, false);
            return new Polyline2<double>(Coordinates);
        }

        public override Polyline2<double> ConstructPolyline(IList<Point2<double>> points)
        {
            if (points == null || points.Count < 2)
                return null;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            Point2<double>[] Coordinates = PointUtils.RemoveAdjacentDups<double>(points, false);
            if (Coordinates.Length < 2)
                return null;
            graph.Add(Coordinates, false);
            return new Polyline2<double>(Coordinates);
        }

        public override Polyline2<double> ConstructPolyline(Point2<double>[] points)
        {
            if (points == null || points.Length < 2)
                return null;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            Point2<double>[] Coordinates = PointUtils.RemoveAdjacentDups<double>(points, false);
            if (Coordinates.Length < 2)
                return null;
            graph.Add(Coordinates, false);
            return new Polyline2<double>(Coordinates);
        }

        public override PolylineSet2<double> ConstructPolylineSet(IEnumerable<Point2<double>[]> chains)
        {
            if (chains == null)
                return null;
            List<Polyline2<double>> constructed = new List<Polyline2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Polyline2<double> chn = ConstructPolyline(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructPolylineSet(constructed);
        }

        public override PolylineSet2<double> ConstructPolylineSet(IEnumerable<Point2<double>>[] chains)
        {
            if (chains == null)
                return null;
            List<Polyline2<double>> constructed = new List<Polyline2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Polyline2<double> chn = ConstructPolyline(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructPolylineSet(constructed);
        }

        public override PolylineSet2<double> ConstructPolylineSet(IList<Point2<double>[]> chains)
        {
            if (chains == null)
                return null;
            List<Polyline2<double>> constructed = new List<Polyline2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Polyline2<double> chn = ConstructPolyline(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructPolylineSet(constructed);
        }

        public override PolylineSet2<double> ConstructPolylineSet(IList<Point2<double>>[] chains)
        {
            if (chains == null)
                return null;
            List<Polyline2<double>> constructed = new List<Polyline2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Polyline2<double> chn = ConstructPolyline(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructPolylineSet(constructed);
        }

        public override PolylineSet2<double> ConstructPolylineSet(IEnumerable<Polyline2<double>> chains)
        {
            if (chains == null)
                return null;
            Polyline2<double>[] Chains = chains.ToArray();

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            foreach (Polyline2<double> ch in Chains)
            {
                if (ch == null)
                    return null; //can't have null chains
                graph.Add(new LineChain2<double>(ch.Points)); //TODO -- fix this, it's broken 
            }
            if (PlanarGraphUtils.AnyIntersections(graph)) //there must be overlap
                return null;

            return new PolylineSet2<double>(Chains);
        }

        public override PolylineSet2<double> ConstructPolylineSet(Point2<double>[][] chains)
        {
            if (chains == null)
                return null;
            List<Polyline2<double>> constructed = new List<Polyline2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Polyline2<double> chn = ConstructPolyline(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructPolylineSet(constructed);
        }

        public override PolylineSet2<double> ConstructPolylineSet(IList<Polyline2<double>> chains)
        {
            if (chains == null || chains.Count < 1)
                return null;
            Polyline2<double>[] Chains = chains.ToArray();

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            foreach (Polyline2<double> ch in Chains)
            {
                if (ch == null)
                    return null; //can't have null chains
                graph.Add(new LineChain2<double>(ch.Points)); //TODO -- fix this, it's broken 
            }
            if (PlanarGraphUtils.AnyIntersections(graph)) //there must be overlap
                return null;

            return new PolylineSet2<double>(Chains);
        }

        public override PolylineSet2<double> ConstructPolylineSet(Polyline2<double>[] chains)
        {
            if (chains == null || chains.Length < 1)
                return null;

            Polyline2<double>[] Chains = new Polyline2<double>[chains.Length];
            Polyline2<double> ch;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();

            for (int i = 0; i < Chains.Length; i++)
            {
                ch = chains[i];
                if (ch == null)
                    return null; //can't have null chains
                Chains[i] = ch;
                graph.Add(new LineChain2<double>(ch.Points)); //TODO -- fix this, it's broken 
            }
            if (PlanarGraphUtils.AnyIntersections(graph)) //there must be overlap
                return null;

            return new PolylineSet2<double>(Chains);
        }

        public override Ring2<double> ConstructRing(IEnumerable<Point2<double>> points)
        {
            if (points == null)
                return null;
            Point2<double>[] pts = PointUtils.RemoveAdjacentDups<double>(points, true);
            if (pts.Length < 3)
                return null;

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            graph.Add(pts, true);
            if (PlanarGraphUtils.AnyIntersections(graph))
                return null;
            Orientation o = Orientation.Clockwise;
            if (PointUtilsDouble.IsCCW(pts))
                o = Orientation.Counterclockwise;
            return new Ring2<double>(pts, false, o);
        }

        public override Ring2<double> ConstructRing(IList<Point2<double>> points)
        {
            if (points == null || points.Count < 3)
                return null;
            Point2<double>[] pts = PointUtils.RemoveAdjacentDups<double>(points, true);
            if (pts.Length < 3)
                return null;

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            graph.Add(pts, true);
            if (PlanarGraphUtils.AnyIntersections(graph))
                return null;
            Orientation o = Orientation.Clockwise;
            if (PointUtilsDouble.IsCCW(pts))
                o = Orientation.Counterclockwise;
            return new Ring2<double>(pts, false, o);
        }

        public override Ring2<double> ConstructRing(Point2<double>[] points)
        {
            if (points == null || points.Length < 3)
                return null;
            Point2<double>[] pts = PointUtils.RemoveAdjacentDups<double>(points, true);
            if (pts.Length < 3)
                return null;

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            graph.Add(pts, true);
            if (PlanarGraphUtils.AnyIntersections(graph))
                return null;
            Orientation o = Orientation.Clockwise;
            if (PointUtilsDouble.IsCCW(pts))
                o = Orientation.Counterclockwise;
            return new Ring2<double>(pts, false, o);
        }

        public override RingSet2<double> ConstructRingSet(IList<Point2<double>[]> chains)
        {
            if (chains == null)
                return null;
            List<Ring2<double>> constructed = new List<Ring2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Ring2<double> chn = ConstructRing(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructRingSet(constructed);
        }

        public override RingSet2<double> ConstructRingSet(IList<Point2<double>>[] chains)
        {
            if (chains == null)
                return null;
            List<Ring2<double>> constructed = new List<Ring2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Ring2<double> chn = ConstructRing(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructRingSet(constructed);
        }

        public override RingSet2<double> ConstructRingSet(IEnumerable<Point2<double>>[] chains)
        {
            if (chains == null)
                return null;
            List<Ring2<double>> constructed = new List<Ring2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Ring2<double> chn = ConstructRing(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructRingSet(constructed);
        }

        public override RingSet2<double> ConstructRingSet(Point2<double>[][] chains)
        {
            if (chains == null)
                return null;
            List<Ring2<double>> constructed = new List<Ring2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Ring2<double> chn = ConstructRing(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructRingSet(constructed);
        }

        public override RingSet2<double> ConstructRingSet(IEnumerable<Point2<double>[]> chains)
        {
            if (chains == null)
                return null;
            List<Ring2<double>> constructed = new List<Ring2<double>>();
            foreach (Point2<double>[] ch in chains)
            {
                if (ch == null)
                    return null;
                Ring2<double> chn = ConstructRing(ch);
                if (chn == null)
                    return null;
                constructed.Add(chn);
            }
            return ConstructRingSet(constructed);
        }

        public override RingSet2<double> ConstructRingSet(IEnumerable<Ring2<double>> chains)
        {
            if (chains == null)
                return null;
            LeakyResizableArray<Ring2<double>> chainSet = new LeakyResizableArray<Ring2<double>>();
            foreach (Ring2<double> ch in chains)
            {
                if (ch == null)
                    return null;
                chainSet.Add(ch);
            }
            chainSet.TrimExcess();
            Ring2<double>[] Chains = chainSet.Data;

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            foreach (Ring2<double> ch in Chains)
            {
                if (ch == null)
                    return null; //can't have null chains
                graph.Add(ch);
            }
            if (PlanarGraphUtils.AnyIntersections(graph)) //there must be overlap
                return null;

            return new RingSet2<double>(Chains);
        }

        public override RingSet2<double> ConstructRingSet(IList<Ring2<double>> chains)
        {
            if (chains == null || chains.Count < 1)
                return null;
            Ring2<double>[] Chains = new Ring2<double>[chains.Count];
            for (int i = 0; i < Chains.Length; i++)
                Chains[i] = chains[i];

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            foreach (Ring2<double> ch in Chains)
            {
                if (ch == null)
                    return null; //can't have null chains
                graph.Add(ch);
            }
            if (PlanarGraphUtils.AnyIntersections(graph)) //there must be overlap
                return null;

            return new RingSet2<double>(Chains);
        }

        public override RingSet2<double> ConstructRingSet(Ring2<double>[] chains)
        {
            if (chains == null || chains.Length < 1)
                return null;
            Ring2<double>[] Chains = new Ring2<double>[chains.Length];
            Array.Copy(chains, Chains, Chains.Length);

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            foreach (Ring2<double> ch in Chains)
            {
                if (ch == null)
                    return null; //can't have null chains
                graph.Add(ch);
            }
            if (PlanarGraphUtils.AnyIntersections(graph)) //there must be overlap
                return null;

            return new RingSet2<double>(Chains);
        }

        public override Geometry2Set<double> ConstructGeometrySet(IEnumerable<IGeometry2<double>> shapes)
        {
            if (shapes!=null)
            {
                List<IGeometry2<double>> tmp = new List<IGeometry2<double>>();
                foreach(IGeometry2<double> cur in shapes)
                {
                    if (cur != null)
                        tmp.Add(cur);
                }

                if (tmp.Count>0)
                {
                    //TODO -- complete this by finding any intersections and failing
                }
            }
            return null;
        }

        private GeometryFactory2Double() : base()
        { }

        private static GeometryFactory2Double instance = new GeometryFactory2Double(); //auto registers with factory
        public static GeometryFactory2Double Instance
        {
            get { return instance; }
        }
    }
}
