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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial
{
    public sealed class Envelope2<T> : IEnvelope<T>, IComparable<Envelope2<T>>, IEquatable<Envelope2<T>>
        where T : IComparable<T>, IEquatable<T>
    {
        public readonly T MinX;
        public readonly T MinY;
        public readonly T MaxX;
        public readonly T MaxY;

        public Envelope2(T minX, T minY, T maxX, T maxY)
        {
            if (minX.CompareTo(maxX) >= 0)
                throw new DegenerateCaseException();
            if (minY.CompareTo(maxY) >= 0)
                throw new DegenerateCaseException();
            this.MinX = minX;
            this.MaxX = maxX;
            this.MinY = minY;
            this.MaxY = maxY;
        }

        public Envelope2(Envelope2<T> cloned)
        {
            if (cloned == null)
                throw new ArgumentNullException();
            this.MinX = cloned.MinX;
            this.MaxX = cloned.MaxX;
            this.MinY = cloned.MinY;
            this.MaxY = cloned.MaxY;
        }

        public int CompareTo(Envelope2<T> other)
        {
            if (other == null)
                return 1;
            if (this.MinX.CompareTo(other.MaxX) > 0)
                return 1;
            if (this.MaxX.CompareTo(other.MinX) < 0)
                return -1;
            //x overlaps
            int res = this.MinX.CompareTo(other.MinX);
            if (res != 0)
                return res;
            res = this.MaxX.CompareTo(other.MaxX);
            if (res != 0)
                return res;
            res = this.MinY.CompareTo(other.MinY);
            if (res != 0)
                return res;
            return this.MaxY.CompareTo(other.MaxY);
        }

        public bool Equals(Envelope2<T> other)
        {
            if (other == null)
                return false;
            return this.MinX.Equals(other.MinX) && this.MinY.Equals(other.MinY) && this.MaxX.Equals(other.MaxX) && this.MaxY.Equals(other.MaxY);
        }

        public static Envelope2<T> Bound(Envelope2<T> a, Envelope2<T> b)
        {
            if (a == null)
                return b;
            else if (b == null)
                return a;

            T minX = a.MinX;
            T maxX = a.MaxX;
            T minY = a.MinY;
            T maxY = a.MaxY;

            if (minX.CompareTo(b.MinX) > 0)
                minX = b.MinX;
            if (minY.CompareTo(b.MinY) > 0)
                minY = b.MinY;
            if (maxX.CompareTo(b.MaxX) < 0)
                maxX = b.MaxX;
            if (maxY.CompareTo(b.MaxY) < 0)
                maxY = b.MaxY;
            return new Envelope2<T>(minX, minY, maxX, maxY);
        }

        public static Envelope2<T> Bound(IEnumerable<Envelope2<T>> boundaries)
        {
            if (boundaries == null)
                throw new ArgumentNullException();
            IEnumerator<Envelope2<T>> enu = boundaries.GetEnumerator();
            if (!enu.MoveNext())
                return null;

            Envelope2<T> cur = enu.Current;
            while (cur == null && enu.MoveNext())
            {
                cur = enu.Current;
            }
            if (cur == null)
                return null;

            T minX = cur.MinX;
            T maxX = cur.MaxX;
            T minY = cur.MinY;
            T maxY = cur.MaxY;

            while (enu.MoveNext())
            {
                cur = enu.Current;
                if (minX.CompareTo(cur.MinX) > 0)
                    minX = cur.MinX;
                if (minY.CompareTo(cur.MinY) > 0)
                    minY = cur.MinY;
                if (maxX.CompareTo(cur.MaxX) < 0)
                    maxX = cur.MaxX;
                if (maxY.CompareTo(cur.MaxY) < 0)
                    maxY = cur.MaxY;
            }
            return new Envelope2<T>(minX, minY, maxX, maxY);
        }

        public static bool EnvelopeOverlap(Envelope2<T> a, Envelope2<T> b)
        {
            if (a == null || b == null)
                return false;
            if (a.MinX.CompareTo(b.MaxX) > 0)
                return false;
            if (b.MinX.CompareTo(a.MaxX) > 0)
                return false;
            if (a.MinY.CompareTo(b.MaxY) > 0)
                return false;
            if (b.MinY.CompareTo(a.MaxY) > 0)
                return false;
            return true;
        }

        public static bool EnvelopeOverlap(T aMinX, T aMaxX, T aMinY, T aMaxY, T bMinX, T bMaxX, T bMinY, T bMaxY)
        {
            if (aMinX.CompareTo(bMaxX) > 0)
                return false;
            if (bMinX.CompareTo(aMaxX) > 0)
                return false;
            if (aMinY.CompareTo(bMaxY) > 0)
                return false;
            if (bMinY.CompareTo(aMaxY) > 0)
                return false;
            return true;
        }

        public static bool EnvelopeContain(Envelope2<T> container, Envelope2<T> contained)
        {
            if (container == null || contained == null)
                return false;
            if (container.MinX.CompareTo(contained.MinX) > 0 ||
                container.MaxX.CompareTo(container.MaxX) < 0 ||
                container.MinY.CompareTo(contained.MinY) > 0 ||
                container.MaxY.CompareTo(container.MaxY) < 0)
                return false;
            return true;
        }

        public static bool EnvelopeContain(T aMinX, T aMaxX, T aMinY, T aMaxY, T bMinX, T bMaxX, T bMinY, T bMaxY)
        {
            if (aMinX.CompareTo(bMinX) > 0 ||
                aMaxX.CompareTo(bMaxX) < 0 ||
                aMinY.CompareTo(bMinY) > 0 ||
                aMaxY.CompareTo(bMaxY) < 0)
                return false;
            return true;
        }

        public static bool PointInside(Envelope2<T> env, T X, T Y)
        {
            if (X.CompareTo(env.MinX) < 0 || X.CompareTo(env.MaxX) > 0)
                return false;
            return Y.CompareTo(env.MinY) >= 0 && Y.CompareTo(env.MaxY) <= 0;
        }

        public T this[int dimension, BoundLimit boundEnd]
        {
            get
            {
                if (boundEnd == BoundLimit.Minimum)
                {
                    if (dimension == 0)
                    {
                        return this.MinX;
                    }
                    else if (dimension == 1)
                    {
                        return this.MinY;
                    }
                }
                else
                {
                    if (dimension == 0)
                    {
                        return this.MaxX;
                    }
                    else if (dimension == 1)
                    {
                        return this.MaxY;
                    }
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        public int Dimensions
        {
            get { return 2; }
        }
    }
}
