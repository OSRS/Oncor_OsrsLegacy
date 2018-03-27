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
using Osrs.Numerics.Graphs;
using Osrs.Numerics.Spatial.Coordinates;
using Osrs.Numerics.Spatial.Operations;
using System.Collections.Generic;
using System.Linq;

namespace Osrs.Numerics.Spatial.Geometry
{
    public static class PlanarGraphUtils
    {
        public static bool ValidRingSet(Ring2<double>[] rings)
        {
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            HashSet<Ring2<double>> ringHash = new HashSet<Ring2<double>>(); //check for the silly condition of multiple references to the same ring
            foreach (Ring2<double> r in rings)
            {
                if (r == null)
                    return false; //not a ring, for shame
                if (ringHash.Contains(r))
                    return false; //oops, duplicate reference
                ringHash.Add(r);
                graph.Add(r);
            }
            ringHash.Clear();
            ringHash = null;

            SegmentGroup<double> activeEdges = new SegmentGroup<double>();
            IEnumerator<Node<double>> points = graph.GetEnumerator(); //walk through the points in xy sorted order

            Node<double> nd;
            LeakyResizableArray<Edge<double>> localEdges;

            int localCt;
            int activeCt = 0;
            Edge<double> localEdge;
            LineSegment2IntersectionResult<double> intersects;
            Point2<double> localStart;
            Point2<double> localEnd;
            Point2<double> activeStart;
            Point2<double> activeEnd;
            Point2<double> intPoint;

            //new point event in the moving front
            while (points.MoveNext())
            {
                nd = points.Current;
                localEdges = nd.Edges; //edges connected to this point

                localCt = (int)localEdges.Count;

                //compute intersections with other edges in the scan area
                for (int i = 0; i < localCt; i++)
                {
                    localEdge = localEdges.Data[i];
                    localStart = localEdge.Start.Point;
                    localEnd = localEdge.End.Point;
                    activeCt = activeEdges.Edges.Count;

                    foreach (Edge<double> activeEdge in activeEdges.Edges)
                    {
                        if (object.ReferenceEquals(localEdge, activeEdge) || object.ReferenceEquals(localEdge.Start.ParentShape, activeEdge.Start.ParentShape))
                            continue; //exiting segment || 2 edges on same ring
                        activeStart = activeEdge.Start.Point;
                        activeEnd = activeEdge.End.Point;

                        intersects = SegmentUtils.ComputeIntersection(localStart, localEnd, activeStart, activeEnd);
                        if (intersects.IntersectionType != LineIntersectionType.NoIntersection)
                        {
                            if (intersects.IntersectionType == LineIntersectionType.CollinearIntersection)
                                return false; // there's a full segment of intersection
                            intPoint = (Point2<double>)intersects.BasePoint;
                            //we have a point intersection, and this is not on the same ring
                            //if the intersection point is not coincident with an endpoint, we have a bad case
                            //otherwise -- this is ok, since there is no common "length"
                            //but we need to check the odd condition that the sequential points don't cross over the ring out-on-in or in-on-out
                            if (PointUtilsDouble.PointsEqual(intPoint, localStart) || PointUtilsDouble.PointsEqual(intPoint, localEnd))
                            {
                                //localEdge.Previous.Start, localStart, localEnd, localEdge.Next.End
                                Ring2<double> srcRing = (Ring2<double>)activeEdge.Start.ParentShape; //we're comparing against the ring
                                if (PointUtilsDouble.PointsEqual(intPoint, localStart))
                                {
                                    if (RingUtils.PointInteriorToRing(srcRing, localEnd))
                                        return false; //point is inside ring
                                    if (RingUtils.PointInteriorToRing(srcRing, localEdge.Previous.Start.Point)) //since localStart, then we need the start of the previous edge
                                        return false; //point is inside ring
                                }
                                else //matched localEnd
                                {
                                    if (RingUtils.PointInteriorToRing(srcRing, localStart))
                                        return false; //point is inside ring
                                    if (RingUtils.PointInteriorToRing(srcRing, localEdge.Next.End.Point)) //since localEnd, then we need the end of the next edge
                                        return false; //point is inside ring
                                }
                            }
                            else if (PointUtilsDouble.PointsEqual(intPoint, activeStart) || PointUtilsDouble.PointsEqual(intPoint, activeEnd))
                            {
                                //localEdge.Previous.Start, localStart, localEnd, localEdge.Next.End
                                Ring2<double> srcRing = (Ring2<double>)localEdge.Start.ParentShape; //we're comparing against the ring
                                if (PointUtilsDouble.PointsEqual(intPoint, activeStart))
                                {
                                    if (RingUtils.PointInteriorToRing(srcRing, activeEnd))
                                        return false; //point is inside ring
                                    if (RingUtils.PointInteriorToRing(srcRing, activeEdge.Previous.Start.Point)) //since localStart, then we need the start of the previous edge
                                        return false; //point is inside ring
                                }
                                else //matched activeEnd
                                {
                                    if (RingUtils.PointInteriorToRing(srcRing, activeStart))
                                        return false; //point is inside ring
                                    if (RingUtils.PointInteriorToRing(srcRing, activeEdge.Next.End.Point)) //since localEnd, then we need the end of the next edge
                                        return false; //point is inside ring
                                }
                            }
                            else
                                return false; //we have a mid-segment intersection between the outer ring and an inner ring
                        }
                    }
                }
                //remove all exiting segments and add all starting segments
                //Action gets called exactly twice per edge -- once to add it, once to remove it
                for (int i = 0; i < localCt; i++)
                {
                    activeEdges.Action(localEdges.Data[i]);
                }
            }
            //now, we need to check that none of the rings are inside of another ring -- we only need to check a single point per ring (unless that point is on the ring)
            //note we need to neglect the inclusive/exclusive nature and just focus on point being interior to ring, since these will often be holes
            for (int i = 1; i < rings.Length; i++)
            {
                Ring2<double> r = rings[i];
                Ring2<double> k;
                for (int j = 0; j < i; j++)
                {
                    k = rings[j];
                    if (RingUtils.PointInteriorToRing(r, k.Points[0]) || RingUtils.PointInteriorToRing(r, k.Points[1])) //have to be sure point not ON ring
                        return false;
                }
            }
            return true;
        }

