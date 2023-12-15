using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 15
    /// </summary>
    public class Day15
    {
        public int Part1(string[] input) => input[0].Split(',')
                                                    .Select(Hash)
                                                    .Sum();

        public int Part2(string[] input)
        {
            List<Lens>[] lenses = Enumerable.Range(0, 256).Select(_ => new List<Lens>()).ToArray();

            foreach (Lens lens in input[0].Split(',').Select(Lens.Parse))
            {
                int hash = Hash(lens.Label);
                List<Lens> box = lenses[hash];

                int index = box.FindIndex(l => l.Label == lens.Label);

                switch ((lens.Instruction, index))
                {
                    case (Instruction.Add, < 0):
                        // add new lens
                        box.Add(lens);
                        break;
                    case (Instruction.Add, >= 0):
                        // replace existing lens
                        box[index] = lens;
                        break;
                    case (Instruction.Remove, >= 0):
                        // remove existing lens
                        box.RemoveAt(index);
                        break;
                }
            }

            return lenses.SelectMany((box, i) => box.Select((lens, j) => (i + 1) * (j + 1) * lens.FocalLength))
                         .Sum();
        }

        /// <summary>
        /// Hash the given value
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>Hash value</returns>
        private static int Hash(string value)
            => value.Aggregate(0, (acc, c) => (acc + c) * 17 % 256);

        /// <summary>
        /// A lens instruction
        /// </summary>
        /// <param name="Label">Lens label</param>
        /// <param name="FocalLength">Lens focal length</param>
        /// <param name="Instruction">Instruction</param>
        private record Lens(string Label, int FocalLength, Instruction Instruction)
        {
            /// <summary>
            /// Parse a lens
            /// </summary>
            /// <param name="line">Line</param>
            /// <returns>Parsed lens</returns>
            public static Lens Parse(string line)
            {
                int index = line.IndexOf('=');

                return index < 0
                           ? new Lens(line[..^1], -1, Instruction.Remove)
                           : new Lens(line[..index], line[^1] - '0', Instruction.Add);
            }
        }

        /// <summary>
        /// Lens instruction
        /// </summary>
        private enum Instruction
        {
            Add,
            Remove
        }
    }
}
