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

using Osrs.Collections.Specialized;
using Osrs.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Osrs.Numerics.Spatial.Geometry
{
    public sealed class PolylineBag2<T> : ISimpleGeometry<Polyline2<T>, T>, IMultiPartPointCollection2<T>, IEquatable<PolylineBag2<T>>, IComparable<PolylineBag2<T>>, IEnumerable<Polyline2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        internal readonly Polyline2<T>[] Lines;

        public uint PartCount
        {
            get { return (uint)this.Lines.Length; }
        }

        public uint VertexCount
        {
            get
            {
                uint ct = 0;
                foreach (Polyline2<T> ch in this.Lines)
                    ct += (uint)ch.Points.Length;
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
                    Envelope2<T> cur = this.Lines[0].Envelope;
                    T minX = cur.MinX;
                    T maxX = cur.MaxX;
                    T minY = cur.MinY;
                    T maxY = cur.MaxY;
                    for (int i = 1; i < this.Lines.Length; i++)
                    {
                        cur = this.Lines[i].Envelope;
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

        public Polyline2<T> this[uint index]
        {
            get { return this.Lines[index]; }
        }

        public Point2<T> this[uint part, uint index]
        {
            get { return this.Lines[part][index]; }
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal PolylineBag2(Polyline2<T>[] chains)
        {
            if (chains == null)
                throw new ArgumentNullException();
            this.Lines = chains;
        }

        public PolylineBag2(PolylineBag2<T> cloned)
        {
            if (cloned == null)
                throw new ArgumentNullException();
            this.Lines = cloned.Lines;
        }

        public uint PartVertexCount(uint part)
        {
            return (uint)(this.Lines[part].Points.Length);
        }

        public PointSet2<T> CoincidentPoints()
        {
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            HashSet<Point2<T>> dupPoints = new HashSet<Point2<T>>();
            Polyline2<T> curChain;
            for (int i = 0; i < this.Lines.Length; i++)
            {
                curChain = this.Lines[i];
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
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            Polyline2<T> curChain;
            for (int i = 1; i < this.Lines.Length; i++)
            {
                curChain = this.Lines[i];
                foreach (Point2<T> curPoint in curChain.Points)
                {
                    if (distinctPoints.Contains(curPoint))
                        return true; //found a dup!
                    distinctPoints.Add(curPoint);
                }
            }
            return false;
        }

        public int CompareTo(PolylineBag2<T> other)
        {
            if (other == null)
                return 1;
            if (object.ReferenceEquals(this, other))
                return 0;
            if (object.ReferenceEquals(this.Lines, other.Lines))
                return 0;
            return this.Envelope.CompareTo(other.Envelope);
        }

        public bool Equals(PolylineBag2<T> other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (object.ReferenceEquals(this.Lines, other.Lines))
                return true;
            if (this.VertexCount.Equals(other.VertexCount) && this.Envelope.Equals(other.Envelope))
            {
                Polyline2<T> us;
                Polyline2<T> them;
                for (int i = 0; i < this.Lines.Length; i++)
                {
                    us = this.Lines[i];
                    them = other.Lines[i];
                    if (us.Points.Length != them.Points.Length)
                        return false; //can't be the same
                    for (int j = 0; j < us.Points.Length; j++)
                    {
                        if (!us.Points[j].Equals(them.Points[j]))
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        public Point2<T>[] ToArray()
        {
            LeakyResizableArray<Point2<T>> pts = new LeakyResizableArray<Point2<T>>();
            foreach (Polyline2<T> ch in this.Lines)
            {
                foreach (Point2<T> pt in ch.Points)
                    pts.Add(pt);
            }
            pts.TrimExcess();
            return pts.Data;
        }

        public Point2<T>[] ToArray(uint part)
        {
            return this.Lines[part].ToArray();
        }

        public Point2<T>[][] ToArrays()
        {
            Point2<T>[][] tmp = new Point2<T>[this.Lines.Length][];
            for (int i = 0; i < this.Lines.Length; i++)
            {
                tmp[i] = this.Lines[i].ToArray();
            }
            return tmp;
        }

        public override bool Equals(object obj)
        {
            if (obj is PolylineBag2<T>)
                return this.Equals(obj as PolylineBag2<T>);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Lines.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("Lines", this.Lines);
            return sb.ToString();
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as PolylineBag2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as PolylineBag2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as PolylineBag2<T>);
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as PolylineBag2<T>);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as PolylineBag2<T>);
        }

        public IEnumerator<Polyline2<T>> GetEnumerator()
        {
            return ((IEnumerable<Polyline2<T>>)this.Lines).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Lines.GetEnumerator();
        }
    }
}
