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

            // 870 - too low - someone else's answer :D so we're close
            // 877 -- too low (just a guess from other possible answers from above
            return grid.ShortestPath((0, 0), (grid.Width - 1, grid.Height - 1), LavaGrid.NextMovesPart1);
        }

        public int Part2(string[] input)
        {
            LavaGrid grid = LavaGrid.Parse(input);
            return grid.ShortestPath((0, 0), (grid.Width - 1, grid.Height - 1), LavaGrid.NextMovesPart2);
        }

        private record LavaGrid(int[,] Grid, int Width, int Height)
        {
            public static LavaGrid Parse(IReadOnlyList<string> input)
            {
                int[,] grid = input.ToGrid<int>();

                return new LavaGrid(grid, input[0].Length, input.Count);
            }

            public int ShortestPath(Point2D start, Point2D target, Func<StepState, IEnumerable<StepState>> nextMoves)
            {
                HashSet<StepState> visited = new();
                Dictionary<StepState, int> distance = new()
                {
                    [new StepState(start, Bearing.South, 0)] = 0,
                    [new StepState(start, Bearing.East, 0)] = 0,
                };

                PriorityQueue<StepState, int> queue = new();
                queue.Enqueue(new StepState(start, Bearing.South, 0), this.Grid[1, 0]);
                queue.Enqueue(new StepState(start, Bearing.East, 0), this.Grid[0, 1]);

                while (queue.Count > 0)
                {
                    StepState current = queue.Dequeue();
                    visited.Add(current);

                    if (current.Point == target)
                    {
                        // guaranteed to be shortest because the queue is ordered by cost
                        return distance[current];
                    }

                    foreach (StepState next in nextMoves(current))
                    {
                        if (!this.InBounds(next.Point))
                        {
                            continue;
                        }

                        if (visited.Contains(next))
                        {
                            continue;
                        }

                        int newDistance = distance[current] + (this.Grid[next.Point.Y, next.Point.X]);

                        // only move if it's cheaper than the current best path
                        if (!distance.TryGetValue(next, out int value) || newDistance < value)
                        {
                            distance[next] = newDistance;
                            queue.Enqueue(next, newDistance);
                        }
                    }
                }

                throw new InvalidOperationException($"No path found from {start} to {target}");
            }

            public static IEnumerable<StepState> NextMovesPart1(StepState state)
            {
                if (state.Consecutive < 2)
                {
                    yield return new StepState(state.Point.Move(state.Bearing), state.Bearing, state.Consecutive + 1);
                }

                switch (state.Bearing)
                {
                    case Bearing.South or Bearing.North:
                        yield return new StepState(state.Point.Move(Bearing.West), Bearing.West, 0);
                        yield return new StepState(state.Point.Move(Bearing.East), Bearing.East, 0);
                        break;

                    case Bearing.East or Bearing.West:
                        yield return new StepState(state.Point.Move(Bearing.North), Bearing.North, 0);
                        yield return new StepState(state.Point.Move(Bearing.South), Bearing.South, 0);
                        break;
                }
            }

            public static IEnumerable<StepState> NextMovesPart2(StepState state)
            {
                if (state.Consecutive < 3)
                {
                    yield return new StepState(state.Point.Move(state.Bearing), state.Bearing, state.Consecutive + 1);
                    yield break;
                }

                if (state.Consecutive < 9)
                {
                    yield return new StepState(state.Point.Move(state.Bearing), state.Bearing, state.Consecutive + 1);
                }

                switch (state.Bearing)
                {
                    case Bearing.South or Bearing.North:
                        yield return new StepState(state.Point.Move(Bearing.West), Bearing.West, 0);
                        yield return new StepState(state.Point.Move(Bearing.East), Bearing.East, 0);
                        break;

                    case Bearing.East or Bearing.West:
                        yield return new StepState(state.Point.Move(Bearing.North), Bearing.North, 0);
                        yield return new StepState(state.Point.Move(Bearing.South), Bearing.South, 0);
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

        private record StepState(Point2D Point, Bearing Bearing, int Consecutive);
    }
}
