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
    public sealed class Envelope3<T> : IEnvelope<T>
        where T : IComparable<T>, IEquatable<T>
    {
        public readonly T MinX;
        public readonly T MinY;
        public readonly T MinZ;
        public readonly T MaxX;
        public readonly T MaxY;
        public readonly T MaxZ;

        public Envelope3(T minX, T minY, T minZ, T maxX, T maxY, T maxZ)
        {
            if (minX.CompareTo(maxX) >= 0)
                throw new DegenerateCaseException();
            if (minY.CompareTo(maxY) >= 0)
                throw new DegenerateCaseException();
            if (minZ.CompareTo(maxZ) >= 0)
                throw new DegenerateCaseException();
            this.MinX = minX;
            this.MaxX = maxX;
            this.MinY = minY;
            this.MaxY = maxY;
            this.MinZ = minZ;
            this.MaxZ = maxZ;
        }

        public Envelope3(Envelope3<T> cloned)
        {
            if (cloned == null)
                throw new ArgumentNullException();
            this.MinX = cloned.MinX;
            this.MaxX = cloned.MaxX;
            this.MinY = cloned.MinY;
            this.MaxY = cloned.MaxY;
            this.MinZ = cloned.MinZ;
            this.MaxZ = cloned.MaxZ;
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
                    else if (dimension == 2)
                    {
                        return this.MinZ;
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
                    else if (dimension == 2)
                    {
                        return this.MaxZ;
                    }
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        public int Dimensions
        {
            get { return 3; }
        }
    }
}
