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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial.Coordinates
{
    public class LineIntersectionResult<T>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly LineIntersectionType IntersectionType;
        public readonly Coordinate2<T> BasePoint;
        public readonly Coordinate2<T> FinalPoint;

        public LineIntersectionResult()
        {
            this.IntersectionType = LineIntersectionType.NoIntersection;
            this.BasePoint = null;
            this.FinalPoint = null;
        }

        public LineIntersectionResult(Coordinate2<T> intersectionPoint)
        {
            if (intersectionPoint != null)
                this.IntersectionType = LineIntersectionType.PointIntersection;
            else
                this.IntersectionType = LineIntersectionType.NoIntersection;
            this.BasePoint = intersectionPoint;
            this.FinalPoint = null;
        }

        public LineIntersectionResult(LineIntersectionType type, Coordinate2<T> basePoint, Coordinate2<T> finalPoint)
        {
            this.IntersectionType = type;
            this.BasePoint = basePoint;
            this.FinalPoint = finalPoint;
        }

        public LineIntersectionResult(Coordinate2<T> basePoint, Coordinate2<T> finalPoint)
        {
            if (basePoint == null)
            {
                if (finalPoint == null)
                {
                    this.IntersectionType = LineIntersectionType.NoIntersection;
                    this.BasePoint = null;
                    this.FinalPoint = null;
                }
                else
                {
                    this.IntersectionType = LineIntersectionType.PointIntersection;
                    this.BasePoint = finalPoint;
                    this.FinalPoint = null;
                }
            }
            else if (finalPoint == null)
            {
                this.IntersectionType = LineIntersectionType.PointIntersection;
                this.BasePoint = basePoint;
                this.FinalPoint = null;
            }
            else
            {
                this.IntersectionType = LineIntersectionType.CollinearIntersection;
                this.BasePoint = basePoint;
                this.FinalPoint = finalPoint;
            }
        }
    }

    //TODO -- Extend for: float, int, long, decimal, biginteger
    public static class Coordinate2Utils
    {
        #region Generic Methods
        public static Envelope2<T> EnvelopeFor<T>(IEnumerable<Coordinate2<T>> points)
            where T : IEquatable<T>, IComparable<T>
        {
            if (points == null)
                return null;
            IEnumerator<Coordinate2<T>> en = points.GetEnumerator();
            if (en.MoveNext())
            {
                Coordinate2<T> cur = en.Current;
                T minX = cur.X;
                T maxX = cur.X;
                T minY = cur.Y;
                T maxY = cur.Y;
                while (en.MoveNext())
                {
                    cur = en.Current;
                    if (minX.CompareTo(cur.X) > 0)
                        minX = cur.X;
                    else if (maxX.CompareTo(cur.X) < 0)
                        maxX = cur.X;
                    if (minY.CompareTo(cur.Y) > 0)
                        minY = cur.Y;
                    else if (maxY.CompareTo(cur.Y) < 0)
                        maxY = cur.Y;
                }
                return new Envelope2<T>(minX, minY, maxX, maxY);
            }
            return null;
        }

        public static Envelope2<T> EnvelopeFor<T>(Coordinate2<T> a, Coordinate2<T> b)
            where T : IEquatable<T>, IComparable<T>
        {
            if (a == null || b == null)
                return null;
            T minX = a.X;
            T maxX = a.X;
            T minY = a.Y;
            T maxY = a.Y;

            if (minX.CompareTo(b.X) > 0)
                minX = b.X;
            else if (maxX.CompareTo(b.X) < 0)
                maxX = b.X;
            if (minY.CompareTo(b.Y) > 0)
                minY = b.Y;
            else if (maxY.CompareTo(b.Y) < 0)
                maxY = b.Y;
            return new Envelope2<T>(minX, minY, maxX, maxY);
        }

        public static Envelope2<T> EnvelopeFor<T>(Coordinate2<T> a, Coordinate2<T> b, Coordinate2<T> c)
            where T : IEquatable<T>, IComparable<T>
        {
            if (a == null || b == null || c == null)
                return null;
            T minX = a.X;
            T maxX = a.X;
            T minY = a.Y;
            T maxY = a.Y;

            if (minX.CompareTo(b.X) > 0)
                minX = b.X;
            else if (maxX.CompareTo(b.X) < 0)
                maxX = b.X;
            if (minY.CompareTo(b.Y) > 0)
                minY = b.Y;
            else if (maxY.CompareTo(b.Y) < 0)
                maxY = b.Y;

            if (minX.CompareTo(c.X) > 0)
                minX = c.X;
            else if (maxX.CompareTo(c.X) < 0)
                maxX = c.X;
            if (minY.CompareTo(c.Y) > 0)
                minY = c.Y;
            else if (maxY.CompareTo(c.Y) < 0)
                maxY = c.Y;
            return new Envelope2<T>(minX, minY, maxX, maxY);
        }

        public static Envelope2<T> EnvelopeFor<T>(Envelope2<T> env, Coordinate2<T> p)
        where T : IEquatable<T>, IComparable<T>
        {
            if (env == null || p == null)
                return null;
            T minX = env.MinX;
            T maxX = env.MaxX;
            T minY = env.MinY;
            T maxY = env.MaxY;

            if (minX.CompareTo(p.X) > 0)
                minX = p.X;
            else if (maxX.CompareTo(p.X) < 0)
                maxX = p.X;
            if (minY.CompareTo(p.Y) > 0)
                minY = p.Y;
            else if (maxY.CompareTo(p.Y) < 0)
                maxY = p.Y;
            return new Envelope2<T>(minX, minY, maxX, maxY);
        }

        //removes all duplicated points
        public static Coordinate2<T>[] RemoveDups<T>(IEnumerable<Coordinate2<T>> points)
            where T : IEquatable<T>, IComparable<T>
        {
            if (points == null)
                return null;
            HashSet<Coordinate2<T>> tmp = new HashSet<Coordinate2<T>>();
            foreach (Coordinate2<T> p in points)
            {
                if (p != null)
                    tmp.Add(p);
            }
            return tmp.ToArray();
        }

        public static Coordinate2<T>[] RemoveAdjacentDups<T>(IEnumerable<Coordinate2<T>> chain)
            where T : IEquatable<T>, IComparable<T>
        {
            return RemoveAdjacentDups<T>(chain, false);
        }

        //removes duplicated points in a chain of points - only adjacent duplicates are removed
        public static Coordinate2<T>[] RemoveAdjacentDups<T>(IEnumerable<Coordinate2<T>> chain, bool isRing)
            where T : IEquatable<T>, IComparable<T>
        {
            if (chain == null)
                return null;
            Coordinate2<T> prev = null;
            List<Coordinate2<T>> tmp = new List<Coordinate2<T>>();
            foreach (Coordinate2<T> curPt in chain)
            {
                if (curPt == null)
                    continue;
                if (curPt.Equals(prev))
                    continue; //dup
                prev = curPt;
                tmp.Add(curPt);
            }
            if (isRing && tmp.Count > 1)
            {
                if (tmp[0].Equals(tmp[tmp.Count - 1])) //start and end are dup on a ring
                    tmp.RemoveAt(tmp.Count - 1); //get rid of the last item
            }
            return tmp.ToArray();
        }
        #endregion

        #region Equality Check
        public static bool PointsEqual(Coordinate2<double> pt1, Coordinate2<double> pt2)
        {
            if (MathUtils.AlmostEqual(pt1.X, pt2.X) && MathUtils.AlmostEqual(pt1.Y, pt2.Y))
                return true;
            return false;
        }

        public static bool PointsEqual(double pt1X, double pt1Y, double pt2X, double pt2Y)
        {
            if (MathUtils.AlmostEqual(pt1X, pt2X) && MathUtils.AlmostEqual(pt1Y, pt2Y))
                return true;
            return false;
        }
        #endregion

        //Angle made by points a and b from origin
        public static double AngleBetween(Coordinate2<double> a, Coordinate2<double> b)
        {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }
        public static double AngleBetween(double aX, double aY, double bX, double bY)
        {
            return Math.Atan2(bY - aY, bX - aX);
        }

        // angle made by axes and point a
        public static double AngleBy(Coordinate2<double> a)
        {
            return Math.Atan2(a.Y, a.X);
        }
        public static double AngleBy(double aX, double aY)
        {
            return Math.Atan2(aY, aX);
        }

        // angle ABC
        public static bool IsAcute(Coordinate2<double> a, Coordinate2<double> b, Coordinate2<double> c)
        {
            double dx0 = a.X - b.X;
            double dy0 = a.Y - b.Y;
            double dx1 = c.X - b.X;
            double dy1 = c.Y - b.Y;
            double dp = dx0 * dx1 + dy0 * dy1;
            return dp > 0;
        }
        public static bool IsAcute(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            double dx0 = aX - bX;
            double dy0 = aY - bY;
            double dx1 = cX - bX;
            double dy1 = cY - bY;
            double dp = dx0 * dx1 + dy0 * dy1;
            return dp > 0;
        }

        // angle ABC
        public static bool IsObtuse(Coordinate2<double> a, Coordinate2<double> b, Coordinate2<double> c)
        {
            double dx0 = a.X - b.X;
            double dy0 = a.Y - b.Y;
            double dx1 = c.X - b.X;
            double dy1 = c.Y - b.Y;
            double dp = dx0 * dx1 + dy0 * dy1;
            return dp < 0;
        }
        public static bool IsObtuse(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            double dx0 = aX - bX;
            double dy0 = aY - bY;
            double dx1 = cX - bX;
            double dy1 = cY - bY;
            double dp = dx0 * dx1 + dy0 * dy1;
            return dp < 0;
        }

        // angle ABC
        public static double AngleBetween(Coordinate2<double> a, Coordinate2<double> b, Coordinate2<double> c)
        {
            double a1 = AngleBetween(b, a);
            double a2 = AngleBetween(b, c);
            return AngleUtils.Diff(a1, a2);
        }
        public static double AngleBetween(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            double a1 = AngleBetween(bX, bY, aX, aY);
            double a2 = AngleBetween(bX, bY, cX, cY);
            return AngleUtils.Diff(a1, a2);
        }

        // angle ABC
        public static double AngleBetweenCircular(Coordinate2<double> a, Coordinate2<double> b, Coordinate2<double> c)
        {
            double da = AngleBetween(b, c) - AngleBetween(b, a);

            if (da < 0)
                return da + Constants.TwoPi;
            return da;
        }
        public static double AngleBetweenCircular(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            double da = AngleBetween(bX, bY, cX, cY) - AngleBetween(bX, bY, aX, aY);

            if (da < 0)
                return da + Constants.TwoPi;
            return da;
        }

        // angle ABC
        public static double AngleBetweenOriented(Coordinate2<double> a, Coordinate2<double> b, Coordinate2<double> c)
        {
            double a1 = AngleBetween(b, a);
            double a2 = AngleBetween(b, c);
            double da = a2 - a1;

            if (da <= Constants.NPi)
                return da + Constants.TwoPi;
            if (da > Math.PI)
                return da - Constants.TwoPi;
            return da;
        }
        public static double AngleBetweenOriented(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            double a1 = AngleBetween(bX, bY, aX, aY);
            double a2 = AngleBetween(bX, bY, cX, cY);
            double da = a2 - a1;

            if (da <= Constants.NPi)
                return da + Constants.TwoPi;
            if (da > Math.PI)
                return da - Constants.TwoPi;
            return da;
        }

        // angle ABC
        public static double InteriorAngle(Coordinate2<double> a, Coordinate2<double> b, Coordinate2<double> c)
        {
            double da = AngleBetween(b, a);
            double db = AngleBetween(b, c);
            return Math.Abs(db - da);
        }
        public static double InteriorAngle(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            double da = AngleBetween(bX, bY, aX, aY);
            double db = AngleBetween(bX, bY, cX, cY);
            return Math.Abs(db - da);
        }

        //Orientation of q with respect to line segment from p1->p2
        public static Orientation Orient(Coordinate2<double> p1, Coordinate2<double> p2, Coordinate2<double> q)
        {
            double dx1 = p2.X - p1.X;
            double dy1 = p2.Y - p1.Y;
            double dx2 = q.X - p2.X;
            double dy2 = q.Y - p2.Y;
            int sign = AngleUtils.DeterminantSign(dx1, dy1, dx2, dy2);
            if (sign < 0)
                return Orientation.Clockwise;
            else if (sign > 0)
                return Orientation.Counterclockwise;
            return Orientation.Collinear;
        }
        public static Orientation Orient(double p1X, double p1Y, double p2X, double p2Y, double qX, double qY)
        {
            double dx1 = p2X - p1X;
            double dy1 = p2Y - p1Y;
            double dx2 = qX - p2X;
            double dy2 = qY - p2Y;
            int sign = AngleUtils.DeterminantSign(dx1, dy1, dx2, dy2);
            if (sign < 0)
                return Orientation.Clockwise;
            else if (sign > 0)
                return Orientation.Counterclockwise;
            return Orientation.Collinear;
        }

        public static int OrientationIndex(Coordinate2<double> p1, Coordinate2<double> p2, Coordinate2<double> q)
        {
            // travelling along p1->p2, turn counter clockwise to get to q return 1,
            // travelling along p1->p2, turn clockwise to get to q return -1,
            // p1, p2 and q are colinear return 0.
            double dx1 = p2.X - p1.X;
            double dy1 = p2.Y - p1.Y;
            double dx2 = q.X - p2.X;
            double dy2 = q.Y - p2.Y;
            return AngleUtils.DeterminantSign(dx1, dy1, dx2, dy2);
        }
        public static int OrientationIndex(double p1X, double p1Y, double p2X, double p2Y, double qX, double qY)
        {
            // travelling along p1->p2, turn counter clockwise to get to q return 1,
            // travelling along p1->p2, turn clockwise to get to q return -1,
            // p1, p2 and q are colinear return 0.
            double dx1 = p2X - p1X;
            double dy1 = p2Y - p1Y;
            double dx2 = qX - p2X;
            double dy2 = qY - p2Y;
            return AngleUtils.DeterminantSign(dx1, dy1, dx2, dy2);
        }

        /// <summary>
        /// Computes whether a ring defined by an array of {@link Coordinate}s is oriented counter-clockwise.
        /// <ul><li>The list of points is assumed to have the first and last points equal.
        /// <li>This will handle coordinate lists which contain repeated points.</ul>
        /// This algorithm is <b>only</b> guaranteed to work with valid rings.
        /// If the ring is invalid (e.g. self-crosses or touches), the computed result may not be correct.
        /// </summary>
        /// <param name="ring"></param>
        /// <returns></returns>
        public static bool IsCCW(Coordinate2<double>[] ring)
        {
            // sanity check
            if (ring.Length < 3)
                return false;
            int nPts = ring.Length - 1;

            // find highest point
            Coordinate2<double> hiPt = ring[0];
            int hiIndex = 0;
            for (int i = 1; i < ring.Length; i++)
            {
                Coordinate2<double> p = ring[i];
                if (p.Y > hiPt.Y)
                {
                    hiPt = p;
                    hiIndex = i;
                }
            }

            // find distinct point before highest point
            int iPrev = hiIndex;
            do
            {
                iPrev = iPrev - 1;
                if (iPrev < 0) iPrev = nPts;
            } while (ring[iPrev].Equals(hiPt) && iPrev != hiIndex);

            // find distinct point after highest point
            int iNext = hiIndex;
            do
            {
                iNext = (iNext + 1) % nPts;
            } while (ring[iNext].Equals(hiPt) && iNext != hiIndex);

            Coordinate2<double> prev = ring[iPrev];
            Coordinate2<double> next = ring[iNext];

            /**
             * This check catches cases where the ring contains an A-B-A configuration of points.
             * This can happen if the ring does not contain 3 distinct points
             * (including the case where the input array has fewer than 4 elements),
             * or it contains coincident line segments.
             */
            if (prev.Equals(hiPt) || next.Equals(hiPt) || prev.Equals(next))
                return false;

            int disc = OrientationIndex(prev, hiPt, next);

            /**
             *  If disc is exactly 0, lines are collinear.  There are two possible cases:
             *  (1) the lines lie along the x axis in opposite directions
             *  (2) the lines lie on top of one another
             *
             *  (1) is handled by checking if next is left of prev ==> CCW
             *  (2) will never happen if the ring is valid, so don't check for it
             *  (Might want to assert this)
             */
            bool isCCW = false;
            if (disc == 0)
            {
                // poly is CCW if prev x is right of next x
                isCCW = (prev.X > next.X);
            }
            else
            {
                // if area is positive, points are ordered CCW
                isCCW = (disc > 0);
            }
            return isCCW;
        }

        public static LineIntersectionType ComputeIntersection(Coordinate2<double> p, Coordinate2<double> p1, Coordinate2<double> p2)
        {
            // do between check first, since it is faster than the orientation test
            if (PointInEnvelope(p1, p2, p))
            {
                if ((OrientationIndex(p1, p2, p) == 0) && (OrientationIndex(p2, p1, p) == 0))
                {
                    return LineIntersectionType.PointIntersection;
                }
            }
            return LineIntersectionType.NoIntersection;
        }
        public static LineIntersectionType ComputeIntersection(double pX, double pY, double p1X, double p1Y, double p2X, double p2Y)
        {
            // do between check first, since it is faster than the orientation test
            if (PointInEnvelope(p1X, p1Y, p2X, p2Y, pX, pY))
            {
                if ((OrientationIndex(p1X, p1Y, p2X, p2Y, pX, pY) == 0) && (OrientationIndex(p2X, p2Y, p1X, p1Y, pX, pY) == 0))
                {
                    return LineIntersectionType.PointIntersection;
                }
            }
            return LineIntersectionType.NoIntersection;
        }

        public static LineIntersectionResult<double> GetIntersection(Coordinate2<double> p1, Coordinate2<double> p2, Coordinate2<double> q1, Coordinate2<double> q2)
        {
            if (p1 == null || p2 == null || q1 == null || q2 == null)
                return null;

            //quick rejection
            if (!Coordinate2Utils.EnvelopesIntersect(p1, p2, q1, q2))
                return new LineIntersectionResult<double>();
            // for each endpoint, compute which side of the other segment it lies
            // if both endpoints lie on the same side of the other segment, the segments do not intersect
            int Pq1 = Coordinate2Utils.OrientationIndex(p1, p2, q1);
            int Pq2 = Coordinate2Utils.OrientationIndex(p1, p2, q2);

            if ((Pq1 > 0 && Pq2 > 0) || (Pq1 < 0 && Pq2 < 0))
                return new LineIntersectionResult<double>();

            int Qp1 = Coordinate2Utils.OrientationIndex(q1, q2, p1);
            int Qp2 = Coordinate2Utils.OrientationIndex(q1, q2, p2);

            if ((Qp1 > 0 && Qp2 > 0) || (Qp1 < 0 && Qp2 < 0))
                return new LineIntersectionResult<double>();
            //end quick rejection

            if (Pq1 == 0 && Pq2 == 0 && Qp1 == 0 && Qp2 == 0) //collinear intersection
            {
                bool p1q1p2 = Coordinate2Utils.PointInEnvelope(p1, p2, q1);
                bool p1q2p2 = Coordinate2Utils.PointInEnvelope(p1, p2, q2);
                bool q1p1q2 = Coordinate2Utils.PointInEnvelope(q1, q2, p1);
                bool q1p2q2 = Coordinate2Utils.PointInEnvelope(q1, q2, p2);

                if (p1q1p2 && p1q2p2)
                    return new LineIntersectionResult<double>(LineIntersectionType.CollinearIntersection, q1, q2);
                if (q1p1q2 && q1p2q2)
                    return new LineIntersectionResult<double>(LineIntersectionType.CollinearIntersection, p1, p2);
                if (p1q1p2 && q1p1q2)
                {
                    if (q1.Equals(p1) && !p1q2p2 && !q1p2q2)
                        return new LineIntersectionResult<double>(q1);
                    return new LineIntersectionResult<double>(LineIntersectionType.CollinearIntersection, q1, p1);
                }
                if (p1q1p2 && q1p2q2)
                {
                    if (q1.Equals(p2) && !p1q2p2 && !q1p1q2)
                        return new LineIntersectionResult<double>(q1);
                    return new LineIntersectionResult<double>(LineIntersectionType.CollinearIntersection, q1, p2);
                }
                if (p1q2p2 && q1p1q2)
                {
                    if (q2.Equals(p1) && !p1q1p2 && !q1p2q2)
                        return new LineIntersectionResult<double>(q2);
                    return new LineIntersectionResult<double>(LineIntersectionType.CollinearIntersection, q2, p1);
                }
                if (p1q2p2 && q1p2q2)
                {
                    if (q2.Equals(p2) && !p1q1p2 && !q1p1q2)
                        return new LineIntersectionResult<double>(q2);
                    return new LineIntersectionResult<double>(LineIntersectionType.CollinearIntersection, q2, p2);
                }
                return new LineIntersectionResult<double>();
            }//end collinear

            // At this point we know that there is a single intersection point (since the lines are not collinear).
            // Check if the intersection is an endpoint. If it is, copy the endpoint as the intersection point. Copying the point rather than computing it
            // ensures the point has the exact value, which is important for robustness. It is sufficient to simply check for an endpoint which is on
            // the other line, since at this point we know that the inputLines must intersect.
            if (Pq1 == 0 || Pq2 == 0 || Qp1 == 0 || Qp2 == 0)
            {
                // Check for two equal endpoints.  This is done explicitly rather than by the orientation tests below in order to improve robustness.
                if (p1.Equals(q1) || p1.Equals(q2))
                    return new LineIntersectionResult<double>(p1);
                else if (p2.Equals(q1) || p2.Equals(q2))
                    return new LineIntersectionResult<double>(p2);
                // Now check to see if any endpoint lies on the interior of the other segment.
                else if (Pq1 == 0)
                    return new LineIntersectionResult<double>(q1);
                else if (Pq2 == 0)
                    return new LineIntersectionResult<double>(q2);
                else if (Qp1 == 0)
                    return new LineIntersectionResult<double>(p1);
                else if (Qp2 == 0)
                    return new LineIntersectionResult<double>(p2);
            } //end exact at endpoint

            //intersectWNormalization
            Coordinate2<double> n1 = new Coordinate2<double>(p1);
            Coordinate2<double> n2 = new Coordinate2<double>(p2);
            Coordinate2<double> n3 = new Coordinate2<double>(q1);
            Coordinate2<double> n4 = new Coordinate2<double>(q2);
            Coordinate2<double> normPt = new Coordinate2<double>();
            Coordinate2Utils.NormalizeToEnvCentre(n1, n2, n3, n4, normPt);

            //safeHCoordinateIntersection
            Coordinate2<double> intPt = null;
            // unrolled computation
            double px = n1.Y - n2.Y;
            double py = n2.X - n1.X;
            double pw = n1.X * n2.Y - n2.X * n1.Y;

            double qx = n3.Y - n4.Y;
            double qy = n4.X - n3.X;
            double qw = n3.X * n4.Y - n4.X * n3.Y;

            double x = py * qw - qy * pw;
            double y = qx * pw - px * qw;
            double w = px * qy - qx * py;

            double xInt = x / w;
            double yInt = y / w;

            if (double.IsNaN(xInt) || double.IsNaN(yInt) || double.IsInfinity(xInt) || double.IsInfinity(yInt))
            {
                intPt = MeanNearest(n1, n2, n3, n4);
                intPt.X += normPt.X;
                intPt.Y += normPt.Y;
                return new LineIntersectionResult<double>(intPt);
            }
            else
                intPt = new Coordinate2<double>(xInt, yInt);

            intPt.X += normPt.X;
            intPt.Y += normPt.Y;
            //End safeHCoordinateIntersection
            //End intersectWNormalization

            if (!(Coordinate2Utils.PointInEnvelope(p1, p2, intPt) || Coordinate2Utils.PointInEnvelope(q1, q2, intPt)))
            {
                intPt = MeanNearest(p1, p2, q1, q2);
            }

            return new LineIntersectionResult<double>(intPt);
        }
        public static LineIntersectionResult<double> GetIntersection(double p1X, double p1Y, double p2X, double p2Y, double q1X, double q1Y, double q2X, double q2Y)
        {
            return GetIntersection(new Coordinate2<double>(p1X, p1Y), new Coordinate2<double>(p2X, p2Y), new Coordinate2<double>(q1X, q1Y), new Coordinate2<double>(q2X, q2Y));
        }

        private static Coordinate2<double> MeanNearest(Coordinate2<double> p1, Coordinate2<double> p2, Coordinate2<double> q1, Coordinate2<double> q2)
        {
            Coordinate2<double> mean = new Coordinate2<double>(p1.X, p1.Y);
            mean.X += p2.X;
            mean.Y += p2.Y;
            mean.X += q1.X;
            mean.Y += q1.Y;
            mean.X += q2.X;
            mean.Y += q2.Y;
            mean.X *= 0.25;
            mean.Y *= 0.25;

            Coordinate2<double> intPt = p1;
            double bestDist = MathUtils.Distance(mean.X, mean.Y, intPt.X, intPt.Y);
            double curDist = MathUtils.Distance(mean.X, mean.Y, p2.X, p2.Y);
            if (curDist < bestDist)
            {
                bestDist = curDist;
                intPt = p2;
            }
            curDist = MathUtils.Distance(mean.X, mean.Y, q1.X, q1.Y);
            if (curDist < bestDist)
            {
                bestDist = curDist;
                intPt = q1;
            }
            curDist = MathUtils.Distance(mean.X, mean.Y, q2.X, q2.Y);
            if (curDist < bestDist)
            {
                bestDist = curDist;
                intPt = q2;
            }
            return intPt;
        }

        /**
         * Normalize the supplied coordinates to
         * so that the midpoint of their intersection envelope
         * lies at the origin.
         *
         * @param n00
         * @param n01
         * @param n10
         * @param n11
         * @param normPt
         */
        public static void NormalizeToEnvCentre(Coordinate2<double> n00, Coordinate2<double> n01, Coordinate2<double> n10, Coordinate2<double> n11, Coordinate2<double> normPt)
        {
            double minX0 = n00.X < n01.X ? n00.X : n01.X;
            double minY0 = n00.Y < n01.Y ? n00.Y : n01.Y;
            double maxX0 = n00.X > n01.X ? n00.X : n01.X;
            double maxY0 = n00.Y > n01.Y ? n00.Y : n01.Y;

            double minX1 = n10.X < n11.X ? n10.X : n11.X;
            double minY1 = n10.Y < n11.Y ? n10.Y : n11.Y;
            double maxX1 = n10.X > n11.X ? n10.X : n11.X;
            double maxY1 = n10.Y > n11.Y ? n10.Y : n11.Y;

            double intMinX = minX0 > minX1 ? minX0 : minX1;
            double intMaxX = maxX0 < maxX1 ? maxX0 : maxX1;
            double intMinY = minY0 > minY1 ? minY0 : minY1;
            double intMaxY = maxY0 < maxY1 ? maxY0 : maxY1;

            double intMidX = (intMinX + intMaxX) / 2.0;
            double intMidY = (intMinY + intMaxY) / 2.0;
            normPt.X = intMidX;
            normPt.Y = intMidY;

            n00.X -= normPt.X;
            n00.Y -= normPt.Y;

            n01.X -= normPt.X;
            n01.Y -= normPt.Y;

            n10.X -= normPt.X;
            n10.Y -= normPt.Y;

            n11.X -= normPt.X;
            n11.Y -= normPt.Y;
        }

        private static double SmallestInAbsValue(double x1, double x2, double x3, double x4)
        {
            double x = x1;
            double xabs = Math.Abs(x);
            if (Math.Abs(x2) < xabs)
            {
                x = x2;
                xabs = Math.Abs(x2);
            }
            if (Math.Abs(x3) < xabs)
            {
                x = x3;
                xabs = Math.Abs(x3);
            }
            if (Math.Abs(x4) < xabs)
                x = x4;
            return x;
        }

        /// <summary>
        /// Test the point q to see whether it intersects the Envelope defined by p1-p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static bool PointInEnvelope(Coordinate2<double> p1, Coordinate2<double> p2, Coordinate2<double> q)
        {
            return ((q.X >= (p1.X < p2.X ? p1.X : p2.X)) && (q.X <= (p1.X > p2.X ? p1.X : p2.X))) &&
                   ((q.Y >= (p1.Y < p2.Y ? p1.Y : p2.Y)) && (q.Y <= (p1.Y > p2.Y ? p1.Y : p2.Y)));
        }
        public static bool PointInEnvelope(double p1X, double p1Y, double p2X, double p2Y, double qX, double qY)
        {
            return ((qX >= (p1X < p2X ? p1X : p2X)) && (qX <= (p1X > p2X ? p1X : p2X))) &&
                   ((qY >= (p1Y < p2Y ? p1Y : p2Y)) && (qY <= (p1Y > p2Y ? p1Y : p2Y)));
        }

        /// <summary>
        /// Test the envelope defined by p1-p2 for intersection with the envelope defined by q1-q2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static bool EnvelopesIntersect(Coordinate2<double> p1, Coordinate2<double> p2, Coordinate2<double> q1, Coordinate2<double> q2)
        {
            double minq = Math.Min(q1.X, q2.X);
            double maxq = Math.Max(q1.X, q2.X);
            double minp = Math.Min(p1.X, p2.X);
            double maxp = Math.Max(p1.X, p2.X);

            if (minp > maxq)
                return false;
            if (maxp < minq)
                return false;

            minq = Math.Min(q1.Y, q2.Y);
            maxq = Math.Max(q1.Y, q2.Y);
            minp = Math.Min(p1.Y, p2.Y);
            maxp = Math.Max(p1.Y, p2.Y);

            if (minp > maxq)
                return false;
            if (maxp < minq)
                return false;
            return true;
        }
        public static bool EnvelopesIntersect(double p1X, double p1Y, double p2X, double p2Y, double q1X, double q1Y, double q2X, double q2Y)
        {
            double minq = Math.Min(q1X, q2X);
            double maxq = Math.Max(q1X, q2X);
            double minp = Math.Min(p1X, p2X);
            double maxp = Math.Max(p1X, p2X);

            if (minp > maxq)
                return false;
            if (maxp < minq)
                return false;

            minq = Math.Min(q1Y, q2Y);
            maxq = Math.Max(q1Y, q2Y);
            minp = Math.Min(p1Y, p2Y);
            maxp = Math.Max(p1Y, p2Y);

            if (minp > maxq)
                return false;
            if (maxp < minq)
                return false;
            return true;
        }

        public static double Distance(Coordinate2<double> a, Coordinate2<double> b)
        {
            double dX = b.X - a.X;
            double dY = b.Y - a.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }
    }
}
