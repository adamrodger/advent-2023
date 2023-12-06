using System.Diagnostics;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 6
    /// </summary>
    public class Day6
    {
        public int Part1(string[] input)
        {
            int[] times = input[0].Numbers<int>();
            int[] distances = input[1].Numbers<int>();

            return times.Zip(distances)
                        .Select(r => new Race(r.First, r.Second))
                        .Aggregate(1, (acc, race) => acc * Enumerable.Range(1, race.TimeLimit - 1).Count(race.BeatsTarget));
        }

        public int Part2(string[] input)
        {
            int times = int.Parse(input[0][11..].Replace(" ", string.Empty));
            long distance = long.Parse(input[1][11..].Replace(" ", string.Empty));

            Race race = new Race(times, distance);

            int lower = Enumerable.Range(1, race.TimeLimit).First(race.BeatsTarget);

            // the curve is symmetrical so it's the same offset from the end as from the start
            int upper = race.TimeLimit - lower;

            return upper - lower + 1;
        }

        /// <summary>
        /// Race conditions
        /// </summary>
        /// <param name="TimeLimit">Race time limit</param>
        /// <param name="TargetDistance">Target distance</param>
        private record Race(int TimeLimit, long TargetDistance)
        {
            /// <summary>
            /// If we wait for the given initial period, would we beat the distance in the time limit?
            /// </summary>
            /// <param name="initialWait">Initial wait time</param>
            /// <returns>Whether this wait time beats the target distance</returns>
            public bool BeatsTarget(int initialWait)
            {
                Debug.Assert(initialWait > 0 && initialWait < this.TimeLimit);

                long distance = (long)initialWait * (this.TimeLimit - initialWait);

                return distance > this.TargetDistance; // assumption: equalling the target isn't good enough
            }
        }
    }
}
