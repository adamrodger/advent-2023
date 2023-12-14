using System;
using System.Collections.Generic;
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
            map.Tilt(Bearing.North);
            return map.Load;
        }

        public int Part2(string[] input)
        {
            Map map = Map.Parse(input);

            List<int> loads = new(256);
            Dictionary<string, int> seen = new(256);

            int loopStart;

            for (int i = 0; ; i++)
            {
                map.Cycle();

                string state = map.Print();

                if (seen.TryGetValue(state, out loopStart))
                {
                    break;
                }

                loads.Add(map.Load);
                seen[state] = i;
            }

            int loopLength = seen.Count - loopStart;
            int index = (1_000_000_000 - loopStart) % loopLength + loopStart - 1;

            return loads[index];
        }

        /// <summary>
        /// Map of rocks and walls
        /// </summary>
        private class Map
        {
            private readonly HashSet<Point2D> walls;
            private readonly List<Point2D> balls;
            private readonly int height;
            private readonly int width;
            private readonly StringBuilder printer;

            /// <summary>
            /// The load on the north wall of the map
            /// </summary>
            public int Load => this.balls.Select(b => this.height - b.Y).Sum();

            /// <summary>
            /// Initialises a new instance of the <see cref="Map"/> class.
            /// </summary>
            /// <param name="walls">Fixed wall positions</param>
            /// <param name="balls">Mobile ball positions</param>
            /// <param name="height">Map height</param>
            /// <param name="width">Map width</param>
            private Map(HashSet<Point2D> walls, List<Point2D> balls, int height, int width)
            {
                this.walls = walls;
                this.balls = balls;
                this.height = height;
                this.width = width;
                this.printer = new StringBuilder(this.width * this.height + this.height * Environment.NewLine.Length);
            }

            /// <summary>
            /// Parse the input
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Parsed map</returns>
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

            /// <summary>
            /// Perform one full tilting cycle on the map
            /// </summary>
            public void Cycle()
            {
                this.Tilt(Bearing.North);
                this.Tilt(Bearing.West);
                this.Tilt(Bearing.South);
                this.Tilt(Bearing.East);
            }

            /// <summary>
            /// Tile the map in the given direction to move all of the balls
            /// </summary>
            /// <param name="bearing">Tilt direction</param>
            public void Tilt(Bearing bearing)
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

                    settled.Add(current);
                }

                this.balls.Clear();
                this.balls.AddRange(settled);
            }

            /// <summary>
            /// Order the balls so they can move correctly depending on the tilt direction
            /// </summary>
            /// <param name="bearing">Tilt direction</param>
            /// <returns>Ordered balls to move</returns>
            private IEnumerable<Point2D> Order(Bearing bearing) => bearing switch
            {
                Bearing.North => this.balls.OrderBy(b => b.Y),
                Bearing.South => this.balls.OrderByDescending(b => b.Y),
                Bearing.East => this.balls.OrderByDescending(b => b.X),
                Bearing.West => this.balls.OrderBy(b => b.X),
                _ => throw new ArgumentOutOfRangeException(nameof(bearing), bearing, null)
            };

            /// <summary>
            /// Check if the given point is still in bounds according to the tilt direction
            /// </summary>
            /// <param name="point">Point</param>
            /// <param name="bearing">Tilt direction</param>
            /// <returns>Ball is still in bounds</returns>
            private bool InBounds(Point2D point, Bearing bearing) => bearing switch
            {
                Bearing.North => point.Y > 0,
                Bearing.South => point.Y < this.height - 1,
                Bearing.East => point.X < this.width - 1,
                Bearing.West => point.X > 0
            };

            /// <summary>
            /// Print the current state of the map
            /// </summary>
            /// <returns>Map state</returns>
            public string Print()
            {
                this.printer.Clear();
                HashSet<Point2D> settled = this.balls.ToHashSet();

                for (int y = 0; y < this.height; y++)
                {
                    for (int x = 0; x < this.width; x++)
                    {
                        if (this.walls.Contains((x, y)))
                        {
                            this.printer.Append('#');
                        }
                        else if (settled.Contains((x, y)))
                        {
                            this.printer.Append('O');
                        }
                        else
                        {
                            this.printer.Append('.');
                        }
                    }

                    this.printer.AppendLine();
                }

                return this.printer.ToString();
            }
        }
    }
}
