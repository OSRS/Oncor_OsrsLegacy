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
    public sealed class CoordinatePair2<T> : IEquatable<CoordinatePair2<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly Coordinate2<T> Start;
        public readonly Coordinate2<T> End;

        public bool IsDegenerate
        {
            get { return this.Start.Equals(this.End); }
        }

        public CoordinatePair2(Coordinate2<T> start, Coordinate2<T> end)
        {
            if (start == null || end == null)
                throw new ArgumentNullException();
            this.Start = start;
            this.End = end;
        }

        public CoordinatePair2(CoordinatePair2<T> other)
        {
            if (other == null)
                throw new ArgumentNullException();
            this.Start = new Coordinate2<T>(other.Start);
            this.End = new Coordinate2<T>(other.End);
        }

        public bool Equals(CoordinatePair2<T> other)
        {
            return this.Start.Equals(other.Start) && this.End.Equals(other.End);
        }

        public bool EqualsNonDirectional(CoordinatePair2<T> other)
        {
            if (this.Start.Equals(other.Start))
                return this.End.Equals(other.End);
            if (this.End.Equals(other.Start))
                return this.Start.Equals(other.End);
            return false;
        }
    }
}
