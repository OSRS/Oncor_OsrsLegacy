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
using System.Text;
using System.Collections;

namespace Osrs.Numerics.Spatial.Geometry
{
    public sealed class LineChainSet2<T> : ISimpleGeometry<LineChain2<T>,T>, IMultiPartPointCollection2<T>, IEquatable<LineChainSet2<T>>, IComparable<LineChainSet2<T>>, IEnumerable<LineChain2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        internal readonly LineChain2<T>[] Chains;

        public uint PartCount
        {
            get { return (uint)this.Chains.Length; }
        }

        public uint VertexCount
        {
            get
            {
                uint ct = 0;
                for (int i = 0; i < this.Chains.Length; i++)
                {
                    ct += (uint)this.Chains[i].Points.Length;
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
                    Envelope2<T> cur = this.Chains[0].Envelope;
                    T minX = cur.MinX;
                    T maxX = cur.MaxX;
                    T minY = cur.MinY;
                    T maxY = cur.MaxY;
                    for (int i = 1; i < this.Chains.Length; i++)
                    {
                        cur = this.Chains[i].Envelope;
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

        IGeometryFactory IGeometry.Factory
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

        public LineChain2<T> this[uint index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public LineChain2<T> this[int index]
        {
            get { return this.Chains[index]; }
        }

        public Point2<T> this[uint part, uint index]
        {
            get { return this.Chains[part][index]; }
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal LineChainSet2(LineChain2<T>[] chains)
        {
            if (chains == null)
                throw new ArgumentNullException();
            if (chains.Length < 1)
                throw new ArgumentException();
            this.Chains = chains;
        }

        public LineChainSet2(LineChainSet2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Chains = other.Chains;
        }

        public uint PartVertexCount(uint part)
        {
            return (uint)(this.Chains[part].Points.Length);
        }

        public PointSet2<T> CoincidentPoints()
        {
            //since each line chain by definition has no coincident points, we only need to compare between chains
            if (this.Chains.Length < 2)
                return null; //duh, only one chain
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            HashSet<Point2<T>> dupPoints = new HashSet<Point2<T>>();
            LineChain2<T> curChain = this.Chains[0];
            foreach (Point2<T> curPoint in curChain.Points)
            {
                distinctPoints.Add(curPoint);
            }

            for (int i = 1; i < this.Chains.Length; i++)
            {
                curChain = this.Chains[i];
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
            if (this.Chains.Length < 2)
                return false; //duh, only one chain
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            LineChain2<T> curChain = this.Chains[0];
            foreach (Point2<T> curPoint in curChain.Points)
            {
                distinctPoints.Add(curPoint);
            }

            for (int i = 1; i < this.Chains.Length; i++)
            {
                curChain = this.Chains[i];
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
            int i = 0;
            Point2<T>[] cur;
            for (int j = 0; j < this.Chains.Length; j++)
            {
                cur = this.Chains[j].Points;
                if (!this.Chains[j].IsReversed)
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
            return this.Chains[part].ToArray();
        }

        public Point2<T>[][] ToArrays()
        {
            Point2<T>[][] pts = new Point2<T>[this.Chains.Length][];
            for (int j = 0; j < this.Chains.Length; j++)
            {
                pts[j] = this.Chains[j].ToArray();
            }
            return pts;
        }

        public int CompareTo(LineChainSet2<T> other)
        {
            if (other == null)
                return 1;
            if (object.ReferenceEquals(this, other))
                return 0;
            if (object.ReferenceEquals(this.Chains, other.Chains))
                return 0;
            return this.Envelope.CompareTo(other.Envelope);
        }

        public bool Equals(LineChainSet2<T> other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (object.ReferenceEquals(this.Chains, other.Chains))
                return true;
            if (this.VertexCount.Equals(other.VertexCount) && this.Envelope.Equals(other.Envelope))
            {
                LineChain2<T> us;
                LineChain2<T> them;
                for (int i = 0; i < this.Chains.Length; i++)
                {
                    us = this.Chains[i];
                    them = other.Chains[i];
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

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return false;
            if (object.ReferenceEquals(this, obj))
                return true;
            if (obj is LineChainSet2<T>)
            {
                LineChainSet2<T> other = obj as LineChainSet2<T>;
                if (other != null && this.Chains.Length == other.Chains.Length)
                {
                    LineChain2<T> curUs;
                    LineChain2<T> curThem;
                    bool match;
                    for (int i = 0; i < this.Chains.Length;i++ )
                    {
                        match = false;
                        curUs = this.Chains[i];
                        for (int j = 0; j < other.Chains.Length; j++)
                        {
                            curThem = other.Chains[j];
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
            return this.Chains.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("Chains", this.Chains);
            return sb.ToString();
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as LineChainSet2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as LineChainSet2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as LineChainSet2<T>);
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as LineChainSet2<T>);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as LineChainSet2<T>);
        }

        public IEnumerator<LineChain2<T>> GetEnumerator()
        {
            return ((IEnumerable<LineChain2<T>>)this.Chains).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Chains.GetEnumerator();
        }

        public static implicit operator LineChainBag2<T>(LineChainSet2<T> item)
        {
            if (item == null)
                return null;
            return new LineChainBag2<T>(item.Chains);
        }
    }
}