        public static bool ValidPolygon(Ring2<double> outer, RingSet2<double> inner)
        {
            if (outer == null || inner == null)
                return false;
            HashSet<Ring2<double>> ringHash = new HashSet<Ring2<double>>(); //check for the silly condition of multiple references to the same ring
            ringHash.Add(outer);

            Envelope2<double> outerEnv = outer.Envelope;

            PlanarChainGraph<double> gr = new PlanarChainGraph<double>();
            foreach (Ring2<double> r in inner.Rings)
            {
                if (!ringHash.Add(r))
                    return false; //oops, duplicate reference
                if (!Envelope2<double>.EnvelopeContain(outerEnv, r.Envelope)) //all inner rings must be contained by outer ring, so the envelopes must overlap
                    return false;
                gr.Add(r);
            }
            ringHash.Clear();
            ringHash = null;

            gr.Add(outer);

            SegmentGroup<double> activeEdges = new SegmentGroup<double>();
            IEnumerator<Node<double>> points = gr.GetEnumerator(); //walk through the points in xy sorted order

            Node<double> nd;
            LeakyResizableArray<Edge<double>> localEdges;

            int localCt;
            int activeCt = 0;
            Edge<double> localEdge;
            LineSegment2IntersectionResult<double> intersects;
            Point2<double> localStart;
            Point2<double> localEnd;
            Point2<double> activeStart;
            Point2<double> activeEnd;
            Point2<double> intPoint;
            SimpleGraph<object> touchingRings = new SimpleGraph<object>();
            HashSet<object> outerRingTouching = new HashSet<object>();

            //new point event in the moving front
            while (points.MoveNext())
            {
                nd = points.Current;
                localEdges = nd.Edges; //edges connected to this point

                localCt = (int)localEdges.Count;

                //compute intersections with other edges in the scan area
                for (int i = 0; i < localCt; i++)
                {
                    localEdge = localEdges.Data[i];
                    localStart = localEdge.Start.Point;
                    localEnd = localEdge.End.Point;
                    activeCt = activeEdges.Edges.Count;

                    foreach (Edge<double> activeEdge in activeEdges.Edges)
                    {
                        if (object.ReferenceEquals(localEdge, activeEdge) || object.ReferenceEquals(localEdge.Start.ParentShape, activeEdge.Start.ParentShape))
                            continue; //exiting edge || 2 edges on same ring
                        activeStart = activeEdge.Start.Point;
                        activeEnd = activeEdge.End.Point;

                        intersects = SegmentUtils.ComputeIntersection(localStart, localEnd, activeStart, activeEnd);
                        if (intersects.IntersectionType != LineIntersectionType.NoIntersection)
                        {
                            if (intersects.IntersectionType == LineIntersectionType.CollinearIntersection)
                                return false; // there's a full segment of intersection - must be between outer ring and an inner ring

                            //ok, we have an intersection that is a point between 2 different rings
                            intPoint = intersects.BasePoint;

                            if (object.ReferenceEquals(localEdge.Start.ParentShape, outer)) //localEdge is on the shell
                            {
                                if (PointUtilsDouble.PointsEqual(intPoint, activeStart))
                                {
                                    //only add the shell touching for segment intersection at segment start point - prevents recounting point
                                    if (!PointUtilsDouble.PointsEqual(intPoint, localEnd))
                                    {
                                        if (!outerRingTouching.Add(activeEdge.Start.ParentShape))
                                            return false; //same ring touches outer ring at multiple non-adjacent points
                                    }
                                    if (!RingUtils.PointInteriorToRing(outer, activeEnd))
                                        return false; //ring is outside of shell or crosses shell at a vertex
                                }
                                else if (PointUtilsDouble.PointsEqual(intPoint, activeEnd))
                                {
                                    //only add the shell touching for segment intersection at segment start point - prevents recounting point
                                    if (!PointUtilsDouble.PointsEqual(intPoint, localEnd))
                                    {
                                        if (!outerRingTouching.Add(activeEdge.Start.ParentShape))
                                            return false; //same ring touches outer ring at multiple non-adjacent points
                                    }
                                    if (!RingUtils.PointInteriorToRing(outer, activeStart))
                                        return false; //ring is outside of shell or crosses shell at a vertex
                                }
                                else
                                    return false; //we have a mid-segment intersection between the outer ring and an inner ring

                            }
                            else if (object.ReferenceEquals(activeEdge.Start.ParentShape, outer)) //activeEdge is on the shell
                            {
                                if (PointUtilsDouble.PointsEqual(intPoint, localStart))
                                {
                                    //only add the shell touching for segment intersection at segment start point - prevents recounting point
                                    if (!PointUtilsDouble.PointsEqual(intPoint, activeEnd))
                                    {
                                        if (!outerRingTouching.Add(localEdge.Start.ParentShape))
                                            return false; //same ring touches outer ring at multiple non-adjacent points
                                    }
                                    if (!RingUtils.PointInteriorToRing(outer, localEnd))
                                        return false; //ring is outside of shell or crosses shell at a vertex
                                }
                                else if (PointUtilsDouble.PointsEqual(intPoint, localEnd))
                                {
                                    //only add the shell touching for segment intersection at segment start point - prevents recounting point
                                    if (!PointUtilsDouble.PointsEqual(intPoint, activeEnd))
                                    {
                                        if (!outerRingTouching.Add(localEdge.Start.ParentShape))
                                            return false; //same ring touches outer ring at multiple non-adjacent points
                                    }
                                    if (!RingUtils.PointInteriorToRing(outer, localStart))
                                        return false; //ring is outside of shell or crosses shell at a vertex
                                }
                                else
                                    return false; //we have a mid-segment intersection between the outer ring and an inner ring
                            }
                            else //both are on inner rings
                            {
                                //since the ringset is valid, we just need to map the connectivity of rings
                                if (!touchingRings.Contains(localEdge.Start.ParentShape))
                                    touchingRings.AddNode(localEdge.Start.ParentShape);
                                if (!touchingRings.Contains(activeEdge.Start.ParentShape))
                                    touchingRings.AddNode(activeEdge.Start.ParentShape);
                                touchingRings.AddEdge(localEdge.Start.ParentShape, activeEdge.Start.ParentShape);
                            }
                        } //end !nointersection
                    } //end foreach
                } //end for

                //remove all exiting segments and add all starting segments
                //Action gets called exactly twice per edge -- once to add it, once to remove it
                for (int i = 0; i < localCt; i++)
                {
                    activeEdges.Action(localEdges.Data[i]);
                }
            }

            //we now know all the rings of the ringset are "legally" connected wrt the shell -- but --
            //inner rings may be adjacent to both the shell and other inner rings to form a "path" cutting the shell
            //any ring COULD be outside the shell -- we did already do the quick envelope check, so just do singular point-in-ring checks

            //shell cutting test (quick)
            object[] shellTouchers = outerRingTouching.ToArray();
            for (int i = shellTouchers.Length - 1; i > 0; i--)
            {
                object adjacentRingA = shellTouchers[i];
                if (touchingRings.Contains(adjacentRingA))
                {
                    for (int j = 0; j < i; j++)
                    {
                        object adjacentRingB = shellTouchers[j];
                        if (touchingRings.Contains(adjacentRingB))
                        {
                            if (HasPath(adjacentRingA, adjacentRingB, touchingRings))
                                return false; //path exists between shell touching inner rings
                        }
                    }
                }
            }

            //ring inside shell testing
            foreach (Ring2<double> innerR in inner.Rings)
            {
                if (!(RingUtils.PointInteriorToRing(outer, innerR.Points[0]) || RingUtils.PointInteriorToRing(outer, innerR.Points[1]))) //at least one of 2 adjacent points must be interior
                    return false;
            }

            return true;
        }

