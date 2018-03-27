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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial
{
    public class GeometryException : SpatialException
    {
        [DebuggerStepThrough]
        public GeometryException() { }

        [DebuggerStepThrough]
        public GeometryException(string message) : base(message) { }

        [DebuggerStepThrough]
        public GeometryException(string message, Exception inner) : base(message, inner) { }
    }

    public class OnEdgeException : GeometryException
    {
        [DebuggerStepThrough]
        public OnEdgeException() { }

        [DebuggerStepThrough]
        public OnEdgeException(string message) : base(message) { }

        [DebuggerStepThrough]
        public OnEdgeException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Indication of a topologic error in the geometry structure
    /// </summary>
    public class TopologyException : GeometryException
    {
        public TopologyException() { }

        public TopologyException(string message) : base(message) { }

        public TopologyException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Indicates a mismatch of some type between coordinates.  May be due to data type mismatches or value range mismatches
    /// </summary>
    public class CoordinateMismatchException : GeometryException
    {
        public CoordinateMismatchException() { }

        public CoordinateMismatchException(string message) : base(message) { }

        public CoordinateMismatchException(string message, Exception inner) : base(message, inner) { }
    }

    public class SpatialException : Exception
    {
        [DebuggerStepThrough]
        public SpatialException() { }

        [DebuggerStepThrough]
        public SpatialException(string message) : base(message) { }

        [DebuggerStepThrough]
        public SpatialException(string message, Exception inner) : base(message, inner) { }
    }
}
