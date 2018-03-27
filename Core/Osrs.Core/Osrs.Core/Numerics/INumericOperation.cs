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

namespace Osrs.Numerics
{
    /// <summary>
    /// An "accumulator" of the "mathematical type" for a grid.
    /// Allows an arbitrary function to be applied to a grid cell that "adds" and "subtracts" from the "mass" of a cell.
    /// Common example would be Add == addition, Remove == subtraction.
    /// More complex may be Add == (factor*increment + current), Remove == (current - factor*decrement)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INumericOperation<T>
    {
        T Add(T current, T increment);

        T Remove(T current, T decrement);
    }

    public sealed class BasicNumericOperation<T> : INumericOperation<T>
    {
        public delegate T AddOp(T current, T increment);
        public delegate T RemoveOp(T current, T decrement);

        private AddOp addImpl;
        public T Add(T current, T increment)
        {
            return this.addImpl(current, increment);
        }

        private RemoveOp remImpl;
        public T Remove(T current, T decrement)
        {
            return this.remImpl(current, decrement);
        }

        public BasicNumericOperation(AddOp add, RemoveOp remove)
        {
            if (add == null || remove == null)
                throw new ArgumentNullException();
            this.addImpl = add;
            this.remImpl = remove;
        }
    }
}
