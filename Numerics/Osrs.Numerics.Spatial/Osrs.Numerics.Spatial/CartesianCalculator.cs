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

using Osrs.Data;
using Osrs.Numerics.Spatial.Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial
{
    public static class CartesianCalculator
    {
        /// <summary>
        /// Compute the Cartesian distance between two coordinates
        /// </summary>
        /// <param name="a">The source coordinate</param>
        /// <param name="b">The destination coordinate</param>
        /// <returns>The Cartesian distance from a to b</returns>
        public static double Distance(Coordinate2<double> a, Coordinate2<double> b)
        {
            return Distance(a.X, a.Y, b.X, b.Y);
        }
        /// <summary>
        /// Compute the Cartesian distance between two coordinates
        /// </summary>
        /// <param name="a">The source coordinate</param>
        /// <param name="b">The destination coordinate</param>
        /// <returns>The Cartesian distance from a to b</returns>
        public static long Distance(Coordinate2<long> a, Coordinate2<long> b)
        {
            return Distance(a.X, a.Y, b.X, b.Y);
        }
        /// <summary>
        /// Compute the Cartesian distance between two coordinates
        /// </summary>
        /// <param name="a">The source coordinate</param>
        /// <param name="b">The destination coordinate</param>
        /// <returns>The Cartesian distance from a to b</returns>
        public static int Distance(Coordinate2<int> a, Coordinate2<int> b)
        {
            return Distance(a.X, a.Y, b.X, b.Y);
        }
        /// <summary>
        /// Compute the Cartesian distance between two coordinates
        /// </summary>
        /// <param name="fromX">the from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <returns>The Cartesian distance from a to b</returns>
        public static double Distance(double fromX, double fromY, double toX, double toY)
        {
            double dY = toY - fromY;
            double dX = toX - fromX;
            dY = dY * dY;
            dX = dX * dX;
            return Math.Sqrt(dY + dX);
        }
        /// <summary>
        /// Compute the Cartesian distance between two coordinates
        /// </summary>
        /// <param name="fromX">the from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <returns>The Cartesian distance from a to b</returns>
        public static long Distance(long fromX, long fromY, long toX, long toY)
        {
            long dY = toY - fromY;
            long dX = toX - fromX;
            dY = dY * dY;
            dX = dX * dX;
            return (long)Math.Round(Math.Sqrt(dY + dX));
        }
        /// <summary>
        /// Compute the Cartesian distance between two coordinates
        /// </summary>
        /// <param name="fromX">the from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <returns>The Cartesian distance from a to b</returns>
        public static int Distance(int fromX, int fromY, int toX, int toY)
        {
            long dY = toY - fromY;
            long dX = toX - fromX;
            dY = dY * dY;
            dX = dX * dX;
            return (int)Math.Round(Math.Sqrt(dY + dX));
        }

        /// <summary>
        /// Compute the Cartesian Manhattan distance (dX+dY) between two coordinates
        /// </summary>
        /// <param name="fromX">the from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <returns>The Cartesian Manhattan distance (dX+dY) from a to b</returns>
        public static double ManhattanDistance(double fromX, double fromY, double toX, double toY)
        {
            double dY = Math.Abs(toY - fromY);
            double dX = Math.Abs(toX - fromX);
            return dY + dX;
        }
        /// <summary>
        /// Compute the Cartesian Manhattan distance (dX+dY) between two coordinates
        /// </summary>
        /// <param name="fromX">the from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <returns>The Cartesian Manhattan distance (dX+dY) from a to b</returns>
        public static long ManhattanDistance(long fromX, long fromY, long toX, long toY)
        {
            long dY = Math.Abs(toY - fromY);
            long dX = Math.Abs(toX - fromX);
            return dY + dX;
        }
        /// <summary>
        /// Compute the Cartesian Manhattan distance (dX+dY) between two coordinates
        /// </summary>
        /// <param name="fromX">the from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <returns>The Cartesian Manhattan distance (dX+dY) from a to b</returns>
        public static int ManhattanDistance(int fromX, int fromY, int toX, int toY)
        {
            int dY = Math.Abs(toY - fromY);
            int dX = Math.Abs(toX - fromX);
            return dY + dX;
        }

        /// <summary>
        /// Compute the direction in radians between two coordinates.
        /// The AngleUtils is represented as 0 for "up" (North) and increases clockwise to 2*pi back at "up"
        /// </summary>
        /// <param name="fromX">the from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <returns>The Cartesian Compass direction from a to b</returns>
        public static double Direction(double fromX, double fromY, double toX, double toY)
        {
            double dY = toY - fromY;
            double dX = toX - fromX;
            if (dX == 0)
            {
                if (dY > 0.0D)
                    return 0.0D;
                return Math.PI;
            }
            dX = Math.Atan2(dY, dX);
            if (dX < Constants.HalfPi) //Q1, Q2 or Q3
                return Constants.HalfPi - dX;
            //Q4
            return Constants.FiveHalfsPi - dX;
        }

        /// <summary>
        /// Calculates the toY for a given origin, distance and fixed toX.  if there is no solution, return NaN
        /// </summary>
        /// <param name="fromX">The from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="distance">the distance from the from x/y at which to derive an intersecting y</param>
        /// <returns>the toY at the provided distance from the provided from x/y</returns>
        public static ValuePair<double> MatchY(double fromX, double fromY, double toX, double distance)
        {
            double dS = distance * distance;
            double dX = toX - fromX;
            if (dX > dS) //distance between the x-ordinates at an equal y is greater than distance - unsolvable
                return new ValuePair<double>(fromX, fromY);
            dX = dX * dX;
            double dY = dS - dX;
            dY = Math.Sqrt(dY); //both + and - of this could be valid
            return new ValuePair<double>(fromY - dY, fromY + dY);
        }
        /// <summary>
        /// Calculates the toY for a given origin, distance and fixed toX.  if there is no solution, return NaN
        /// </summary>
        /// <param name="fromX">The from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="distance">the distance from the from x/y at which to derive an intersecting y</param>
        /// <returns>the toY at the provided distance from the provided from x/y</returns>
        public static ValuePair<long> MatchY(long fromX, long fromY, long toX, long distance)
        {
            if (Math.Abs(toX - fromX) >= Math.Abs(distance))
                return new ValuePair<long>(fromY, fromY);
            long dS = distance * distance;
            long dX = toX - fromX;
            if (dX > dS) //distance between the x-ordinates at an equal y is greater than distance - unsolvable
                return new ValuePair<long>(fromX, fromY);
            dX = dX * dX;
            long dY = dS - dX;
            dY = (long)Math.Sqrt(dY); //both + and - of this could be valid
            return new ValuePair<long>(fromY - dY, fromY + dY);
        }
        /// <summary>
        /// Calculates the toY for a given origin, distance and fixed toX.  if there is no solution, return NaN
        /// </summary>
        /// <param name="fromX">The from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toX">the to x ordinate</param>
        /// <param name="distance">the distance from the from x/y at which to derive an intersecting y</param>
        /// <returns>the toY at the provided distance from the provided from x/y</returns>
        public static ValuePair<int> MatchY(int fromX, int fromY, int toX, int distance)
        {
            if (Math.Abs(toX - fromX) >= Math.Abs(distance))
                return new ValuePair<int>(fromY, fromY);
            int dS = distance * distance;
            int dX = toX - fromX;
            if (dX > dS) //distance between the x-ordinates at an equal y is greater than distance - unsolvable
                return new ValuePair<int>(fromX, fromY);
            dX = dX * dX;
            int dY = dS - dX;
            dY = (int)Math.Sqrt(dY); //both + and - of this could be valid
            return new ValuePair<int>(fromY - dY, fromY + dY);
        }

        /// <summary>
        /// Calculates the toX for a given origin, distance and fixed toY.  if there is no solution, return NaN
        /// </summary>
        /// <param name="fromX">The from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <param name="distance">the distance from the from x/y at which to derive an intersecting x</param>
        /// <returns>the toX at the provided distance from the provided from x/y</returns>
        public static ValuePair<double> MatchX(double fromX, double fromY, double toY, double distance)
        {
            double dS = distance * distance;
            double dY = toY - fromY;
            if (dY > dS) //distance between the x-ordinates at an equal y is greater than distance - unsolvable
                return new ValuePair<double>(fromX, fromY);
            dY = dY * dY;
            double dX = dS - dY;
            dX = Math.Sqrt(dX); //both + and - of this could be valid
            return new ValuePair<double>(fromX - dX, fromX + dX);
        }
        /// <summary>
        /// Calculates the toX for a given origin, distance and fixed toY.  if there is no solution, return NaN
        /// </summary>
        /// <param name="fromX">The from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <param name="distance">the distance from the from x/y at which to derive an intersecting x</param>
        /// <returns>the toX at the provided distance from the provided from x/y</returns>
        public static ValuePair<long> MatchX(long fromX, long fromY, long toY, long distance)
        {
            if (Math.Abs(toY - fromY) >= Math.Abs(distance))
                return new ValuePair<long>(fromX, fromX);
            long dS = distance * distance;
            long dY = toY - fromY;
            if (dY > dS) //distance between the x-ordinates at an equal y is greater than distance - unsolvable
                return new ValuePair<long>(fromX, fromY);
            dY = dY * dY;
            long dX = dS - dY;
            dX = (long)Math.Sqrt(dX); //both + and - of this could be valid
            return new ValuePair<long>(fromX - dX, fromX + dX);
        }
        /// <summary>
        /// Calculates the toX for a given origin, distance and fixed toY.  if there is no solution, return NaN
        /// </summary>
        /// <param name="fromX">The from x ordinate</param>
        /// <param name="fromY">the from y ordinate</param>
        /// <param name="toY">the to y ordinate</param>
        /// <param name="distance">the distance from the from x/y at which to derive an intersecting x</param>
        /// <returns>the toX at the provided distance from the provided from x/y</returns>
        public static ValuePair<int> MatchX(int fromX, int fromY, int toY, int distance)
        {
            if (Math.Abs(toY - fromY) >= Math.Abs(distance))
                return new ValuePair<int>(fromX, fromX);
            int dS = distance * distance;
            int dY = toY - fromY;
            if (dY > dS) //distance between the x-ordinates at an equal y is greater than distance - unsolvable
                return new ValuePair<int>(fromX, fromY);
            dY = dY * dY;
            int dX = dS - dY;
            dX = (int)Math.Sqrt(dX); //both + and - of this could be valid
            return new ValuePair<int>(fromX - dX, fromX + dX);
        }

        /// <summary>
        /// Get an enumerable of coordinates at the provided radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<int>> GetRadial(Coordinate2<int> centroid, int radius)
        {
            return GetRadial(centroid, radius, 0, 0, int.MaxValue, int.MaxValue);
        }
        /// <summary>
        /// Get an enumerable of coordinates at the provided radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <param name="minX">Minimum allowed x ordinate in result set, culls possible points</param>
        /// <param name="minY">Minimum allowed y ordinate in result set, culls possible points</param>
        /// <param name="maxX">Maximum allowed x ordinate in result set, culls possible points</param>
        /// <param name="maxY">Maximum allowed y ordinate in result set, culls possible points</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<int>> GetRadial(Coordinate2<int> centroid, int radius, int minX, int minY, int maxX, int maxY)
        {
            return new RadialIntEnumerable(centroid, radius, minX, minY, maxX, maxY);
        }

        /// <summary>
        /// Get an enumerable of coordinates at the provided radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<long>> GetRadial(Coordinate2<long> centroid, long radius)
        {
            return GetRadial(centroid, radius, 0, 0, long.MaxValue, long.MaxValue);
        }
        /// <summary>
        /// Get an enumerable of coordinates at the provided radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <param name="minX">Minimum allowed x ordinate in result set, culls possible points</param>
        /// <param name="minY">Minimum allowed y ordinate in result set, culls possible points</param>
        /// <param name="maxX">Maximum allowed x ordinate in result set, culls possible points</param>
        /// <param name="maxY">Maximum allowed y ordinate in result set, culls possible points</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<long>> GetRadial(Coordinate2<long> centroid, long radius, long minX, long minY, long maxX, long maxY)
        {
            return new RadialLongEnumerable(centroid, radius, minX, minY, maxX, maxY);
        }

        /// <summary>
        /// Get an enumerable of coordinates at the provided radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<double>> GetRadial(Coordinate2<double> centroid, double radius)
        {
            return GetRadial(centroid, radius, 0.1d, 0.0d, 0.0d, double.MaxValue, double.MaxValue);
        }
        /// <summary>
        /// Get an enumerable of coordinates at the provided radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <param name="epsilon">The epsilon (threshold) distance between coordinates (often related to cell size in a grid)</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<double>> GetRadial(Coordinate2<double> centroid, double radius, double epsilon)
        {
            return GetRadial(centroid, radius, epsilon, 0.0d, 0.0d, double.MaxValue, double.MaxValue);
        }
        /// <summary>
        /// Get an enumerable of coordinates at the provided radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <param name="epsilon">The epsilon (threshold) distance between coordinates (often related to cell size in a grid)</param>
        /// <param name="minX">Minimum allowed x ordinate in result set, culls possible points</param>
        /// <param name="minY">Minimum allowed y ordinate in result set, culls possible points</param>
        /// <param name="maxX">Maximum allowed x ordinate in result set, culls possible points</param>
        /// <param name="maxY">Maximum allowed y ordinate in result set, culls possible points</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<double>> GetRadial(Coordinate2<double> centroid, double radius, double epsilon, double minX, double minY, double maxX, double maxY)
        {
            return new RadialDoubleEnumerable(centroid, radius, epsilon, minX, minY, maxX, maxY);
        }

        /// <summary>
        /// Get an enumerable of coordinates at the provided manhattan distance (dX+dY) radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<int>> GetManhattan(Coordinate2<int> centroid, int radius)
        {
            return GetManhattan(centroid, radius, 0, 0, int.MaxValue, int.MaxValue);
        }
        /// <summary>
        /// Get an enumerable of coordinates at the provided manhattan distance (dX+dY) radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <param name="minX">Minimum allowed x ordinate in result set, culls possible points</param>
        /// <param name="minY">Minimum allowed y ordinate in result set, culls possible points</param>
        /// <param name="maxX">Maximum allowed x ordinate in result set, culls possible points</param>
        /// <param name="maxY">Maximum allowed y ordinate in result set, culls possible points</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<int>> GetManhattan(Coordinate2<int> centroid, int radius, int minX, int minY, int maxX, int maxY)
        {
            if (radius == 0)
                return null;
            int rad = Math.Abs(radius);

            bool check = false;
            int xmin = centroid.X - rad;
            int xmax = centroid.X + rad;
            int ymax = centroid.Y + rad;
            int ymin = centroid.Y - rad;
            if (minX > xmin)
            {
                xmin = minX;
                check = true;
            }
            if (maxX < xmax)
            {
                xmax = maxX;
                check = true;
            }
            if (maxY < ymax)
            {
                ymax = maxY;
                check = true;
            }
            if (minY > ymin)
            {
                ymin = minY;
                check = true;
            }

            int curX;
            int curXm;
            int curYm;
            int curYp;
            int yPic = 1;

            List<Coordinate2<int>> cells = new List<Coordinate2<int>>();

            //Compute all paired values
            curX = centroid.X + rad;
            curXm = centroid.X - rad;
            int dist;

            if (check)
            {
                #region checked
                if (xmin == centroid.X - rad)
                    cells.Add(new Coordinate2<int>(xmin, centroid.Y));
                if (xmax == centroid.X + rad)
                    cells.Add(new Coordinate2<int>(xmax, centroid.Y));
                if (ymin == centroid.Y - rad)
                    cells.Add(new Coordinate2<int>(centroid.X, ymin));
                if (ymax == centroid.Y + rad)
                    cells.Add(new Coordinate2<int>(centroid.X, ymax));
                while (curX > centroid.X)
                {
                    curYm = centroid.Y - yPic;
                    curYp = centroid.Y + yPic;
                    while (curYp <= ymax || curYm >= ymin)
                    {
                        dist = CartesianCalculator.ManhattanDistance(centroid.X, centroid.Y, curX, curYp);
                        if (dist < rad)
                            yPic++;
                        else if (dist == rad)
                        {
                            if (curYm >= ymin)
                            {
                                if (curX <= xmax)
                                    cells.Add(new Coordinate2<int>(curX, curYm));
                                if (curXm >= xmin)
                                    cells.Add(new Coordinate2<int>(curXm, curYm));
                            }
                            if (curYp <= ymax)
                            {
                                if (curX <= xmax)
                                    cells.Add(new Coordinate2<int>(curX, curYp));
                                if (curXm >= xmin)
                                    cells.Add(new Coordinate2<int>(curXm, curYp));
                            }
                        }
                        else
                            break;
                        curYp++;
                        curYm--;
                    }
                    curX--;
                    curXm++;
                }
                #endregion
            }
            else
            {
                #region unchecked
                cells.Add(new Coordinate2<int>(xmin, centroid.Y));
                cells.Add(new Coordinate2<int>(xmax, centroid.Y));
                cells.Add(new Coordinate2<int>(centroid.X, ymin));
                cells.Add(new Coordinate2<int>(centroid.X, ymax));
                while (curX > centroid.X)
                {
                    curYm = centroid.Y - yPic;
                    curYp = centroid.Y + yPic;
                    while (curYp <= ymax || curYm >= ymin)
                    {
                        dist = CartesianCalculator.ManhattanDistance(centroid.X, centroid.Y, curX, curYp);
                        if (dist < rad)
                            yPic++;
                        else if (dist == rad)
                        {
                            cells.Add(new Coordinate2<int>(curX, curYm));
                            cells.Add(new Coordinate2<int>(curXm, curYm));
                            cells.Add(new Coordinate2<int>(curX, curYp));
                            cells.Add(new Coordinate2<int>(curXm, curYp));
                        }
                        else
                            break;
                        curYp++;
                        curYm--;
                    }
                    curX--;
                    curXm++;
                }
                #endregion
            }
            return cells;
        }

        /// <summary>
        /// Get an enumerable of coordinates at the provided manhattan distance (dX+dY) radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<long>> GetManhattan(Coordinate2<long> centroid, long radius)
        {
            return GetManhattan(centroid, radius, 0, 0, long.MaxValue, long.MaxValue);
        }
        /// <summary>
        /// Get an enumerable of coordinates at the provided manhattan distance (dX+dY) radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <param name="minX">Minimum allowed x ordinate in result set, culls possible points</param>
        /// <param name="minY">Minimum allowed y ordinate in result set, culls possible points</param>
        /// <param name="maxX">Maximum allowed x ordinate in result set, culls possible points</param>
        /// <param name="maxY">Maximum allowed y ordinate in result set, culls possible points</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<long>> GetManhattan(Coordinate2<long> centroid, long radius, long minX, long minY, long maxX, long maxY)
        {
            if (radius == 0)
                return null;
            long rad = Math.Abs(radius);

            bool check = false;
            long xmin = centroid.X - rad;
            long xmax = centroid.X + rad;
            long ymax = centroid.Y + rad;
            long ymin = centroid.Y - rad;
            if (minX > xmin)
            {
                xmin = minX;
                check = true;
            }
            if (maxX < xmax)
            {
                xmax = maxX;
                check = true;
            }
            if (maxY < ymax)
            {
                ymax = maxY;
                check = true;
            }
            if (minY > ymin)
            {
                ymin = minY;
                check = true;
            }

            long curX;
            long curXm;
            long curYm;
            long curYp;
            long yPic = 1;

            List<Coordinate2<long>> cells = new List<Coordinate2<long>>();

            //Compute all paired values
            curX = centroid.X + rad;
            curXm = centroid.X - rad;
            long dist = centroid.Y - rad;

            if (check)
            {
                #region checked
                if (xmin == centroid.X - rad)
                    cells.Add(new Coordinate2<long>(xmin, centroid.Y));
                if (xmax == centroid.X + rad)
                    cells.Add(new Coordinate2<long>(xmax, centroid.Y));
                if (ymin == centroid.Y - rad)
                    cells.Add(new Coordinate2<long>(centroid.X, ymin));
                if (ymax == centroid.Y + rad)
                    cells.Add(new Coordinate2<long>(centroid.X, ymax));
                while (curX > centroid.X)
                {
                    curYm = centroid.Y - yPic;
                    curYp = centroid.Y + yPic;
                    while (curYp <= ymax || curYm >= ymin)
                    {
                        dist = CartesianCalculator.ManhattanDistance(centroid.X, centroid.Y, curX, curYp);
                        if (dist < rad)
                            yPic++;
                        else if (dist == rad)
                        {
                            if (curYm >= ymin)
                            {
                                if (curX <= xmax)
                                    cells.Add(new Coordinate2<long>(curX, curYm));
                                if (curXm >= xmin)
                                    cells.Add(new Coordinate2<long>(curXm, curYm));
                            }
                            if (curYp <= ymax)
                            {
                                if (curX <= xmax)
                                    cells.Add(new Coordinate2<long>(curX, curYp));
                                if (curXm >= xmin)
                                    cells.Add(new Coordinate2<long>(curXm, curYp));
                            }
                        }
                        else
                            break;
                        curYp++;
                        curYm--;
                    }
                    curX--;
                    curXm++;
                }
                #endregion
            }
            else
            {
                #region unchecked
                cells.Add(new Coordinate2<long>(xmin, centroid.Y));
                cells.Add(new Coordinate2<long>(xmax, centroid.Y));
                cells.Add(new Coordinate2<long>(centroid.X, ymin));
                cells.Add(new Coordinate2<long>(centroid.X, ymax));
                while (curX > centroid.X)
                {
                    curYm = centroid.Y - yPic;
                    curYp = centroid.Y + yPic;
                    while (curYp <= ymax)
                    {
                        dist = CartesianCalculator.ManhattanDistance(centroid.X, centroid.Y, curX, curYp);
                        if (dist < rad)
                            yPic++;
                        else if (dist == rad)
                        {
                            cells.Add(new Coordinate2<long>(curX, curYm));
                            cells.Add(new Coordinate2<long>(curXm, curYm));
                            cells.Add(new Coordinate2<long>(curX, curYp));
                            cells.Add(new Coordinate2<long>(curXm, curYp));
                        }
                        else
                            break;
                        curYp++;
                        curYm--;
                    }
                    curX--;
                    curXm++;
                }
                #endregion
            }

            return cells;
        }

        /// <summary>
        /// Get an enumerable of coordinates at the provided manhattan distance (dX+dY) radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<double>> GetManhattan(Coordinate2<double> centroid, double radius)
        {
            return GetManhattan(centroid, radius, 0.1d, 0.0d, 0.0d, double.MaxValue, double.MaxValue);
        }
        /// <summary>
        /// Get an enumerable of coordinates at the provided manhattan distance (dX+dY) radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <param name="epsilon">The epsilon (threshold) distance between coordinates (often related to cell size in a grid)</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<double>> GetManhattan(Coordinate2<double> centroid, double radius, double epsilon)
        {
            return GetManhattan(centroid, radius, epsilon, 0.0d, 0.0d, double.MaxValue, double.MaxValue);
        }
        /// <summary>
        /// Get an enumerable of coordinates at the provided manhattan distance (dX+dY) radius from the defined centroid / origin
        /// </summary>
        /// <param name="centroid">The origin coordinate of the radius</param>
        /// <param name="radius">The fixed radius to compute coordinates for</param>
        /// <param name="epsilon">The epsilon (threshold) distance between coordinates (often related to cell size in a grid)</param>
        /// <param name="minX">Minimum allowed x ordinate in result set, culls possible points</param>
        /// <param name="minY">Minimum allowed y ordinate in result set, culls possible points</param>
        /// <param name="maxX">Maximum allowed x ordinate in result set, culls possible points</param>
        /// <param name="maxY">Maximum allowed y ordinate in result set, culls possible points</param>
        /// <returns>An enumerable computed on-demand (lazily) of coordinates at the fixed radius.</returns>
        public static IEnumerable<Coordinate2<double>> GetManhattan(Coordinate2<double> centroid, double radius, double epsilon, double minX, double minY, double maxX, double maxY)
        {
            double eps;
            if (double.IsInfinity(epsilon) || double.IsNaN(epsilon))
                eps = 0.1d;
            else
                eps = Math.Abs(epsilon);
            if (radius == 0.0d || double.IsInfinity(radius) || double.IsNaN(radius))
                return null;
            double rad = Math.Abs(radius);

            List<Coordinate2<double>> cells = new List<Coordinate2<double>>();
            if (rad < eps)
                return cells;

            bool check = false;
            double xmin = centroid.X - rad;
            double xmax = centroid.X + rad;
            double ymax = centroid.Y + rad;
            double ymin = centroid.Y - rad;
            if (minX > xmin)
            {
                xmin = minX;
                check = true;
            }
            if (maxX < xmax)
            {
                xmax = maxX;
                check = true;
            }
            if (maxY < ymax)
            {
                ymax = maxY;
                check = true;
            }
            if (minY > ymin)
            {
                ymin = minY;
                check = true;
            }

            double curX;
            double curXm;
            double curYm;
            double curYp;
            double yPic = eps;

            //Compute all paired values
            curX = centroid.X + rad;
            curXm = centroid.X - rad;
            double dist = centroid.Y - rad;
            double dTmp;

            if (check)
            {
                #region checked
                if (xmin == centroid.X - rad)
                    cells.Add(new Coordinate2<double>(xmin, centroid.Y));
                if (xmax == centroid.X + rad)
                    cells.Add(new Coordinate2<double>(xmax, centroid.Y));
                if (ymax == centroid.Y + rad)
                    cells.Add(new Coordinate2<double>(centroid.X, ymin));
                if (ymin == centroid.Y - rad)
                    cells.Add(new Coordinate2<double>(centroid.X, ymax));
                while (curX > centroid.X)
                {
                    curYm = centroid.Y - yPic;
                    curYp = centroid.Y + yPic;
                    while (curYp <= ymax || curYm >= ymin)
                    {
                        dist = CartesianCalculator.ManhattanDistance(centroid.X, centroid.Y, curX, curYp);
                        dTmp = dist - rad;
                        if (Math.Abs(dTmp) < eps)
                        {
                            if (curYm >= ymin)
                            {
                                if (curX <= xmax)
                                    cells.Add(new Coordinate2<double>(curX, curYm));
                                if (curXm >= xmin)
                                    cells.Add(new Coordinate2<double>(curXm, curYm));
                            }
                            if (curYp <= ymax)
                            {
                                if (curX <= xmax)
                                    cells.Add(new Coordinate2<double>(curX, curYp));
                                if (curXm >= xmin)
                                    cells.Add(new Coordinate2<double>(curXm, curYp));
                            }
                        }
                        else if (dTmp < 0.0d)
                            yPic++;
                        else
                            break;
                        curYp++;
                        curYm--;
                    }
                    curX--;
                    curXm++;
                }
                #endregion
            }
            else
            {
                #region unchecked
                cells.Add(new Coordinate2<double>(xmin, centroid.Y));
                cells.Add(new Coordinate2<double>(xmax, centroid.Y));
                cells.Add(new Coordinate2<double>(centroid.X, ymin));
                cells.Add(new Coordinate2<double>(centroid.X, ymax));
                while (curX > centroid.X)
                {
                    curYm = centroid.Y - yPic;
                    curYp = centroid.Y + yPic;
                    while (curYp <= ymax)
                    {
                        dist = CartesianCalculator.ManhattanDistance(centroid.X, centroid.Y, curX, curYp);
                        dTmp = dist - rad;
                        if (Math.Abs(dTmp) < eps)
                        {
                            cells.Add(new Coordinate2<double>(curX, curYm));
                            cells.Add(new Coordinate2<double>(curXm, curYm));
                            cells.Add(new Coordinate2<double>(curX, curYp));
                            cells.Add(new Coordinate2<double>(curXm, curYp));
                        }
                        else if (dTmp < 0.0d)
                            yPic++;
                        else
                            break;
                        curYp++;
                        curYm--;
                    }
                    curX--;
                    curXm++;
                }
                #endregion
            }

            return cells;
        }

        public sealed class RadialIntEnumerable : IEnumerable<Coordinate2<int>>
        {
            private readonly Coordinate2<int> centroid;
            private readonly int radius;
            private readonly int minX;
            private readonly int minY;
            private readonly int maxX;
            private readonly int maxY;
            public RadialIntEnumerable(Coordinate2<int> centroid, int radius, int minX, int minY, int maxX, int maxY)
            {
                if (radius == 0)
                    this.radius = 1;
                else
                    this.radius = Math.Abs(radius);
                this.centroid = centroid;

                this.minX = minX;
                this.minY = minY;
                this.maxX = maxX;
                this.maxY = maxY;
            }

            #region IEnumerable<Coordinate2<int>> Members
            public IEnumerator<Coordinate2<int>> GetEnumerator()
            {
                return new RadialIntEnumerator(centroid, radius, minX, minY, maxX, maxY);
            }
            #endregion

            #region IEnumerable Members
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion

            public sealed class RadialIntEnumerator : IEnumerator<Coordinate2<int>>
            {
                private Coordinate2<int> current;
                private readonly Coordinate2<int> centroid;
                private readonly int centerX;
                private readonly int centerY;
                private readonly int minX;
                private readonly int minY;
                private readonly int maxX;
                private readonly int maxY;
                private readonly int radius;
                private bool check = false;
                private readonly List<Coordinate2<int>> ordinals = new List<Coordinate2<int>>();
                private bool ordinalsComplete = false;
                private int curCt = 0;
                private int limXP = int.MaxValue;
                private int limXM = int.MinValue;
                private int limYP = int.MaxValue;
                private int limYM = int.MinValue;

                private int yPic = 1; //start y offset from center
                private int offX;
                private int offY = 1;
                private int curYP;
                private int curXP;

                private int calcX;
                private int calcY;

                internal RadialIntEnumerator(Coordinate2<int> centroid, int radius, int minX, int minY, int maxX, int maxY)
                {
                    if (radius == 0)
                        this.radius = 1;
                    else
                        this.radius = Math.Abs(radius);
                    this.centroid = centroid;
                    this.centerX = centroid.X;
                    this.centerY = centroid.Y;

                    this.minX = centerX - radius;
                    this.minY = centerY - radius;
                    this.maxX = centerX + radius;
                    this.maxY = centerY + radius;

                    this.limXP = maxX;
                    this.limXM = minX;
                    this.limYP = maxY;
                    this.limYM = minY;

                    if (this.limXM > this.minX)
                        this.check = true;
                    if (this.limXP < this.maxX)
                        this.check = true;
                    if (this.limYP < this.maxY)
                        this.check = true;
                    if (this.limYM > this.minY)
                        this.check = true;

                    this.Reset();
                }

                private void GetOrdinals()
                {
                    this.ordinals.Clear();
                    this.curCt = 0;
                    if (this.check)
                    {
                        #region checked
                        if (this.maxY <= this.limYP)
                            this.ordinals.Add(new Coordinate2<int>(this.centerX, this.maxY));
                        if (this.maxX <= this.limXP)
                            this.ordinals.Add(new Coordinate2<int>(this.maxX, this.centerY));
                        if (this.minY >= this.limYM)
                            this.ordinals.Add(new Coordinate2<int>(this.centerX, this.minY));
                        if (this.minX >= this.limXM)
                            this.ordinals.Add(new Coordinate2<int>(this.minX, this.centerY));
                        #endregion
                    }
                    else
                    {
                        #region unchecked
                        this.ordinals.Add(new Coordinate2<int>(centerX, maxY));
                        this.ordinals.Add(new Coordinate2<int>(maxX, centerY));
                        this.ordinals.Add(new Coordinate2<int>(centerX, minY));
                        this.ordinals.Add(new Coordinate2<int>(minX, centerY));
                        #endregion
                    }
                }

                public bool NextNonOrdinal()
                {
                    int ct = this.ordinals.Count;
                    if (ct > 0)
                    {
                        if (this.curCt < ct)
                        {
                            this.current = this.ordinals[this.curCt];
                            this.curCt++;
                            return true;
                        }
                        this.ordinals.Clear(); //we've already provided these
                    }
                    if (this.check)
                    {
                        #region checked
                        int dist;
                        if (curXP > centerX)
                        {
                            dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                            if (dist == radius)
                            {
                                calcX = centerX - offX;
                                calcY = centerY - offY;
                                if (curXP <= limXP)
                                {
                                    if (curYP <= limYP)
                                        this.ordinals.Add(new Coordinate2<int>(curXP, curYP));
                                    if (calcY >= limYM)
                                        this.ordinals.Add(new Coordinate2<int>(curXP, calcY));
                                }
                                if (calcX >= limXM)
                                {
                                    if (curYP <= limYP)
                                        this.ordinals.Add(new Coordinate2<int>(calcX, curYP));
                                    if (calcY >= limYM)
                                        this.ordinals.Add(new Coordinate2<int>(calcX, calcY));
                                }
                                curYP++;
                                offY++;
                            }
                            else if (dist > radius) //move in, past top then spin up y
                            {
                                offY = yPic;
                                curYP = centerY + yPic;
                                curXP--;
                                offX--;
                                dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                while (dist < radius)
                                {
                                    curYP++;
                                    offY++;
                                    yPic++;
                                    dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                }
                            }
                            if (this.ordinals.Count < 1)
                                return this.NextNonOrdinal();
                            else
                            {
                                this.current = this.ordinals[0];
                                this.curCt = 1;
                                return true;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region unchecked
                        int dist;
                        if (curXP > centerX)
                        {
                            dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                            if (dist == radius)
                            {
                                calcX = centerX - offX;
                                calcY = centerY - offY;
                                this.ordinals.Add(new Coordinate2<int>(curXP, curYP));
                                this.ordinals.Add(new Coordinate2<int>(curXP, calcY));
                                this.ordinals.Add(new Coordinate2<int>(calcX, curYP));
                                this.ordinals.Add(new Coordinate2<int>(calcX, calcY));
                                curYP++;
                                offY++;
                            }
                            else if (dist > radius) //move in, past top then spin up y
                            {
                                offY = yPic;
                                curYP = centerY + yPic;
                                curXP--;
                                offX--;
                                dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                while (dist < radius)
                                {
                                    curYP++;
                                    offY++;
                                    yPic++;
                                    dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                }
                            }
                            if (this.ordinals.Count < 1)
                                return this.NextNonOrdinal();
                            else
                            {
                                this.current = this.ordinals[0];
                                this.curCt = 1;
                                return true;
                            }
                        }
                        #endregion
                    }
                    return false;//back to middle and already done ordinal directions
                }

                #region IEnumerator<Coordinate2<int>> Members
                public Coordinate2<int> Current
                {
                    get { return this.current; }
                }
                #endregion

                #region IDisposable Members
                public void Dispose()
                {
                    return;
                }
                #endregion

                #region IEnumerator Members
                object System.Collections.IEnumerator.Current
                {
                    get { return this.current; }
                }

                public bool MoveNext()
                {
                    if (this.ordinalsComplete)
                        return this.NextNonOrdinal();

                    if (this.curCt == 0)
                    {
                        this.GetOrdinals();
                        int ct = this.ordinals.Count;
                        if (ct > 0)
                        {
                            if (this.curCt < ct)
                            {
                                this.current = this.ordinals[this.curCt];
                                this.curCt++;
                                return true;
                            }
                            this.ordinals.Clear();
                        }
                    }
                    else
                    {
                        if (this.curCt < this.ordinals.Count)
                        {
                            this.current = this.ordinals[this.curCt];
                            this.curCt++;
                            return true;
                        }
                        this.ordinals.Clear();
                    }
                    this.ordinalsComplete = true;
                    return this.NextNonOrdinal();
                }

                public void Reset()
                {
                    this.yPic = 1; //start y offset from center
                    this.offX = radius;
                    this.offY = 1;
                    this.curYP = centerY + yPic;
                    this.curXP = centerX + radius;
                    this.curCt = 0;
                    this.ordinals.Clear();
                    this.ordinalsComplete = false;
                }
                #endregion
            }
        }

        public sealed class RadialLongEnumerable : IEnumerable<Coordinate2<long>>
        {
            private readonly Coordinate2<long> centroid;
            private readonly long radius;
            private readonly long minX;
            private readonly long minY;
            private readonly long maxX;
            private readonly long maxY;
            public RadialLongEnumerable(Coordinate2<long> centroid, long radius, long minX, long minY, long maxX, long maxY)
            {
                if (radius == 0)
                    this.radius = 1;
                else
                    this.radius = Math.Abs(radius);
                this.centroid = centroid;

                this.minX = minX;
                this.minY = minY;
                this.maxX = maxX;
                this.maxY = maxY;
            }

            #region IEnumerable<Coordinate2<long>> Members
            public IEnumerator<Coordinate2<long>> GetEnumerator()
            {
                return new RadialLongEnumerator(centroid, radius, minX, minY, maxX, maxY);
            }
            #endregion

            #region IEnumerable Members
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion

            public sealed class RadialLongEnumerator : IEnumerator<Coordinate2<long>>
            {
                private Coordinate2<long> current;
                private readonly Coordinate2<long> centroid;
                private readonly long centerX;
                private readonly long centerY;
                private readonly long minX;
                private readonly long minY;
                private readonly long maxX;
                private readonly long maxY;
                private readonly long radius;
                private bool check = false;
                private List<Coordinate2<long>> ordinals = new List<Coordinate2<long>>();
                private bool ordinalsComplete = false;
                private int curCt = 0;
                private long limXP = int.MaxValue;
                private long limXM = int.MinValue;
                private long limYP = int.MaxValue;
                private long limYM = int.MinValue;

                private long yPic = 1; //start y offset from center
                private long offX;
                private long offY = 1;
                private long curYP;
                private long curXP;

                private long calcX;
                private long calcY;

                internal RadialLongEnumerator(Coordinate2<long> centroid, long radius, long minX, long minY, long maxX, long maxY)
                {
                    if (radius == 0)
                        this.radius = 1;
                    else
                        this.radius = Math.Abs(radius);
                    this.centroid = centroid;
                    this.centerX = centroid.X;
                    this.centerY = centroid.Y;

                    this.minX = centerX - radius;
                    this.minY = centerY - radius;
                    this.maxX = centerX + radius;
                    this.maxY = centerY + radius;

                    this.limXP = maxX;
                    this.limXM = minX;
                    this.limYP = maxY;
                    this.limYM = minY;

                    if (this.limXM > this.minX)
                        this.check = true;
                    if (this.limXP < this.maxX)
                        this.check = true;
                    if (this.limYP < this.maxY)
                        this.check = true;
                    if (this.limYM > this.minY)
                        this.check = true;

                    this.Reset();
                }

                private void GetOrdinals()
                {
                    this.ordinals.Clear();
                    this.curCt = 0;
                    if (this.check)
                    {
                        #region checked
                        if (this.maxY <= this.limYP)
                            this.ordinals.Add(new Coordinate2<long>(this.centerX, this.maxY));
                        if (this.maxX <= this.limXP)
                            this.ordinals.Add(new Coordinate2<long>(this.maxX, this.centerY));
                        if (this.minY >= this.limYM)
                            this.ordinals.Add(new Coordinate2<long>(this.centerX, this.minY));
                        if (this.minX >= this.limXM)
                            this.ordinals.Add(new Coordinate2<long>(this.minX, this.centerY));
                        #endregion
                    }
                    else
                    {
                        #region unchecked
                        this.ordinals.Add(new Coordinate2<long>(centerX, maxY));
                        this.ordinals.Add(new Coordinate2<long>(maxX, centerY));
                        this.ordinals.Add(new Coordinate2<long>(centerX, minY));
                        this.ordinals.Add(new Coordinate2<long>(minX, centerY));
                        #endregion
                    }
                }

                public bool NextNonOrdinal()
                {
                    int ct = this.ordinals.Count;
                    if (ct > 0)
                    {
                        if (this.curCt < ct)
                        {
                            this.current = this.ordinals[this.curCt];
                            this.curCt++;
                            return true;
                        }
                        this.ordinals.Clear(); //we've already provided these
                    }
                    if (this.check)
                    {
                        #region checked
                        long dist;
                        if (curXP > centerX)
                        {
                            dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                            if (dist == radius)
                            {
                                calcX = centerX - offX;
                                calcY = centerY - offY;
                                if (curXP <= limXP)
                                {
                                    if (curYP <= limYP)
                                        this.ordinals.Add(new Coordinate2<long>(curXP, curYP));
                                    if (calcY >= limYM)
                                        this.ordinals.Add(new Coordinate2<long>(curXP, calcY));
                                }
                                if (calcX >= limXM)
                                {
                                    if (curYP <= limYP)
                                        this.ordinals.Add(new Coordinate2<long>(calcX, curYP));
                                    if (calcY >= limYM)
                                        this.ordinals.Add(new Coordinate2<long>(calcX, calcY));
                                }
                                curYP++;
                                offY++;
                            }
                            else if (dist > radius) //move in, past top then spin up y
                            {
                                offY = yPic;
                                curYP = centerY + yPic;
                                curXP--;
                                offX--;
                                dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                while (dist < radius)
                                {
                                    curYP++;
                                    offY++;
                                    yPic++;
                                    dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                }
                            }
                            if (this.ordinals.Count < 1)
                                return this.NextNonOrdinal();
                            else
                            {
                                this.current = this.ordinals[0];
                                this.curCt = 1;
                                return true;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region unchecked
                        long dist;
                        if (curXP > centerX)
                        {
                            dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                            if (dist == radius)
                            {
                                calcX = centerX - offX;
                                calcY = centerY - offY;
                                this.ordinals.Add(new Coordinate2<long>(curXP, curYP));
                                this.ordinals.Add(new Coordinate2<long>(curXP, calcY));
                                this.ordinals.Add(new Coordinate2<long>(calcX, curYP));
                                this.ordinals.Add(new Coordinate2<long>(calcX, calcY));
                                curYP++;
                                offY++;
                            }
                            else if (dist > radius) //move in, past top then spin up y
                            {
                                offY = yPic;
                                curYP = centerY + yPic;
                                curXP--;
                                offX--;
                                dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                while (dist < radius)
                                {
                                    curYP++;
                                    offY++;
                                    yPic++;
                                    dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                }
                            }
                            if (this.ordinals.Count < 1)
                                return this.NextNonOrdinal();
                            else
                            {
                                this.current = this.ordinals[0];
                                this.curCt = 1;
                                return true;
                            }
                        }
                        #endregion
                    }
                    return false;//back to middle and already done ordinal directions
                }

                #region IEnumerator<Coordinate2<long>> Members
                public Coordinate2<long> Current
                {
                    get { return this.current; }
                }
                #endregion

                #region IDisposable Members
                public void Dispose()
                {
                    return;
                }
                #endregion

                #region IEnumerator Members
                object System.Collections.IEnumerator.Current
                {
                    get { return this.current; }
                }

                public bool MoveNext()
                {
                    if (this.ordinalsComplete)
                        return this.NextNonOrdinal();

                    if (this.curCt == 0)
                    {
                        this.GetOrdinals();
                        int ct = this.ordinals.Count;
                        if (ct > 0)
                        {
                            if (this.curCt < ct)
                            {
                                this.current = this.ordinals[this.curCt];
                                this.curCt++;
                                return true;
                            }
                            this.ordinals.Clear();
                        }
                    }
                    else
                    {
                        if (this.curCt < this.ordinals.Count)
                        {
                            this.current = this.ordinals[this.curCt];
                            this.curCt++;
                            return true;
                        }
                        this.ordinals.Clear();
                    }
                    this.ordinalsComplete = true;
                    return this.NextNonOrdinal();
                }

                public void Reset()
                {
                    this.yPic = 1; //start y offset from center
                    this.offX = radius;
                    this.offY = 1;
                    this.curYP = centerY + yPic;
                    this.curXP = centerX + radius;
                    this.curCt = 0;
                    this.ordinals.Clear();
                    this.ordinalsComplete = false;
                }
                #endregion
            }
        }

        public sealed class RadialDoubleEnumerable : IEnumerable<Coordinate2<double>>
        {
            private readonly Coordinate2<double> centroid;
            private readonly double radius;
            private readonly double minX;
            private readonly double minY;
            private readonly double maxX;
            private readonly double maxY;
            private readonly double eps;
            public RadialDoubleEnumerable(Coordinate2<double> centroid, double radius, double epsilon, double minX, double minY, double maxX, double maxY)
            {
                if (double.IsInfinity(epsilon) || double.IsNaN(epsilon))
                    this.eps = 0.1d;
                else
                    this.eps = Math.Abs(epsilon);
                if (radius == 0.0d || double.IsInfinity(radius) || double.IsNaN(radius))
                    this.radius = 1.0d;
                else
                    this.radius = Math.Abs(radius);
                this.centroid = centroid;

                this.minX = minX;
                this.minY = minY;
                this.maxX = maxX;
                this.maxY = maxY;
            }

            #region IEnumerable<Coordinate2<double>> Members
            public IEnumerator<Coordinate2<double>> GetEnumerator()
            {
                return new RadialDoubleEnumerator(centroid, radius, this.eps, minX, minY, maxX, maxY);
            }
            #endregion

            #region IEnumerable Members
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion

            public sealed class RadialDoubleEnumerator : IEnumerator<Coordinate2<double>>
            {
                private Coordinate2<double> current;
                private readonly Coordinate2<double> centroid;
                private readonly double centerX;
                private readonly double centerY;
                private readonly double minX;
                private readonly double minY;
                private readonly double maxX;
                private readonly double maxY;
                private readonly double radius;
                private bool check = false;
                private int curCt = 0;
                private double limXP = double.MaxValue;
                private double limXM = double.MinValue;
                private double limYP = double.MaxValue;
                private double limYM = double.MinValue;

                private double yPic = 1; //start y offset from center
                private double offX;
                private double offY = 1;
                private double curYP;
                private double curXP;

                private double calcX;
                private double calcY;
                private double eps;
                private List<Coordinate2<double>> ordinals = new List<Coordinate2<double>>();
                private bool ordinalsComplete = false;

                internal RadialDoubleEnumerator(Coordinate2<double> centroid, double radius, double epsilon, double minX, double minY, double maxX, double maxY)
                {
                    if (radius == 0)
                        this.radius = 1;
                    else
                        this.radius = Math.Abs(radius);
                    this.centroid = centroid;
                    this.eps = epsilon;

                    this.centroid = centroid;
                    this.centerX = centroid.X;
                    this.centerY = centroid.Y;

                    this.minX = centerX - radius;
                    this.minY = centerY - radius;
                    this.maxX = centerX + radius;
                    this.maxY = centerY + radius;

                    this.limXP = maxX;
                    this.limXM = minX;
                    this.limYP = maxY;
                    this.limYM = minY;

                    if (this.limXM > this.minX)
                        this.check = true;
                    if (this.limXP < this.maxX)
                        this.check = true;
                    if (this.limYP < this.maxY)
                        this.check = true;
                    if (this.limYM > this.minY)
                        this.check = true;

                    this.Reset();
                }

                private void GetOrdinals()
                {
                    this.ordinals.Clear();
                    this.curCt = 0;
                    if (this.check)
                    {
                        #region checked
                        if (this.maxY <= this.limYP)
                            this.ordinals.Add(new Coordinate2<double>(this.centerX, this.maxY));
                        if (this.maxX <= this.limXP)
                            this.ordinals.Add(new Coordinate2<double>(this.maxX, this.centerY));
                        if (this.minY >= this.limYM)
                            this.ordinals.Add(new Coordinate2<double>(this.centerX, this.minY));
                        if (this.minX >= this.limXM)
                            this.ordinals.Add(new Coordinate2<double>(this.minX, this.centerY));
                        #endregion
                    }
                    else
                    {
                        #region unchecked
                        this.ordinals.Add(new Coordinate2<double>(centerX, maxY));
                        this.ordinals.Add(new Coordinate2<double>(maxX, centerY));
                        this.ordinals.Add(new Coordinate2<double>(centerX, minY));
                        this.ordinals.Add(new Coordinate2<double>(minX, centerY));
                        #endregion
                    }
                }

                public bool NextNonOrdinal()
                {
                    int ct = this.ordinals.Count;
                    if (ct > 0)
                    {
                        if (this.curCt < ct)
                        {
                            this.current = this.ordinals[this.curCt];
                            this.curCt++;
                            return true;
                        }
                        this.ordinals.Clear(); //we've already provided these
                    }
                    if (this.check)
                    {
                        #region checked
                        double dist;
                        if (curXP > centerX)
                        {
                            dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                            if (this.eps >= radius - dist)
                            {
                                calcX = centerX - offX;
                                calcY = centerY - offY;
                                if (curXP <= limXP)
                                {
                                    if (curYP <= limYP)
                                        this.ordinals.Add(new Coordinate2<double>(curXP, curYP));
                                    if (calcY >= limYM)
                                        this.ordinals.Add(new Coordinate2<double>(curXP, calcY));
                                }
                                if (calcX >= limXM)
                                {
                                    if (curYP <= limYP)
                                        this.ordinals.Add(new Coordinate2<double>(calcX, curYP));
                                    if (calcY >= limYM)
                                        this.ordinals.Add(new Coordinate2<double>(calcX, calcY));
                                }
                                curYP += this.eps;
                                offY += this.eps;
                            }
                            else if (dist > radius) //move in, past top then spin up y
                            {
                                offY = yPic;
                                curYP = centerY + yPic;
                                curXP -= this.eps;
                                offX -= this.eps;
                                dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                while (this.eps < radius - dist)
                                {
                                    curYP += this.eps;
                                    offY += this.eps;
                                    yPic += this.eps;
                                    dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                }
                            }
                            if (this.ordinals.Count < 1)
                                return this.NextNonOrdinal();
                            else
                            {
                                this.current = this.ordinals[0];
                                this.curCt = 1;
                                return true;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region unchecked
                        double dist;
                        if (curXP > centerX)
                        {
                            dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                            if (this.eps >= radius - dist)
                            {
                                calcX = centerX - offX;
                                calcY = centerY - offY;
                                this.ordinals.Add(new Coordinate2<double>(curXP, curYP));
                                this.ordinals.Add(new Coordinate2<double>(curXP, calcY));
                                this.ordinals.Add(new Coordinate2<double>(calcX, curYP));
                                this.ordinals.Add(new Coordinate2<double>(calcX, calcY));
                                curYP += this.eps;
                                offY += this.eps;
                            }
                            else if (dist > radius) //move in, past top then spin up y
                            {
                                offY = yPic;
                                curYP = centerY + yPic;
                                curXP -= this.eps;
                                offX -= this.eps;
                                dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                while (this.eps < radius - dist)
                                {
                                    curYP += this.eps;
                                    offY += this.eps;
                                    yPic += this.eps;
                                    dist = CartesianCalculator.Distance(centerX, centerY, curXP, curYP);
                                }
                            }
                            if (this.ordinals.Count < 1)
                                return this.NextNonOrdinal();
                            else
                            {
                                this.current = this.ordinals[0];
                                this.curCt = 1;
                                return true;
                            }
                        }
                        #endregion
                    }
                    return false;//back to middle and already done ordinal directions
                }

                #region IEnumerator<Coordinate2<double>> Members
                public Coordinate2<double> Current
                {
                    get { return this.current; }
                }
                #endregion

                #region IDisposable Members
                public void Dispose()
                {
                    return;
                }
                #endregion

                #region IEnumerator Members
                object System.Collections.IEnumerator.Current
                {
                    get { return this.current; }
                }

                public bool MoveNext()
                {
                    if (this.ordinalsComplete)
                        return this.NextNonOrdinal();

                    if (this.curCt == 0)
                        this.GetOrdinals();
                    int ct = this.ordinals.Count;
                    if (ct > 0)
                    {
                        if (this.curCt < ct)
                        {
                            this.current = this.ordinals[this.curCt];
                            this.curCt++;
                            return true;
                        }
                        this.ordinals.Clear();
                    }
                    this.ordinalsComplete = true;
                    return this.NextNonOrdinal();
                }

                public void Reset()
                {
                    this.yPic = this.eps; //start y offset from center
                    this.offX = radius;
                    this.offY = this.eps;
                    this.curYP = centerY + yPic;
                    this.curXP = centerX + radius;
                    this.curCt = 0;
                    this.ordinals.Clear();
                    this.ordinalsComplete = false;
                }
                #endregion
            }
        }
    }
}
