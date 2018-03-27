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

namespace Osrs.Collections.Specialized
{
    /// <summary>
    /// An appendable dictionary of K/V pairs where the key is of type T and the item is of type V.
    /// </summary>
    /// <typeparam name="T">Type of the key.</typeparam>
    /// <typeparam name="V">Type of the item.</typeparam>
    public interface IItemSet<T, V> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the value associated with the specified key. 
        /// </summary>
        /// <param name="index">The value of the key to get.</param>
        /// <returns>The value for the provided key.</returns>
        V this[T index]
        {
            get;
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="id">The key to add.</param>
        /// <param name="item">The value to add.</param>
        /// <returns></returns>
        bool Add(T id, V item);

        /// <summary>
        /// Returns the value for the key provided.
        /// </summary>
        /// <param name="id">Object containing the key.</param>
        /// <returns>Object containing the value.</returns>
        V Get(T id);
    }

    /// <summary>
    /// An appendable dictionary of K/V pairs where the key is of type T and the value is of type string.
    /// </summary>
    /// <typeparam name="T">Type for the key.</typeparam>
    public interface INameSet<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the value associated with the specified key. 
        /// </summary>
        /// <param name="index">The value of the key to get.</param>
        /// <returns>String value for the provided key.</returns>
        string this[T index]
        {
            get;
        }

        /// <summary>
        /// Gets the key associated with the specified value.
        /// </summary>
        /// <param name="index">String value of the item to get.</param>
        /// <returns>Key for the provided value.</returns>
        T this[string index]
        {
            get;
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="id">The key to add.</param>
        /// <param name="name">The value to add.</param>
        /// <returns>Boolean value.</returns>
        bool Add(T id, string name);

        /// <summary>
        /// Provides the value for the key provided.
        /// </summary>
        /// <param name="Id">Object containing the key.</param>
        /// <returns>Value of type string.</returns>
        string GetName(T Id);

        /// <summary>
        /// Provides the key for the value provided.
        /// </summary>
        /// <param name="name">Value of type string.</param>
        /// <returns>Object containing the key.</returns>
        T GetId(string name);
    }

    /// <summary>
    /// An appendable list of ID/Name/Value triples where the IDs are of type T, names are of type string and the values are of type V.
    /// </summary>
    /// <typeparam name="T">Type for the ids.</typeparam>
    /// <typeparam name="V">Type for the values.</typeparam>
    public interface INamedValueSet<T, V> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the value for the key provided.
        /// </summary>
        /// <param name="index">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        V this[T index]
        {
            get;
        }

        /// <summary>
        /// Gets the value for the string provided.
        /// </summary>
        /// <param name="index">The name of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        V this[string index]
        {
            get;
        }

        /// <summary>
        /// Adds the specified ID/Name/Value to an appendable list.
        /// </summary>
        /// <param name="id">The id of the item to add.</param>
        /// <param name="name">The name of the item to add.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>Boolean Value.</returns>
        bool Add(T id, string name, V item);

        /// <summary>
        /// Returns the name of the item based on the id provided.
        /// </summary>
        /// <param name="Id">The id of the item.</param>  
        /// <returns>The name of the item.</returns>
        string GetName(T Id);

        /// <summary>
        /// Returns the id of the item based on the name provided
        /// </summary>
        /// <param name="name">The name of the item.</param>  
        /// <returns>The ID of the item.</returns>
        T GetId(string name);

        /// <summary>
        /// Returns the value of the item based on the id provided.
        /// </summary>
        /// <param name="id">The id of the item.</param>  
        /// <returns>The value of the item.</returns>
        V GetItem(T id);

        /// <summary>
        /// Returns the value of the item based on the id provided.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns>The value of the item.</returns>
        V GetItem(string name);
    }
}
