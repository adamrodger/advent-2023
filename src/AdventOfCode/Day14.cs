using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

            List<int> loads = new();

            for (int i = 0; i < 1000; i++)
            {
                map.Cycle();
                int load = map.Load;

                loads.Add(load);

                //Debug.WriteLine(map.Print());
            }


            // 102921 -- too high
            // 102188 -- too high

            Dictionary<int, int> analysis = loads.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            // 101400 -- too high
            // 101288 -- incorrect (5min lockout, no too high/low info)

            var keys = analysis.Where(kvp => kvp.Value > 1).OrderBy(kvp => kvp.Key);

            int cycleLength = keys.Count();

            int index = ((1_000_000_000 - (analysis.Count - cycleLength)) % cycleLength) + analysis.Count - 1;

            return loads[index];
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
                //Debug.WriteLine(this.Print());

                this.Move(Bearing.West);
                //Debug.WriteLine(this.Print());

                this.Move(Bearing.South);
                //Debug.WriteLine(this.Print());

                this.Move(Bearing.East);
                //Debug.WriteLine(this.Print());
            }

            public void Move(Bearing bearing)
            {
                IEnumerable<Point2D> ordered = this.Order(bearing);
                HashSet<Point2D> settled = new();

                foreach (Point2D point in ordered)
                {
                    Point2D current = point;
                    Point2D next = current.Move(bearing);

                    while (this.InBounds(current, bearing) && !this.walls.Contains(next) && !settled.Contains(next))
                    {
                        current = next;
                        next = current.Move(bearing);
                    }

                    bool added = settled.Add(current);
                    Debug.Assert(added);
                }

                Debug.Assert(this.balls.Count == settled.Count, "We've lost some balls...");

                this.balls.Clear();

                foreach (Point2D ball in settled)
                {
                    this.balls.Add(ball);
                }
            }

            private IEnumerable<Point2D> Order(Bearing bearing) => bearing switch
            {
                Bearing.North => this.balls.OrderBy(b => b.Y),
                Bearing.South => this.balls.OrderByDescending(b => b.Y),
                Bearing.East => this.balls.OrderByDescending(b => b.X),
                Bearing.West => this.balls.OrderBy(b => b.X),
                _ => throw new ArgumentOutOfRangeException(nameof(bearing), bearing, null)
            };

            private bool InBounds(Point2D point, Bearing bearing) => bearing switch
            {
                Bearing.North => point.Y > 0,
                Bearing.South => point.Y < this.height - 1,
                Bearing.East => point.X < this.width - 1,
                Bearing.West => point.X > 0
            };

            public string Print()
            {
                StringBuilder s = new(this.width * this.height + this.height * Environment.NewLine.Length);
                HashSet<Point2D> settled = this.balls.ToHashSet();

                for (int y = 0; y < this.height; y++)
                {
                    for (int x = 0; x < this.width; x++)
                    {
                        if (this.walls.Contains((x, y)))
                        {
                            s.Append('#');
                        }
                        else if (settled.Contains((x, y)))
                        {
                            s.Append('O');
                        }
                        else
                        {
                            s.Append('.');
                        }
                    }

                    s.AppendLine();
                }

                return s.ToString();
            }
        }
    }
}
