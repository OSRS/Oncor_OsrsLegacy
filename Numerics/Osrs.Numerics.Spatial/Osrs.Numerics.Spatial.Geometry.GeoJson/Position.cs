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

using System.Collections.Generic;

namespace Osrs.Numerics.Spatial.Geometry
{
    public enum PositionType
    {
        Unknown,
        Point,
        MultiPoint,
        LineString,
        MultiLineString,
        Polygon,
        MultiPolygon,
        GeometryCollection,
    }

    public sealed class PositionSet : Position
    {
        public readonly List<Position> Positions = new List<Position>();

        public PositionSet()
          : base(double.NaN, double.NaN, PositionType.Unknown)
        {
        }

        public PositionSet(PositionType hint)
          : base(double.NaN, double.NaN, hint)
        {
        }
    }

    public class Position
    {
        public readonly double X;
        public readonly double Y;
        public readonly PositionType GeometryHint;

        public Position()
          : this(double.NaN, double.NaN, PositionType.Unknown)
        {
        }

        public Position(double x, double y)
          : this(x, y, PositionType.Unknown)
        {
        }

        public Position(double x, double y, PositionType hint)
        {
            this.X = x;
            this.Y = y;
            this.GeometryHint = hint;
        }
    }
}
