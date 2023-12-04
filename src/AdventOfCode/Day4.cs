using System;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 4
    /// </summary>
    public class Day4
    {
        public int Part1(string[] input)
        {
            ScratchCard[] scratchCards = input.Select(ScratchCard.Parse).ToArray();
            return scratchCards.Select(s => s.Score).Sum();
        }

        public int Part2(string[] input)
        {
            ScratchCard[] scratchCards = input.Select(ScratchCard.Parse).ToArray();

            var copies = scratchCards.ToDictionary(s => s.Id, _ => 1);

            foreach (ScratchCard card in scratchCards)
            {
                for (int i = 0; i < card.Matches; i++)
                {
                    copies[card.Id + i + 1] += copies[card.Id];
                }
            }

            return copies.Values.Sum();
        }

        /// <summary>
        /// A scratch card
        /// </summary>
        /// <param name="Id">Card ID</param>
        /// <param name="Matches">Number of matching numbers</param>
        /// <param name="Score">Card score</param>
        private record ScratchCard(int Id, int Matches, int Score)
        {
            /// <summary>
            /// Parse a scratch card from input like "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53"
            /// </summary>
            /// <param name="line">Input line</param>
            /// <returns>Scratch card</returns>
            public static ScratchCard Parse(string line)
            {
                string[] idNums = line.Split(": ");

                int id = int.Parse(idNums[0][5..]);

                string[] winningDrawn = idNums[1].Split(" | ");

                var winning = winningDrawn[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToHashSet();
                var drawn = winningDrawn[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToHashSet();

                int matches = winning.Intersect(drawn).Count();
                int score = matches > 0 ? (int)Math.Pow(2, matches - 1) : 0;

                return new ScratchCard(id, matches, score);
            }
        }
    }
}
