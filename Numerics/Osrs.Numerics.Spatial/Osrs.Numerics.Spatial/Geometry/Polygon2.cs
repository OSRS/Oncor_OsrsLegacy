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

namespace Osrs.Numerics.Spatial.Geometry
{
    //Single complex polygon -- one outer ring, any number of disjoint inner rings
    public sealed class Polygon2<T> : ISimpleGeometry<Ring2<T>, T>, IGeometry2<T>, INestedPointCollection2<T>, IEquatable<Polygon2<T>>, IComparable<Polygon2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly Ring2<T> OuterRing;
        public readonly RingSet2<T> InnerRings;

        public Inclusivity BoundingType
        {
            get
            {
                return (this.OuterRing.Orientation == Orientation.Counterclockwise ? Inclusivity.Inclusive : Inclusivity.Exclusive);
            }
        }

        public bool IsDisjoint
        {
            get { return false; }
        }

        public bool HasHoles
        {
            get { return this.InnerRings!=null; }
        }

        public uint OuterVertexCount
        {
            get { return (uint)this.OuterRing.Points.Length; }
        }

        public uint VertexCount
        {
            get
            {
                uint ct = (uint)this.OuterRing.Points.Length;
                if (this.InnerRings != null)
                {
                    Ring2<T>[] rings = this.InnerRings.Rings;
                    for (int i = 0; i < rings.Length; i++)
                    {
                        ct += (uint)rings[i].Points.Length;
                    }
                }
                return ct;
            }
        }

        public bool HasEnvelope
        {
            get { return true; }
        }

