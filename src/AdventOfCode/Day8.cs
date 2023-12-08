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

            string current = "AAA";
            int i = 0;
            int total = 0;

            while (current != "ZZZ")
            {
                char instruction = instructions[i];

                current = instruction == 'L' ? steps[current].Left : steps[current].Right;

                i++;
                i %= instructions.Length;

                total++;
            }

            return total;
        }

        public int Part2(string[] input)
        {
            foreach (string line in input)
            {
                throw new NotImplementedException("Part 2 not implemented");
            }

            return 0;
        }
    }
}
