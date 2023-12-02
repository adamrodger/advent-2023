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
        public int Part1(string[] input) => input.Select(Game.Parse)
                                                 .Where(g => g.Rounds.All(r => r.IsPossible))
                                                 .Select(g => g.Id)
                                                 .Sum();

        public int Part2(string[] input) => input.Select(Game.Parse)
                                                 .Sum(g => g.Power());

        /// <summary>
        /// An entire game
        /// </summary>
        /// <param name="Id">Game ID</param>
        /// <param name="Rounds">All the rounds drawn in the game</param>
        private readonly record struct Game(int Id, ICollection<Draw> Rounds)
        {
            /// <summary>
            /// Parse a game from an entire line of draws in a string like "Game 13: 3 red, 11 green, 18 blue; 11 green, 1 red, 3 blue"
            /// </summary>
            /// <param name="line">Game line</param>
            /// <returns>Parsed game</returns>
            public static Game Parse(string line)
            {
                // TODO: Improve perf by using Spans and IndexOf instead of doing all these splits
                string[] idRounds = line.Split(": ");

                int id = int.Parse(idRounds[0].Split(' ')[1]);

                // 3 red, 11 green, 18 blue; 11 green, 1 red, 3 blue; 12 blue, 5 red, 2 green; 16 blue, 8 red, 5 green; 8 red, 12 blue, 19 green; 17 blue, 4 green, 6 red
                string[] rounds = idRounds[1].Split(';');

                var draws = rounds.Select(Draw.Parse).ToArray();
                return new Game(id, draws);
            }

            /// <summary>
            /// The power of the game is the lowest possible number of each coloured cube multiplied together
            /// </summary>
            /// <returns>Game power</returns>
            public int Power()
            {
                int redRequired = 0, greenRequired = 0, blueRequired = 0;

                foreach (Draw draw in this.Rounds)
                {
                    redRequired = Math.Max(redRequired, draw.Red);
                    greenRequired = Math.Max(greenRequired, draw.Green);
                    blueRequired = Math.Max(blueRequired, draw.Blue);
                }

                return redRequired * greenRequired * blueRequired;
            }
        }

        /// <summary>
        /// An individual draw of coloured cubes
        /// </summary>
        /// <param name="Red">Red cubes drawn</param>
        /// <param name="Green">Green cubes drawn</param>
        /// <param name="Blue">Blue cubes drawn</param>
        private record Draw(int Red, int Green, int Blue)
        {
            /// <summary>
            /// Parse the colours selected in a single draw from a string like "11 green, 1 red, 3 blue"
            /// </summary>
            /// <param name="line">Draw string</param>
            /// <returns>Parsed draw</returns>
            public static Draw Parse(string line)
            {
                // colours are all optional and default to 0
                int red = 0, green = 0, blue = 0;

                // 3 red, 11 green, 18 blue
                string[] colours = line.Trim().Split(", ");

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

            /// <summary>
            /// Is the game possible in part 1?
            /// </summary>
            public bool IsPossible => this.Red <= 12 && this.Green <= 13 && this.Blue <= 14;
        }
    }
}
