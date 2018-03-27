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

using Osrs.Numerics.Spatial.Geometry;
using System;
using System.Collections.Generic;

namespace Osrs.Numerics.Spatial.Operations
{
    internal sealed class PointGraph<T>
        where T : IEquatable<T>, IComparable<T>
    {
        public readonly List<PointNode> Points = new List<PointNode>();
        public readonly List<PointEdge> Edges = new List<PointEdge>();

        public sealed class PointNode
        {
            public readonly Point2<T> Point;
            public readonly List<PointEdge> Edges = new List<PointEdge>();

            public PointNode(Point2<T> point)
            {
                this.Point = point;
            }
        }

        public sealed class PointEdge
        {
            public readonly int Index;
            public readonly PointNode Start;
            public readonly PointNode End;

            public PointEdge(PointNode start, PointNode end, int index)
            {
                this.Start = start;
                this.End = end;
                this.Index = index;
            }
        }
    }
}
