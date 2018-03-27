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

namespace Osrs.Numerics.Graphs
{
    public class SimpleGraph<T>
    {
        private Dictionary<T, SimpleGraph<T>.Node> nodes = new Dictionary<T, SimpleGraph<T>.Node>();

        public bool Contains(T item)
        {
            return this.nodes.ContainsKey(item);
        }

        public void AddNode(T item)
        {
            if (this.nodes.ContainsKey(item))
                return;
            this.nodes.Add(item, new SimpleGraph<T>.Node(item));
        }

        public void AddEdge(T a, T b)
        {
            SimpleGraph<T>.Node s = this.nodes[a];
            SimpleGraph<T>.Node d = this.nodes[b];
            foreach (SimpleGraph<T>.Edge edge in s.Edges)
            {
                if (edge.Start.Item.Equals((object)d.Item) || edge.End.Item.Equals((object)d.Item))
                    return;
            }
            SimpleGraph<T>.Edge edge1 = new SimpleGraph<T>.Edge(s, d);
        }

        public T[] GetAdjacent(T item)
        {
            if (!this.nodes.ContainsKey(item))
                return new T[0];
            List<SimpleGraph<T>.Edge> list = this.nodes[item].Edges;
            T[] objArray = new T[list.Count];
            for (int index = 0; index < objArray.Length; ++index)
            {
                SimpleGraph<T>.Edge edge = list[index];
                objArray[index] = !edge.Start.Equals((object)item) ? edge.Start.Item : edge.End.Item;
            }
            return objArray;
        }

        internal class Node
        {
            internal List<SimpleGraph<T>.Edge> Edges = new List<SimpleGraph<T>.Edge>();
            internal T Item;

            public Node(T item)
            {
                this.Item = item;
            }
        }

        internal class Edge
        {
            internal SimpleGraph<T>.Node Start;
            internal SimpleGraph<T>.Node End;

            public Edge(SimpleGraph<T>.Node s, SimpleGraph<T>.Node d)
            {
                this.Start = s;
                this.End = d;
                s.Edges.Add(this);
                d.Edges.Add(this);
            }
        }
    }
}
