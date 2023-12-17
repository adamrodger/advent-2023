using System;
using System.Collections.Generic;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 17
    /// </summary>
    public class Day17
    {
        public int Part1(string[] input)
        {
            LavaGrid grid = LavaGrid.Parse(input);
            return grid.ShortestPath(0, 3);
        }

        public int Part2(string[] input)
        {
            LavaGrid grid = LavaGrid.Parse(input);
            return grid.ShortestPath(4, 10);
        }

        /// <summary>
        /// Lava grid
        /// </summary>
        /// <param name="Grid">Grid of tile costs</param>
        /// <param name="Width">Grid width</param>
        /// <param name="Height">Grid height</param>
        private record LavaGrid(int[,] Grid, int Width, int Height)
        {
            /// <summary>
            /// Parse the grid
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Lava grid</returns>
            public static LavaGrid Parse(IReadOnlyList<string> input)
            {
                int[,] grid = input.ToGrid<int>();

                return new LavaGrid(grid, input[0].Length, input.Count);
            }

            /// <summary>
            /// Calculate the shortest valid path from the top left of the grid to the bottom right
            /// within the given constraints
            /// </summary>
            /// <param name="minimumMoves">Minimum moves before we're allowed to make a turn</param>
            /// <param name="maximumMoves">Maximum moves before we must make a turn</param>
            /// <returns>Shortest valid path</returns>
            /// <exception cref="InvalidOperationException">No path found</exception>
            public int ShortestPath(int minimumMoves, int maximumMoves)
            {
                StepState startSouth = new((0, 1), Bearing.South, 1);
                StepState startEast = new((1, 0), Bearing.East, 1);
                Point2D target = (this.Width - 1, this.Height - 1);

                Dictionary<StepState, int> distances = new()
                {
                    [startSouth] = this.Grid[1, 0],
                    [startEast] = this.Grid[0, 1],
                };

                PriorityQueue<StepState, int> queue = new();
                queue.Enqueue(startSouth, this.Grid[1, 0]);
                queue.Enqueue(startEast, this.Grid[0, 1]);

                HashSet<StepState> visited = new();

                while (queue.Count > 0)
                {
                    StepState current = queue.Dequeue();
                    visited.Add(current);

                    if (current.Point == target)
                    {
                        // guaranteed to be shortest because the queue is ordered by cost
                        return distances[current];
                    }

                    foreach (StepState next in NextMoves(current, minimumMoves, maximumMoves))
                    {
                        if (!this.InBounds(next.Point))
                        {
                            continue;
                        }

                        if (visited.Contains(next))
                        {
                            continue;
                        }

                        int nextDistance = distances[current] + this.Grid[next.Point.Y, next.Point.X];

                        // only move if it's cheaper than the current best path
                        if (!distances.TryGetValue(next, out int currentBest) || nextDistance < currentBest)
                        {
                            distances[next] = nextDistance;
                            queue.Enqueue(next, nextDistance);
                        }
                    }
                }

                throw new InvalidOperationException("No path found");
            }

            /// <summary>
            /// Enumerable the possible next moves from the current state
            /// </summary>
            /// <param name="state">Current state</param>
            /// <param name="minimumMoves">Minimum moves before we're allowed to make a turn</param>
            /// <param name="maximumMoves">Maximum moves before we must make a turn</param>
            /// <returns>Valid next moves</returns>
            private static IEnumerable<StepState> NextMoves(StepState state, int minimumMoves, int maximumMoves)
            {
                if (state.Consecutive < minimumMoves)
                {
                    yield return new StepState(state.Point.Move(state.Bearing), state.Bearing, state.Consecutive + 1);

                    // not met minimum yet, so can't make any further moves
                    yield break;
                }

                if (state.Consecutive < maximumMoves)
                {
                    yield return new StepState(state.Point.Move(state.Bearing), state.Bearing, state.Consecutive + 1);
                }

                switch (state.Bearing)
                {
                    case Bearing.South or Bearing.North:
                        yield return new StepState(state.Point.Move(Bearing.West), Bearing.West, 1);
                        yield return new StepState(state.Point.Move(Bearing.East), Bearing.East, 1);
                        break;

                    case Bearing.East or Bearing.West:
                        yield return new StepState(state.Point.Move(Bearing.North), Bearing.North, 1);
                        yield return new StepState(state.Point.Move(Bearing.South), Bearing.South, 1);
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
        }

        /// <summary>
        /// Step state
        /// </summary>
        /// <param name="Point">Current point</param>
        /// <param name="Bearing">Bearing taken to enter this point (to prevent backtracking, which is not allowed)</param>
        /// <param name="Consecutive">Number of consecutive steps taken on this bearing</param>
        private record StepState(Point2D Point, Bearing Bearing, int Consecutive);
    }
}
