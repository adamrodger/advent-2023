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
            Map map = Map.Parse(input);

            // just establish some kind of number to check the cycle logic :D
            map.Cycle();
            map.Cycle();
            map.Cycle();
            map.Cycle();
            map.Cycle();
            map.Cycle();
            map.Cycle();

            // 102921 -- too high

            return map.Load;
        }

        private class Map
        {
            private readonly ISet<Point2D> walls;
            private readonly IList<Point2D> balls;
            private readonly int height;
            private readonly int width;

            public int Load => this.balls.Select(b => this.height - b.Y).Sum();

            private Map(ISet<Point2D> walls, IList<Point2D> balls, int height, int width)
            {
                this.walls = walls;
                this.balls = balls;
                this.height = height;
                this.width = width;
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

                return new Map(walls, balls, input.Count, input[0].Length);
            }

            public void Cycle()
            {
                this.Move(Bearing.North);
                this.Move(Bearing.West);
                this.Move(Bearing.South);
                this.Move(Bearing.East);
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

                    while (this.InBounds(current, bearing) && !this.walls.Contains(next) && !settled.Contains(next))
                    {
                        current = next;
                        next = current.Move(bearing);
                    }

                    settled.Add(current);
                    this.balls.Add(current);
                }
            }

            private bool InBounds(Point2D point, Bearing bearing) => bearing switch
            {
                Bearing.North => point.Y > 0,
                Bearing.South => point.Y < this.height,
                Bearing.East => point.X < this.width,
                Bearing.West => point.X > 0
            };
        }
    }
}
