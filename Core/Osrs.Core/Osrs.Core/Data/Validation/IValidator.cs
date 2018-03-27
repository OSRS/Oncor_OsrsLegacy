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

namespace Osrs.Data.Validation
{
    public interface IValidator
    {
        bool IsValidObject(object item);
    }

    public interface IValidator<T> : IValidator
    {
        bool IsValid(T item);
    }

    public sealed class GenericCollectionValidator<T> : IValidator<IEnumerable<T>>
    {
        private IValidator<T> validator;

        public GenericCollectionValidator(IValidator<T> validator)
        {
            if (validator == null)
                throw new NullReferenceException("Validator cannot be null");
            this.validator = validator;
        }

        public bool IsValid(IEnumerable<T> item)
        {
            if (item == null)
                return false;
            foreach (T element in item)
            {
                if (!this.validator.IsValid(element))
                    return false;
            }
            return true;
        }

        public bool IsValidObject(object o)
        {
            if (o == null)
                return false;
            return this.IsValid(o as IEnumerable<T>);
        }
    }

    public sealed class SimpleValidator : IValidator
    {
        private bool noNulls = true;

        public bool AllowNull
        {
            get { return !this.noNulls; }
        }

        public SimpleValidator()
        {
        }
        public SimpleValidator(bool allowNull)
        {
            this.noNulls = !allowNull;
        }

        public bool IsValidObject(object o)
        {
            if (o == null && this.noNulls)
                return false;
            return true;
        }
    }

    public sealed class GenericSimpleValidator<T> : IValidator<T>
    {
        private bool noNulls = true;
        public bool AllowNull
        {
            get { return !this.noNulls; }
        }

        public GenericSimpleValidator()
        {
        }
        public GenericSimpleValidator(bool allowNull)
        {
            this.noNulls = !allowNull;
        }

        public bool IsValidObject(object o)
        {
            if (o == null && this.noNulls)
                return false;
            return this.IsValid(o.As<T>());
        }

        public bool IsValid(T item)
        {
            if (item == null && this.noNulls)
                return false;
            return true;
        }
    }

    /// <summary>
    /// Provides a constant validation result for any item of type T.
    /// </summary>
    public class GenericNullValidator<T> : IValidator<T>
    {
        private bool retVal;

        /// <summary>
        /// Creates a GenericNullValidator instance with a return value of retVal.
        /// </summary>
        public GenericNullValidator()
        {
            this.retVal = true;
        }

        /// <summary>
        /// Creates a GenericNullValidator instance with the retVal variable set to the value provided.
        /// </summary>
        /// <param name="retVal">Boolean value to be set as the  retVal variable</param>
        public GenericNullValidator(bool retVal)
        {
            this.retVal = retVal;
        }


        /// <summary>
        /// Returns the value of retVal.
        /// </summary>
        /// <param name="item">Item to be validated.</param>
        /// <returns>Boolean value equal to retVal variable.</returns>
        public bool IsValid(T item)
        {
            return this.retVal;
        }
        /// <summary>
        /// Returns the value of retVal.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value equal to retVal variable.</returns>
        public bool IsValidObject(object item)
        {
            return this.retVal;
        }
    }
}
