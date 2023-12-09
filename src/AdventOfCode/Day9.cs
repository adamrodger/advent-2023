using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 9
    /// </summary>
    public class Day9
    {
        public long Part1(string[] input)
        {
            long total = 0;

            foreach (string line in input)
            {
                long[] numbers = line.Numbers<long>();
                total += numbers[^1] + CalculateNext(numbers);
            }

            return total;
        }

        public long Part2(string[] input)
        {
            long total = 0;

            foreach (string line in input)
            {
                long[] numbers = line.Numbers<long>();
                total += numbers[0] - CalculatePrevious(numbers);
            }

            return total;
        }

        private static long CalculateNext(IList<long> numbers)
        {
            var diffs = numbers.Zip(numbers.Skip(1)).Select(pair => pair.Second - pair.First).ToArray();

            if (diffs.All(d => d == 0))
            {
                return 0;
            }

            long next = CalculateNext(diffs);

            return diffs[^1] + next;
        }

        private static long CalculatePrevious(IList<long> numbers)
        {
            var diffs = numbers.Zip(numbers.Skip(1)).Select(pair => pair.Second - pair.First).ToArray();

            if (diffs.All(d => d == 0))
            {
                return 0;
            }

            long next = CalculatePrevious(diffs);

            return diffs[0] - next;
        }
    }
}
