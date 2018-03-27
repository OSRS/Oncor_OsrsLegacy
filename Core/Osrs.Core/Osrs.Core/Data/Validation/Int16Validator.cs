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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Data.Validation
{
    /// <summary>
    /// Provides validation for a short entity.
    /// </summary>
    public class Int16Validator : IValidator<short>
    {
        private bool hasMin;
        private bool hasMax;
        private short min;
        private short max;

        /// <summary>
        /// Creates a Int16Validator instance without a Minimum nor a Maximum value.
        /// </summary>
        public Int16Validator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a Int16Validator instance with only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="limit">Short to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public Int16Validator(short limit, BoundLimit type)
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
        /// Creates a Int16Validator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">Short to be set as the minimum value.  Minimum value should be less than maximum value.</param>
        /// <param name="max">Short to be set as the maximum value.  Maximum value should be greater than minimum value.</param>
        public Int16Validator(short min, short max)
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
        /// <param name="item">Short to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(short item)
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
        /// Validates that the item is a short.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is short)
            {
                return this.IsValid((short)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for an integer entity.
    /// </summary>
    public class Int32Validator : IValidator<int>
    {
        private bool hasMin;
        private bool hasMax;
        private int min;
        private int max;

        /// <summary>
        /// Creates a Int32Validator instance without a Minimum nor a Maximum value.
        /// </summary>
        public Int32Validator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a Int32Validator instance with only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="limit">Int value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public Int32Validator(int limit, BoundLimit type)
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
        /// Creates an Int32Validator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">Int value to be set as the minimum value.  Minimum value should be less than maximum value.</param>
        /// <param name="max">Int value to be set as the maximum value.  Maximum value should be greater than minimum value.</param>
        public Int32Validator(int min, int max)
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
        /// <param name="item">Int to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(int item)
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
        /// Validates that the item is an int.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is int)
            {
                return this.IsValid((int)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a long entity.
    /// </summary>
    public class Int64Validator : IValidator<long>
    {
        private bool hasMin;
        private bool hasMax;
        private long min;
        private long max;

        /// <summary>
        /// Creates a Int64Validator instance without a minimum nor a maximum value.
        /// </summary>
        public Int64Validator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a Int64Validator instance with only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="limit">Long to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public Int64Validator(long limit, BoundLimit type)
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
        /// Creates a Int64Validator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">Long to be set as the minimum value.  Minimum value should be less than maximum value.</param>
        /// <param name="max">Long to be set as the maximum value.  Maximum value should be greater than minimum value.</param>
        public Int64Validator(long min, long max)
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
        /// <param name="item">Long to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(long item)
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
        /// Validates that the item is a Long.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is long)
            {
                return this.IsValid((long)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a SByte entity.
    /// </summary>
    public class SByteValidator : IValidator<sbyte>
    {
        private bool hasMin;
        private bool hasMax;
        private sbyte min;
        private sbyte max;

        /// <summary>
        /// Creates a SByteValidator instance without a minimum nor a maximum value.
        /// </summary>
        public SByteValidator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a SByteValidator instance with only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="limit">SByte value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public SByteValidator(sbyte limit, BoundLimit type)
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
        /// Creates a SByteValidator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">SByte value to be set as the minimum value.  Minimum value should be less than maximum value.</param>
        /// <param name="max">SByte value to be set as the maximum value.  Maximum value should be greater than minimum value.</param>
        public SByteValidator(sbyte min, sbyte max)
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
        /// <param name="item">SByte to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(sbyte item)
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
        /// Validates that the item is a SByte.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is sbyte)
            {
                return this.IsValid((sbyte)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a float entity.
    /// </summary>
    public class SingleValidator : IValidator<float>
    {
        private bool hasMin;
        private bool hasMax;
        private float min;
        private float max;

        /// <summary>
        /// Creates a SingleValidator instance without a Minimum and a Maximum value.
        /// </summary>
        public SingleValidator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a SingleValidator instance with only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="limit">Float value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public SingleValidator(float limit, BoundLimit type)
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
        /// Creates a SingleValidator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">Float to be set as the minimum value.  Minimum value should be less than maximum value.</param>
        /// <param name="max">Float to be set as the maximum value.  Maximum value should be greater than minimum value. </param>
        public SingleValidator(float min, float max)
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
        /// <param name="item">Float to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(float item)
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
        /// Validates that the item is a float.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is float)
            {
                return this.IsValid((float)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a UInt16 entity.
    /// </summary>
    public class UInt16Validator : IValidator<ushort>
    {
        private bool hasMin;
        private bool hasMax;
        private ushort min;
        private ushort max;

        /// <summary>
        /// Creates a UInt16Validator instance without a minimum nor a maximum value.
        /// </summary>
        public UInt16Validator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a Int16Validator instance with only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="limit">UShort to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public UInt16Validator(ushort limit, BoundLimit type)
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
        /// Creates a UInt16Validator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">UShort to be set as the minimum value.  Minimum value should be less than maximum value. </param>
        /// <param name="max">UShort to be set as the maximum value.  Maximum value should be greater than minimum value. </param>
        public UInt16Validator(ushort min, ushort max)
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
        /// <param name="item">Ushort to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(ushort item)
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
        /// Validates that the item is a ushort.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is ushort)
            {
                return this.IsValid((ushort)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a UInt32 entity.
    /// </summary>
    public class UInt32Validator : IValidator<uint>
    {
        private bool hasMin;
        private bool hasMax;
        private uint min;
        private uint max;

        /// <summary>
        /// Creates a UInt32Validator instance without a Minimum nor a Maximum value.
        /// </summary>
        public UInt32Validator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a UInt32Validator instance with only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="limit">Integer to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public UInt32Validator(uint limit, BoundLimit type)
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
        /// Creates a UInt32Validator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">Integer to be set as the minimum value.  Minimum value should be less than maximum value. </param>
        /// <param name="max">Integer to be set as the maximum value.  Maximum value should be greater than minimum value. </param>
        public UInt32Validator(uint min, uint max)
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
        /// <param name="item">Integer to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(uint item)
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
        /// Validates that the item is an integer.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is uint)
            {
                return this.IsValid((uint)item);
            }
            return false;
        }
    }

    /// <summary>
    /// Provides validation for a UInt64 entity.
    /// </summary>
    public class UInt64Validator : IValidator<ulong>
    {
        private bool hasMin;
        private bool hasMax;
        private ulong min;
        private ulong max;

        /// <summary>
        /// Creates a UInt64Validator instance without a Minimum nor a Maximum value.
        /// </summary>
        public UInt64Validator()
        {
            this.hasMax = false;
            this.hasMin = false;
        }

        /// <summary>
        /// Creates a UInt64Validator instance with only a Minimum or a Maximum value.
        /// </summary>
        /// <param name="limit">ULong value to be set as the minimum or maximum value.</param>
        /// <param name="type">Type of limit, based on enumeration BoundLimit, to be set for this instance.</param>
        public UInt64Validator(ulong limit, BoundLimit type)
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
        /// Creates a UInt64Validator instance with a Minimum and a Maximum value.  Minimum value should be less than maximum value.  If maximum value is not less than mimumim value, only minimum value will be set.
        /// </summary>
        /// <param name="min">ULong to be set as the minimum value.  Minimum value should be less than maximum value. </param>
        /// <param name="max">ULong to be set as the maximum value.  Maximum value should be greater than minimum value. </param>
        public UInt64Validator(ulong min, ulong max)
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
        /// <param name="item">Ulong to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValid(ulong item)
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
        /// Validates that the item is a ulong.
        /// </summary>
        /// <param name="item">Object to be validated.</param>
        /// <returns>Boolean value.</returns>
        public bool IsValidObject(object item)
        {
            if (item is ulong)
            {
                return this.IsValid((ulong)item);
            }
            return false;
        }
    }
}