        public Envelope2<T> Envelope
        {
            get
            {
                return this.OuterRing.Envelope;
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

        /// <summary>
        /// The number of rings in a polygon including the outer ring (SectionCount - 1 == number of holes)
        /// </summary>
        public uint SectionCount
        {
            get { return this.InnerRings == null ? 1 : (uint)this.InnerRings.Rings.Length + 1; }
        }

        public Ring2<T> this[uint index]
        {
            get 
            {
                if (index == 0)
                    return this.OuterRing;
                else if (index > 0)
                {
                    return this.InnerRings.Rings[index - 1];
                }
                throw new IndexOutOfRangeException();
            }
        }

        public Point2<T> this[uint section, uint index]
        {
            get
            {
                if (section == 0)
                    return this.OuterRing.Points[index];
                return this.InnerRings.Rings[section - 1].Points[index];
            }
        }

        internal Polygon2(Ring2<T> outerRing)
        {
            if (outerRing == null)
                throw new ArgumentNullException();
            this.OuterRing = outerRing;
        }

        internal Polygon2(Ring2<T> outerRing, RingSet2<T> innerRings)
        {
            if (outerRing == null)
                throw new ArgumentNullException();
            this.OuterRing = outerRing;
            this.InnerRings = innerRings;
        }

        public Polygon2(Polygon2<T> cloned)
        {
            if (cloned == null)
                throw new ArgumentNullException();
            this.OuterRing = cloned.OuterRing;
            this.InnerRings = cloned.InnerRings;
        }

        public uint SectionVertexCount(uint section)
        {
            if (section == 0)
                return (uint)this.OuterRing.Points.Length;
            return (uint)(this.InnerRings.Rings[section - 1].Points.Length);
        }

        public Polygon2<T> Reverse()
        {
            Ring2<T> o = this.OuterRing.Reverse();
            RingSet2<T> i = this.InnerRings.Reverse();
            return new Polygon2<T>(o, i);
        }

        public PointSet2<T> CoincidentPoints()
        {
            //since each line chain by definition has no coincident points, we only need to compare between chains
            if (this.InnerRings == null)
                return null; //duh, only one chain
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            HashSet<Point2<T>> dupPoints = new HashSet<Point2<T>>();
            Ring2<T> curChain = this.OuterRing;
            foreach (Point2<T> curPoint in curChain.Points)
            {
                distinctPoints.Add(curPoint);
            }

            Ring2<T>[] rings = this.InnerRings.Rings;
            for (int i = 0; i < rings.Length; i++)
            {
                curChain = rings[i];
                foreach (Point2<T> curPoint in curChain.Points)
                {
                    if (distinctPoints.Contains(curPoint))
                        dupPoints.Add(curPoint); //found a dup!
                    else
                        distinctPoints.Add(curPoint);
                }
            }
            if (dupPoints.Count < 1)
                return null;
            return new PointSet2<T>(dupPoints.ToArray()); //avoid overhead checking for dups in constructor
        }

        public bool HasCoincidentPoints()
        {
            if (this.InnerRings == null)
                return false; //duh, only one chain
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            Ring2<T> curChain = this.OuterRing;
            foreach (Point2<T> curPoint in curChain.Points)
            {
                distinctPoints.Add(curPoint);
            }

            Ring2<T>[] rings = this.InnerRings.Rings;
            for (int i = 0; i < rings.Length; i++)
            {
                curChain = rings[i];
                foreach (Point2<T> curPoint in curChain.Points)
                {
                    if (distinctPoints.Contains(curPoint))
                        return true; //found a dup!
                    distinctPoints.Add(curPoint);
                }
            }
            return false;
        }
        
        public Point2<T>[] ToArray()
        {
            Point2<T>[] pts = new Point2<T>[this.VertexCount];
            Array.Copy(this.OuterRing.Points, pts, this.OuterRing.Points.Length);
            if (this.InnerRings == null)
                return pts;
            int i = this.OuterRing.Points.Length;
            Point2<T>[] cur;
            Ring2<T>[] rings = this.InnerRings.Rings;
            for (int j = 0; j < rings.Length; j++)
            {
                cur = rings[j].Points;
                if (!rings[j].IsReversed)
                {
                    Array.Copy(cur, 0, pts, i, cur.Length);
                    i += cur.Length;
                }
                else
                {
                    int max = cur.Length - 1;
                    for (int k = max; k > -1; k--)
                    {
                        pts[i] = cur[k];
                        i++;
                    }
                }
            }
            return pts;
        }

        public Point2<T>[] ToArray(uint section)
        {
            if (section == 0)
                return this.OuterRing.ToArray();
            return this.InnerRings.Rings[section - 1].ToArray();
        }

        public Point2<T>[][] ToArrays()
        {
            Point2<T>[][] pts = new Point2<T>[this.InnerRings.Rings.Length + 1][];
            pts[0] = this.OuterRing.ToArray();
            for (int i = 0; i < this.InnerRings.Rings.Length; i++)
                pts[i - 1] = this.InnerRings.Rings[i].ToArray();
            return pts;
        }

        public int CompareTo(Polygon2<T> other)
        {
            if (other == null)
                return 1;
            if (object.ReferenceEquals(this, other))
                return 0;
            if (object.ReferenceEquals(this.OuterRing, other.OuterRing))
                return 0;
            return this.Envelope.CompareTo(other.Envelope);
        }

        public bool Equals(Polygon2<T> other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (object.ReferenceEquals(this.OuterRing, other.OuterRing))
            {
                if (this.InnerRings == null)
                    return other.InnerRings == null;
                if (other.InnerRings == null)
                    return false;
                if (object.ReferenceEquals(this.InnerRings, other.InnerRings))
                    return true;
            }
            if (this.VertexCount.Equals(other.VertexCount) && this.Envelope.Equals(other.Envelope))
            {
                if (this.OuterRing.Equals(other.OuterRing))
                {
                    if (this.InnerRings == null) //same number of vertices and outer rings are equal -- must both have no inner rings
                        return true;
                    return this.InnerRings.Equals(other.InnerRings);
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return false;
            if (object.ReferenceEquals(this, obj))
                return true;
            if (obj is Polygon2<T>)
            {
                Polygon2<T> other = obj as Polygon2<T>;
                if (other != null)
                {
                    return this.Equals(other);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.OuterRing.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("OuterRing", this.OuterRing.ToString());
            if (this.InnerRings != null)
                sb.Append("InnerRings", this.InnerRings.ToString());
            return sb.ToString();
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as Polygon2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as Polygon2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as Polygon2<T>);
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as Polygon2<T>);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as Polygon2<T>);
        }

        public static explicit operator Ring2<T>(Polygon2<T> item)
        {
            if (item == null)
                return null;
            return item.OuterRing;
        }

        public static implicit operator PolygonSet2<T>(Polygon2<T> item)
        {
            if (item == null)
                return null;
            return new PolygonSet2<T>(new Polygon2<T>[] { item });
        }

        public static implicit operator PolygonBag2<T>(Polygon2<T> item)
        {
            if (item == null)
                return null;
            return new PolygonBag2<T>(new Polygon2<T>[] { item });
        }
    }
}
