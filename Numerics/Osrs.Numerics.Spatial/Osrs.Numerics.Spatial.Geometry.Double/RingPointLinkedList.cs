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

namespace Osrs.Numerics.Spatial.Geometry
{
    internal sealed class RingPointLinkedList<T>
        where T : IEquatable<T>, IComparable<T>
    {
        internal RingPointLinkedListNode RootNode;
        internal int Count;

        internal RingPointLinkedList(IEnumerable<Point2<T>> points)
        {
            if (points == null)
                return;

            this.Count = 0;
            this.RootNode = new RingPointLinkedListNode();
            RingPointLinkedListNode curNode = this.RootNode;

            foreach (Point2<T> curPoint in points)
            {
                curNode.Point = curPoint;
                curNode.Next = new RingPointLinkedListNode();
                curNode.Next.Previous = curNode;
                curNode = curNode.Next;
                this.Count++;
            }

            curNode = curNode.Previous;
            curNode.Next.Previous = null; //release empty node
            curNode.Next = this.RootNode;
            this.RootNode.Previous = curNode;
        }

        internal void Remove(RingPointLinkedListNode curNode)
        {
            if (object.ReferenceEquals(curNode, this.RootNode))
                this.RootNode = curNode.Previous; //re-establish a root node from those already visited

            curNode.Previous.Next = curNode.Next;
            curNode.Next.Previous = curNode.Previous;
            curNode.Previous = null;
            curNode.Next = null;
            curNode.Point = null;
            this.Count--;
        }

        internal sealed class RingPointLinkedListNode
        {
            internal Point2<T> Point;

            internal RingPointLinkedListNode Next;
            internal RingPointLinkedListNode Previous;
        }
    }
}
