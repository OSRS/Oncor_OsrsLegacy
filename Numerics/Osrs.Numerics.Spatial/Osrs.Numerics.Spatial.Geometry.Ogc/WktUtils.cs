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
using System.Text;

namespace Osrs.Numerics.Spatial.Geometry
{
    public static class WktUtils
    {
        //POINT(0 0)
        public static string ToWkt(Point2<double> geom)
        {
            if (geom != null)
                return string.Format("POINT({0} {1})", geom.X, geom.Y);
            return string.Empty;
        }

        //MULTIPOINT((0 0),(1 2))
        public static string ToWkt(PointBag2<double> geom)
        {
            if (geom != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("MULTIPOINT(");
                for (uint i = 0; i < geom.VertexCount; i++)
                {
                    sb.Append('(');
                    sb.Append(geom[i].X);
                    sb.Append(' ');
                    sb.Append(geom[i].Y);
                    sb.Append("),");
                }
                if (geom.VertexCount > 0)
                    sb[sb.Length - 1] = ')'; //replace last paren
                else
                    sb.Append(')'); //empty case

                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(PointSet2<double> geom)
        {
            if (geom != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("MULTIPOINT(");
                for (uint i = 0; i < geom.VertexCount; i++)
                {
                    sb.Append('(');
                    sb.Append(geom[i].X);
                    sb.Append(' ');
                    sb.Append(geom[i].Y);
                    sb.Append("),");
                }
                if (geom.VertexCount > 0)
                    sb[sb.Length - 1] = ')'; //replace last paren
                else
                    sb.Append(')'); //empty case

                return sb.ToString();
            }
            return string.Empty;
        }

        //LINESTRING(0 0,1 1,1 2)
        public static string ToWkt(LineChain2<double> geom)
        {
            if (geom != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("LINESTRING(");
                for(uint i=0;i<geom.VertexCount;i++)
                {
                    sb.Append(geom[i].X);
                    sb.Append(' ');
                    sb.Append(geom[i].Y);
                    sb.Append(',');
                }
                sb[sb.Length - 1] = ')';
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(LineSegment2<double> geom)
        {
            if (geom != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("LINESTRING(");

                sb.Append(geom.Start.X);
                sb.Append(' ');
                sb.Append(geom.Start.Y);

                sb.Append(',');

                sb.Append(geom.End.X);
                sb.Append(' ');
                sb.Append(geom.End.Y);

                sb.Append(')');
                return sb.ToString();
            }
            return string.Empty;
        }

        //MULTILINESTRING((0 0,1 1,1 2),(2 3,3 2,5 4))
        public static string ToWkt(LineChainBag2<double> geom)
        {
            if (geom != null)
            {
                LineChain2<double> cur;

                StringBuilder sb = new StringBuilder();
                sb.Append("MULTILINESTRING(");
                for(uint i=0;i<geom.PartCount;i++)
                {
                    sb.Append('(');
                    cur = geom[i];

                    for(uint j=0;j< cur.VertexCount;j++)
                    {
                        sb.Append(cur[j].X);
                        sb.Append(' ');
                        sb.Append(cur[j].Y);
                        sb.Append(',');
                    }
                    sb.Length = sb.Length - 1; //truncate last ,
                    sb.Append("),");
                }
                if (geom.PartCount > 0)
                    sb[sb.Length - 1] = ')';
                else
                    sb.Append(')');
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(LineChainSet2<double> geom)
        {
            if (geom != null)
            {
                LineChain2<double> cur;

                StringBuilder sb = new StringBuilder();
                sb.Append("MULTILINESTRING(");
                for (uint i = 0; i < geom.PartCount; i++)
                {
                    sb.Append('(');
                    cur = geom[i];

                    for (uint j = 0; j < cur.VertexCount; j++)
                    {
                        sb.Append(cur[j].X);
                        sb.Append(' ');
                        sb.Append(cur[j].Y);
                        sb.Append(',');
                    }
                    sb.Length = sb.Length - 1; //truncate last ,
                    sb.Append("),");
                }
                if (geom.PartCount > 0)
                    sb[sb.Length - 1] = ')';
                else
                    sb.Append(')');
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(Polyline2<double> geom)
        {
            if (geom != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("LINESTRING(");
                for (uint i = 0; i < geom.VertexCount; i++)
                {
                    sb.Append(geom[i].X);
                    sb.Append(' ');
                    sb.Append(geom[i].Y);
                    sb.Append(',');
                }
                sb[sb.Length - 1] = ')';
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(PolylineBag2<double> geom)
        {
            if (geom != null)
            {
                Polyline2<double> cur;

                StringBuilder sb = new StringBuilder();
                sb.Append("MULTILINESTRING(");
                for (uint i = 0; i < geom.PartCount; i++)
                {
                    sb.Append('(');
                    cur = geom[i];

                    for (uint j = 0; j < cur.VertexCount; j++)
                    {
                        sb.Append(cur[j].X);
                        sb.Append(' ');
                        sb.Append(cur[j].Y);
                        sb.Append(',');
                    }
                    sb.Length = sb.Length - 1; //truncate last ,
                    sb.Append("),");
                }
                if (geom.PartCount > 0)
                    sb[sb.Length - 1] = ')';
                else
                    sb.Append(')');
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(PolylineSet2<double> geom)
        {
            if (geom != null)
            {
                Polyline2<double> cur;

                StringBuilder sb = new StringBuilder();
                sb.Append("MULTILINESTRING(");
                for (uint i = 0; i < geom.PartCount; i++)
                {
                    sb.Append('(');
                    cur = geom[i];

                    for (uint j = 0; j < cur.VertexCount; j++)
                    {
                        sb.Append(cur[j].X);
                        sb.Append(' ');
                        sb.Append(cur[j].Y);
                        sb.Append(',');
                    }
                    sb.Length = sb.Length - 1; //truncate last ,
                    sb.Append("),");
                }
                if (geom.PartCount > 0)
                    sb[sb.Length - 1] = ')';
                else
                    sb.Append(')');
                return sb.ToString();
            }
            return string.Empty;
        }

        //POLYGON((0 0,4 0,4 4,0 4,0 0),(1 1, 2 1, 2 2, 1 2,1 1))
        public static string ToWkt(Ring2<double> geom)
        {
            if (geom != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("POLYGON((");
                for (uint i = 0; i < geom.VertexCount; i++)
                {
                    sb.Append(geom[i].X);
                    sb.Append(' ');
                    sb.Append(geom[i].Y);
                    sb.Append(',');
                }
                sb[sb.Length - 1] = ')';
                sb.Append(')');
                return sb.ToString();
            }
            return string.Empty;
        }

        //MULTIPOLYGON(((0 0,4 0,4 4,0 4,0 0),(1 1,2 1,2 2,1 2,1 1)), ((-1 -1,-1 -2,-2 -2,-2 -1,-1 -1)))
        public static string ToWkt(RingBag2<double> geom)
        {
            if (geom != null)
            {
                Ring2<double> cur;

                StringBuilder sb = new StringBuilder();
                sb.Append("MULTIPOLYGON(");
                for (uint i = 0; i < geom.PartCount; i++)
                {
                    sb.Append("((");
                    cur = geom[i];

                    for (uint j = 0; j < cur.VertexCount; j++)
                    {
                        sb.Append(cur[j].X);
                        sb.Append(' ');
                        sb.Append(cur[j].Y);
                        sb.Append(',');
                    }
                    sb.Length = sb.Length - 1; //truncate last ,
                    sb.Append(")),");
                }
                if (geom.PartCount > 0)
                    sb[sb.Length - 1] = ')';
                else
                    sb.Append(')');
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(RingSet2<double> geom)
        {
            if (geom != null)
            {
                Ring2<double> cur;

                StringBuilder sb = new StringBuilder();
                sb.Append("MULTIPOLYGON(");
                for (uint i = 0; i < geom.PartCount; i++)
                {
                    sb.Append("((");
                    cur = geom[i];

                    for (uint j = 0; j < cur.VertexCount; j++)
                    {
                        sb.Append(cur[j].X);
                        sb.Append(' ');
                        sb.Append(cur[j].Y);
                        sb.Append(',');
                    }
                    sb.Length = sb.Length - 1; //truncate last ,
                    sb.Append(")),");
                }
                if (geom.PartCount > 0)
                    sb[sb.Length - 1] = ')';
                else
                    sb.Append(')');
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(Polygon2<double> geom)
        {
            if (geom != null)
            {
                Ring2<double> cur;
                StringBuilder sb = new StringBuilder();
                sb.Append("POLYGON(");
                for (uint j = 0; j < geom.SectionCount; j++)
                {
                    cur = geom[j];
                    sb.Append('(');
                    for (uint i = 0; i < cur.VertexCount; i++)
                    {
                        sb.Append(cur[i].X);
                        sb.Append(' ');
                        sb.Append(cur[i].Y);
                        sb.Append(',');
                    }
                    sb[sb.Length - 1] = ')';
                    sb.Append(',');
                }
                sb[sb.Length - 1] = ')';
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(PolygonBag2<double> geom)
        {
            if (geom != null)
            {
                Polygon2<double> curPoly;
                Ring2<double> cur;

                StringBuilder sb = new StringBuilder();
                sb.Append("MULTIPOLYGON(");
                for (uint k = 0; k < geom.PartCount; k++)
                {
                    curPoly = geom[k];
                    sb.Append('(');

                    for (uint j = 0; j < curPoly.SectionCount; j++)
                    {
                        cur = curPoly[j];
                        sb.Append('(');
                        for (uint i = 0; i < cur.VertexCount; i++)
                        {
                            sb.Append(cur[i].X);
                            sb.Append(' ');
                            sb.Append(cur[i].Y);
                            sb.Append(',');
                        }
                        sb[sb.Length - 1] = ')';
                        sb.Append(',');
                    }

                    sb[sb.Length - 1] = ')';
                    sb.Append(',');
                }
                sb[sb.Length - 1] = ')';
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string ToWkt(PolygonSet2<double> geom)
        {
            if (geom != null)
            {
                Polygon2<double> curPoly;
                Ring2<double> cur;

                StringBuilder sb = new StringBuilder();
                sb.Append("MULTIPOLYGON(");
                for (uint k = 0; k < geom.PartCount; k++)
                {
                    curPoly = geom[k];
                    sb.Append('(');

                    for (uint j = 0; j < curPoly.SectionCount; j++)
                    {
                        cur = curPoly[j];
                        sb.Append('(');
                        for (uint i = 0; i < cur.VertexCount; i++)
                        {
                            sb.Append(cur[i].X);
                            sb.Append(' ');
                            sb.Append(cur[i].Y);
                            sb.Append(',');
                        }
                        sb[sb.Length - 1] = ')';
                        sb.Append(',');
                    }

                    sb[sb.Length - 1] = ')';
                    sb.Append(',');
                }
                sb[sb.Length - 1] = ')';
                return sb.ToString();
            }
            return string.Empty;
        }

        //TODO -- implement these once the geometry types are implemented
        //GEOMETRYCOLLECTION(POINT(2 3),LINESTRING(2 3,3 4))
        public static string ToWkt(Geometry2Bag<double> geom)
        {
            throw new NotImplementedException();
        }

        public static string ToWkt(Geometry2Set<double> geom)
        {
            throw new NotImplementedException();
        }
    }
}
