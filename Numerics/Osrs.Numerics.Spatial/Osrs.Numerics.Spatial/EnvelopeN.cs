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

namespace Osrs.Numerics.Spatial
{
    public sealed class EnvelopeN<T> : IEnvelope<T>
        where T : IComparable<T>, IEquatable<T>
    {
        private readonly T[] ordinateMins;
        private readonly T[] ordinateMaxs;

        public EnvelopeN(T[] mins, T[] maxes)
        {
            MethodContract.NotNull(mins, nameof(mins));
            MethodContract.NotNull(maxes, nameof(maxes));
            MethodContract.Assert(mins.Length == maxes.Length, nameof(mins) + " length doesn't match " + nameof(maxes));

            this.ordinateMaxs = new T[maxes.Length];
            this.ordinateMins = new T[mins.Length];
            Array.Copy(mins, this.ordinateMins, mins.Length);
            Array.Copy(maxes, this.ordinateMaxs, maxes.Length);
        }

        public EnvelopeN(EnvelopeN<T> cloned)
        {
            if (cloned == null)
                throw new ArgumentNullException();
            this.ordinateMaxs = cloned.ordinateMaxs; //note a shared ref, not a duplicate
            this.ordinateMins = cloned.ordinateMins; //note a shared ref, not a duplicate
        }

        public T this[int dimension, BoundLimit boundEnd]
        {
            get
            {
                if (boundEnd == BoundLimit.Minimum)
                {
                    if (dimension < this.ordinateMaxs.Length)
                        return this.ordinateMins[dimension];
                }
                else
                {
                    if (dimension < this.ordinateMaxs.Length)
                        return this.ordinateMaxs[dimension];
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        public int Dimensions
        {
            get { return this.ordinateMaxs.Length; }
        }
    }
}
