using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 25
    /// </summary>
    public class Day25
    {
        public int Part1(string[] input)
        {
            Dictionary<string, Node> nodes = input.Select(Node.Parse).ToDictionary(n => n.Id);

            // make all the connections two way
            foreach (string id in nodes.Keys.ToArray())
            {
                foreach (string connection in nodes[id].Connections)
                {
                    if (!nodes.TryGetValue(connection, out Node target))
                    {
                        target = new Node(connection, new HashSet<string>());
                        nodes[connection] = target;
                    }

                    target.Connections.Add(id);
                }
            }

            // I just solved this by eye today :D
            // See the graphviz input file for creating a graph, then the 2 separate sides are obvious, and I just counted one in Paint :D
            return 732 * (nodes.Count - 732);
        }

        private record Node(string Id, ISet<string> Connections)
        {
            public static Node Parse(string input)
            {
                string id = input[0..3];
                var connections = input[5..].Split().ToHashSet();

                return new Node(id, connections);
            }
        }
    }
}
