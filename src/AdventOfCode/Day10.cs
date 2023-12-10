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
            PipeGrid grid = PipeGrid.Parse(input);
            IDictionary<Point2D, int> loop = grid.FollowLoop();
            return loop.Values.Max();
        }

        public int Part2(string[] input)
        {
            PipeGrid grid = PipeGrid.Parse(input);
            IDictionary<Point2D, int> loop = grid.FollowLoop();

            // See: https://en.wikipedia.org/wiki/Point_in_polygon

            // start outside the loop
            bool inside = false;
            int enclosed = 0;

            input.ForEach((point, _) =>
            {
                if (loop.ContainsKey(point))
                {
                    Tile tile = grid.Tiles[point];

                    if (tile is Tile.SouthEast or Tile.SouthWest or Tile.NorthSouth)
                    {
                        // each time we cross a matching vertical boundary that's in the loop, we flip inside or outside the loop
                        inside = !inside;
                    }
                }
                else if (inside)
                {
                    // this tile isn't part of the loop itself, and we're currently looking inside the loop. This tile is enclosed
                    enclosed++;
                }
            });
            
            return enclosed;
        }

        /// <summary>
        /// A grid of pipes
        /// </summary>
        /// <param name="Tiles">Tiles on the grid</param>
        /// <param name="Start">Start location</param>
        private record PipeGrid(IDictionary<Point2D, Tile> Tiles, Point2D Start)
        {
            /// <summary>
            /// Parse the grid
            /// </summary>
            /// <param name="input">Input map</param>
            /// <returns>Parsed grid</returns>
            public static PipeGrid Parse(IReadOnlyList<string> input)
            {
                Dictionary<Point2D, Tile> tiles = new();
                Point2D start = (0, 0);

                input.ForEach((point, c) =>
                {
                    if (c == 'S')
                    {
                        start = point;
                        return;
                    }

                    tiles[point] = c switch
                    {
                        '.' => Tile.Ground,
                        '|' => Tile.NorthSouth,
                        '-' => Tile.EastWest,
                        'L' => Tile.NorthEast,
                        'J' => Tile.NorthWest,
                        'F' => Tile.SouthEast,
                        '7' => Tile.SouthWest,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                });

                tiles[start] = CalculateStartType(tiles, start);

                return new PipeGrid(tiles, start);
            }

            /// <summary>
            /// Work out which type of pipe the start point must be
            /// </summary>
            /// <param name="grid">Grid</param>
            /// <param name="start">Start point</param>
            /// <returns>Start pipe type</returns>
            private static Tile CalculateStartType(IDictionary<Point2D, Tile> grid, Point2D start)
            {
                Tile above = grid[start.Move(Bearing.North)];
                Tile below = grid[start.Move(Bearing.South)];
                Tile left = grid[start.Move(Bearing.West)];

                bool aboveConnected = above is Tile.NorthSouth or Tile.SouthEast or Tile.SouthWest;
                bool belowConnected = below is Tile.NorthSouth or Tile.NorthEast or Tile.NorthWest;
                bool leftConnected = left is Tile.EastWest or Tile.NorthEast or Tile.SouthEast;

                if (aboveConnected)
                {
                    if (belowConnected)
                    {
                        return Tile.NorthSouth;
                    }

                    return leftConnected
                               ? Tile.NorthWest
                               : Tile.NorthEast;
                }

                if (belowConnected)
                {
                    return leftConnected
                               ? Tile.SouthWest
                               : Tile.SouthEast;
                }

                return Tile.EastWest;
            }

            /// <summary>
            /// Find the loop from the starting location
            /// </summary>
            /// <returns>Loop points with depth from start</returns>
            public IDictionary<Point2D, int> FollowLoop()
            {
                (Point2D Left, Point2D Right) connections = this.Connections(this.Start);

                Queue<(Point2D, int)> queue = new();
                queue.Enqueue((connections.Left, 1));
                queue.Enqueue((connections.Right, 1));

                Dictionary<Point2D, int> visited = new()
                {
                    [this.Start] = 0
                };

                while (queue.Count != 0)
                {
                    (Point2D current, int depth) = queue.Dequeue();
                    visited[current] = depth;

                    connections = this.Connections(current);

                    if (!visited.ContainsKey(connections.Left))
                    {
                        queue.Enqueue((connections.Left, depth + 1));
                    }
                    else if (!visited.ContainsKey(connections.Right))
                    {
                        queue.Enqueue((connections.Right, depth + 1));
                    }
                }

                return visited;
            }

            /// <summary>
            /// Get the connections from the given location
            /// </summary>
            /// <param name="current">Current location</param>
            /// <returns>Two connection points</returns>
            private (Point2D Left, Point2D Right) Connections(Point2D current) => this.Tiles[current] switch
            {
                Tile.Ground => throw new InvalidOperationException(),
                Tile.NorthSouth => (current + (0, -1), current + (0, 1)),
                Tile.EastWest => (current + (-1, 0), current + (1, 0)),
                Tile.NorthEast => (current + (0, -1), current + (1, 0)),
                Tile.NorthWest => (current + (0, -1), current + (-1, 0)),
                Tile.SouthEast => (current + (0, 1), current + (1, 0)),
                Tile.SouthWest => (current + (0, 1), current + (-1, 0)),
                _ => throw new ArgumentOutOfRangeException(nameof(current), current, null)
            };
        }

        /// <summary>
        /// Pipe map tile type
        /// </summary>
        private enum Tile
        {
            Ground,
            NorthSouth,
            EastWest,
            NorthEast,
            NorthWest,
            SouthEast,
            SouthWest
        }
    }
}
