using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 16
    /// </summary>
    public class Day16
    {
        public int Part1(string[] input)
        {
            char[,] grid = input.ToGrid();

            return Simulate(grid, (0,0), Bearing.East);
        }

        public int Part2(string[] input)
        {
            char[,] grid = input.ToGrid();

            int max = int.MinValue;
            
            foreach (int x in Enumerable.Range(0, input[0].Length))
            {
                int result = Simulate(grid, (x, 0), Bearing.South);
                max = Math.Max(max, result);

                result = Simulate(grid, (x, input.Length - 1), Bearing.North);
                max = Math.Max(max, result);
            }

            foreach (int y in Enumerable.Range(0, input.Length))
            {
                int result = Simulate(grid, (0, y), Bearing.East);
                max = Math.Max(max, result);

                result = Simulate(grid, (input[0].Length - 1, y), Bearing.West);
                max = Math.Max(max, result);
            }

            return max;
        }

        private static int Simulate(char[,] grid, Point2D start, Bearing startBearing)
        {
            Queue<(Point2D Point, Bearing Bearing)> queue = new();
            queue.Enqueue((start, startBearing));

            HashSet<(Point2D Point, Bearing Bearing)> seen = new();

            while (queue.Count > 0)
            {
                (Point2D point, Bearing bearing) = queue.Dequeue();

                seen.Add((point, bearing));

                char current = grid[point.Y, point.X];

                foreach ((Point2D Point, Bearing Bearing) next in Next(current, point, bearing))
                {
                    if (next.Point.X < 0 || next.Point.X >= grid.GetLength(1) || next.Point.Y < 0 || next.Point.Y >= grid.GetLength(0))
                    {
                        // fell off the edge
                        continue;
                    }

                    if (!seen.Contains(next))
                    {
                        queue.Enqueue(next);
                    }
                }
            }

            return seen.Select(pair => pair.Point).Distinct().Count();
        }

        private static IEnumerable<(Point2D Point, Bearing Bearing)> Next(char current, Point2D point, Bearing bearing)
        {
            switch (current)
            {
                case '.':
                    yield return (point.Move(bearing), bearing);
                    break;
                case '-':
                    switch (bearing)
                    {
                        case Bearing.East or Bearing.West:
                            yield return (point.Move(bearing), bearing);
                            break;
                        case Bearing.North or Bearing.South:
                            yield return (point.Move(Bearing.East), Bearing.East);
                            yield return (point.Move(Bearing.West), Bearing.West);
                            break;
                    }

                    break;
                case '|':
                    switch (bearing)
                    {
                        case Bearing.East or Bearing.West:
                            yield return (point.Move(Bearing.North), Bearing.North);
                            yield return (point.Move(Bearing.South), Bearing.South);
                            break;
                        case Bearing.North or Bearing.South:
                            yield return (point.Move(bearing), bearing);
                            break;
                    }

                    break;
                case '\\':
                    switch (bearing)
                    {
                        case Bearing.North:
                            yield return (point.Move(Bearing.West), Bearing.West);
                            break;
                        case Bearing.South:
                            yield return (point.Move(Bearing.East), Bearing.East);
                            break;
                        case Bearing.East:
                            yield return (point.Move(Bearing.South), Bearing.South);
                            break;
                        case Bearing.West:
                            yield return (point.Move(Bearing.North), Bearing.North);
                            break;
                    }

                    break;
                case '/':
                    switch (bearing)
                    {
                        case Bearing.North:
                            yield return (point.Move(Bearing.East), Bearing.East);
                            break;
                        case Bearing.South:
                            yield return (point.Move(Bearing.West), Bearing.West);
                            break;
                        case Bearing.East:
                            yield return (point.Move(Bearing.North), Bearing.North);
                            break;
                        case Bearing.West:
                            yield return (point.Move(Bearing.South), Bearing.South);
                            break;
                    }

                    break;
            }
        }
    }
}
