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

        public long Part2(string[] input)
        {
            Dictionary<string, PartRule> rules = input.TakeWhile(line => !string.IsNullOrEmpty(line))
                                                      .Select(PartRule.Parse)
                                                      .ToDictionary(r => r.Id);

            // 171210436954933152 -- too high

            return PossibleCombinations(rules["in"], PartBounds.Default, rules);
        }

        private long PossibleCombinations(PartRule current, PartBounds bounds, Dictionary<string, PartRule> rules)
        {
            long total = 0;

            foreach (PartPredicate predicate in current.Predicates)
            {
                if (predicate.Destination == "R")
                {
                    return 0;
                }

                if (predicate.Destination == "A")
                {
                    return bounds.Possibilities;
                }

                PartRule destination = rules[predicate.Destination];

                total += PossibleCombinations(destination, bounds.MatchedBounds(predicate), rules);
                total += PossibleCombinations(destination, bounds.NotMatchedBounds(predicate), rules);
            }

            return total;
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

        private record PartPredicate(Predicate<Part> Predicate, char Property, PartOperation Operation, int Value, string Destination)
        {
            public static PartPredicate Parse(string input)
            {
                // a<2006:qkq
                // m>2090:A
                // rfg

                char property = input[0];
                int colonIndex = input.IndexOf(':');

                Predicate<Part> predicate;
                PartOperation operation;
                string destination;
                int value;

                if (input.Contains('<'))
                {
                    value = int.Parse(input[2..colonIndex]);

                    predicate = property switch
                    {
                        'x' => part => part.X < value,
                        'm' => part => part.M < value,
                        'a' => part => part.A < value,
                        's' => part => part.S < value,
                    };

                    operation = PartOperation.LessThan;

                    destination = input[(colonIndex + 1)..];
                }
                else if (input.Contains('>'))
                {
                    value = int.Parse(input[2..colonIndex]);

                    predicate = property switch
                    {
                        'x' => part => part.X > value,
                        'm' => part => part.M > value,
                        'a' => part => part.A > value,
                        's' => part => part.S > value,
                    };

                    operation = PartOperation.GreaterThan;

                    destination = input[(colonIndex + 1)..];
                }
                else
                {
                    predicate = _ => true;
                    operation = PartOperation.Follow;
                    destination = input;
                    value = 0;
                }

                return new PartPredicate(predicate, property, operation, value, destination);
            }
        }

        private enum PartOperation
        {
            GreaterThan,
            LessThan,
            Follow
        }

        private record PartBounds(int MinX, int MaxX, int MinM, int MaxM, int MinA, int MaxA, int MinS, int MaxS)
        {
            public static readonly PartBounds Default = new(1, 4000, 1, 4000, 1, 4000, 1, 4000);

            public long Possibilities => (long)(MaxX - MinX) * (MaxM - MinM) * (MaxA - MinA) * (MaxS - MinS);

            public PartBounds MatchedBounds(PartPredicate predicate) => (predicate.Property, predicate.Operation) switch
            {
                (_, PartOperation.Follow) => this,
                ('x', PartOperation.LessThan) => this with { MaxX = predicate.Value - 1 },
                ('x', PartOperation.GreaterThan) => this with { MinX = predicate.Value + 1 },
                ('m', PartOperation.LessThan) => this with { MaxM = predicate.Value - 1 },
                ('m', PartOperation.GreaterThan) => this with { MinM = predicate.Value + 1 },
                ('a', PartOperation.LessThan) => this with { MaxA = predicate.Value - 1 },
                ('a', PartOperation.GreaterThan) => this with { MinA = predicate.Value + 1 },
                ('s', PartOperation.LessThan) => this with { MaxS = predicate.Value - 1 },
                ('s', PartOperation.GreaterThan) => this with { MinS = predicate.Value + 1 },
                _ => throw new ArgumentOutOfRangeException(nameof(predicate), predicate, $"Invalid predicate {predicate}")
            };

            public PartBounds NotMatchedBounds(PartPredicate predicate) => (predicate.Property, predicate.Operation) switch
            {
                (_, PartOperation.Follow) => this,
                ('x', PartOperation.LessThan) => this with { MinX = predicate.Value },
                ('x', PartOperation.GreaterThan) => this with { MaxX = predicate.Value },
                ('m', PartOperation.LessThan) => this with { MinM = predicate.Value },
                ('m', PartOperation.GreaterThan) => this with { MaxM = predicate.Value },
                ('a', PartOperation.LessThan) => this with { MinA = predicate.Value },
                ('a', PartOperation.GreaterThan) => this with { MaxA = predicate.Value },
                ('s', PartOperation.LessThan) => this with { MinS = predicate.Value },
                ('s', PartOperation.GreaterThan) => this with { MaxS = predicate.Value },
                _ => throw new ArgumentOutOfRangeException(nameof(predicate), predicate, $"Invalid predicate {predicate}")
            };
        }
    }
}
