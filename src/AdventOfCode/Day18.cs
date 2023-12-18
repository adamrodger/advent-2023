using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 18
    /// </summary>
    public class Day18
    {
        public int Part1(string[] input)
        {
            HashSet<Point2D> points = new();
            Point2D current = (0, 0);

            points.Add(current);

            foreach (string line in input)
            {
                Bearing direction = line[0] switch
                {
                    'U' => Bearing.North,
                    'D' => Bearing.South,
                    'R' => Bearing.East,
                    'L' => Bearing.West,
                    _ => throw new ArgumentOutOfRangeException()
                };

                int steps = line.Numbers<int>().First();

                foreach (Point2D next in Dig(current, direction, steps))
                {
                    points.Add(next);
                    current = next;
                }
            }

            int minX = points.MinBy(p => p.X).X;
            int maxX = points.MaxBy(p => p.X).X;
            int minY = points.MinBy(p => p.Y).Y;
            int maxY = points.MaxBy(p => p.Y).Y;

            StringBuilder s = new();

            for (int y = minY; y <= maxY; y++)
            {
                s.Append($"{y:D4}: ");

                for (int x = minX; x <= maxX; x++)
                {
                    s.Append(points.Contains((x, y)) ? '#' : '.');
                }

                s.AppendLine();
            }

            Debug.WriteLine(s.ToString());

            // from visual inspection, flood fill will work instead of the parity checking style from a previous day

            Point2D point = (Enumerable.Range(minX, maxX - minX).First(x => points.Contains((x, 0)) && !points.Contains((x + 1, 0))) + 1, 0);

            Queue<Point2D> queue = new();
            queue.Enqueue(point);

            while (queue.Count > 0)
            {
                point = queue.Dequeue();

                if (points.Contains(point))
                {
                    continue;
                }

                points.Add(point);

                foreach (Point2D next in point.Adjacent4().Where(n => !points.Contains(n)))
                {
                    queue.Enqueue(next);
                }
            }

            return points.Count;
        }

        public int Part2(string[] input)
        {
            foreach (string line in input)
            {
                throw new NotImplementedException("Part 2 not implemented");
            }

            return 0;
        }

        private static IEnumerable<Point2D> Dig(Point2D start, Bearing direction, int steps)
        {
            Point2D current = start;

            for (int i = 0; i < steps; i++)
            {
                current = current.Move(direction);
                yield return current;
            }
        }
    }
}
