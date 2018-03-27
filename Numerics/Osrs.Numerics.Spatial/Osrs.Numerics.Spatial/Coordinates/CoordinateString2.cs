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

namespace Osrs.Numerics.Spatial.Coordinates
{
    public sealed class CoordinateString2<T> : IEquatable<CoordinateString2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly Coordinate2<T>[] Coordinates;

        public CoordinateString2(Coordinate2<T>[] coordinates)
        {
            this.Coordinates = coordinates;
        }

        public CoordinateString2(IEnumerable<Coordinate2<T>> coordinates)
        {
            if (coordinates != null)
                this.Coordinates = coordinates.ToArray();
        }

        public CoordinateString2(CoordinateString2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Coordinates = new Coordinate2<T>[other.Coordinates.Length];
            Array.Copy(other.Coordinates, this.Coordinates, this.Coordinates.Length);
        }

        public bool Equals(CoordinateString2<T> other)
        {
            if (this.Coordinates.Length != other.Coordinates.Length)
                return false;
            for (int i = 0; i < this.Coordinates.Length; i++)
            {
                if (!this.Coordinates[i].Equals(other.Coordinates[i]))
                    return false;
            }
            return true;
        }

        public bool EqualsNonDirectional(CoordinateString2<T> other)
        {
            if (this.Coordinates.Length != other.Coordinates.Length)
                return false;
            if (this.Coordinates[0].Equals(other.Coordinates[0]))
            {
                for (int i = 0; i < this.Coordinates.Length; i++)
                {
                    if (!this.Coordinates[i].Equals(other.Coordinates[i]))
                        return false;
                }
                return true;
            }
            else if (this.Coordinates[this.Coordinates.Length - 1].Equals(other.Coordinates[0]))
            {
                int id = this.Coordinates.Length - 1;
                for (int i = 0; i < this.Coordinates.Length; i++)
                {
                    if (!this.Coordinates[id - i].Equals(other.Coordinates[i]))
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
