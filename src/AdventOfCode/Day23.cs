using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 23
    /// </summary>
    public class Day23
    {
        public int Part1(string[] input)
        {
            var graph = GardenGraph.Parse(input, Part.One);
            return graph.LongestPath();
        }

        public int Part2(string[] input)
        {
            var graph = GardenGraph.Parse(input, Part.Two);
            return graph.LongestPath();
        }

        /// <summary>
        /// Graph of the garden
        /// </summary>
        /// <param name="Vertices">Garden path vertices</param>
        /// <param name="Start">Start point</param>
        /// <param name="End">End point</param>
        private record GardenGraph(IDictionary<Point2D, List<(Point2D, int)>> Vertices, Point2D Start, Point2D End)
        {
            /// <summary>
            /// Parse the garden graph
            /// </summary>
            /// <param name="input">Input</param>
            /// <param name="part">Problem part</param>
            /// <returns>Garden graph</returns>
            public static GardenGraph Parse(IReadOnlyList<string> input, Part part)
            {
                Dictionary<Point2D, List<(Point2D, int)>> vertices = new();
                Point2D start = (-1, -1);
                Point2D end = (-1, -1);

                input.ForEach((point, c) =>
                {
                    if (c == '#')
                    {
                        return;
                    }

                    if (point.Y == 0 && c == '.')
                    {
                        start = point;
                    }

                    if (point.Y == input.Count - 1 && c == '.')
                    {
                        end = point;
                    }

                    var nextMoves = part == Part.One ? NextMovesPart1(input, point) : NextMovesPart2(input, point);

                    foreach (Point2D neighbour in nextMoves)
                    {
                        vertices.GetOrCreate(point, () => new List<(Point2D, int)>()).Add((neighbour, 1));
                    }
                });

                if (part == Part.Two)
                {
                    // find the corridors and combine into a single leap between branch points with appropriate cost
                    ISet<Point2D> corridors = vertices.Where(kvp => kvp.Value.Count == 2)
                                                      .Select(kvp => kvp.Key)
                                                      .ToHashSet();

                    foreach (Point2D point in corridors)
                    {
                        vertices.Remove(point, out List<(Point2D Point, int Cost)> neighbours);

                        List<(Point2D, int)> before = vertices[neighbours[0].Point];
                        List<(Point2D, int)> after = vertices[neighbours[1].Point];

                        before.RemoveAll(p => p.Item1 == point);
                        after.RemoveAll(p => p.Item1 == point);

                        before.Add((neighbours[1].Point, neighbours[0].Cost + neighbours[1].Cost));
                        after.Add((neighbours[0].Point, neighbours[0].Cost + neighbours[1].Cost));
                    }
                }

                return new GardenGraph(vertices, start, end);
            }

            /// <summary>
            /// Find the longest path through the garden
            /// </summary>
            /// <returns>Number of steps on the longest path</returns>
            public int LongestPath()
            {
                return this.LongestPath(this.Start, 0, new HashSet<Point2D>());
            }

            /// <summary>
            /// Find the longest path through the garden from the given point to the end
            /// </summary>
            /// <param name="current">Current location</param>
            /// <param name="steps">Current number of steps taken to reach this location</param>
            /// <param name="visited">Locations visited along the way</param>
            /// <returns>Number of steps on the longest path</returns>
            private int LongestPath(Point2D current, int steps, ISet<Point2D> visited)
            {
                if (current == this.End)
                {
                    return steps;
                }

                int longest = int.MinValue;

                foreach ((Point2D next, int cost) in this.Vertices[current])
                {
                    if (visited.Contains(next))
                    {
                        continue;
                    }

                    visited.Add(next);

                    int nextSteps = this.LongestPath(next, steps + cost, visited);
                    longest = Math.Max(longest, nextSteps);

                    visited.Remove(next);
                }

                return longest;
            }

            /// <summary>
            /// Get the next moves available from the given point in part 1
            /// </summary>
            /// <param name="grid">Input grid</param>
            /// <param name="current">Current position</param>
            /// <returns>Valid next moves</returns>
            private static IEnumerable<Point2D> NextMovesPart1(IReadOnlyList<string> grid, Point2D current)
            {
                Point2D above = current + (0, -1);
                Point2D below = current + (0, 1);
                Point2D left = current + (-1, 0);
                Point2D right = current + (1, 0);

                if (InBounds(above) && grid[above.Y][above.X] is '.' or '^')
                {
                    yield return above;
                }

                if (InBounds(below) && grid[below.Y][below.X] is '.' or 'v')
                {
                    yield return below;
                }

                if (InBounds(left) && grid[left.Y][left.X] is '.' or '<')
                {
                    yield return left;
                }

                if (InBounds(right) && grid[right.Y][right.X] is '.' or '>')
                {
                    yield return right;
                }

                bool InBounds(Point2D point) => point.X >= 0
                                             && point.X < grid[0].Length
                                             && point.Y >= 0
                                             && point.Y < grid.Count;
            }

            /// <summary>
            /// Get the next moves available from the given point in part 2
            /// </summary>
            /// <param name="grid">Input grid</param>
            /// <param name="current">Current position</param>
            /// <returns>Valid next moves</returns>
            private static IEnumerable<Point2D> NextMovesPart2(IReadOnlyList<string> grid, Point2D current)
            {
                foreach (Point2D point in current.Adjacent4())
                {
                    if (InBounds(point) && grid[point.Y][point.X] != '#')
                    {
                        yield return point;
                    }
                }

                bool InBounds(Point2D point) => point.X >= 0
                                             && point.X < grid[0].Length
                                             && point.Y >= 0
                                             && point.Y < grid.Count;
            }
        }
    }
}
