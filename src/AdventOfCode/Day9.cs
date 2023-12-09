using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 9
    /// </summary>
    public class Day9
    {
        public int Part1(string[] input) => input.Select(SensorReadings.Parse)
                                                 .Select(s => s.NextTerm())
                                                 .Sum();

        public int Part2(string[] input) => input.Select(SensorReadings.Parse)
                                                 .Select(s => s.PreviousTerm())
                                                 .Sum();

        /// <summary>
        /// Readings from the OASIS sensor
        /// </summary>
        /// <param name="Values">Reading values</param>
        private record SensorReadings(IList<int> Values)
        {
            /// <summary>
            /// Parse the readings from the given line
            /// </summary>
            /// <param name="line"></param>
            /// <returns>Parsed sensor readings</returns>
            public static SensorReadings Parse(string line) => new(line.Split().Select(int.Parse).ToArray());

            /// <summary>
            /// Calculate the next term in the sensor reading sequence
            /// </summary>
            /// <returns>Next term</returns>
            public int NextTerm() => this.Values[^1] + NextDelta(this.Values);

            /// <summary>
            /// Calculate the previous term in the sensor reading sequence
            /// </summary>
            /// <returns>Previous term</returns>
            public int PreviousTerm() => this.Values[0] - PreviousDelta(this.Values);

            /// <summary>
            /// Get the delta to the next term
            /// </summary>
            /// <param name="numbers">Starting numbers</param>
            /// <returns>Next delta</returns>
            private static int NextDelta(IList<int> numbers)
            {
                var diffs = numbers.Zip(numbers.Skip(1))
                                   .Select(pair => pair.Second - pair.First)
                                   .ToArray();

                if (diffs.All(d => d == 0))
                {
                    return 0;
                }

                int next = NextDelta(diffs);

                return diffs[^1] + next;
            }

            /// <summary>
            /// Get the delta to the previous term
            /// </summary>
            /// <param name="numbers">Starting numbers</param>
            /// <returns>Previous delta</returns>
            private static int PreviousDelta(IList<int> numbers)
            {
                var diffs = numbers.Zip(numbers.Skip(1))
                                   .Select(pair => pair.Second - pair.First)
                                   .ToArray();

                if (diffs.All(d => d == 0))
                {
                    return 0;
                }

                int previous = PreviousDelta(diffs);

                return diffs[0] - previous;
            }
        }
    }
}
