using System;
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
                        .Aggregate(1, (acc, race) => acc * race.PossibleSolutions());
        }

        public int Part2(string[] input)
        {
            int times = int.Parse(input[0][11..].Replace(" ", string.Empty));
            long distance = long.Parse(input[1][11..].Replace(" ", string.Empty));

            Race race = new Race(times, distance);
            return race.PossibleSolutions();
        }

        /// <summary>
        /// Race conditions
        /// </summary>
        /// <param name="TimeLimit">Race time limit</param>
        /// <param name="TargetDistance">Target distance</param>
        private record Race(int TimeLimit, long TargetDistance)
        {
            /// <summary>
            /// How many different wait periods could be used to beat the target distance within the time limit?
            /// </summary>
            /// <returns>Possible solutions</returns>
            public int PossibleSolutions()
            {
                double delta = Math.Sqrt(((long)this.TimeLimit * this.TimeLimit) - (4 * this.TargetDistance));

                double lower = (this.TimeLimit - delta) / 2;
                double upper = (this.TimeLimit + delta) / 2;

                return (int)(Math.Ceiling(upper) - Math.Floor(lower) - 1);
            }
        }
    }
}
