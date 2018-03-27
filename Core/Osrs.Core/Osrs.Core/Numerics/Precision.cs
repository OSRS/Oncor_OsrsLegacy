﻿//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
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

namespace Osrs.Numerics
{
    /// <summary>
    /// Support Interface for Precision Operations (like AlmostEquals).
    /// </summary>
    /// <typeparam name="T">Type of the implementing class.</typeparam>
    public interface IPrecisionSupport<in T>
    {
        /// <summary>
        /// Returns a Norm of a value of this type, which is appropriate for measuring how
        /// close this value is to zero.
        /// </summary>
        /// <returns>A norm of this value.</returns>
        double Norm();

        /// <summary>
        /// Returns a Norm of the difference of two values of this type, which is
        /// appropriate for measuring how close together these two values are.
        /// </summary>
        /// <param name="otherValue">The value to compare with.</param>
        /// <returns>A norm of the difference between this and the other value.</returns>
        double NormOfDifference(T otherValue);
    }

    /// <summary>
    /// Utilities for working with floating point numbers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Useful links:
    /// <list type="bullet">
    /// <item>
    /// http://docs.sun.com/source/806-3568/ncg_goldberg.html#689 - What every computer scientist should know about floating-point arithmetic
    /// </item>
    /// <item>
    /// http://en.wikipedia.org/wiki/Machine_epsilon - Gives the definition of machine epsilon
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    public static class Precision
    {
        /// <summary>
        /// The number of binary digits used to represent the binary number for a double precision floating
        /// point value. i.e. there are this many digits used to represent the
        /// actual number, where in a number as: 0.134556 * 10^5 the digits are 0.134556 and the exponent is 5.
        /// </summary>
        const int DoubleWidth = 53;

        /// <summary>
        /// The number of binary digits used to represent the binary number for a single precision floating
        /// point value. i.e. there are this many digits used to represent the
        /// actual number, where in a number as: 0.134556 * 10^5 the digits are 0.134556 and the exponent is 5.
        /// </summary>
        const int SingleWidth = 24;

        /// <summary>
        /// Standard epsilon, the maximum relative precision of IEEE 754 double-precision floating numbers (64 bit).
        /// According to the definition of Prof. Demmel and used in LAPACK and Scilab.
        /// </summary>
        public static readonly double DoublePrecision = Math.Pow(2, -DoubleWidth);

        /// <summary>
        /// Standard epsilon, the maximum relative precision of IEEE 754 double-precision floating numbers (64 bit).
        /// According to the definition of Prof. Higham and used in the ISO C standard and MATLAB.
        /// </summary>
        public static readonly double PositiveDoublePrecision = 2 * DoublePrecision;

        /// <summary>
        /// Standard epsilon, the maximum relative precision of IEEE 754 single-precision floating numbers (32 bit).
        /// According to the definition of Prof. Demmel and used in LAPACK and Scilab.
        /// </summary>
        public static readonly double SinglePrecision = Math.Pow(2, -SingleWidth);

        /// <summary>
        /// Standard epsilon, the maximum relative precision of IEEE 754 single-precision floating numbers (32 bit).
        /// According to the definition of Prof. Higham and used in the ISO C standard and MATLAB.
        /// </summary>
        public static readonly double PositiveSinglePrecision = 2 * SinglePrecision;

        /// <summary>
        /// Actual machine epsilon, the smallest number that can be subtracted from 1, yielding a results different than 1.
        /// This is also known as unit roundoff error. According to the definition of Prof. Demmel.
        /// On a standard machine this is equivalent to `DoublePrecision`.
        /// </summary>
        public static readonly double MachineEpsilon = MeasureMachineEpsilon();

        /// <summary>
        /// Actual machine epsilon, the smallest number that can be added to 1, yielding a results different than 1.
        /// This is also known as unit roundoff error. According to the definition of Prof. Higham.
        /// On a standard machine this is equivalent to `PositiveDoublePrecision`.
        /// </summary>
        public static readonly double PositiveMachineEpsilon = MeasurePositiveMachineEpsilon();

        /// <summary>
        /// The number of significant decimal places of double-precision floating numbers (64 bit).
        /// </summary>
        public static readonly int DoubleDecimalPlaces = (int)Math.Floor(Math.Abs(Math.Log10(DoublePrecision)));

        /// <summary>
        /// The number of significant decimal places of single-precision floating numbers (32 bit).
        /// </summary>
        public static readonly int SingleDecimalPlaces = (int)Math.Floor(Math.Abs(Math.Log10(SinglePrecision)));

        /// <summary>
        /// Value representing 10 * 2^(-53) = 1.11022302462516E-15
        /// </summary>
        static readonly double DefaultDoubleAccuracy = DoublePrecision * 10;

        /// <summary>
        /// Value representing 10 * 2^(-24) = 5.96046447753906E-07
        /// </summary>
        static readonly float DefaultSingleAccuracy = (float)(SinglePrecision * 10);

        /// <summary>
        /// Compares two doubles and determines which double is bigger.
        /// a &lt; b -> -1; a ~= b (almost equal according to parameter) -> 0; a &gt; b -> +1.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumAbsoluteError">The absolute accuracy required for being almost equal.</param>
        public static int CompareTo(this double a, double b, double maximumAbsoluteError)
        {
            // NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return a.CompareTo(b);
            }

            // If A or B are infinity (positive or negative) then
            // only return true if first is smaller
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a.CompareTo(b);
            }

            // If the numbers are equal to within the number of decimal places
            // then there's technically no difference
            if (AlmostEqual(a, b, maximumAbsoluteError))
            {
                return 0;
            }

