﻿//Copyright 2017 Open Science, Engineering, Research and Development Information Systems Open, LLC. (OSRS Open)
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
    public sealed class Geometry2Set<T> : IMultiPartPointCollection2<T>, IGeometry2<T>, IEquatable<Geometry2Set<T>>, IComparable<Geometry2Set<T>>
        where T : IEquatable<T>, IComparable<T>
    {

        public bool Equals(Geometry2Set<T> other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Geometry2Set<T> other)
        {
            throw new NotImplementedException();
        }

        public Envelope2<T> Envelope
        {
            get { throw new NotImplementedException(); }
        }

        public uint VertexCount
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasEnvelope
        {
            get { throw new NotImplementedException(); }
        }

        public Point2<T>[] ToArray(uint part)
        {
            throw new NotImplementedException();
        }

        public Point2<T>[][] ToArrays()
        {
            throw new NotImplementedException();
        }

        public uint PartCount
        {
            get { throw new NotImplementedException(); }
        }

        public GeometryFactory2Base<T> Factory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IGeometryFactory<T> IGeometry<T>.Factory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IEnvelope<T> IGeometry<T>.Envelope
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IGeometryFactory IGeometry.Factory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IEnvelope IGeometry.Envelope
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public uint PartVertexCount(uint part)
        {
            throw new NotImplementedException();
        }

        public Point2<T> this[uint part, uint index]
        {
            get { throw new NotImplementedException(); }
        }

        public PointSet2<T> CoincidentPoints()
        {
            throw new NotImplementedException();
        }

        public Point2<T>[] ToArray()
        {
            throw new NotImplementedException();
        }

        public bool HasCoincidentPoints()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IGeometry other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IGeometry other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IGeometry<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IGeometry<T> other)
        {
            throw new NotImplementedException();
        }
    }
}
