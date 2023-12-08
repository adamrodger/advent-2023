using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 8
    /// </summary>
    public class Day8
    {
        public int Part1(string[] input)
        {
            string instructions = input[0];

            Dictionary<string, (string Left, string Right)> steps = new(input.Length - 2);

            foreach (string line in input.Skip(2))
            {
                steps[line[..3]] = (line[7..10], line[12..15]);
            }

            return StepLength("AAA", x => x == "ZZZ", instructions, steps);
        }

        public long Part2(string[] input)
        {
            // this is going to be finding the loop length of each one and then LCM of those

            string instructions = input[0];

            Dictionary<string, (string Left, string Right)> steps = new(input.Length - 2);

            foreach (string line in input.Skip(2))
            {
                steps[line[..3]] = (line[7..10], line[12..15]);
            }

            string[] startNodes = steps.Keys.Where(s => s.EndsWith('A')).ToArray();
            long[] counts = new long[startNodes.Length];

            foreach ((string start, int i) in startNodes.Select((s, i) => (s, i)))
            {
                counts[i] = StepLength(start, x => x.EndsWith('Z'), instructions, steps);
            }

            return counts.Aggregate(findLCM);
        }

        private static int StepLength(string start,
                                      Predicate<string> target,
                                      string instructions,
                                      IDictionary<string, (string Left, string Right)> steps)
        {
            string current = start;
            int i = 0;
            int total = 0;

            while (!target(current))
            {
                char instruction = instructions[i];

                current = instruction == 'L' ? steps[current].Left : steps[current].Right;

                i++;
                i %= instructions.Length;

                total++;
            }

            return total;
        }

        public static long findLCM(long a, long b)
        {
            long num1, num2;

            if (a > b)
            {
                num1 = a;
                num2 = b;
            }
            else
            {
                num1 = b;
                num2 = a;
            }

            for (int i = 1; i <= num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num2;
        }
    }
}