            // The numbers differ by more than the decimal places, so
            // we can check the normal way to see if the first is
            // larger than the second.
            return a.CompareTo(b);
        }

        /// <summary>
        /// Compares two doubles and determines which double is bigger.
        /// a &lt; b -> -1; a ~= b (almost equal according to parameter) -> 0; a &gt; b -> +1.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places on which the values must be compared. Must be 1 or larger.</param>
        public static int CompareTo(this double a, double b, int decimalPlaces)
        {
            // NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return a.CompareTo(b);
            }

            // If A or B are infinity (positive or negative) then
            // only return true if first is smaller
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a.CompareTo(b);
            }

            // If the numbers are equal to within the number of decimal places
            // then there's technically no difference
            if (AlmostEqual(a, b, decimalPlaces))
            {
                return 0;
            }

            // The numbers differ by more than the decimal places, so
            // we can check the normal way to see if the first is
            // larger than the second.
            return a.CompareTo(b);
        }

        /// <summary>
        /// Compares two doubles and determines which double is bigger.
        /// a &lt; b -> -1; a ~= b (almost equal according to parameter) -> 0; a &gt; b -> +1.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumError">The relative accuracy required for being almost equal.</param>
        public static int CompareToRelative(this double a, double b, double maximumError)
        {
            // NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return a.CompareTo(b);
            }

            // If A or B are infinity (positive or negative) then
            // only return true if first is smaller
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a.CompareTo(b);
            }

            // If the numbers are equal to within the number of decimal places
            // then there's technically no difference
            if (AlmostEqualRelative(a, b, maximumError))
            {
                return 0;
            }

            // The numbers differ by more than the decimal places, so
            // we can check the normal way to see if the first is
            // larger than the second.
            return a.CompareTo(b);
        }

        /// <summary>
        /// Compares two doubles and determines which double is bigger.
        /// a &lt; b -> -1; a ~= b (almost equal according to parameter) -> 0; a &gt; b -> +1.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places on which the values must be compared. Must be 1 or larger.</param>
        public static int CompareToRelative(this double a, double b, int decimalPlaces)
        {
            // NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return a.CompareTo(b);
            }

            // If A or B are infinity (positive or negative) then
            // only return true if first is smaller
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a.CompareTo(b);
            }

            // If the numbers are equal to within the number of decimal places
            // then there's technically no difference
            if (AlmostEqualRelative(a, b, decimalPlaces))
            {
                return 0;
            }

            // The numbers differ by more than the decimal places, so
            // we can check the normal way to see if the first is
            // larger than the second.
            return a.CompareTo(b);
        }

        /// <summary>
        /// Compares two doubles and determines which double is bigger.
        /// a &lt; b -> -1; a ~= b (almost equal according to parameter) -> 0; a &gt; b -> +1.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maxNumbersBetween">The maximum error in terms of Units in Last Place (<c>ulps</c>), i.e. the maximum number of decimals that may be different. Must be 1 or larger.</param>
        public static int CompareToNumbersBetween(this double a, double b, long maxNumbersBetween)
        {
            // NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return a.CompareTo(b);
            }

            // If A or B are infinity (positive or negative) then
            // only return true if first is smaller
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a.CompareTo(b);
            }

            // If the numbers are equal to within the tolerance then
            // there's technically no difference
            if (AlmostEqualNumbersBetween(a, b, maxNumbersBetween))
            {
                return 0;
            }

            return a.CompareTo(b);
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 10^(-numberOfDecimalPlaces). We divide by 
        /// two so that we have half the range on each side of the numbers, e.g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLarger(this double a, double b, int decimalPlaces)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareTo(a, b, decimalPlaces) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 10^(-numberOfDecimalPlaces). We divide by 
        /// two so that we have half the range on each side of the numbers, e.g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLarger(this float a, float b, int decimalPlaces)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareTo(a, b, decimalPlaces) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumAbsoluteError">The absolute accuracy required for being almost equal.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLarger(this double a, double b, double maximumAbsoluteError)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareTo(a, b, maximumAbsoluteError) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumAbsoluteError">The absolute accuracy required for being almost equal.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLarger(this float a, float b, double maximumAbsoluteError)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareTo(a, b, maximumAbsoluteError) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 10^(-numberOfDecimalPlaces). We divide by 
        /// two so that we have half the range on each side of the numbers, e.g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLargerRelative(this double a, double b, int decimalPlaces)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareToRelative(a, b, decimalPlaces) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 10^(-numberOfDecimalPlaces). We divide by 
        /// two so that we have half the range on each side of the numbers, e.g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLargerRelative(this float a, float b, int decimalPlaces)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareToRelative(a, b, decimalPlaces) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumError">The relative accuracy required for being almost equal.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLargerRelative(this double a, double b, double maximumError)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareToRelative(a, b, maximumError) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumError">The relative accuracy required for being almost equal.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLargerRelative(this float a, float b, double maximumError)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareToRelative(a, b, maximumError) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the tolerance or not. Equality comparison is based on the binary representation.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maxNumbersBetween">The maximum number of floating point values for which the two values are considered equal. Must be 1 or larger.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLargerNumbersBetween(this double a, double b, long maxNumbersBetween)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareToNumbersBetween(a, b, maxNumbersBetween) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is larger than the <c>second</c>
        /// value to within the tolerance or not. Equality comparison is based on the binary representation.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maxNumbersBetween">The maximum number of floating point values for which the two values are considered equal. Must be 1 or larger.</param>
        /// <returns><c>true</c> if the first value is larger than the second value; otherwise <c>false</c>.</returns>
        public static bool IsLargerNumbersBetween(this float a, float b, long maxNumbersBetween)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareToNumbersBetween(a, b, maxNumbersBetween) > 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 10^(-numberOfDecimalPlaces). We divide by 
        /// two so that we have half the range on each side of th<paramref name="decimalPlaces"/>g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmaller(this double a, double b, int decimalPlaces)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareTo(a, b, decimalPlaces) < 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 10^(-numberOfDecimalPlaces). We divide by 
        /// two so that we have half the range on each side of th<paramref name="decimalPlaces"/>g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmaller(this float a, float b, int decimalPlaces)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareTo(a, b, decimalPlaces) < 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumAbsoluteError">The absolute accuracy required for being almost equal.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmaller(this double a, double b, double maximumAbsoluteError)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareTo(a, b, maximumAbsoluteError) < 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumAbsoluteError">The absolute accuracy required for being almost equal.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmaller(this float a, float b, double maximumAbsoluteError)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareTo(a, b, maximumAbsoluteError) < 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmallerRelative(this double a, double b, int decimalPlaces)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareToRelative(a, b, decimalPlaces) < 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmallerRelative(this float a, float b, int decimalPlaces)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareToRelative(a, b, decimalPlaces) < 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumError">The relative accuracy required for being almost equal.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmallerRelative(this double a, double b, double maximumError)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareToRelative(a, b, maximumError) < 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the specified number of decimal places or not.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumError">The relative accuracy required for being almost equal.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmallerRelative(this float a, float b, double maximumError)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareToRelative(a, b, maximumError) < 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the tolerance or not. Equality comparison is based on the binary representation.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maxNumbersBetween">The maximum number of floating point values for which the two values are considered equal. Must be 1 or larger.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmallerNumbersBetween(this double a, double b, long maxNumbersBetween)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return CompareToNumbersBetween(a, b, maxNumbersBetween) < 0;
        }

        /// <summary>
        /// Compares two doubles and determines if the <c>first</c> value is smaller than the <c>second</c>
        /// value to within the tolerance or not. Equality comparison is based on the binary representation.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maxNumbersBetween">The maximum number of floating point values for which the two values are considered equal. Must be 1 or larger.</param>
        /// <returns><c>true</c> if the first value is smaller than the second value; otherwise <c>false</c>.</returns>
        public static bool IsSmallerNumbersBetween(this float a, float b, long maxNumbersBetween)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves, and thus they're not bigger or
            // smaller than anything either
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return CompareToNumbersBetween(a, b, maxNumbersBetween) < 0;
        }

        /// <summary>
        /// Returns the magnitude of the number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The magnitude of the number.</returns>
        public static int Magnitude(this double value)
        {
            // Can't do this with zero because the 10-log of zero doesn't exist.
            if (value.Equals(0.0))
            {
                return 0;
            }

            // Note that we need the absolute value of the input because Log10 doesn't
            // work for negative numbers (obviously).
            double magnitude = Math.Log10(Math.Abs(value));

            var truncated = (int)Math.Truncate(magnitude);

            // To get the right number we need to know if the value is negative or positive
            // truncating a positive number will always give use the correct magnitude
            // truncating a negative number will give us a magnitude that is off by 1 (unless integer)
            return magnitude < 0d && truncated != magnitude
                ? truncated - 1
                : truncated;
        }


        /// <summary>
        /// Returns the magnitude of the number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The magnitude of the number.</returns>
        public static int Magnitude(this float value)
        {
            // Can't do this with zero because the 10-log of zero doesn't exist.
            if (value.Equals(0.0f))
            {
                return 0;
            }

            // Note that we need the absolute value of the input because Log10 doesn't
            // work for negative numbers (obviously).
            var magnitude = Convert.ToSingle(Math.Log10(Math.Abs(value)));

            var truncated = (int)Math.Truncate(magnitude);

            // To get the right number we need to know if the value is negative or positive
            // truncating a positive number will always give use the correct magnitude
            // truncating a negative number will give us a magnitude that is off by 1 (unless integer)
            return magnitude < 0f && truncated != magnitude
                ? truncated - 1
                : truncated;
        }

        /// <summary>
        /// Returns the number divided by it's magnitude, effectively returning a number between -10 and 10.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value of the number.</returns>
        public static double ScaleUnitMagnitude(this double value)
        {
            if (value.Equals(0.0))
            {
                return value;
            }

            int magnitude = Magnitude(value);
            return value * Math.Pow(10, -magnitude);
        }

        /// <summary>
        /// Gets the equivalent <c>long</c> value for the given <c>double</c> value.
        /// </summary>
        /// <param name="value">The <c>double</c> value which should be turned into a <c>long</c> value.</param>
        /// <returns>
        /// The resulting <c>long</c> value.
        /// </returns>
        static long AsInt64(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        /// <summary>
        /// Returns a 'directional' long value. This is a long value which acts the same as a double,
        /// e.g. a negative double value will return a negative double value starting at 0 and going
        /// more negative as the double value gets more negative.
        /// </summary>
        /// <param name="value">The input double value.</param>
        /// <returns>A long value which is roughly the equivalent of the double value.</returns>
        static long AsDirectionalInt64(double value)
        {
            // Convert in the normal way.
            long result = AsInt64(value);

            // Now find out where we're at in the range
            // If the value is larger/equal to zero then we can just return the value
            // if the value is negative we subtract long.MinValue from it.
            return (result >= 0) ? result : (long.MinValue - result);
        }

        /// <summary>
        /// Returns a 'directional' int value. This is a int value which acts the same as a float,
        /// e.g. a negative float value will return a negative int value starting at 0 and going
        /// more negative as the float value gets more negative.
        /// </summary>
        /// <param name="value">The input float value.</param>
        /// <returns>An int value which is roughly the equivalent of the double value.</returns>
        static int AsDirectionalInt32(float value)
        {
            // Convert in the normal way.
            int result = FloatToInt32Bits(value);

            // Now find out where we're at in the range
            // If the value is larger/equal to zero then we can just return the value
            // if the value is negative we subtract int.MinValue from it.
            return (result >= 0) ? result : (int.MinValue - result);
        }

        /// <summary>
        /// Increments a floating point number to the next bigger number representable by the data type.
        /// </summary>
        /// <param name="value">The value which needs to be incremented.</param>
        /// <param name="count">How many times the number should be incremented.</param>
        /// <remarks>
        /// The incrementation step length depends on the provided value.
        /// Increment(double.MaxValue) will return positive infinity.
        /// </remarks>
        /// <returns>The next larger floating point value.</returns>
        public static double Increment(this double value, int count = 1)
        {
            if (double.IsInfinity(value) || double.IsNaN(value) || count == 0)
            {
                return value;
            }

            if (count < 0)
            {
                return Decrement(value, -count);
            }

            // Translate the bit pattern of the double to an integer.
            // Note that this leads to:
            // double > 0 --> long > 0, growing as the double value grows
            // double < 0 --> long < 0, increasing in absolute magnitude as the double 
            //                          gets closer to zero!
            //                          i.e. 0 - double.epsilon will give the largest long value!
            long intValue = AsInt64(value);
            if (intValue < 0)
            {
                intValue -= count;
            }
            else
            {
                intValue += count;
            }

            // Note that long.MinValue has the same bit pattern as -0.0.
            if (intValue == long.MinValue)
            {
                return 0;
            }

            // Note that not all long values can be translated into double values. There's a whole bunch of them 
            // which return weird values like infinity and NaN
            return BitConverter.Int64BitsToDouble(intValue);
        }

        /// <summary>
        /// Decrements a floating point number to the next smaller number representable by the data type.
        /// </summary>
        /// <param name="value">The value which should be decremented.</param>
        /// <param name="count">How many times the number should be decremented.</param>
        /// <remarks>
        /// The decrementation step length depends on the provided value.
        /// Decrement(double.MinValue) will return negative infinity.
        /// </remarks>
        /// <returns>The next smaller floating point value.</returns>
        public static double Decrement(this double value, int count = 1)
        {
            if (double.IsInfinity(value) || double.IsNaN(value) || count == 0)
            {
                return value;
            }

            if (count < 0)
            {
                return Decrement(value, -count);
            }

            // Translate the bit pattern of the double to an integer.
            // Note that this leads to:
            // double > 0 --> long > 0, growing as the double value grows
            // double < 0 --> long < 0, increasing in absolute magnitude as the double 
            //                          gets closer to zero!
            //                          i.e. 0 - double.epsilon will give the largest long value!
            long intValue = AsInt64(value);

            // If the value is zero then we'd really like the value to be -0. So we'll make it -0 
            // and then everything else should work out.
            if (intValue == 0)
            {
                // Note that long.MinValue has the same bit pattern as -0.0.
                intValue = long.MinValue;
            }

            if (intValue < 0)
            {
                intValue += count;
            }
            else
            {
                intValue -= count;
            }

            // Note that not all long values can be translated into double values. There's a whole bunch of them 
            // which return weird values like infinity and NaN
            return BitConverter.Int64BitsToDouble(intValue);
        }

        /// <summary>
        /// Forces small numbers near zero to zero, according to the specified absolute accuracy.
        /// </summary>
        /// <param name="a">The real number to coerce to zero, if it is almost zero.</param>
        /// <param name="maxNumbersBetween">The maximum count of numbers between the zero and the number <paramref name="a"/>.</param>
        /// <returns>
        ///     Zero if |<paramref name="a"/>| is fewer than <paramref name="maxNumbersBetween"/> numbers from zero, <paramref name="a"/> otherwise.
        /// </returns>
        public static double CoerceZero(this double a, int maxNumbersBetween)
        {
            return CoerceZero(a, (long)maxNumbersBetween);
        }

        /// <summary>
        /// Forces small numbers near zero to zero, according to the specified absolute accuracy.
        /// </summary>
        /// <param name="a">The real number to coerce to zero, if it is almost zero.</param>
        /// <param name="maxNumbersBetween">The maximum count of numbers between the zero and the number <paramref name="a"/>.</param>
        /// <returns>
        ///     Zero if |<paramref name="a"/>| is fewer than <paramref name="maxNumbersBetween"/> numbers from zero, <paramref name="a"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="maxNumbersBetween"/> is smaller than zero.
        /// </exception>
        public static double CoerceZero(this double a, long maxNumbersBetween)
        {
            if (maxNumbersBetween < 0)
            {
                throw new ArgumentOutOfRangeException("maxNumbersBetween");
            }

            if (double.IsInfinity(a) || double.IsNaN(a))
            {
                return a;
            }

            // We allow maxNumbersBetween between 0 and the number so
            // we need to check if there a
            if (NumbersBetween(0.0, a) <= (ulong)maxNumbersBetween)
            {
                return 0.0;
            }

            return a;
        }

        /// <summary>
        /// Forces small numbers near zero to zero, according to the specified absolute accuracy.
        /// </summary>
        /// <param name="a">The real number to coerce to zero, if it is almost zero.</param>
        /// <param name="maximumAbsoluteError">The absolute threshold for <paramref name="a"/> to consider it as zero.</param>
        /// <returns>Zero if |<paramref name="a"/>| is smaller than <paramref name="maximumAbsoluteError"/>, <paramref name="a"/> otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="maximumAbsoluteError"/> is smaller than zero.
        /// </exception>
        public static double CoerceZero(this double a, double maximumAbsoluteError)
        {
            if (maximumAbsoluteError < 0)
            {
                throw new ArgumentOutOfRangeException("maximumAbsoluteError");
            }

            if (double.IsInfinity(a) || double.IsNaN(a))
            {
                return a;
            }

            if (Math.Abs(a) < maximumAbsoluteError)
            {
                return 0.0;
            }

            return a;
        }

        /// <summary>
        /// Forces small numbers near zero to zero.
        /// </summary>
        /// <param name="a">The real number to coerce to zero, if it is almost zero.</param>
        /// <returns>Zero if |<paramref name="a"/>| is smaller than 2^(-53) = 1.11e-16, <paramref name="a"/> otherwise.</returns>
        public static double CoerceZero(this double a)
        {
            return CoerceZero(a, DoublePrecision);
        }

        /// <summary>
        /// Determines the range of floating point numbers that will match the specified value with the given tolerance.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maxNumbersBetween">The <c>ulps</c> difference.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="maxNumbersBetween"/> is smaller than zero.
        /// </exception>
        /// <returns>Tuple of the bottom and top range ends.</returns>
        public static Tuple<double, double> RangeOfMatchingFloatingPointNumbers(this double value, long maxNumbersBetween)
        {
            // Make sure ulpDifference is non-negative
            if (maxNumbersBetween < 1)
            {
                throw new ArgumentOutOfRangeException("maxNumbersBetween");
            }

            // If the value is infinity (positive or negative) just
            // return the same infinity for the range.
            if (double.IsInfinity(value))
            {
                return new Tuple<double, double>(value, value);
            }

            // If the value is a NaN then the range is a NaN too.
            if (double.IsNaN(value))
            {
                return new Tuple<double, double>(double.NaN, double.NaN);
            }

            // Translate the bit pattern of the double to an integer.
            // Note that this leads to:
            // double > 0 --> long > 0, growing as the double value grows
            // double < 0 --> long < 0, increasing in absolute magnitude as the double 
            //                          gets closer to zero!
            //                          i.e. 0 - double.epsilon will give the largest long value!
            long intValue = AsInt64(value);

            // We need to protect against over- and under-flow of the intValue when
            // we start to add the ulpsDifference.
            if (intValue < 0)
            {
                // Note that long.MinValue has the same bit pattern as
                // -0.0. Therefore we're working in opposite direction (i.e. add if we want to
                // go more negative and subtract if we want to go less negative)
                var topRangeEnd = Math.Abs(long.MinValue - intValue) < maxNumbersBetween
                    // Got underflow, which can be fixed by splitting the calculation into two bits
                    // first get the remainder of the intValue after subtracting it from the long.MinValue
                    // and add that to the ulpsDifference. That way we'll turn positive without underflow
                    ? BitConverter.Int64BitsToDouble(maxNumbersBetween + (long.MinValue - intValue))
                    // No problems here, move along.
                    : BitConverter.Int64BitsToDouble(intValue - maxNumbersBetween);

                var bottomRangeEnd = Math.Abs(intValue) < maxNumbersBetween
                    // Underflow, which means we'd have to go further than a long would allow us.
                    // Also we couldn't translate it back to a double, so we'll return -Double.MaxValue
                    ? -double.MaxValue
                    // intValue is negative. Adding the positive ulpsDifference means that it gets less negative.
                    // However due to the conversion way this means that the actual double value gets more negative :-S
                    : BitConverter.Int64BitsToDouble(intValue + maxNumbersBetween);

                return new Tuple<double, double>(bottomRangeEnd, topRangeEnd);
            }
            else
            {
                // IntValue is positive
                var topRangeEnd = long.MaxValue - intValue < maxNumbersBetween
                    // Overflow, which means we'd have to go further than a long would allow us.
                    // Also we couldn't translate it back to a double, so we'll return Double.MaxValue
                    ? double.MaxValue
                    // No troubles here
                    : BitConverter.Int64BitsToDouble(intValue + maxNumbersBetween);

                // Check the bottom range end for underflows
                var bottomRangeEnd = intValue > maxNumbersBetween
                    // No problems here. IntValue is larger than ulpsDifference so we'll end up with a
                    // positive number.
                    ? BitConverter.Int64BitsToDouble(intValue - maxNumbersBetween)
                    // Int value is bigger than zero but smaller than the ulpsDifference. So we'll need to deal with
                    // the reversal at the negative end
                    : BitConverter.Int64BitsToDouble(long.MinValue + (maxNumbersBetween - intValue));

                return new Tuple<double, double>(bottomRangeEnd, topRangeEnd);
            }
        }

        /// <summary>
        /// Returns the floating point number that will match the value with the tolerance on the maximum size (i.e. the result is
        /// always bigger than the value)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maxNumbersBetween">The <c>ulps</c> difference.</param>
        /// <returns>The maximum floating point number which is <paramref name="maxNumbersBetween"/> larger than the given <paramref name="value"/>.</returns>
        public static double MaximumMatchingFloatingPointNumber(this double value, long maxNumbersBetween)
        {
            return RangeOfMatchingFloatingPointNumbers(value, maxNumbersBetween).Item2;
        }

        /// <summary>
        /// Returns the floating point number that will match the value with the tolerance on the minimum size (i.e. the result is
        /// always smaller than the value)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maxNumbersBetween">The <c>ulps</c> difference.</param>
        /// <returns>The minimum floating point number which is <paramref name="maxNumbersBetween"/> smaller than the given <paramref name="value"/>.</returns>
        public static double MinimumMatchingFloatingPointNumber(this double value, long maxNumbersBetween)
        {
            return RangeOfMatchingFloatingPointNumbers(value, maxNumbersBetween).Item1;
        }

        /// <summary>
        /// Determines the range of <c>ulps</c> that will match the specified value with the given tolerance.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="relativeDifference">The relative difference.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="relativeDifference"/> is smaller than zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="value"/> is <c>double.PositiveInfinity</c> or <c>double.NegativeInfinity</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="value"/> is <c>double.NaN</c>.
        /// </exception>
        /// <returns>
        /// Tuple with the number of ULPS between the <c>value</c> and the <c>value - relativeDifference</c> as first,
        /// and the number of ULPS between the <c>value</c> and the <c>value + relativeDifference</c> as second value.
        /// </returns>
        public static Tuple<long, long> RangeOfMatchingNumbers(this double value, double relativeDifference)
        {
            // Make sure the relative is non-negative 
            if (relativeDifference < 0)
            {
                throw new ArgumentOutOfRangeException("relativeDifference");
            }

            // If the value is infinity (positive or negative) then
            // we can't determine the range.
            if (double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException("value");
            }

            // If the value is a NaN then we can't determine the range.
            if (double.IsNaN(value))
            {
                throw new ArgumentOutOfRangeException("value");
            }

            // If the value is zero (0.0) then we can't calculate the relative difference
            // so return the ulps counts for the difference.
            if (value.Equals(0))
            {
                var v = AsInt64(relativeDifference);
                return new Tuple<long, long>(v, v);
            }

            // Calculate the ulps for the maximum and minimum values
            // Note that these can overflow
            long max = AsDirectionalInt64(value + (relativeDifference * Math.Abs(value)));
            long min = AsDirectionalInt64(value - (relativeDifference * Math.Abs(value)));

            // Calculate the ulps from the value
            long intValue = AsDirectionalInt64(value);

            // Determine the ranges
            return new Tuple<long, long>(Math.Abs(intValue - min), Math.Abs(max - intValue));
        }

        /// <summary>
        /// Evaluates the count of numbers between two double numbers
        /// </summary>
        /// <param name="a">The first parameter.</param>
        /// <param name="b">The second parameter.</param>
        /// <remarks>The second number is included in the number, thus two equal numbers evaluate to zero and two neighbor numbers evaluate to one. Therefore, what is returned is actually the count of numbers between plus 1.</remarks>
        /// <returns>The number of floating point values between <paramref name="a"/> and <paramref name="b"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="a"/> is <c>double.PositiveInfinity</c> or <c>double.NegativeInfinity</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="a"/> is <c>double.NaN</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="b"/> is <c>double.PositiveInfinity</c> or <c>double.NegativeInfinity</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="b"/> is <c>double.NaN</c>.
        /// </exception>
        public static ulong NumbersBetween(this double a, double b)
        {
            if (double.IsNaN(a) || double.IsInfinity(a))
            {
                throw new ArgumentOutOfRangeException("a");
            }

            if (double.IsNaN(b) || double.IsInfinity(b))
            {
                throw new ArgumentOutOfRangeException("b");
            }

            // Calculate the ulps for the maximum and minimum values
            // Note that these can overflow
            long intA = AsDirectionalInt64(a);
            long intB = AsDirectionalInt64(b);

            // Now find the number of values between the two doubles. This should not overflow
            // given that there are more long values than there are double values
            return (a >= b) ? (ulong)(intA - intB) : (ulong)(intB - intA);
        }

        /// <summary>
        /// Evaluates the minimum distance to the next distinguishable number near the argument value.
        /// </summary>
        /// <param name="value">The value used to determine the minimum distance.</param>
        /// <returns>
        /// Relative Epsilon (positive double or NaN).
        /// </returns>
        /// <remarks>Evaluates the <b>negative</b> epsilon. The more common positive epsilon is equal to two times this negative epsilon.</remarks>
        /// <seealso cref="PositiveEpsilonOf(double)"/>
        public static double EpsilonOf(this double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                return double.NaN;
            }

            long signed64 = BitConverter.DoubleToInt64Bits(value);
            if (signed64 == 0)
            {
                signed64++;
                return BitConverter.Int64BitsToDouble(signed64) - value;
            }
            if (signed64-- < 0)
            {
                return BitConverter.Int64BitsToDouble(signed64) - value;
            }
            return value - BitConverter.Int64BitsToDouble(signed64);
        }

        /// <summary>
        /// Evaluates the minimum distance to the next distinguishable number near the argument value.
        /// </summary>
        /// <param name="value">The value used to determine the minimum distance.</param>
        /// <returns>Relative Epsilon (positive double or NaN)</returns>
        /// <remarks>Evaluates the <b>positive</b> epsilon. See also <see cref="EpsilonOf"/></remarks>
        /// <seealso cref="EpsilonOf(double)"/>
        public static double PositiveEpsilonOf(this double value)
        {
            return 2 * EpsilonOf(value);
        }

        /// <summary>
        /// Converts a float value to a bit array stored in an int.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The bit array.</returns>
        static int FloatToInt32Bits(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// Calculates the actual positive double precision machine epsilon - the smallest number that can be added to 1, yielding a results different than 1.
        /// This is also known as unit roundoff error. According to the definition of Prof. Demmel.
        /// </summary>
        /// <returns>Positive Machine epsilon</returns>
        static double MeasureMachineEpsilon()
        {
            double eps = 1.0d;

            while ((1.0d - (eps / 2.0d)) < 1.0d)
                eps /= 2.0d;

            return eps;
        }

        /// <summary>
        /// Calculates the actual positive double precision machine epsilon - the smallest number that can be added to 1, yielding a results different than 1.
        /// This is also known as unit roundoff error. According to the definition of Prof. Higham.
        /// </summary>
        /// <returns>Machine epsilon</returns>
        static double MeasurePositiveMachineEpsilon()
        {
            double eps = 1.0d;

            while ((1.0d + (eps / 2.0d)) > 1.0d)
                eps /= 2.0d;

            return eps;
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal
        /// within the specified maximum absolute error.
        /// </summary>
        /// <param name="a">The norm of the first value (can be negative).</param>
        /// <param name="b">The norm of the second value (can be negative).</param>
        /// <param name="diff">The norm of the difference of the two values (can be negative).</param>
        /// <param name="maximumAbsoluteError">The absolute accuracy required for being almost equal.</param>
        /// <returns>True if both doubles are almost equal up to the specified maximum absolute error, false otherwise.</returns>
        public static bool AlmostEqualNorm(this double a, double b, double diff, double maximumAbsoluteError)
        {
            // If A or B are infinity (positive or negative) then
            // only return true if they are exactly equal to each other -
            // that is, if they are both infinities of the same sign.
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a == b;
            }

            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves.
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return Math.Abs(diff) < maximumAbsoluteError;
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal
        /// within the specified maximum absolute error.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumAbsoluteError">The absolute accuracy required for being almost equal.</param>
        /// <returns>True if both doubles are almost equal up to the specified maximum absolute error, false otherwise.</returns>
        public static bool AlmostEqualNorm<T>(this T a, T b, double maximumAbsoluteError)
            where T : IPrecisionSupport<T>
        {
            return AlmostEqualNorm(a.Norm(), b.Norm(), a.NormOfDifference(b), maximumAbsoluteError);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal
        /// within the specified maximum error.
        /// </summary>
        /// <param name="a">The norm of the first value (can be negative).</param>
        /// <param name="b">The norm of the second value (can be negative).</param>
        /// <param name="diff">The norm of the difference of the two values (can be negative).</param>
        /// <param name="maximumError">The accuracy required for being almost equal.</param>
        /// <returns>True if both doubles are almost equal up to the specified maximum error, false otherwise.</returns>
        public static bool AlmostEqualNormRelative(this double a, double b, double diff, double maximumError)
        {
            // If A or B are infinity (positive or negative) then
            // only return true if they are exactly equal to each other -
            // that is, if they are both infinities of the same sign.
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a == b;
            }

            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves.
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            // If one is almost zero, fall back to absolute equality
            if (Math.Abs(a) < DoublePrecision || Math.Abs(b) < DoublePrecision)
            {
                return Math.Abs(diff) < maximumError;
            }

            if ((a == 0 && Math.Abs(b) < maximumError) || (b == 0 && Math.Abs(a) < maximumError))
            {
                return true;
            }

            return Math.Abs(diff) < maximumError * Math.Max(Math.Abs(a), Math.Abs(b));
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal
        /// within the specified maximum error.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumError">The accuracy required for being almost equal.</param>
        /// <returns>True if both doubles are almost equal up to the specified maximum error, false otherwise.</returns>
        public static bool AlmostEqualNormRelative<T>(this T a, T b, double maximumError)
            where T : IPrecisionSupport<T>
        {
            return AlmostEqualNormRelative(a.Norm(), b.Norm(), a.NormOfDifference(b), maximumError);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal within
        /// the specified maximum error.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumAbsoluteError">The accuracy required for being almost equal.</param>
        public static bool AlmostEqual(this double a, double b, double maximumAbsoluteError)
        {
            return AlmostEqualNorm(a, b, a - b, maximumAbsoluteError);
        }

        /// <summary>
        /// Compares two complex and determines if they are equal within
        /// the specified maximum error.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumAbsoluteError">The accuracy required for being almost equal.</param>
        public static bool AlmostEqual(this float a, float b, double maximumAbsoluteError)
        {
            return AlmostEqualNorm(a, b, a - b, maximumAbsoluteError);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal within
        /// the specified maximum error.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumError">The accuracy required for being almost equal.</param>
        public static bool AlmostEqualRelative(this double a, double b, double maximumError)
        {
            return AlmostEqualNormRelative(a, b, a - b, maximumError);
        }

        /// <summary>
        /// Compares two complex and determines if they are equal within
        /// the specified maximum error.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maximumError">The accuracy required for being almost equal.</param>
        public static bool AlmostEqualRelative(this float a, float b, double maximumError)
        {
            return AlmostEqualNormRelative(a, b, a - b, maximumError);
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true if the two values differ by no more than 10 * 2^(-52); false otherwise.</returns>
        public static bool AlmostEqual(this double a, double b)
        {
            return AlmostEqualNorm(a, b, a - b, DefaultDoubleAccuracy);
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true if the two values differ by no more than 10 * 2^(-52); false otherwise.</returns>
        public static bool AlmostEqual(this float a, float b)
        {
            return AlmostEqualNorm(a, b, a - b, DefaultSingleAccuracy);
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true if the two values differ by no more than 10 * 2^(-52); false otherwise.</returns>
        public static bool AlmostEqualRelative(this double a, double b)
        {
            return AlmostEqualNormRelative(a, b, a - b, DefaultDoubleAccuracy);
        }

        /// <summary>
        /// Checks whether two real numbers are almost equal.
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>true if the two values differ by no more than 10 * 2^(-52); false otherwise.</returns>
        public static bool AlmostEqualRelative(this float a, float b)
        {
            return AlmostEqualNormRelative(a, b, a - b, DefaultSingleAccuracy);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal to within the specified number of decimal places or not, using the 
        /// number of decimal places as an absolute measure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 0.5e-decimalPlaces. We divide by 
        /// two so that we have half the range on each side of the numbers, e.g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The norm of the first value (can be negative).</param>
        /// <param name="b">The norm of the second value (can be negative).</param>
        /// <param name="diff">The norm of the difference of the two values (can be negative).</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool AlmostEqualNorm(this double a, double b, double diff, int decimalPlaces)
        {
            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves.
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            // If A or B are infinity (positive or negative) then
            // only return true if they are exactly equal to each other -
            // that is, if they are both infinities of the same sign.
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a == b;
            }

            // The values are equal if the difference between the two numbers is smaller than
            // 10^(-numberOfDecimalPlaces). We divide by two so that we have half the range
            // on each side of the numbers, e.g. if decimalPlaces == 2, 
            // then 0.01 will equal between 0.005 and 0.015, but not 0.02 and not 0.00
            return Math.Abs(diff) < Math.Pow(10, -decimalPlaces) / 2d;
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal to within the specified number of decimal places or not, using the 
        /// number of decimal places as an absolute measure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 0.5e-decimalPlaces. We divide by 
        /// two so that we have half the range on each side of the numbers, e.g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool AlmostEqualNorm<T>(this T a, T b, int decimalPlaces)
            where T : IPrecisionSupport<T>
        {
            return AlmostEqualNorm(a.Norm(), b.Norm(), a.NormOfDifference(b), decimalPlaces);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal to within the specified number of decimal places or not. If the numbers
        /// are very close to zero an absolute difference is compared, otherwise the relative difference is compared.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 10^(-numberOfDecimalPlaces). We divide by 
        /// two so that we have half the range on each side of the numbers, e.g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The norm of the first value (can be negative).</param>
        /// <param name="b">The norm of the second value (can be negative).</param>
        /// <param name="diff">The norm of the difference of the two values (can be negative).</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="decimalPlaces"/> is smaller than zero.</exception>
        public static bool AlmostEqualNormRelative(this double a, double b, double diff, int decimalPlaces)
        {
            if (decimalPlaces < 0)
            {
                // Can't have a negative number of decimal places
                throw new ArgumentOutOfRangeException("decimalPlaces");
            }

            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves.
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            // If A or B are infinity (positive or negative) then
            // only return true if they are exactly equal to each other -
            // that is, if they are both infinities of the same sign.
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a == b;
            }

            // If both numbers are equal, get out now. This should remove the possibility of both numbers being zero
            // and any problems associated with that.
            if (a.Equals(b))
            {
                return true;
            }

            // If one is almost zero, fall back to absolute equality
            if (Math.Abs(a) < DoublePrecision || Math.Abs(b) < DoublePrecision)
            {
                // The values are equal if the difference between the two numbers is smaller than
                // 10^(-numberOfDecimalPlaces). We divide by two so that we have half the range
                // on each side of the numbers, e.g. if decimalPlaces == 2, 
                // then 0.01 will equal between 0.005 and 0.015, but not 0.02 and not 0.00
                return Math.Abs(diff) < Math.Pow(10, -decimalPlaces) / 2d;
            }

            // If the magnitudes of the two numbers are equal to within one magnitude the numbers could potentially be equal
            int magnitudeOfFirst = Magnitude(a);
            int magnitudeOfSecond = Magnitude(b);
            int magnitudeOfMax = Math.Max(magnitudeOfFirst, magnitudeOfSecond);
            if (magnitudeOfMax > (Math.Min(magnitudeOfFirst, magnitudeOfSecond) + 1))
            {
                return false;
            }

            // The values are equal if the difference between the two numbers is smaller than
            // 10^(-numberOfDecimalPlaces). We divide by two so that we have half the range
            // on each side of the numbers, e.g. if decimalPlaces == 2, 
            // then 0.01 will equal between 0.00995 and 0.01005, but not 0.0015 and not 0.0095
            return Math.Abs(diff) < Math.Pow(10, magnitudeOfMax - decimalPlaces) / 2d;
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal to within the specified number of decimal places or not. If the numbers
        /// are very close to zero an absolute difference is compared, otherwise the relative difference is compared.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are equal if the difference between the two numbers is smaller than 10^(-numberOfDecimalPlaces). We divide by 
        /// two so that we have half the range on each side of the numbers, e.g. if <paramref name="decimalPlaces"/> == 2, then 0.01 will equal between 
        /// 0.005 and 0.015, but not 0.02 and not 0.00
        /// </para>
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool AlmostEqualNormRelative<T>(this T a, T b, int decimalPlaces)
            where T : IPrecisionSupport<T>
        {
            return AlmostEqualNormRelative(a.Norm(), b.Norm(), a.NormOfDifference(b), decimalPlaces);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal to within the specified number of decimal places or not, using the 
        /// number of decimal places as an absolute measure.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool AlmostEqual(this double a, double b, int decimalPlaces)
        {
            return AlmostEqualNorm(a, b, a - b, decimalPlaces);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal to within the specified number of decimal places or not, using the 
        /// number of decimal places as an absolute measure.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool AlmostEqual(this float a, float b, int decimalPlaces)
        {
            return AlmostEqualNorm(a, b, a - b, decimalPlaces);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal to within the specified number of decimal places or not. If the numbers
        /// are very close to zero an absolute difference is compared, otherwise the relative difference is compared.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool AlmostEqualRelative(this double a, double b, int decimalPlaces)
        {
            return AlmostEqualNormRelative(a, b, a - b, decimalPlaces);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal to within the specified number of decimal places or not. If the numbers
        /// are very close to zero an absolute difference is compared, otherwise the relative difference is compared.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool AlmostEqualRelative(this float a, float b, int decimalPlaces)
        {
            return AlmostEqualNormRelative(a, b, a - b, decimalPlaces);
        }

        /// <summary>
        /// Compares two doubles and determines if they are equal to within the tolerance or not. Equality comparison is based on the binary representation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Determines the 'number' of floating point numbers between two values (i.e. the number of discrete steps 
        /// between the two numbers) and then checks if that is within the specified tolerance. So if a tolerance 
        /// of 1 is passed then the result will be true only if the two numbers have the same binary representation 
        /// OR if they are two adjacent numbers that only differ by one step.
        /// </para>
        /// <para>
        /// The comparison method used is explained in http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm . The article
        /// at http://www.extremeoptimization.com/resources/Articles/FPDotNetConceptsAndFormats.aspx explains how to transform the C code to 
        /// .NET enabled code without using pointers and unsafe code.
        /// </para>
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maxNumbersBetween">The maximum number of floating point values between the two values. Must be 1 or larger.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxNumbersBetween"/> is smaller than one.</exception>
        public static bool AlmostEqualNumbersBetween(this double a, double b, long maxNumbersBetween)
        {
            // Make sure maxNumbersBetween is non-negative and small enough that the
            // default NAN won't compare as equal to anything.
            if (maxNumbersBetween < 1)
            {
                throw new ArgumentOutOfRangeException("maxNumbersBetween");
            }

            // If A or B are infinity (positive or negative) then
            // only return true if they are exactly equal to each other -
            // that is, if they are both infinities of the same sign.
            if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                return a == b;
            }

            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves.
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            // Get the first double and convert it to an integer value (by using the binary representation)
            long firstUlong = AsDirectionalInt64(a);

            // Get the second double and convert it to an integer value (by using the binary representation)
            long secondUlong = AsDirectionalInt64(b);

            // Now compare the values. 
            // Note that this comparison can overflow so we'll approach this differently
            // Do note that we could overflow this way too. We should probably check that we don't.
            return (a > b) ? (secondUlong + maxNumbersBetween >= firstUlong) : (firstUlong + maxNumbersBetween >= secondUlong);
        }

        /// <summary>
        /// Compares two floats and determines if they are equal to within the tolerance or not. Equality comparison is based on the binary representation.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="maxNumbersBetween">The maximum number of floating point values between the two values. Must be 1 or larger.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxNumbersBetween"/> is smaller than one.</exception>
        public static bool AlmostEqualNumbersBetween(this float a, float b, int maxNumbersBetween)
        {
            // Make sure maxNumbersBetween is non-negative and small enough that the
            // default NAN won't compare as equal to anything.
            if (maxNumbersBetween < 1)
            {
                throw new ArgumentOutOfRangeException("maxNumbersBetween");
            }

            // If A or B are infinity (positive or negative) then
            // only return true if they are exactly equal to each other -
            // that is, if they are both infinities of the same sign.
            if (float.IsInfinity(a) || float.IsInfinity(b))
            {
                return a == b;
            }

            // If A or B are a NAN, return false. NANs are equal to nothing,
            // not even themselves.
            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            // Get the first float and convert it to an integer value (by using the binary representation)
            int firstUlong = AsDirectionalInt32(a);

            // Get the second float and convert it to an integer value (by using the binary representation)
            int secondUlong = AsDirectionalInt32(b);

            // Now compare the values. 
            // Note that this comparison can overflow so we'll approach this differently
            // Do note that we could overflow this way too. We should probably check that we don't.
            return (a > b) ? (secondUlong + maxNumbersBetween >= firstUlong) : (firstUlong + maxNumbersBetween >= secondUlong);
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="maximumAbsoluteError">The accuracy required for being almost equal.</param>
        public static bool ListAlmostEqual(this IList<double> a, IList<double> b, double maximumAbsoluteError)
        {
            return ListForAll(a, b, AlmostEqual, maximumAbsoluteError);
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="maximumAbsoluteError">The accuracy required for being almost equal.</param>
        public static bool ListAlmostEqual(this IList<float> a, IList<float> b, double maximumAbsoluteError)
        {
            return ListForAll(a, b, AlmostEqual, maximumAbsoluteError);
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="maximumError">The accuracy required for being almost equal.</param>
        public static bool ListAlmostEqualRelative(this IList<double> a, IList<double> b, double maximumError)
        {
            return ListForAll(a, b, AlmostEqualRelative, maximumError);
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="maximumError">The accuracy required for being almost equal.</param>
        public static bool ListAlmostEqualRelative(this IList<float> a, IList<float> b, double maximumError)
        {
            return ListForAll(a, b, AlmostEqualRelative, maximumError);
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool ListAlmostEqual(this IList<double> a, IList<double> b, int decimalPlaces)
        {
            return ListForAll(a, b, AlmostEqual, decimalPlaces);
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool ListAlmostEqual(this IList<float> a, IList<float> b, int decimalPlaces)
        {
            return ListForAll(a, b, AlmostEqual, decimalPlaces);
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool ListAlmostEqualRelative(this IList<double> a, IList<double> b, int decimalPlaces)
        {
            return ListForAll(a, b, AlmostEqualRelative, decimalPlaces);
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="decimalPlaces">The number of decimal places.</param>
        public static bool ListAlmostEqualRelative(this IList<float> a, IList<float> b, int decimalPlaces)
        {
            return ListForAll(a, b, AlmostEqualRelative, decimalPlaces);
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="maximumAbsoluteError">The accuracy required for being almost equal.</param>
        public static bool ListAlmostEqualNorm<T>(this IList<T> a, IList<T> b, double maximumAbsoluteError)
            where T : IPrecisionSupport<T>
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null || a.Count != b.Count)
            {
                return false;
            }

            for (int i = 0; i < a.Count; i++)
            {
                if (!AlmostEqualNorm(a[i], b[i], maximumAbsoluteError))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two lists of doubles and determines if they are equal within the
        /// specified maximum error.
        /// </summary>
        /// <param name="a">The first value list.</param>
        /// <param name="b">The second value list.</param>
        /// <param name="maximumError">The accuracy required for being almost equal.</param>
        public static bool ListAlmostEqualNormRelative<T>(this IList<T> a, IList<T> b, double maximumError)
            where T : IPrecisionSupport<T>
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null || a.Count != b.Count)
            {
                return false;
            }

            for (int i = 0; i < a.Count; i++)
            {
                if (!AlmostEqualNormRelative(a[i], b[i], maximumError))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ListForAll<T, TP>(IList<T> a, IList<T> b, Func<T, T, TP, bool> predicate, TP parameter)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null || a.Count != b.Count)
            {
                return false;
            }

            for (int i = 0; i < a.Count; i++)
            {
                if (!predicate(a[i], b[i], parameter))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
