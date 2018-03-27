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
    public sealed class Coordinate4<T> : ICoordinate<T>, IEquatable<Coordinate4<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public T X;
        public T Y;
        public T Z;
        public T M;

        public Coordinate4()
        {
        }

        public Coordinate4(T xyzm)
        {
            this.X = xyzm;
            this.Y = xyzm;
            this.Z = xyzm;
            this.M = xyzm;
        }

        public Coordinate4(T x, T y, T z, T m)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.M = m;
        }

        public Coordinate4(Coordinate4<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.X = other.X;
            this.Y = other.Y;
            this.Z = other.Z;
            this.M = other.M;
        }

        public bool Equals(Coordinate4<T> other)
        {
            return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Z.Equals(other.Z) && this.M.Equals(other.M);
        }

        public int Dimensions
        {
            get { return 4; }
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
                else if (dimension == 3)
                    return this.M;
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
                else if (dimension == 3)
                    this.M = value;
                throw new IndexOutOfRangeException();
            }
        }

        public bool Equals(ICoordinate other)
        {
            if (other is Coordinate4<T>)
                return this.Equals(other as Coordinate4<T>);
            return false;
        }

        public int CompareTo(ICoordinate other)
        {
            if (other == null)
                return 1;
            if (other is Coordinate4<T>)
                return this.CompareTo(other as Coordinate4<T>);
            throw new TypeMismatchException();
        }
    }
}
