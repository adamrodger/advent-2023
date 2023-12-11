using System;
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
            var map = GalaxyMap.Parse(input);

            return map.GalaxyPairs()
                      .Select(pair => map.MinimumDistance(pair.Start, pair.End, 2))
                      .Sum();
        }

        public long Part2(string[] input, int scalingFactor = 1_000_000)
        {
            var map = GalaxyMap.Parse(input);

            return map.GalaxyPairs()
                      .Select(pair => map.MinimumDistance(pair.Start, pair.End, scalingFactor))
                      .Sum();
        }

        /// <summary>
        /// A map of a region of space containing multiple galaxies
        /// </summary>
        /// <param name="Galaxies">Galaxy locations in the observed map</param>
        /// <param name="EmptyColumns">Index of every column which does not contain a galaxy</param>
        /// <param name="EmptyRows">Index of every row which does not contain a galaxy</param>
        private record GalaxyMap(ISet<Point2D> Galaxies, ISet<int> EmptyColumns, ISet<int> EmptyRows)
        {
            /// <summary>
            /// Parse the map
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Map</returns>
            public static GalaxyMap Parse(IReadOnlyList<string> input)
            {
                HashSet<Point2D> galaxies = new();

                input.ForEach((point, c) =>
                {
                    if (c == '#')
                    {
                        galaxies.Add(point);
                    }
                });

                var emptyColumns = Enumerable.Range(0, input[0].Length).Where(c => galaxies.All(g => g.X != c)).ToHashSet();
                var emptyRows = Enumerable.Range(0, input.Count).Where(c => galaxies.All(g => g.Y != c)).ToHashSet();

                return new GalaxyMap(galaxies, emptyColumns, emptyRows);
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

            /// <summary>
            /// Calculate the minimum distance from the start to end points taking into account space expansion in empty regions
            /// </summary>
            /// <param name="start">Start point</param>
            /// <param name="end">End point</param>
            /// <param name="scalingFactor">Scaling factor to apply to empty regions - e.g. 2 would indicate empty regions double in size</param>
            /// <returns>Minimum distance after expansion is taken into account</returns>
            public long MinimumDistance(Point2D start, Point2D end, int scalingFactor)
            {
                // calculate the top left and bottom right co-ordinates of the rectangle defined by the galaxies
                Point2D topLeft = (Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
                Point2D bottomRight = (Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));

                long distance = topLeft.ManhattanDistance(bottomRight);

                // add the expanded columns/rows
                long expandedColumns = Enumerable.Range(topLeft.X, bottomRight.X - topLeft.X).Count(this.EmptyColumns.Contains);
                long expandedRows = Enumerable.Range(topLeft.Y, bottomRight.Y - topLeft.Y).Count(this.EmptyRows.Contains);

                return distance
                     + (expandedColumns * (scalingFactor - 1))
                     + (expandedRows * (scalingFactor - 1));
            }
        }
    }
}
