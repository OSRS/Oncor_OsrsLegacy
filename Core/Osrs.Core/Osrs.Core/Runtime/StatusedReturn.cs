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

namespace Osrs.Runtime
{
    public enum OperationStatus
    {
        /// <summary>
        /// Worst class of error, total failure, unrecoverable. System may or may not be runnable in the future.  Uncertain as to state of data, probable data loss or corruption.  Need to "re-try" current operation and possibly perform some "clean-up" such as a possible existence check and delete.  May be that the system needs to restart before any operation can succeed.
        /// </summary>
        SystemFault = 6,
        /// <summary>
        /// Critical failure, unrecoverable operation.  May result in unstable state, including possible data loss or corruption.  Often a "broken" connection during some operation, may be a partial write.  Need to "re-try" current operation and possibly perform some "clean-up" such as a possible existence check and delete.
        /// </summary>
        CriticalFailure = 5,
        /// <summary>
        /// Non-Critical failure, possibly unrecoverable.  No unstable state, operation failed. Often associated with inability to "connect" to external resource. No data loss or corruption.  Need to "re-try" current operation.
        /// </summary>
        NonCriticalFailure = 4,
        /// <summary>
        /// Successful operation with some errors and side-effects.  Generally associated with "collisions" and "clean-up" after operation, such as closing a connection or file. No data loss or corruption, but there are side-effects.  The data of the method may not be exactly what was expected.  May need to verify result before continuing.
        /// </summary>
        SuccessWithSideEffectsAndErrors = 3,
        /// <summary>
        /// Successful operation with some errors.  Generally associated with "clean-up" after operation, such as closing a connection or file. No data loss, corruption, or side-effect.  No need to "re-try" current operation.
        /// </summary>
        SuccessWithErrors = 2,
        /// <summary>
        /// Successful operation with some side effects.  Generally associated with threading or operation "collisions", the data of the method may not be exactly what was expected.  May need to verify result before continuing.
        /// </summary>
        SuccessWithSideEffects = 1,
        /// <summary>
        /// Success, no error, no side-effects.
        /// </summary>
        Success = 0
    }

    public interface IStatusedReturn
    {
        OperationStatus Status
        {
            get;
        }
        Exception Exception
        {
            get;
        }
        string Message
        {
            get;
        }
    }

    public sealed class StatusedReturn<T> : IStatusedReturn
    {
        private readonly T returnValue = default(T);
        public T ReturnValue
        {
            get { return this.returnValue; }
        }

        private readonly OperationStatus status = OperationStatus.Success;
        public OperationStatus Status
        {
            get { return this.status; }
        }

        private readonly Exception e = null;
        public Exception Exception
        {
            get { return this.e; }
        }

        private readonly string message = null;
        public string Message
        {
            get { return this.message; }
        }

        private readonly int returnCode = int.MinValue;
        public int ReturnCode
        {
            get { return this.returnCode; }
        }

