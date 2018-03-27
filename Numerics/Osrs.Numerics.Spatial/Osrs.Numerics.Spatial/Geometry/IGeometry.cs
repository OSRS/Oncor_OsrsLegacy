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
    public interface IGeometry : IComparable, IComparable<IGeometry>, IEquatable<IGeometry>
    {
        IGeometryFactory Factory { get; }

        uint VertexCount //total vertices, all rings, all parts, all sections, etc. -- for a point this == 1
        {
            get;
        }

        //should be false for point, true for everything else -- do not allow degenerate case -- make that null
        //may imply 2d envelope or 3d depending on geometry this is on
        bool HasEnvelope
        {
            get;
        }

        IEnvelope Envelope
        {
            get;
        }
    }

    public interface IGeometry<T> : IGeometry, IComparable<IGeometry<T>>, IEquatable<IGeometry<T>>
        where T : IComparable<T>, IEquatable<T>
    {
        new IGeometryFactory<T> Factory { get; }

        new IEnvelope<T> Envelope
        {
            get;
        }
    }

    public interface IGeometry2<T> : IGeometry<T>
        where T : IComparable<T>, IEquatable<T>
    {
        new GeometryFactory2Base<T> Factory
        {
            get;
        }

        new Envelope2<T> Envelope
        {
            get;
        }
    }

    public interface IGeometry3<T> : IGeometry<T>
        where T : IComparable<T>, IEquatable<T>
    {
        new Envelope3<T> Envelope
        {
            get;
        }
    }

    public interface IGeometry4<T> : IGeometry<T>
        where T : IComparable<T>, IEquatable<T>
    {
        new Envelope4<T> Envelope
        {
            get;
        }
    }

    public interface IGeometryN<T> : IGeometry<T>
        where T : IComparable<T>, IEquatable<T>
    {
        new EnvelopeN<T> Envelope
        {
            get;
        }
    }
}
