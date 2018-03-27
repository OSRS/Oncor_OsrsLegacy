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
    public interface IPointCollection : IGeometry
    {
    }

    public interface IPointCollection<T> : IGeometry<T>, IPointCollection
        where T : IEquatable<T>, IComparable<T>
    {
    }

    public interface IPointCollection2<T> : IGeometry2<T>, IPointCollection<T>
        where T : IEquatable<T>, IComparable<T>
    {
        Point2<T>[] ToArray();
    }

    public interface IPointBag : IPointCollection
    {
        //Checks for actual coincident points
        bool HasCoincidentPoints();
    }

    public interface IPointBag<T> : IPointCollection<T>, IPointBag
        where T : IEquatable<T>, IComparable<T>
    {
    }

    public interface IPointBag2<T> : IPointCollection2<T>, IPointBag<T>
        where T : IEquatable<T>, IComparable<T>
    {
        //builds a set of all coincident points (only one of each - gets locations)
        PointSet2<T> CoincidentPoints();
    }

    public interface IPointSet : IPointCollection
    {
    }

    public interface IPointSet<T> : IPointSet, IPointCollection<T>
        where T : IEquatable<T>, IComparable<T>
    {
    }

    public interface IPointSet2<T> : IPointCollection2<T>, IPointSet<T>
        where T : IEquatable<T>, IComparable<T>
    {
    }

    //TODO -- add IEnumerable<Point2<T>>
    //simple set of points, such as point sets/bags, ring, chain
    public interface ISimplePointCollection2<T> : ISimpleGeometry<Point2<T>, T>, IPointBag2<T>
        where T : IEquatable<T>, IComparable<T>
    {
    }

    //TODO -- add IEnumerable<IEnumerable<Point2<T>>>
    //multiple sets of points, such as chain sets/bags, ring sets/bags
    public interface IMultiPartPointCollection2<T> : IMultiPartGeometry<Point2<T>, T>, IPointBag2<T>
        where T : IEquatable<T>, IComparable<T>
    {
        Point2<T>[] ToArray(uint part);
        Point2<T>[][] ToArrays();
    }

    //collection of points with inner collections of points, such as a polygon w/holes
    public interface INestedPointCollection2<T> : INestedGeometry<Point2<T>, T>, IPointBag2<T>
        where T : IEquatable<T>, IComparable<T>
    {
        Point2<T>[] ToArray(uint section);
        Point2<T>[][] ToArrays();
    }

    //collection of points with inner collections of points, with inner collections of points, such as a polygon bags/sets w/holes
    public interface IMultiPartNestedPointCollection2<T> : IMultiPartNestedGeometry<Point2<T>, T>, IPointBag2<T>
        where T : IEquatable<T>, IComparable<T>
    {
        Point2<T>[][] ToArray(uint part);
        Point2<T>[] ToArray(uint part, uint section);
        Point2<T>[][][] ToArrays();
    }
}
