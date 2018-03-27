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
    /// Provides validation for a Boolean entity provided.
    /// </summary>
    public class BooleanValidator : IValidator<bool>
    {
        /// <summary>
        /// Creates a BooleanValidator instance.
        /// </summary>
        public BooleanValidator()
        {
        }

        /// <summary>
        /// Validates that the item is a Boolean value.
        /// </summary>
        /// <param name="item">Boolean to be validated.</param>
        /// <returns>Returns true if the item is valid, false otherwise.</returns>
        public bool IsValid(bool item)
        {
            return true;
        }

        /// <summary>
        /// Validates that the object is a Boolean value.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Returns true if the item is valid, false otherwise.</returns>
        public bool IsValidObject(object item)
        {
            if (item is bool)
            {
                return this.IsValid((bool)item);
            }
            return false;
        }
    }

    /// <summary>
    /// A validator for a byte[]
    /// </summary>
    public sealed class ByteArrayValidator : IValidator<byte[]>
    {
        private bool allowNull;
        private int minLength;
        private int maxLength;

        /// <summary>
        /// Is null an acceptable value for the array itself (not the content)
        /// </summary>
        public bool AllowNull
        {
            get { return this.allowNull; }
        }
        /// <summary>
        /// What is the minimum allowed length of the array (0 allowing empty arrays)
        /// </summary>
        public int MinLength
        {
            get { return this.minLength; }
        }
        /// <summary>
        /// What is the maximum allowed length of the array, must be equal or larger than min
        /// </summary>
        public int MaxLength
        {
            get { return this.maxLength; }
        }

        public ByteArrayValidator()
            : this(false, 0, int.MaxValue)
        { }

        public ByteArrayValidator(bool allowNull)
            : this(allowNull, 0, int.MaxValue)
        { }

        public ByteArrayValidator(int limit, BoundLimit type)
            : this(false, limit, type)
        { }

        public ByteArrayValidator(bool allowNull, int limit, BoundLimit type)
        {
            this.allowNull = allowNull;
            if (type == BoundLimit.Maximum)
            {
                this.minLength = 0;
                if (limit < 0)
                    this.maxLength = 0;
                else
                    this.maxLength = limit;
            }
            else
            {
                this.maxLength = int.MaxValue;
                if (limit < 0)
                    this.minLength = 0;
                else
                    this.minLength = limit;
            }
        }

        public ByteArrayValidator(int min, int max)
            : this(false, min, max)
        { }

        public ByteArrayValidator(bool allowNull, int min, int max)
        {
            if (max > min)
                throw new ArgumentException("max > min");
            this.allowNull = allowNull;
            if (min < 0)
            {
                this.minLength = 0;
                if (max < 0)
                    this.maxLength = 0;
                else
                    this.maxLength = max;
            }
            else
            {
                this.minLength = min;
                this.maxLength = max;
            }
        }

        /// <summary>
        /// Checks an array for legal size and existance
        /// </summary>
        /// <param name="item">The array to check</param>
        /// <returns>true if value, false otherwise</returns>
        public bool IsValid(byte[] item)
        {
            if (item == null)
                return this.allowNull;
            if (item.Length < this.minLength || item.Length > this.maxLength)
                return false;
            return true;
        }

        /// <summary>
        /// Checks an array for legal size and existance
        /// </summary>
        /// <param name="item">The array to check</param>
        /// <returns>true if value, false otherwise</returns>
        public bool IsValidObject(object o)
        {
            if (o is byte[])
                return this.IsValid(o as byte[]);
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a Byte entity.
    /// </summary>
    public class ByteValidator : IValidator<byte>
    {
        private bool hasMin;
        private bool hasMax;
        private byte min;
        private byte max;

        /// <summary>
        /// Creates a ByteValidator instance without a Minimum and Maximum value.
        /// </summary>
        public ByteValidator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a new ByteValidator instance with only a Minimum or only a Maximum value.
        /// </summary>
        /// <param name="limit">Byte value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public ByteValidator(byte limit, BoundLimit type)
        {
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
        /// Creates a new ByteValidator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">Byte value to be set as the minimum value.  Minimum value should be less than maximum value.</param>
        /// <param name="max">Byte value to be set as the maximum value.  Maximum value should be greater than minimum value.</param>
        public ByteValidator(byte min, byte max)
        {
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
        /// Validates that the item is valid based on the minimum and/or maximum values set for the instance.
        /// </summary>
        /// <param name="item">Byte value to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(byte item)
        {
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
        /// Validates that the object is a Byte value.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is byte)
            {
                return this.IsValid((byte)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a Date/Time entity
    /// </summary>
    public class DateTimeValidator : IValidator<DateTime>
    {
        private bool hasKind;
        private DateTimeKind dtKind;
        private bool hasMin;
        private DateTime min;
        private bool hasMax;
        private DateTime max;

        /// <summary>
        /// Creates a DateTimeValidator instance without a DateTime Kind identified, a Minimum value and Maximum value.
        /// </summary>
        public DateTimeValidator()
        {
            this.hasKind = false;
            this.hasMin = false;
            this.hasMax = false;
        }

        /// <summary>
        /// Creates a DateTimeValidator instance with a DateTime Kind but without a Minimum and Maximum value.
        /// </summary>
        /// <param name="dtKind">Specified as either universal time (UTC) or local time.</param>
        public DateTimeValidator(DateTimeKind dtKind)
        {
            this.hasKind = true;
            this.dtKind = dtKind;
            this.hasMin = false;
            this.hasMax = false;
        }

        /// <summary>
        /// Creates a DateTimeValidator instance with only a Minimum or a Maximum value and without a DateTime Kind.
        /// </summary>
        /// <param name="limit">DateTime value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public DateTimeValidator(DateTime limit, BoundLimit type)
        {
            this.hasKind = false;
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
        /// Creates a DateTimeValidator instance with a Minimum and a Maximum value and without a DateTime Kind.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">DateTime value to be set as the minimum value.  Minimum value should be less than maximum value.</param>
        /// <param name="max">DateTime value to be set as the maximum value.  Maximum value should be greater than minimum value.</param>
        public DateTimeValidator(DateTime min, DateTime max)
        {
            this.hasKind = false;
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
        /// Creates a DateTimeValidator instance with a a DateTime Kind, and only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="dtKind">DateTime kind specified as either universal time (UTC) or local time.</param>
        /// <param name="limit">DateTime value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public DateTimeValidator(DateTimeKind dtKind, DateTime limit, BoundLimit type)
        {
            this.hasKind = true;
            this.dtKind = dtKind;
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
        /// Creates a DateTimeValidator instance with a a DateTime Kind, and a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="dtKind">DateTime kind specified as either universal time (UTC) or local time</param>
        /// <param name="min">DateTime value to be set as the minimum value.  Minimum value should be less than maximum value.</param>
        /// <param name="max">DateTime value to be set as the maximum value.  Maximum value should be greater than minimum value.</param>
        public DateTimeValidator(DateTimeKind dtKind, DateTime min, DateTime max)
        {
            this.hasKind = true;
            this.dtKind = dtKind;
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
        /// Validates that the item is valid based on the DateTime Kind and/or minimum and/or maximum values set for the instance.
        /// </summary>
        /// <param name="item">DateTime value to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(DateTime item)
        {
            if (this.hasKind && item.Kind != this.dtKind)
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
        /// Validates that the item is a DateTime value.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is DateTime)
            {
                return this.IsValid((DateTime)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a Decimal entity.
    /// </summary>
    public class DecimalValidator : IValidator<decimal>
    {
        private bool hasMin;
        private bool hasMax;
        private decimal min;
        private decimal max;

        /// <summary>
        /// Creates a DecimalValidator instance without a Minimum and a Maximum value.
        /// </summary>
        public DecimalValidator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a DecimalValidator instance with only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="limit">Decimal value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public DecimalValidator(decimal limit, BoundLimit type)
        {
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
        /// Creates a DecimalValidator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">Decimal value to be set as the minimum value.  Minimum value should be less than maximum value.</param>
        /// <param name="max">Decimal value to be set as the maximum value.  Maximum value should be greater than minimum value.</param>
        public DecimalValidator(decimal min, decimal max)
        {
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
        /// Validates that the item is valid based on the minimum and/or maximum values set for the instance.
        /// </summary>
        /// <param name="item">Decimal to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(decimal item)
        {
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
        /// Validates that the item is a Decimal value.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is decimal)
            {
                return this.IsValid((decimal)item);
            }
            return false;
        }
    }
}
