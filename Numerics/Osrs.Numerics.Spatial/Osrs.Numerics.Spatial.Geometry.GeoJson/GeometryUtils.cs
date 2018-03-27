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

namespace Osrs.Numerics.Spatial.Geometry
{
    internal static class GeometryUtils
    {
        private static GeometryFactory2Double factory = GeometryFactory2Double.Instance;

        public static Point2<double> BuildPoint(Position p)
        {
            int num1;
            if (p != null)
            {
                double num2 = double.NaN;
                if (!num2.Equals(p.X))
                {
                    num2 = double.NaN;
                    num1 = num2.Equals(p.Y) ? 1 : 0;
                }
                else
                    num1 = 1;
            }
            else
                num1 = 1;
            if (num1 == 0)
                return factory.ConstructPoint(p.X, p.Y);
            return null;
        }

        public static PointBag2<double> BuildMultiPoint(Position p)
        {
            return BuildMultiPoint(p as PositionSet);
        }

        public static PointBag2<double> BuildMultiPoint(PositionSet ps)
        {
            try
            {
                if (ps != null)
                {
                    List<Point2<double>> list = BuildCoords(ps);
                    if (list != null && list.Count > 1)
                        return factory.ConstructPointBag((IEnumerable<Point2<double>>)list);
                }
            }
            catch
            {
            }
            return null;
        }

        public static Polyline2<double> BuildLineString(Position p)
        {
            return BuildLineString(p as PositionSet);
        }

        public static Polyline2<double> BuildLineString(PositionSet ps)
        {
            try
            {
                if (ps != null)
                {
                    List<Point2<double>> list = BuildCoords(ps);
                    if (list != null && list.Count > 1)
                        return factory.ConstructPolyline(list);
                }
            }
            catch
            {
            }
            return null;
        }

        public static PolylineBag2<double> BuildMultiLineString(Position p)
        {
            return BuildMultiLineString(p as PositionSet);
        }

        public static PolylineBag2<double> BuildMultiLineString(PositionSet ps)
        {
            try
            {
                if (ps != null)
                {
                    if (ps.Positions.Count > 0)
                    {
                        List<Polyline2<double>> list = new List<Polyline2<double>>();
                        foreach (Position position in ps.Positions)
                        {
                            Polyline2<double> lineString = BuildLineString(position as PositionSet);
                            if (lineString == null)
                                return null;
                            list.Add(lineString);
                        }
                        if (list.Count > 0)
                            return factory.ConstructPolylineBag(list);
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public static Polygon2<double> BuildPolygon(Position p)
        {
            return BuildPolygon(p as PositionSet);
        }

        public static Polygon2<double> BuildPolygon(PositionSet ps)
        {
            try
            {
                if (ps != null && ps.Positions.Count > 0)
                {
                    List<Point2<double>> list1 = BuildCoords(ps.Positions[0] as PositionSet);
                    if (list1 != null && list1.Count > 2)
                    {
                        if (ps.Positions.Count == 1)
                            return factory.ConstructPolygon(factory.ConstructRing(list1));
                        Ring2<double> linearRing1 = factory.ConstructRing(list1);
                        List<Ring2<double>> list2 = new List<Ring2<double>>();
                        for (int index = 1; index < ps.Positions.Count; ++index)
                        {
                            Position position = ps.Positions[index];
                            if (position != null)
                            {
                                List<Point2<double>> list3 = BuildCoords(position as PositionSet);
                                if (list3 != null)
                                {
                                    Ring2<double> linearRing2 = factory.ConstructRing(list3);
                                    list2.Add(linearRing2);
                                }
                            }
                        }
                        return factory.ConstructPolygon(linearRing1, list2.ToArray());
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public static PolygonBag2<double> BuildMultiPolygon(Position p)
        {
            return BuildMultiPolygon(p as PositionSet);
        }

        public static PolygonBag2<double> BuildMultiPolygon(PositionSet ps)
        {
            if (ps != null && ps.Positions.Count > 0)
            {
                List<Polygon2<double>> list = new List<Polygon2<double>>();
                foreach (Position position in ps.Positions)
                {
                    Polygon2<double> polygon = BuildPolygon(position as PositionSet);
                    if (polygon == null)
                        return null;
                    list.Add(polygon);
                }
                if (list.Count > 0)
                    return factory.ConstructPolygonBag(list);
            }
            return null;
        }

        public static Geometry2Bag<double> BuildGeometryCollection(Position p)
        {
            return BuildGeometryCollection(p as PositionSet);
        }

        public static Geometry2Bag<double> BuildGeometryCollection(PositionSet ps)
        {
            if (ps != null && (ps.GeometryHint == PositionType.GeometryCollection || ps.GeometryHint == PositionType.Unknown))
            {
                List<IGeometry2<double>> list = new List<IGeometry2<double>>();
                foreach (Position p in ps.Positions)
                {
                    IGeometry2<double> geometry = null;
                    if (p.GeometryHint == PositionType.Point)
                        geometry = BuildPoint(p);
                    else if (p.GeometryHint == PositionType.MultiPoint)
                        geometry = BuildMultiPoint(p as PositionSet);
                    else if (p.GeometryHint == PositionType.LineString)
                        geometry = BuildLineString(p as PositionSet);
                    else if (p.GeometryHint == PositionType.MultiLineString)
                        geometry = BuildMultiLineString(p as PositionSet);
                    else if (p.GeometryHint == PositionType.Polygon)
                        geometry = BuildPolygon(p as PositionSet);
                    else if (p.GeometryHint == PositionType.MultiPolygon)
                        geometry = BuildMultiPolygon(p as PositionSet);
                    if (geometry == null)
                        return null;
                    list.Add(geometry);
                }
                if (list.Count > 0)
                    return factory.ConstructGeometryBag(list);
            }
            return null;
        }

        public static List<Point2<double>> BuildCoords(Position p)
        {
            return BuildCoords(p as PositionSet);
        }

        public static List<Point2<double>> BuildCoords(PositionSet ps)
        {
            if (ps == null)
                return null;
            List<Point2<double>> list = new List<Point2<double>>();
            foreach (Position position in ps.Positions)
            {
                double num1 = double.NaN;
                int num2;
                if (!num1.Equals(position.X))
                {
                    num1 = double.NaN;
                    num2 = !num1.Equals(position.Y) ? 1 : 0;
                }
                else
                    num2 = 0;
                if (num2 == 0)
                    return null;
                list.Add(factory.ConstructPoint(position.X, position.Y));
            }
            return list;
        }

        public static Point2<double> BuildCoord(Position p)
        {
            int num1;
            if (p != null)
            {
                double num2 = double.NaN;
                if (!num2.Equals(p.X))
                {
                    num2 = double.NaN;
                    num1 = num2.Equals(p.Y) ? 1 : 0;
                }
                else
                    num1 = 1;
            }
            else
                num1 = 1;
            if (num1 == 0)
                return factory.ConstructPoint(p.X, p.Y);
            return null;
        }
    }
}
