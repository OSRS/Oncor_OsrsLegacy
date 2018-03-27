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

using Osrs.Numerics.Spatial.Coordinates;
using System;

namespace Osrs.Numerics.Spatial.Geometry
{
    public class LineSegment2IntersectionResult<T>
        where T : IEquatable<T>, IComparable<T>
    {
        public LineIntersectionType IntersectionType = LineIntersectionType.NoIntersection;
        public Point2<T> BasePoint = null;
        public Point2<T> FinalPoint = null;

        public LineSegment2IntersectionResult()
        { }

        public LineSegment2IntersectionResult(Point2<T> intersectionPoint)
        {
            if (intersectionPoint != null)
                this.IntersectionType = LineIntersectionType.PointIntersection;
            else
                this.IntersectionType = LineIntersectionType.NoIntersection;
            this.BasePoint = intersectionPoint;
            this.FinalPoint = null;
        }

        public LineSegment2IntersectionResult(LineIntersectionType type, Point2<T> basePoint, Point2<T> finalPoint)
        {
            this.IntersectionType = type;
            this.BasePoint = basePoint;
            this.FinalPoint = finalPoint;
        }

        public LineSegment2IntersectionResult(Point2<T> basePoint, Point2<T> finalPoint)
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

    public static class SegmentUtils
    {
        //Orientation of q with respect to line segment from p1->p2
        public static Orientation Orient(Point2<double> p1, Point2<double> p2, Point2<double> q)
        {
            //double dx1 = p2.X - p1.X;
            //double dy1 = p2.Y - p1.Y;
            //double dx2 = q.X - p2.X;
            //double dy2 = q.Y - p2.Y;
            //int sign = AngleUtils.DeterminantSign(dx1, dy1, dx2, dy2);
            int sign = AngleUtils.DeterminantSign(p2.X - p1.X, p2.Y - p1.Y, q.X - p2.X, q.Y - p2.Y);
            if (sign < 0)
                return Orientation.Clockwise;
            else if (sign > 0)
                return Orientation.Counterclockwise;
            return Orientation.Collinear;
        }

        public static int OrientationIndex(Point2<double> p1, Point2<double> p2, Point2<double> q)
        {
            // travelling along p1->p2, turn counter clockwise to get to q return 1,
            // travelling along p1->p2, turn clockwise to get to q return -1,
            // p1, p2 and q are colinear return 0.
            //double dx1 = p2.X - p1.X;
            //double dy1 = p2.Y - p1.Y;
            //double dx2 = q.X - p2.X;
            //double dy2 = q.Y - p2.Y;
            //return AngleUtils.DeterminantSign(dx1, dy1, dx2, dy2);
            return AngleUtils.DeterminantSign(p2.X - p1.X, p2.Y - p1.Y, q.X - p2.X, q.Y - p2.Y);
        }

        public static LineIntersectionType ComputeIntersectionType(LineSegment2<double> segment, Point2<double> p)
        {
            if (segment == null || p == null)
                return LineIntersectionType.NoIntersection;
            // do between check first, since it is faster than the orientation test
            if (PointInEnvelope(segment.Start, segment.End, p))
            {
                if ((OrientationIndex(segment.Start, segment.End, p) == 0) && (OrientationIndex(segment.End, segment.Start, p) == 0))
                    return LineIntersectionType.PointIntersection;
            }
            return LineIntersectionType.NoIntersection;
        }
        public static LineIntersectionType ComputeIntersectionType(Point2<double> p1, Point2<double> p2, Point2<double> p)
        {
            // do between check first, since it is faster than the orientation test
            if (PointInEnvelope(p1, p2, p))
            {
                if ((OrientationIndex(p1, p2, p) == 0) && (OrientationIndex(p2, p1, p) == 0))
                    return LineIntersectionType.PointIntersection;
            }
            return LineIntersectionType.NoIntersection;
        }

        public static LineSegment2IntersectionResult<double> ComputeIntersection(LineSegment2<double> segmentP, LineSegment2<double> segmentQ)
        {
            if (segmentP == null || segmentQ == null)
                return new LineSegment2IntersectionResult<double>();
            return ComputeIntersection(segmentP.Start, segmentP.End, segmentQ.Start, segmentQ.End);
        }
        public static LineSegment2IntersectionResult<double> ComputeIntersection(Point2<double> p1, Point2<double> p2, Point2<double> q1, Point2<double> q2)
        {
            if (p1 == null || p2 == null || q1 == null || q2 == null)
                return null;

            //quick rejection
            if (!Coordinate2Utils.EnvelopesIntersect(p1, p2, q1, q2))
                return new LineSegment2IntersectionResult<double>();
            // for each endpoint, compute which side of the other segment it lies
            // if both endpoints lie on the same side of the other segment, the segments do not intersect
            int Pq1 = Coordinate2Utils.OrientationIndex(p1, p2, q1);
            int Pq2 = Coordinate2Utils.OrientationIndex(p1, p2, q2);

            if ((Pq1 > 0 && Pq2 > 0) || (Pq1 < 0 && Pq2 < 0))
                return new LineSegment2IntersectionResult<double>();

            int Qp1 = Coordinate2Utils.OrientationIndex(q1, q2, p1);
            int Qp2 = Coordinate2Utils.OrientationIndex(q1, q2, p2);

            if ((Qp1 > 0 && Qp2 > 0) || (Qp1 < 0 && Qp2 < 0))
                return new LineSegment2IntersectionResult<double>();
            //end quick rejection

            if (Pq1 == 0 && Pq2 == 0 && Qp1 == 0 && Qp2 == 0) //collinear intersection
            {
                bool p1q1p2 = Coordinate2Utils.PointInEnvelope(p1, p2, q1);
                bool p1q2p2 = Coordinate2Utils.PointInEnvelope(p1, p2, q2);
                bool q1p1q2 = Coordinate2Utils.PointInEnvelope(q1, q2, p1);
                bool q1p2q2 = Coordinate2Utils.PointInEnvelope(q1, q2, p2);

                if (p1q1p2 && p1q2p2)
                    return new LineSegment2IntersectionResult<double>(LineIntersectionType.CollinearIntersection, q1, q2);
                if (q1p1q2 && q1p2q2)
                    return new LineSegment2IntersectionResult<double>(LineIntersectionType.CollinearIntersection, p1, p2);
                if (p1q1p2 && q1p1q2)
                {
                    if (q1.Equals(p1) && !p1q2p2 && !q1p2q2)
                        return new LineSegment2IntersectionResult<double>(q1);
                    return new LineSegment2IntersectionResult<double>(LineIntersectionType.CollinearIntersection, q1, p1);
                }
                if (p1q1p2 && q1p2q2)
                {
                    if (q1.Equals(p2) && !p1q2p2 && !q1p1q2)
                        return new LineSegment2IntersectionResult<double>(q1);
                    return new LineSegment2IntersectionResult<double>(LineIntersectionType.CollinearIntersection, q1, p2);
                }
                if (p1q2p2 && q1p1q2)
                {
                    if (q2.Equals(p1) && !p1q1p2 && !q1p2q2)
                        return new LineSegment2IntersectionResult<double>(q2);
                    return new LineSegment2IntersectionResult<double>(LineIntersectionType.CollinearIntersection, q2, p1);
                }
                if (p1q2p2 && q1p2q2)
                {
                    if (q2.Equals(p2) && !p1q1p2 && !q1p1q2)
                        return new LineSegment2IntersectionResult<double>(q2);
                    return new LineSegment2IntersectionResult<double>(LineIntersectionType.CollinearIntersection, q2, p2);
                }
                return new LineSegment2IntersectionResult<double>();
            }//end collinear

            // At this point we know that there is a single intersection point (since the lines are not collinear).
            // Check if the intersection is an endpoint. If it is, copy the endpoint as the intersection point. Copying the point rather than computing it
            // ensures the point has the exact value, which is important for robustness. It is sufficient to simply check for an endpoint which is on
            // the other line, since at this point we know that the inputLines must intersect.
            if (Pq1 == 0 || Pq2 == 0 || Qp1 == 0 || Qp2 == 0)
            {
                // Check for two equal endpoints.  This is done explicitly rather than by the orientation tests below in order to improve robustness.
                if (p1.Equals(q1) || p1.Equals(q2))
                    return new LineSegment2IntersectionResult<double>(p1);
                else if (p2.Equals(q1) || p2.Equals(q2))
                    return new LineSegment2IntersectionResult<double>(p2);
                // Now check to see if any endpoint lies on the interior of the other segment.
                else if (Pq1 == 0)
                    return new LineSegment2IntersectionResult<double>(q1);
                else if (Pq2 == 0)
                    return new LineSegment2IntersectionResult<double>(q2);
                else if (Qp1 == 0)
                    return new LineSegment2IntersectionResult<double>(p1);
                else if (Qp2 == 0)
                    return new LineSegment2IntersectionResult<double>(p2);
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
                xInt = intPt.X + normPt.X;
                yInt = intPt.Y + normPt.Y;
                return new LineSegment2IntersectionResult<double>(p1.Factory.ConstructPoint(xInt, yInt));
            }
            else
            {
                xInt += normPt.X;
                yInt += normPt.Y;
                intPt = new Coordinate2<double>(xInt, yInt);
            }
            //End safeHCoordinateIntersection
            //End intersectWNormalization

            if (!(Coordinate2Utils.PointInEnvelope(p1, p2, intPt) || Coordinate2Utils.PointInEnvelope(q1, q2, intPt)))
            {
                intPt = MeanNearest(p1, p2, q1, q2);
            }

            return new LineSegment2IntersectionResult<double>(p1.Factory.ConstructPoint(intPt.X, intPt.Y));
        }

        private static Coordinate2<double> MeanNearest(Coordinate2<double> p1, Coordinate2<double> p2, Coordinate2<double> q1, Coordinate2<double> q2)
        {
            Coordinate2<double> mean = new Coordinate2<double>((p1.X + p2.X + q1.X + q2.X) * 0.25, (p1.Y + p2.Y + q1.Y + q2.Y) * 0.25);

            Coordinate2<double> intPt = p1;
            double bestDist = SegmentUtils.Length(mean.X, mean.Y, intPt.X, intPt.Y);
            double curDist = SegmentUtils.Length(mean.X, mean.Y, p2.X, p2.Y);
            if (curDist < bestDist)
            {
                bestDist = curDist;
                intPt = p2;
            }
            curDist = SegmentUtils.Length(mean.X, mean.Y, q1.X, q1.Y);
            if (curDist < bestDist)
            {
                bestDist = curDist;
                intPt = q1;
            }
            curDist = SegmentUtils.Length(mean.X, mean.Y, q2.X, q2.Y);
            if (curDist < bestDist)
            {
                //bestDist = curDist;
                //intPt = q2;
                return q2;
            }
            return intPt;
        }

        public static bool HasIntersection(LineSegment2<double> segmentP, LineSegment2<double> segmentQ)
        {
            if (segmentP == null || segmentQ == null)
                return false;
            LineSegment2IntersectionResult<double> tmp = ComputeIntersection(segmentP.Start, segmentP.End, segmentQ.Start, segmentQ.End);
            return tmp.IntersectionType != LineIntersectionType.NoIntersection;
        }
        public static bool HasIntersection(Point2<double> p1, Point2<double> p2, Point2<double> q1, Point2<double> q2)
        {
            LineSegment2IntersectionResult<double> tmp = ComputeIntersection(p1, p2, q1, q2);
            return tmp.IntersectionType != LineIntersectionType.NoIntersection;
        }

        public static Point2<double> Bisector(Point2<double> p1, Point2<double> p2)
        {
            double dX = p2.X - p1.X;
            double dY = p2.Y - p1.Y;
            double l1X = p1.X + (dX * 0.5);
            double l1Y = p1.Y + (dY * 0.5);
            double l2X = p1.X - dY + (dX * 0.5);
            double l2Y = p1.Y + dX + (dY * 0.5);

            double w = l1X * l2Y - l2X * l1Y;
            if (w == 0)
                w = double.Epsilon;
            else if (double.IsInfinity(w) || double.IsNaN(w))
            {
                if (double.IsPositiveInfinity(w))
                    w = double.MaxValue;
                else if (double.IsNegativeInfinity(w))
                    w = double.MinValue;
                else
                    w = double.Epsilon;
            }

            dX = (l1Y - l2Y) / w;
            dY = (l2X - l1X) / w;

            return p1.Factory.ConstructPoint(dX, dY);
        }

        public static Point2<double> MidPoint(Point2<double> p1, Point2<double> p2)
        {
            return p1.Factory.ConstructPoint((p1.X + p2.X) * 0.5, (p1.Y + p2.Y) * 0.5);
        }

        /// <summary>
        /// Test the point q to see whether it intersects the Envelope defined by p1-p2
        /// </summary>
        internal static bool PointInEnvelope(Point2<double> p1, Point2<double> p2, Point2<double> q)
        {
            return ((q.X >= (p1.X < p2.X ? p1.X : p2.X)) && (q.X <= (p1.X > p2.X ? p1.X : p2.X))) &&
                   ((q.Y >= (p1.Y < p2.Y ? p1.Y : p2.Y)) && (q.Y <= (p1.Y > p2.Y ? p1.Y : p2.Y)));
        }

        /// <summary>
        /// Test the envelope defined by p1-p2 for intersection with the envelope defined by q1-q2
        /// </summary>
        internal static bool EnvelopesIntersect(Point2<double> p1, Point2<double> p2, Point2<double> q1, Point2<double> q2)
        {
            double minp = p1.X < p2.X ? p1.X : p2.X;
            double maxq = q1.X > q2.X ? q1.X : q2.X;

            if (minp > maxq)
                return false;

            double minq = q1.X < q2.X ? q1.X : q2.X;
            double maxp = p1.X > p2.X ? p1.X : p2.X;

            if (maxp < minq)
                return false;

            maxq = q1.Y > q2.Y ? q1.Y : q2.Y;
            minp = p1.Y < p2.Y ? p1.Y : p2.Y;

            if (minp > maxq)
                return false;

            minq = q1.Y < q2.Y ? q1.Y : q2.Y;
            maxp = p1.Y > p2.Y ? p1.Y : p2.Y;

            if (maxp < minq)
                return false;
            return true;
        }

        public static Point2<double> NearestPoint(Point2<double> p0, Point2<double> p1, Point2<double> q)
        {
            if (p0.Equals(p1))
                return p0;

            double s1 = (p1.Y - p0.Y) * (q.Y - p0.Y) + (p1.X - p0.X) * (q.X - p0.X);
            double s2 = (p1.Y - p0.Y) * (q.Y - p1.Y) + (p1.X - p0.X) * (q.X - p1.X);
            if (s1 * s2 <= 0)
            {
                double lam = (p0.Y - p1.Y) * (p0.Y - p1.Y) + (p0.X - p1.X) * (p0.X - p1.X); //dotproduct of p0-p1
                lam = ((q.Y - p0.Y) * (p0.Y - p1.Y) + (q.X - p0.X) * (p0.X - p1.X)) / lam;
                return q.Factory.ConstructPoint(p0.X + lam * (p0.X - p1.X), p0.Y + lam * (p0.Y - p1.Y));
            }
            else
                return (s1 > 0) ? p1 : p0;
        }

        public static double Distance(LineSegment2<double> seg, Point2<double> q)
        {
            return Distance(seg.Start, seg.End, q);
        }

        public static double Distance(Point2<double> p1, Point2<double> p2, Point2<double> q)
        {
            if (p1.Equals(p2))
                return PointUtilsDouble.Distance(p1, q);
            double r = ((q.X - p1.X) * (p2.X - p1.X) + (q.Y - p1.Y) * (p2.Y - p1.Y)) / ((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));

            if (r <= 0.0d)
                return PointUtilsDouble.Distance(q, p1);
            if (r >= 1.0d)
                return PointUtilsDouble.Distance(q, p2);

            double s = ((p1.Y - q.Y) * (p2.X - p1.X) - (p1.X - q.X) * (p2.Y - p1.Y)) / ((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));

            return Math.Abs(s) * Math.Sqrt(((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y)));
        }

        public static double DistancePerpendicular(LineSegment2<double> seg, Point2<double> q)
        {
            return DistancePerpendicular(seg.Start, seg.End, q);
        }

        public static double DistancePerpendicular(Point2<double> p1, Point2<double> p2, Point2<double> q)
        {
            double s = ((p1.Y - q.Y) * (p2.X - p1.X) - (p1.X - q.X) * (p2.Y - p1.Y)) / ((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));

            return Math.Abs(s) * Math.Sqrt(((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y)));
        }

        public static double Length(LineSegment2<double> seg)
        {
            if (seg == null)
                return double.NaN;
            return Length(seg.Start, seg.End);
        }

        public static double Length(Point2<double> start, Point2<double> end)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double Length(double startX, double startY, double endX, double endY)
        {
            double dx = endX - startX;
            double dy = endY - startY;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
