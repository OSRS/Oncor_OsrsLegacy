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
    public enum BoundLimit
    {
        Minimum = 0,
        Maximum = 1
    }

    public static class MathUtils
    {
        public static bool IsInfiniteOrNaN(this double val)
        {
            return double.IsInfinity(val) || double.IsNaN(val);
        }

        /// <summary>
        /// Evaluates f(x) = ax+bx+cx+... for the provided a,b,c...
        /// </summary>
        /// <param name="x"></param>
        /// <param name="coeff"></param>
        /// <returns></returns>
        public static double EvaluateLinear(double x, double[] coeff)
        {
            double res = 0.0d;
            for (int i = 0; i < coeff.Length; i++)
                res = checked(res + (x * coeff[i]));

            return res;
        }

        /// <summary>
        /// Evaluates f(x) = ax^i+ax^(i+1)... for the provided a,b,c,... with i starting at 0
        /// </summary>
        /// <param name="x"></param>
        /// <param name="coeff"></param>
        /// <returns></returns>
        public static double EvaluatePolynomial(double x, double[] coeff)
        {
            return EvaluatePolynomial(x, coeff, 0);
        }

        /// <summary>
        /// Evaluates f(x) = ax^i+ax^(i+1)... for the provided a,b,c,... with i starting at startPower
        /// </summary>
        /// <param name="x"></param>
        /// <param name="coeff"></param>
        /// <param name="startPower"></param>
        /// <returns></returns>
        public static double EvaluatePolynomial(double x, double[] coeff, int startPower)
        {
            double res = 0.0d;
            for (int i = 0; i < coeff.Length; i++)
                res = checked(res + (coeff[i] * Math.Pow(x, startPower + i)));
            return res;
        }

        #region Algebra
        public static double Pythagorian(double dX, double dY)
        {
            return Math.Sqrt(dX * dX + dY * dY);
        }

        public static double Pythagorian(double dX, double dY, double dZ)
        {
            return Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
        }

        public static double Pythagorian(double[] distancePartSet)
        {
            if (distancePartSet == null)
                return double.NaN;
            double dX = distancePartSet[0];
            double dY;
            for (int i = 1; i < distancePartSet.Length; i++)
            {
                dY = distancePartSet[i];
                dX += dY * dY;
            }
            return Math.Sqrt(dX);
        }

        public static double Inverse(this double a)
        {
            return 1.0 / a;
        }

        public static float Inverse(this float a)
        {
            return 1.0f / a;
        }

        public static long BinomialCoefficient(int n, int k)
        {
            CheckBinomial(n, k);
            if ((n == k) || (k == 0))
                return 1;
            if ((k == 1) || (k == n - 1))
                return n;
            if (k > n / 2)
                return BinomialCoefficient(n, n - k);

            long result = 1;
            if (n <= 61)
            {
                int i = n - k + 1;
                for (int j = 1; j <= k; j++)
                {
                    result = result * i / j;
                    i++;
                }
            }
            else if (n <= 66)
            {
                int i = n - k + 1;
                for (int j = 1; j <= k; j++)
                {
                    long d = Gcd(i, j);
                    result = (result / (j / d)) * (i / d);
                    i++;
                }
            }
            else
            {
                int i = n - k + 1;
                for (int j = 1; j <= k; j++)
                {
                    long d = Gcd(i, j);
                    result = (result / (j / d)) * (i / d);
                    i++;
                }
            }
            return result;
        }

        public static double BinomialCoefficientDouble(int n, int k)
        {
            CheckBinomial(n, k);
            if ((n == k) || (k == 0))
                return 1d;
            if ((k == 1) || (k == n - 1))
                return n;
            if (k > n / 2)
                return BinomialCoefficientDouble(n, n - k);
            if (n < 67)
                return BinomialCoefficientDouble(n, k);

            double result = 1d;
            for (int i = 1; i <= k; i++)
                result *= (double)(n - k + i) / (double)i;

            return Math.Floor(result + 0.5);
        }

        public static double BinomialCoefficientLog(int n, int k)
        {
            CheckBinomial(n, k);
            if ((n == k) || (k == 0))
                return 0;
            if ((k == 1) || (k == n - 1))
                return Math.Log(n);

            if (n < 67)
                return Math.Log(BinomialCoefficient(n, k));

            if (n < 1030)
                return Math.Log(BinomialCoefficientDouble(n, k));
            if (k > n / 2)
                return BinomialCoefficientLog(n, n - k);

            double logSum = 0;

            for (int i = n - k + 1; i <= n; i++)
                logSum += Math.Log(i);

            for (int i = 2; i <= k; i++)
                logSum -= Math.Log(i);

            return logSum;
        }

        private static void CheckBinomial(int n, int k)
        {
            if (n < k)
                throw new MathException("n >= k");
            if (n < 0)
                throw new SignException("n");
        }
        #endregion

        #region Trig Functions
        /// <summary>
        /// A numerically precise version of sin(theta * pi)
        /// </summary>
        public static double SinPiT(double theta)
        {
            double res = Math.Abs(theta) % 2.0;
            if (res < 0.25)
                res = Math.Sin(res * Math.PI);
            else if (res < 0.75)
                res = Math.Cos((res - 0.5) * Math.PI);
            else if (res < 1.25)
                res = -Math.Sin((res - 1.0) * Math.PI);
            else if (res < 1.75)
                res = -Math.Cos((res - 1.5) * Math.PI);
            else
                res = Math.Sin((res - 2.0) * Math.PI);

            return theta < 0 ? -res : res;
        }

        /// <summary>
        /// A numerically precise version of |sin(theta * pi)|
        /// </summary>
        private static double AbsSinPi(double theta)
        {
            double res = Math.Abs(theta) % 1.0;

            if (res < 0.25)
                res = Math.Sin(res * Math.PI);
            else if (res < 0.75)
                res = Math.Cos((res - 0.5) * Math.PI);
            else
                res = Math.Sin((res - 1.0) * Math.PI);

            return Math.Abs(res);
        }

        public static double SecH(double radians)
        {
            return 1 / Math.Cosh(radians);
        }

        public static double CscH(double radians)
        {
            return 1 / Math.Sinh(radians);
        }

        public static double ASec(double radians)
        {
            return Math.Acos(1 / radians);
        }

        public static double ACsc(double radians)
        {
            return Math.Asin(1 / radians);
        }

        public static double ASinH(double radians)
        {
            return Math.Log(Math.Sqrt(radians * radians + 1) + radians);
        }

        public static double ACosH(double radians)
        {
            return Math.Log(Math.Sqrt(radians * radians - 1) + radians);
        }

        public static double ATanH(double radians)
        {
            if (radians == 0)
                return 0;

            return Math.Log((radians + 1) / (1 - radians)) * 0.5;
        }

        public static double ACotH(double radians)
        {
            if (radians == 0)
                return 0;
            return ATanH(1 / radians);
        }
        #endregion

        #region Geometry Functions
        #region Point-Point Distances
        public static int Distance(int fromX, int fromY, int toX, int toY)
        {
            long dY = toY - fromY;
            long dX = toX - fromX;
            return (int)Math.Sqrt((dY * dY) + (dX * dX));
        }

        public static long Distance(long fromX, long fromY, long toX, long toY)
        {
            long dY = toY - fromY;
            long dX = toX - fromX;
            return (long)Math.Sqrt((dY * dY) + (dX * dX));
        }

        public static float Distance(float fromX, float fromY, float toX, float toY)
        {
            double dY = toY - fromY;
            double dX = toX - fromX;
            return (float)Math.Sqrt(dY * dY + dX * dX);
        }

        public static double Distance(double fromX, double fromY, double toX, double toY)
        {
            double dY = toY - fromY;
            double dX = toX - fromX;
            return Math.Sqrt(dY * dY + dX * dX);
        }

        public static int ManhattanDistance(int fromX, int fromY, int toX, int toY)
        {
            long dY = toY - fromY;
            long dX = toX - fromX;
            return (int)(dY + dX);
        }

        public static long ManhattanDistance(long fromX, long fromY, long toX, long toY)
        {
            long dY = toY - fromY;
            long dX = toX - fromX;
            return dY + dX;
        }

        public static float ManhattanDistance(float fromX, float fromY, float toX, float toY)
        {
            double dY = toY - fromY;
            double dX = toX - fromX;
            return (float)(dY + dX);
        }

        public static double ManhattanDistance(double fromX, double fromY, double toX, double toY)
        {
            double dY = toY - fromY;
            double dX = toX - fromX;
            return dY + dX;
        }
        #endregion

        #region Point-Point Directions
        public static double Direction(int fromX, int fromY, int toX, int toY)
        {
            int dY = toY - fromY;
            int dX = toX - fromX;
            return Math.Atan2(dY, dX);
        }
        public static double Direction(long fromX, long fromY, long toX, long toY)
        {
            long dY = toY - fromY;
            long dX = toX - fromX;
            return Math.Atan2(dY, dX);
        }
        public static double Direction(double fromX, double fromY, double toX, double toY)
        {
            double dY = toY - fromY;
            double dX = toX - fromX;
            return Math.Atan2(dY, dX);
        }

        public static double CompassDirection(int fromX, int fromY, int toX, int toY)
        {
            int dY = toY - fromY;
            int dX = toX - fromX;
            if (dX == 0)
            {
                if (dY < 0) //straight down
                    return Math.PI;
                return 0.0; //straight up
            }
            double dd = Math.Atan2(dY, dX);
            if (dd < 0.0)
                return Constants.TwoPi + dd;
            return dd;
        }
        public static double CompassDirection(long fromX, long fromY, long toX, long toY)
        {
            long dY = toY - fromY;
            long dX = toX - fromX;
            if (dX == 0)
            {
                if (dY < 0) //straight down
                    return Math.PI;
                return 0.0; //straight up
            }
            double dd = Math.Atan2(dY, dX);
            if (dd < 0.0)
                return Constants.TwoPi + dd;
            return dd;
        }
        public static double CompassDirection(double fromX, double fromY, double toX, double toY)
        {
            double dY = toY - fromY;
            double dX = toX - fromX;
            if (dX == 0.0)
            {
                if (dY < 0.0) //straight down
                    return Math.PI;
                return 0.0; //straight up
            }
            dX = Math.Atan2(dY, dX);
            if (dX < Constants.HalfPi)
                return Constants.HalfPi - dX;
            return Constants.FiveHalfsPi - dX;
        }
        #endregion

        #region General Plane Geometry
        public static double CircleCircumference(double radius)
        {
            return Constants.TwoPi * radius;
        }

        public static double CircularArcLength(double angle, double radius)
        {
            return radius * angle;
        }

        public static double CircularArcArea(double angle, double radius)
        {
            return (angle / Constants.TwoPi) * Math.PI * Math.Pow(radius, 2);
        }

        public static double RegularPolygonArea(int numberOfSides, double lengthOfSide)
        {
            return (double)numberOfSides * Math.Pow(lengthOfSide, 2) * Math.Sin(Constants.TwoPi / numberOfSides) * 0.5;
        }

        public static double RegularPolygonAngleSum(int numberOfSides)
        {
            return (numberOfSides - 2) * Math.PI;
        }

        public static double RegularPolygonAngle(int numberOfSides)
        {
            return RegularPolygonAngleSum(numberOfSides) / numberOfSides;
        }
        #endregion

        #region Area/Volume
        public static double TriangleArea(double lenA, double lenB, double lenC)
        {
            double t = (lenA + lenB + lenC) * 0.5;
            return Math.Sqrt(t * (t - lenA) * (t - lenB) * (t - lenC));
        }

        public static double TriangleArea(double length, double height)
        {
            return length * height * 0.5;
        }

        public static double TriangleArea(double sideLength)
        {
            return Constants.QuarterSqrt3 * Math.Pow(sideLength, 2);
        }

        public static double QuadrilateralDiagonalArea(double diagAC, double diagBD, double lenA, double lenB, double lenC, double lenD)
        {
            double t = Math.Pow(lenB, 2) + Math.Pow(lenD, 2) - Math.Pow(lenA, 2) - Math.Pow(lenC, 2);
            t = Math.Pow(t, 2);
            double t2 = Math.Sqrt(Math.Pow(diagAC, 2) * Math.Pow(diagBD, 2) * 4) * 0.25;
            return Math.Sqrt(t - t2);
        }

        public static double QuadrilateralAngleArea(double angleARad, double angleBRad, double lenA, double lenB, double lenC, double lenD)
        {
            double t = (lenA + lenB + lenC + lenD) * 0.5;
            double t2 = lenA * lenB * lenC * lenD * Math.Pow(Math.Cos((angleARad + angleBRad) * 0.5), 2);
            t2 = (t - lenA) * (t - lenB) * (t - lenC) * (t - lenD);
            return Math.Sqrt(t - t2);
        }

        public static double TrapezoidArea(double height, double lenA, double lenB)
        {
            return height * 0.5 * (lenA + lenB);
        }

        public static double CircleArea(double radius)
        {
            return Constants.TwoPi * Math.Pow(radius, 2);
        }

        public static double EllipseArea(double minorRadius, double majorRadius)
        {
            return Math.PI * minorRadius * majorRadius;
        }

        public static double CubeVolume(double dimension)
        {
            return Math.Pow(dimension, 3);
        }

        public static double CubeArea(double dimension)
        {
            return Math.Pow(dimension, 2) * 6;
        }

        public static double RectangleVolume(double length, double width, double height)
        {
            return length * width * height;
        }

        public static double RectangleArea(double length, double width, double height)
        {
            return ((length * height) + (length * width) + (height * width)) * 2;
        }

        public static double PrismArea(double perimeterBase, double areaBase, double length)
        {
            return (perimeterBase * length) + (areaBase * 2);
        }

        public static double CylinderVolume(double radius, double height)
        {
            return Math.PI * Math.Pow(radius, 2) * height;
        }

        public static double CylinderArea(double radius, double height)
        {
            return (Constants.TwoPi * Math.Pow(radius, 2)) + (Constants.TwoPi * radius * height);
        }

        public static double PyramidVolume(double baseWidth, double baseLength, double height)
        {
            return baseLength * baseWidth * height * Constants.OneThird;
        }

        public static double ConeVolume(double baseRadius, double height)
        {
            return Math.PI * Math.Pow(baseRadius, 2) * height * Constants.OneThird;
        }

        public static double SphereVolume(double radius)
        {
            return Constants.FourThirdsPi * Math.Pow(radius, 3);
        }

        public static double SphereArea(double radius)
        {
            return Constants.FourPi * Math.Pow(radius, 2);
        }

        public static double EllipsoidVolume(double radius1, double radius2, double radius3)
        {
            return radius1 * radius2 * radius3 * Constants.FourThirdsPi;
        }
        #endregion
        #endregion

        #region Powers/Roots
        private static readonly double[] pows10 = new double[] { 1E0, 1E1, 1E2, 1E3, 1E4, 1E5, 1E6, 1E7, 1E8, 1E9, 1E10, 1E11, 1E12, 1E13, 1E14, 1E15, 1E16 };
        public static double Pow10(uint power)
        {
            if (power < 17)
                return pows10[power];
            return Math.Pow(10, power);
        }

        public static double NthRoot(double a, double n)
        {
            return Math.Pow(a, 1 / n);
        }

        public static float NthRoot(float a, float n)
        {
            return (float)NthRoot((double)a, (double)n);
        }

        public static ulong NthRoot(ulong a, ulong n)
        {
            return (ulong)NthRoot((double)a, (double)n);
        }

        public static long NthRoot(long a, long n)
        {
            return (long)NthRoot((double)a, (double)n);
        }

        public static uint NthRoot(uint a, uint n)
        {
            return (uint)NthRoot((double)a, (double)n);
        }

        public static int NthRoot(int a, int n)
        {
            return (int)NthRoot((double)a, (double)n);
        }

        public static ushort NthRoot(ushort a, ushort n)
        {
            return (ushort)NthRoot((double)a, (double)n);
        }

        public static short NthRoot(short a, short n)
        {
            return (short)NthRoot((double)a, (double)n);
        }

        /// <summary>
        /// Integer Power
        /// </summary>
        public static long IntPow(long radix, uint exponent)
        {
            return (long)Math.Round(Math.Pow(radix, exponent));
        }

        /// <summary>
        /// Raises 2 to the provided integer exponent.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static int IntPow2(int exponent)
        {
            if (exponent < 0 || exponent >= 31)
                throw new ArgumentOutOfRangeException("exponent");
            return 1 << exponent;
        }

        /// <summary>
        /// Evaluates the logarithm to base 2 of the provided integer value.
        /// </summary>
        public static int IntLog2(int x)
        {
            if (x <= 65536)
            {
                if (x <= 256)
                {
                    if (x <= 16)
                    {
                        if (x <= 4)
                        {
                            if (x <= 2)
                            {
                                if (x <= 1)
                                    return 0;
                                return 1;
                            }
                            return 2;
                        }
                        if (x <= 8)
                            return 3;
                        return 4;
                    }
                    if (x <= 64)
                    {
                        if (x <= 32)
                            return 5;
                        return 6;
                    }
                    if (x <= 128)
                        return 7;
                    return 8;
                }

                if (x <= 4096)
                {
                    if (x <= 1024)
                    {
                        if (x <= 512)
                            return 9;
                        return 10;
                    }
                    if (x <= 2048)
                        return 11;
                    return 12;
                }
                if (x <= 16384)
                {
                    if (x <= 8192)
                        return 13;
                    return 14;
                }
                if (x <= 32768)
                    return 15;
                return 16;
            }
            if (x <= 16777216)
            {
                if (x <= 1048576)
                {
                    if (x <= 262144)
                    {
                        if (x <= 131072)
                            return 17;
                        return 18;
                    }
                    if (x <= 524288)
                        return 19;
                    return 20;
                }
                if (x <= 4194304)
                {
                    if (x <= 2097152)
                        return 21;
                    return 22;
                }
                if (x <= 8388608)
                    return 23;
                return 24;
            }
            if (x <= 268435456)
            {
                if (x <= 67108864)
                {
                    if (x <= 33554432)
                        return 25;
                    return 26;
                }
                if (x <= 134217728)
                    return 27;
                return 28;
            }
            if (x <= 1073741824)
            {
                if (x <= 536870912)
                    return 29;
                return 30;
            }
            return 31;
        }

        /// <summary>
        /// Returns the smallest integer power of two bigger or equal to the value. 
        /// </summary>
        public static int CeilingToPowerOf2(int value)
        {
            if (value <= 0)
                return 0;
            return IntPow2(IntLog2(value));
        }

        /// <summary>
        /// Returns the biggest integer power of two smaller or equal to the value. 
        /// </summary>
        public static int FloorToPowerOf2(int value)
        {
            int log = IntLog2(value);
            int retHalf = log == 0 ? 0 : IntPow2(log - 1);
            return retHalf == value >> 1 ? value : retHalf;
        }

        /// <summary>
        /// Returns the logarithm for base <code>b</code> of <code>x</code>.
        /// </summary>
        /// <param name="numericBase"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Log(double baseNumber, double x)
        {
            return Math.Log(x) / Math.Log(baseNumber);
        }
        #endregion

        #region Divisors/Multiples
        public static bool EvenlyDivisible(decimal a, decimal b)
        {
            return a % b == 0;
        }

        public static bool EvenlyDivisible(double a, double b)
        {
            return a % b == 0;
        }

        public static bool EvenlyDivisible(float a, float b)
        {
            return a % b == 0;
        }

        public static bool EvenlyDivisible(ulong a, ulong b)
        {
            return a % b == 0;
        }

        public static bool EvenlyDivisible(long a, long b)
        {
            return a % b == 0;
        }

        public static bool EvenlyDivisible(uint a, uint b)
        {
            return a % b == 0;
        }

        public static bool EvenlyDivisible(int a, int b)
        {
            return a % b == 0;
        }

        public static bool EvenlyDivisible(ushort a, ushort b)
        {
            return a % b == 0;
        }

        public static bool EvenlyDivisible(short a, short b)
        {
            return a % b == 0;
        }

        public static decimal Gcd(decimal a, decimal b)
        {
            decimal rem = a % b;
            a = b;
            b = rem;

            while (rem > 0)
            {
                rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        public static double Gcd(double a, double b)
        {
            double rem = a % b;
            a = b;
            b = rem;

            while (rem > double.Epsilon)
            {
                rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        public static float Gcd(float a, float b)
        {
            float rem = a % b;
            a = b;
            b = rem;

            while (rem > float.Epsilon)
            {
                rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        public static ulong Gcd(ulong a, ulong b)
        {
            ulong rem = a % b;
            a = b;
            b = rem;

            while (rem > 0)
            {
                rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        public static long Gcd(long a, long b)
        {
            long rem = a % b;
            a = b;
            b = rem;

            while (rem > 0)
            {
                rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        public static uint Gcd(uint a, uint b)
        {
            uint rem = a % b;
            a = b;
            b = rem;

            while (rem > 0)
            {
                rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        public static int Gcd(int a, int b)
        {
            int rem = a % b;
            a = b;
            b = rem;

            while (rem > 0)
            {
                rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        public static ushort Gcd(ushort a, ushort b)
        {
            return (ushort)Gcd((int)a, (int)b);
        }

        public static short Gcd(short a, short b)
        {
            return (short)Gcd((int)a, (int)b);
        }

        public static decimal Lcm(decimal a, decimal b)
        {
            return (a * b) / Gcd(a, b);
        }

        public static double Lcm(double a, double b)
        {
            return (a * b) / Gcd(a, b);
        }

        public static float Lcm(float a, float b)
        {
            return (a * b) / Gcd(a, b);
        }

        public static ulong Lcm(ulong a, ulong b)
        {
            return (a * b) / Gcd(a, b);
        }

        public static long Lcm(long a, long b)
        {
            return (a * b) / Gcd(a, b);
        }

        public static uint Lcm(uint a, uint b)
        {
            return (a * b) / Gcd(a, b);
        }

        public static int Lcm(int a, int b)
        {
            return (a * b) / Gcd(a, b);
        }

        public static ushort Lcm(ushort a, ushort b)
        {
            return (ushort)Lcm((int)a, (int)b);
        }

        public static short Lcm(short a, short b)
        {
            return (short)Lcm((int)a, (int)b);
        }
        #endregion

        #region General
        public static bool IsNegativeSensitive(double val)
        {
            return val == 0.0 && (1.0 / val < 0.0);
        }

        public static bool IsNegativeZero(double val)
        {
            return val == 0.0 && (1.0 / val < 0.0);
        }

        public static bool IsNegativeSensitive(float val)
        {
            return val == 0.0f && (1.0f / val < 0.0f);
        }

        public static bool IsNegativeZero(float val)
        {
            return val == 0.0f && (1.0f / val < 0.0f);
        }

        public static int FloorDiv(int x, int y)
        {
            int i = x / y;
            if (x >= 0)
            {
                if (y > 0 || x % y == 0)
                    return i;
                else
                    return i--;
            }
            else if (y > 0 && x % y != 0)
                return i--;
            return i;
        }

        public static long FloorDiv(long x, long y)
        {
            long i = x / y;
            if (x >= 0)
            {
                if (y > 0 || x % y == 0)
                    return i;
                else
                    return i--;
            }
            else if (y > 0 && x % y != 0)
                return i--;
            return i;
        }

        public static int FloorRem(int x, int y)
        {
            if (y == -1 || y == 1)
                return 0; //no remainder possible

            int i = x % y;

            if (x >= 0)
            {
                if (y > 0)
                    return i;
                else if (i == 0)
                    return 0;
                else
                    return y + i;
            }
            else
            {
                if (y > 0)
                {
                    if (i == 0)
                        return 0;
                    return y + i;
                }
            }
            return i;
        }

        public static long FloorRem(long x, long y)
        {
            if (y == -1 || y == 1)
                return 0; //no remainder possible

            long i = x % y;

            if (x >= 0)
            {
                if (y > 0)
                    return i;
                else if (i == 0)
                    return 0;
                else
                    return y + i;
            }
            else
            {
                if (y > 0)
                {
                    if (i == 0)
                        return 0;
                    return y + i;
                }
            }
            return i;
        }

        /// <summary>
        /// Increments a floating point number to the next bigger number representable by the data type.
        /// </summary>
        public static double Increment(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
                return value;

            long signed64 = BitConverter.DoubleToInt64Bits(value);
            if (signed64 < 0)
                signed64--;
            else
                signed64++;

            if (signed64 == -9223372036854775808)
                return 0;

            value = BitConverter.Int64BitsToDouble(signed64);

            return double.IsNaN(value) ? double.NaN : value;
        }

        /// <summary>
        /// Decrements a floating point number to the next smaller number representable by the data type.
        /// </summary>
        public static double Decrement(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
                return value;

            long signed64 = BitConverter.DoubleToInt64Bits(value);
            if (signed64 == 0)
                return -double.Epsilon;

            if (signed64 < 0)
                signed64++;
            else
                signed64--;

            value = BitConverter.Int64BitsToDouble(signed64);

            return double.IsNaN(value) ? double.NaN : value;
        }

        /// <summary>
        /// Returns n!. Shorthand for n Factorial, the product of the numbers <code>1,...,n</code>.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static long Factorial(int n)
        {
            if (n < 0)
                throw new SignException("n");
            if (n > 20)
                throw new OverflowException("n!");
            return Factorials[n];
        }
        private static readonly long[] Factorials = new long[] {1L, 1L, 2L, 6L, 24L, 120L, 720L, 5040L, 40320L,
            362880L,3628800L,39916800L,479001600L,6227020800L,87178291200L,1307674368000L,20922789888000L,
            355687428096000L,6402373705728000L,121645100408832000L,2432902008176640000L};


        /// <summary>
        /// Returns n!. Shorthand for n Factorial, the product of the numbers <code>1,...,n</code> as a <code>double</code>.
        /// </summary>
        /// <param name="n">value to compute</param>
        /// <returns><code>n!</code></returns>
        public static double FactorialDouble(int n)
        {
            if (n < 0)
                throw new SignException("n");
            if (n < 21)
                return Factorial(n);
            if (n > 170)
                throw new OverflowException("n!");
            return Math.Floor(Math.Exp(FactorialLog(n)) + 0.5);
        }

        /// <summary>
        /// Returns the natural logarithm of n!.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double FactorialLog(int n)
        {
            if (n < 0)
                throw new SignException("n");
            if (n < 21)
                return Math.Log(Factorial(n));
            double logSum = 0;
            for (int i = 2; i <= n; i++)
                logSum += Math.Log(i);
            return logSum;
        }

        #region Even/Odd
        public static bool IsEven(decimal a)
        {
            return a % 2 == 0;
        }

        public static bool IsEven(double a)
        {
            return a % 2 == 0;
        }

        public static bool IsEven(float a)
        {
            return a % 2 == 0;
        }

        public static bool IsEven(ulong a)
        {
            return a % 2 == 0;
        }

        public static bool IsEven(long a)
        {
            return a % 2 == 0;
        }

        public static bool IsEven(uint a)
        {
            return a % 2 == 0;
        }

        public static bool IsEven(int a)
        {
            return a % 2 == 0;
        }

        public static bool IsEven(ushort a)
        {
            return a % 2 == 0;
        }

        public static bool IsEven(short a)
        {
            return a % 2 == 0;
        }

        public static bool IsOdd(decimal a)
        {
            return a % 2 != 0;
        }

        public static bool IsOdd(double a)
        {
            return a % 2 != 0;
        }

        public static bool IsOdd(float a)
        {
            return a % 2 != 0;
        }

        public static bool IsOdd(ulong a)
        {
            return a % 2 != 0;
        }

        public static bool IsOdd(long a)
        {
            return a % 2 != 0;
        }

        public static bool IsOdd(uint a)
        {
            return a % 2 != 0;
        }

        public static bool IsOdd(int a)
        {
            return a % 2 != 0;
        }

        public static bool IsOdd(ushort a)
        {
            return a % 2 != 0;
        }

        public static bool IsOdd(short a)
        {
            return a % 2 != 0;
        }
        #endregion

        #region GAMMA/DIGAMMA
        /// <summary>
        /// Returns the natural logarithm of Gamma for a real value &gt; 0.
        /// </summary>
        /// <returns>A value ln|Gamma(value))| for value &gt; 0</returns>
        public static double GammaLn(double value)
        {
            double x, y, ser, temp;
            double[] coefficient = new double[]{
                    76.18009172947146,
                    -86.50532032941677,
                    24.01409824083091,
                    -1.231739572450155,
                    0.1208650973866179e-2,
                    -0.5395239384953e-5
                };

            y = x = value;
            temp = x + 5.5;
            temp -= ((x + 0.5) * Math.Log(temp));
            ser = 1.000000000190015;

            for (int j = 0; j <= 5; j++)
                ser += (coefficient[j] / ++y);

            return -temp + Math.Log(2.5066282746310005 * ser / x);
        }

        /// <summary>
        /// Returns the gamma function for real values (except at 0, -1, -2, ...).
        /// For numeric stability, consider to use GammaLn for positive values.
        /// </summary>
        /// <returns>A value Gamma(value) for value != 0,-1,-2,...</returns>
        public static double Gamma(double value)
        {
            if (value > 0.0)
                return Math.Exp(GammaLn(value));

            double reflection = 1.0 - value;
            double s = Math.Sin(Math.PI * reflection);

            if (Equals(0.0, s))
                return double.NaN; // singularity, undefined

            return Math.PI / (s * Math.Exp(GammaLn(reflection)));
        }

        /// <summary>
        /// Returns the regularized lower incomplete gamma function
        /// P(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0.
        /// </summary>
        public static double GammaRegularized(double a, double x)
        {
            const int MaxIterations = 100;
            double eps = double.Epsilon;
            double fpmin = double.Epsilon / eps;

            if (a < 0.0 || x < 0.0)
                throw new ArgumentOutOfRangeException("a and x cannot be <0");

            double gln = GammaLn(a);
            if (x < a + 1.0)
            {
                // Series Representation
                if (x <= 0.0)
                    return 0.0;
                else
                {
                    double ap = a;
                    double del, sum = del = 1.0 / a;

                    for (int n = 0; n < MaxIterations; n++)
                    {
                        ++ap;
                        del *= x / ap;
                        sum += del;

                        if (Math.Abs(del) < Math.Abs(sum) * eps)
                            return sum * Math.Exp(-x + a * Math.Log(x) - gln);
                    }
                }
            }
            else
            {
                // Continued fraction representation

                double b = x + 1.0 - a;
                double c = 1.0 / fpmin;
                double d = 1.0 / b;
                double h = d;

                for (int i = 1; i <= MaxIterations; i++)
                {
                    double an = -i * (i - a);
                    b += 2.0;
                    d = an * d + b;

                    if (Math.Abs(d) < fpmin)
                        d = fpmin;

                    c = b + an / c;

                    if (Math.Abs(c) < fpmin)
                        c = fpmin;

                    d = 1.0 / d;
                    double del = d * c;
                    h *= del;

                    if (Math.Abs(del - 1.0) <= eps)
                        return 1.0 - Math.Exp(-x + a * Math.Log(x) - gln) * h;
                }
            }
            throw new IterationsExceededException("a");
        }

        /// <summary>
        /// Returns the digamma (psi) function of real values (except at 0, -1, -2, ...).
        /// Digamma is the logarithmic derivative of the <see cref="Gamma"/> function.
        /// </summary>
        public static double Digamma(double x)
        {
            double y = 0;
            double nz = 0.0;
            bool negative = (x <= 0);

            if (negative)
            {
                double q = x;
                double p = Math.Floor(q);
                negative = true;

                if (AlmostEqual(p, q))
                    return double.NaN; // singularity, undefined
                nz = q - p;
                if (nz != 0.5)
                {
                    if (nz > 0.5)
                    {
                        p = p + 1.0;
                        nz = q - p;
                    }

                    nz = Math.PI / Math.Tan(Math.PI * nz);
                }
                else
                    nz = 0.0;
                x = 1.0 - x;
            }

            if ((x <= 10.0) && (x == Math.Floor(x)))
            {
                y = 0.0;
                int n = (int)Math.Floor(x);

                for (int i = 1; i <= n - 1; i++)
                    y = y + 1.0 / i;
                y = y - Constants.EulerGamma;
            }
            else
            {
                double s = x;
                double w = 0.0;

                while (s < 10.0)
                {
                    w = w + 1.0 / s;
                    s = s + 1.0;
                }

                if (s < 1.0e17)
                {
                    double z = 1.0 / (s * s);
                    double polv = 8.33333333333333333333e-2;
                    polv = polv * z - 2.10927960927960927961e-2;
                    polv = polv * z + 7.57575757575757575758e-3;
                    polv = polv * z - 4.16666666666666666667e-3;
                    polv = polv * z + 3.96825396825396825397e-3;
                    polv = polv * z - 8.33333333333333333333e-3;
                    polv = polv * z + 8.33333333333333333333e-2;
                    y = z * polv;
                }
                else
                    y = 0.0;
                y = Math.Log(s) - 0.5 / s - y - w;
            }
            if (negative)
                return y - nz;
            return y;
        }
        #endregion

        #region BETA
        /// <summary>
        /// Returns the Euler Beta function of real valued z > 0, w > 0.
        /// Beta(z,w) = Beta(w,z).
        /// </summary>
        public static double Beta(double z, double w)
        {
            return Math.Exp(GammaLn(z) + GammaLn(w) - GammaLn(z + w));
        }

        /// <summary>
        /// Returns the natural logarithm of the Euler Beta function of real valued z > 0, w > 0.
        /// BetaLn(z,w) = BetaLn(w,z).
        /// </summary>
        public static double BetaLn(double z, double w)
        {
            return GammaLn(z) + GammaLn(w) - GammaLn(z + w);
        }

        /// <summary>
        /// Returns the regularized lower incomplete beta function
        /// I_x(a,b) = 1/Beta(a,b) * int(t^(a-1)*(1-t)^(b-1),t=0..x) for real a &gt; 0, b &gt; 0, 1 &gt;= x &gt;= 0.
        /// </summary>
        public static double BetaRegularized(double a, double b, double x)
        {
            if (x < 0.0 || a < 0.0 || b < 0.0)
                throw new SignException("a,b,x cannot be <0");
            if (x > 1.0)
                throw new ArgumentOutOfRangeException("x must be in range [0.0-1.0]");
            double bt = (x == 0.0 || x == 1.0) ? 0.0 : Math.Exp(GammaLn(a + b) - GammaLn(a) - GammaLn(b) + a * Math.Log(x) + b * Math.Log(1.0 - x));

            bool symmetryTransformation = (x >= (a + 1.0) / (a + b + 2.0));

            const int MaxIterations = 100;
            double eps = Constants.RelativeAccuracy;
            double fpmin = double.Epsilon / eps;

            if (symmetryTransformation)
            {
                x = 1.0 - x;
                double swap = a;
                a = b;
                b = swap;
            }

            double qab = a + b;
            double qap = a + 1.0;
            double qam = a - 1.0;
            double c = 1.0;
            double d = 1.0 - qab * x / qap;

            if (Math.Abs(d) < fpmin)
                d = fpmin;

            d = 1.0 / d;
            double h = d;

            for (int m = 1, m2 = 2; m <= MaxIterations; m++, m2 += 2)
            {
                double aa = m * (b - m) * x / ((qam + m2) * (a + m2));
                d = 1.0 + aa * d;

                if (Math.Abs(d) < fpmin)
                    d = fpmin;

                c = 1.0 + aa / c;
                if (Math.Abs(c) < fpmin)
                    c = fpmin;

                d = 1.0 / d;
                h *= d * c;
                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
                d = 1.0 + aa * d;

                if (Math.Abs(d) < fpmin)
                    d = fpmin;

                c = 1.0 + aa / c;

                if (Math.Abs(c) < fpmin)
                    c = fpmin;

                d = 1.0 / d;
                double del = d * c;
                h *= del;

                if (Math.Abs(del - 1.0) <= eps)
                {
                    if (symmetryTransformation)
                        return 1.0 - bt * h / a;
                    return bt * h / a;
                }
            }
            throw new IterationsExceededException("a,b");
        }
        #endregion

        #region ERF
        /// <summary>
        /// Returns the error function erf(x) = 2/sqrt(pi) * int(exp(-t^2),t=0..x)
        /// </summary>
        public static double Erf(double x)
        {
            if (double.IsNegativeInfinity(x))
                return -1.0;

            if (double.IsPositiveInfinity(x))
                return 1.0;

            return x < 0.0 ? -GammaRegularized(0.5, x * x) : GammaRegularized(0.5, x * x);
        }

        /// <summary>
        /// Returns the inverse error function erf^-1(x).
        /// </summary>
        /// <remarks>
        /// <p>The algorithm uses a minimax approximation by rational functions
        /// and the result has a relative error whose absolute value is less
        /// than 1.15e-9.</p>
        /// </remarks>
        public static double ErfInverse(double x)
        {
            if (x < -1.0 || x > 1.0)
                throw new ArgumentOutOfRangeException("p must be in range [-1.0,1.0]");

            x = 0.5 * (x + 1.0);

            // Define break-points.
            double plow = 0.02425;
            double phigh = 1 - plow;

            double q;

            // Rational approximation for lower region:
            if (x < plow)
            {
                q = Math.Sqrt(-2 * Math.Log(x));
                return (((((erfinv_c[0] * q + erfinv_c[1]) * q + erfinv_c[2]) * q + erfinv_c[3]) * q + erfinv_c[4]) * q + erfinv_c[5]) /
                    ((((erfinv_d[0] * q + erfinv_d[1]) * q + erfinv_d[2]) * q + erfinv_d[3]) * q + 1)
                    * Constants.SqrtOneHalf;
            }

            // Rational approximation for upper region:
            if (phigh < x)
            {
                q = Math.Sqrt(-2 * Math.Log(1 - x));
                return -(((((erfinv_c[0] * q + erfinv_c[1]) * q + erfinv_c[2]) * q + erfinv_c[3]) * q + erfinv_c[4]) * q + erfinv_c[5]) /
                    ((((erfinv_d[0] * q + erfinv_d[1]) * q + erfinv_d[2]) * q + erfinv_d[3]) * q + 1)
                    * Constants.SqrtOneHalf;
            }

            // Rational approximation for central region:
            q = x - 0.5;
            double r = q * q;
            return (((((erfinv_a[0] * r + erfinv_a[1]) * r + erfinv_a[2]) * r + erfinv_a[3]) * r + erfinv_a[4]) * r + erfinv_a[5]) * q /
                (((((erfinv_b[0] * r + erfinv_b[1]) * r + erfinv_b[2]) * r + erfinv_b[3]) * r + erfinv_b[4]) * r + 1)
                * Constants.SqrtOneHalf;
        }

        private static readonly double[] erfinv_a = {
                -3.969683028665376e+01, 2.209460984245205e+02,
                -2.759285104469687e+02, 1.383577518672690e+02,
                -3.066479806614716e+01, 2.506628277459239e+00
            };

        private static readonly double[] erfinv_b = {
                -5.447609879822406e+01, 1.615858368580409e+02,
                -1.556989798598866e+02, 6.680131188771972e+01,
                -1.328068155288572e+01
            };

        private static readonly double[] erfinv_c = {
                -7.784894002430293e-03, -3.223964580411365e-01,
                -2.400758277161838e+00, -2.549732539343734e+00,
                4.374664141464968e+00, 2.938163982698783e+00
            };

        private static readonly double[] erfinv_d = {
                7.784695709041462e-03, 3.224671290700398e-01,
                2.445134137142996e+00, 3.754408661907416e+00
            };
        #endregion

        #region HARMONIC
        /// <summary>
        /// Evaluates the n-th harmonic number Hn = sum(1/k,k=1..n).
        /// </summary>
        /// <param name="n">n >= 0</param>
        /// <remarks>
        /// See <a http="http://en.wikipedia.org/wiki/Harmonic_Number">Wikipedia - Harmonic Number</a>
        /// </remarks>
        public static double HarmonicNumber(int n)
        {
            if (n < 0)
                throw new SignException("n");

            if (n >= HarmonicPrecompSize)
            {
                double n2 = n * n;
                double n4 = n2 * n2;
                return Constants.EulerGamma + Math.Log(n) + 0.5 / n - 1.0 / (12.0 * n2) + 1.0 / (120.0 * n4);
            }

            return HarmonicNumbers[n];
        }

        private const int HarmonicPrecompSize = 32;
        public static double[] HarmonicNumbers = new double[] {
            0.0,1.0,1.5,1.833333333333333333333333,2.083333333333333333333333,
            2.283333333333333333333333,2.45,2.592857142857142857142857,
            2.717857142857142857142857,2.828968253968253968253968,
            2.928968253968253968253968,3.019877344877344877344877,
            3.103210678210678210678211,3.180133755133755133755134,
            3.251562326562326562326562,3.318228993228993228993229,
            3.380728993228993228993229,3.439552522640757934875582,
            3.495108078196313490431137,3.547739657143681911483769,
            3.597739657143681911483769,3.645358704762729530531388,
            3.690813250217274985076843,3.734291511086840202468147,
            3.775958177753506869134814,3.815958177753506869134814,
            3.854419716215045330673275,3.891456753252082367710312,
            3.927171038966368081996027,3.961653797587057737168440,
            3.994987130920391070501774,4.027245195436520102759838
        };
        #endregion
        #endregion

        #region Comparisons
        /// <summary>
        /// Returns true iff both arguments are NaN or neither is NaN and they are equal
        /// </summary>
        /// <param name="x">first value</param>
        /// <param name="y">second value</param>
        /// <returns>true if the values are equal or both are NaN</returns>
        public static bool Equals(double x, double y)
        {
            return (double.IsNaN(x) && double.IsNaN(y)) || x == y;
        }

        /// <summary>
        /// Returns true iff both arguments are equal or within the range of allowed error (inclusive).  <p>Two NaNs are considered equals, as are two infinities with same sign.</p>
        /// </summary>
        /// <param name="x">first value</param>
        /// <param name="y">second value</param>
        /// <param name="eps">eps the amount of absolute error to allow</param>
        /// <returns>true if the values are equal or within range of each other</returns>
        public static bool Equals(double x, double y, double eps)
        {
            return ((double.IsNaN(x) && double.IsNaN(y)) || x == y) || (Math.Abs(y - x) <= eps);
        }

        /// <summary>
        /// Returns true iff both arguments are null or have same dimensions and all their elements are {@link #equals(double,double) equals}
        /// </summary>
        /// <param name="x">first array</param>
        /// <param name="y">second array</param>
        /// <returns>true if the values are both null or have same dimension and equal elements</returns>
        public static bool Equals(double[] x, double[] y)
        {
            if (x == null || y == null)
                return x == null && y == null;
            if (x.Length != y.Length)
                return false;
            for (int i = 0; i < x.Length; ++i)
            {
                if (!Equals(x[i], y[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares two numbers given some amount of allowed error.
        /// </summary>
        /// <param name="x">first number</param>
        /// <param name="y">second number</param>
        /// <param name="eps">eps the amount of error to allow when checking for equality</param>
        /// <returns></returns>
        public static int CompareTo(double x, double y, double eps)
        {
            if (Equals(x, y, eps))
                return 0;
            else if (x < y)
                return -1;
            return 1;
        }

        /// <summary>
        /// Evaluates the minimum distance to the next distinguishable number near the argument value.
        /// </summary>
        /// <returns>Relative Epsilon (positive double or NaN).</returns>
        /// <remarks>Evaluates the <b>negative</b> epsilon. The more common positive epsilon is equal to two times this negative epsilon.</remarks>
        public static double EpsilonOf(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
                return double.NaN;

            long signed64 = BitConverter.DoubleToInt64Bits(value);

            if (signed64 == 0)
            {
                signed64++;
                return BitConverter.Int64BitsToDouble(signed64) - value;
            }

            signed64--;

            if (signed64 < 0)
                return BitConverter.Int64BitsToDouble(signed64) - value;
            return value - BitConverter.Int64BitsToDouble(signed64);
        }

        /// <summary>
        /// Evaluates the minimum distance to the next distinguishable number near the argument value.
        /// </summary>
        /// <returns>Relative Epsilon (positive double or NaN)</returns>
        /// <remarks>Evaluates the positive epsilon</remarks>
        public static double PositiveEpsilonOf(double value)
        {
            return 2 * EpsilonOf(value);
        }

        /// <summary>
        /// Evaluates the count of numbers between two double numbers
        /// </summary>
        public static ulong NumbersBetween(double a, double b)
        {
            if (double.IsNaN(a) || double.IsInfinity(a) || double.IsNaN(b) || double.IsInfinity(b))
            {
                throw new ArgumentException("Values may not be NaN or Infinity");
            }

            ulong ua = ToLexicographicalOrderedUInt64(a);
            ulong ub = ToLexicographicalOrderedUInt64(b);

            return (a >= b) ? ua - ub : ub - ua;
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <param name="maxNumbersBetween">The maximum count of numbers between the two numbers plus one ([a,a] -> 0, [a,a+e] -> 1, [a,a+2e] -> 2, ...).</param>
        public static bool AlmostEqual(double a, double b, int maxNumbersBetween)
        {
            return AlmostEqual(a, b, (ulong)maxNumbersBetween);
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <param name="maxNumbersBetween">The maximum count of numbers between the two numbers plus one ([a,a] -> 0, [a,a+e] -> 1, [a,a+2e] -> 2, ...).</param>
        public static bool AlmostEqual(double a, double b, ulong maxNumbersBetween)
        {
            if (maxNumbersBetween < 0)
                throw new ArgumentException("MaxNumbersBetween cannot be <0");

            if (double.IsNaN(a) || double.IsNaN(b))
                return false;
            if (a == b)
                return true;

            if (double.IsInfinity(a) || double.IsInfinity(b))
                return false;

            ulong between = NumbersBetween(a, b);
            return between <= maxNumbersBetween;
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <param name="diff">The difference of the two numbers according to the Norm</param>
        /// <param name="relativeAccuracy">The relative accuracy required for being almost equal.</param>
        public static bool AlmostEqualNorm(double a, double b, double diff, double relativeAccuracy)
        {
            if ((a == 0 && Math.Abs(b) < relativeAccuracy) || (b == 0 && Math.Abs(a) < relativeAccuracy))
                return true;

            return Math.Abs(diff) < relativeAccuracy * Math.Max(Math.Abs(a), Math.Abs(b));
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <param name="diff">The difference of the two numbers according to the Norm</param>
        public static bool AlmostEqualNorm(double a, double b, double diff)
        {
            return AlmostEqualNorm(a, b, diff, Constants.DefaultRelativeAccuracy);
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <param name="relativeAccuracy">The relative accuracy required for being almost equal.</param>
        public static bool AlmostEqual(double a, double b, double relativeAccuracy)
        {
            return AlmostEqualNorm(a, b, a - b, relativeAccuracy);
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        public static bool AlmostEqual(double a, double b)
        {
            return AlmostEqualNorm(a, b, a - b, Constants.DefaultRelativeAccuracy);
        }

        /// <summary>
        /// Checks whether two real arrays are almost equal.
        /// </summary>
        /// <param name="x">The first vector</param>
        /// <param name="y">The second vector</param>
        public static bool AlmostEqual(double[] x, double[] y)
        {
            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; i++)
            {
                if (!AlmostEqual(x[i], y[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// True if the given number is almost equal to zero, according to the specified absolute accuracy.
        /// </summary>
        public static bool AlmostZero(double a, double absoluteAccuracy)
        {
            return Math.Abs(a) < absoluteAccuracy;
        }

        /// <summary>
        /// True if the given number is almost equal to zero.
        /// </summary>
        public static bool AlmostZero(double a)
        {
            return Math.Abs(a) < Constants.DefaultRelativeAccuracy;
        }
        #endregion

        #region Conversions
        public static bool TryParse(string s, out double result)
        {
            bool tmp = double.TryParse(s, out result);
            if (!tmp)
                result = double.NaN;
            return tmp;
        }

        public static bool TryParse(string s, out float result)
        {
            bool tmp = float.TryParse(s, out result);
            if (!tmp)
                result = float.NaN;
            return tmp;
        }

        public static bool TryParse(string s, out double result, double failResult)
        {
            bool tmp = double.TryParse(s, out result);
            if (!tmp)
                result = failResult;
            return tmp;
        }

        public static bool TryParse(string s, out float result, float failResult)
        {
            bool tmp = float.TryParse(s, out result);
            if (!tmp)
                result = failResult;
            return tmp;
        }

        public static int SingleToInt32Bits(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        public static float Int32BitsToSingle(int value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// Maps a double to an unsigned long integer which provides lexicographical ordering.
        /// </summary>
        public static ulong ToLexicographicalOrderedUInt64(double value)
        {
            long signed64 = BitConverter.DoubleToInt64Bits(value);
            ulong unsigned64 = (ulong)signed64;

            return (signed64 >= 0) ? unsigned64 : 0x8000000000000000 - unsigned64;
        }

        /// <summary>
        /// Maps a double to an signed long integer which provides lexicographical ordering.
        /// </summary>
        public static long ToLexicographicalOrderedInt64(double value)
        {
            long signed64 = BitConverter.DoubleToInt64Bits(value);

            return (signed64 >= 0) ? signed64 : (long)(0x8000000000000000 - (ulong)signed64);
        }

        /// <summary>
        /// Maps a double to an unsigned long integer which provides lexicographical ordering.
        /// </summary>
        public static uint ToLexicographicalOrderedUInt32(float value)
        {
            int signed32 = SingleToInt32Bits(value);
            uint unsigned32 = (uint)signed32;

            return (signed32 >= 0) ? unsigned32 : 0x80000000 - unsigned32;
        }

        /// <summary>
        /// Maps a double to an signed long integer which provides lexicographical ordering.
        /// </summary>
        public static int ToLexicographicalOrderedInt32(float value)
        {
            int signed32 = SingleToInt32Bits(value);

            return (signed32 >= 0) ? signed32 : (int)(0x80000000 - (uint)signed32);
        }

        public static void SwitchEndian(byte[] b)
        {
            int i = b.Length - 1;
            int j = 0;
            byte t;
            while (j < i)
            {
                t = b[i];
                b[i] = b[j];
                b[j] = t;
                i++;
                j--;
            }
        }

        public static double SwitchEndian(double v)
        {
            byte[] b = System.BitConverter.GetBytes(v);
            SwitchEndian(b);
            return System.BitConverter.ToDouble(b, 0);
        }

        public static float SwitchEndian(float v)
        {
            byte[] b = System.BitConverter.GetBytes(v);
            SwitchEndian(b);
            return System.BitConverter.ToSingle(b, 0);
        }

        public static short SwitchEndian(short v)
        {
            return (short)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
        }

        public static ushort SwitchEndian(ushort v)
        {
            return (ushort)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
        }

        public static int SwitchEndian(int v)
        {
            return (((v & 0x000000ff) << 24) + ((v & 0x0000ff00) << 8) +
                    ((int)(v & 0x00ff0000) >> 8) + ((int)(v & 0xff000000) >> 24));
        }

        public static uint SwitchEndian(uint v)
        {
            return (((v & 0x000000ff) << 24) + ((v & 0x0000ff00) << 8) +
                    ((uint)(v & 0x00ff0000) >> 8) + ((uint)(v & 0xff000000) >> 24));
        }

        public static long SwitchEndian(long v)
        {
            return (long)(((SwitchEndian((int)v) & 0xffffffffL) << 0x20) | (SwitchEndian((int)(v >> 0x20)) & 0xffffffffL));
        }

        public static ulong SwitchEndian(ulong v)
        {
            return (ulong)(((SwitchEndian((uint)v) & 0xffffffffL) << 0x20) | (SwitchEndian((uint)(v >> 0x20)) & 0xffffffffL));
        }
        #endregion
    }
}
