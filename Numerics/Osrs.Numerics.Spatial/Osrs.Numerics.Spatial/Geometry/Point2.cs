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
    public sealed class Point2<T> : IGeometry2<T>, ISimpleGeometry<Point2<T>, T>, IComparable<Point2<T>>, IEquatable<Point2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly T X; //for performance
        public readonly T Y; //for performance

        public Point2<T> this[uint index]
        {
            get
            {
                if (index == 0)
                    return this;
                throw new IndexOutOfRangeException();
            }
        }

        public Envelope2<T> Envelope
        {
            get { return null; }
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
            get { return 1; }
        }

        public bool HasEnvelope
        {
            get { return false; }
        }

        public GeometryFactory2Base<T> Factory
        {
            get { return GeometryFactory2Manager.Factory<T>(); }
        }

        IGeometryFactory<T> IGeometry<T>.Factory
        {
            get { return this.Factory; }
        }

        IGeometryFactory IGeometry.Factory
        {
            get
            {
                return this.Factory;
            }
        }

        internal Point2(T x, T y)
        {
            if (x != null && y != null)
            {
                this.X = x;
                this.Y = y;
            }
            else
                throw new ArgumentNullException();
        }

        public Point2(Point2<T> other)
        {
            if (other != null)
            {
                this.X = other.X;
                this.Y = other.Y;
            }
            else
                throw new ArgumentNullException();
        }

        public int CompareTo(Point2<T> other)
        {
            if (other == null)
                return 1;
            int dx = this.X.CompareTo(other.X);
            if (dx == 0)
                return this.Y.CompareTo(other.Y);
            return dx;
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as Point2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as Point2<T>);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as Point2<T>);
        }

        public bool Equals(Point2<T> other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;
            return this.X.Equals(other.X) && this.Y.Equals(other.Y);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as Point2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as Point2<T>);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Point2<T>);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("x", this.X.ToString());
            sb.Append("y", this.Y.ToString());
            return sb.ToString();
        }

        public static bool operator ==(Point2<T> a, Point2<T> b)
        {
            if (Object.ReferenceEquals(a, null))
                return Object.ReferenceEquals(b, null);
            if (Object.ReferenceEquals(b, null))
                return false;
            return a.X.Equals(b.X) && a.Y.Equals(b.Y);
        }

        public static bool operator !=(Point2<T> a, Point2<T> b)
        {
            return !(a == b);
        }

        public static implicit operator Coordinate2<T>(Point2<T> item)
        {
            if (item == null)
                return null;
            return new Coordinate2<T>(item.X, item.Y);
        }

        public static implicit operator PointSet2<T>(Point2<T> item)
        {
            if (item == null)
                return null;
            return new PointSet2<T>(new Point2<T>[] { item });
        }

        public static implicit operator PointBag2<T>(Point2<T> item)
        {
            if (item == null)
                return null;
            return new PointBag2<T>(new Point2<T>[] { item });
        }
    }
}