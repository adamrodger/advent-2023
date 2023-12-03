using System.IO;
using Xunit;
using Xunit.Abstractions;


namespace AdventOfCode.Tests
{
    public class Day3Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day3 solver;

        public Day3Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day3();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day3.txt");
            return input;
        }

        private static string[] GetSampleInput()
        {
            return new string[]
            {
                
            };
        }

        [Fact]
        public void Part1_SampleInput_ProducesCorrectResponse()
        {
            var expected = -1;

            var result = solver.Part1(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = -1;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 3 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_SampleInput_ProducesCorrectResponse()
        {
            var expected = -1;

            var result = solver.Part2(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            var expected = -1;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 3 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}