using System;
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

        public long Part2(string[] input) => input.Select(line => Spring.Parse(line, 5))
                                                  .Select(s => s.Combinations())
                                                  .Sum();

        /// <summary>
        /// Description of a spring with unknown segments, and the required sizes of the segments
        /// </summary>
        /// <param name="Description">Spring description</param>
        /// <param name="Groups">Segment groups</param>
        private record Spring(string Description, int[] Groups)
        {
            private readonly Dictionary<(int DescriptionRemaining, int GroupsRemaining), long> cache = new();

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
            public long Combinations() => this.Combinations(this.Description, this.Groups);

            /// <summary>
            /// Recursively try all the different combinations that could satisfy the spring description by walking
            /// through the description and branching at each wildcard
            /// </summary>
            /// <param name="spring">Current spring description slice</param>
            /// <param name="groups">Remaining groups</param>
            /// <returns>Total number of combinations from the given starting conditions</returns>
            private long Combinations(ReadOnlySpan<char> spring, ReadOnlySpan<int> groups)
            {
                if (groups.IsEmpty)
                {
                    return spring.Contains('#')
                               ? 0  // ran out of groups but some spring sections are still left over
                               : 1; // satisfied all groups with nothing left to check
                }

                if (spring.IsEmpty)
                {
                    return 0; // ran out of spring but there are groups still unsatisfied
                }

                (int, int) key = (spring.Length, groups.Length);

                if (!this.cache.TryGetValue(key, out long combinations))
                {
                    combinations = spring[0] switch
                    {
                        '.' => this.ConsumeAsRegularSegment(spring, groups),
                        '#' => this.ConsumeAsBrokenSegment(spring, groups),
                        '?' => this.ConsumeAsRegularSegment(spring, groups) + this.ConsumeAsBrokenSegment(spring, groups),
                        _ => throw new ArgumentOutOfRangeException($"Invalid segment char '{spring[0]}'")
                    };

                    this.cache[key] = combinations;
                }

                return combinations;
            }

            /// <summary>
            /// Consume the current leading char as if it's a regular segment (i.e. treat is as a . char)
            /// </summary>
            /// <param name="spring">Spring description</param>
            /// <param name="groups">Groups to match</param>
            /// <returns>Number of valid combinations from this state</returns>
            private long ConsumeAsRegularSegment(ReadOnlySpan<char> spring, ReadOnlySpan<int> groups)
            {
                return this.Combinations(spring[1..], groups);
            }

            /// <summary>
            /// Consume the current leading char as if it's a broken segment (i.e. treat is as a # char)
            /// </summary>
            /// <param name="spring">Spring description</param>
            /// <param name="groups">Groups to match</param>
            /// <returns>Number of valid combinations from this state</returns>
            private long ConsumeAsBrokenSegment(ReadOnlySpan<char> spring, ReadOnlySpan<int> groups)
            {
                int groupSize = groups[0];

                if (spring.Length < groupSize)
                {
                    return 0; // can't possibly fit
                }

                if (spring[..groupSize].Contains('.'))
                {
                    return 0; // hit a terminator before the group could be fully matched
                }

                if (spring.Length == groupSize)
                {
                    // rest of the string must all be # or ? chars
                    return groups.Length == 1
                               ? 1 // last group got satisfied
                               : 0; // got to the end before satisfying all groups
                }

                if (spring[groupSize] != '#')
                {
                    // found a group terminator so we can fast forward and check the next group
                    return this.Combinations(spring[(groupSize + 1)..], groups[1..]);
                }

                // the run of segments is too long to match the current group
                return 0;
            }
        }
    }
}