        public StatusedReturn()
        {
        }
        public StatusedReturn(T item)
        {
        }
        public StatusedReturn(T item, OperationStatus status)
            : this(item, status, null, null, int.MinValue)
        {
        }
        public StatusedReturn(T item, OperationStatus status, Exception e)
            : this(item, status, e, null, int.MinValue)
        {
        }
        public StatusedReturn(T item, OperationStatus status, string message)
            : this(item, status, null, message, int.MinValue)
        {
        }
        public StatusedReturn(T item, OperationStatus status, int returnCode)
            : this(item, status, null, null, returnCode)
        {
        }
        public StatusedReturn(T item, OperationStatus status, Exception e, string message)
            : this(item, status, e, message, int.MinValue)
        {
        }
        public StatusedReturn(T item, OperationStatus status, Exception e, int returnCode)
            : this(item, status, e, null, returnCode)
        {
        }
        public StatusedReturn(T item, OperationStatus status, Exception e, string message, int returnCode)
        {
            this.returnValue = item;
            this.status = status;
            this.e = e;
            this.message = message;
            this.returnCode = returnCode;
        }
        public StatusedReturn(OperationStatus status)
            : this(default(T), status, null, null, int.MinValue)
        {
        }
        public StatusedReturn(OperationStatus status, Exception e)
            : this(default(T), status, e, null, int.MinValue)
        {
        }
        public StatusedReturn(OperationStatus status, string message)
            : this(default(T), status, null, message, int.MinValue)
        {
        }
        public StatusedReturn(OperationStatus status, int returnCode)
            : this(default(T), status, null, null, returnCode)
        {
        }
        public StatusedReturn(OperationStatus status, Exception e, string message)
            : this(default(T), status, e, message, int.MinValue)
        {
        }
        public StatusedReturn(OperationStatus status, Exception e, int returnCode)
            : this(default(T), status, e, null, returnCode)
        {
        }
        public StatusedReturn(OperationStatus status, Exception e, string message, int returnCode)
            : this(default(T), status, e, message, returnCode)
        {
        }
        public StatusedReturn(Exception e)
            : this(default(T), OperationStatus.Success, e, null, int.MinValue)
        {
        }
        public StatusedReturn(Exception e, string message)
            : this(default(T), OperationStatus.Success, e, null, int.MinValue)
        {
        }
        public StatusedReturn(Exception e, int returnCode)
            : this(default(T), OperationStatus.Success, e, null, returnCode)
        {
        }
        public StatusedReturn(Exception e, string message, int returnCode)
            : this(default(T), OperationStatus.Success, e, message, returnCode)
        {
        }
        public StatusedReturn(string message)
            : this(default(T), OperationStatus.Success, null, message, int.MinValue)
        {
        }
        public StatusedReturn(string message, int returnCode)
            : this(default(T), OperationStatus.Success, null, message, returnCode)
        {
        }
        public StatusedReturn(int returnCode)
            : this(default(T), OperationStatus.Success, null, null, returnCode)
        {
        }
    }

    public sealed class StatusedReturnVoid : IStatusedReturn
    {
        private OperationStatus status = OperationStatus.Success;
        public OperationStatus Status
        {
            get { return this.status; }
        }

        private Exception e = null;
        public Exception Exception
        {
            get { return this.e; }
        }

        private string message = null;
        public string Message
        {
            get { return this.message; }
        }

        private int returnCode = int.MinValue;
        public int ReturnCode
        {
            get { return this.returnCode; }
        }

        public StatusedReturnVoid()
        {
        }
        public StatusedReturnVoid(OperationStatus status)
            : this(status, null, null, int.MinValue)
        {
        }
        public StatusedReturnVoid(OperationStatus status, Exception e)
            : this(status, e, null, int.MinValue)
        {
        }
        public StatusedReturnVoid(OperationStatus status, string message)
            : this(status, null, message, int.MinValue)
        {
        }
        public StatusedReturnVoid(OperationStatus status, int returnCode)
            : this(status, null, null, returnCode)
        {
        }
        public StatusedReturnVoid(OperationStatus status, Exception e, string message)
            : this(status, e, message, int.MinValue)
        {
        }
        public StatusedReturnVoid(OperationStatus status, Exception e, int returnCode)
            : this(status, e, null, returnCode)
        {
        }
        public StatusedReturnVoid(OperationStatus status, Exception e, string message, int returnCode)
        {
            this.status = status;
            this.e = e;
            this.message = message;
            this.returnCode = returnCode;
        }
        public StatusedReturnVoid(Exception e)
            : this(OperationStatus.Success, e, null, int.MinValue)
        {
        }
        public StatusedReturnVoid(Exception e, string message)
            : this(OperationStatus.Success, e, null, int.MinValue)
        {
        }
        public StatusedReturnVoid(Exception e, int returnCode)
            : this(OperationStatus.Success, e, null, returnCode)
        {
        }
        public StatusedReturnVoid(Exception e, string message, int returnCode)
            : this(OperationStatus.Success, e, message, returnCode)
        {
        }
        public StatusedReturnVoid(string message)
            : this(OperationStatus.Success, null, message, int.MinValue)
        {
        }
        public StatusedReturnVoid(string message, int returnCode)
            : this(OperationStatus.Success, null, message, returnCode)
        {
        }
        public StatusedReturnVoid(int returnCode)
            : this(OperationStatus.Success, null, null, returnCode)
        {
        }
    }
}
