using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 11
    /// </summary>
    public class Day11
    {
        public long Part1(string[] input)
        {
            var map = GalaxyMap.Parse(input, 2);

            return map.GalaxyPairs()
                      .Select(pair => (long)pair.Start.ManhattanDistance(pair.End))
                      .Sum();
        }

        public long Part2(string[] input, int scalingFactor = 1_000_000)
        {
            var map = GalaxyMap.Parse(input, scalingFactor);

            return map.GalaxyPairs()
                      .Select(pair => (long)pair.Start.ManhattanDistance(pair.End))
                      .Sum();
        }

        /// <summary>
        /// A map of a region of space containing multiple galaxies
        /// </summary>
        /// <param name="Galaxies">Galaxy locations in the observed map</param>
        private record GalaxyMap(IReadOnlyList<Point2D> Galaxies)
        {
            /// <summary>
            /// Parse the map
            /// </summary>
            /// <param name="input">Input</param>
            /// <param name="scalingFactor">Galaxy expansion factor - empty regions will grow by this factor</param>
            /// <returns>Map</returns>
            public static GalaxyMap Parse(IReadOnlyList<string> input, int scalingFactor)
            {
                int[] emptyColumns = Enumerable.Range(0, input[0].Length).Where(col => input.All(l => l[col] == '.')).ToArray();
                int[] emptyRows = Enumerable.Range(0, input.Count).Where(row => input[row].All(c => c == '.')).ToArray();

                List<Point2D> galaxies = new();

                input.ForEach((point, c) =>
                {
                    if (c != '#')
                    {
                        return;
                    }

                    int expandedColumns = emptyColumns.Count(x => x <= point.X) * (scalingFactor - 1);
                    int expandedRows = emptyRows.Count(y => y <= point.Y) * (scalingFactor - 1);

                    galaxies.Add(point + (expandedColumns, expandedRows));
                });

                return new GalaxyMap(galaxies);
            }

            /// <summary>
            /// Get the pairwise galaxies (i.e. order doesn't matter, no repeats)
            /// </summary>
            /// <returns>Galaxy pairs</returns>
            public IEnumerable<(Point2D Start, Point2D End)> GalaxyPairs()
            {
                foreach ((int i, Point2D start) in this.Galaxies.Enumerate())
                {
                    foreach (Point2D end in this.Galaxies.Skip(i + 1))
                    {
                        yield return (start, end);
                    }
                }
            }
        }
    }
}
