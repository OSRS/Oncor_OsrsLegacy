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

using Osrs.Text;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Osrs.Numerics.Spatial.Geometry
{
    public sealed class PointSet2<T> : ISimplePointCollection2<T>, IEquatable<PointSet2<T>>, IComparable<PointSet2<T>>, IEnumerable<Point2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        internal readonly Point2<T>[] Points;

        public uint VertexCount
        {
            get { return (uint)this.Points.Length; }
        }

        private Envelope2<T> envelope;
        public Envelope2<T> Envelope
        {
            get
            {
                if (this.envelope == null)
                {
                    Point2<T> cur = this.Points[0];
                    T minX = cur.X;
                    T maxX = cur.X;
                    T minY = cur.Y;
                    T maxY = cur.Y;
                    for (int i = 1; i < this.Points.Length; i++)
                    {
                        cur = this.Points[i];
                        if (minX.CompareTo(cur.X) > 0)
                            minX = cur.X;
                        else if (maxX.CompareTo(cur.X) < 0)
                            maxX = cur.X;
                        if (minY.CompareTo(cur.Y) > 0)
                            minY = cur.Y;
                        else if (maxY.CompareTo(cur.Y) < 0)
                            maxY = cur.Y;
                    }
                    this.envelope = new Envelope2<T>(minX, minY, maxX, maxY);
                }
                return this.envelope;
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
            get { return this.Points[index]; }
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal PointSet2(Point2<T>[] points)
        {
            if (points == null)
                throw new ArgumentNullException();
            this.Points = points;
        }

        internal PointSet2(LineSegment2<T> item)
        {
            if (item == null)
                throw new ArgumentNullException();
            this.Points = new Point2<T>[] { item.Start, item.End };
        }

        public PointSet2(PointSet2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Points = other.Points;
        }

        public Point2<T>[] ToArray()
        {
            Point2<T>[] tmp = new Point2<T>[this.Points.Length];
            Array.Copy(this.Points, tmp, tmp.Length);
            return tmp;
        }

        public PointSet2<T> CoincidentPoints()
        {
            return null;
        }

        public bool HasCoincidentPoints()
        {
            return false;
        }

        public override int GetHashCode()
        {
            return this.Points.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("Points", this.Points);
            return sb.ToString();
        }

        public bool Equals(PointSet2<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as PointSet2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as PointSet2<T>);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PointSet2<T>);
        }

        public int CompareTo(PointSet2<T> other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as PointSet2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as PointSet2<T>);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as PointSet2<T>);
        }

        public IEnumerator<Point2<T>> GetEnumerator()
        {
            return ((IEnumerable<Point2<T>>)this.Points).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Points.GetEnumerator();
        }

        public static bool operator ==(PointSet2<T> a, PointSet2<T> b)
        {
            if (Object.ReferenceEquals(a, null))
                return Object.ReferenceEquals(b, null);
            if (Object.ReferenceEquals(b, null))
                return false;
            if (Object.ReferenceEquals(a, b))
                return true;
            return false; //TODO -- add value semantics at some point
        }

        public static bool operator !=(PointSet2<T> a, PointSet2<T> b)
        {
            return !(a == b);
        }

        public static implicit operator PointBag2<T>(PointSet2<T> item)
        {
            if (item == null)
                return null;
            return new PointBag2<T>(item.Points);
        }
    }
}
