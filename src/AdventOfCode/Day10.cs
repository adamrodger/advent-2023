using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 10
    /// </summary>
    public class Day10
    {
        public int Part1(string[] input)
        {
            Dictionary<Point2D, Pipe> grid = new();
            Point2D start = (0, 0);

            for (int y = 0; y < input.Length; y++)
            {
                string line = input[y];

                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == 'S')
                    {
                        start = (x, y);
                        continue;
                    }

                    Pipe pipe = line[x].ToPipe();

                    if (pipe != Pipe.None)
                    {
                        grid[(x, y)] = pipe;
                    }
                }
            }

            Queue<(Point2D, int)> queue = new();

            // TODO: work out which way the start can go instead of hard coding
            queue.Enqueue((start + (0, -1), 1));
            queue.Enqueue((start + (0, 1), 1));

            Dictionary<Point2D, int> visited = new() { [start] = 0 };

            while (queue.Any())
            {
                (Point2D current, int depth) = queue.Dequeue();
                visited[current] = depth;

                (Point2D Left, Point2D Right) next = grid[current].Connections(current);

                if (!visited.ContainsKey(next.Left))
                {
                    queue.Enqueue((next.Left, depth + 1));
                }

                if (!visited.ContainsKey(next.Right))
                {
                    queue.Enqueue((next.Right, depth + 1));
                }
            }

            // 14179 -- too high - left hard-coded sample input start positions :D

            return visited.Values.Max();
        }

        public int Part2(string[] input)
        {
            Dictionary<Point2D, Pipe> grid = new();
            Point2D start = (0, 0);

            for (int y = 0; y < input.Length; y++)
            {
                string line = input[y];

                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == 'S')
                    {
                        start = (x, y);
                    }

                    Pipe pipe = line[x].ToPipe();

                    if (pipe != Pipe.None)
                    {
                        grid[(x, y)] = pipe;
                    }
                }
            }

            Queue<(Point2D, int)> queue = new();

            // TODO: work out which way the start can go instead of hard coding
            queue.Enqueue((start + (0, -1), 1));
            queue.Enqueue((start + (0, 1), 1));

            Dictionary<Point2D, int> visited = new() { [start] = 0 };

            while (queue.Count != 0)
            {
                (Point2D current, int depth) = queue.Dequeue();
                visited[current] = depth;

                (Point2D Left, Point2D Right) next = grid[current].Connections(current);

                if (!visited.ContainsKey(next.Left))
                {
                    queue.Enqueue((next.Left, depth + 1));
                }

                if (!visited.ContainsKey(next.Right))
                {
                    queue.Enqueue((next.Right, depth + 1));
                }
            }

            // TODO: work out which way the start can go instead of hard coding
            grid[start] = Pipe.NorthSouth;

            // start outside the loop
            bool within = false;
            int enclosed = 0;

            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[0].Length; x++)
                {
                    if (!visited.ContainsKey((x, y)))
                    {
                        if (within)
                        {
                            enclosed++;
                        }

                        continue;
                    }

                    Pipe pipe = grid[(x, y)]; // we visited this, so it's inside the loop

                    if (pipe is Pipe.SouthEast or Pipe.SouthWest or Pipe.NorthSouth)
                    {
                        within = !within;
                    }
                }
            }

            // 260 -- too low  -- was checking the grid contains key rather than visited :D

            return enclosed;
        }
    }

    public enum Pipe
    {
        None,
        NorthSouth,
        EastWest,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
        Start,
    }

    public static class PipeExtensions
    {
        public static Pipe ToPipe(this char c) => c switch
        {
            '.' => Pipe.None,
            '|' => Pipe.NorthSouth,
            '-' => Pipe.EastWest,
            'L' => Pipe.NorthEast,
            'J' => Pipe.NorthWest,
            'F' => Pipe.SouthEast,
            '7' => Pipe.SouthWest,
            'S' => Pipe.Start,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, "Unknown pipe value")
        };

        public static (Point2D Left, Point2D Right) Connections(this Pipe pipe, Point2D current) => pipe switch
        {
            Pipe.None => throw new InvalidOperationException(),
            Pipe.Start => throw new InvalidOperationException(),
            Pipe.NorthSouth => (current + (0, -1), current + (0, 1)),
            Pipe.EastWest => (current + (-1, 0), current + (1, 0)),
            Pipe.NorthEast => (current + (0, -1), current + (1, 0)),
            Pipe.NorthWest => (current + (0, -1), current + (-1, 0)),
            Pipe.SouthEast => (current + (0, 1), current + (1, 0)),
            Pipe.SouthWest => (current + (0, 1), current + (-1, 0)),
            _ => throw new ArgumentOutOfRangeException(nameof(pipe), pipe, null)
        };
    }
}
