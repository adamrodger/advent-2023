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
            List<Point2D> galaxies = new();

            input.ForEach((point, c) =>
            {
                if (c == '#')
                {
                    galaxies.Add(point);
                }
            });

            int[] emptyColumns = Enumerable.Range(0, input[0].Length).Where(c => galaxies.All(g => g.X != c)).ToArray();
            int[] emptyRows = Enumerable.Range(0, input.Length).Where(c => galaxies.All(g => g.Y != c)).ToArray();

            long total = 0;

            for (int i = 0; i < galaxies.Count; i++)
            {
                Point2D galaxy = galaxies[i];

                foreach (Point2D other in galaxies.Skip(i + 1))
                {
                    Point2D topLeft = (Math.Min(galaxy.X, other.X), Math.Min(galaxy.Y, other.Y));
                    Point2D bottomRight = (Math.Max(galaxy.X, other.X), Math.Max(galaxy.Y, other.Y));

                    long distance = topLeft.ManhattanDistance(bottomRight);

                    // add the expanded columns/rows
                    long expandedColumns = Enumerable.Range(topLeft.X, bottomRight.X - topLeft.X).Count(emptyColumns.Contains) * 1_000_000;
                    long expandedRows = Enumerable.Range(topLeft.Y, bottomRight.Y - topLeft.Y).Count(emptyRows.Contains) * 1_000_000;

                    total += distance + expandedColumns + expandedRows;
                }
            }

            // 9674341 -- too high

            // part 2
            // 544723977692 -- too high

            return total;
        }

        public long Part2(string[] input)
        {
            List<Point2D> galaxies = new();

            input.ForEach((point, c) =>
            {
                if (c == '#')
                {
                    galaxies.Add(point);
                }
            });

            int[] emptyColumns = Enumerable.Range(0, input[0].Length).Where(c => galaxies.All(g => g.X != c)).ToArray();
            int[] emptyRows = Enumerable.Range(0, input.Length).Where(c => galaxies.All(g => g.Y != c)).ToArray();

            long total = 0;

            for (int i = 0; i < galaxies.Count; i++)
            {
                Point2D galaxy = galaxies[i];

                foreach (Point2D other in galaxies.Skip(i + 1))
                {
                    Point2D topLeft = (Math.Min(galaxy.X, other.X), Math.Min(galaxy.Y, other.Y));
                    Point2D bottomRight = (Math.Max(galaxy.X, other.X), Math.Max(galaxy.Y, other.Y));

                    long distance = topLeft.ManhattanDistance(bottomRight);

                    // add the expanded columns/rows
                    long expandedColumns = Enumerable.Range(topLeft.X, bottomRight.X - topLeft.X).Count(emptyColumns.Contains) * 1_000_000;
                    long expandedRows = Enumerable.Range(topLeft.Y, bottomRight.Y - topLeft.Y).Count(emptyRows.Contains) * 1_000_000;

                    total += distance + expandedColumns + expandedRows;
                }
            }

            // 544723977692 -- too high

            return total;
        }
    }
}
