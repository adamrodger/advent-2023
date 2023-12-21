using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 21
    /// </summary>
    public class Day21
    {
        public long Part1(string[] input)
        {
            var map = GardenMap.Parse(input);
            IDictionary<int, long> stepLookup = map.PossibleFinishingPoints(64);
            return stepLookup[64];
        }

        public long Part2(string[] input)
        {
            var map = GardenMap.Parse(input);

            int height = input.Length;
            (int quotient, int remainder) = Math.DivRem(26_501_365, height);

            IDictionary<int, long> stepLookup = map.PossibleFinishingPoints(remainder + 2 * height);

            // I had to look up a big spoiler for these maths - I don't really get quadratic equations tbh
            long d2 = stepLookup[remainder] + stepLookup[remainder + 2 * height] - 2 * stepLookup[remainder + height];
            long d1 = stepLookup[remainder + 2 * height] - stepLookup[remainder + height];

            return stepLookup[remainder + 2 * height] + (quotient - 2) * (2 * d1 + (quotient - 1) * d2) / 2;
        }

        /// <summary>
        /// Map of the garden
        /// </summary>
        /// <param name="Spaces">Points which can be occupied in the garden</param>
        /// <param name="Start">Start position</param>
        /// <param name="Height">Garden height and width (which assumes a square garden)</param>
        private record GardenMap(ISet<Point2D> Spaces, Point2D Start, int Height)
        {
            /// <summary>
            /// Parse the input
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Garden map</returns>
            public static GardenMap Parse(IReadOnlyList<string> input)
            {
                HashSet<Point2D> spaces = new();
                Point2D start = (0, 0);

                input.ForEach((point, c) =>
                {
                    if (c == '#')
                    {
                        return;
                    }

                    if (c == 'S')
                    {
                        start = point;
                    }

                    spaces.Add(point);
                });

                return new GardenMap(spaces, start, input.Count);
            }

            /// <summary>
            /// Get the number of possible finishing points for paths of each length up to and including the given maximum length
            /// </summary>
            /// <param name="maxSteps">Maximum path length</param>
            /// <returns>Lookup of path length to number of possible finishing points</returns>
            public IDictionary<int, long> PossibleFinishingPoints(int maxSteps)
            {
                var cache = new Dictionary<int, long> { [0] = 1 }; // number of steps to number of possible occupied points
                var visited = new HashSet<Point2D>();
                var frontier = new HashSet<Point2D> { this.Start };

                // move the frontier forward by one step until max steps, tracking visited points as we go
                foreach (int i in Enumerable.Range(1, maxSteps))
                {
                    visited.UnionWith(frontier);
                    
                    frontier = frontier.SelectMany(e => e.Adjacent4())
                                       .Where(e => !visited.Contains(e))
                                       .Where(e => this.Spaces.Contains((this.Wrap(e.X), this.Wrap(e.Y))))
                                       .ToHashSet();

                    long previous = i > 1 ? cache[i - 2] : 0;
                    cache[i] = previous + frontier.Count;
                }

                return cache;
            }

            /// <summary>
            /// Wrap the given index around the garden map
            /// </summary>
            /// <param name="i">Dimension index</param>
            /// <returns>Absolute map location on the same dimension</returns>
            private int Wrap(int i)
            {
                i %= this.Height;

                if (i < 0)
                {
                    i += this.Height;
                }

                return i;
            }
        }
    }
}
