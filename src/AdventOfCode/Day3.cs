using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 3
    /// </summary>
    public class Day3
    {
        public int Part1(string[] input)
        {
            Schematic schematic = Schematic.Parse(input);

            return schematic.Parts
                            .Where(p => schematic.Symbols.Any(s => s.IsAdjacent(p)))
                            .Select(p => p.Value)
                            .Sum();
        }

        public int Part2(string[] input)
        {
            Schematic schematic = Schematic.Parse(input);

            return schematic.Symbols
                            .Where(s => s.Value == '*')
                            .Select(s => schematic.Parts.Where(s.IsAdjacent).ToArray())
                            .Where(adjacent => adjacent.Length == 2)
                            .Select(a => a[0].Value * a[1].Value)
                            .Sum();
        }

        /// <summary>
        /// The engine schematic, with the parts and symbols that comprise it
        /// </summary>
        /// <param name="Parts">Parts</param>
        /// <param name="Symbols">Symbols</param>
        private readonly record struct Schematic(ICollection<Part> Parts, ICollection<Symbol> Symbols)
        {
            /// <summary>
            /// Parse the schematic from the input
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Parsed engine schematic</returns>
            public static Schematic Parse(IEnumerable<string> input)
            {
                var symbols = new List<Symbol>();
                var parts = new List<Part>();

                foreach ((int y, string line) in input.Enumerate())
                {
                    for (int x = 0; x < line.Length; x++)
                    {
                        char c = line[x];

                        if (c == '.')
                        {
                            continue;
                        }

                        if (!char.IsAsciiDigit(c))
                        {
                            symbols.Add(new Symbol(c, x, y));
                            continue;
                        }

                        // TODO: Improve perf using Spans instead of allocating an array
                        ReadOnlySpan<char> number = line.Skip(x).TakeWhile(char.IsAsciiDigit).ToArray();
                        parts.Add(new Part(int.Parse(number), x, x + number.Length - 1, y));

                        // skip over the bits we parsed
                        x += number.Length - 1;
                    }
                }

                return new Schematic(parts, symbols);
            }
        }

        /// <summary>
        /// An engine part
        /// </summary>
        /// <param name="Value">The part number</param>
        /// <param name="Start">The start index of the part number</param>
        /// <param name="End">The end index of the part number</param>
        /// <param name="Row">The row which contains the part</param>
        private record Part(int Value, int Start, int End, int Row);

        /// <summary>
        /// A symbol on the engine schematic
        /// </summary>
        /// <param name="Value">Symbol value</param>
        /// <param name="Column">Schematic column</param>
        /// <param name="Row">Schematic row</param>
        private record Symbol(char Value, int Column, int Row)
        {
            /// <summary>
            /// Check if the symbol is adjacent to the given part
            /// </summary>
            /// <param name="part">Engine part</param>
            /// <returns>If the part is adjacent to this symbol</returns>
            public bool IsAdjacent(Part part) => this.Column >= part.Start - 1
                                              && this.Column <= part.End + 1
                                              && this.Row >= part.Row - 1
                                              && this.Row <= part.Row + 1;
        }
    }
}
