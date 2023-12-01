using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 1
    /// </summary>
    public partial class Day1
    {
        public int Part1(string[] input)
        {
            int total = 0;

            foreach (string line in input)
            {
                total += (line.First(char.IsAsciiDigit) - '0') * 10;
                total += line.Last(char.IsAsciiDigit) - '0';
            }

            return total;
        }

        public int Part2(string[] input)
        {
            IDictionary<string, int> names = new Dictionary<string, int>
            {
                ["one"] = 1,
                ["two"] = 2,
                ["three"] = 3,
                ["four"] = 4,
                ["five"] = 5,
                ["six"] = 6,
                ["seven"] = 7,
                ["eight"] = 8,
                ["nine"] = 9
            };

            int total = 0;

            foreach (string line in input)
            {
                string first = Forward().Match(line).Value;
                string last = Backward().Match(line).Value;

                int lineAnswer = (names.TryGetValue(first, out int n1) ? n1 : first[0] - '0') * 10;
                lineAnswer += names.TryGetValue(last, out int n2) ? n2 : last[0] - '0';

                total += lineAnswer;
            }

            return total;
        }

        [GeneratedRegex(@"(\d|one|two|three|four|five|six|seven|eight|nine)")]
        private static partial Regex Forward();

        [GeneratedRegex(@"(\d|one|two|three|four|five|six|seven|eight|nine)", RegexOptions.RightToLeft)]
        private static partial Regex Backward();
    }
}
