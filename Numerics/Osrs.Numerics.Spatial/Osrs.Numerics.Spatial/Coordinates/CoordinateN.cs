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
    public sealed class CoordinateN<T> : ICoordinate<T>, IEquatable<CoordinateN<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        internal readonly T[] ordinates;

        public CoordinateN()
        {
            this.ordinates = new T[0];
        }

        public CoordinateN(T[] ordinates)
        {
            if (ordinates == null)
                throw new ArgumentNullException();
            this.ordinates = new T[ordinates.Length];
            Array.Copy(ordinates, this.ordinates, this.ordinates.Length);
        }

        public CoordinateN(CoordinateN<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.ordinates = new T[other.ordinates.Length];
            Array.Copy(other.ordinates, this.ordinates, this.ordinates.Length);
        }

        public bool Equals(CoordinateN<T> other)
        {
            if (this.ordinates.Length == other.ordinates.Length)
            {
                for (int i = 0; i < this.ordinates.Length; i++)
                {
                    if (!this.ordinates[i].Equals(other.ordinates[i]))
                        return false;
                }
            }
            return false;
        }

        public int Dimensions
        {
            get { return this.ordinates.Length; }
        }

        public T this[int dimension]
        {
            get
            {
                return this.ordinates[dimension];
            }
            set
            {
                this.ordinates[dimension] = value;
            }
        }

        public bool Equals(ICoordinate other)
        {
            if (other is CoordinateN<T>)
                return this.Equals(other as CoordinateN<T>);
            return false;
        }

        public int CompareTo(ICoordinate other)
        {
            if (other == null)
                return 1;
            if (other is CoordinateN<T>)
                return this.CompareTo(other as CoordinateN<T>);
            throw new TypeMismatchException();
        }
    }
}
