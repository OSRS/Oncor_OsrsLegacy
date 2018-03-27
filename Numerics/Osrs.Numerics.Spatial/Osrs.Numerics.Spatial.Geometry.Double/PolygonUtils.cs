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

using System.Collections.Generic;

namespace Osrs.Numerics.Spatial.Geometry
{
    public static class PolygonUtils
    {
        #region Containment
        //point is inside the ring - not on the edge
        public static bool Contains(Polygon2<double> ring, Point2<double> pt)
        {
            if (ring == null || pt == null)
                return false;

            if (ring.HasHoles)
            {
                if (RingUtils.Contains(ring.OuterRing, pt))
                {
                    for (int i = 0; i < ring.InnerRings.Rings.Length; i++)
                    {
                        if (RingUtils.Contains(ring.InnerRings.Rings[i], pt))
                            return false; //in a hole
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return RingUtils.Contains(ring.OuterRing, pt);
        }

        public static bool Contains(Polygon2<double> outer, Polygon2<double> inner)
        {
            if (outer == null || inner == null)
                return false;

            if (outer.HasHoles)
            {
                if (RingUtils.Contains(outer.OuterRing, inner.OuterRing))
                {
                    for (int i = 0; i < outer.InnerRings.Rings.Length; i++)
                    {
                        if (RingUtils.Contains(outer.InnerRings.Rings[i], inner.OuterRing))
                            return false; //in a hole
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return RingUtils.Contains(outer.OuterRing, inner.OuterRing);
        }

        //is the "inner" (point) inside or on the "outer" (ring)
        //point is in or on the ring
        public static bool ContainsByArea(Polygon2<double> ring, Point2<double> pt)
        {
            if (ring == null || pt == null)
                return false;

            return RingUtils.ContainsByArea(ring.OuterRing, pt);
        }

        //is the "inner" (ring) inside the "outer" (ring)
        //matches check for polygon holes, so single shared points on edge are ok, they share no area nor segment
        public static bool ContainsByArea(Polygon2<double> outer, Polygon2<double> inner)
        {
            if (outer == null || inner == null)
                return false;

            return RingUtils.ContainsByArea(outer.OuterRing, inner.OuterRing);
        }

        //is the point inside the ring regardless of inclusive/exclusive ring orientation
        private const double mX = double.MaxValue - 1.0d;
        public static bool PointInteriorToPolygon(Polygon2<double> ring, Point2<double> pt)
        {
            if (ring == null || pt == null)
                return false;

            if (ring.HasHoles)
            {
                if (RingUtils.PointInteriorToRing(ring.OuterRing, pt))
                {
                    for (int i = 0; i < ring.InnerRings.Rings.Length; i++)
                    {
                        if (RingUtils.PointOnOrInteriorToRing(ring.InnerRings.Rings[i], pt))
                            return false; //in a hole
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return RingUtils.PointInteriorToRing(ring.OuterRing, pt);
        }

        //is the point inside the ring regardless of inclusive/exclusive ring orientation
        public static bool PointOnOrInteriorToPolygon(Polygon2<double> ring, Point2<double> pt)
        {
            if (ring == null || pt == null)
                return false;

            if (ring.HasHoles)
            {
                if (RingUtils.PointOnOrInteriorToRing(ring.OuterRing, pt))
                {
                    for (int i = 0; i < ring.InnerRings.Rings.Length; i++)
                    {
                        if (RingUtils.PointInteriorToRing(ring.InnerRings.Rings[i], pt))
                            return false; //in a hole
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return RingUtils.PointOnOrInteriorToRing(ring.OuterRing, pt);
        }
        #endregion

        public static Polygon2<double> InvertExtent(PolygonSet2<double> polys)
        {
            if (polys != null)
            {
                Ring2<double> outer = polys.Factory.ConstructRing(polys.Envelope);
                Ring2<double>[] inners = new Ring2<double>[polys.Polygons.Length];
                for (int i = 0; i < inners.Length; i++)
                {
                    inners[i] = polys.Polygons[i].OuterRing;
                }
                RingSet2<double> innerSet = polys.Factory.ConstructRingSet(inners);
                if (innerSet == null)
                    return null;
                return polys.Factory.ConstructPolygon(outer, innerSet);
            }
            return null;
        }

        public static Polygon2<double> Simplify(double minSegmentLength, Polygon2<double> poly)
        {
            if (poly == null)
                return null;

            Ring2<double> outer = RingUtils.Simplify(minSegmentLength, poly.OuterRing);
            if (outer == null)
                return null;

            if (poly.HasHoles)
            {
                List<Ring2<double>> inners = new List<Ring2<double>>();
                Ring2<double> curInner;
                double distSum;
                double minDist = 3.0 * minSegmentLength;
                for (int i = 0; i < poly.InnerRings.Rings.Length; i++)
                {
                    curInner = poly.InnerRings.Rings[i];

                    //quick reject if the ring is "small"
                    distSum = 0;
                    for (uint j = 1; j < curInner.VertexCount; j++)
                    {
                        distSum += SegmentUtils.Length(curInner[j - 1], curInner[j]);
                        if (distSum >= minDist) //quick exit for big rings
                            break;
                    }
                    distSum += SegmentUtils.Length(curInner[curInner.VertexCount - 1], curInner[0]);
                    if (distSum <= minDist)
                        continue; //can't exist as a ring with that small a boundary

                    curInner = RingUtils.Simplify(minSegmentLength, curInner);
                    if (curInner != null)
                        inners.Add(curInner);
                }

                if (inners.Count < 1)
                    return outer; //no holes came through
                RingSet2<double> rs = poly.Factory.ConstructRingSet(inners);
                if (rs == null)
                    return null;
                return poly.Factory.ConstructPolygon(outer, rs);
            }
            else
                return outer;
        }

        public static List<Polygon2<double>> CleanSimplePolygonPoints(Point2<double>[] outerRingPoints, Ring2<double>[] innerRings)
        {
            List<Ring2<double>> rings = RingUtils.CleanSimpleRingPoints(outerRingPoints);
            if (rings == null || rings.Count < 1)
                return null;

            List<Polygon2<double>> res = new List<Polygon2<double>>();
            Polygon2<double> tmpP;

            if (rings.Count == 1)
            {
                tmpP = rings[0].Factory.ConstructPolygon(rings[0], innerRings);
                if (tmpP == null)
                    return null;
                res.Add(tmpP);
            }
            else
            {
                List<Ring2<double>> inRings = new List<Ring2<double>>();
                foreach (Ring2<double> curRing in rings)
                {
                    if (curRing == null)
                        continue;
                    inRings.Clear();
                    foreach (Ring2<double> curHole in innerRings)
                    {
                        if (RingUtils.PointInteriorToRing(curRing, curHole.Points[0]) || RingUtils.PointInteriorToRing(curRing, curHole.Points[1]))
                            inRings.Add(curHole);
                    }
                    if (inRings.Count < 1)
                        res.Add((Polygon2<double>)curRing);
                    else
                    {
                        tmpP = curRing.Factory.ConstructPolygon(curRing, inRings.ToArray());
                        if (tmpP == null)
                            return null;
                        res.Add(tmpP);
                    }
                }
            }

            return res;
        }

        public static Polygon2<double> OpenSimplePolygonPoints(Point2<double>[] outerRingPoints, Ring2<double>[] innerRings)
        {
            Ring2<double> ring = RingUtils.OpenSimpleRingPoints(outerRingPoints);
            if (ring == null)
                return null;

            Polygon2<double> tmpP;

            tmpP = ring.Factory.ConstructPolygon(ring, innerRings);
            return tmpP;
        }
    }
}
