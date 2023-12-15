using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 15
    /// </summary>
    public class Day15
    {
        public int Part1(string[] input)
        {
            int total = 0;

            foreach (string value in input[0].Split(','))
            {
                int hash = 0;

                foreach (char c in value)
                {
                    hash += c;
                    hash *= 17;
                    hash %= 256;
                }

                total += hash;
            }

            return total;
        }

        public int Part2(string[] input)
        {
            List<List<Lens>> lenses = Enumerable.Range(0, 256).Select(_ => new List<Lens>()).ToList();

            foreach (string value in input[0].Split(','))
            {
                Lens lens = Lens.Parse(value);
                int hash = lens.Hash;

                if (lens.Instruction == Instruction.Add)
                {
                    int index = lenses[hash].FindIndex(l => l.Label == lens.Label);

                    if (index < 0)
                    {
                        lenses[hash].Add(lens);
                    }
                    else
                    {
                        lenses[hash][index] = lens;
                    }
                }
                else
                {
                    int index = lenses[hash].FindIndex(l => l.Label == lens.Label);

                    if (index >= 0)
                    {
                        lenses[hash].RemoveAt(index);
                    }
                }
            }

            int power = 0;

            for (int i = 0; i < lenses.Count; i++)
            {
                var box = lenses[i];

                for (int j = 0; j < box.Count; j++)
                {
                    Lens lens = box[j];

                    power += (i + 1) * (j + 1) * lens.FocalLength;
                }
            }

            // 6973805 -- too high

            return power;
        }

        private record Lens(string Label, int FocalLength, Instruction Instruction, string Description)
        {
            public static Lens Parse(string line)
            {
                if (line.Contains('='))
                {
                    string[] parts = line.Split('=');
                    return new Lens(parts[0], int.Parse(parts[1]), Instruction.Add, line);
                }

                return new Lens(new string(line.TakeWhile(c => c is >= 'a' and <= 'z').ToArray()), -1, Instruction.Remove, line);
            }

            public int Hash => this.Label.Aggregate(0, (acc, c) => ((acc + c) * 17) % 256);
        }

        private enum Instruction
        {
            Add,
            Remove
        }
    }
}
