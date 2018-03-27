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
using Osrs.Numerics;
using System;

namespace Osrs.Data.Validation
{
    /// <summary>
    /// Provides validation for a Guid entity.
    /// </summary>
    public class GuidValidator : IValidator<Guid>
    {
        private bool allowEmpty;

        /// <summary>
        /// Creates a GuidValidator instance that sets the allowEmpty variable equal to false.
        /// </summary>
        public GuidValidator() : this(false)
        {
        }

        /// <summary>
        /// Creates a GuidValidator instance that sets the allowEmpty variable equal to the parameter passed in.
        /// </summary>
        /// <param name="allowEmpty">Boolean value to be set as the allowEmpty variable.</param>
        public GuidValidator(bool allowEmpty)
        {
            this.allowEmpty = allowEmpty;
        }

        /// <summary>
        /// Validates that the item is not the empty guid.
        /// </summary>
        /// <param name="item">Guid to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(Guid item)
        {
            if (!this.allowEmpty)
            {
                return !Guid.Empty.Equals(item);
            }
            return true;
        }

        /// <summary>
        /// Validates that the item is a guid.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is Guid)
            {
                return this.IsValid((Guid)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Validator for string values, can check for null and min/max lengths of the string
    /// </summary>
    public sealed class StringValidator : IValidator<string>
    {
        private bool allowNull;
        private int minLength;
        private int maxLength;

        /// <summary>
        /// boolean indicating if this validator considers null a valid value.
        /// True if null is indeed a valid value
        /// </summary>
        public bool AllowNull
        {
            get { return this.allowNull; }
        }
        /// <summary>
        /// int indicating the minimum length in characters of a valid string
        /// </summary>
        public int MinLength
        {
            get { return this.minLength; }
        }
        /// <summary>
        /// int indicating the maximum length in characters of a valid string
        /// </summary>
        public int MaxLength
        {
            get { return this.maxLength; }
        }

        public StringValidator(int lengthLimit, BoundLimit BoundLimit) : this(true, lengthLimit, BoundLimit)
        {
        }
        public StringValidator(int minLength, int maxLength) : this(true, minLength, maxLength)
        {
        }
        public StringValidator(bool allowNull) : this(allowNull, 0, int.MaxValue)
        {
        }
        public StringValidator(bool allowNull, int lengthLimit, BoundLimit BoundLimit)
        {
            this.allowNull = allowNull;
            if (BoundLimit == BoundLimit.Maximum)
            {
                this.maxLength = lengthLimit;
                this.minLength = 0;
            }
            else
            {
                this.minLength = lengthLimit;
                this.maxLength = int.MaxValue;
            }
        }
        public StringValidator(bool allowNull, int minLength, int maxLength)
        {
            if (minLength > maxLength)
                throw new ArgumentOutOfRangeException("minLength > maxLength");
            this.allowNull = allowNull;
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Ensures the provided string is valid
        /// </summary>
        /// <param name="item">the string to validate</param>
        /// <returns>true if valid, false otherwise</returns>
        public bool IsValid(string item)
        {
            if (item == null)
                return !this.allowNull;
            int len = item.Length;
            return len >= this.minLength && len <= this.maxLength;
        }

        /// <summary>
        /// Ensures the provided string is valid
        /// </summary>
        /// <param name="item">the string to validate</param>
        /// <returns>true if valid, false otherwise</returns>
        public bool IsValidObject(object o)
        {
            if (o is string)
                return this.IsValid(o as string);
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a time interval
    /// </summary>
    public class TimeSpanValidator : IValidator<TimeSpan>
    {
        private bool allowZero;
        private bool hasMin;
        private TimeSpan min;
        private bool hasMax;
        private TimeSpan max;

        /// <summary>
        /// Creates a TimeSpanValidator instance where a time interval must be greater than 0 (zero) and no minimum nor maximum time interval has been established
        /// </summary>
        public TimeSpanValidator() : this(false)
        {
        }

        /// <summary>
        /// Creates a TimeSpanValidator instance where the time interval value is set to the parameter provided and no minimum nor maximum time interval has been established.
        /// </summary>
        /// <param name="allowZero">Value to be set as the time interval value</param>
        public TimeSpanValidator(bool allowZero)
        {
            this.allowZero = allowZero;
        }

        /// <summary>
        /// Creates a TimeSpanValidator instance where a time interval must be greater than 0 (zero) and either the minimum or maximum time interval is established.
        /// </summary>
        /// <param name="limit">Time interval value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public TimeSpanValidator(TimeSpan limit, BoundLimit type) : this(false, limit, type)
        {
        }

        /// <summary>
        /// Creates a TimeSpanValidator instance where a time interval must be greater than 0 (zero) and both the minimum and maximum time interval is established.
        /// </summary>
        /// <param name="min">Time interval value to be set as the minimum.</param>
        /// <param name="max">Time interval value to be set as the maximum.</param>
        public TimeSpanValidator(TimeSpan min, TimeSpan max) : this(false, min, max)
        {
        }

        /// <summary>
        /// Creates a TimeSpanValidator instance where the time interval value is set to the parameter provided and either the minimum or maximum time interval is established.
        /// </summary>
        /// <param name="allowZero">Value to be set as the time interval variable.</param>
        /// <param name="limit">Time interval value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public TimeSpanValidator(bool allowZero, TimeSpan limit, BoundLimit type)
        {
            this.allowZero = allowZero;
            if (type == BoundLimit.Maximum)
            {
                this.hasMax = true;
                this.max = limit;
                this.hasMin = false;
            }
            else
            {
                this.hasMin = true;
                this.min = limit;
                this.hasMax = false;
            }
        }

        /// <summary>
        /// Creates a TimeSpanValidator instance where the time interval value is set to the parameter provided and both the minimum and maximum time interval is established.
        /// </summary>
        /// <param name="allowZero">Value to be set as the time interval variable.</param>
        /// <param name="min">Time interval value to be set as the minimum.</param>
        /// <param name="max">Time interval value to be set as the maximum.</param>
        public TimeSpanValidator(bool allowZero, TimeSpan min, TimeSpan max)
        {
            this.allowZero = allowZero;
            if (min <= max)
            {
                this.min = min;
                this.max = max;
                this.hasMin = true;
                this.hasMax = true;
            }
            else
            {
                this.min = min;
                this.hasMin = true;
                this.hasMax = false;
            }
        }

        /// <summary>
        /// Validates that the item is valid based on the time interval variable and minimum and/or maximum time intervals set for the instance.
        /// </summary>
        /// <param name="item">TimeSpan to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(TimeSpan item)
        {
            if (!this.allowZero && TimeSpan.Zero.Equals(item))
            {
                return false;
            }
            if (this.hasMin && this.hasMax)
            {
                return !(item < this.min || item > this.max);
            }
            if (this.hasMin)
            {
                return item >= this.min;
            }
            if (this.hasMax)
            {
                return item <= this.max;
            }
            return true;
        }

        /// <summary>
        /// Validates that the item is a Timespan.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is TimeSpan)
            {
                return this.IsValid((TimeSpan)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a uniform resource identifier entity.
    /// </summary>
    public class UriSimpleValidator : IValidator<Uri>
    {
        /// <summary>
        /// Creates a UriSimpleValidator instance.
        /// </summary>
        public UriSimpleValidator()
        {
        }

        /// <summary>
        /// Validates that the item is not null.
        /// </summary>
        /// <param name="item">URI to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(Uri item)
        {
            return !(item == null);
        }

        /// <summary>
        /// Validates that the item is a uniform resource identifier. 
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is Uri)
            {
                return this.IsValid((Uri)item);
            }
            return false;
        }
    }
}
