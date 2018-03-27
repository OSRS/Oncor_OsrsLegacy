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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Osrs
{
    /// <summary>
    /// Enumeration to provide an additional value over a boolean.  Instead of just true/false, the tristate provides an unknown value to represent a state that may be true or false but is currently uncertain.
    /// </summary>
    public enum TriStateResult
    {
        False = -1,
        Unknown = 0,
        True = 1
    }

    public static class CoreUtils
    {
        public const string NO_ARGS_NULL = "No arguments may be null";
        public const string NO_ARGS_EMPTY = "No arguments may be empty";
        public const string NO_ARGS_NULLOREMPTY = "No arguments may be null or empty";

        private const double oneSeventh = 1.0d / 7.0d;

        /// <summary>
        /// Returns the date matching the day of year provided, starting at day 0 (Jan 1).
        /// </summary>
        /// <param name="year"></param>
        /// <param name="dayOfYear"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static DateTime DateFromDOY(ushort year, ushort dayOfYear)
        {
            DateTime when = new DateTime(year, 1, 1);
            return when.AddDays(dayOfYear);
        }

        [DebuggerStepThrough]
        public static DateTime DateFromDOY(DateTime firstDayOfYear, ushort dayOfYear)
        {
            return firstDayOfYear.AddDays(dayOfYear);
        }

        [DebuggerStepThrough]
        public static DateTime DateFromDOY(ushort year, ushort firstMonthOfYear, ushort dayOfYear)
        {
            return DateFromDOY(year, firstMonthOfYear, 1, dayOfYear);
        }

        [DebuggerStepThrough]
        public static DateTime DateFromDOY(ushort year, ushort firstMonthOfYear, ushort firstDayOfYear, ushort dayOfYear)
        {
            DateTime when = new DateTime(year, firstMonthOfYear, firstDayOfYear);
            return DateFromDOY(when, dayOfYear);
        }

        /// <summary>
        /// Which week of the year is the portrayed date in.
        /// Based upon the elapsed weeks of the year, not on the calendar weeks.
        /// So, how many weeks have elapsed since the beginning of the year based upon 7 day weeks.
        /// </summary>
        /// <param name="when">The date for the elapsed week measurement</param>
        /// <returns>and integer of the number of weeks elapsed from 0-52</returns>
        [DebuggerStepThrough]
        public static int ElapsedWeeksInYear(this DateTime when)
        {
            return (int)(when.DayOfYear * oneSeventh);
        }

        /// <summary>
        /// Which week of the year is the portrayed date in.
        /// Based upon the week number within the year, not on the calendar weeks.
        /// So, how many weeks have elapsed since the beginning of the year based upon 7 day weeks.
        /// </summary>
        /// <param name="when">The date for the elapsed week measurement</param>
        /// <returns>and integer of the number of weeks elapsed from 0-52</returns>
        [DebuggerStepThrough]
        public static int WeekOfYear(this DateTime when)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(when,
                CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
        }

        [DebuggerStepThrough]
        public static int Millenia(DateTime from, DateTime to)
        {
            return (int)((to.Year - from.Year) * 0.001);
        }

        [DebuggerStepThrough]
        public static int Centuries(DateTime from, DateTime to)
        {
            return (int)((to.Year - from.Year) * 0.01);
        }

        [DebuggerStepThrough]
        public static int Decades(DateTime from, DateTime to)
        {
            return (int)((to.Year - from.Year) * 0.1);
        }

        [DebuggerStepThrough]
        public static int Years(DateTime from, DateTime to)
        {
            return (int)(Months(from, to) * 0.083333333333333333333333333333333);
        }

        [DebuggerStepThrough]
        public static long Months(DateTime from, DateTime to)
        {
            int fMonth = from.Month;
            int tMonth = to.Month;
            if (fMonth == tMonth)
                return (long)(to.Year - from.Year) * 12L;
            int fYear = from.Year;
            int tYear = to.Year;
            if (fYear == tYear)
                return (long)(tMonth - fMonth);

            long years = (long)(tYear - fYear) * 12L;

            if (years > 0)
                return years - 12L + (long)(12 - fMonth + tMonth);
            else    //to is earlier than from
                return years + 12L - (long)(12 - tMonth + fMonth);
        }

        [DebuggerStepThrough]
        public static long Weeks(DateTime from, DateTime to)
        {
            return (long)(Days(from, to) * 0.14285714285714285714285714285714);
        }

        [DebuggerStepThrough]
        public static long Days(DateTime from, DateTime to)
        {
            return (long)(to.Subtract(from).TotalDays);
        }

        [DebuggerStepThrough]
        public static long Hours(DateTime from, DateTime to)
        {
            return (long)(to.Subtract(from).TotalHours);
        }

        [DebuggerStepThrough]
        public static long Minutes(DateTime from, DateTime to)
        {
            return (long)(to.Subtract(from).TotalMinutes);
        }

        [DebuggerStepThrough]
        public static long Seconds(DateTime from, DateTime to)
        {
            return (long)(to.Subtract(from).TotalSeconds);
        }

        [DebuggerStepThrough]
        public static long Milliseconds(DateTime from, DateTime to)
        {
            return (long)(to.Subtract(from).TotalMilliseconds);
        }

        [DebuggerStepThrough]
        public static bool IsLeap(int year)
        {
            if (year % 4 == 0)
            {
                if (year % 400 == 0)
                    return true;
                else if (year % 100 == 0)
                    return false;
                return true;
            }
            return false;
        }

        [DebuggerStepThrough]
        public static short DaysInMonth(DateTime when)
        {
            if (when.Month != 2)
                return DaysInMonth(when.Month);
            return (short)(IsLeap(when.Year) ? 29 : 28);
        }

        [DebuggerStepThrough]
        public static short DaysInMonth(int month, int year)
        {
            if (month != 2)
                return DaysInMonth(month);
            return (short)(IsLeap(year) ? 29 : 28);
        }

        //not corrected for leap
        [DebuggerStepThrough]
        public static short DaysInMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return 31;
                case 2:
                    return 28;
                case 3:
                    return 31;
                case 4:
                    return 30;
                case 5:
                    return 31;
                case 6:
                    return 30;
                case 7:
                    return 31;
                case 8:
                    return 31;
                case 9:
                    return 30;
                case 10:
                    return 31;
                case 11:
                    return 30;
                case 12:
                    return 31;
            }
            return 0;
        }

        [DebuggerStepThrough]
        public static IList<T> ToList<T>(this IEnumerable<T> coll)
        {
            if (coll == null)
                return null;
            List<T> tmp = new List<T>();
            foreach (T item in coll)
                tmp.Add(item);
            return tmp;
        }

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this IEmpty e)
        {
            return (e == null || e.IsEmpty);
        }

        [DebuggerStepThrough]
        public static bool IsObjectNull(this object o)
        {
            return o == null;
        }

        [DebuggerStepThrough]
        public static bool Is<T>(this object o)
        {
            return o is T;
        }

        [DebuggerStepThrough]
        public static bool Is(this object o, Type t)
        {
            if (o == null)
            {
                return true; //null is assignable to anything
            }

            return t.GetTypeInfo().IsAssignableFrom(o.GetType().GetTypeInfo());
        }

        [DebuggerStepThrough]
        public static T As<T>(this object o)
        {
            #pragma warning disable 168
            try
            {
                return (T)o;
            }
            catch (InvalidCastException e)
            {
                return default(T);
            }
            #pragma warning restore 168
        }

        /// <summary>
        /// Is this date before "to"
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsBefore(this DateTime from, DateTime to)
        {
            return from.Ticks < to.Ticks;
        }

        /// <summary>
        /// Is this date after "to"
        /// </summary>
        /// <param name="from">this date</param>
        /// <param name="to">to date</param>
        /// <returns>true if from is after to, otherwise false</returns>
        [DebuggerStepThrough]
        public static bool IsAfter(this DateTime from, DateTime to)
        {
            return from.Ticks > to.Ticks;
        }

        public delegate T MapOp<T>(T value);
        public delegate T ReduceOp<T>(IEnumerable<T> value);

        //TODO -- Add threading
        [DebuggerStepThrough]
        public static void Map<T>(IList<T> values, MapOp<T> op)
        {
            if (values == null || op == null)
                throw new ArgumentNullException();
            for (int i = 0; i < values.Count; i++)
                values[i] = op(values[i]);
        }

        [DebuggerStepThrough]
        public static T Reduce<T>(IList<T> values, ReduceOp<T> red)
        {
            if (values == null || red == null)
                throw new ArgumentNullException();
            return red(values);
        }

        [DebuggerStepThrough]
        public static T Reduce<T>(IList<T> values, MapOp<T> op, ReduceOp<T> red)
        {
            Map(values, op);
            return red(values);
        }

        private static readonly int cores = Environment.ProcessorCount;
        public static int CpuCoreCount
        {
            [DebuggerStepThrough]
            get { return cores; }
        }

        private static readonly int threads = (int)Math.Floor((float)cores * 1.5f);
        public static int CpuLoadingThreadCount
        {
            [DebuggerStepThrough]
            get { return threads; }
        }
    }
}