        private static bool HasPath(object adjacentRingA, object adjacentRingB, SimpleGraph<object> graph)
        {
            HashSet<object> visited = new HashSet<object>();
            bool more = true;
            List<object> actives = new List<object>();
            List<object> newActives = new List<object>();
            actives.Add(adjacentRingB);
            while (more)
            {
                more = false;
                foreach (object cur in actives)
                {
                    visited.Add(cur);
                    object[] items = graph.GetAdjacent(cur);
                    foreach (object o in items)
                    {
                        if (object.ReferenceEquals(o, adjacentRingA))
                            return true;
                        if (visited.Add(o))
                        {
                            more = true;
                            newActives.Add(o);
                        }
                    }
                }
                actives = newActives;
                newActives.Clear();
            }
            return false;
        }

        internal static bool AnyIntersections(PlanarGraph<double> graph)
        {
            if (graph == null)
                return false;
            SegmentGroup<double> activeEdges = new SegmentGroup<double>();
            IEnumerator<Node<double>> points = graph.GetEnumerator(); //walk through the points in xy sorted order

            Node<double> nd;
            LeakyResizableArray<Edge<double>> localEdges;

            int localCt;
            int activeCt = 0;
            Edge<double> localEdge;
            LineIntersectionResult<double> intersects;
            Point2<double> localStart;
            Point2<double> localEnd;
            Point2<double> activeStart;
            Point2<double> activeEnd;

            //new point event in the moving front
            while (points.MoveNext())
            {
                nd = points.Current;
                localEdges = nd.Edges; //edges connected to this point

                localCt = (int)localEdges.Count;

                //compute intersections with other edges in the scan area
                for (int i = 0; i < localCt; i++)
                {
                    localEdge = localEdges.Data[i];
                    localStart = localEdge.Start.Point;
                    localEnd = localEdge.End.Point;
                    activeCt = activeEdges.Edges.Count;

                    foreach (Edge<double> activeEdge in activeEdges)
                    {
                        if (object.ReferenceEquals(localEdge, activeEdge))
                            continue; //can't have a "full" match -- this is an exiting segment
                        activeStart = activeEdge.Start.Point;
                        activeEnd = activeEdge.End.Point;

                        intersects = Coordinate2Utils.GetIntersection(localStart.X, localStart.Y, localEnd.X, localEnd.Y, activeStart.X, activeStart.Y, activeEnd.X, activeEnd.Y);
                        if (intersects.IntersectionType != LineIntersectionType.NoIntersection)
                        {
                            if (intersects.IntersectionType == LineIntersectionType.CollinearIntersection)
                                return true; // there's a full segment of intersection
                            if (object.ReferenceEquals(localEdge.Previous, activeEdge) || object.ReferenceEquals(localEdge.Next, activeEdge))
                                continue; // this intersection is a single point at the common point of adjacent segments in a chain/ring
                            return true;
                        }
                    }
                }
                //remove all exiting segments and add all starting segments
                //Action gets called exactly twice per edge -- once to add it, once to remove it
                for (int i = 0; i < localCt; i++)
                {
                    activeEdges.Action(localEdges.Data[i]);
                }
            }
            return false;
        }

