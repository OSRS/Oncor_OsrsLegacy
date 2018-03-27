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

namespace Osrs
{
    /// <summary>
    /// This class universal event arguments for transporting single value.
    /// </summary>
    /// <typeparam name="T">Event data.</typeparam>
    public class EventArgs<T> : EventArgs
    {
        private T value;

        /// <summary>
        /// Gets event data.
        /// </summary>
        public T Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Event data.</param>
        public EventArgs(T value)
        {
            this.value = value;
        }
    }

    /// <summary>
    /// This class provides data for error events and methods.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        private Exception exception = null;

        /// <summary>
        /// Gets exception.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>exception</b> is null reference value.</exception>
        public ExceptionEventArgs(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            this.exception = exception;
        }
    }
}
