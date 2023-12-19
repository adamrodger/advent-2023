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
            Dictionary<string, PartRule> rules = input.TakeWhile(line => !string.IsNullOrEmpty(line))
                                                      .Select(PartRule.Parse)
                                                      .ToDictionary(r => r.Id);

            return input.SkipWhile(line => !string.IsNullOrEmpty(line)) // skip the rule definitions
                        .Skip(1) // skip the blank line
                        .Select(Part.Parse)
                        .Where(p => p.IsAccepted(rules))
                        .Sum(p => p.Total);
        }

        public long Part2(string[] input)
        {
            Dictionary<string, PartRule> rules = input.TakeWhile(line => !string.IsNullOrEmpty(line))
                                                      .Select(PartRule.Parse)
                                                      .ToDictionary(r => r.Id);

            rules["A"] = new PartRule("A", Array.Empty<PartPredicate>());
            rules["R"] = new PartRule("R", Array.Empty<PartPredicate>());

            return rules["in"].PossibleCombinations(PartBounds.Default, rules);
        }

        /// <summary>
        /// Machine part
        /// </summary>
        /// <param name="X">Extremely cool looking score</param>
        /// <param name="M">Musical score</param>
        /// <param name="A">Aerodynamic score</param>
        /// <param name="S">Shiny score</param>
        private record Part(int X, int M, int A, int S)
        {
            /// <summary>
            /// Total score for the part
            /// </summary>
            public int Total => this.X + this.M + this.A + this.S;

            /// <summary>
            /// Parse a part from input
            /// </summary>
            /// <param name="input">Input line</param>
            /// <returns>Part</returns>
            public static Part Parse(string input)
            {
                var numbers = input.Numbers<int>();

                return new Part(numbers[0], numbers[1], numbers[2], numbers[3]);
            }

            /// <summary>
            /// Check if the part would be accepted according to the given rules
            /// </summary>
            /// <param name="rules">Part rules</param>
            /// <returns>Part is accepted</returns>
            public bool IsAccepted(IDictionary<string, PartRule> rules)
            {
                string current = "in";

                while (current != "A" && current != "R")
                {
                    PartRule rule = rules[current];
                    current = rule.Predicates.First(p => p.Matches(this)).Destination;
                }

                return current == "A";
            }
        }

        /// <summary>
        /// Rule for matching a part
        /// </summary>
        /// <param name="Id">Rule ID</param>
        /// <param name="Predicates">Rule predicates</param>
        private record PartRule(string Id, IReadOnlyList<PartPredicate> Predicates)
        {
            /// <summary>
            /// Parse a part rule from input
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Part rule</returns>
            public static PartRule Parse(string input)
            {
                // Part rules look like:
                //     px{a<2006:qkq,m>2090:A,rfg}

                int index = input.IndexOf('{');

                string id = input[..index];
                PartPredicate[] predicates = input[(index + 1)..^1].Split(',').Select(PartPredicate.Parse).ToArray();

                return new PartRule(id, predicates);
            }

            /// <summary>
            /// Check how many valid part combinations match the current rule from the given score bounds
            /// </summary>
            /// <param name="bounds">Part score bounds</param>
            /// <param name="rules">Part rules lookup</param>
            /// <returns>Possible combinations of parts that match the given bounds for this rule</returns>
            public long PossibleCombinations(PartBounds bounds, IDictionary<string, PartRule> rules)
            {
                if (this.Id == "R")
                {
                    return 0;
                }

                if (this.Id == "A")
                {
                    return bounds.Combinations;
                }

                long total = 0;

                foreach (PartPredicate predicate in this.Predicates)
                {
                    // reduce the valid bounds according to the current predicate
                    PartBounds matchedBounds = bounds.WhenMatching(predicate);
                    total += rules[predicate.Destination].PossibleCombinations(matchedBounds, rules);

                    // assume next predicate didn't match this one, so reduce the bounds accordingly in the opposite direction
                    bounds = bounds.WhenNotMatching(predicate);
                }

                return total;
            }
        }

        /// <summary>
        /// Predicate for matching a part
        /// </summary>
        /// <param name="Property">Matched property</param>
        /// <param name="Operation">Match operation</param>
        /// <param name="Value">Match value</param>
        /// <param name="Destination">Destination if matched</param>
        private record PartPredicate(char Property, PartOperation Operation, int Value, string Destination)
        {
            /// <summary>
            /// Parse a predicate from input
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Predicate</returns>
            public static PartPredicate Parse(string input)
            {
                // Predicate will be in one of the forms:
                //     a<2006:qkq
                //     m>2090:A
                //     rfg

                int colonIndex = input.IndexOf(':');

                if (colonIndex < 0)
                {
                    // third input type - always matches and just passes through to the given ID
                    return new PartPredicate('?', PartOperation.PassThrough, 0, input);
                }

                char property = input[0];
                PartOperation operation = input.Contains('<') ? PartOperation.LessThan : PartOperation.GreaterThan;
                int value = int.Parse(input[2..colonIndex]);
                string destination = input[(colonIndex + 1)..];

                return new PartPredicate(property, operation, value, destination);
            }

            /// <summary>
            /// Check if the predicate matches the given part
            /// </summary>
            /// <param name="part">Part</param>
            /// <returns>Predicate matches</returns>
            public bool Matches(Part part) => (this.Property, this.Operation) switch
            {
                (_, PartOperation.PassThrough) => true,
                ('x', PartOperation.LessThan) => part.X < this.Value,
                ('x', PartOperation.GreaterThan) => part.X > this.Value,
                ('m', PartOperation.LessThan) => part.M < this.Value,
                ('m', PartOperation.GreaterThan) => part.M > this.Value,
                ('a', PartOperation.LessThan) => part.A < this.Value,
                ('a', PartOperation.GreaterThan) => part.A > this.Value,
                ('s', PartOperation.LessThan) => part.S < this.Value,
                ('s', PartOperation.GreaterThan) => part.S > this.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Part predicate operation
        /// </summary>
        private enum PartOperation
        {
            /// <summary>
            /// The source property must be greater than the predicate value to follow the target
            /// </summary>
            GreaterThan,

            /// <summary>
            /// The source property must be less than the predicate value to follow the target
            /// </summary>
            LessThan,

            /// <summary>
            /// The predicate always matches and we just pass through to the target
            /// </summary>
            PassThrough
        }

        /// <summary>
        /// Bounds on which parts count as valid
        /// </summary>
        /// <param name="MinX">Minimum X value</param>
        /// <param name="MaxX">Maximum X value</param>
        /// <param name="MinM">Minimum M value</param>
        /// <param name="MaxM">Maximum M value</param>
        /// <param name="MinA">Minimum A value</param>
        /// <param name="MaxA">Maximum A value</param>
        /// <param name="MinS">Minimum S value</param>
        /// <param name="MaxS">Maximum S value</param>
        private record PartBounds(int MinX, int MaxX, int MinM, int MaxM, int MinA, int MaxA, int MinS, int MaxS)
        {
            /// <summary>
            /// Default bound limits
            /// </summary>
            public static readonly PartBounds Default = new(1, 4000, 1, 4000, 1, 4000, 1, 4000);

            /// <summary>
            /// Number of valid part combinations according to the current bounds
            /// </summary>
            public long Combinations => (long)(MaxX - MinX + 1) * (MaxM - MinM + 1) * (MaxA - MinA + 1) * (MaxS - MinS + 1);

            /// <summary>
            /// Calculate the new bounds as if the predicate was matched
            /// </summary>
            /// <param name="predicate">Predicate</param>
            /// <returns>New bounds</returns>
            public PartBounds WhenMatching(PartPredicate predicate) => (predicate.Property, predicate.Operation) switch
            {
                (_, PartOperation.PassThrough) => this,
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

            /// <summary>
            /// Calculate the new bounds as if the predicate wasn't matched
            /// </summary>
            /// <param name="predicate">Predicate</param>
            /// <returns>New bounds</returns>
            public PartBounds WhenNotMatching(PartPredicate predicate) => (predicate.Property, predicate.Operation) switch
            {
                (_, PartOperation.PassThrough) => this,
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
