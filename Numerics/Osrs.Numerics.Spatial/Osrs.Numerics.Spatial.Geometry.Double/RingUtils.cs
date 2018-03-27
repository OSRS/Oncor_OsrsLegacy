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
using Osrs.Numerics.Spatial.Coordinates;
using Osrs.Numerics.Spatial.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Osrs.Numerics.Spatial.Geometry
{
    public static class RingUtils
    {
        private static readonly GeometryFactory2Double factory = GeometryFactory2Double.Instance;
        #region Containment
        //point is inside the ring - not on the edge
        public static bool Contains(Ring2<double> ring, Point2<double> pt)
        {
            if (ring == null || pt == null)
                return false;
            if (Envelope2<double>.PointInside(ring.Envelope, pt.X, pt.Y)) //quick envelope reject
            {
                Point2<double> curPt;
                Point2<double>[] ringPoints = ring.Points;
                double maxX;
                if (ring.Envelope.MaxX < mX)
                    maxX = ring.Envelope.MaxX + 1.0d;
                else
                    maxX = double.MaxValue;

                Point2<double> fromPt;
                Point2<double> toPt = ringPoints[0];
                int intersectCount = 0;
                LineIntersectionResult<double> hits;
                for (int i = 1; i < ringPoints.Length; i++)
                {
                    fromPt = toPt;
                    toPt = ringPoints[i];
                    if ((fromPt.Y <= pt.Y && toPt.Y >= pt.Y) || (fromPt.Y >= pt.Y && toPt.Y <= pt.Y)) //envelope intersection
                    {
                        if (fromPt.X < pt.X && toPt.X < pt.X)
                            continue; //almost an intersection, but env is "left" of the ray
                        hits = Coordinate2Utils.GetIntersection(fromPt.X, fromPt.Y, toPt.X, toPt.Y, pt.X, pt.Y, maxX, pt.Y);
                        if (hits.IntersectionType == LineIntersectionType.PointIntersection) //have to check if this is an endpoint
                        {
                            if (PointUtilsDouble.PointsEqual(pt, hits.BasePoint))
                                return false; // source point is on the edge
                            if (PointUtilsDouble.PointsEqual(hits.BasePoint, fromPt))
                            {
                                curPt = ringPoints[i - 1];
                                if (curPt.Y > pt.Y && toPt.Y > pt.Y) //previous and next points to intersect point are above ray
                                {
                                    intersectCount++;
                                }
                                else if (curPt.Y < pt.Y && toPt.Y < pt.Y) //previous and next points to intersect point are below ray - don't add to intersection count
                                { /*do nothing*/ }
                                else
                                    intersectCount++;
                            }
                            else if (PointUtilsDouble.PointsEqual(hits.BasePoint, toPt))
                            {
                                int id = i + 1;
                                if (ringPoints.Length < id)
                                    curPt = ringPoints[id];
                                else
                                    curPt = ringPoints[0]; //wrap around
                            }
                            else
                                intersectCount++; //ray crosses in the middle of the segment
                        }
                    }
                }
                //handle last segment
                fromPt = toPt;
                toPt = ringPoints[0];
                if ((fromPt.Y <= pt.Y && toPt.Y >= pt.Y) || (fromPt.Y >= pt.Y && toPt.Y <= pt.Y)) //envelope intersection
                {
                    if (!(fromPt.X < pt.X && toPt.X < pt.X)) //almost an intersection, but env is "left" of the ray
                    {
                        hits = Coordinate2Utils.GetIntersection(fromPt.X, fromPt.Y, toPt.X, toPt.Y, pt.X, pt.Y, maxX, pt.Y);
                        if (hits.IntersectionType == LineIntersectionType.PointIntersection) //have to check if this is an endpoint
                        {
                            if (PointUtilsDouble.PointsEqual(pt, hits.BasePoint))
                                return false; // source point is on the edge
                            if (PointUtilsDouble.PointsEqual(hits.BasePoint, fromPt))
                            {
                                curPt = ringPoints[ringPoints.Length - 2];
                                if (curPt.Y > pt.Y && toPt.Y > pt.Y) //previous and next points to intersect point are above ray
                                {
                                    intersectCount++;
                                }
                                else if (curPt.Y < pt.Y && toPt.Y < pt.Y) //previous and next points to intersect point are below ray - don't add to intersection count
                                { /*do nothing*/ }
                                else
                                    intersectCount++;
                            }
                            else if (PointUtilsDouble.PointsEqual(hits.BasePoint, toPt))
                            { /*do nothing*/ }
                            else
                                intersectCount++; //ray crosses in the middle of the segment
                        }
                    }
                }
                if (intersectCount % 2 == 0) //point is not on the interior of the ring
                    return ring.Orientation != Orientation.Counterclockwise; //means "inside" of ring is inside (this is not a hole)
                return ring.Orientation == Orientation.Clockwise; //this is a hole, and the point is "outside" the hole, which is the "inside" of the ring
            }
            return false;
        }

        public static bool Contains(Ring2<double> outer, Ring2<double> inner)
        {
            if (outer == null || inner == null)
                return false;

            if (Envelope2<double>.EnvelopeOverlap(outer.Envelope, inner.Envelope))
            {
                PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
                graph.Add(outer);
                graph.Add(inner);

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
                            if (object.ReferenceEquals(localEdge.Start.ParentShape, activeEdge.Start.ParentShape))
                                continue; //2 edges on same ring
                            activeStart = activeEdge.Start.Point;
                            activeEnd = activeEdge.End.Point;

                            intersects = Coordinate2Utils.GetIntersection(localStart.X, localStart.Y, localEnd.X, localEnd.Y, activeStart.X, activeStart.Y, activeEnd.X, activeEnd.Y);
                            if (intersects.IntersectionType != LineIntersectionType.NoIntersection)
                                return false;
                        }
                    }
                    //remove all exiting segments and add all starting segments
                    //Action gets called exactly twice per edge -- once to add it, once to remove it
                    for (int i = 0; i < localCt; i++)
                        activeEdges.Action(localEdges.Data[i]);
                }
                //now we know that the rings don't overlap, determine if the ring is on the inside
                if (Contains(outer, inner.Points[0]))
                    return outer.Orientation == Orientation.Counterclockwise; //point on interior of ring, is ring interior "inside" of ring (or is ring inside out - a hole)
                return outer.Orientation == Orientation.Clockwise;
            }
            return false;
        }

        //is the "inner" (point) inside or on the "outer" (ring)
        //point is in or on the ring
        public static bool ContainsByArea(Ring2<double> ring, Point2<double> pt)
        {
            if (ring == null || pt == null)
                return false;
            if (Envelope2<double>.PointInside(ring.Envelope, pt.X, pt.Y)) //quick envelope reject
            {
                Point2<double> curPt;
                Point2<double>[] ringPoints = ring.Points;
                double maxX;
                if (ring.Envelope.MaxX < mX)
                    maxX = ring.Envelope.MaxX + 1.0d;
                else
                    maxX = double.MaxValue;

                Point2<double> fromPt;
                Point2<double> toPt = ringPoints[0];
                int intersectCount = 0;
                LineIntersectionResult<double> hits;
                for (int i = 1; i < ringPoints.Length; i++)
                {
                    fromPt = toPt;
                    toPt = ringPoints[i];
                    if ((fromPt.Y <= pt.Y && toPt.Y >= pt.Y) || (fromPt.Y >= pt.Y && toPt.Y <= pt.Y)) //envelope intersection
                    {
                        if (fromPt.X < pt.X && toPt.X < pt.X)
                            continue; //almost an intersection, but env is "left" of the ray
                        hits = Coordinate2Utils.GetIntersection(fromPt.X, fromPt.Y, toPt.X, toPt.Y, pt.X, pt.Y, maxX, pt.Y);
                        if (hits.IntersectionType == LineIntersectionType.PointIntersection) //have to check if this is an endpoint
                        {
                            if (PointUtilsDouble.PointsEqual(pt, hits.BasePoint))
                                return true; // source point is on the edge
                            if (PointUtilsDouble.PointsEqual(hits.BasePoint, fromPt))
                            {
                                curPt = ringPoints[i - 1];
                                if (curPt.Y > pt.Y && toPt.Y > pt.Y) //previous and next points to intersect point are above ray
                                {
                                    intersectCount++;
                                }
                                else if (curPt.Y < pt.Y && toPt.Y < pt.Y) //previous and next points to intersect point are below ray - don't add to intersection count
                                { /*do nothing*/ }
                                else
                                    intersectCount++;
                            }
                            else if (PointUtilsDouble.PointsEqual(hits.BasePoint, toPt))
                            {
                                int id = i + 1;
                                if (ringPoints.Length < id)
                                    curPt = ringPoints[id];
                                else
                                    curPt = ringPoints[0]; //wrap around
                            }
                            else
                                intersectCount++; //ray crosses in the middle of the segment
                        }
                    }
                }

                //handle last segment
                fromPt = toPt;
                toPt = ringPoints[0];
                if ((fromPt.Y <= pt.Y && toPt.Y >= pt.Y) || (fromPt.Y >= pt.Y && toPt.Y <= pt.Y)) //envelope intersection
                {
                    if (!(fromPt.X < pt.X && toPt.X < pt.X)) //almost an intersection, but env is "left" of the ray
                    {
                        hits = Coordinate2Utils.GetIntersection(fromPt.X, fromPt.Y, toPt.X, toPt.Y, pt.X, pt.Y, maxX, pt.Y);
                        if (hits.IntersectionType == LineIntersectionType.PointIntersection) //have to check if this is an endpoint
                        {
                            if (PointUtilsDouble.PointsEqual(pt, hits.BasePoint))
                                return true; // source point is on the edge
                            if (PointUtilsDouble.PointsEqual(hits.BasePoint, fromPt))
                            {
                                curPt = ringPoints[ringPoints.Length - 2];
                                if (curPt.Y > pt.Y && toPt.Y > pt.Y) //previous and next points to intersect point are above ray
                                {
                                    intersectCount++;
                                }
                                else if (curPt.Y < pt.Y && toPt.Y < pt.Y) //previous and next points to intersect point are below ray - don't add to intersection count
                                { /*do nothing*/ }
                                else
                                    intersectCount++;
                            }
                            else if (PointUtilsDouble.PointsEqual(hits.BasePoint, toPt))
                            { /*do nothing*/ }
                            else
                                intersectCount++; //ray crosses in the middle of the segment
                        }
                    }
                }
                if (intersectCount % 2 == 0) //point is not on the interior of the ring
                    return ring.Orientation != Orientation.Counterclockwise; //means "inside" of ring is inside (this is not a hole)
                return ring.Orientation == Orientation.Clockwise; //this is a hole, and the point is "outside" the hole, which is the "inside" of the ring
            }
            return false;
        }

        //is the "inner" (ring) inside the "outer" (ring)
        //matches check for polygon holes, so single shared points on edge are ok, they share no area nor segment
        public static bool ContainsByArea(Ring2<double> outer, Ring2<double> inner)
        {
            if (outer == null || inner == null)
                return false;
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            graph.Add(outer);
            graph.Add(inner);

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
            bool pointOnEdgeFound = false; //point from inner ring can only touch outer ring at 1 location, otherwise outer ring is really 2 rings with no hole

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
                        if (object.ReferenceEquals(localEdge.Start.ParentShape, activeEdge.Start.ParentShape))
                            continue; //2 edges on same ring
                        activeStart = activeEdge.Start.Point;
                        activeEnd = activeEdge.End.Point;

                        intersects = Coordinate2Utils.GetIntersection(localStart.X, localStart.Y, localEnd.X, localEnd.Y, activeStart.X, activeStart.Y, activeEnd.X, activeEnd.Y);
                        if (intersects.IntersectionType != LineIntersectionType.NoIntersection)
                        {
                            if (intersects.IntersectionType == LineIntersectionType.CollinearIntersection)
                                return false; // there's a full segment of intersection
                            if (object.ReferenceEquals(localEdge.Previous, activeEdge) || object.ReferenceEquals(localEdge.Next, activeEdge))
                                continue; // this intersection is a single point at the common point of adjacent segments in a chain/ring
                            //we have a point intersection, and this is not on the same ring -- this is ok, since there is no common "length"
                            if (pointOnEdgeFound)
                                return false; //2 intersecting points
                            pointOnEdgeFound = true;
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
            //now we know that the rings don't overlap, determine if the ring is on the inside
            if (RingUtils.Contains(outer, inner.Points[0]) || RingUtils.Contains(outer, inner.Points[1]))
                return true;
            return false;
        }

        //is the point inside the ring regardless of inclusive/exclusive ring orientation
        private const double mX = double.MaxValue - 1.0d;
        public static bool PointInteriorToRing(Ring2<double> ring, Point2<double> pt)
        {
            if (ring == null || pt == null)
                return false;
            if (Envelope2<double>.PointInside(ring.Envelope, pt.X, pt.Y)) //quick envelope reject
            {
                Point2<double> curPt;
                Point2<double>[] ringPoints = ring.Points;
                double maxX;
                if (ring.Envelope.MaxX < mX)
                    maxX = ring.Envelope.MaxX + 1.0d;
                else
                    maxX = double.MaxValue;

                Point2<double> fromPt;
                Point2<double> toPt = ringPoints[0];
                int intersectCount = 0;
                LineIntersectionResult<double> hits;
                for (int i = 1; i < ringPoints.Length; i++)
                {
                    fromPt = toPt;
                    toPt = ringPoints[i];
                    if ((fromPt.Y <= pt.Y && toPt.Y >= pt.Y) || (fromPt.Y >= pt.Y && toPt.Y <= pt.Y)) //envelope intersection
                    {
                        if (fromPt.X < pt.X && toPt.X < pt.X)
                            continue; //almost an intersection, but env is "left" of the ray
                        hits = Coordinate2Utils.GetIntersection(fromPt.X, fromPt.Y, toPt.X, toPt.Y, pt.X, pt.Y, maxX, pt.Y);
                        if (hits.IntersectionType == LineIntersectionType.PointIntersection) //have to check if this is an endpoint
                        {
                            if (PointUtilsDouble.PointsEqual(pt, hits.BasePoint))
                                return false; // source point is on the edge
                            if (PointUtilsDouble.PointsEqual(hits.BasePoint, fromPt))
                            {
                                curPt = ringPoints[i - 1];
                                if (curPt.Y > pt.Y && toPt.Y > pt.Y) //previous and next points to intersect point are above ray
                                {
                                    intersectCount++;
                                }
                                else if (curPt.Y < pt.Y && toPt.Y < pt.Y) //previous and next points to intersect point are below ray - don't add to intersection count
                                { /*do nothing*/ }
                                else
                                    intersectCount++;
                            }
                            else if (PointUtilsDouble.PointsEqual(hits.BasePoint, toPt))
                            {
                                int id = i + 1;
                                if (ringPoints.Length < id)
                                    curPt = ringPoints[id];
                                else
                                    curPt = ringPoints[0]; //wrap around
                            }
                            else
                                intersectCount++; //ray crosses in the middle of the segment
                        }
                    }
                }
                //handle last segment
                fromPt = toPt;
                toPt = ringPoints[0];
                if ((fromPt.Y <= pt.Y && toPt.Y >= pt.Y) || (fromPt.Y >= pt.Y && toPt.Y <= pt.Y)) //envelope intersection
                {
                    if (!(fromPt.X < pt.X && toPt.X < pt.X)) //almost an intersection, but env is "left" of the ray
                    {
                        hits = Coordinate2Utils.GetIntersection(fromPt.X, fromPt.Y, toPt.X, toPt.Y, pt.X, pt.Y, maxX, pt.Y);
                        if (hits.IntersectionType == LineIntersectionType.PointIntersection) //have to check if this is an endpoint
                        {
                            if (PointUtilsDouble.PointsEqual(pt, hits.BasePoint))
                                return false; // source point is on the edge
                            if (PointUtilsDouble.PointsEqual(hits.BasePoint, fromPt))
                            {
                                curPt = ringPoints[ringPoints.Length - 2];
                                if (curPt.Y > pt.Y && toPt.Y > pt.Y) //previous and next points to intersect point are above ray
                                {
                                    intersectCount++;
                                }
                                else if (curPt.Y < pt.Y && toPt.Y < pt.Y) //previous and next points to intersect point are below ray - don't add to intersection count
                                { /*do nothing*/ }
                                else
                                    intersectCount++;
                            }
                            else if (PointUtilsDouble.PointsEqual(hits.BasePoint, toPt))
                            { /*do nothing*/ }
                            else
                                intersectCount++; //ray crosses in the middle of the segment
                        }
                    }
                }

                return intersectCount % 2 == 1; //point is not on the interior of the ring if intersection count is odd
            }
            return false;
        }

        //is the point inside the ring regardless of inclusive/exclusive ring orientation
        public static bool PointOnOrInteriorToRing(Ring2<double> ring, Point2<double> pt)
        {
            if (ring == null || pt == null)
                return false;
            if (Envelope2<double>.PointInside(ring.Envelope, pt.X, pt.Y)) //quick envelope reject
            {
                Point2<double> curPt;
                Point2<double>[] ringPoints = ring.Points;
                double maxX;
                if (ring.Envelope.MaxX < mX)
                    maxX = ring.Envelope.MaxX + 1.0d;
                else
                    maxX = double.MaxValue;

                Point2<double> fromPt;
                Point2<double> toPt = ringPoints[0];
                int intersectCount = 0;
                LineIntersectionResult<double> hits;
                for (int i = 1; i < ringPoints.Length; i++)
                {
                    fromPt = toPt;
                    toPt = ringPoints[i];
                    if ((fromPt.Y <= pt.Y && toPt.Y >= pt.Y) || (fromPt.Y >= pt.Y && toPt.Y <= pt.Y)) //envelope intersection
                    {
                        if (fromPt.X < pt.X && toPt.X < pt.X)
                            continue; //almost an intersection, but env is "left" of the ray
                        hits = Coordinate2Utils.GetIntersection(fromPt.X, fromPt.Y, toPt.X, toPt.Y, pt.X, pt.Y, maxX, pt.Y);
                        if (hits.IntersectionType == LineIntersectionType.PointIntersection) //have to check if this is an endpoint
                        {
                            if (PointUtilsDouble.PointsEqual(pt, hits.BasePoint))
                                return true; // source point is on the edge
                            if (PointUtilsDouble.PointsEqual(hits.BasePoint, fromPt))
                            {
                                curPt = ringPoints[i - 1];
                                if (curPt.Y > pt.Y && toPt.Y > pt.Y) //previous and next points to intersect point are above ray
                                {
                                    intersectCount++;
                                }
                                else if (curPt.Y < pt.Y && toPt.Y < pt.Y) //previous and next points to intersect point are below ray - don't add to intersection count
                                { /*do nothing*/ }
                                else
                                    intersectCount++;
                            }
                            else if (PointUtilsDouble.PointsEqual(hits.BasePoint, toPt))
                            {
                                int id = i + 1;
                                if (ringPoints.Length < id)
                                    curPt = ringPoints[id];
                                else
                                    curPt = ringPoints[0]; //wrap around
                            }
                            else
                                intersectCount++; //ray crosses in the middle of the segment
                        }
                    }
                }

                //handle last segment
                fromPt = toPt;
                toPt = ringPoints[0];
                if ((fromPt.Y <= pt.Y && toPt.Y >= pt.Y) || (fromPt.Y >= pt.Y && toPt.Y <= pt.Y)) //envelope intersection
                {
                    if (!(fromPt.X < pt.X && toPt.X < pt.X)) //almost an intersection, but env is "left" of the ray
                    {
                        hits = Coordinate2Utils.GetIntersection(fromPt.X, fromPt.Y, toPt.X, toPt.Y, pt.X, pt.Y, maxX, pt.Y);
                        if (hits.IntersectionType == LineIntersectionType.PointIntersection) //have to check if this is an endpoint
                        {
                            if (PointUtilsDouble.PointsEqual(pt, hits.BasePoint))
                                return true; // source point is on the edge
                            if (PointUtilsDouble.PointsEqual(hits.BasePoint, fromPt))
                            {
                                curPt = ringPoints[ringPoints.Length - 2];
                                if (curPt.Y > pt.Y && toPt.Y > pt.Y) //previous and next points to intersect point are above ray
                                {
                                    intersectCount++;
                                }
                                else if (curPt.Y < pt.Y && toPt.Y < pt.Y) //previous and next points to intersect point are below ray - don't add to intersection count
                                { /*do nothing*/ }
                                else
                                    intersectCount++;
                            }
                            else if (PointUtilsDouble.PointsEqual(hits.BasePoint, toPt))
                            { /*do nothing*/ }
                            else
                                intersectCount++; //ray crosses in the middle of the segment
                        }
                    }
                }
                return intersectCount % 2 == 1; //point is not on the interior of the ring if intersection count is odd
            }
            return false;
        }
        #endregion

        public static Ring2<double> Simplify(double minSegmentLength, Ring2<double> sourceRing)
        {
            if (sourceRing == null || minSegmentLength <= 0.0)
                return null;

            if (sourceRing.VertexCount < 4)
                return sourceRing; //can't simplify without destroying ring

            RingPointLinkedList<double> ptList = new RingPointLinkedList<double>(sourceRing.Points);
            RingPointLinkedList<double>.RingPointLinkedListNode curNode = ptList.RootNode;
            bool anyRemoved = true;

            Point2<double> p0;
            Point2<double> p1;

            while (anyRemoved)
            {
                anyRemoved = false;
                p1 = curNode.Point; //root point
                curNode = curNode.Next;
                while (!object.ReferenceEquals(curNode, ptList.RootNode))
                {
                    p0 = p1;
                    p1 = curNode.Point;
                    if (SegmentUtils.Length(p0, p1) < minSegmentLength)
                    {
                        ptList.Remove(curNode.Previous);
                        if (ptList.Count < 4) //need at least 3 points to preserve a ring
                        {
                            anyRemoved = false;
                            break;
                        }
                        anyRemoved = true;
                        p1 = curNode.Next.Point;
                        curNode = curNode.Next; //ensure we don't remove adjacent points on one round of removals
                    }
                    curNode = curNode.Next;
                }
            }

            Point2<double>[] pts = new Point2<double>[ptList.Count];
            curNode = ptList.RootNode;
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = curNode.Point;
                curNode = curNode.Next;
            }

            Ring2<double> result = factory.ConstructRing(pts);
            //TODO deal with bad geometry issues IF they arise
            if (result == null)
                return null; //resulted in bad geometry
            return new Ring2<double>(pts, sourceRing.IsReversed, sourceRing.Orientation);
        }

        #region ErrorCorrection
        //TODO -- FINISH THIS METHOD
        /// <summary>
        /// Takes an ordered array of points intended to be a ring and cleans the points to make one or more legal rings.
        /// If there are intersection points, the ring is split into multiple rings as needed.
        /// The resulting rings are returned as a RingSet.
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <returns>A list of point arrays.  Each point array is a legal ring</returns>
        public static RingSet2<double> CleanRingPoints(Point2<double>[] sourcePoints)
        {
            if (sourcePoints == null || sourcePoints.Length < 3) //not enough to be a ring
                return null;

            sourcePoints = PointUtils.RemoveAdjacentDups(sourcePoints, true); //from here on, sourcePoints is a new array

            if (sourcePoints == null || sourcePoints.Length < 3) //not enough to be a ring
                return null;

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            graph.Add(sourcePoints, true);

            List<int> segIntersectStartPointA = new List<int>();
            List<int> segIntersectStartPointB = new List<int>();
            List<Point2<double>> segIntersectPoint = new List<Point2<double>>();
            List<bool> segIntersectionCollinear = new List<bool>();
            bool anySharedSegments = false;

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

                            segIntersectPoint.Add(factory.ConstructPoint(intersects.BasePoint.X, intersects.BasePoint.Y));
                            segIntersectionCollinear.Add(intersects.IntersectionType == LineIntersectionType.CollinearIntersection);
                            anySharedSegments = anySharedSegments || intersects.IntersectionType == LineIntersectionType.CollinearIntersection;

                            //figure out which segments are involved by start points
                            for (int ptIndex = 0; ptIndex < sourcePoints.Length; ptIndex++)
                            {
                                if (sourcePoints[ptIndex] == localStart)
                                {
                                    segIntersectStartPointA.Add(ptIndex);
                                    if (segIntersectionCollinear.Count == segIntersectStartPointA.Count && segIntersectionCollinear.Count == segIntersectStartPointB.Count)
                                        break;
                                    continue;
                                }
                                else if (sourcePoints[ptIndex] == activeStart)
                                {
                                    segIntersectStartPointB.Add(ptIndex);
                                    if (segIntersectionCollinear.Count == segIntersectStartPointA.Count && segIntersectionCollinear.Count == segIntersectStartPointB.Count)
                                        break;
                                    continue;
                                }
                            }
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

            if (segIntersectPoint.Count < 1) //no intersections detected, so we're already good
            {
                Orientation o = Orientation.Clockwise;
                if (PointUtilsDouble.IsCCW(sourcePoints))
                    o = Orientation.Counterclockwise;
                return new Ring2<double>(sourcePoints, false, o); //implicitly converted to a RingSet2
            }

            //ok, at this point we have detected all intersections and we can proceed accordingly to build the rings

            if (anySharedSegments) //this is the worst case, we have to "merge" partial rings that have shared segements
            {
                //at a shared segement
            }


            return null;
        }

        public static List<Ring2<double>> CheckAndCleanSimpleRingPoints(Point2<double>[] sourcePoints)
        {
            if (sourcePoints == null || sourcePoints.Length < 3)
                return null;
            Point2<double>[] pts = PointUtils.RemoveAdjacentDups<double>(sourcePoints, true);
            if (pts.Length < 3)
                return null;

            List<Ring2<double>> rings = new List<Ring2<double>>();

            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            graph.Add(pts, true);
            if (!PlanarGraphUtils.AnyIntersections(graph))
            {
                Orientation o = Orientation.Clockwise;
                if (PointUtilsDouble.IsCCW(pts))
                    o = Orientation.Counterclockwise;
                rings.Add(new Ring2<double>(pts, false, o));
            }
            else
            {
                Ring2<double> rng = OpenSimpleRingPointsImpl(pts);
                if (rng != null)
                    rings.Add(rng);
                else
                    rings = CleanSimpleRingPointsImpl(pts, true);
            }

            return rings;
        }

        /// <summary>
        /// Takes an ordered array of points intended to be a ring and cleans the points to make one or more legal rings.
        /// This method only cleans non-adjacent duplicate points, not crossing segments.  This is generally suitable for legal geometry in less stringent software applications.
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <returns></returns>
        public static List<Ring2<double>> CleanSimpleRingPoints(Point2<double>[] sourcePoints)
        {
            return CleanSimpleRingPoints(sourcePoints, true);
        }

        public static List<Ring2<double>> CleanSimpleRingPoints(Point2<double>[] sourcePoints, bool allOrNothing)
        {
            if (sourcePoints == null || sourcePoints.Length < 3) //not enough to be a ring
                return null;

            sourcePoints = PointUtils.RemoveAdjacentDups(sourcePoints, true); //from here on, sourcePoints is a new array

            if (sourcePoints == null || sourcePoints.Length < 3) //not enough to be a ring
                return null;

            return CleanSimpleRingPointsImpl(sourcePoints, allOrNothing);
        }

        private static List<Ring2<double>> CleanSimpleRingPointsImpl(Point2<double>[] sourcePoints, bool allOrNothing)
        {
            List<Point2<double>[]> ringParts = new List<Point2<double>[]>();
            ringParts.Add(sourcePoints);
            ScanFix(ringParts, 0);

            List<Ring2<double>> rings = new List<Ring2<double>>();
            foreach (Point2<double>[] curPoints in ringParts)
            {
                PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
                graph.Add(curPoints, true);
                if (!PlanarGraphUtils.AnyIntersections(graph))
                {
                    Orientation o = Orientation.Clockwise;
                    if (PointUtilsDouble.IsCCW(curPoints))
                        o = Orientation.Counterclockwise;
                    rings.Add(new Ring2<double>(curPoints, false, o));
                }
                else
                {
                    if (allOrNothing)
                        return null;
                }
            }

            return rings;
        }

        private static void ScanFix(List<Point2<double>[]> ringParts, int index)
        {
            Point2<double>[] sourcePoints = ringParts[index];

            Point2<double> pi;
            Point2<double> pj;

            for (int i = 0; i < sourcePoints.Length - 2; i++)
            {
                pi = sourcePoints[i];
                for (int j = sourcePoints.Length - 1; j > i + 2; j--)
                {
                    pj = sourcePoints[j];

                    if (pi.Equals(pj))
                    {
                        Point2<double>[] ringHigh = new Point2<double>[j - i];
                        Point2<double>[] ringLow = new Point2<double>[sourcePoints.Length - ringHigh.Length];

                        for (int k = 0; k <= i; k++)
                            ringLow[k] = sourcePoints[k];
                        for (int k = 0; k < ringHigh.Length; k++)
                            ringHigh[k] = sourcePoints[i + k];
                        int kj = ringHigh.Length;
                        for (int k = i + 1; k < ringLow.Length; k++)
                            ringLow[k] = sourcePoints[k + kj];

                        ringParts[index] = ringLow; //we've already scanned these points and they are good
                        ringParts.Add(ringHigh);
                        if (ringHigh.Length > 3)
                            ScanFix(ringParts, ringParts.Count - 1);
                    }
                }
            }
        }

        private const double Eps = 0.0000000000000001;
        /// <summary>
        /// Takes an ordered array of points intended to be a ring and cleans the points to make a legal ring.
        /// This method only cleans non-adjacent duplicate points, not crossing segments.  This is generally suitable for legal geometry in less stringent software applications.
        /// The method "moves" one of the duplicated points to open a path into the adjacent region.
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <returns></returns>
        public static Ring2<double> OpenSimpleRingPoints(Point2<double>[] sourcePoints)
        {
            if (sourcePoints == null || sourcePoints.Length < 3) //not enough to be a ring
                return null;

            sourcePoints = PointUtils.RemoveAdjacentDups(sourcePoints, true); //from here on, sourcePoints is a new array

            if (sourcePoints == null || sourcePoints.Length < 3) //not enough to be a ring
                return null;

            return OpenSimpleRingPointsImpl(sourcePoints);
        }

        private static Ring2<double> OpenSimpleRingPointsImpl(Point2<double>[] sourcePoints)
        {
            Point2<double> pi;
            Point2<double> pj;
            Point2<double> mp;
            double dX;
            double dY;

            for (int i = 0; i < sourcePoints.Length - 2; i++)
            {
                pi = sourcePoints[i];
                //we know adjacent points are non-duplicate, so start at i+2
                for (int j = i + 2; j < sourcePoints.Length; j++)
                {
                    pj = sourcePoints[j];
                    if (pi.Equals(pj))
                    {
                        //we have a dup, lets open the ring a bit
                        //we'll move pi toward the point after pi
                        mp = sourcePoints[i + 1];
                        dX = Math.Min(Eps, Math.Abs(mp.X - pi.X));
                        dY = Math.Min(Eps, Math.Abs(mp.Y - pi.Y));
                        if (mp.X > pi.X)
                        {
                            if (mp.Y > pi.Y)
                                sourcePoints[i] = new Point2<double>(pi.X + dX, pi.Y + dY);
                            else if (mp.Y < pi.Y)
                                sourcePoints[i] = new Point2<double>(pi.X + dX, pi.Y - dY);
                            else
                                sourcePoints[i] = new Point2<double>(pi.X + dX, pi.Y);
                        }
                        else if (mp.X < pi.X)
                        {
                            if (mp.Y > pi.Y)
                                sourcePoints[i] = new Point2<double>(pi.X - dX, pi.Y + dY);
                            else if (mp.Y < pi.Y)
                                sourcePoints[i] = new Point2<double>(pi.X - dX, pi.Y - dY);
                            else
                                sourcePoints[i] = new Point2<double>(pi.X - dX, pi.Y);
                        }
                        else
                        {
                            if (mp.Y > pi.Y)
                                sourcePoints[i] = new Point2<double>(pi.X, pi.Y + dY);
                            else if (mp.Y < pi.Y)
                                sourcePoints[i] = new Point2<double>(pi.X, pi.Y - dY);
                            else
                                sourcePoints[i] = new Point2<double>(pi.X, pi.Y);
                        }
                        break; //we'll stop looking at pi and move to the next point - so break from the j loop
                        //Note - this can cause problems in one of 2 cases:
                        //    if moving the point crosses or intersects over another segment because there are VERY close paths
                        //    if there are more points at this same location which happen to be moved to form a crossing
                    }
                }
            }

            //if this returns null, then the fixes didn't complete properly (one of the problems mentioned).
            //May be fixed by calling this method again, but not likely -- we could be in a worse shape than when we started.
            PlanarChainGraph<double> graph = new PlanarChainGraph<double>();
            graph.Add(sourcePoints, true);
            if (!PlanarGraphUtils.AnyIntersections(graph))
            {
                Orientation o = Orientation.Clockwise;
                if (PointUtilsDouble.IsCCW(sourcePoints))
                    o = Orientation.Counterclockwise;
                return new Ring2<double>(sourcePoints, false, o);
            }
            return null;
        }
        #endregion
    }
}
