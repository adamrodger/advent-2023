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
        public int Part1(string[] input) => BeamGrid.Parse(input).EnergisedTiles((0, 0), Bearing.East);

        public int Part2(string[] input) => BeamGrid.Parse(input).MaxEnergisedTiles();

        /// <summary>
        /// A grid of reflectors and splitters for light beams
        /// </summary>
        /// <param name="Grid">Grid</param>
        /// <param name="Width">Grid width</param>
        /// <param name="Height">Grid height</param>
        private record BeamGrid(char[,] Grid, int Width, int Height)
        {
            private readonly Queue<(Point2D Point, Bearing Bearing)> queue = new();
            private readonly HashSet<(Point2D Point, Bearing Bearing)> seen = new();

            /// <summary>
            /// Parse the beam grid
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Beam grid</returns>
            public static BeamGrid Parse(IReadOnlyList<string> input)
            {
                var grid = input.ToGrid();
                int width = input[0].Length;
                int height = input.Count;

                return new BeamGrid(grid, width, height);
            }

            /// <summary>
            /// Maximum energised tiles that can be achieved from all starting edge points
            /// </summary>
            /// <returns>Max energised tiles</returns>
            public int MaxEnergisedTiles() => this.WalkEdges().Max(edge => this.EnergisedTiles(edge.Point, edge.Bearing));

            /// <summary>
            /// Calculate how many tiles would be energised by the light beam starting at the given point on the given bearing
            /// </summary>
            /// <param name="startPoint">Start point</param>
            /// <param name="startBearing">Start bearing</param>
            /// <returns>Number of energised tiles</returns>
            public int EnergisedTiles(Point2D startPoint, Bearing startBearing)
            {
                this.queue.Enqueue((startPoint, startBearing));

                while (this.queue.Count > 0)
                {
                    (Point2D point, Bearing bearing) = this.queue.Dequeue();

                    this.seen.Add((point, bearing));

                    char current = this.Grid[point.Y, point.X];

                    foreach (Bearing move in NextMoves(current, bearing))
                    {
                        (Point2D Point, Bearing Bearing) next = (point.Move(move), move);

                        if (this.InBounds(next.Point) && !this.seen.Contains(next))
                        {
                            this.queue.Enqueue(next);
                        }
                    }
                }

                int energised = this.seen.Select(pair => pair.Point).Distinct().Count();

                // TODO: Retain the seen list between calls so we can memoize future calls without re-simulating
                this.seen.Clear();

                return energised;
            }

            /// <summary>
            /// Calculate where the light beam will move next after hitting the current tile on the given bearing
            ///
            /// The beam can split if it hits a splitter, hence why more than one result can be returned
            /// </summary>
            /// <param name="current">Current tile</param>
            /// <param name="bearing">Current bearing</param>
            /// <returns>Bearing(s) of the light beam continuation(s)</returns>
            private static IEnumerable<Bearing> NextMoves(char current, Bearing bearing)
            {
                switch (current)
                {
                    case '.':
                        yield return bearing;
                        break;
                    case '-':
                        switch (bearing)
                        {
                            case Bearing.East or Bearing.West:
                                yield return bearing;
                                break;
                            case Bearing.North or Bearing.South:
                                yield return Bearing.East;
                                yield return Bearing.West;
                                break;
                        }

                        break;
                    case '|':
                        switch (bearing)
                        {
                            case Bearing.East or Bearing.West:
                                yield return Bearing.North;
                                yield return Bearing.South;
                                break;
                            case Bearing.North or Bearing.South:
                                yield return bearing;
                                break;
                        }

                        break;
                    case '\\':
                        switch (bearing)
                        {
                            case Bearing.North:
                                yield return Bearing.West;
                                break;
                            case Bearing.South:
                                yield return Bearing.East;
                                break;
                            case Bearing.East:
                                yield return Bearing.South;
                                break;
                            case Bearing.West:
                                yield return Bearing.North;
                                break;
                        }

                        break;
                    case '/':
                        switch (bearing)
                        {
                            case Bearing.North:
                                yield return Bearing.East;
                                break;
                            case Bearing.South:
                                yield return Bearing.West;
                                break;
                            case Bearing.East:
                                yield return Bearing.North;
                                break;
                            case Bearing.West:
                                yield return Bearing.South;
                                break;
                        }

                        break;
                }
            }

            /// <summary>
            /// Check if the given point is in bounds
            /// </summary>
            /// <param name="point">Point</param>
            /// <returns>Point is in bounds</returns>
            private bool InBounds(Point2D point) => point.X >= 0 && point.X < this.Width
                                                 && point.Y >= 0 && point.Y < this.Height;

            /// <summary>
            /// Get all the edge locations along with their inward pointing bearing
            /// </summary>
            /// <returns>Edges</returns>
            private IEnumerable<(Point2D Point, Bearing Bearing)> WalkEdges()
            {
                foreach (int x in Enumerable.Range(0, this.Width))
                {
                    yield return ((x, 0), Bearing.South);
                    yield return ((x, this.Height - 1), Bearing.North);
                }

                foreach (int y in Enumerable.Range(0, this.Height))
                {
                    yield return ((0, y), Bearing.East);
                    yield return ((this.Width - 1, y), Bearing.West);
                }
            }
        }
    }
}
