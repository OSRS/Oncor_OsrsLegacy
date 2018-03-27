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
    public sealed class PolygonBag2<T> : ISimpleGeometry<Polygon2<T>, T>, IGeometry2<T>, IMultiPartNestedPointCollection2<T>, IEquatable<PolygonBag2<T>>, IComparable<PolygonBag2<T>>, IEnumerable<Polygon2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        internal readonly Polygon2<T>[] Polygons;

        public uint PartCount
        {
            get { return (uint)this.Polygons.Length; }
        }

        private Envelope2<T> envelope;
        public Envelope2<T> Envelope
        {
            get 
            {
                if (this.envelope == null)
                {
                    Envelope2<T>[] envs = new Envelope2<T>[this.Polygons.Length];
                    for (int i = 0; i < envs.Length; i++)
                    {
                        envs[i] = this.Polygons[i].Envelope;
                    }
                    this.envelope = Envelope2<T>.Bound(envs);
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

        public uint VertexCount
        {
            get 
            {
                uint ct = 0;
                foreach (Polygon2<T> po in this.Polygons)
                {
                    ct += po.VertexCount;
                }
                return ct;
            }
        }

        public bool HasEnvelope
        {
            get { return true; }
        }

        public Polygon2<T> this[uint index]
        {
            get { return this.Polygons[index]; }
        }

        public Point2<T> this[uint part, uint section, uint index]
        {
            get 
            {
                Polygon2<T> pt = this.Polygons[part];
                return pt[section, index];
            }
        }

        internal PolygonBag2(Polygon2<T>[] polygons)
        {
            if (polygons == null)
                throw new ArgumentNullException();
            if (polygons.Length < 1)
                throw new ArgumentException();
            this.Polygons = polygons;
        }

        public PolygonBag2(PolygonBag2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Polygons = other.Polygons;
        }

        public uint PartVertexCount(uint part)
        {
            return this.Polygons[part].VertexCount;
        }

        public uint SectionCount()
        {
            uint ct=0;
            foreach (Polygon2<T> po in this.Polygons)
                ct += po.SectionCount;
            return ct;
        }

        public uint SectionCount(uint part)
        {
            return this.Polygons[part].SectionCount;
        }

        public uint SectionVertexCount(uint part, uint section)
        {
            return this.Polygons[part].SectionVertexCount(section);
        }

        public PointSet2<T> CoincidentPoints()
        {
            if (this.Polygons.Length < 2)
                return this.Polygons[0].CoincidentPoints(); //duh, only one chain
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            HashSet<Point2<T>> dupPoints = new HashSet<Point2<T>>();
            Polygon2<T> curChain;
            for (int i = 1; i < this.Polygons.Length; i++)
            {
                curChain = this.Polygons[i];
                foreach (Point2<T> curPoint in curChain.OuterRing.Points)
                {
                    if (distinctPoints.Contains(curPoint))
                        dupPoints.Add(curPoint); //found a dup!
                    else
                        distinctPoints.Add(curPoint);
                }
                if (curChain.InnerRings != null)
                {
                    foreach (Ring2<T> iRing in curChain.InnerRings.Rings)
                    {
                        foreach (Point2<T> curPoint in iRing.Points)
                        {
                            if (distinctPoints.Contains(curPoint))
                                dupPoints.Add(curPoint); //found a dup!
                            else
                                distinctPoints.Add(curPoint);
                        }
                    }
                }
            }
            if (dupPoints.Count < 1)
                return null;
            return new PointSet2<T>(dupPoints.ToArray());
        }

        public bool HasCoincidentPoints()
        {
            if (this.Polygons.Length < 2)
                return this.Polygons[0].HasCoincidentPoints(); //duh, only one chain
            HashSet<Point2<T>> distinctPoints = new HashSet<Point2<T>>();
            Polygon2<T> curChain;
            for (int i = 1; i < this.Polygons.Length; i++)
            {
                curChain = this.Polygons[i];
                foreach (Point2<T> curPoint in curChain.OuterRing.Points)
                {
                    if (distinctPoints.Contains(curPoint))
                        return true; //found a dup!
                    distinctPoints.Add(curPoint);
                }
                if (curChain.InnerRings != null)
                {
                    foreach (Ring2<T> iRing in curChain.InnerRings.Rings)
                    {
                        foreach (Point2<T> curPoint in iRing.Points)
                        {
                            if (distinctPoints.Contains(curPoint))
                                return true; //found a dup!
                            distinctPoints.Add(curPoint);
                        }
                    }
                }
            }
            return false;
        }

        public Point2<T>[] ToArray()
        {
            Point2<T>[] pts = new Point2<T>[this.VertexCount];
            int i = 0;
            foreach (Polygon2<T> po in this.Polygons)
            {
                Array.Copy(po.OuterRing.Points, 0, pts, i, po.OuterRing.Points.Length);
                i += po.OuterRing.Points.Length;
                if (po.InnerRings == null)
                    continue;
                Point2<T>[] cur;
                Ring2<T>[] rings = po.InnerRings.Rings;
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
            }
            return pts;
        }

        public Point2<T>[] ToArray(uint part, uint section)
        {
            return this.Polygons[part].ToArray(section);
        }

        public Point2<T>[][] ToArray(uint part)
        {
            return this.Polygons[part].ToArrays();
        }

        public Point2<T>[][][] ToArrays()
        {
            Point2<T>[][][] pts = new Point2<T>[this.Polygons.Length][][];
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = this.Polygons[i].ToArrays();
            }
            return pts;
        }

        public bool Equals(PolygonBag2<T> other)
        {
            if (other == null)
                return false;
            if (object.ReferenceEquals(this, other))
                return true;
            if (object.ReferenceEquals(this.Polygons, other.Polygons))
                return true;
            if (this.Polygons.Length == other.Polygons.Length && this.VertexCount.Equals(other.VertexCount) && this.Envelope.Equals(other.Envelope))
            {
                Polygon2<T> us;
                Polygon2<T> them;
                bool match = false;
                for (int i = 0; i < this.Polygons.Length; i++)
                {
                    us = this.Polygons[i];
                    for (int j = 0; j < other.Polygons.Length; j++)
                    {
                        them = other.Polygons[j];
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

        public int CompareTo(PolygonBag2<T> other)
        {
            if (other == null)
                return 1;
            if (object.ReferenceEquals(this, other))
                return 0;
            if (object.ReferenceEquals(this.Polygons, other.Polygons))
                return 0;
            return this.Envelope.CompareTo(other.Envelope);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is PolygonBag2<T>)
                return this.Equals(obj as PolygonBag2<T>);
            return false;
        }

        public override int GetHashCode()
        {
            return this.Polygons.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("Polygons", this.Polygons);
            return sb.ToString();
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as PolygonBag2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as PolygonBag2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as PolygonBag2<T>);
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as PolygonBag2<T>);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as PolygonBag2<T>);
        }

        public IEnumerator<Polygon2<T>> GetEnumerator()
        {
            return ((IEnumerable<Polygon2<T>>)this.Polygons).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Polygons.GetEnumerator();
        }

        public static explicit operator PointBag2<T>(PolygonBag2<T> item)
        {
            if (item == null)
                return null;
            return new PointBag2<T>(item.ToArray()); //ouch-that may be big
        }
    }
}
