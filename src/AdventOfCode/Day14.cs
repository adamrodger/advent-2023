using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 14
    /// </summary>
    public class Day14
    {
        public int Part1(string[] input)
        {
            HashSet<Point2D> walls = new();
            Queue<Point2D> balls = new();
            HashSet<Point2D> settled = new();

            input.ForEach((point, c) =>
            {
                if (c == '#')
                {
                    walls.Add(point);
                }
                else if (c == 'O')
                {
                    balls.Enqueue(point);
                }
            });

            while (balls.Count != 0)
            {
                Point2D current = balls.Dequeue();

                while (current.Y != 0 && !walls.Contains(current.Above) && !settled.Contains(current.Above))
                {
                    current = current.Above;
                }

                settled.Add(current);
            }

            return settled.Select(s => input.Length - s.Y).Sum();
        }

        public int Part2(string[] input)
        {
            foreach (string line in input)
            {
                throw new NotImplementedException("Part 2 not implemented");
            }

            return 0;
        }
    }
}
