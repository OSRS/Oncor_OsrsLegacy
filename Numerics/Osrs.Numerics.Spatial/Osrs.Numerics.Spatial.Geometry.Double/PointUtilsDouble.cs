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
    public static class PointUtilsDouble
    {
        public static bool PointsEqual(Point2<double> pt1, Point2<double> pt2)
        {
            if (MathUtils.AlmostEqual(pt1.X, pt2.X) && MathUtils.AlmostEqual(pt1.Y, pt2.Y))
                return true;
            return false;
        }

        public static bool PointsEqual(Point2<double> pt1, Coordinate2<double> pt2)
        {
            if (MathUtils.AlmostEqual(pt1.X, pt2.X) && MathUtils.AlmostEqual(pt1.Y, pt2.Y))
                return true;
            return false;
        }

        public static bool PointsEqual(Coordinate2<double> pt1, Point2<double> pt2)
        {
            if (MathUtils.AlmostEqual(pt1.X, pt2.X) && MathUtils.AlmostEqual(pt1.Y, pt2.Y))
                return true;
            return false;
        }

        //Angle made by points a and b from origin
        public static double AngleBetween(Point2<double> a, Point2<double> b)
        {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }

        // angle made by axes and point a
        public static double AngleBy(Point2<double> a)
        {
            return Math.Atan2(a.Y, a.X);
        }

        // angle ABC
        public static bool IsAcute(Point2<double> a, Point2<double> b, Point2<double> c)
        {
            //double dx0 = a.X - b.X;
            //double dy0 = a.Y - b.Y;
            //double dx1 = c.X - b.X;
            //double dy1 = c.Y - b.Y;
            //double dp = (a.X - b.X) * (c.X - b.X) + (a.Y - b.Y) * (c.Y - b.Y);
            return (a.X - b.X) * (c.X - b.X) + (a.Y - b.Y) * (c.Y - b.Y) > 0;
        }

        // angle ABC
        public static bool IsObtuse(Point2<double> a, Point2<double> b, Point2<double> c)
        {
            //double dx0 = a.X - b.X;
            //double dy0 = a.Y - b.Y;
            //double dx1 = c.X - b.X;
            //double dy1 = c.Y - b.Y;
            //double dp = dx0 * dx1 + dy0 * dy1;
            //double dp = (a.X - b.X) * (c.X - b.X) + (a.Y - b.Y) * (c.Y - b.Y);
            return (a.X - b.X) * (c.X - b.X) + (a.Y - b.Y) * (c.Y - b.Y) < 0;
        }

        // angle ABC
        public static double AngleBetween(Point2<double> a, Point2<double> b, Point2<double> c)
        {
            return AngleUtils.Diff(AngleBetween(b, a), AngleBetween(b, c));
        }

        // angle ABC
        public static double AngleBetweenOriented(Point2<double> a, Point2<double> b, Point2<double> c)
        {
            double da = AngleBetween(b, c) - AngleBetween(b, a);

            if (da <= Constants.NPi)
                return da + Constants.TwoPi;
            if (da > Math.PI)
                return da - Constants.TwoPi;
            return da;
        }

        // angle ABC
        public static double AngleBetweenCircular(Point2<double> a, Point2<double> b, Point2<double> c)
        {
            double da = AngleBetween(b, c) - AngleBetween(b, a);

            if (da < 0)
                return da + Constants.TwoPi;
            return da;
        }

        // angle ABC
        public static double InteriorAngle(Point2<double> a, Point2<double> b, Point2<double> c)
        {
            return Math.Abs(AngleBetween(b, c) - AngleBetween(b, a));
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
        public static bool IsCCW(Point2<double>[] ring)
        {
            if (ring.Length < 3)
                return false;
            int nPts = ring.Length - 1;

            // find highest point
            Point2<double> hiPt = ring[0];
            int hiIndex = 0;
            for (int i = 1; i < ring.Length; i++)
            {
                Point2<double> p = ring[i];
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
                if (iPrev < 0)
                    iPrev = nPts;
            } while (ring[iPrev].Equals(hiPt) && iPrev != hiIndex);

            // find distinct point after highest point
            int iNext = hiIndex;
            do
            {
                iNext = (iNext + 1) % nPts;
            } while (ring[iNext].Equals(hiPt) && iNext != hiIndex);

            Point2<double> prev = ring[iPrev];
            Point2<double> next = ring[iNext];

            // This check catches cases where the ring contains an A-B-A configuration of points. 
            // This can happen if the ring does not contain 3 distinct points (including the case where the input array has fewer than 4 elements), or it contains coincident line segments.
            if (prev.Equals(hiPt) || next.Equals(hiPt) || prev.Equals(next))
                return false;

            int disc = Coordinate2Utils.OrientationIndex(prev.X, prev.Y, hiPt.X, hiPt.Y, next.X, next.Y);

            // If disc is exactly 0, lines are collinear.  There are two possible cases:
            //  (1) the lines lie along the x axis in opposite directions (is handled by checking if next is left of prev ==> CCW)
            //  (2) the lines lie on top of one another (will never happen if the ring is valid, so don't check for it)
            bool isCCW = false;
            if (disc == 0) //poly is CCW if prev x is right of next x
                isCCW = (prev.X > next.X);
            else // if area is positive, points are ordered CCW
                isCCW = (disc > 0);
            return isCCW;
        }

        /// <summary>
        /// Computes the orientation of a point array (CCW or CW) to determine whether this point array indicates the interior is "inside" or "outside".
        /// For a CCW orientation, the interior is "inside" or "inclusionary"
        /// For a CW orientation, the interior is "outside" or "exclusionary"
        /// </summary>
        /// <param name="ring"></param>
        /// <returns></returns>
        public static Inclusivity BoundingType(Point2<double>[] ring)
        {
            if (IsCCW(ring))
                return Inclusivity.Inclusive;
            return Inclusivity.Exclusive;
        }

        public static double Distance(Point2<double> a, Point2<double> b)
        {
            double dX = b.X - a.X;
            double dY = b.Y - a.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }
    }
}
