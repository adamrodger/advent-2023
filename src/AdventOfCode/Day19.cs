using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 19
    /// </summary>
    public class Day19
    {
        public int Part1(string[] input)
        {
            bool parseParts = false;

            Dictionary<string, PartRule> rules = new();
            List<Part> parts = new();

            foreach (string line in input)
            {
                if (string.IsNullOrEmpty(line))
                {
                    parseParts = true;
                    continue;
                }

                if (parseParts)
                {
                    parts.Add(Part.Parse(line));
                }
                else
                {
                    var rule = PartRule.Parse(line);
                    rules[rule.Id] = rule;
                }
            }

            int total = 0;

            foreach (Part part in parts)
            {
                string current = "in";

                while (current != "A" && current != "R")
                {
                    PartRule rule = rules[current];
                    current = rule.Predicates.First(p => p.Predicate(part)).Destination;
                }

                if (current == "A")
                {
                    total += part.Total;
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

        private record Part(int X, int M, int A, int S)
        {
            public int Total => X + M + A + S;

            public static Part Parse(string input)
            {
                var numbers = input.Numbers<int>();

                return new Part(numbers[0], numbers[1], numbers[2], numbers[3]);
            }
        }

        private record PartRule(string Id, IReadOnlyList<PartPredicate> Predicates)
        {
            /// <summary>
            /// Parse a string to a part rule
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static PartRule Parse(string input)
            {
                // px{a<2006:qkq,m>2090:A,rfg}

                int index = input.IndexOf('{');

                string id = input[..index];
                PartPredicate[] predicates = input[(index + 1)..^1].Split(',').Select(PartPredicate.Parse).ToArray();

                return new PartRule(id, predicates);
            }
        }

        private record PartPredicate(Predicate<Part> Predicate, string Destination)
        {
            public static PartPredicate Parse(string input)
            {
                // a<2006:qkq
                // m>2090:A
                // rfg

                char property = input[0];
                int colonIndex = input.IndexOf(':');

                Predicate<Part> predicate;
                string destination;

                if (input.Contains('<'))
                {
                    int n = int.Parse(input[2..colonIndex]);

                    predicate = property switch
                    {
                        'x' => part => part.X < n,
                        'm' => part => part.M < n,
                        'a' => part => part.A < n,
                        's' => part => part.S < n,
                    };

                    destination = input[(colonIndex + 1)..];
                }
                else if (input.Contains('>'))
                {
                    int n = int.Parse(input[2..colonIndex]);

                    predicate = property switch
                    {
                        'x' => part => part.X > n,
                        'm' => part => part.M > n,
                        'a' => part => part.A > n,
                        's' => part => part.S > n,
                    };

                    destination = input[(colonIndex + 1)..];
                }
                else
                {
                    predicate = _ => true;
                    destination = input;
                }

                return new PartPredicate(predicate, destination);
            }
        }
    }
}
