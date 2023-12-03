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

            int total = 0;

            foreach (Part part in schematic.Parts)
            {
                if (part.Edges().Any(schematic.Symbols.ContainsKey))
                {
                    total += part.Value;
                }
            }

            return total;
        }

        public int Part2(string[] input)
        {
            Schematic schematic = Schematic.Parse(input);

            int total = 0;

            foreach (Symbol symbol in schematic.Symbols.Values.Where(s => s.Value == '*'))
            {
                // is it adjacent to 2 parts?
                // TODO: Improve perf by not allocating an array for each search
                Part[] adjacent = schematic.Parts.Where(symbol.IsAdjacent).ToArray();

                if (adjacent.Length == 2)
                {
                    total += adjacent[0].Value * adjacent[1].Value;
                }
            }

            return total;
        }

        /// <summary>
        /// The engine schematic, with the parts and symbols that comprise it
        /// </summary>
        /// <param name="Parts">Parts</param>
        /// <param name="Symbols">Symbols, indexed by their location</param>
        private readonly record struct Schematic(ICollection<Part> Parts, IDictionary<Point2D, Symbol> Symbols)
        {
            /// <summary>
            /// Parse the schematic from the input
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Parsed engine schematic</returns>
            public static Schematic Parse(IEnumerable<string> input)
            {
                var symbols = new Dictionary<Point2D, Symbol>();
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
                            Point2D location = new Point2D(x, y);
                            symbols[location] = new Symbol(c, x, y);
                            continue;
                        }

                        ReadOnlySpan<char> number = line.Skip(x).TakeWhile(char.IsAsciiDigit).ToArray();
                        parts.Add(new Part(int.Parse(number), x, x + number.Length - 1, y));
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
        private record Part(int Value, int Start, int End, int Row)
        {
            /// <summary>
            /// Walk around all the edge locations of the part
            /// </summary>
            /// <returns>Edge locations</returns>
            public IEnumerable<Point2D> Edges()
            {
                // left side
                yield return (this.Start - 1, this.Row - 1);
                yield return (this.Start - 1, this.Row);
                yield return (this.Start - 1, this.Row + 1);
                
                // right side
                yield return (this.End + 1, this.Row - 1);
                yield return (this.End + 1, this.Row);
                yield return (this.End + 1, this.Row + 1);

                // top and bottom
                for (int x = this.Start; x <= this.End; x++)
                {
                    yield return (x, this.Row - 1);
                    yield return (x, this.Row + 1);
                }
            }
        }

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
