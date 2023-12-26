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

            // Manually found the edges to remove by converting the input to dot format and plotting in Graphviz with:
            //     dot -Ksfdp -Tsvg day25_graphviz.dot -o day25.svg
            //
            // The edges were then removed from day26_graphviz_pruned.dot and the SVG regenerated
            // to confirm it creates two distinct regions
            nodes["mfc"].Connections.Remove("vph");
            nodes["vph"].Connections.Remove("mfc");
            nodes["vmt"].Connections.Remove("sfm");
            nodes["sfm"].Connections.Remove("vmt");
            nodes["rmg"].Connections.Remove("fql");
            nodes["fql"].Connections.Remove("rmg");

            // now we can just count the nodes in one cluster to get the answer
            Queue<string> queue = new();
            queue.Enqueue(nodes.Keys.First());

            HashSet<string> visited = new();

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                visited.Add(current);

                foreach (string next in nodes[current].Connections.Where(c => !visited.Contains(c)))
                {
                    queue.Enqueue(next);
                }
            }
            
            return visited.Count * (nodes.Count - visited.Count);
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
