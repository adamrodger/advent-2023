using System.IO;
using Xunit;
using Xunit.Abstractions;


namespace AdventOfCode.Tests
{
    public class Day4Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day4 solver;

        public Day4Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day4();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day4.txt");
            return input;
        }

        private static string[] GetSampleInput()
        {
            return new string[]
            {
                "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
                "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
                "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
                "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
                "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
                "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11"
            };
        }

        [Fact]
        public void Part1_SampleInput_ProducesCorrectResponse()
        {
            var expected = 13;

            var result = solver.Part1(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 26218;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 4 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_SampleInput_ProducesCorrectResponse()
        {
            var expected = 30;

            var result = solver.Part2(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            var expected = 9997537;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 4 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
