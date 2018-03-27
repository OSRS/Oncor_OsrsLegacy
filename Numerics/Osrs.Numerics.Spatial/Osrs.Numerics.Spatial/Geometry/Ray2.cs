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

namespace Osrs.Numerics.Spatial.Geometry
{
    public sealed class Ray2<T> : IGeometry2<T>, IEquatable<Ray2<T>>, IComparable<Ray2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly Point2<T> Start;
        public readonly Point2<T> OrientationPoint;

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

        internal Ray2(Point2<T> start, Point2<T> orientationPoint)
        {
            if (start == null || orientationPoint == null)
                throw new ArgumentNullException();
            if (start.Equals(orientationPoint))
                throw new DegenerateCaseException();
            this.Start = start;
            this.OrientationPoint = orientationPoint;
        }

        public Ray2(Ray2<T> cloned)
        {
            if (cloned == null)
                throw new ArgumentNullException();
            this.Start = cloned.Start;
            this.OrientationPoint = cloned.OrientationPoint;
        }

        public bool Equals(Ray2<T> other)
        {
            if (other == null)
                return false;
            return this.Start.Equals(other.Start) && this.OrientationPoint.Equals(other.OrientationPoint);
        }

        public int CompareTo(Ray2<T> other)
        {
            int bas = this.Start.CompareTo(other.Start);
            if (bas!=0)
                return bas;
            return this.OrientationPoint.CompareTo(other.OrientationPoint);
        }

        public override bool Equals(object obj)
        {
            if (obj is Ray2<T>)
                return this.Equals(obj as Ray2<T>);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Start.GetHashCode() ^ this.OrientationPoint.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("Start", this.Start.ToString());
            sb.Append("OrientationPoint", this.OrientationPoint.ToString());
            return sb.ToString();
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as Ray2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as Ray2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as Ray2<T>);
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as Ray2<T>);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as Ray2<T>);
        }

        public static bool operator ==(Ray2<T> a, Ray2<T> b)
        {
            if (Object.ReferenceEquals(a, null))
                return Object.ReferenceEquals(b, null);
            if (Object.ReferenceEquals(b, null))
                return false;
            return a.Start.Equals(b.Start) && a.OrientationPoint.Equals(b.OrientationPoint);
        }

        public static bool operator !=(Ray2<T> a, Ray2<T> b)
        {
            return !(a == b);
        }

        public Envelope2<T> Envelope
        {
            get { return null; }
        }

        public uint VertexCount
        {
            get { return 1; }
        }

        public bool HasEnvelope
        {
            get { return false; }
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
    }
}
