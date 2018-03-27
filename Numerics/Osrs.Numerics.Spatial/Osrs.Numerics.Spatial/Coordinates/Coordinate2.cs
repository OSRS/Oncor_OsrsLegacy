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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial.Coordinates
{
    public sealed class Coordinate2<T> : ICoordinate<T>, IEquatable<Coordinate2<T>>, IComparable<Coordinate2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public T X;
        public T Y;

        public int Dimensions
        {
            get { return 2; }
        }

        public T this[int dimension]
        {
            get
            {
                if (dimension == 0)
                    return this.X;
                else if (dimension == 1)
                    return this.Y;
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (dimension == 0)
                    this.X = value;
                else if (dimension == 1)
                    this.Y = value;
                throw new IndexOutOfRangeException();
            }
        }

        public Coordinate2()
        {
        }

        public Coordinate2(T xy)
        {
            this.X = xy;
            this.Y = xy;
        }

        public Coordinate2(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }

        public Coordinate2(Coordinate2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.X = other.X;
            this.Y = other.Y;
        }

        public bool Equals(Coordinate2<T> other)
        {
            if (other == null)
                return false;
            return this.X.Equals(other.X) && this.Y.Equals(other.Y);
        }

        public bool Equals(ICoordinate other)
        {
            if (other is Coordinate2<T>)
                return this.Equals(other as Coordinate2<T>);
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Coordinate2<T>)
                return this.Equals(obj as Coordinate2<T>);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        public override string ToString()
        {
            ObjectToStringBuilder sb = new ObjectToStringBuilder(this);
            sb.Append("x", this.X.ToString());
            sb.Append("y", this.Y.ToString());
            return sb.ToString();
        }

        public int CompareTo(Coordinate2<T> other)
        {
            int dx = this.X.CompareTo(other.X);
            if (dx == 0)
                return this.Y.CompareTo(other.Y);
            return dx;
        }

        public static bool operator ==(Coordinate2<T> a, Coordinate2<T> b)
        {
            if (Object.ReferenceEquals(a, null))
                return Object.ReferenceEquals(b, null);
            if (Object.ReferenceEquals(b, null))
                return false;
            return a.X.Equals(b.X) && a.Y.Equals(b.Y);
        }

        public static bool operator !=(Coordinate2<T> a, Coordinate2<T> b)
        {
            return !(a == b);
        }

        public int CompareTo(ICoordinate other)
        {
            if (other == null)
                return 1;
            if (other is Coordinate2<T>)
                return this.CompareTo(other as Coordinate2<T>);
            throw new TypeMismatchException();
        }
    }
}
