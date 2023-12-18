using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 18
    /// </summary>
    public class Day18
    {
        // this originally generated the perimeter points then flood-filled the centre
        public long Part1(string[] input) => DigPlan.ForPart1(input).CalculateArea();

        // Had to look up hints for part 2. I didn't know the maths for finding the area of an irregular polygon
        public long Part2(string[] input) => DigPlan.ForPart2(input).CalculateArea();

        /// <summary>
        /// Plan for digging the lava lake
        /// </summary>
        /// <param name="Instructions">Plan instructions for digging the perimeter</param>
        private record DigPlan(IList<DigInstruction> Instructions)
        {
            /// <summary>
            /// Instructions for digging the lava lake in part 1
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Dig plan</returns>
            public static DigPlan ForPart1(IReadOnlyList<string> input)
            {
                var instructions = from line in input
                                   let direction = line[0] switch
                                   {
                                       'U' => Bearing.North,
                                       'D' => Bearing.South,
                                       'R' => Bearing.East,
                                       'L' => Bearing.West,
                                       _ => throw new ArgumentOutOfRangeException()
                                   }
                                   let steps = line.Numbers<int>().First()
                                   select new DigInstruction(direction, steps);

                return new DigPlan(instructions.ToArray());
            }

            /// <summary>
            /// Instructions for digging the lava lake in part 2
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Dig plan</returns>
            public static DigPlan ForPart2(IReadOnlyList<string> input)
            {
                var instructions = from line in input
                                   let direction = line[^2] switch
                                   {
                                       '3' => Bearing.North,
                                       '1' => Bearing.South,
                                       '0' => Bearing.East,
                                       '2' => Bearing.West,
                                       _ => throw new ArgumentOutOfRangeException()
                                   }
                                   let steps = int.Parse(line[^7..^2], NumberStyles.HexNumber)
                                   select new DigInstruction(direction, steps);

                return new DigPlan(instructions.ToArray());
            }

            /// <summary>
            /// Calculate the entire area of the lava lake
            /// </summary>
            /// <returns>Lava lake area</returns>
            public long CalculateArea()
            {
                (IList<(long X, long Y)> vertices, long perimeter) = this.PlanPerimeter();

                long area = CalculateInternalArea(vertices);

                // See https://en.wikipedia.org/wiki/Pick%27s_theorem
                return area + perimeter / 2 + 1;
            }

            /// <summary>
            /// Use the instructions to map out the perimeter
            /// </summary>
            /// <returns>All the vertices and the total perimeter length</returns>
            private (IList<(long X, long Y)> Vertices, long Perimeter) PlanPerimeter()
            {
                long x = 0;
                long y = 0;
                long perimeter = 0;
                List<(long X, long Y)> vertices = new() { (0, 0) };

                foreach ((Bearing direction, var steps) in this.Instructions)
                {
                    long deltaX = 0;
                    long deltaY = 0;

                    switch (direction)
                    {
                        case Bearing.North:
                            deltaY -= steps;
                            break;
                        case Bearing.South:
                            deltaY += steps;
                            break;
                        case Bearing.East:
                            deltaX += steps;
                            break;
                        case Bearing.West:
                            deltaX -= steps;
                            break;
                    }

                    x += deltaX;
                    y += deltaY;
                    perimeter += steps;
                    vertices.Add((x, y));
                }

                return (vertices, perimeter);
            }

            /// <summary>
            /// Calculate the internal area of the lava lake defined by the given vertices
            /// </summary>
            /// <param name="vertices">Vertices</param>
            /// <returns>Internal area</returns>
            /// <remarks>
            /// See:
            ///     https://en.m.wikipedia.org/wiki/Shoelace_formula#Triangle_formula
            ///     https://www.theoremoftheday.org/GeometryAndTrigonometry/Shoelace/TotDShoelace.pdf
            /// </remarks>
            private static long CalculateInternalArea(IList<(long X, long Y)> vertices)
            {
                long area = 0;

                // create a closed loop and iterate the vertices anti-clockwise
                IEnumerable<((long X, long Y) First, (long X, long Y) Second)> loop = vertices.Zip(vertices.Skip(1).Append(vertices[0]));

                foreach (((long X, long Y) first, (long X, long Y) second) in loop)
                {
                    area += (first.X * second.Y) - (second.X * first.Y);
                }

                return area / 2;
            }
        }

        /// <summary>
        /// Instruction for digging an edge of the lava lake
        /// </summary>
        /// <param name="Direction">Dig direction</param>
        /// <param name="Steps">Number of steps to dig</param>
        private record DigInstruction(Bearing Direction, int Steps);
    }
}
