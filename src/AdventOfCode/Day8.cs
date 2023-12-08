using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 8
    /// </summary>
    public class Day8
    {
        public int Part1(string[] input)
        {
            Map map = Map.Parse(input);
            return map.TotalSteps("AAA", "ZZZ");
        }

        public long Part2(string[] input)
        {
            Map map = Map.Parse(input);

            // find the loop length of each start node and then LCM all of them to find when they'd line up
            return map.Nodes
                      .Keys
                      .Where(n => n.EndsWith('A'))
                      .Select(n => (long)map.TotalSteps(n, x => x.EndsWith('Z')))
                      .Aggregate(Maths.LowestCommonMultiple);
        }

        /// <summary>
        /// The map to follow
        /// </summary>
        /// <param name="Directions">Map directions</param>
        /// <param name="Nodes">Map nodes</param>
        private record Map(IList<TurnDirection> Directions, IDictionary<string, Node> Nodes)
        {
            /// <summary>
            /// Parse the map
            /// </summary>
            /// <param name="input">Input lines</param>
            /// <returns>Map</returns>
            public static Map Parse(IList<string> input)
            {
                TurnDirection[] directions = input[0].Select(c => c == 'L' ? TurnDirection.Left : TurnDirection.Right).ToArray();

                Dictionary<string, Node> nodes = input.Skip(2).Select(Node.Parse).ToDictionary(n => n.Id);

                return new Map(directions, nodes);
            }

            /// <summary>
            /// Find the total number of steps between two nodes
            /// </summary>
            /// <param name="start">Start node</param>
            /// <param name="end">End node</param>
            /// <returns>Total number of steps</returns>
            public int TotalSteps(string start, string end) => this.TotalSteps(start, x => x == end);

            /// <summary>
            /// Find the total number of steps from the start node to a matching node
            /// </summary>
            /// <param name="start">Start node</param>
            /// <param name="target">Target match condition</param>
            /// <returns>Total number of steps</returns>
            public int TotalSteps(string start, Predicate<string> target)
            {
                if (target(start))
                {
                    return 0;
                }

                Node current = this.Nodes[start];
                int total = 0;

                foreach (TurnDirection direction in this.Directions.Cycle())
                {
                    total++;

                    string next = current.Next(direction);
                    current = this.Nodes[next];

                    if (target(current.Id))
                    {
                        return total;
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// A node on the map
        /// </summary>
        /// <param name="Id">Node ID</param>
        /// <param name="Left">Next node ID if the left path is taken</param>
        /// <param name="Right">Next node ID if the right path is taken</param>
        private record Node(string Id, string Left, string Right)
        {
            /// <summary>
            /// Parse a node from a string like "AAA = (BBB, CCC)"
            /// </summary>
            /// <param name="line">Input line</param>
            /// <returns>Node</returns>
            public static Node Parse(string line) => new(line[..3], line[7..10], line[12..15]);

            /// <summary>
            /// Get the next node from the given direction
            /// </summary>
            /// <param name="direction">Direction</param>
            /// <returns>Next node ID</returns>
            public string Next(TurnDirection direction) => direction == TurnDirection.Left ? this.Left : this.Right;
        }
    }
}
