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

namespace Osrs.Security
{
    public class SecurityException : System.Security.SecurityException
    {
        [DebuggerStepThrough]
        public SecurityException() { }

        [DebuggerStepThrough]
        public SecurityException(string message) : base(message) { }

        [DebuggerStepThrough]
        public SecurityException(string message, Exception inner) : base(message, inner) { }
    }

    public class UnauthorizedAccessException : SecurityException
    {
        [DebuggerStepThrough]
        public UnauthorizedAccessException() { }

        [DebuggerStepThrough]
        public UnauthorizedAccessException(string message) : base(message) { }

        [DebuggerStepThrough]
        public UnauthorizedAccessException(string message, Exception inner) : base(message, inner) { }
    }

    public class NullContextException : IllegalContextException
    {
        [DebuggerStepThrough]
        public NullContextException() { }

        [DebuggerStepThrough]
        public NullContextException(string message) : base(message) { }

        [DebuggerStepThrough]
        public NullContextException(string message, Exception inner) : base(message, inner) { }
    }

    public class InvalidCredentialException : AuthenticationException
    {
        [DebuggerStepThrough]
        public InvalidCredentialException() { }

        [DebuggerStepThrough]
        public InvalidCredentialException(string message) : base(message) { }

        [DebuggerStepThrough]
        public InvalidCredentialException(string message, Exception inner) : base(message, inner) { }
    }

    public class InsufficientPermissionException : UnauthorizedAccessException
    {
        [DebuggerStepThrough]
        public InsufficientPermissionException() { }

        [DebuggerStepThrough]
        public InsufficientPermissionException(string message) : base(message) { }

        [DebuggerStepThrough]
        public InsufficientPermissionException(string message, Exception inner) : base(message, inner) { }
    }

    public class IllegalContextException : SecurityException
    {
        [DebuggerStepThrough]
        public IllegalContextException() { }

        [DebuggerStepThrough]
        public IllegalContextException(string message) : base(message) { }

        [DebuggerStepThrough]
        public IllegalContextException(string message, Exception inner) : base(message, inner) { }
    }

    public class AuthenticationException : UnauthorizedAccessException
    {
        [DebuggerStepThrough]
        public AuthenticationException() { }

        [DebuggerStepThrough]
        public AuthenticationException(string message) : base(message) { }

        [DebuggerStepThrough]
        public AuthenticationException(string message, Exception inner) : base(message, inner) { }
    }
}
