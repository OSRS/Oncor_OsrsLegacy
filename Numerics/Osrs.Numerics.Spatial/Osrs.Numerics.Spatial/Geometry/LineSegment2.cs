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
using Osrs.Text;
using System;

namespace Osrs.Numerics.Spatial.Geometry
{
    //Line segment oriented from Start->End
    public sealed class LineSegment2<T> : ISimpleGeometry<Point2<T>,T>, IGeometry2<T>, IEquatable<LineSegment2<T>>, IComparable<LineSegment2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly Point2<T> Start;
        public readonly Point2<T> End;

        public Envelope2<T> Envelope
        {
            get
            {
                T minX = this.Start.X;
                T minY = this.Start.Y;
                T maxX = this.End.X;
                T maxY = this.End.Y;
                if (maxX.CompareTo(minX) < 0)
                {
                    maxX = this.Start.X;
                    minX = this.End.X;
                }
                if (maxY.CompareTo(minY) < 0)
                {
                    maxY = this.Start.Y;
                    minY = this.End.Y;
                }
                return new Envelope2<T>(minX, minY, maxX, maxY);
            }
        }

        IEnvelope<T> IGeometry<T>.Envelope
        {
            get
            {
                return this.Envelope;
            }
        }

        IEnvelope IGeometry.Envelope
        {
            get
            {
                return this.Envelope;
            }
        }

        public uint VertexCount
        {
            get { return 2; }
        }

        public bool HasEnvelope
        {
            get { return true; }
        }

        public GeometryFactory2Base<T> Factory
        {
            get { return GeometryFactory2Manager.Factory<T>(); }
        }

        IGeometryFactory<T> IGeometry<T>.Factory
        {
            get
            {
                return this.Factory;
            }
        }

        IGeometryFactory IGeometry.Factory
        {
            get
            {
                return this.Factory;
            }
        }

        public Point2<T> this[uint index]
        {
            get
            {
                if (index == 0)
                    return this.Start;
                if (index == 1)
                    return this.End;
                throw new IndexOutOfRangeException();
            }
        }

        internal LineSegment2(Point2<T> start, Point2<T> end)
        {
            if (start == null || end == null)
                throw new ArgumentNullException();
            if (start.Equals(end))
                throw new DegenerateCaseException();
            this.Start = start;
            this.End = end;
        }

        public LineSegment2(LineSegment2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Start = other.Start;
            this.End = other.End;
        }

        public LineSegment2<T> Reverse()
        {
            return new LineSegment2<T>(this.End, this.Start);
        }
        
        public bool Equals(LineSegment2<T> other)
        {
            if (other == null)
                return false;
            return this.Start.Equals(other.Start) && this.End.Equals(other.End);
        }

        public bool EqualsNonDirectional(LineSegment2<T> other)
        {
            if (this.Start.Equals(other.Start))
                return this.End.Equals(other.End);
            if (this.End.Equals(other.Start))
                return this.Start.Equals(other.End);
            return false;
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as LineSegment2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as LineSegment2<T>);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as LineSegment2<T>);
        }

        public int CompareTo(LineSegment2<T> other)
        {
            Point2<T> min;
            Point2<T> max;
            if (this.Start.CompareTo(this.End) < 0)
            {
                min = this.Start;
                max = this.End;
            }
            else
            {
                min = this.End;
                max = this.Start;
            }
            Point2<T> minO;
            Point2<T> maxO;
            if (other.Start.CompareTo(other.End) < 0)
            {
                minO = other.Start;
                maxO = other.End;
            }
            else
            {
                minO = other.End;
                maxO = other.Start;
            }

            if (max.CompareTo(minO) <= 0)
                return -1; //max of this is <= min of that
            else if (min.CompareTo(maxO) >= 0)
                return 1; //min of this is >= max of that
            return min.CompareTo(minO); //partially overlap
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as LineSegment2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as LineSegment2<T>);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as LineSegment2<T>);
        }

        public override int GetHashCode()
        {
            return this.Start.GetHashCode() ^ this.End.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("Start", this.Start.ToString());
            sb.Append("End", this.Start.ToString());
            return sb.ToString();
        }

        public static bool operator ==(LineSegment2<T> a, LineSegment2<T> b)
        {
            if (Object.ReferenceEquals(a, null))
                return Object.ReferenceEquals(b, null);
            if (Object.ReferenceEquals(b, null))
                return false;
            return a.Start.Equals(b.Start) && a.End.Equals(b.End);
        }

        public static bool operator !=(LineSegment2<T> a, LineSegment2<T> b)
        {
            return !(a == b);
        }

        public static implicit operator CoordinatePair2<T>(LineSegment2<T> item)
        {
            if (item == null)
                return null;
            return new CoordinatePair2<T>(item.Start, item.End);
        }

        public static implicit operator LineChain2<T>(LineSegment2<T> item)
        {
            if (item == null)
                return null;
            return new LineChain2<T>(new Point2<T>[]{item.Start, item.End});
        }

        public static explicit operator LineSegment2<T>(LineChain2<T> item)
        {
            if (item == null)
                return null;
            Point2<T> st = (Point2<T>)item.Points[0];
            Point2<T> ed = (Point2<T>)item.Points[item.Points.Length-1];
            return new LineSegment2<T>(st, ed);
        }

        public static implicit operator Polyline2<T>(LineSegment2<T> item)
        {
            if (item == null)
                return null;
            return new Polyline2<T>(new Point2<T>[] { item.Start, item.End });
        }

        public static explicit operator LineSegment2<T>(Polyline2<T> item)
        {
            if (item == null)
                return null;
            Point2<T> st = (Point2<T>)item.Points[0];
            Point2<T> ed = (Point2<T>)item.Points[item.Points.Length - 1];
            if (st.Equals(ed))
                throw new DegenerateCaseException();
            return new LineSegment2<T>(st, ed);
        }

        public static explicit operator Line2<T>(LineSegment2<T> item)
        {
            if (item == null)
                return null;
            return new Line2<T>(item.Start, item.End);
        }

        public static explicit operator Ray2<T>(LineSegment2<T> item)
        {
            if (item == null)
                return null;
            return new Ray2<T>(item.Start, item.End);
        }
    }
}
