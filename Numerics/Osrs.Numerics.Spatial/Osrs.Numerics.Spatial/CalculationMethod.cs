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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Numerics.Spatial
{
    public enum LineIntersectionType
    {
        NoIntersection = 0,
        PointIntersection = 1,
        CollinearIntersection = 2
    }

    public enum PointAreaIntersectionType
    {
        /// <summary>
        /// Feature fully contains the node -- the node is inside the feature
        /// </summary>
        Contains,
        /// <summary>
        /// Feature intersects the node on an edge -- the node is on an edge of the feature
        /// </summary>
        OnEdge,
        /// <summary>
        /// Feature intersects the node at a node -- the node is coincident with a node of the feature
        /// </summary>
        Point,
        /// <summary>
        /// No intersection or overlap -- the node is outside of the feature
        /// </summary>
        None
    }

    /// <summary>
    /// Method for computing directions
    /// </summary>
    public enum DirectionMethod
    {
        /// <summary>
        /// Use loxodrome (rhumb line) - line of constant AngleUtils/heading.
        /// Loxodromes are not "shortest distance" (great-circle) paths, but are instead constant heading paths.
        /// All straight lines in a Mercator projection are constant bearing.
        /// </summary>
        Loxodrome,
        /// <summary>
        /// Use the initial bearing as the direction (non-loxodrome) based upon a great-circle path
        /// </summary>
        StartBearing,
        /// <summary>
        /// Use the mid-point bearing as the direction (non-loxodrome) based upon a great-circle path
        /// </summary>
        MidBearing
    }

    /// <summary>
    /// Preferred calculation method to use, to favor accuracy or speed
    /// </summary>
    public enum CalculationMethod
    {
        /// <summary>
        /// Favor speed over accuracy
        /// </summary>
        Fast,
        /// <summary>
        /// Give best balance of speed and accuracy
        /// </summary>
        Balanced,
        /// <summary>
        /// Favor accurate results over performance
        /// </summary>
        Accurate
    }
}
