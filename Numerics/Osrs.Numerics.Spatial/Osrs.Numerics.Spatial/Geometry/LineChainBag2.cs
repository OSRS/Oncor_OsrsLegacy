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
    //Polyline -- collection of Line chains
    //non-simple - contained chains can overlap in any way
    public sealed class LineChainBag2<T> : ISimpleGeometry<LineChain2<T>,T>, IMultiPartPointCollection2<T>, IEquatable<LineChainBag2<T>>, IComparable<LineChainBag2<T>>, IEnumerable<LineChain2<T>>
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
                foreach (LineChain2<T> ch in this.Chains)
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

        public LineChain2<T> this[uint index]
        {
            get { return this.Chains[index]; }
        }

        public Point2<T> this[uint part, uint index]
        {
            get { return this.Chains[part][index]; }
        }

        //Trusted constructor -- uses array directly -- no nulls!
        internal LineChainBag2(LineChain2<T>[] chains)
        {
            if (chains == null)
                throw new ArgumentNullException();
            this.Chains = chains;
        }

        public LineChainBag2(LineChainBag2<T> cloned)
        {
            if (cloned == null)
                throw new ArgumentNullException();
            this.Chains = cloned.Chains;
        }

        public uint PartVertexCount(uint part)
        {
            return (uint)this.Chains[part].Points.Length;
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

        public int CompareTo(LineChainBag2<T> other)
        {
            if (other == null)
                return 1;
            if (object.ReferenceEquals(this, other))
                return 0;
            if (object.ReferenceEquals(this.Chains, other.Chains))
                return 0;
            return this.Envelope.CompareTo(other.Envelope);
        }

        public bool Equals(LineChainBag2<T> other)
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
            Point2<T>[][] tmp = new Point2<T>[this.Chains.Length][];
            for (int i = 0; i < this.Chains.Length; i++)
            {
                tmp[i] = this.Chains[i].ToArray();
            }
            return tmp;
        }

        public override bool Equals(object obj)
        {
            if (obj is LineChainBag2<T>)
                return this.Equals(obj as LineChainBag2<T>);
            return base.Equals(obj);
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
            return this.CompareTo(obj as LineChainBag2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as LineChainBag2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as LineChainBag2<T>);
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as LineChainBag2<T>);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as LineChainBag2<T>);
        }

        public IEnumerator<LineChain2<T>> GetEnumerator()
        {
            return ((IEnumerable<LineChain2<T>>)this.Chains).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Chains.GetEnumerator();
        }
    }
}
