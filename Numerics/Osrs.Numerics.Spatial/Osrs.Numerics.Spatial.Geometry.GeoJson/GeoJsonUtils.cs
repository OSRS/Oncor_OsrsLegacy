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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial.Geometry
{
    public static class GeoJsonUtils
    {
        private const string geoJsonType = "type";
        private const string geoJsonCoordinates = "coordinates";
        private const string geoJsonGeometry = "geometry";
        private const string geoJsonGeometries = "geometries";
        private const string geoJsonBbox = "bbox";
        private const string geoJsonProperties = "properties";
        private const string geoJsonFeatures = "features";
        private const string geoJsonFeature = "Feature";
        private const string geoJsonFeatureCollection = "FeatureCollection";
        private const string geoJsonCrs = "crs";
        private const string geoJsonName = "name";
        private const string geoJsonLink = "link";
        private const string geoJsonHref = "href";
        private const string geoJsonPoint = "Point";
        private const string geoJsonMultiPoint = "MultiPoint";
        private const string geoJsonLineString = "LineString";
        private const string geoJsonMultiLineString = "MultiLineString";
        private const string geoJsonPolygon = "Polygon";
        private const string geoJsonMultiPolygon = "MultiPolygon";
        private const string geoJsonGeometryCollection = "GeometryCollection";

        private static Position FetchCoordinates(JArray arr)
        {
            return GeoJsonUtils.FetchCoordinates(arr, PositionType.Unknown);
        }

        private static Position FetchCoordinates(JArray arr, PositionType hint)
        {
            if (arr == null || arr.Count <= 0)
                return (Position)null;
            if (arr[0].Type != JTokenType.Array)
            {
                try
                {
                    return new Position(Extensions.Value<double>((IEnumerable<JToken>)arr[0]), Extensions.Value<double>((IEnumerable<JToken>)arr[1]), hint);
                }
                catch
                {
                }
                return (Position)null;
            }
            PositionSet positionSet = new PositionSet(hint);
            foreach (JToken jtoken in arr)
            {
                if (jtoken != null && jtoken.Type == JTokenType.Array)
                {
                    Position position = GeoJsonUtils.FetchCoordinates(jtoken as JArray);
                    if (position != null)
                        positionSet.Positions.Add(position);
                }
            }
            return (Position)positionSet;
        }

        private static KeyValuePair<string, JObject> GetCrs(JObject o)
        {
            JToken jtoken1 = o["crs"];
            if (jtoken1 != null && jtoken1.Type == JTokenType.Object)
            {
                JObject jobject1 = jtoken1 as JObject;
                if (jobject1 != null)
                {
                    JToken jtoken2 = jobject1["type"];
                    if (jtoken2 != null && jtoken2.Type == JTokenType.String && "name" == jtoken2.ToString())
                    {
                        JToken jtoken3 = jobject1["properties"];
                        if (jtoken3 != null && jtoken3.Type == JTokenType.Object)
                        {
                            JObject jobject2 = jtoken3 as JObject;
                            if (jobject2 != null)
                            {
                                JToken jtoken4 = jobject2["name"];
                                if (jtoken4 != null && jtoken4.Type == JTokenType.String)
                                    return new KeyValuePair<string, JObject>(jtoken4.ToString(), jobject1);
                            }
                        }
                    }
                }
            }
            return new KeyValuePair<string, JObject>((string)null, (JObject)null);
        }

        public static IGeometry2<double> ExtractGeometry(JObject o)
        {
            IGeometry2<double> geometry1 = null;
            o = GeoJsonUtils.ExtractGeoJson(o);
            if (o != null)
            {
                JObject jobject = (JObject)null;
                string str1 = (string)null;
                KeyValuePair<string, JObject> crs = GeoJsonUtils.GetCrs(o);
                if (crs.Key != null)
                {
                    jobject = crs.Value;
                    str1 = crs.Key;
                }
                JToken jtoken1 = o["type"];
                if (jtoken1 != null && jtoken1.Type == JTokenType.String)
                {
                    string str2 = jtoken1.ToString();
                    if (!string.IsNullOrEmpty(str2))
                    {
                        string str3 = str2;
                        if (str3 == "Feature")
                        {
                            JToken jtoken2 = o["geometry"];
                            if (jtoken2 == null || jtoken2.Type != JTokenType.Object)
                                return null;
                            JObject o1 = jtoken2 as JObject;
                            if (jobject != null)
                                o1["crs"] = (JToken)jobject;
                            return GeoJsonUtils.ExtractGeometry(o1);
                        }
                        if (str3 == "FeatureCollection")
                        {
                            JToken jtoken2 = o["features"];
                            if (jtoken2 != null && jtoken2.Type == JTokenType.Array)
                            {
                                JArray jarray = (JArray)jtoken2;
                                List<IGeometry2<double>> list = new List<IGeometry2<double>>();
                                foreach (JToken jtoken3 in jarray)
                                {
                                    if (jtoken3 != null && jtoken3.Type == JTokenType.Object)
                                    {
                                        JObject o1 = jtoken2 as JObject;
                                        if (jobject != null && o1["crs"] == null)
                                            o1["crs"] = (JToken)jobject;
                                        IGeometry2<double> geometry2 = GeoJsonUtils.ExtractGeometry(o1);
                                        if (geometry2 == null)
                                            return null;
                                        list.Add(geometry2);
                                    }
                                }
                                //return (Geometry)(Geometry.DefaultFactory.CreateGeometryCollection((IGeometry[])list.ToArray()) as GeometryCollection);
                                return null; //right now we can't handle mixed geometry types
                            }
                        }
                        else if (str3 == "GeometryCollection")
                        {
                            JToken jtoken2 = o["geometries"];
                            if (jtoken2 != null && jtoken2.Type == JTokenType.Array)
                            {
                                JArray jarray = (JArray)jtoken2;
                                List<IGeometry2<double>> list = new List<IGeometry2<double>>();
                                foreach (JToken jtoken3 in jarray)
                                {
                                    if (jtoken3 != null && jtoken3.Type == JTokenType.Object)
                                    {
                                        JObject o1 = jtoken2 as JObject;
                                        if (jobject != null && o1["crs"] == null)
                                            o1["crs"] = (JToken)jobject;
                                        IGeometry2<double> geometry2 = GeoJsonUtils.ExtractGeometry(o1);
                                        if (geometry2 == null)
                                            return null;
                                        list.Add(geometry2);
                                    }
                                }
                                //return (Geometry)(Geometry.DefaultFactory.CreateGeometryCollection((IGeometry[])list.ToArray()) as GeometryCollection);
                                return null; //right now we can't handle mixed geometry types
                            }
                        }
                        else
                        {
                            JToken jtoken2 = o["coordinates"];
                            if (jtoken2 != null && jtoken2.Type == JTokenType.Array)
                            {
                                JArray arr = jtoken2 as JArray;
                                if (arr != null)
                                {
                                    if ("Point" == str2)
                                        geometry1 = GeometryUtils.BuildPoint(GeoJsonUtils.FetchCoordinates(arr, PositionType.Point));
                                    else if ("MultiPoint" == str2)
                                        geometry1 = GeometryUtils.BuildMultiPoint(GeoJsonUtils.FetchCoordinates(arr, PositionType.MultiPoint));
                                    else if ("LineString" == str2)
                                        geometry1 = GeometryUtils.BuildLineString(GeoJsonUtils.FetchCoordinates(arr, PositionType.LineString));
                                    else if ("MultiLineString" == str2)
                                        geometry1 = GeometryUtils.BuildMultiLineString(GeoJsonUtils.FetchCoordinates(arr, PositionType.MultiLineString));
                                    else if ("Polygon" == str2)
                                        geometry1 = GeometryUtils.BuildPolygon(GeoJsonUtils.FetchCoordinates(arr, PositionType.Polygon));
                                    else if ("MultiPolygon" == str2)
                                        geometry1 = GeometryUtils.BuildMultiPolygon(GeoJsonUtils.FetchCoordinates(arr, PositionType.MultiPolygon));
                                    //if (geometry1 != null && jobject != null)
                                    //    geometry1.UserData = (object)str1;  //TODO - deal with the JTable issue
                                    return geometry1;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static JObject ExtractGeoJson(JObject o)
        {
            if (o != null)
            {
                JToken jtoken1 = o["type"];
                JObject jobject1 = (JObject)null;
                if (jtoken1 != null && jtoken1.Type == JTokenType.String)
                {
                    string str = jtoken1.ToString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        if ("Point" == str || "Polygon" == str || ("LineString" == str || "MultiLineString" == str) || ("MultiPoint" == str || "MultiPolygon" == str || "GeometryCollection" == str))
                            return o;
                        JToken jtoken2 = o["crs"];
                        if (jtoken2 != null && jtoken2.Type == JTokenType.Object)
                            jobject1 = jtoken2 as JObject;
                        if ("Feature" == str)
                        {
                            JToken jtoken3 = o["geometry"];
                            if (jtoken3 != null && jtoken3.Type == JTokenType.Object)
                            {
                                o = GeoJsonUtils.ExtractGeoJson(jtoken3 as JObject);
                                if (o != null)
                                {
                                    if (jobject1 != null)
                                    {
                                        JToken jtoken4 = o["crs"];
                                        if (jtoken4 == null || jtoken4.Type == JTokenType.None || jtoken4.Type == JTokenType.Null)
                                            o.Add("crs", (JToken)jobject1);
                                    }
                                    return o;
                                }
                            }
                        }
                        else if ("FeatureCollection" == str)
                        {
                            JToken jtoken3 = o["features"];
                            if (jtoken3 != null && jtoken3.Type == JTokenType.Array)
                            {
                                JArray jarray1 = jtoken3 as JArray;
                                if (jarray1 != null && jarray1.Count > 0)
                                {
                                    JArray jarray2 = new JArray();
                                    JObject jobject2 = new JObject();
                                    jobject2.Add("type", (JToken)"GeometryCollection");
                                    jobject2.Add("geometries", (JToken)jarray2);
                                    if (jobject1 != null)
                                        jobject2.Add("crs", (JToken)jobject1);
                                    foreach (JToken jtoken4 in jarray1)
                                    {
                                        if (jtoken4 != null && jtoken4.Type == JTokenType.Object)
                                        {
                                            JToken jtoken5 = (JToken)GeoJsonUtils.ExtractGeoJson(jtoken4 as JObject);
                                            if (jtoken5 != null)
                                                jarray2.Add(jtoken5);
                                        }
                                    }
                                    return jobject2;
                                }
                            }
                        }
                    }
                }
            }
            return (JObject)null;
        }

        public static IGeometry2<double> ParseGeometry(string json)
        {
            JObject o = GeoJsonUtils.ParseGeoJson(json);
            if (o != null)
                return GeoJsonUtils.ExtractGeometry(o);
            return null;
        }

        public static JObject ParseGeoJson(string json)
        {
            JObject o = JToken.Parse(json) as JObject;
            if (o != null)
                return GeoJsonUtils.ExtractGeoJson(o);
            return (JObject)null;
        }

        public static JObject ToGeoJson(IGeometry2<double> geom)
        {
            if (geom is Geometry2Bag<double>)
                return GeoJsonUtils.ToGeoJson(geom as Geometry2Bag<double>);
            if (geom is PolygonBag2<double>)
                return GeoJsonUtils.ToGeoJson(geom as PolygonBag2<double>);
            if (geom is Polygon2<double>)
                return GeoJsonUtils.ToGeoJson(geom as Polygon2<double>);
            if (geom is Polyline2<double>)
                return GeoJsonUtils.ToGeoJson(geom as Polyline2<double>);
            if (geom is PolylineBag2<double>)
                return GeoJsonUtils.ToGeoJson(geom as PolylineBag2<double>);
            if (geom is Point2<double>)
                return GeoJsonUtils.ToGeoJson(geom as Point2<double>);
            if (geom is PointBag2<double>)
                return GeoJsonUtils.ToGeoJson(geom as PointBag2<double>);
            return (JObject)null;
        }

        public static JObject ToGeoJson(Geometry2Bag<double> geom)
        {
            if (geom == null)
                return (JObject)null;
            JObject jobject = new JObject();
            jobject.Add("type", (JToken)new JValue("GeometryCollection"));
            JArray jarray = new JArray();
            jobject.Add("geometries", (JToken)jarray);
            foreach (IGeometry2<double> geometry in geom)
            {
                JToken jtoken;
                if (geometry is PolygonBag2<double>)
                    jtoken = (JToken)GeoJsonUtils.ToGeoJson(geometry as PolygonBag2<double>);
                else if (geometry is Polygon2<double>)
                    jtoken = (JToken)GeoJsonUtils.ToGeoJson(geometry as Polygon2<double>);
                else if (geometry is Polyline2<double>)
                    jtoken = (JToken)GeoJsonUtils.ToGeoJson(geometry as Polyline2<double>);
                else if (geometry is PolylineBag2<double>)
                    jtoken = (JToken)GeoJsonUtils.ToGeoJson(geometry as PolylineBag2<double>);
                else if (geometry is Point2<double>)
                {
                    jtoken = (JToken)GeoJsonUtils.ToGeoJson(geometry as Point2<double>);
                }
                else
                {
                    if (!(geometry is PointBag2<double>))
                        return null;
                    jtoken = (JToken)GeoJsonUtils.ToGeoJson(geometry as PointBag2<double>);
                }
                if (jtoken == null)
                    return null;
                jarray.Add(jtoken);
            }
            return jobject;
        }

        public static JObject ToGeoJson(Polyline2<double> geom)
        {
            if (geom == null)
                return (JObject)null;
            JObject jobject = new JObject();
            jobject.Add("type", (JToken)new JValue("LineString"));
            JArray jarray = new JArray();
            foreach (Point2<double> n in geom)
            {
                jarray.Add((JToken)new JArray()
                    {
                      (JToken) new JValue(n.X),
                      (JToken) new JValue(n.Y)
                    });
            }
            jobject.Add("coordinates", (JToken)jarray);
            return jobject;
        }

        public static JObject ToGeoJson(PolylineBag2<double> geom)
        {
            if (geom == null)
                return (JObject)null;
            JObject jobject = new JObject();
            jobject.Add("type", (JToken)new JValue("MultiLineString"));
            JArray jarray1 = new JArray();
            foreach(Polyline2<double> lineString in geom)
            {
                JArray jarray2 = new JArray();
                foreach(Point2<double> coordinateN in lineString)
                {
                    jarray2.Add((JToken)new JArray()
                      {
                        (JToken) new JValue(coordinateN.X),
                        (JToken) new JValue(coordinateN.Y)
                      });
                }
                jarray1.Add((JToken)jarray2);
            }
            jobject.Add("coordinates", (JToken)jarray1);
            return jobject;
        }

        public static JObject ToGeoJson(Point2<double> geom)
        {
            if (geom == null)
                return (JObject)null;
            return new JObject()
              {
                {
                  "type",
                  (JToken) new JValue("Point")
                },
                {
                  "coordinates",
                  (JToken) new JArray()
                  {
                    (JToken) new JValue(geom.X),
                    (JToken) new JValue(geom.Y)
                  }
                }
              };
        }

        public static JObject ToGeoJson(PointBag2<double> geom)
        {
            if (geom == null)
                return (JObject)null;
            JObject jobject = new JObject();
            jobject.Add("type", (JToken)new JValue("MultiPoint"));
            JArray jarray = new JArray();
            foreach (Point2<double> point in geom)
                jarray.Add((JToken)new JArray()
                    {
                        (JToken) new JValue(point.X),
                        (JToken) new JValue(point.Y)
                    });
            jobject.Add("coordinates", (JToken)jarray);
            return jobject;
        }

        public static JObject ToGeoJson(Polygon2<double> geom)
        {
            if (geom == null)
                return (JObject)null;
            JObject jobject = new JObject();
            jobject.Add("type", (JToken)new JValue("Polygon"));
            JArray jarray1 = new JArray();
            JArray jarray2 = new JArray();
            jarray1.Add(jarray2); //for the outer ring
            foreach (Point2<double> coordinate in geom.OuterRing)
            {
                jarray2.Add((JToken)new JArray()
                    {
                      (JToken) coordinate.X,
                      (JToken) coordinate.Y
                    }
                );
            }
            if (geom.HasHoles)
            {
                foreach (Ring2<double> linearRing in geom.InnerRings)
                {
                    jarray2 = new JArray();
                    foreach (Point2<double> coordinate in linearRing)
                        jarray2.Add((JToken)new JArray()
                            {
                              (JToken) coordinate.X,
                              (JToken) coordinate.Y
                            });
                    jarray1.Add((JToken)jarray2);
                }
            }
            jobject.Add("coordinates", (JToken)jarray1);
            return jobject;
        }

        public static JObject ToGeoJson(PolygonBag2<double> geom)
        {
            if (geom == null)
                return (JObject)null;
            JObject jobject = new JObject();
            jobject.Add("type", (JToken)new JValue("MultiPolygon"));
            JArray jarray1 = new JArray();
            foreach (Polygon2<double> polygon in geom)
            {
                JArray jarray2 = new JArray();
                foreach (Point2<double> coordinate in polygon.OuterRing)
                    jarray2.Add((JToken)new JArray()
                      {
                        (JToken) coordinate.X,
                        (JToken) coordinate.Y
                      });
                if (polygon.HasHoles)
                {
                    foreach (Ring2<double> linearRing in polygon.InnerRings)
                    {
                        JArray jarray3 = new JArray();
                        foreach (Point2<double> coordinate in linearRing)
                            jarray3.Add((JToken)new JArray()
                              {
                                (JToken) coordinate.X,
                                (JToken) coordinate.Y
                              });
                        jarray2.Add((JToken)jarray3);
                    }
                }
                jarray1.Add((JToken)jarray2);
            }
            jobject.Add("coordinates", (JToken)jarray1);
            return jobject;
        }
    }
}
