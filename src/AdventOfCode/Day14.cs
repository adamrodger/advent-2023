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
            Map map = Map.Parse(input);
            map.Move(Bearing.North);
            return map.Load;
        }

        public int Part2(string[] input)
        {
            foreach (string line in input)
            {
                throw new NotImplementedException("Part 2 not implemented");
            }

            return 0;
        }

        private class Map
        {
            private readonly ISet<Point2D> walls;
            private readonly IList<Point2D> balls;
            private readonly int height;

            public int Load => this.balls.Select(b => this.height - b.Y).Sum();

            public Map(ISet<Point2D> walls, IList<Point2D> balls, int height)
            {
                this.walls = walls;
                this.balls = balls;
                this.height = height;
            }

            public static Map Parse(IReadOnlyList<string> input)
            {
                HashSet<Point2D> walls = new();
                List<Point2D> balls = new();

                input.ForEach((point, c) =>
                {
                    if (c == '#')
                    {
                        walls.Add(point);
                    }
                    else if (c == 'O')
                    {
                        balls.Add(point);
                    }
                });

                return new Map(walls, balls, input.Count);
            }

            public void Move(Bearing bearing)
            {
                Queue<Point2D> toMove = new(this.balls);
                HashSet<Point2D> settled = new();
                this.balls.Clear();

                while (toMove.Count != 0)
                {
                    Point2D current = toMove.Dequeue();
                    Point2D next = current.Move(bearing);

                    while (current.Y != 0 && !walls.Contains(next) && !settled.Contains(next))
                    {
                        current = next;
                        next = current.Move(bearing);
                    }

                    settled.Add(current);
                    this.balls.Add(current);
                }
            }
        }
    }
}
