using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Tests
{
    public class Day9Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day9 solver;

        public Day9Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day9();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day9.txt");
            return input;
        }

        private static string[] GetSampleInput()
        {
            return new string[]
            {
                "0 3 6 9 12 15",
                "1 3 6 10 15 21",
                "10 13 16 21 30 45"
            };
        }

        [Fact]
        public void Part1_SampleInput_ProducesCorrectResponse()
        {
            var expected = 114;

            var result = solver.Part1(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 2_043_183_816;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 9 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_SampleInput_ProducesCorrectResponse()
        {
            var expected = 2;

            var result = solver.Part2(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            var expected = 1118;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 9 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
