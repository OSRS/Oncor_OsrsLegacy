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

using NpgsqlTypes;
using Osrs.Numerics.Spatial.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial.Postgres
{
    public static class NpgSpatialUtils
    {
        private static GeometryFactory2Base<double> factory = GeometryFactory2Double.Instance;

        public static Point2<double> ToGeom(NpgsqlPoint geom)
        {
            return factory.ConstructPoint(geom.X, geom.Y);
        }

        public static IGeometry2<double> ToGeom(NpgsqlTypes.PostgisGeometry geom)
        {
            if (geom!=null)
            {
                if (geom is PostgisPoint)
                    return ToGeom(geom as PostgisPoint);
                else if (geom is PostgisPolygon)
                    return ToGeom(geom as PostgisPolygon);
                else if (geom is PostgisMultiPolygon)
                    return ToGeom(geom as PostgisMultiPolygon);
                else if (geom is PostgisLineString)
                    return ToGeom(geom as PostgisLineString);
                else if (geom is PostgisMultiLineString)
                    return ToGeom(geom as PostgisMultiLineString);
                else if (geom is PostgisMultiPoint)
                    return ToGeom(geom as PostgisMultiPoint);
            }
            return null;
        }

        public static Point2<double> ToGeom(PostgisPoint geom)
        {
            if (geom!=null)
            {
                return factory.ConstructPoint(geom.X, geom.Y);
            }
            return null;
        }

        public static Polyline2<double> ToGeom(PostgisLineString geom)
        {
            if (geom != null)
            {
                return factory.ConstructPolyline(Points(geom));
            }
            return null;
        }

        public static PolylineBag2<double> ToGeom(PostgisMultiLineString geom)
        {
            if (geom != null)
            {
                List<List<Point2<double>>> pts = Points(geom);
                List<Polyline2<double>> lines = new List<Polyline2<double>>();
                foreach(List<Point2<double>> cur in pts)
                {
                    lines.Add(factory.ConstructPolyline(cur));
                }
                return factory.ConstructPolylineBag(lines);
            }
            return null;
        }

        public static PointBag2<double> ToGeom(PostgisMultiPoint geom)
        {
            if (geom != null)
            {
                return factory.ConstructPointBag(Points(geom));
            }
            return null;
        }

        public static PolygonBag2<double> ToGeom(PostgisMultiPolygon geom)
        {
            if (geom != null)
            {
                List<List<List<Point2<double>>>> pts = Points(geom);
                List<Polygon2<double>> polys = new List<Polygon2<double>>();
                foreach(List<List<Point2<double>>> cur in pts)
                {
                    polys.Add(ToGeom(cur));
                }
                return factory.ConstructPolygonBag(polys);
            }
            return null;
        }

        private static Polygon2<double> ToGeom(List<List<Point2<double>>> pts)
        {
            if (pts.Count > 1) //outer and inner
            {
                List<Point2<double>> outer = pts[0];
                if (outer[0].Equals(outer[outer.Count - 1]))
                    outer.RemoveAt(outer.Count - 1);

                List<Ring2<double>> inner = new List<Ring2<double>>();
                for (int i = 1; i < pts.Count; i++)
                {
                    if (pts[i][0].Equals(pts[i][pts[i].Count - 1]))
                        pts[i].RemoveAt(pts[i].Count - 1);
                    inner.Add(factory.ConstructRing(pts[i]));
                }
                pts = null;
                return factory.ConstructPolygon(factory.ConstructRing(outer), factory.ConstructRingSet(inner));
            }
            else
                return factory.ConstructPolygon(factory.ConstructRing(pts[0]));
        }

        public static Polygon2<double> ToGeom(PostgisPolygon geom)
        {
            if (geom != null)
            {
                return ToGeom(Points(geom));
            }
            return null;
        }

        public static List<Point2<double>> Points(IEnumerable<Coordinate2D> geom)
        {
            if (geom!=null)
            {
                List<Point2<double>> tmp = new List<Point2<double>>();
                foreach(Coordinate2D cur in geom)
                {
                    tmp.Add(factory.ConstructPoint(cur.X, cur.Y));
                }
                return tmp;
            }
            return null;
        }

        public static List<List<Point2<double>>> Points(IEnumerable<IEnumerable<Coordinate2D>> geom)
        {
            if (geom!=null)
            {
                List<List<Point2<double>>> tmp = new List<List<Point2<double>>>();
                foreach(IEnumerable<Coordinate2D> cur in geom)
                {
                    tmp.Add(Points(cur));
                }
                return tmp;
            }
            return null;
        }

        public static List<List<List<Point2<double>>>> Points(IEnumerable<IEnumerable<IEnumerable<Coordinate2D>>> geom)
        {
            if (geom!=null)
            {
                List<List<List<Point2<double>>>> tmp = new List<List<List<Point2<double>>>>();
                foreach (IEnumerable<IEnumerable<Coordinate2D>> cur in geom)
                {
                    tmp.Add(Points(cur));
                }
                return tmp;
            }
            return null;
        }

        public static List<Coordinate2D> Points(IEnumerable<Point2<double>> geom)
        {
            if (geom!=null)
            {
                List<Coordinate2D> pts = new List<Coordinate2D>();
                foreach(Point2<double> cur in geom)
                {
                    pts.Add(new Coordinate2D(cur.X, cur.Y));
                }
                return pts;
            }
            return null;
        }

        public static List<List<Coordinate2D>> Points(IEnumerable<IEnumerable<Point2<double>>> geom)
        {
            if (geom!=null)
            {
                List<List<Coordinate2D>> tmp = new List<List<Coordinate2D>>();
                foreach(IEnumerable<Point2<double>> cur in geom)
                {
                    tmp.Add(Points(cur));
                }
                return tmp;
            }
            return null;
        }

        public static List<List<Coordinate2D>> Points(Polygon2<double> geom)
        {
            if (geom != null)
            {
                List<List<Coordinate2D>> tmp = new List<List<Coordinate2D>>();
                tmp.Add(Points((IEnumerable<Point2<double>>)geom.OuterRing));

                if (geom.HasHoles)
                {
                    foreach (IEnumerable<Point2<double>> cur in geom.InnerRings)
                    {
                        tmp.Add(Points(cur));
                    }
                }

                foreach(List<Coordinate2D> cur in tmp)
                {
                    cur.Add(cur[0]);
                }

                return tmp;
            }
            return null;
        }

        public static List<List<List<Coordinate2D>>> Points(IEnumerable<Polygon2<double>> geom)
        {
            if (geom!=null)
            {
                List<List<List<Coordinate2D>>> tmp = new List<List<List<Coordinate2D>>>();
                foreach (Polygon2<double> cur in geom)
                {
                    tmp.Add(Points(cur));
                }
                return tmp;
            }
            return null;
        }

        public static NpgsqlPoint ToNpg(Point2<double> geom)
        {
            if (geom != null)
                return new NpgsqlPoint(geom.X, geom.Y);
            return default(NpgsqlPoint);
        }

        public static PostgisGeometry ToPGis(IGeometry2<double> geom)
        {
            if (geom != null)
            {
                if (geom is Point2<double>)
                    return ToPGis(geom as Point2<double>);
                else if (geom is LineSegment2<double>)
                    return ToPGis(geom as LineSegment2<double>);
                else if (geom is Polygon2<double>)
                    return ToPGis(geom as Polygon2<double>);
                else if (geom is PolygonSet2<double>)
                    return ToPGis(geom as PolygonSet2<double>);
                else if (geom is PolygonBag2<double>)
                    return ToPGis(geom as PolygonBag2<double>);
                else if (geom is Ring2<double>)
                    return ToPGis(geom as Ring2<double>);
                else if (geom is LineChain2<double>)
                    return ToPGis(geom as LineChain2<double>);
                else if (geom is LineChainSet2<double>)
                    return ToPGis(geom as LineChainSet2<double>);
                else if (geom is LineChainBag2<double>)
                    return ToPGis(geom as LineChainBag2<double>);
                else if (geom is Polyline2<double>)
                    return ToPGis(geom as Polyline2<double>);
                else if (geom is PolylineSet2<double>)
                    return ToPGis(geom as PolylineSet2<double>);
                else if (geom is PolylineBag2<double>)
                    return ToPGis(geom as PolylineBag2<double>);
                else if (geom is PointSet2<double>)
                    return ToPGis(geom as PointSet2<double>);
                else if (geom is PointBag2<double>)
                    return ToPGis(geom as PointBag2<double>);
            }
            return null;
        }

        public static PostgisPoint ToPGis(Point2<double> geom)
        {
            if (geom != null)
                return new PostgisPoint(geom.X, geom.Y);
            return null;
        }

        public static PostgisLineString ToPGis(LineSegment2<double> geom)
        {
            if (geom != null)
            {
                return new PostgisLineString(new Coordinate2D[] { new Coordinate2D(geom.Start.X, geom.Start.Y), new Coordinate2D(geom.End.X, geom.End.Y) });
            }
            return null;
        }

        public static PostgisLineString ToPGis(LineChain2<double> geom)
        {
            if (geom != null)
            {
                return new PostgisLineString(Points(geom));
            }
            return null;
        }

        public static PostgisLineString ToPGis(Polyline2<double> geom)
        {
            if (geom != null)
            {
                return new PostgisLineString(Points(geom));
            }
            return null;
        }

        public static PostgisMultiLineString ToPGis(LineChainBag2<double> geom)
        {
            if (geom != null)
            {
                return new PostgisMultiLineString(Points(geom));
            }
            return null;
        }

        public static PostgisMultiLineString ToPGis(LineChainSet2<double> geom)
        {
            if (geom != null)
            {
                return new PostgisMultiLineString(Points(geom));
            }
            return null;
        }

        public static PostgisMultiLineString ToPGis(PolylineBag2<double> geom)
        {
            if (geom != null)
            {
                return new PostgisMultiLineString(Points(geom));
            }
            return null;
        }

        public static PostgisMultiLineString ToPGis(PolylineSet2<double> geom)
        {
            if (geom != null)
            {
                return new PostgisMultiLineString(Points(geom));
            }
            return null;
        }

        public static PostgisMultiPoint ToPGis(PointBag2<double> geom)
        {
            if (geom != null)
                return new PostgisMultiPoint(Points(geom));
            return null;
        }

        public static PostgisMultiPoint ToPGis(PointSet2<double> geom)
        {
            if (geom != null)
                return new PostgisMultiPoint(Points(geom));
            return null;
        }

        public static PostgisMultiPolygon ToPGis(PolygonBag2<double> geom)
        {
            if (geom != null)
                return new PostgisMultiPolygon(Points(geom));
            return null;
        }

        public static PostgisMultiPolygon ToPGis(PolygonSet2<double> geom)
        {
            if (geom != null)
                return new PostgisMultiPolygon(Points(geom));
            return null;
        }

        public static PostgisPolygon ToPGis(Ring2<double> geom)
        {
            if (geom != null)
            {
                List<List<Coordinate2D>> pts = new List<List<Coordinate2D>>();
                pts.Add(Points((IEnumerable<Point2<double>>)geom));
                pts[0].Add(pts[0][0]);
                return new PostgisPolygon(pts);
            }
            return null;
        }

        public static PostgisPolygon ToPGis(Polygon2<double> geom)
        {
            if (geom != null)
            {
                return new PostgisPolygon(Points(geom));
            }
            return null;
        }
    }
}
