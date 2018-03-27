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
    public interface IAngle
    {
        double Degrees
        {
            get;
        }

        double Radians
        {
            get;
        }

        double Gradians
        {
            get;
        }

        double ArcSeconds
        {
            get;
        }
    }

    public interface ISignedAngle : IAngle, ISigned
    { }

    //standard angle -- 0.0==right, clockwise to Pi, counterclockwise to -Pi
    public sealed class PlaneAngle : ISignedAngle, ISignedNumeric<PlaneAngle>
    {
        public static readonly PlaneAngle Zero = new PlaneAngle(0.0d);
        public static readonly PlaneAngle NaN = new PlaneAngle(double.NaN);
        public static readonly PlaneAngle NegativeInfinity = new PlaneAngle(double.NegativeInfinity);
        public static readonly PlaneAngle PositiveInfinity = new PlaneAngle(double.PositiveInfinity);

        internal readonly double data;

        public PlaneAngle(double radians)
        {
            if (double.IsInfinity(radians) || double.IsNaN(radians))
                this.data = double.NaN;
            this.data = AngleUtils.Normalize(radians);
        }

        public PlaneAngle(double data, AngularUnit units)
            : this(AngleUtils.GetRadians(data, units))
        { }

        public double Radians
        {
            get { return this.data; }
        }

        public double Degrees
        {
            get { return Math.Round(this.data * Constants.DegreesPerRadian, 2); }
        }

        public double Gradians
        {
            get { return Constants.GradiansPerRadian * this.data; }
        }

        public double ArcSeconds
        {
            get { return Constants.ArcSecondPerRadian * this.data; }
        }

        public bool IsInfinity
        {
            get { return double.IsInfinity(this.data); }
        }

        public bool IsNegativeInfinity
        {
            get { return double.IsNegativeInfinity(this.data); }
        }

        public bool IsPositiveInfinity
        {
            get { return double.IsPositiveInfinity(this.data); }
        }

        public bool IsInfinityOrNaN
        {
            get { return double.IsNaN(this.data) || double.IsInfinity(this.data); }
        }

        public bool IsNaN
        {
            get { return double.IsNaN(this.data); }
        }

        public bool IsPositive
        {
            get { return this.data >= 0.0; }
        }

        public bool IsNegative
        {
            get { return this.data < 0.0; }
        }

        public bool IsLessThan(PlaneAngle other)
        {
            return this.data < other.data;
        }

        public bool IsGreaterThan(PlaneAngle other)
        {
            return this.data > other.data;
        }

        public bool IsLessThanOrEqual(PlaneAngle other)
        {
            return this.data <= other.data;
        }

        public bool IsGreaterThanOrEqual(PlaneAngle other)
        {
            return this.data >= other.data;
        }

        public PlaneAngle Abs()
        {
            return new PlaneAngle(Math.Abs(this.data));
        }

        public PlaneAngle Negate()
        {
            return new PlaneAngle(-this.data);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="value">An object to compare with this object.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object value)
        {
            if (value is PlaneAngle)
            {
                return CompareTo((PlaneAngle)value);
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Compares the current angle to the specified angle.
        /// </summary>
        /// <param name="value">An angle.</param>
        /// <returns>
        /// Zero if the angles are equal.
        /// Less than zero if the current angle is less than the <paramref name="value"/>.
        /// Greater than zero if the current angle is greater than the <paramref name="value"/>.
        /// </returns>
        public int CompareTo(PlaneAngle value)
        {
            return Math.Sign(this.data - value.data);
        }

        public bool Equals(PlaneAngle value)
        {
            return this.data == value.data;
        }

        public PlaneAngle Add(PlaneAngle other)
        {
            return new PlaneAngle(this.Radians + other.Radians);
        }

        public PlaneAngle Divide(PlaneAngle other)
        {
            return new PlaneAngle(this.Radians / other.Radians);
        }

        public PlaneAngle Modulo(PlaneAngle other)
        {
            return new PlaneAngle(this.Radians % other.Radians);
        }

        public PlaneAngle Multiply(PlaneAngle other)
        {
            return new PlaneAngle(this.Radians * other.Radians);
        }

        public PlaneAngle Subtract(PlaneAngle other)
        {
            return new PlaneAngle(this.Radians - other.Radians);
        }

        public override bool Equals(object value)
        {
            if (value is PlaneAngle)
            {
                return Equals((PlaneAngle)value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.data.GetHashCode();
        }

        #region Statics
        public static PlaneAngle Add(PlaneAngle value1, double value2)
        {
            return new PlaneAngle(value1.data + value2);
        }

        public static PlaneAngle Add(PlaneAngle value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1.data + value2.data);
        }

        public static PlaneAngle Divide(PlaneAngle value1, double value2)
        {
            return new PlaneAngle(value1.data / value2);
        }

        public static double Divide(PlaneAngle value1, PlaneAngle value2)
        {
            return value1.data / value2.data;
        }

        public static PlaneAngle Max(PlaneAngle value1, PlaneAngle value2)
        {
            return new PlaneAngle(Math.Max(value1.data, value2.data));
        }

        public static PlaneAngle Min(PlaneAngle value1, PlaneAngle value2)
        {
            return new PlaneAngle(Math.Min(value1.data, value2.data));
        }

        public static PlaneAngle Modulo(PlaneAngle value1, double value2)
        {
            return new PlaneAngle(value1.data % value2);
        }

        public static PlaneAngle Modulo(double value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1 % value2.data);
        }

        public static PlaneAngle Modulo(PlaneAngle value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1.data % value2.data);
        }

        public static PlaneAngle Multiply(PlaneAngle value1, double value2)
        {
            return new PlaneAngle(value1.data * value2);
        }

        public static PlaneAngle Multiply(PlaneAngle value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1.data * value2.data);
        }

        public static PlaneAngle Subtract(PlaneAngle value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1.data - value2.data);
        }

        public static PlaneAngle Subtract(double value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1 - value2.data);
        }

        public static PlaneAngle Subtract(PlaneAngle value1, double value2)
        {
            return new PlaneAngle(value1.data - value2);
        }

        public static int Sign(PlaneAngle value)
        {
            return Math.Sign(value.data);
        }

        public static PlaneAngle Acos(double value)
        {
            return new PlaneAngle(Math.Acos(value));
        }

        public static PlaneAngle Asin(double value)
        {
            return new PlaneAngle(Math.Asin(value));
        }

        public static PlaneAngle Atan(double value)
        {
            return new PlaneAngle(Math.Atan(value));
        }

        public static PlaneAngle Atan2(double y, double x)
        {
            return new PlaneAngle(Math.Atan2(y, x));
        }

        public static double Cos(PlaneAngle value)
        {
            return Math.Cos(value.data);
        }

        public static double Cosh(PlaneAngle value)
        {
            return Math.Cosh(value.data);
        }

        public static double Sin(PlaneAngle value)
        {
            return Math.Sin(value.data);
        }

        public static double Sinh(PlaneAngle value)
        {
            return Math.Sinh(value.data);
        }

        public static double Tan(PlaneAngle value)
        {
            return Math.Tan(value.data);
        }

        public static double Tanh(PlaneAngle value)
        {
            return Math.Tanh(value.data);
        }

        public static PlaneAngle Blend(PlaneAngle value1, PlaneAngle value2, double amount)
        {
            return new PlaneAngle(value1.data + amount * (value2.data - value1.data));
        }

        public static PlaneAngle Clamp(PlaneAngle value, PlaneAngle minimum, PlaneAngle maximum)
        {
            return new PlaneAngle(Math.Max(minimum.data, Math.Min(value.data, maximum.data)));
        }
        #endregion

        #region operators
        public static bool operator ==(PlaneAngle value1, PlaneAngle value2)
        {
            return value1.data == value2.data;
        }

        public static bool operator !=(PlaneAngle value1, PlaneAngle value2)
        {
            return value1.data != value2.data;
        }

        public static bool operator <(PlaneAngle value1, PlaneAngle value2)
        {
            return value1.data < value2.data;
        }

        public static bool operator <=(PlaneAngle value1, PlaneAngle value2)
        {
            return value1.data <= value2.data;
        }

        public static bool operator >(PlaneAngle value1, PlaneAngle value2)
        {
            return value1.data > value2.data;
        }

        public static bool operator >=(PlaneAngle value1, PlaneAngle value2)
        {
            return value1.data >= value2.data;
        }

        public static PlaneAngle operator -(PlaneAngle value)
        {
            return new PlaneAngle(-value.data);
        }

        public static PlaneAngle operator +(PlaneAngle value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1.data + value2.data);
        }

        public static PlaneAngle operator -(PlaneAngle value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1.data - value2.data);
        }

        public static PlaneAngle operator *(PlaneAngle value1, double value2)
        {
            return new PlaneAngle(value1.data * value2);
        }

        public static PlaneAngle operator *(double value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1 * value2.data);
        }

        public static PlaneAngle operator /(PlaneAngle value1, double value2)
        {
            return new PlaneAngle(value1.data / value2);
        }

        public static double operator /(PlaneAngle value1, PlaneAngle value2)
        {
            return value1.data / value2.data;
        }

        public static PlaneAngle operator %(PlaneAngle value1, PlaneAngle value2)
        {
            return new PlaneAngle(value1.data % value2.data);
        }

        public static explicit operator PlaneAngle(double radians)
        {
            return new PlaneAngle(radians);
        }

        public static implicit operator double(PlaneAngle angle)
        {
            return angle.data;
        }
        #endregion
    }

    //compass angle -- 0.0==up/north, clockwise to 2Pi
    public sealed class CompassAngle : IAngle, INumeric<CompassAngle>
    {
        public static readonly CompassAngle Zero = new CompassAngle(0.0d);
        public static readonly CompassAngle NaN = new CompassAngle(double.NaN);
        public static readonly CompassAngle NegativeInfinity = new CompassAngle(double.NegativeInfinity);
        public static readonly CompassAngle PositiveInfinity = new CompassAngle(double.PositiveInfinity);

        internal readonly double data;

        public CompassAngle(double radians)
        {
            if (double.IsInfinity(radians) || double.IsNaN(radians))
                this.data = double.NaN;
            this.data = AngleUtils.NormalizePositive(radians);
        }

        public CompassAngle(double data, AngularUnit units)
            : this(AngleUtils.GetRadians(data, units))
        { }

        public double Radians
        {
            get { return this.data; }
        }

        public double Degrees
        {
            get { return Math.Round(this.data * Constants.DegreesPerRadian, 2); }
        }

        public double Gradians
        {
            get { return Constants.GradiansPerRadian * this.data; }
        }

        public double ArcSeconds
        {
            get { return Constants.ArcSecondPerRadian * this.data; }
        }

        public bool IsInfinity
        {
            get { return double.IsInfinity(this.data); }
        }

        public bool IsInfinityOrNaN
        {
            get { return double.IsNaN(this.data) || double.IsInfinity(this.data); }
        }

        public bool IsNaN
        {
            get { return double.IsNaN(this.data); }
        }

        public bool IsLessThan(CompassAngle other)
        {
            return this.data < other.data;
        }

        public bool IsGreaterThan(CompassAngle other)
        {
            return this.data > other.data;
        }

        public bool IsLessThanOrEqual(CompassAngle other)
        {
            return this.data <= other.data;
        }

        public bool IsGreaterThanOrEqual(CompassAngle other)
        {
            return this.data >= other.data;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="value">An object to compare with this object.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object value)
        {
            if (value is CompassAngle)
            {
                return CompareTo((CompassAngle)value);
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Compares the current angle to the specified angle.
        /// </summary>
        /// <param name="value">An angle.</param>
        /// <returns>
        /// Zero if the angles are equal.
        /// Less than zero if the current angle is less than the <paramref name="value"/>.
        /// Greater than zero if the current angle is greater than the <paramref name="value"/>.
        /// </returns>
        public int CompareTo(CompassAngle value)
        {
            return Math.Sign(this.data - value.data);
        }

        public bool Equals(CompassAngle value)
        {
            return this.data == value.data;
        }

        public CompassAngle Add(CompassAngle other)
        {
            return new CompassAngle(this.Radians + other.Radians);
        }

        public CompassAngle Divide(CompassAngle other)
        {
            return new CompassAngle(this.Radians / other.Radians);
        }

        public CompassAngle Modulo(CompassAngle other)
        {
            return new CompassAngle(this.Radians % other.Radians);
        }

        public CompassAngle Multiply(CompassAngle other)
        {
            return new CompassAngle(this.Radians * other.Radians);
        }

        public CompassAngle Subtract(CompassAngle other)
        {
            return new CompassAngle(this.Radians - other.Radians);
        }

        public override bool Equals(object value)
        {
            if (value is CompassAngle)
            {
                return Equals((CompassAngle)value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.data.GetHashCode();
        }

        #region statics
        public static CompassAngle Add(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(value1.data + value2.data);
        }

        public static CompassAngle Add(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data + value2);
        }

        public static CompassAngle Divide(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data / value2);
        }

        public static double Divide(double value1, CompassAngle value2)
        {
            return value1 / value2.data;
        }

        public static double Divide(CompassAngle value1, CompassAngle value2)
        {
            return value1.data / value2.data;
        }

        public static CompassAngle Max(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(Math.Max(value1.data, value2.data));
        }

        public static CompassAngle Min(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(Math.Min(value1.data, value2.data));
        }

        public static CompassAngle Modulo(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(value1.data % value2.data);
        }

        public static CompassAngle Modulo(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data % value2);
        }

        public static CompassAngle Modulo(double value1, CompassAngle value2)
        {
            return new CompassAngle(value1 % value2.data);
        }

        public static CompassAngle Multiply(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data * value2);
        }

        public static CompassAngle Multiply(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(value1.data * value2.data);
        }

        public static CompassAngle Subtract(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(value1.data - value2.data);
        }

        public static CompassAngle Subtract(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data - value2);
        }

        public static CompassAngle Subtract(double value1, CompassAngle value2)
        {
            return new CompassAngle(value1 - value2.data);
        }

        public static CompassAngle Acos(double value)
        {
            return new CompassAngle(Math.Acos(value));
        }

        public static CompassAngle Asin(double value)
        {
            return new CompassAngle(Math.Asin(value));
        }

        public static CompassAngle Atan(double value)
        {
            return new CompassAngle(Math.Atan(value));
        }

        public static CompassAngle Atan2(double y, double x)
        {
            return new CompassAngle(Math.Atan2(y, x));
        }

        public static double Cos(CompassAngle value)
        {
            return Math.Cos(value.data);
        }

        public static double Cosh(CompassAngle value)
        {
            return Math.Cosh(value.data);
        }

        public static double Sin(CompassAngle value)
        {
            return Math.Sin(value.data);
        }

        public static double Sinh(CompassAngle value)
        {
            return Math.Sinh(value.data);
        }

        public static double Tan(CompassAngle value)
        {
            return Math.Tan(value.data);
        }

        public static double Tanh(CompassAngle value)
        {
            return Math.Tanh(value.data);
        }
        #endregion

        #region operators
        public static bool operator ==(CompassAngle value1, CompassAngle value2)
        {
            return value1.data == value2.data;
        }

        public static bool operator !=(CompassAngle value1, CompassAngle value2)
        {
            return value1.data != value2.data;
        }

        public static bool operator <(CompassAngle value1, CompassAngle value2)
        {
            return value1.data < value2.data;
        }

        public static bool operator <=(CompassAngle value1, CompassAngle value2)
        {
            return value1.data <= value2.data;
        }

        public static bool operator >(CompassAngle value1, CompassAngle value2)
        {
            return value1.data > value2.data;
        }

        public static bool operator >=(CompassAngle value1, CompassAngle value2)
        {
            return value1.data >= value2.data;
        }

        public static CompassAngle operator +(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(value1.data + value2.data);
        }

        public static CompassAngle operator +(double value1, CompassAngle value2)
        {
            return new CompassAngle(value1 + value2.data);
        }

        public static CompassAngle operator +(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data + value2);
        }

        public static CompassAngle operator -(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(value1.data - value2.data);
        }

        public static CompassAngle operator -(double value1, CompassAngle value2)
        {
            return new CompassAngle(value1 - value2.data);
        }

        public static CompassAngle operator -(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data - value2);
        }

        public static CompassAngle operator *(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(value1.data * value2.data);
        }

        public static CompassAngle operator *(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data * value2);
        }

        public static CompassAngle operator *(double value1, CompassAngle value2)
        {
            return new CompassAngle(value1 * value2.data);
        }

        public static CompassAngle operator /(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(value1.data / value2.data);
        }

        public static CompassAngle operator /(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data / value2);
        }

        public static CompassAngle operator /(double value1, CompassAngle value2)
        {
            return new CompassAngle(value1 / value2.data);
        }

        public static CompassAngle operator %(CompassAngle value1, CompassAngle value2)
        {
            return new CompassAngle(value1.data % value2.data);
        }

        public static CompassAngle operator %(double value1, CompassAngle value2)
        {
            return new CompassAngle(value1 % value2.data);
        }

        public static CompassAngle operator %(CompassAngle value1, double value2)
        {
            return new CompassAngle(value1.data % value2);
        }

        public static implicit operator double(CompassAngle value)
        {
            return value.data;
        }

        public static explicit operator CompassAngle(double value)
        {
            return new CompassAngle(value);
        }
        #endregion
    }
}
