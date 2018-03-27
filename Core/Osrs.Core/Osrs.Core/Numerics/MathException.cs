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

namespace Osrs.Numerics
{
    public class MathException : Exception
    {
        [DebuggerStepThrough]
        public MathException() { }

        [DebuggerStepThrough]
        public MathException(string message) : base(message) { }

        [DebuggerStepThrough]
        public MathException(string message, Exception inner) : base(message, inner) { }
    }

    public class UnderflowException : MathException
    {
        [DebuggerStepThrough]
        public UnderflowException() { }

        [DebuggerStepThrough]
        public UnderflowException(string message) : base(message) { }

        [DebuggerStepThrough]
        public UnderflowException(string message, Exception inner) : base(message, inner) { }
    }

    public class SignException : MathException
    {
        [DebuggerStepThrough]
        public SignException() { }

        [DebuggerStepThrough]
        public SignException(string message) : base(message) { }

        [DebuggerStepThrough]
        public SignException(string message, Exception inner) : base(message, inner) { }
    }

    public class OverflowException : MathException
    {
        [DebuggerStepThrough]
        public OverflowException() { }

        [DebuggerStepThrough]
        public OverflowException(string message) : base(message) { }

        [DebuggerStepThrough]
        public OverflowException(string message, Exception inner) : base(message, inner) { }
    }

    public class LimitException : MathException
    {
        [DebuggerStepThrough]
        public LimitException() { }

        [DebuggerStepThrough]
        public LimitException(string message) : base(message) { }

        [DebuggerStepThrough]
        public LimitException(string message, Exception inner) : base(message, inner) { }
    }

    public class IterationsExceededException : MathException
    {
        [DebuggerStepThrough]
        public IterationsExceededException() { }

        [DebuggerStepThrough]
        public IterationsExceededException(string message) : base(message) { }

        [DebuggerStepThrough]
        public IterationsExceededException(string message, Exception inner) : base(message, inner) { }
    }

    public class FunctionEvaluationException : MathException
    {
        [DebuggerStepThrough]
        public FunctionEvaluationException() { }

        [DebuggerStepThrough]
        public FunctionEvaluationException(string message) : base(message) { }

        [DebuggerStepThrough]
        public FunctionEvaluationException(string message, Exception inner) : base(message, inner) { }
    }

    public class DimensionMismatchException : MathException
    {
        [DebuggerStepThrough]
        public DimensionMismatchException() { }

        [DebuggerStepThrough]
        public DimensionMismatchException(string message) : base(message) { }

        [DebuggerStepThrough]
        public DimensionMismatchException(string message, Exception inner) : base(message, inner) { }
    }

    public class DegenerateCaseException : MathException
    {
        [DebuggerStepThrough]
        public DegenerateCaseException() { }

        [DebuggerStepThrough]
        public DegenerateCaseException(string message) : base(message) { }

        [DebuggerStepThrough]
        public DegenerateCaseException(string message, Exception inner) : base(message, inner) { }
    }

    public class ConvergenceException : MathException
    {
        [DebuggerStepThrough]
        public ConvergenceException() { }

        [DebuggerStepThrough]
        public ConvergenceException(string message) : base(message) { }

        [DebuggerStepThrough]
        public ConvergenceException(string message, Exception inner) : base(message, inner) { }
    }
}
