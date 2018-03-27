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

// <copyright file="Constants.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2010 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;

namespace Osrs.Numerics
{
    public static class Constants
    {
        #region Time
        /// <summary>cesium resonance frequency for cesium fountain clock (9,192,631,770 Hz)</summary>
        public const long CesiumPerSecond = 9192631770;
        /// <summary>fraction of second per cesium resonance frequency for cesium fountain clock (9,192,631,770 Hz)</summary>
        public const double SecondPerCesium = 1.0 / 9192631770;
        /// <summary>standard seconds per year (1/31,556,925.9747 of the tropical year 1900)</summary>
        public const double EphemerisSecondsPerYear = 31556925.9747;
        /// <summary>60*60</summary>
        public const int SecondsPerHour = 3600;
        /// <summary>1/(60*60)</summary>
        public const double HoursPerSecond = 1.0 / 3600;
        /// <summary>60*60*24</summary>
        public const int SecondsPerDay = 86400;
        /// <summary>1/(60*60*24)</summary>
        public const double DaysPerSeconds = 1.0 / 86400;
        /// <summary>60*60*24*365</summary>
        public const int SecondsPerIntYear = 31536000;
        /// <summary>1/(60*60*24*365)</summary>
        public const double IntYearsPerSecond = 1.0 / 31536000;
        /// <summary>60*60*24*365.25</summary>
        public const int SecondsPerMeanYear = 31557600;
        /// <summary>1/(60*60*24*365.25)</summary>
        public const double MeanYearsPerSecond = 1.0 / 31557600;
        /// <summary>60*24</summary>
        public const int MinutesPerDay = 1440;
        /// <summary>60*24</summary>
        public const double DaysPerMinute = 1.0 / 1440;
        /// <summary>60*24*365</summary>
        public const int MinutesPerIntYear = 525600;
        /// <summary>60*24*365.25</summary>
        public const int MinutesPerMeanYear = 525960;
        #endregion

        #region LogConstants
        /// <summary>log[2](e)</summary>
        public const double Log2E = 1.4426950408889634073599246810018921374266459541530d;
        /// <summary>log[10](e)</summary>
        public const double Log10E = 0.43429448190325182765112891891660508229439700580366d;
        /// <summary>log[e](2)</summary>
        public const double Ln2 = 0.69314718055994530941723212145817656807550013436026d;
        /// <summary>log[e](10)</summary>
        public const double Ln10 = 2.3025850929940456840179914546843642076011014886288d;
        /// <summary>log[e](pi)</summary>
        public const double LnPi = 1.1447298858494001741434273513530587116472948129153d;
        /// <summary>The number log[e](2*pi)/2</summary>
        public const double Ln2PiOver2 = 0.91893853320467274178032973640561763986139747363780d;
        #endregion

        #region BaseConstants
        /// <summary>2^(-53)</summary>
        public static readonly double RelativeAccuracy = MathUtils.EpsilonOf(1.0d);
        /// <summary>2^(-52)</summary>
        public static readonly double PositiveRelativeAccuracy = MathUtils.PositiveEpsilonOf(1.0d);
        /// <summary>10 * 2^(-52)</summary>
        public static readonly double DefaultRelativeAccuracy = 10 * PositiveRelativeAccuracy;
        /// <summary>Safe minimum, such that 1 / SAFE_MIN does not overflow.  <p>In IEEE 754 arithmetic, this is also the smallest normalized number 2<sup>-1022</sup>.</p></summary>
        public const double SafeMin = 2.2250738585072014E-308;
        public const double OneThird = 1.0d / 3.0d;
        public const double TwoThirds = 2.0d / 3.0d;
        public const double FourThirds = 4.0d / 3.0d;
        public const double OneSixth = 1.0d / 6.0d;
        /// <summary>sqrt(2)</summary>
        public const double SqrtTwo = 1.4142135623730950488016887242096980785696718753769d;
        /// <summary>sqrt(1/2) = 1/sqrt(2) = sqrt(2)/2</summary>
        public const double SqrtOneHalf = 0.70710678118654752440084436210484903928483593768845d;
        /// <summary>sqrt(3)/2</summary>
        public const double HalfSqrt3 = 0.86602540378443864676372317075293618347140262690520d;
        /// <summary>sqrt(3)/4</summary>
        public const double QuarterSqrt3 = 0.4330127018922193d;
        #endregion

