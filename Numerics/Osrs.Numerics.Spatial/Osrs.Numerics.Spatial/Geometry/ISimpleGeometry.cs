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

namespace Osrs.Numerics.Spatial.Geometry
{
    public interface ISimpleGeometry<T,V> : IGeometry<V>
        where T : IGeometry<V>
        where V : IComparable<V>, IEquatable<V>
    {
        T this[uint index]
        {
            get;
        }
    }

    public interface IMultiPartGeometry<T, V> : IGeometry<V>
        where T : IGeometry<V>
        where V : IComparable<V>, IEquatable<V>
    {
        uint PartCount
        {
            get;
        }

        uint PartVertexCount(uint part);

        T this[uint part, uint index]
        {
            get;
        }
    }

    public interface INestedGeometry<T, V> : IGeometry<V>
        where T : IGeometry<V>
        where V : IComparable<V>, IEquatable<V>
    {
        uint SectionCount
        {
            get;
        }

        uint SectionVertexCount(uint section);

        T this[uint section, uint index]
        {
            get;
        }
    }

    public interface IMultiPartNestedGeometry<T, V> : IGeometry<V>
        where T : IGeometry<V>
        where V : IComparable<V>, IEquatable<V>
    {
        uint PartCount
        {
            get;
        }

        uint PartVertexCount(uint part);

        uint SectionCount();
        uint SectionCount(uint part);

        uint SectionVertexCount(uint part, uint section);

        T this[uint part, uint section, uint index]
        {
            get;
        }
    }
}
