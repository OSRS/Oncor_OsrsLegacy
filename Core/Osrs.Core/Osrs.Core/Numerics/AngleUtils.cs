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

namespace Osrs.Numerics
{
    public enum Orientation
    {
        Clockwise = -1,
        Collinear = 0,
        Counterclockwise = 1
    }

    public enum AngularUnit
    {
        Radians = 0,
        Degrees = 1,
        Gradians = 2,
        ArcSeconds = 3
    }

    public static class AngleUtils
    {
        #region Conversions
        public static double RadiansToDegrees(double radians)
        {
            return radians * Constants.DegreesPerRadian;
        }
        public static double DegreesToRadians(double degrees)
        {
            return degrees * Constants.RadiansPerDegree;
        }

        public static double RadiansToGradians(double radians)
        {
            return radians * Constants.RadiansPerGradian;
        }
        public static double GradiansToRadians(double gradians)
        {
            return gradians * Constants.GradiansPerRadian;
        }

        public static double RadiansToArcSeconds(double radians)
        {
            return Constants.RadianPerArcSecond * radians;
        }
        public static double ArcSecondsToRadians(double arcSeconds)
        {
            return Constants.ArcSecondPerRadian * arcSeconds;
        }

        public static double RadiansToArcMinutes(double radians)
        {
            return (Constants.RadianPerArcSecond * radians) / 60;
        }
        public static double ArcMinutesToRadians(double arcMinutes)
        {
            return Constants.ArcSecondPerRadian * (arcMinutes * 60);
        }

        public static double GetRadians(double a, AngularUnit au)
        {
            switch (au)
            {
                case AngularUnit.Radians:
                    return a;
                case AngularUnit.Degrees:
                    return AngleUtils.DegreesToRadians(a);
                case AngularUnit.Gradians:
                    return AngleUtils.GradiansToRadians(a);
                case AngularUnit.ArcSeconds:
                    return AngleUtils.ArcSecondsToRadians(a);
            }
            return double.NaN;
        }
        #endregion

        /// <summary>
        /// Normalize an angle in a 2&pi wide interval around a center value. <p>This method has three main uses:</p> <ul><li>normalize an angle between 0 and 2&pi;:<br/><code>a = MathUtils.normalizeAngle(a, Math.PI);</code></li><li>normalize an angle between -&pi; and +&pi;<br/><code>a = MathUtils.normalizeAngle(a, 0.0);</code></li><li>compute the angle between two defining angular positions:<br><code>angle = MathUtils.normalizeAngle(end, start) - start;</code></li></ul><p>Note that due to numerical accuracy and since &pi; cannot be represented exactly, the result interval is <em>closed</em>, it cannot be half-closed as would be more satisfactory in a purely mathematical view.</p>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static double NormalizeAngle(double a, double center)
        {
            return a - (Constants.TwoPi * Math.Floor((a + Math.PI - center) / Constants.TwoPi));
        }

        public static double NormalizeAngle(double a)
        {
            if (a < Constants.TwoPi)
                return a;
            if (Math.Abs(a) < Constants.TwoPi)
                return Constants.TwoPi - a;
            return NormalizeAngle(a, Math.PI);
        }

        public static double ToDegrees(double radians)
        {
            return radians * Constants.DegreesPerRadian;
        }

        public static double ToRadians(double degrees)
        {
            return degrees * Constants.RadiansPerDegree;
        }

        public static Orientation Orient(double angleA, double angleB)
        {
            double cp = Math.Sin(angleB - angleA);
            if (cp > 0)
                return Orientation.Counterclockwise;
            if (cp < 0)
                return Orientation.Clockwise;
            return Orientation.Collinear;
        }

        public static Orientation Orient(PlaneAngle angleA, PlaneAngle angleB)
        {
            double cp = Math.Sin(angleB.data - angleA.data);
            if (cp > 0)
                return Orientation.Counterclockwise;
            if (cp < 0)
                return Orientation.Clockwise;
            return Orientation.Collinear;
        }

