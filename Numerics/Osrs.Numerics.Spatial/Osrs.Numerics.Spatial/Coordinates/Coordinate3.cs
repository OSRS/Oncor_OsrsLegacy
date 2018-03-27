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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial.Coordinates
{
    public sealed class Coordinate3<T> : ICoordinate<T>, IEquatable<Coordinate3<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public T X;
        public T Y;
        public T Z;

        public int Dimensions
        {
            get { return 3; }
        }

        public T this[int dimension]
        {
            get
            {
                if (dimension == 0)
                    return this.X;
                else if (dimension == 1)
                    return this.Y;
                else if (dimension == 2)
                    return this.Z;
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (dimension == 0)
                    this.X = value;
                else if (dimension == 1)
                    this.Y = value;
                else if (dimension == 2)
                    this.Z = value;
                throw new IndexOutOfRangeException();
            }
        }

        public Coordinate3()
        {
        }

        public Coordinate3(T xyz)
        {
            this.X = xyz;
            this.Y = xyz;
            this.Z = xyz;
        }

        public Coordinate3(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Coordinate3(Coordinate3<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.X = other.X;
            this.Y = other.Y;
            this.Z = other.Z;
        }

        public bool Equals(Coordinate3<T> other)
        {
            return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z);
        }

        public bool Equals(ICoordinate other)
        {
            if (other is Coordinate3<T>)
                return this.Equals(other as Coordinate3<T>);
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Coordinate3<T>)
                return this.Equals(obj as Coordinate3<T>);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }

        public static bool operator ==(Coordinate3<T> a, Coordinate3<T> b)
        {
            if (Object.ReferenceEquals(a, null))
                return Object.ReferenceEquals(b, null);
            if (Object.ReferenceEquals(b, null))
                return false;
            return a.X.Equals(b.X) && a.Y.Equals(b.Y) && a.Z.Equals(b.Z);
        }
        public static bool operator !=(Coordinate3<T> a, Coordinate3<T> b)
        {
            return !(a == b);
        }

        public int CompareTo(ICoordinate other)
        {
            if (other == null)
                return 1;
            if (other is Coordinate3<T>)
                return this.CompareTo(other as Coordinate3<T>);
            throw new TypeMismatchException();
        }
    }
}
