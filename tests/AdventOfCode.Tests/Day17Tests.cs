using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Tests
{
    public class Day17Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day17 solver;

        public Day17Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day17();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day17.txt");
            return input;
        }

        private static string[] GetSampleInput()
        {
            return new string[]
            {
                "2413432311323",
                "3215453535623",
                "3255245654254",
                "3446585845452",
                "4546657867536",
                "1438598798454",
                "4457876987766",
                "3637877979653",
                "4654967986887",
                "4564679986453",
                "1224686865563",
                "2546548887735",
                "4322674655533",
            };
        }

        [Fact]
        public void Part1_SampleInput_ProducesCorrectResponse()
        {
            var expected = 102;

            var result = solver.Part1(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 928;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 17 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_SampleInput_ProducesCorrectResponse()
        {
            var expected = 94;

            var result = solver.Part2(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            var expected = 1104;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 17 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
