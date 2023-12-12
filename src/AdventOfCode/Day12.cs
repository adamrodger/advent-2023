using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 12
    /// </summary>
    public class Day12
    {
        public long Part1(string[] input) => input.Select(line => Spring.Parse(line, 1))
                                                  .Select(s => s.Combinations())
                                                  .Sum();

        // 38181545406 -- too low
        public long Part2(string[] input) => input.Select(line => Spring.Parse(line, 5))
                                                  .Select(s => s.Combinations())
                                                  .Sum();

        /// <summary>
        /// Description of a spring with unknown segments, and the required sizes of the segments
        /// </summary>
        /// <param name="Description">Spring description</param>
        /// <param name="Groups">Segment groups</param>
        private record Spring(string Description, IReadOnlyList<int> Groups)
        {
            /// <summary>
            /// Parse the spring from an input line
            /// </summary>
            /// <param name="line">Input line</param>
            /// <param name="repeats">Number of times to repeat the input line (primarily for part 2)</param>
            /// <returns>Parsed spring</returns>
            public static Spring Parse(string line, int repeats)
            {
                string[] parts = line.Split();

                string description = string.Join("?", Enumerable.Repeat(parts[0], repeats));
                string groups = string.Join(",", Enumerable.Repeat(parts[1], repeats));

                return new Spring(description, groups.Numbers<int>());
            }

            /// <summary>
            /// Find all the possible combinations of this spring description which satisfy the required groups
            /// </summary>
            /// <returns>Total combinations</returns>
            public long Combinations() => this.Combinations(0, 0, 0, new Dictionary<(int, int, int), long>());

            /// <summary>
            /// Recursively try all the different combinations that could satisfy the spring description by walking
            /// through the description and branching at each wildcard
            /// </summary>
            /// <param name="springIndex">Current index into the spring description</param>
            /// <param name="groupIndex">Current group ID being checked</param>
            /// <param name="groupConsumed">Number of characters consumed in the group we're currently checking</param>
            /// <param name="cache">Memoization cache</param>
            /// <returns>Total number of combinations from the given starting conditions</returns>
            private long Combinations(int springIndex, int groupIndex, int groupConsumed, IDictionary<(int, int, int), long> cache)
            {
                if (cache.TryGetValue((springIndex, groupIndex, groupConsumed), out long combinations))
                {
                    return combinations;
                }

                // check if we fell off the end of the description
                if (springIndex == this.Description.Length)
                {
                    if (groupIndex == this.Groups.Count && groupConsumed == 0)
                    {
                        // finished satisfying all the groups
                        return 1;
                    }

                    if (groupIndex == this.Groups.Count - 1 && this.Groups[groupIndex] == groupConsumed)
                    {
                        // final group was satisfied by the final character
                        return 1;
                    }

                    // current group couldn't be satisfied or there were still some left that didn't get checked
                    return 0;
                }

                // advance through the description, checking combinations of remaining groups from here
                char c = this.Description[springIndex];

                if (c is '.' or '?')
                {
                    if (groupConsumed == 0)
                    {
                        // not currently checking a group, just advance in the description
                        combinations += this.Combinations(springIndex + 1, groupIndex, 0, cache);
                    }
                    else if (groupIndex < this.Groups.Count && this.Groups[groupIndex] == groupConsumed)
                    {
                        // finished a group, move to the next one
                        combinations += this.Combinations(springIndex + 1, groupIndex + 1, 0, cache);
                    }
                }

                if (c is '#' or '?')
                {
                    // consume this one in the current group
                    combinations += this.Combinations(springIndex + 1, groupIndex, groupConsumed + 1, cache);
                }

                cache[(springIndex, groupIndex, groupConsumed)] = combinations;

                return combinations;
            }
        }
    }
}
