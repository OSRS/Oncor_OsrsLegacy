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

namespace Osrs.Runtime
{
    public static class MethodContract
    {
        public static bool CheckFor(int fromInclusive, int toExclusive, Predicate<int> validator)
        {
            if (fromInclusive > toExclusive)
                throw new ArgumentException("fromInclusive must be less than or equal to toExclusive.");
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            for(int i=fromInclusive;i<toExclusive;i++)
            {
                if (!validator(i))
                    return false;
            }
            return true;
        }

        public static void AssertFor(int fromInclusive, int toExclusive, Predicate<int> validator, string name)
        {
            Assert(CheckFor(fromInclusive, toExclusive, validator), name);
        }

        public static void Assert(bool assertion, string name)
        {
            if (!assertion)
                throw new ArgumentException(name);
        }

        public static void NotNull(object obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException(name);
        }

        public static void NotEmpty(IEmpty obj, string name)
        {
            if (obj != null && obj.IsEmpty)
                throw new ArgumentException(name);
        }

        public static void NotEmpty(string obj, string name)
        {
            if (obj!=null && string.Empty.Equals(obj))
                throw new ArgumentException(name);
        }

        public static void NotNullOrEmpty(IEmpty obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException(name);
            if (obj.IsEmpty)
                throw new ArgumentException(name);
        }

        public static void NotNullOrEmpty(string obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException(name);
            if (string.Empty.Equals(obj))
                throw new ArgumentException(name);
        }
    }
}
