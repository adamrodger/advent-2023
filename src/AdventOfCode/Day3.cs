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
            var symbols = new Dictionary<Point2D, Symbol>();
            var parts = new List<Part>();

            // parse
            for (int y = 0; y < input.Length; y++)
            {
                string line = input[y];

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

            int total = 0;

            foreach (Part part in parts)
            {
                var locations = Enumerable.Range(part.Row - 1, 3)
                                          .SelectMany(y => Enumerable.Range(part.Start - 1, part.End - part.Start + 3)
                                                                     .Select(x => new Point2D(x, y)));

                if (locations.Any(symbols.ContainsKey))
                {
                    total += part.Value;
                }
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

        private record Part(int Value, int Start, int End, int Row);

        private record Symbol(char Value, int Column, int Row);
    }
}
