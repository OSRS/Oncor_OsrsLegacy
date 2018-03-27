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
using System.Threading;

namespace Osrs.Data.Generators
{
    public class Int64SequenceGenerator : ISequenceGenerator<long>
    {
        private long seed;
        private long increment;
        private long current;

        public Int64SequenceGenerator()
            : this(0, 1, 0)
        {
        }

        public Int64SequenceGenerator(long seed)
            : this(seed, 1, seed)
        {
        }

        public Int64SequenceGenerator(long seed, long increment)
            : this(seed, increment, seed)
        {
        }

        public Int64SequenceGenerator(long seed, long increment, long current)
        {
            if (seed > current)
                throw new ArgumentException("seed cannot be greater than current");
            this.seed = seed;
            this.increment = increment;
            this.current = current;
        }

        public long Current
        {
            get
            {
                return this.current;
            }
        }

        public long Next()
        {
            return Interlocked.Add(ref this.current, this.increment);
        }

        public object NextObject()
        {
            return Next();
        }
    }

    public class Int32SequenceGenerator : ISequenceGenerator<int>
    {
        private int seed;
        private int increment;
        private int current;

        public Int32SequenceGenerator()
            : this(0, 1, 0)
        {
        }

        public Int32SequenceGenerator(int seed)
            : this(seed, 1, seed)
        {
        }

        public Int32SequenceGenerator(int seed, int increment)
            : this(seed, increment, seed)
        {
        }

        public Int32SequenceGenerator(int seed, int increment, int current)
        {
            if (seed > current)
                throw new ArgumentException("seed cannot be greater than current");
            this.seed = seed;
            this.increment = increment;
            this.current = current;
        }

        public int Current
        {
            get
            {
                return this.current;
            }
        }

        public int Next()
        {
            return Interlocked.Add(ref this.current, this.increment);
        }

        public object NextObject()
        {
            return Next();
        }
    }
}
