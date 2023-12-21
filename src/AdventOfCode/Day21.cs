using System;
using System.Collections.Generic;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 21
    /// </summary>
    public class Day21
    {
        public int Part1(string[] input)
        {
            var map = GardenMap.Parse(input);
            return map.PossibleFinishingPoints();
        }

        public int Part2(string[] input)
        {
            foreach (string line in input)
            {
                throw new NotImplementedException("Part 2 not implemented");
            }

            return 0;
        }

        private record GardenMap(IDictionary<Point2D, IList<Point2D>> Vertices, Point2D Start)
        {
            public static GardenMap Parse(IReadOnlyList<string> input)
            {
                Dictionary<Point2D, IList<Point2D>> vertices = new();
                Point2D start = (0, 0);

                input.ForEach((current, c) =>
                {
                    if (c == '#')
                    {
                        return;
                    }

                    if (c == 'S')
                    {
                        start = current;
                    }

                    vertices[current] = new List<Point2D>();

                    foreach (Point2D neighbour in current.Adjacent4())
                    {
                        if (neighbour.X < 0 || neighbour.X >= input[0].Length || neighbour.Y < 0 || neighbour.Y >= input.Count)
                        {
                            // out of bounds
                            continue;
                        }

                        if (input[neighbour.Y][neighbour.X] != '#')
                        {
                            vertices[current].Add(neighbour);
                        }
                    }
                });

                return new GardenMap(vertices, start);
            }

            public int PossibleFinishingPoints()
            {
                var cache = new Dictionary<(Point2D Point, int Steps), int>();
                var endPoints = new HashSet<Point2D>();

                this.Visit(this.Start, 0, cache, endPoints);

                return endPoints.Count;
            }

            private int Visit(Point2D current, int steps, IDictionary<(Point2D Point, int Steps), int> cache, ISet<Point2D> endPoints)
            {
                var key = (current, steps);

                if (cache.TryGetValue(key, out int possiblePaths))
                {
                    return possiblePaths;
                }

                if (steps == 64)
                {
                    cache[(current, steps)] = 1;
                    endPoints.Add(current);
                    return 1;
                }

                foreach (Point2D neighbour in this.Vertices[current])
                {
                    possiblePaths += this.Visit(neighbour, steps + 1, cache, endPoints);
                }

                cache[key] = possiblePaths;
                return possiblePaths;
            }
        }
    }
}