        //DeterminantSign Code taken from JTS, direct implementation of INRI algorithm
        /*
         *************************************************************************
         * Author : Olivier Devillers
         * Olivier.Devillers@sophia.inria.fr
         * http:/www.inria.fr:/prisme/personnel/devillers/anglais/determinant.html
         **************************************************************************
         *
         **************************************************************************
         *              Copyright (c) 1995  by  INRIA Prisme Project
         *                  BP 93 06902 Sophia Antipolis Cedex, France.
         *                           All rights reserved
         **************************************************************************
         */
        /// <summary>
        /// Computes the sign of the determinant of the 2x2 matrix with the given entries, in a robust way.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        #pragma warning disable 0642
        public static int DeterminantSign(double x1, double y1, double x2, double y2)
        {
            // returns -1 if the determinant is negative,
            // returns  1 if the determinant is positive,
            // retunrs  0 if the determinant is null.
            int sign;
            double swap;
            double k;
            long count = 0;

            sign = 1;

            //  testing null entries
            if ((x1 == 0.0) || (y2 == 0.0))
            {
                if ((y1 == 0.0) || (x2 == 0.0))
                    return 0;
                else if (y1 > 0)
                {
                    if (x2 > 0)
                        return -sign;
                    else
                        return sign;
                }
                else
                {
                    if (x2 > 0)
                        return sign;
                    else
                        return -sign;
                }
            }
            if ((y1 == 0.0) || (x2 == 0.0))
            {
                if (y2 > 0)
                {
                    if (x1 > 0)
                        return sign;
                    else
                        return -sign;
                }
                else
                {
                    if (x1 > 0)
                        return -sign;
                    else
                        return sign;
                }
            }

            //  making y coordinates positive and permuting the entries
            //  so that y2 is the biggest one
            if (0.0 < y1)
            {
                if (0.0 < y2)
                {
                    if (y1 <= y2)
                        ;
                    else
                    //if (y2 > y1)
                    {
                        sign = -sign;
                        swap = x1;
                        x1 = x2;
                        x2 = swap;
                        swap = y1;
                        y1 = y2;
                        y2 = swap;
                    }
                }
                else
                {
                    if (y1 <= -y2)
                    {
                        sign = -sign;
                        x2 = -x2;
                        y2 = -y2;
                    }
                    else
                    {
                        swap = x1;
                        x1 = -x2;
                        x2 = swap;
                        swap = y1;
                        y1 = -y2;
                        y2 = swap;
                    }
                }
            }
            else
            {
                if (0.0 < y2)
                {
                    if (-y1 <= y2)
                    {
                        sign = -sign;
                        x1 = -x1;
                        y1 = -y1;
                    }
                    else
                    {
                        swap = -x1;
                        x1 = x2;
                        x2 = swap;
                        swap = -y1;
                        y1 = y2;
                        y2 = swap;
                    }
                }
                else
                {
                    if (y1 >= y2)
                    {
                        x1 = -x1;
                        y1 = -y1;
                        x2 = -x2;
                        y2 = -y2;
                    }
                    else
                    {
                        sign = -sign;
                        swap = -x1;
                        x1 = -x2;
                        x2 = swap;
                        swap = -y1;
                        y1 = -y2;
                        y2 = swap;
                    }
                }
            }

            //  making x coordinates positive
            //  if |x2| < |x1| one can conclude
            if (0.0 < x1)
            {
                if (0.0 < x2)
                {
                    if (x1 <= x2)
                        ;
                    else
                        //if (x2 > x1)
                        return sign;
                }
                else
                    return sign;
            }
            else
            {
                if (0.0 < x2)
                    return -sign;
                else
                {
                    if (x1 >= x2)
                    {
                        sign = -sign;
                        x1 = -x1;
                        x2 = -x2;
                    }
                    else
                        return -sign;
                }
            }

            //  all entries strictly positive   x1 <= x2 and y1 <= y2
            while (true)
            {
                count = count + 1;
                k = Math.Floor(x2 / x1);
                x2 = x2 - k * x1;
                y2 = y2 - k * y1;

                //  testing if R (new U2) is in U1 rectangle
                if (y2 < 0.0)
                    return -sign;
                if (y2 > y1)
                    return sign;

                //  finding R'
                if (x1 > x2 + x2)
                {
                    if (y1 < y2 + y2)
                        return sign;
                }
                else
                {
                    if (y1 > y2 + y2)
                        return -sign;
                    else
                    {
                        x2 = x1 - x2;
                        y2 = y1 - y2;
                        sign = -sign;
                    }
                }
                if (y2 == 0.0)
                {
                    if (x2 == 0.0)
                        return 0;
                    else
                        return -sign;
                }
                if (x2 == 0.0)
                    return sign;

                //  exchange 1 and 2 role.
                k = Math.Floor(x1 / x2);
                x1 = x1 - k * x2;
                y1 = y1 - k * y2;

                //  testing if R (new U1) is in U2 rectangle
                if (y1 < 0.0)
                    return sign;
                if (y1 > y2)
                    return -sign;

                //  finding R'
                if (x2 > x1 + x1)
                {
                    if (y2 < y1 + y1)
                        return -sign;
                }
                else
                {
                    if (y2 > y1 + y1)
                        return sign;
                    else
                    {
                        x1 = x2 - x1;
                        y1 = y2 - y1;
                        sign = -sign;
                    }
                }
                if (y1 == 0.0)
                {
                    if (x1 == 0.0)
                        return 0;
                    else
                        return sign;
                }
                if (x1 == 0.0)
                    return -sign;
            }
        }
        #pragma warning restore 0642

        public static double Normalize(double angle)
        {
            while (angle > Math.PI)
                angle -= Constants.TwoPi;
            while (angle <= Constants.NPi)
                angle += Constants.TwoPi;
            return angle;
        }

        public static double NormalizePositive(double angle)
        {
            while (angle < 0.0)
                angle += Constants.TwoPi;

            while (angle >= Constants.TwoPi)
                angle -= Constants.TwoPi;

            if (angle < 0.0 || angle > Constants.TwoPi)
                return 0.0;
            return angle;
        }

        public static double Diff(double angleA, double angleB)
        {
            double da;

            if (angleA < angleB)
                da = angleB - angleA;
            else
                da = angleA - angleB;

            if (da > Math.PI)
                da = Constants.TwoPi - da;

            return da;
        }
    }
}
