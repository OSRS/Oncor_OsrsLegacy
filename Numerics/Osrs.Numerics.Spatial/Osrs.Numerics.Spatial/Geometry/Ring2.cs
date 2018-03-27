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
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Osrs.Numerics.Spatial.Geometry
{
    //ring is oriented - either clockwise or counter -- noted by read order of the array to allow sharing of array for different windings
    //single, simple polygon (no holes) - one ring
    public sealed class Ring2<T> : ISimpleGeometry<Point2<T>, T>, IGeometry2<T>, IEquatable<Ring2<T>>, IComparable<Ring2<T>>, IEnumerable<Point2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        internal readonly Point2<T>[] Points;  //the points
        public readonly bool IsReversed;     //the order of points in the ring -- if false, then 0-->N-->0 (wrap), otherwise N-->0-->N (wrap)
        public readonly Orientation Orientation; //orientation of the ring read in order by IsReversed - clockwise or counterclockwise.

        public Inclusivity BoundingType
        {
            get 
            {
                return (this.Orientation == Orientation.Counterclockwise ? Inclusivity.Inclusive : Inclusivity.Exclusive);
            }
        }

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

        public bool HasEnvelope
        {
            get { return true; }
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

        public Point2<T> this[uint section, uint index]
        {
            get
            {
                if (section == 0)
                    return this.Points[index];
                throw new IndexOutOfRangeException();
            }
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal Ring2(Point2<T>[] points) : this(points, false, Orientation.Counterclockwise)
        {
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal Ring2(Point2<T>[] points, Orientation orientation) : this(points, false, Orientation.Counterclockwise)
        {
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal Ring2(Point2<T>[] points, bool isReversed) : this(points, isReversed, Orientation.Counterclockwise)
        {
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal Ring2(Point2<T>[] points, bool isReversed, Orientation orientation)
        {
            if (points == null)
                throw new ArgumentNullException();
            if (points.Length < 3)
                throw new ArgumentException();
            this.Points = points;
            this.IsReversed = isReversed;
            this.Orientation = orientation;
        }

        public Ring2(Ring2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Points = other.Points;
            this.IsReversed = other.IsReversed;
            this.Orientation = other.Orientation;
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal Ring2(Ring2<T> other, bool isReversed, Orientation orientation)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Points = other.Points;
            this.IsReversed = isReversed;
            this.Orientation = Orientation.Counterclockwise;
        }

        //rings are by definition simple, closed and have no self-intersections - so they cannot have coincident points
        public PointSet2<T> CoincidentPoints()
        {
            return null;
        }

        public bool HasCoincidentPoints()
        {
            return false;
        }

        public Ring2<T> Reverse()
        {
            return new Ring2<T>(this, !this.IsReversed, (this.Orientation == Orientation.Counterclockwise ? Orientation.Clockwise : Orientation.Counterclockwise));
        }

        public Point2<T>[] ToArray()
        {
            Point2<T>[] tmp = new Point2<T>[this.Points.Length];
            if (!this.IsReversed)
                Array.Copy(this.Points, tmp, this.Points.Length);
            else
            {
                int max=tmp.Length-1;
                for (int i = 0; i < tmp.Length; i++)
                {
                    tmp[i] = this.Points[max - i];
                }
            }
            return tmp;
        }

        public Point2<T>[] ToArray(uint section)
        {
            if (section == 0)
                return this.Points.ToArray();
            throw new IndexOutOfRangeException();
        }

        public Point2<T>[][] ToArrays()
        {
            return new Point2<T>[][] { this.Points.ToArray() };
        }

        public int CompareTo(Ring2<T> other)
        {
            if (other == null)
                return 1;
            if (object.ReferenceEquals(this, other))
                return 0;
            if (object.ReferenceEquals(this.Points, other.Points))
                return 0;
            return this.Envelope.CompareTo(other.Envelope);
        }

        public bool Equals(Ring2<T> other)
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

        public bool EqualsNonDirectional(Ring2<T> other)
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
            if (obj is Ring2<T>)
                return this.Equals(obj as Ring2<T>);
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

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as Ring2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as Ring2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as Ring2<T>);
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as Ring2<T>);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as Ring2<T>);
        }

        public IEnumerator<Point2<T>> GetEnumerator()
        {
            return ((IEnumerable<Point2<T>>)this.Points).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Points.GetEnumerator();
        }

        public static implicit operator CoordinateString2<T>(Ring2<T> item)
        {
            if (item == null)
                return null;
            Coordinate2<T>[] pts = new Coordinate2<T>[item.Points.Length];
            for (int i = 0; i < pts.Length; i++)
                pts[i] = item.Points[i];
            return new CoordinateString2<T>(pts);
        }

        public static implicit operator LineChain2<T>(Ring2<T> item)
        {
            if (item == null)
                return null;
            return new LineChain2<T>(item);
        }

        public static implicit operator PointSet2<T>(Ring2<T> item)
        {
            if (item == null)
                return null;
            return new PointSet2<T>(item.Points);
        }

        public static implicit operator PointBag2<T>(Ring2<T> item)
        {
            if (item == null)
                return null;
            return new PointBag2<T>(item.Points);
        }

        public static implicit operator Polygon2<T>(Ring2<T> item)
        {
            if (item == null)
                return null;
            return new Polygon2<T>(item);
        }

        public static implicit operator LineChainBag2<T>(Ring2<T> item)
        {
            if (item == null)
                return null;
            return new LineChainBag2<T>(new LineChain2<T>[] { item });
        }

        public static implicit operator LineChainSet2<T>(Ring2<T> item)
        {
            if (item == null)
                return null;
            return new LineChainSet2<T>(new LineChain2<T>[] { item });
        }

        public static implicit operator RingBag2<T>(Ring2<T> item)
        {
            if (item == null)
                return null;
            return new RingBag2<T>(new Ring2<T>[] { item });
        }

        public static implicit operator RingSet2<T>(Ring2<T> item)
        {
            if (item == null)
                return null;
            return new RingSet2<T>(new Ring2<T>[] { item });
        }
    }
}
