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

namespace Osrs.Numerics.Spatial
{
    public sealed class Envelope4<T> : IEnvelope<T>
        where T : IComparable<T>, IEquatable<T>
    {
        public readonly T MinX;
        public readonly T MinY;
        public readonly T MinZ;
        public readonly T MinM;
        public readonly T MaxX;
        public readonly T MaxY;
        public readonly T MaxZ;
        public readonly T MaxM;

        public Envelope4(T minX, T minY, T minZ, T minM, T maxX, T maxY, T maxZ, T maxM)
        {
            if (minX.CompareTo(maxX) >= 0)
                throw new DegenerateCaseException();
            if (minY.CompareTo(maxY) >= 0)
                throw new DegenerateCaseException();
            if (minZ.CompareTo(maxZ) >= 0)
                throw new DegenerateCaseException();
            if (minM.CompareTo(maxM) >= 0)
                throw new DegenerateCaseException();
            this.MinX = minX;
            this.MaxX = maxX;
            this.MinY = minY;
            this.MaxY = maxY;
            this.MinZ = minZ;
            this.MaxZ = maxZ;
            this.MinM = minM;
            this.MaxM = maxM;
        }

        public Envelope4(Envelope4<T> cloned)
        {
            if (cloned == null)
                throw new ArgumentNullException();
            this.MinX = cloned.MinX;
            this.MaxX = cloned.MaxX;
            this.MinY = cloned.MinY;
            this.MaxY = cloned.MaxY;
            this.MinZ = cloned.MinZ;
            this.MaxZ = cloned.MaxZ;
            this.MinM = cloned.MinM;
            this.MaxM = cloned.MaxM;
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
                    else if (dimension == 3)
                    {
                        return this.MinM;
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
                    else if (dimension == 3)
                    {
                        return this.MaxM;
                    }
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        public int Dimensions
        {
            get { return 4; }
        }
    }
}