        #region Transcendentals
        /// <summary>catalan constant</summary>
        public const double Catalan = 0.9159655941772190150546035149323841107741493742816721342664981196217630197762547694794d;
        /// <summary>euler-mascheroni constant</summary>
        public const double EulerGamma = 0.5772156649015328606065120900824024310421593359399235988057672348849d;
        /// <summary>golden ratio (1+sqrt(5))/2</summary>
        public const double GoldenRatio = 1.6180339887498948482045868343656381177203091798057628621354486227052604628189024497072d;
        /// <summary>Glaisher Constant e^(1/12 - Zeta(-1))</summary>
        public const double Glaisher = 1.2824271291006226368753425688697917277676889273250011920637400217404063088588264611297d;
        /// <summary>Khinchin constant prod(k=1 -> inf){1+1/(k*(k+2))^log(k,2)}</summary>
        public const double Khinchin = 2.6854520010653064453097148354817956938203822939944629530511523455572188595371520028011d;
        /// <summary>The Euler-Mascheroni constant</summary>
        /// <remarks>lim(n -> inf){ Sum(k=1 -> n) { 1/k - log(n) } }</remarks>
        public const double EulerMascheroni = 0.5772156649015328606065120900824024310421593359399235988057672348849d;
        /// <summary>1/e</summary>
        public const double InvE = 0.36787944117144232159552377016146086744581113103176d;
        /// <summary>sqrt(e)</summary>
        public const double SqrtE = 1.6487212707001281468486507878141635716537761007101d;
        #endregion

        #region AngularUnits
        /// <summary>-pi</summary>
        public const double NPi = -Math.PI;
        /// <summary>-2*pi</summary>
        public const double NTwoPi = -2.0 * Math.PI;
        /// <summary>2*pi</summary>
        public const double TwoPi = (2.0d * System.Math.PI);
        /// <summary>4*pi</summary>
        public const double FourPi = (4.0d * System.Math.PI);
        /// <summary>(5*pi)/2</summary>
        public const double FiveHalfsPi = (5.0d * System.Math.PI) / 2.0d;
        /// <summary>(3*pi)/2</summary>
        public const double ThreeHalfsPi = (3.0d * System.Math.PI) / 2.0d;
        /// <summary>(4/3)*pi</summary>
        public const double FourThirdsPi = (Constants.FourThirds * System.Math.PI);
        /// <summary>(3*pi)/4</summary>
        public const double ThreeFourthsPi = (3.0 * Math.PI) / 4.0;
        /// <summary>pi/2</summary>
        public const double HalfPi = 1.5707963267948966192313216916397514420985846996876d;
        /// <summary>pi/4</summary>
        public const double QuarterPi = 0.78539816339744830961566084581987572104929234984378d;
        /// <summary>sqrt(pi)</summary>
        public const double SqrtPi = 1.7724538509055160272981674833411451827975494561224d;
        /// <summary>sqrt(2pi)</summary>
        public const double SqrtTwoPi = 2.5066282746310005024157652848110452530069867406099d;
        /// <summary>1/pi</summary>
        public const double InvPi = 0.31830988618379067153776752674502872406891929148091d;
        /// <summary>2/pi</summary>
        public const double TwoInvPi = 0.63661977236758134307553505349005744813783858296182d;
        /// <summary>1/sqrt(pi)</summary>
        public const double InvSqrtPi = 0.56418958354775628694807945156077258584405062932899d;
        /// <summary>1/sqrt(2pi)</summary>
        public const double InvSqrtTwoPi = 0.39894228040143267793994605993438186847585863116492d;
        /// <summary>2/sqrt(pi)</summary>
        public const double TwoInvSqrtPi = 1.1283791670955125738961589031215451716881012586580d;
        /// <summary>degrees in a circle = 360</summary>
        public const double CircleDegrees = 360.000000d;
        /// <summary>degrees in a half-circle = 180</summary>
        public const double HalfCircleDegrees = 180.000000d;
        /// <summary>degrees in a quarter-circle = 90</summary>
        public const double QuarterCircleDegrees = 90.000000d;
        /// <summary>degrees per radian (180/pi)</summary>
        public const double DegreesPerRadian = (180.0d / Math.PI);
        /// <summary>radians per degree (pi/180)</summary>
        public const double RadiansPerDegree = 0.017453292519943295769236907684886127134428718885417d;
        /// <summary>grad per radian (pi)/200</summary>
        public const double GradiansPerRadian = 0.015707963267948966192313216916397514420985846996876d;
        /// <summary>grad per radian (pi)/200</summary>
        public const double RadiansPerGradian = 1 / GradiansPerRadian;
        /// <summary>radian per arcsecond</summary>
        public const double RadianPerArcSecond = 0.0000048481368111d;
        /// <summary>arcsecond per radian</summary>
        public const double ArcSecondPerRadian = 206264.80624689895d;
        /// <summary>The number sqrt(2*pi*e)</summary>
        public const double Sqrt2PiE = 4.1327313541224929384693918842998526494455219169913d;
        /// <summary>The number log(sqrt(2*pi))</summary>
        public const double LogSqrt2Pi = 0.91893853320467274178032973640561763986139747363778;
        /// <summary>The number log(sqrt(2*pi*e))</summary>
        public const double LogSqrt2PiE = 1.4189385332046727417803297364056176398613974736378d;
        /// <summary>The number log(2 * sqrt(e / pi))</summary>
        public const double LogTwoSqrtEOverPi = 0.6207822376352452223455184457816472122518527279025978;
        /// <summary>The number 1/sqrt(2pi)</summary>
        public const double InvSqrt2Pi = 0.39894228040143267793994605993438186847585863116492d;
        /// <summary>The number 2 * sqrt(e / pi)</summary>
        public const double TwoSqrtEOverPi = 1.8603827342052657173362492472666631120594218414085755;
        #endregion

