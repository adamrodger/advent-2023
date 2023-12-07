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
        public int Part1(string[] input) => input.Select(Hand.Parse)
                                                 .OrderBy(h => h, new Part1Comparer())
                                                 .Select((hand, rank) => hand.Bid * (rank + 1))
                                                 .Sum();

        public int Part2(string[] input) => input.Select(Hand.Parse)
                                                 .OrderBy(h => h, new Part2Comparer())
                                                 .Select((hand, rank) => hand.Bid * (rank + 1))
                                                 .Sum();

        /// <summary>
        /// A card in the hand
        /// </summary>
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

        /// <summary>
        /// A hand of cards in Camel Cards
        /// </summary>
        /// <param name="Cards">Cards</param>
        /// <param name="Bid">Hand bid</param>
        private record Hand(Card[] Cards, int Bid)
        {
            /// <summary>
            /// Parse the line to a hand
            /// </summary>
            /// <param name="line">Input line</param>
            /// <returns>Hand</returns>
            public static Hand Parse(string line)
            {
                Card[] cards = line[..5].Select(c => c switch
                {
                    // wouldn't it be nice if we could add static Parse methods to enums?
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

                int bid = int.Parse(line[6..]);

                return new Hand(cards, bid);
            }
        }

        /// <summary>
        /// Compare two hands using the rules for part 1
        /// </summary>
        private class Part1Comparer : IComparer<Hand>
        {
            /// <summary>
            /// Compare the hands
            /// </summary>
            /// <param name="left">Left hand</param>
            /// <param name="right">Right hand</param>
            /// <returns>Comparison result</returns>
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

        /// <summary>
        /// Compare two hands using the rules for part 2, where Jacks count as Jokers
        /// </summary>
        private class Part2Comparer : IComparer<Hand>
        {
            /// <summary>
            /// Compare the hands
            /// </summary>
            /// <param name="left">Left hand</param>
            /// <param name="right">Right hand</param>
            /// <returns>Comparison result</returns>
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

                // move the jokers to create the strongest possible hand
                MoveJokers(leftGroups);
                MoveJokers(rightGroups);

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
                    if (leftCard == Card.Jack && rightCard != Card.Jack)
                    {
                        // jokers always lose against non-jokers
                        return -1;
                    }

                    if (leftCard != Card.Jack && rightCard == Card.Jack)
                    {
                        // jokers always lose against non-jokers
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

            /// <summary>
            /// Move any jokers in the groups to create the strongest hand possible
            /// </summary>
            /// <param name="groups">Existing card groups, which will be modified if any jokers need to move</param>
            private static void MoveJokers(IDictionary<Card, int> groups)
            {
                int jokers = groups.GetOrDefault(Card.Jack);

                if (jokers == 0)
                {
                    // nothing to move
                    return;
                }

                // the jokers are going to move somewhere else
                groups.Remove(Card.Jack);

                if (jokers == 5)
                {
                    // no group to join, so they all pretend to be Aces to create 5 of a kind
                    groups[Card.Ace] = jokers;
                    return;
                }

                // they join the current strongest card to make it even stronger
                Card strongest = groups.MaxBy(g => g.Value).Key;
                groups[strongest] += jokers;
            }
        }
    }
}
