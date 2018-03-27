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

using Osrs.Runtime;
using Osrs.Text;
using System;

namespace Osrs.Numerics.Spatial.Geometry
{
    public sealed class Line2<T> : IGeometry2<T>, IEquatable<Line2<T>>, IComparable<Line2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly Point2<T> Anchor;
        public readonly Point2<T> OrientationPoint;

        public uint VertexCount
        {
            get { return 0; }
        }

        public bool HasEnvelope
        {
            get { return false; }
        }

        public Envelope2<T> Envelope
        {
            get
            {
                return null;
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

        internal Line2(Point2<T> anchor, Point2<T> orientationPoint)
        {
            if (anchor == null || orientationPoint == null)
            {
                MethodContract.NotNull(anchor, nameof(anchor));
                MethodContract.NotNull(orientationPoint, nameof(orientationPoint));
            }
            if (anchor.Equals(orientationPoint))
                throw new DegenerateCaseException();
            this.Anchor = anchor;
            this.OrientationPoint = orientationPoint;
        }

        public Line2(Line2<T> cloned)
        {
            if (cloned == null)
                throw new ArgumentNullException();
            this.Anchor = cloned.Anchor;
            this.OrientationPoint = cloned.OrientationPoint;
        }

        public bool Equals(Line2<T> other)
        {
            if (other == null)
                return false;
            return this.Anchor.Equals(other.Anchor) && this.OrientationPoint.Equals(other.OrientationPoint);
        }

        public bool Equals(IGeometry<T> other)
        {
            return this.Equals(other as Line2<T>);
        }

        public bool Equals(IGeometry other)
        {
            return this.Equals(other as Line2<T>);
        }

        public int CompareTo(Line2<T> other)
        {
            int bas = this.Anchor.CompareTo(other.Anchor);
            if (bas != 0)
                return bas;
            return this.OrientationPoint.CompareTo(other.OrientationPoint);
        }

        public int CompareTo(IGeometry<T> other)
        {
            return this.CompareTo(other as Line2<T>);
        }

        public int CompareTo(IGeometry other)
        {
            return this.CompareTo(other as Line2<T>);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as Line2<T>);
        }

        public override bool Equals(object obj)
        {
            if (obj is Line2<T>)
                return this.Equals(obj as Line2<T>);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Anchor.GetHashCode() ^ this.OrientationPoint.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("Anchor", this.Anchor.ToString());
            sb.Append("OrientationPoint", this.OrientationPoint.ToString());
            return sb.ToString();
        }

        public static bool operator ==(Line2<T> a, Line2<T> b)
        {
            if (Object.ReferenceEquals(a, null))
                return Object.ReferenceEquals(b, null);
            if (Object.ReferenceEquals(b, null))
                return false;
            return a.Anchor.Equals(b.Anchor) && a.OrientationPoint.Equals(b.OrientationPoint);
        }

        public static bool operator !=(Line2<T> a, Line2<T> b)
        {
            return !(a == b);
        }
    }
}
