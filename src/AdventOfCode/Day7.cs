using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 7
    /// </summary>
    public class Day7
    {
        public long Part1(string[] input) => input.Select(Hand.Parse)
                                                  .OrderBy(h => h, new Part1Comparer())
                                                  .Select((hand, rank) => hand.Bid * ((long)rank + 1))
                                                  .Sum();

        public long Part2(string[] input) => input.Select(Hand.Parse)
                                                  .OrderBy(h => h, new Part2Comparer())
                                                  .Select((hand, rank) => hand.Bid * ((long)rank + 1))
                                                  .Sum();
        // 252525782 - too high
        // 252285416 - too high
        // 251655412 - too high

        private enum Card
        {
            Ace = 14,
            King = 13,
            Queen = 12,
            Jack = 11,
            Ten = 10,
            Nine = 9,
            Eight = 8,
            Seven = 7,
            Six = 6,
            Five = 5,
            Four = 4,
            Three = 3,
            Two = 2
        }

        private record Hand(Card[] Cards, long Bid)
        {
            public static Hand Parse(string line)
            {
                var cards = line[..5].Select(c => c switch
                {
                    'A' => Card.Ace,
                    'K' => Card.King,
                    'Q' => Card.Queen,
                    'J' => Card.Jack,
                    'T' => Card.Ten,
                    '9' => Card.Nine,
                    '8' => Card.Eight,
                    '7' => Card.Seven,
                    '6' => Card.Six,
                    '5' => Card.Five,
                    '4' => Card.Four,
                    '3' => Card.Three,
                    '2' => Card.Two,
                    _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
                }).ToArray();

                long bid = long.Parse(line[6..]);

                return new Hand(cards, bid);
            }
        }

        private class Part1Comparer : IComparer<Hand>
        {
            public int Compare(Hand left, Hand right)
            {
                var leftGroups = new Dictionary<Card, int>();
                var rightGroups = new Dictionary<Card, int>();

                foreach (Card card in left.Cards)
                {
                    leftGroups[card] = leftGroups.GetOrCreate(card) + 1;
                }

                foreach (Card card in right.Cards)
                {
                    rightGroups[card] = rightGroups.GetOrCreate(card) + 1;
                }

                /*
                 * Compare the powers of each hand
                 *
                 * Possible:
                 * 5
                 * 4, 1
                 * 3, 2
                 * 3, 1, 1
                 * 2, 2, 1
                 * 2, 1, 1, 1
                 * 1, 1, 1, 1, 1
                 */
                var powers = leftGroups.Values.OrderByDescending(x => x).Zip(rightGroups.Values.OrderByDescending(x => x));

                foreach ((int leftGroup, int rightGroup) in powers)
                {
                    int comparison = leftGroup.CompareTo(rightGroup);

                    if (comparison != 0)
                    {
                        return comparison;
                    }
                }

                // hands have equal power, compare card values left to right
                foreach ((Card leftCard, Card rightCard) in left.Cards.Zip(right.Cards))
                {
                    int comparison = leftCard.CompareTo(rightCard);

                    if (comparison != 0)
                    {
                        return comparison;
                    }
                }

                // hands are identical
                return 0;
            }
        }

        private class Part2Comparer : IComparer<Hand>
        {
            public int Compare(Hand left, Hand right)
            {
                var leftGroups = new Dictionary<Card, int>();
                var rightGroups = new Dictionary<Card, int>();

                foreach (Card card in left.Cards)
                {
                    leftGroups[card] = leftGroups.GetOrCreate(card) + 1;
                }

                foreach (Card card in right.Cards)
                {
                    rightGroups[card] = rightGroups.GetOrCreate(card) + 1;
                }

                /*
                 * Compare the powers of each hand
                 *
                 * Possible:
                 * 5
                 * 4, 1
                 * 3, 2
                 * 3, 1, 1
                 * 2, 2, 1
                 * 2, 1, 1, 1
                 * 1, 1, 1, 1, 1
                 */

                int leftJokers = leftGroups.GetValueOrDefault(Card.Jack);
                int rightJokers = rightGroups.GetValueOrDefault(Card.Jack);

                // jokers join the biggest group
                if (left.Cards.Any(c => c == Card.Jack))
                {
                    if (left.Cards.All(c => c == Card.Jack))
                    {
                        // unless they're all jokers, then they all just pretend to be 5 of a kind Aces
                        leftGroups[Card.Ace] = 5;
                    }
                    else
                    {
                        Card leftMaxGroup = leftGroups.Where(g => g.Key != Card.Jack).MaxBy(g => g.Value).Key;
                        leftGroups[leftMaxGroup] += leftJokers;
                    }
                }

                // jokers join the biggest group
                if (right.Cards.Any(c => c == Card.Jack))
                {
                    if (right.Cards.All(c => c == Card.Jack))
                    {
                        // unless they're all jokers, then they all just pretend to be 5 of a kind Aces
                        rightGroups[Card.Ace] = 5;
                    }
                    else
                    {
                        Card rightMaxGroup = rightGroups.Where(g => g.Key != Card.Jack).MaxBy(g => g.Value).Key;
                        rightGroups[rightMaxGroup] += rightJokers;
                    }
                }

                IEnumerable<int> leftNonJokerGroups = leftGroups.Where(g => g.Key != Card.Jack)
                                                                .Select(g => g.Value)
                                                                .OrderByDescending(x => x);

                IEnumerable<int> rightNonJokerGroups = rightGroups.Where(g => g.Key != Card.Jack)
                                                                  .Select(g => g.Value)
                                                                  .OrderByDescending(x => x);

                var powers = leftNonJokerGroups.Zip(rightNonJokerGroups);

                foreach ((int leftGroup, int rightGroup) in powers)
                {
                    int comparison = leftGroup.CompareTo(rightGroup);

                    if (comparison != 0)
                    {
                        return comparison;
                    }
                }

                // hands have equal power, compare card values left to right
                foreach ((Card leftCard, Card rightCard) in left.Cards.Zip(right.Cards))
                {
                    if (leftCard == Card.Jack && rightCard != Card.Jack)
                    {
                        return -1;
                    }

                    if (leftCard != Card.Jack && rightCard == Card.Jack)
                    {
                        return 1;
                    }

                    int comparison = leftCard.CompareTo(rightCard);

                    if (comparison != 0)
                    {
                        return comparison;
                    }
                }

                // hands are identical
                return 0;
            }
        }
    }
}
