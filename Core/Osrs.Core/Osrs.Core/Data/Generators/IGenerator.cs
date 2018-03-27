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
namespace Osrs.Data.Generators
{
    public interface IGenerator
    {
        object NextObject();
    }

    public interface IBiDirectionalGenerator : IGenerator
    {
        object PreviousObject();
    }

    public interface IGenerator<T> : IGenerator
    {
        T Next();
    }

    public interface IBiDirectionalGenerator<T> : IGenerator<T>, IBiDirectionalGenerator
    {
        T Previous();
    }

    public sealed class DefaultGenerator<T> : IGenerator<T>
    {
        private T value;
        public T Next()
        {
            return this.value;
        }

        public object NextObject()
        {
            return this.value;
        }

        public DefaultGenerator(T value)
        {
            this.value = value;
        }
    }

    /// <summary>
    /// Generator that produces and ordered set of values, where each subsequent value is an increment or decrement of the previous value, no values are repeated
    /// </summary>
    public interface ISequenceGenerator : IGenerator
    {
    }

    /// <summary>
    /// Generator that produces and ordered set of values, where each subsequent value is an increment or decrement of the previous value, no values are repeated
    /// </summary>
    /// <typeparam name="T">Type of values to produce</typeparam>
    public interface ISequenceGenerator<T> : ISequenceGenerator, IGenerator<T>
    {
    }
}
