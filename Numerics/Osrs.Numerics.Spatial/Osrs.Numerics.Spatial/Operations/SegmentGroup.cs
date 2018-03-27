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

namespace Osrs.Numerics.Spatial.Operations
{
    internal sealed class SegmentGroup<T> : IEnumerable<Edge<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        public enum EdgeAction
        {
            Added,
            Removed
        }

        public readonly HashSet<Edge<T>> Edges = new HashSet<Edge<T>>();

        public EdgeAction Action(Edge<T> edge)
        {
            if (this.Edges.Contains(edge))
            {
                this.Edges.Remove(edge);
                return EdgeAction.Removed;
            }
            this.Edges.Add(edge);
            return EdgeAction.Added;
        }

        public void Add(Edge<T> edge)
        {
            this.Edges.Add(edge);
        }

        public void Remove(Edge<T> edge)
        {
            try
            {
                this.Edges.Remove(edge);
            }
            catch { }
        }

        public IEnumerator<Edge<T>> GetEnumerator()
        {
            return this.Edges.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Edges.GetEnumerator();
        }
    }
}
