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
    public enum Inclusivity
    {
        Inclusive,
        Exclusive,
    }

    /// <summary>
    /// Represents the basic calculations between numeric types such as: +,-,*,/,%, in an interface.  This allows referring to generic types and performing calculations.
    /// The two type parameters allow for wrapper types that perform operations upon the native types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface ICalculable<T, V>
    {
        T Add(V other);
        T Subtract(V other);
        T Multiply(V other);
        T Divide(V other);
        T Modulo(V other);
    }

    /// <summary>
    /// Represents the basic calculations between numeric types such as: +,-,*,/,%, in an interface.  This allows referring to generic types and performing calculations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICalculable<T> : ICalculable<T, T>
    { }

    /// <summary>
    /// Represents a number type that has the extended operations above the standard +,-,*,/,%.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface IExtendedCalculable<T, V> : ICalculable<T, V>
    {
        T Exp();
        T Log();
        T Log(V baseNumber);
        T Pow(V factor);
        T SqRt();
    }

    /// <summary>
    /// Represents a number type that has the extended operations above the standard +,-,*,/,%.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExtendedCalculable<T> : IExtendedCalculable<T, T>
    { }

    /// <summary>
    /// Represents a value type that is logically comparable as numbers are for relative valuation / positioning.
    /// Any type that implements IComparable, could also implement this interface.
    /// This interface is meant to imply a numeric type, rather than just a comparable type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface ILogicallyComparable<T, V> : IEquatable<V>, IComparable<V>
    {
        bool IsLessThan(V other);
        bool IsGreaterThan(V other);
        bool IsLessThanOrEqual(V other);
        bool IsGreaterThanOrEqual(V other);
    }

    /// <summary>
    /// Represents a value type that is logically comparable as numbers are for relative valuation / positioning.
    /// Any type that implements IComparable, could also implement this interface.
    /// This interface is meant to imply a numeric type, rather than just a comparable type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogicallyComparable<T> : ILogicallyComparable<T, T>
    { }

    public interface INumber<T> : ICalculable<T>, ILogicallyComparable<T>
    { }

    public interface ISignedNumber<T> : INumber<T>, ISigned<T>
    { }

    public interface INumeric<T> : INumber<T>
    {
        bool IsNaN
        {
            get;
        }

        bool IsInfinity
        {
            get;
        }

        bool IsInfinityOrNaN
        {
            get;
        }
    }

    public interface ISignedNumeric<T> : INumeric<T>, ISignedNumber<T>
    {
        bool IsPositiveInfinity
        {
            get;
        }

        bool IsNegativeInfinity
        {
            get;
        }
    }

    /// <summary>
    /// Container type for other values to give numeric semantics
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INumericValue<T, V> : INumeric<V>
    {
    }

    /// <summary>
    /// Container type for other values to give numeric semantics
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISignedNumericValue<T, V> : INumericValue<T, V>, ISignedNumeric<V>
    {
    }

    /// <summary>
    /// Represents a signed value type that may be positive or negative.
    /// </summary>
    public interface ISigned
    {
        bool IsPositive
        {
            get;
        }

        bool IsNegative
        {
            get;
        }
    }

    /// <summary>
    /// Represents a signed value type that can be converted to the absolute value of itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISigned<T> : ISigned
    {
        T Abs();
        T Negate();
    }
}
