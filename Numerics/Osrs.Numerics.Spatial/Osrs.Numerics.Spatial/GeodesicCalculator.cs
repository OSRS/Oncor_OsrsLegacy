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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial
{
    public sealed class GeodesicCalculator
    {
        private const double meanRadiusKm = 6371.0;  //km
        private const double meanRadius = 6371000.0; //m
        private readonly CalculationMethod method;
        private readonly DirectionMethod dirMethod;

        public GeodesicCalculator(CalculationMethod method, DirectionMethod directionMethod)
        {
            this.method = method;
            this.dirMethod = directionMethod;
        }

        /// <summary>
        /// Compute the Cartesian distance between two coordinates using the current preferences for performance
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The great circle distance from a to b</returns>
        public double Distance(double fromX, double fromY, double toX, double toY)
        {
            if (this.method == CalculationMethod.Fast)
                return DistanceCosines(fromX, fromY, toX, toY);
            if (this.method == CalculationMethod.Balanced)
                return DistanceHaversine(fromX, fromY, toX, toY);
            if (Math.Max(Math.Abs(toY - fromY), Math.Abs(toX - fromX)) < 1)
                return DistanceBowring(fromX, fromY, toX, toY);
            return DistanceVincente(fromX, fromY, toX, toY);
        }

        /// <summary>
        /// Compute the Cartesian distance between two coordinates using the law of cosines method (fast, low accuracy)
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The great circle distance from a to b</returns>
        public double DistanceCosines(double fromX, double fromY, double toX, double toY)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            toX = AngleUtils.ToRadians(toX);
            toY = AngleUtils.ToRadians(toY);
            double a = Math.Acos(Math.Sin(fromY) * Math.Sin(toY) + Math.Cos(fromY) * Math.Cos(toY) * Math.Cos(toX - fromX));
            return a * meanRadius;
        }

        /// <summary>
        /// Compute the Cartesian distance between two coordinates using the Haversine method (moderately fast, moderately accurate)
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The great circle distance from a to b</returns>
        public double DistanceHaversine(double fromX, double fromY, double toX, double toY)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            toX = AngleUtils.ToRadians(toX);
            toY = AngleUtils.ToRadians(toY);
            double dY = toY - fromY;
            double dX = toX - fromX;

            double a = Math.Pow(Math.Sin(dY / 2), 2.0) + (Math.Cos(toY) * Math.Cos(fromY) * Math.Pow(Math.Sin(dX / 2), 2.0));
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return meanRadius * c;
        }

        //WGS 84 ellipsoial consts
        private const double a = 6378137;
        private const double b = 6356752.314245;
        private const double f = 1 / 298.257223563;
        /// <summary>
        /// Compute the Cartesian distance between two coordinatesusing the Vincente method (slower - iterative, highly accurate)
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The great circle distance from a to b</returns>
        public double DistanceVincente(double fromX, double fromY, double toX, double toY)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            toX = AngleUtils.ToRadians(toX);
            toY = AngleUtils.ToRadians(toY);
            double L = toX - fromX;
            double U1 = Math.Atan((1 - f) * Math.Tan(fromY));
            double U2 = Math.Atan((1 - f) * Math.Tan(toY));
            double sinU1 = Math.Sin(U1);
            double cosU1 = Math.Cos(U1);
            double sinU2 = Math.Sin(U2);
            double cosU2 = Math.Cos(U2);
            double lambda = L;
            double lambdaP;
            double iterLimit = 100;
            double cosSqAlpha = 0.0;
            double sinSigma = 0.0;
            double cos2SigmaM = 0.0;
            double cosSigma = 0.0;
            double sigma = 0.0;
            do
            {
                double sinLambda = Math.Sin(lambda);
                double cosLambda = Math.Cos(lambda);
                sinSigma = Math.Sqrt((cosU2 * sinLambda) * (cosU2 * sinLambda) + (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) * (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
                if (sinSigma == 0)
                    return 0; //coincident points
                cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
                sigma = Math.Atan2(sinSigma, cosSigma);
                double sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
                cosSqAlpha = 1 - sinAlpha * sinAlpha;
                cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / cosSqAlpha;
                if (double.IsNaN(cos2SigmaM))
                    cos2SigmaM = 0; //equatorial line: cosSqAlpha=0
                double C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
                lambdaP = lambda;
                lambda = L + (1 - C) * f * sinAlpha * (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
            } while (Math.Abs(lambda - lambdaP) > 1e-12 && --iterLimit > 0);
            if (iterLimit == 0)
                return this.DistanceHaversine(fromX, fromY, toX, toY); // formula failed to converge, use next method
            double uSq = cosSqAlpha * (a * a - b * b) / (b * b);
            double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
            double deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) - B / 6 * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));

            return b * A * (sigma - deltaSigma);
        }

        private static readonly double er = (Math.Pow(a, 2.0) - Math.Pow(b, 2.0)) / Math.Pow(b, 2.0);
        private static readonly double er3 = 3 * er;
        /// <summary>
        /// Compute the Cartesian distance between two coordinatesusing the Bowring method (moderately fast, moderately accurate)
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The great circle distance from a to b</returns>
        public double DistanceBowring(double fromX, double fromY, double toX, double toY)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            toX = AngleUtils.ToRadians(toX);
            toY = AngleUtils.ToRadians(toY);
            double a = Math.Sqrt(1 + er * Math.Pow(Math.Cos(fromY), 4.0));
            double b = Math.Sqrt(1 + er * Math.Pow(Math.Cos(fromY), 2.0));
            double c = Math.Sqrt(1 + er);

            double dLat = toY - fromY;
            double dLon = toX - fromX;
            double w = (a * dLon) / 2.0;

            double d = dLat * Math.Sin(2 * fromY + Constants.TwoThirds * dLat);
            d = d * (1 + (er3 / (4 * Math.Pow(b, 2.0))));
            d = d * (dLat / (2.0 * b));

            double ia = 1 / a;
            double e = Math.Sin(d) * Math.Cos(w);
            double f = ia * Math.Sin(w) * (b * Math.Cos(fromY) * Math.Cos(d) - Math.Sin(fromY) * Math.Sin(d));
            //double g = Math.Atan(f / e);
            double s = Math.Asin(Math.Sqrt(Math.Pow(e, 2.0) + Math.Pow(f, 2.0)) * 2.0);
            //double h = ia * (Math.Sin(fromY) + b * Math.Cos(fromY) * Math.Tan(d)) * Math.Tan(w);

            return (a * c * s) / Math.Pow(b, 2.0);
        }

        /// <summary>
        /// Compute the Cartesian distance between two coordinates using a loxodrome path
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The rhumb line distance from a to b</returns>
        public double DistanceLoxodrome(double fromX, double fromY, double toX, double toY)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            toX = AngleUtils.ToRadians(toX);
            toY = AngleUtils.ToRadians(toY);
            double dLon = toX - fromX;
            double dLat = toY - fromY;
            double dPhi = Math.Log(Math.Tan(toY / 2 + Math.PI / 4) / Math.Tan(fromY / 2 + Math.PI / 4));
            double q = (!double.IsNaN(dLat / dPhi)) ? dLat / dPhi : Math.Cos(fromY);

            // E-W line gives dPhi=0
            // if dLon over 180° take shorter rhumb across 180° meridian:
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);

            return Math.Sqrt(dLat * dLat + q * q * dLon * dLon) * meanRadius;
        }

        /// <summary>
        /// Compute the Manhattan (dX+dY) distance between two coordinates using Great Circle methods for each axis
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The manhattan distance from a to b over 2 great circle axial paths (x and y)</returns>
        public double ManhattanDistance(double fromX, double fromY, double toX, double toY)
        {
            double dY = this.Distance(fromX, fromY, fromX, toY); // hold X
            double dX = this.Distance(fromX, fromY, toX, fromY); // hold Y
            return dY + dX;
        }

        /// <summary>
        /// Compute the compass direction between two points using the current preferences for directions
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The compass direction from a to b</returns>
        public double Direction(double fromX, double fromY, double toX, double toY)
        {
            switch (this.dirMethod)
            {
                case DirectionMethod.Loxodrome:
                    return this.DirectionLoxodrome(fromX, fromY, toX, toY);
                case DirectionMethod.StartBearing:
                    return this.DirectionStartBearing(fromX, fromY, toX, toY);
            }
            return this.DirectionMidBearing(fromX, fromY, toX, toY);
        }

        /// <summary>
        /// Compute the compass direction between two points using the starting bearing as the direction
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The compass direction from a to b</returns>
        public double DirectionStartBearing(double fromX, double fromY, double toX, double toY)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            toX = AngleUtils.ToRadians(toX);
            toY = AngleUtils.ToRadians(toY);
            double dLon = toX - fromX;
            double y = Math.Sin(dLon) * Math.Cos(toY);
            double x = Math.Cos(fromY) * Math.Sin(toY) - Math.Sin(fromY) * Math.Cos(toY) * Math.Cos(dLon);
            return AngleUtils.ToDegrees(Math.Atan2(y, x));
        }

        /// <summary>
        /// Compute the compass direction between two points using the Mid-point of the path as the direction
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The compass direction from a to b</returns>
        public double DirectionMidBearing(double fromX, double fromY, double toX, double toY)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            toX = AngleUtils.ToRadians(toX);
            toY = AngleUtils.ToRadians(toY);
            double dLon = toX - fromX;
            var Bx = Math.Cos(toY) * Math.Cos(dLon);
            var By = Math.Cos(toY) * Math.Sin(dLon);
            var lat3 = AngleUtils.ToDegrees(Math.Atan2(Math.Sin(fromY) + Math.Sin(toY), Math.Sqrt((Math.Cos(fromY) + Bx) * (Math.Cos(fromY) + Bx) + By * By)));
            var lon3 = AngleUtils.ToDegrees(fromX + Math.Atan2(By, Math.Cos(fromY) + Bx));
            return DirectionStartBearing(lon3, lat3, toX, toY);
        }

        /// <summary>
        /// Compute the compass direction between two points using the loxodrome path as the direction
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="toX">destination X ordinate</param>
        /// <param name="toY">destination Y ordinate</param>
        /// <returns>The compass direction from a to b</returns>
        public double DirectionLoxodrome(double fromX, double fromY, double toX, double toY)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            toX = AngleUtils.ToRadians(toX);
            toY = AngleUtils.ToRadians(toY);
            double dLon = toX - fromX;
            double dLat = toY - fromY;
            double dPhi = Math.Log(Math.Tan(toY / 2 + Math.PI / 4) / Math.Tan(fromY / 2 + Math.PI / 4));
            double q = (!double.IsNaN(dLat / dPhi)) ? dLat / dPhi : Math.Cos(fromY);

            // E-W line gives dPhi=0
            // if dLon over 180° take shorter rhumb across 180° meridian:
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);

            return Math.Atan2(dLon, dPhi);
        }

        /// <summary>
        /// Compute the "destination" point by travelling along a great circle path starting at the provided point.
        /// The path is based upon an initial direction and a fixed distance to travel along the great circle (may circumnavigate the globe)
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="distance">the distance to travel in meters</param>
        /// <param name="direction">the direction to travel in radians of compass AngleUtils (0 is north)</param>
        /// <returns>A coordinate of the final location</returns>
        public Coordinate2<double> Destination(double fromX, double fromY, double distance, double direction)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            double dR = distance / meanRadius;
            double brng = AngleUtils.ToRadians(direction);
            double lat2 = Math.Asin(Math.Sin(fromY) * Math.Cos(dR) + Math.Cos(fromY) * Math.Sin(dR) * Math.Cos(brng));
            double lon2 = fromX + Math.Atan2(Math.Sin(brng) * Math.Sin(dR) * Math.Cos(fromY), Math.Cos(dR) - Math.Sin(fromY) * Math.Sin(lat2));
            return new Coordinate2<double>(lon2, lat2);
        }

        private const double QuarterPi = Math.PI / 4;
        /// <summary>
        /// Compute the "destination" point by travelling along a loxodrome starting at the provided point.
        /// The path is based upon an constant direction and a fixed distance to travel along the loxodrome (may circumnavigate the globe, eventually spiral to pole)
        /// </summary>
        /// <param name="fromX">source X ordinate</param>
        /// <param name="fromY">source Y ordinate</param>
        /// <param name="distance">the distance to travel in meters</param>
        /// <param name="direction">the direction to travel in radians of compass AngleUtils (0 is north)</param>
        /// <returns>A coordinate of the final location</returns>
        public Coordinate2<double> DestinationLoxodrome(double fromX, double fromY, double distance, double direction)
        {
            fromX = AngleUtils.ToRadians(fromX);
            fromY = AngleUtils.ToRadians(fromY);
            double brng = AngleUtils.ToRadians(direction);
            double dLat = distance * Math.Cos(brng);
            double lat2 = fromY + dLat;
            double dPhi = Math.Log(Math.Tan(lat2 / 2 + QuarterPi) / Math.Tan(fromY / 2 + QuarterPi));
            double q = !double.IsNaN(dLat / dPhi) ? dLat / dPhi : Math.Cos(fromY);
            double dLon = distance * Math.Sin(brng) / q;

            if (Math.Abs(lat2) > Constants.HalfPi)
                lat2 = lat2 > 0 ? Math.PI - lat2 : -(Math.PI - lat2);
            double lon2 = (fromX + dLon + Math.PI) % Constants.TwoPi - Math.PI;
            return new Coordinate2<double>(lon2, lat2);
        }

        //Distance from the great circle path to the external point
        /// <summary>
        /// Compute the distance to the great circle passing through the path points from the external point.
        /// This is the distance in meters from the external point, to the great circle containing the two path points.
        /// </summary>
        /// <param name="fromPath">First point on the great circle</param>
        /// <param name="toPath">Second point on the great circle</param>
        /// <param name="external">Point to compute the distance from the great circle to</param>
        /// <returns>The distance in meters to the external point</returns>
        public double DistanceTo(Coordinate2<double> fromPath, Coordinate2<double> toPath, Coordinate2<double> external)
        {
            double d13 = this.Distance(fromPath.X, fromPath.Y, toPath.X, toPath.Y);
            double o13 = this.Direction(fromPath.X, fromPath.Y, toPath.X, toPath.Y);
            double o12 = this.Direction(fromPath.X, fromPath.Y, external.X, external.Y);
            return Math.Asin(Math.Sin(d13 / meanRadius) * Math.Sin(o13 - o12)) * meanRadius;
        }

        //Distance from the start point of the great circle to the point on the path closest to the external point
        /// <summary>
        /// Computes the distance of the point on the great circle between the two path points that is closest to the external point.
        /// This is the distance from the fromPath point along the great circle, that minimizes the distance to the external point.
        /// </summary>
        /// <param name="fromPath">First point on the great circle</param>
        /// <param name="toPath">Second point on the great circle</param>
        /// <param name="external">Point to compute the distance along the great circle from the start point to</param>
        /// <returns>The distance in meters from the fromPath point, to the point on the great circle closest to the external point</returns>
        public double NearestDistance(Coordinate2<double> fromPath, Coordinate2<double> toPath, Coordinate2<double> external)
        {
            double d13 = this.Distance(fromPath.X, fromPath.Y, toPath.X, toPath.Y);
            double dt = this.DistanceTo(fromPath, toPath, external);
            return Math.Acos(Math.Cos(d13 / meanRadius) / Math.Cos(dt / meanRadius)) * meanRadius;
        }
    }
}
