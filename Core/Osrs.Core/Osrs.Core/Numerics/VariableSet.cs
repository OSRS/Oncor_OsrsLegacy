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
using System.Collections.Generic;
using System.Diagnostics;

namespace Osrs.Numerics
{
    //indicates a mutable variable, works for structs as well - generally used in equations as value placeholders
    public interface IVariable<T>
    {
        T Value
        {
            get;
            set;
        }
    }

    /// <summary>
    /// A collection of named values, as in a dictionary, for use in mathematical functions in which each named value represents a variable in the function.
    /// For example, in a function r=Ax + By + C, the parameters of A, B, C, x and y may all be contained in a VariableSet.
    /// If a function uses the same variable multiple times, it's value will be consistent in all places (e.g. y=Ax + Bx), x is a single variable used twice.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class VariableSet<T>
    {
        private readonly Dictionary<string, T> values = new Dictionary<string, T>();

        public T this[string variable]
        {
            [DebuggerStepThrough]
            get { return this.values[variable]; }
            [DebuggerStepThrough]
            set
            {
                this.values[variable] = value;
            }
        }

        [DebuggerStepThrough]
        public VariableSet(IEnumerable<string> variables)
        {
            foreach (string s in variables)
            {
                if (!this.values.ContainsKey(s))
                    this.values[s] = default(T);
            }
        }

        [DebuggerStepThrough]
        public VariableSet(IDictionary<string, T> variables)
        {
            foreach (string s in variables.Keys)
            {
                this.values[s] = variables[s];
            }
        }
    }
}
