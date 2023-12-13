using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Collections;
using Optional.Unsafe;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 13
    /// </summary>
    public class Day13
    {
        public int Part1(string input) => input.Trim()
                                               .Split("\n\n")
                                               .Select(Map.Parse)
                                               .Select(map => map.ReflectionScore().ValueOrFailure())
                                               .Sum();

        public int Part2(string input)
        {
            string[] maps = input.Trim().Split("\n\n");

            int total = 0;

            foreach (string m in maps)
            {
                int originalScore = Map.Parse(m).ReflectionScore().ValueOrFailure();

                int score = GenerateReplacements(m)
                            .Select(Map.Parse)
                            .Select(map => map.ReflectionScore(originalScore))
                            .First(score => score.HasValue)
                            .ValueOrFailure();

                total += score;
            }

            return total;
        }

        /// <summary>
        /// Generate every replacement image for the original image, with only one cell flipped in each
        /// </summary>
        /// <param name="original">Original image</param>
        /// <returns>Replacement images with one cell flipped in each</returns>
        private static IEnumerable<string> GenerateReplacements(string original)
        {
            for (int i = 0; i < original.Length; i++)
            {
                if (original[i] is not '#' or '.')
                {
                    continue;
                }

                string opposite = original[i] == '#' ? "." : "#";
                string replacement = original.Remove(i, 1).Insert(i, opposite);

                yield return replacement;
            }
        }

        /// <summary>
        /// Image map
        /// </summary>
        /// <param name="Points">Map points</param>
        private record Map(IReadOnlyList<string> Points)
        {
            /// <summary>
            /// Parse the map from an input line
            /// </summary>
            /// <param name="input">Input line</param>
            /// <returns>Map</returns>
            public static Map Parse(string input)
            {
                string[] split = input.Split('\n');

                return new Map(split);
            }

            /// <summary>
            /// Get the score of the reflection found in this image
            /// </summary>
            /// <param name="ignoreScore">(Optional) Ignore the reflection if it has this score</param>
            /// <returns>Reflection score</returns>
            public Option<int> ReflectionScore(int? ignoreScore = null)
            {
                IEnumerable<int> vertical = this.CandidateVerticalReflections()
                                                .Where(tuple => this.CheckVerticalReflection(tuple.Start, tuple.Size))
                                                .Select(tuple => tuple.Start + 1)
                                                .Where(score => !ignoreScore.HasValue || score != ignoreScore.Value);

                IEnumerable<int> horizontal = this.CandidateHorizontalReflections()
                                                  .Where(tuple => this.CheckHorizontalReflection(tuple.Start, tuple.Size))
                                                  .Select(tuple => (tuple.Start + 1) * 100)
                                                  .Where(score => !ignoreScore.HasValue || score != ignoreScore.Value);

                return vertical.Concat(horizontal).FirstOrNone();
            }

            /// <summary>
            /// Find all the columns that could be valid reflection points in the first row
            /// </summary>
            /// <returns>Potential reflection point</returns>
            private IEnumerable<(int Start, int Size)> CandidateVerticalReflections()
            {
                string row = this.Points[0];

                for (int x = 0; x < row.Length - 1; x++)
                {
                    if (TryReflectionPoint(x, out int steps))
                    {
                        // we got all the way to the edge with reflections
                        yield return (x, steps);
                    }
                }

                yield break;

                // check if this column could be a reflection point
                bool TryReflectionPoint(int x, out int steps)
                {
                    int lx = x;
                    int rx = x + 1;
                    steps = 0;

                    // walk outwards until we hit an edge or find a non-reflected value
                    while (lx >= 0 && rx < row.Length)
                    {
                        if (row[lx] != row[rx])
                        {
                            return false;
                        }

                        lx--;
                        rx++;
                        steps++;
                    }

                    return true;
                }
            }

            /// <summary>
            /// Check if the given reflection point is valid on all rows
            /// </summary>
            /// <param name="start">Reflection point start</param>
            /// <param name="size">Reflection point size</param>
            /// <returns>Valid reflection point on every row</returns>
            private bool CheckVerticalReflection(int start, int size)
            {
                foreach (string row in this.Points.Skip(1))
                {
                    var leftSlice = Enumerable.Range(start - size + 1, size);
                    var rightSlice = Enumerable.Range(start + 1, size).Reverse();

                    foreach ((int left, int right) in leftSlice.Zip(rightSlice))
                    {
                        if (row[left] != row[right])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            /// <summary>
            /// Find all the rows that could be valid reflection points in the first column
            /// </summary>
            /// <returns>Potential reflection point</returns>
            private IEnumerable<(int Start, int Size)> CandidateHorizontalReflections()
            {
                for (int y = 0; y < this.Points.Count - 1; y++)
                {
                    if (TryReflectionPoint(y, out int steps))
                    {
                        // we got all the way to the edge with reflections
                        yield return (y, steps);
                    }
                }

                yield break;

                // check if this row could be a reflection point
                bool TryReflectionPoint(int y, out int steps)
                {
                    int ly = y;
                    int ry = y + 1;
                    steps = 0;

                    // walk outwards until we hit an edge or find a non-reflected value
                    while (ly >= 0 && ry < this.Points.Count)
                    {
                        if (this.Points[ly][0] != this.Points[ry][0])
                        {
                            return false;
                        }

                        ly--;
                        ry++;
                        steps++;
                    }

                    return true;
                }
            }

            /// <summary>
            /// Check if the given reflection point is valid on all columns
            /// </summary>
            /// <param name="start">Reflection point start</param>
            /// <param name="size">Reflection point size</param>
            /// <returns>Valid reflection point on every column</returns>
            private bool CheckHorizontalReflection(int start, int size)
            {
                for (int x = 1; x < this.Points[0].Length; x++)
                {
                    var leftSlice = Enumerable.Range(start - size + 1, size);
                    var rightSlice = Enumerable.Range(start + 1, size).Reverse();

                    foreach ((int left, int right) in leftSlice.Zip(rightSlice))
                    {
                        if (this.Points[left][x] != this.Points[right][x])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
