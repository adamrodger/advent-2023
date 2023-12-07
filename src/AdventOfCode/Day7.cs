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
                                                 .OrderBy(h => h)
                                                 .Select((hand, rank) => hand.Bid * (rank + 1))
                                                 .Sum();

        public int Part2(string[] input) => input.Select(line => line.Replace('J', '*')) // replace Jack with Joker
                                                 .Select(Hand.Parse)
                                                 .OrderBy(h => h)
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
            Two = 2,
            Joker = 1,
        }

        /// <summary>
        /// Rank of each hand
        /// </summary>
        private enum HandRank
        {
            HighCard = 1,
            Pair = 2,
            TwoPair = 3,
            ThreeOfAKind = 4,
            FullHouse = 5,
            FourOfAKind = 6,
            FiveOfAKind = 7
        }

        /// <summary>
        /// A hand of cards in Camel Cards
        /// </summary>
        /// <param name="Cards">Cards</param>
        /// <param name="Rank">Hand rank</param>
        /// <param name="Bid">Hand bid</param>
        private record Hand(Card[] Cards, HandRank Rank, int Bid) : IComparable<Hand>
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
                    '*' => Card.Joker,
                    _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
                }).ToArray();

                HandRank rank = CalculateRank(cards);

                int bid = int.Parse(line[6..]);

                return new Hand(cards, rank, bid);
            }

            /// <summary>
            /// Compare this hand to another hand
            /// </summary>
            /// <param name="other">Other hand</param>
            /// <returns>Comparison result</returns>
            public int CompareTo(Hand other)
            {
                int comparison = this.Rank.CompareTo(other.Rank);

                if (comparison != 0)
                {
                    return comparison;
                }

                // hands have equal rank, compare card values one by one
                foreach ((Card leftCard, Card rightCard) in this.Cards.Zip(other.Cards))
                {
                    comparison = leftCard.CompareTo(rightCard);

                    if (comparison != 0)
                    {
                        return comparison;
                    }
                }

                // hands are identical
                return 0;
            }

            /// <summary>
            /// Calculate the best rank for this hand of cards
            /// </summary>
            /// <param name="cards">Cards</param>
            /// <returns>Hand rank</returns>
            private static HandRank CalculateRank(IEnumerable<Card> cards)
            {
                // group cards by type
                var groups = new Dictionary<Card, int>(5);

                foreach (Card card in cards)
                {
                    int value = groups.GetOrDefault(card);
                    groups[card] = value + 1;
                }

                // move the jokers (if any) to create the strongest possible hand
                MoveJokers(groups);

                // use the group counts to determine rank
                int[] counts = groups.Values.OrderDescending().ToArray();

                return counts switch
                {
                    [5] => HandRank.FiveOfAKind,
                    [4, 1] => HandRank.FourOfAKind,
                    [3, 2] => HandRank.FullHouse,
                    [3, 1, 1] => HandRank.ThreeOfAKind,
                    [2, 2, 1] => HandRank.TwoPair,
                    [2, 1, 1, 1] => HandRank.Pair,
                    [1, 1, 1, 1, 1] => HandRank.HighCard,
                    _ => throw new ArgumentOutOfRangeException(nameof(counts), counts, null)
                };
            }

            /// <summary>
            /// Move any jokers in the groups to create the strongest hand possible
            /// </summary>
            /// <param name="groups">Existing card groups, which will be modified if any jokers need to move</param>
            private static void MoveJokers(IDictionary<Card, int> groups)
            {
                int jokers = groups.GetOrDefault(Card.Joker);

                if (jokers == 0)
                {
                    // nothing to move
                    return;
                }

                // the jokers are going to move somewhere else
                groups.Remove(Card.Joker);

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
