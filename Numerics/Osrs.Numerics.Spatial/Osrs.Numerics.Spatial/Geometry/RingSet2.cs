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
    //multiple simple polygons -- only outer rings, all disjoint - no overlaps
    //rings CAN intersect at any number of non-adjacent points, no linear intersections
    public sealed class RingSet2<T> : ISimpleGeometry<Ring2<T>, T>, IMultiPartPointCollection2<T>, IEquatable<RingSet2<T>>, IComparable<RingSet2<T>>, IEnumerable<Ring2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        internal readonly Ring2<T>[] Rings;

        public uint PartCount
        {
            get { return (uint)this.Rings.Length; }
        }

        public uint VertexCount
        {
            get
            {
                uint ct = 0;
                for (int i = 0; i < this.Rings.Length; i++)
                {
                    ct += (uint)(this.Rings[i].Points.Length);
                }
                return ct;
            }
        }

        private Envelope2<T> envelope;
        public Envelope2<T> Envelope
        {
            get
            {
                if (this.envelope == null)
                {
                    Envelope2<T> cur = this.Rings[0].Envelope;
                    T minX = cur.MinX;
                    T maxX = cur.MaxX;
                    T minY = cur.MinY;
                    T maxY = cur.MaxY;
                    for (int i = 1; i < this.Rings.Length; i++)
                    {
                        cur = this.Rings[i].Envelope;
                        if (minX.CompareTo(cur.MinX) > 0)
                            minX = cur.MinX;
                        if (maxX.CompareTo(cur.MaxX) < 0)
                            maxX = cur.MaxX;
                        if (minY.CompareTo(cur.MinY) > 0)
                            minY = cur.MinY;
                        if (maxY.CompareTo(cur.MaxY) < 0)
                            maxY = cur.MaxY;
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

        public Ring2<T> this[uint index]
        {
            get { return this.Rings[index]; }
        }

        public Point2<T> this[uint part, uint index]
        {
            get { return this.Rings[part][index]; }
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal RingSet2(Ring2<T>[] chains)
        {
            if (chains == null)
                throw new ArgumentNullException();
            if (chains.Length < 1)
                throw new ArgumentException();
            this.Rings = chains;
        }

        public RingSet2(RingSet2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Rings = other.Rings;
        }

        public uint PartVertexCount(uint part)
        {
            return (uint)(this.Rings[part].Points.Length);
        }

        public PointSet2<T> CoincidentPoints()
        {
            //since each line ring by definition has no coincident points, we only need to compare between rings
            if (this.Rings.Length < 2)
                return null; //duh, only one ring
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            HashSet<Point2<T>> dupPoints = new HashSet<Point2<T>>();
            Ring2<T> curChain = this.Rings[0];
            foreach (Point2<T> curPoint in curChain.Points)
            {
                distinctPoints.Add(curPoint);
            }

            for (int i = 1; i < this.Rings.Length; i++)
            {
                curChain = this.Rings[i];
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
            if (this.Rings.Length < 2)
                return false; //duh, only one chain
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            Ring2<T> curChain = this.Rings[0];
            foreach (Point2<T> curPoint in curChain.Points)
            {
                distinctPoints.Add(curPoint);
            }

            for (int i = 1; i < this.Rings.Length; i++)
            {
                curChain = this.Rings[i];
                foreach (Point2<T> curPoint in curChain.Points)
                {
                    if (distinctPoints.Contains(curPoint))
                        return true; //found a dup!
                    distinctPoints.Add(curPoint);
                }
            }
            return false;
        }

        public RingSet2<T> Reverse()
        {
            Ring2<T>[] r = new Ring2<T>[this.Rings.Length];
            for (int i = 0; i < r.Length; i++)
                r[i] = this.Rings[i].Reverse();
            return new RingSet2<T>(r);
        }

        public Point2<T>[] ToArray()
        {
            Point2<T>[] pts = new Point2<T>[this.VertexCount];
            int i = 0;
            Point2<T>[] cur;
            for (int j = 0; j < this.Rings.Length; j++)
            {
                cur = this.Rings[j].Points;
                if (!this.Rings[j].IsReversed)
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

        public Point2<T>[] ToArray(uint part)
        {
            return this.Rings[part].ToArray();
        }

        public Point2<T>[][] ToArrays()
        {
            Point2<T>[][] pts = new Point2<T>[this.Rings.Length][];
            for (int j = 0; j < this.Rings.Length; j++)
            {
                pts[j] = this.Rings[j].ToArray();
            }
            return pts;
        }

        public int CompareTo(RingSet2<T> other)
        {
            if (other == null)
                return 1;
            if (object.ReferenceEquals(this, other))
                return 0;
            if (object.ReferenceEquals(this.Rings, other.Rings))
                return 0;
            return this.Envelope.CompareTo(other.Envelope);
        }

        public bool Equals(RingSet2<T> other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (object.ReferenceEquals(this.Rings, other.Rings))
                return true;
            if (this.Rings.Length == other.Rings.Length && this.VertexCount.Equals(other.VertexCount) && this.Envelope.Equals(other.Envelope))
            {
                Ring2<T> us;
                Ring2<T> them;
                bool match=false;
                for (int i = 0; i < this.Rings.Length; i++)
                {
                    us = this.Rings[i];
                    for (int j = 0; j < other.Rings.Length; j++)
                    {
                        them = other.Rings[j];
                        if (us.Equals(them))
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                        return false;
                    match = false;
                }
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return false;
            if (object.ReferenceEquals(this, obj))
                return true;
            if (obj is RingSet2<T>)
            {
                RingSet2<T> other = obj as RingSet2<T>;
                if (other != null && this.Rings.Length == other.Rings.Length)
                {
                    Ring2<T> curUs;
                    Ring2<T> curThem;
                    bool match;
                    for (int i = 0; i < this.Rings.Length;i++ )
                    {
                        match = false;
                        curUs = this.Rings[i];
                        for (int j = 0; j < other.Rings.Length; j++)
                        {
                            curThem = other.Rings[j];
                            if (curThem.Equals(curUs))
                            {
                                match = true;
                                break; //no point checking more
                            }
                        }
                        if (!match)
                            return false; //didn't find a chain
                    }
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Rings.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("Rings", this.Rings);
            return sb.ToString();
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as RingSet2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as RingSet2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as RingSet2<T>);
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as RingSet2<T>);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as RingSet2<T>);
        }

        public IEnumerator<Ring2<T>> GetEnumerator()
        {
            return ((IEnumerable<Ring2<T>>)this.Rings).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Rings.GetEnumerator();
        }

        public static implicit operator RingBag2<T>(RingSet2<T> item)
        {
            if (item == null)
                return null;
            return new RingBag2<T>(item.Rings);
        }
    }
}
