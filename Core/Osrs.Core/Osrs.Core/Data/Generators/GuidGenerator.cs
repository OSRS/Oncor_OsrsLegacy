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

namespace Osrs.Data.Generators
{
    public sealed class GuidGenerator : IGenerator<Guid>
    {
        private object syncRoot = new object();

        public GuidGenerator()
        {
        }

        public Guid Next()
        {
            lock (this.syncRoot)
            {
                return Guid.NewGuid();
            }
        }

        public object NextObject()
        {
            return this.Next();
        }
    }

    /// <summary>
    /// Generator that produces DateTimes serially
    /// </summary>
    public sealed class DateTimeGenerator : IGenerator<DateTime>
    {
        private object syncRoot = new object();

        public DateTimeGenerator()
        {
        }

        /// <summary>
        /// Synchronized call to generate current DateTime stamp
        /// </summary>
        /// <returns>DateTime.Now</returns>
        public DateTime Next()
        {
            lock (this.syncRoot)
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Synchronized call to generate current DateTime stamp
        /// </summary>
        /// <returns>DateTime.Now</returns>
        public object NextObject()
        {
            return this.Next();
        }
    }
}