        internal static bool AnyIntersections(PlanarChainGraph<double> graph)
        {
            if (graph == null)
                return false;
            SegmentGroup<double> activeEdges = new SegmentGroup<double>();
            IEnumerator<Node<double>> points = graph.GetEnumerator(); //walk through the points in xy sorted order

            Node<double> nd;
            LeakyResizableArray<Edge<double>> localEdges;

            int localCt;
            int activeCt = 0;
            Edge<double> localEdge;
            LineIntersectionResult<double> intersects;
            Point2<double> localStart;
            Point2<double> localEnd;
            Point2<double> activeStart;
            Point2<double> activeEnd;

            //new point event in the moving front
            while (points.MoveNext())
            {
                nd = points.Current;
                localEdges = nd.Edges; //edges connected to this point

                localCt = (int)localEdges.Count;

                //compute intersections with other edges in the scan area
                for (int i = 0; i < localCt; i++)
                {
                    localEdge = localEdges.Data[i];
                    localStart = localEdge.Start.Point;
                    localEnd = localEdge.End.Point;
                    activeCt = activeEdges.Edges.Count;

                    foreach (Edge<double> activeEdge in activeEdges.Edges)
                    {
                        if (object.ReferenceEquals(localEdge, activeEdge))
                            continue; //can't have a "full" match -- this is an exiting segment
                        activeStart = activeEdge.Start.Point;
                        activeEnd = activeEdge.End.Point;

                        intersects = Coordinate2Utils.GetIntersection(localStart.X, localStart.Y, localEnd.X, localEnd.Y, activeStart.X, activeStart.Y, activeEnd.X, activeEnd.Y);
                        if (intersects.IntersectionType != LineIntersectionType.NoIntersection)
                        {
                            if (object.ReferenceEquals(localEdge.Previous, activeEdge) || object.ReferenceEquals(localEdge.Next, activeEdge))
                                continue; // this is adjacent segments in a chain/ring
                            return true;
                        }
                    }
                }
                //remove all exiting segments and add all starting segments
                //Action gets called exactly twice per edge -- once to add it, once to remove it
                for (int i = 0; i < localCt; i++)
                {
                    activeEdges.Action(localEdges.Data[i]);
                }
            }
            return false;
        }
    }
}
