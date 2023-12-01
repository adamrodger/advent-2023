using System.IO;
using Xunit;
using Xunit.Abstractions;


namespace AdventOfCode.Tests
{
    public class Day1Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day1 solver;

        public Day1Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day1();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day1.txt");
            return input;
        }

        [Fact]
        public void Part1_SampleInput_ProducesCorrectResponse()
        {
            var expected = 142;

            var result = solver.Part1(
            [
                "1abc2",
                "pqr3stu8vwx",
                "a1b2c3d4e5f",
                "treb7uchet",
            ]);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 56049;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 1 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_SampleInput_ProducesCorrectResponse()
        {
            var expected = 281;

            var result = solver.Part2(
            [
                "two1nine",
                "eightwothree",
                "abcone2threexyz",
                "xtwone3four",
                "4nineeightseven2",
                "zoneight234",
                "7pqrstsixteen",
            ]);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            var expected = 54530;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 1 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
