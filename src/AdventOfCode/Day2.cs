using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 2
    /// </summary>
    public class Day2
    {
        public int Part1(string[] input)
        {
            // Game 13: 3 red, 11 green, 18 blue; 11 green, 1 red, 3 blue; 12 blue, 5 red, 2 green; 16 blue, 8 red, 5 green; 8 red, 12 blue, 19 green; 17 blue, 4 green, 6 red

            int total = 0;

            foreach (string line in input)
            {
                string[] idInstructions = line.Split(": ");

                int id = int.Parse(idInstructions[0].Split(' ')[1]);

                // 3 red, 11 green, 18 blue; 11 green, 1 red, 3 blue; 12 blue, 5 red, 2 green; 16 blue, 8 red, 5 green; 8 red, 12 blue, 19 green; 17 blue, 4 green, 6 red
                string[] rounds = idInstructions[1].Split(';');

                if (rounds.All(DrawIsPossible))
                {
                    total += id;
                }
            }

            return total;
        }

        private static bool DrawIsPossible(string draw)
        {
            var result = ParseDraw(draw);
            return result.Red <= 12 && result.Green <= 13 && result.Blue <= 14;
        }

        private static Game ParseGame(string line)
        {
            string[] idInstructions = line.Split(": ");

            int id = int.Parse(idInstructions[0].Split(' ')[1]);

            // 3 red, 11 green, 18 blue; 11 green, 1 red, 3 blue; 12 blue, 5 red, 2 green; 16 blue, 8 red, 5 green; 8 red, 12 blue, 19 green; 17 blue, 4 green, 6 red
            string[] rounds = idInstructions[1].Split(';');

            var draws = rounds.Select(ParseDraw).ToArray();
            return new Game(id, draws);
        }

        private static Draw ParseDraw(string draw)
        {
            int red = 0, green = 0, blue = 0;

            // 3 red, 11 green, 18 blue
            string[] colours = draw.Trim().Split(", ");

            foreach (string colour in colours)
            {
                string[] parts = colour.Split();

                switch (parts[1])
                {
                    case "red":
                        red = int.Parse(parts[0]);
                        break;
                    case "green":
                        green = int.Parse(parts[0]);
                        break;
                    case "blue":
                        blue = int.Parse(parts[0]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new Draw(red, green, blue);
        }

        public long Part2(string[] input)
        {
            long total = 0;

            foreach (string line in input)
            {
                Game game = ParseGame(line);

                int minRed = game.Rounds.Max(g => g.Red);
                int minGreen = game.Rounds.Max(g => g.Green);
                int minBlue = game.Rounds.Max(g => g.Blue);

                total += minRed * minGreen * minBlue;
            }

            return total;
        }

        private record Game(int Id, ICollection<Draw> Rounds);

        private record Draw(int Red, int Green, int Blue);
    }
}
