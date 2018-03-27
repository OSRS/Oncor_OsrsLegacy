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
namespace Osrs
{
    /// <summary>
    /// Provides the conversion capability defined in the <code>Converter(TInput, TOutput)</code> delegate without the need for a delegate.  This provides greater flexibility to pass the object reference around and retain the item that performs the conversion.
    /// Additionally, the IConverter provides full round-trip conversion for places where a serialization or transformation context is required.
    /// 
    /// For one-way conversion <see cref="IConverterHalf"/>
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public interface IConverter<TFrom, TTo> : IConverterHalf<TFrom, TTo>
    {
        TFrom Reverse(TTo item);
    }

    /// <summary>
    /// Provides the conversion capability defined in the <code>Converter(TInput, TOutput)</code> delegate without the need for a delegate.  This provides greater flexibility to pass the object reference around and retain the item that performs the conversion.
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public interface IConverterHalf<TFrom, TTo>
    {
        TTo Forward(TFrom item);
    }
}
