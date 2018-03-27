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
using System.Linq;
using System.Collections;

namespace Osrs.Numerics.Spatial.Geometry
{
    //single polyline - may self-intersect
    public sealed class Polyline2<T> : ISimplePointCollection2<T>, IEquatable<Polyline2<T>>, IComparable<Polyline2<T>>, IEnumerable<Point2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        internal readonly Point2<T>[] Points;
        internal readonly bool IsReversed;

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

        IEnvelope<T> IGeometry<T>.Envelope
        {
            get
            {
                return this.Envelope;
            }
        }

        IGeometryFactory IGeometry.Factory
        {
            get
            {
                return this.Factory;
            }
        }

        IEnvelope IGeometry.Envelope
        {
            get
            {
                return this.Envelope;
            }
        }

        public Point2<T> this[uint index]
        {
            get
            {
                if (!this.IsReversed)
                    return this.Points[index];
                else
                    return this.Points[this.Points.Length - index++];
            }
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal Polyline2(Point2<T>[] points)
        {
            if (points == null)
                throw new ArgumentNullException();
            if (points.Length < 2)
                throw new ArgumentException();
            this.Points = points;
            this.IsReversed = false;
        }

        public Polyline2(Polyline2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Points = other.Points;
            this.IsReversed = other.IsReversed;
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal Polyline2(Polyline2<T> other, bool isReversed)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Points = other.Points;
            this.IsReversed = isReversed;
        }

        internal Polyline2(Ring2<T> other)
        {
            //this is safe since a ring is simple and closed, therefore a chain will be as well
            //converse is not true -- closing a ring can create a self-intersection
            this.Points = other.Points;
            this.IsReversed = other.IsReversed;
        }

        public PointSet2<T> CoincidentPoints()
        {
            HashSet<Point2<T>> pts = new HashSet<Point2<T>>();
            HashSet<Point2<T>> dups = new HashSet<Point2<T>>();
            for (int i = 1; i < this.Points.Length; i++)
            {
                Point2<T> cur = this.Points[i];
                if (pts.Contains(cur))
                    dups.Add(cur);
                else
                    pts.Add(cur);
            }
            return new PointSet2<T>(dups.ToArray());
        }

        public bool HasCoincidentPoints()
        {//n^2, ouch
            for (int i = 1; i < this.Points.Length; i++)
            {
                Point2<T> cur = this.Points[i];
                for (int j = 0; j < i; j++)
                {
                    if (cur.Equals(this.Points[j]))
                        return true;
                }
            }
            return false;
        }

        public Polyline2<T> Reverse()
        {
            return new Polyline2<T>(this, !this.IsReversed);
        }

        public Point2<T>[] ToArray()
        {
            Point2<T>[] tmp = new Point2<T>[this.Points.Length];
            if (!this.IsReversed)
                Array.Copy(this.Points, tmp, this.Points.Length);
            else
            {
                int max = tmp.Length - 1;
                for (int i = 0; i < tmp.Length; i++)
                {
                    tmp[i] = this.Points[max - i];
                }
            }
            return tmp;
        }

        public int CompareTo(Polyline2<T> other)
        {
            if (other == null)
                return 1;
            if (object.ReferenceEquals(this, other))
                return 0;
            if (object.ReferenceEquals(this.Points, other.Points))
                return 0;
            return this.Envelope.CompareTo(other.Envelope);
        }

        public bool Equals(Polyline2<T> other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true; //cheat for memory eqivalence
            if (object.ReferenceEquals(this.Points, other.Points))
                return true;
            if (this.Points.Length != other.Points.Length)
                return false;
            if (this.IsReversed == other.IsReversed)
            {
                for (int i = 0; i < this.Points.Length; i++)
                {
                    if (!this.Points[i].Equals(other.Points[i]))
                        return false;
                }
            }
            else
            {
                int le = this.Points.Length - 1;
                for (int i = 0; i < this.Points.Length; i++)
                {
                    if (!this.Points[i].Equals(other.Points[le - i]))
                        return false;
                }
            }
            return true;
        }

        public bool EqualsNonDirectional(Polyline2<T> other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true; //cheat for memory eqivalence
            if (this.Points.Length != other.Points.Length)
                return false;
            if (this.Points[0].Equals(other.Points[0])) //forward compare
            {
                for (int i = 0; i < this.Points.Length; i++)
                {
                    if (!this.Points[i].Equals(other.Points[i]))
                        return false;
                }
                return true;
            }
            else if (this.Points[this.Points.Length - 1].Equals(other.Points[0])) //reverse compare
            {
                int id = this.Points.Length - 1;
                for (int i = 0; i < this.Points.Length; i++)
                {
                    if (!this.Points[id - i].Equals(other.Points[i]))
                        return false;
                }
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Polyline2<T>)
                return this.Equals(obj as Polyline2<T>);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Points.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("Points", this.Points);
            sb.Append("IsReversed", this.IsReversed);
            return sb.ToString();
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as Polyline2<T>);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as Polyline2<T>);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as Polyline2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as Polyline2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as Polyline2<T>);
        }

        public IEnumerator<Point2<T>> GetEnumerator()
        {
            return ((IEnumerable<Point2<T>>)this.Points).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Points.GetEnumerator();
        }

        public static implicit operator PointBag2<T>(Polyline2<T> item)
        {
            if (item == null)
                return null;
            return new PointBag2<T>(item.Points);
        }

        public static implicit operator PolylineBag2<T>(Polyline2<T> item)
        {
            if (item == null)
                return null;
            return new PolylineBag2<T>(new Polyline2<T>[]{item});
        }

        public static implicit operator PolylineSet2<T>(Polyline2<T> item)
        {
            if (item == null)
                return null;
            return new PolylineSet2<T>(new Polyline2<T>[] { item });
        }
    }
}