        #region Mathematical Constants
        /// <summary>
        /// The size of a double in bytes.
        /// </summary>
        public const int SizeOfDouble = sizeof(double);

        /// <summary>
        /// The size of an int in bytes.
        /// </summary>
        public const int SizeOfInt = sizeof(int);

        /// <summary>
        /// The size of a float in bytes.
        /// </summary>
        public const int SizeOfFloat = sizeof(float);

        /// <summary>
        /// The size of a Complex in bytes.
        /// </summary>
        public const int SizeOfComplex = 2 * SizeOfDouble;
        #endregion

        #region MetricUnits
        /// <summary>1 000 000 000 000 000 000 000 000</summary>
        public const double Yotta = 1e24;
        /// <summary>1 000 000 000 000 000 000 000</summary>
        public const double Zetta = 1e21;
        /// <summary>1 000 000 000 000 000 000</summary>
        public const long Exa = 1000000000000000000;
        /// <summary>1 000 000 000 000 000</summary>
        public const long Peta = 1000000000000000;
        /// <summary>1 000 000 000 000</summary>
        public const long Tera = 1000000000000;
        /// <summary>1 000 000 000</summary>
        public const int Giga = 1000000000;
        /// <summary>1 000 000</summary>
        public const int Mega = 1000000;
        /// <summary>1 000</summary>
        public const int Kilo = 1000;
        /// <summary>100</summary>
        public const int Hecto = 100;
        /// <summary>10</summary>
        public const int Deca = 10;
        /// <summary>0.1</summary>
        public const double Deci = 1e-1;
        /// <summary>0.01</summary>
        public const double Centi = 1e-2;
        /// <summary>0.001</summary>
        public const double Milli = 1e-3;
        /// <summary>0.000 001</summary>
        public const double Micro = 1e-6;
        /// <summary>0.000 000 001</summary>
        public const double Nano = 1e-9;
        /// <summary>0.000 000 000 001</summary>
        public const double Pico = 1e-12;
        /// <summary>0.000 000 000 000 001</summary>
        public const double Femto = 1e-15;
        /// <summary>0.000 000 000 000 000 001</summary>
        public const double Atto = 1e-18;
        /// <summary>0.000 000 000 000 000 000 001</summary>
        public const double Zepto = 1e-21;
        /// <summary>0.000 000 000 000 000 000 000 001</summary>
        public const double Yocto = 1e-24;
        #endregion
    }
}
