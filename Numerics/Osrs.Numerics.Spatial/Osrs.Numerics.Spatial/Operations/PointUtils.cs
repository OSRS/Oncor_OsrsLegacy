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
using System.Linq;

namespace Osrs.Numerics.Spatial.Operations
{
    public static class PointUtils
    {
        public static Envelope2<T> EnvelopeFor<T>(IEnumerable<Point2<T>> points)
            where T : IEquatable<T>, IComparable<T>
        {
            if (points == null)
                return null;
            IEnumerator<Point2<T>> en = points.GetEnumerator();
            if (en.MoveNext())
            {
                Point2<T> cur = en.Current;
                T minX = cur.X;
                T maxX = cur.X;
                T minY = cur.Y;
                T maxY = cur.Y;
                while (en.MoveNext())
                {
                    cur = en.Current;
                    if (minX.CompareTo(cur.X) > 0)
                        minX = cur.X;
                    else if (maxX.CompareTo(cur.X) < 0)
                        maxX = cur.X;
                    if (minY.CompareTo(cur.Y) > 0)
                        minY = cur.Y;
                    else if (maxY.CompareTo(cur.Y) < 0)
                        maxY = cur.Y;
                }
                return new Envelope2<T>(minX, minY, maxX, maxY);
            }
            return null;
        }

        public static Envelope2<T> EnvelopeFor<T>(Point2<T> a, Point2<T> b)
            where T : IEquatable<T>, IComparable<T>
        {
            if (a == null || b == null)
                return null;
            T minX = a.X;
            T maxX = a.X;
            T minY = a.Y;
            T maxY = a.Y;

            if (minX.CompareTo(b.X) > 0)
                minX = b.X;
            else if (maxX.CompareTo(b.X) < 0)
                maxX = b.X;
            if (minY.CompareTo(b.Y) > 0)
                minY = b.Y;
            else if (maxY.CompareTo(b.Y) < 0)
                maxY = b.Y;
            return new Envelope2<T>(minX, minY, maxX, maxY);
        }

        public static Envelope2<T> EnvelopeFor<T>(Point2<T> a, Point2<T> b, Point2<T> c)
            where T : IEquatable<T>, IComparable<T>
        {
            if (a == null || b == null || c == null)
                return null;
            T minX = a.X;
            T maxX = a.X;
            T minY = a.Y;
            T maxY = a.Y;

            if (minX.CompareTo(b.X) > 0)
                minX = b.X;
            else if (maxX.CompareTo(b.X) < 0)
                maxX = b.X;
            if (minY.CompareTo(b.Y) > 0)
                minY = b.Y;
            else if (maxY.CompareTo(b.Y) < 0)
                maxY = b.Y;

            if (minX.CompareTo(c.X) > 0)
                minX = c.X;
            else if (maxX.CompareTo(c.X) < 0)
                maxX = c.X;
            if (minY.CompareTo(c.Y) > 0)
                minY = c.Y;
            else if (maxY.CompareTo(c.Y) < 0)
                maxY = c.Y;
            return new Envelope2<T>(minX, minY, maxX, maxY);
        }

        public static Envelope2<T> EnvelopeFor<T>(Envelope2<T> env, Point2<T> p)
        where T : IEquatable<T>, IComparable<T>
        {
            if (env == null || p == null)
                return null;
            T minX = env.MinX;
            T maxX = env.MaxX;
            T minY = env.MinY;
            T maxY = env.MaxY;

            if (minX.CompareTo(p.X) > 0)
                minX = p.X;
            else if (maxX.CompareTo(p.X) < 0)
                maxX = p.X;
            if (minY.CompareTo(p.Y) > 0)
                minY = p.Y;
            else if (maxY.CompareTo(p.Y) < 0)
                maxY = p.Y;
            return new Envelope2<T>(minX, minY, maxX, maxY);
        }

        //removes all duplicated points
        public static Point2<T>[] RemoveDups<T>(IEnumerable<Point2<T>> points)
            where T : IEquatable<T>, IComparable<T>
        {
            if (points == null)
                return null;
            HashSet<Point2<T>> tmp = new HashSet<Point2<T>>();
            foreach (Point2<T> p in points)
            {
                if (p != null)
                    tmp.Add(p);
            }
            return tmp.ToArray();
        }

        public static Point2<T>[] RemoveAdjacentDups<T>(IEnumerable<Point2<T>> chain)
            where T : IEquatable<T>, IComparable<T>
        {
            return RemoveAdjacentDups<T>(chain, false);
        }

        //removes duplicated points in a chain of points - only adjacent duplicates are removed
        public static Point2<T>[] RemoveAdjacentDups<T>(IEnumerable<Point2<T>> chain, bool isRing)
            where T : IEquatable<T>, IComparable<T>
        {
            if (chain == null)
                return null;
            Point2<T> prev = null;
            List<Point2<T>> tmp = new List<Point2<T>>();
            foreach (Point2<T> curPt in chain)
            {
                if (curPt == null)
                    continue;
                if (curPt.Equals(prev))
                    continue; //dup
                prev = curPt;
                tmp.Add(curPt);
            }
            if (isRing && tmp.Count > 1)
            {
                if (tmp[0].Equals(tmp[tmp.Count - 1])) //start and end are dup on a ring
                    tmp.RemoveAt(tmp.Count - 1); //get rid of the last item
            }
            return tmp.ToArray();
        }
    }
}
