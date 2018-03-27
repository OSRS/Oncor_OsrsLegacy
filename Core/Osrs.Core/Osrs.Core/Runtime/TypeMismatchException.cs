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
using System.Diagnostics;

namespace Osrs.Runtime
{
    public sealed class SystemOfflineException : Exception
    {
        [DebuggerStepThrough]
        public SystemOfflineException() { }

        [DebuggerStepThrough]
        public SystemOfflineException(string message) : base(message) { }

        [DebuggerStepThrough]
        public SystemOfflineException(string message, Exception inner) : base(message, inner) { }
    }

    public class TypeMismatchException : Exception
    {
        [DebuggerStepThrough]
        public TypeMismatchException() { }

        [DebuggerStepThrough]
        public TypeMismatchException(string message) : base(message) { }

        [DebuggerStepThrough]
        public TypeMismatchException(string message, Exception inner) : base(message, inner) { }
    }

    public class InstantiationException : Exception
    {
        [DebuggerStepThrough]
        public InstantiationException() { }

        [DebuggerStepThrough]
        public InstantiationException(string message) : base(message) { }

        [DebuggerStepThrough]
        public InstantiationException(string message, Exception inner) : base(message, inner) { }
    }

    public class StringEmptyOrNullException : Exception
    {
        [DebuggerStepThrough]
        public StringEmptyOrNullException() { }

        [DebuggerStepThrough]
        public StringEmptyOrNullException(string message) : base(message) { }

        [DebuggerStepThrough]
        public StringEmptyOrNullException(string message, Exception inner) : base(message, inner) { }
    }
}
