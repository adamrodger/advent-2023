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
                                                  .OrderBy(h => h)
                                                  .Select((hand, rank) => hand.Bid * ((long)rank + 1))
                                                  .Sum();

        public int Part2(string[] input)
        {
            foreach (string line in input)
            {
                throw new NotImplementedException("Part 2 not implemented");
            }

            return 0;
        }

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

        private record Hand(Card[] Cards, long Bid) : IComparable<Hand>
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

            /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
            /// <param name="other">An object to compare with this instance.</param>
            /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
            /// <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description></item><item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description></item></list></returns>
            public int CompareTo(Hand other)
            {
                var thisGroups = new Dictionary<Card, int>();
                var otherGroups = new Dictionary<Card, int>();

                foreach (Card card in this.Cards)
                {
                    thisGroups[card] = thisGroups.GetOrCreate(card) + 1;
                }

                foreach (Card card in other.Cards)
                {
                    otherGroups[card] = otherGroups.GetOrCreate(card) + 1;
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
                var powers = thisGroups.Values.OrderByDescending(x => x).Zip(otherGroups.Values.OrderByDescending(x => x));

                foreach ((int thisGroup, int otherGroup) in powers)
                {
                    int comparison = thisGroup.CompareTo(otherGroup);

                    if (comparison != 0)
                    {
                        return comparison;
                    }
                }

                // hands have equal power, compare card values left to right
                foreach ((Card thisCard, Card otherCard) in this.Cards.Zip(other.Cards))
                {
                    int comparison = thisCard.CompareTo(otherCard);

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
