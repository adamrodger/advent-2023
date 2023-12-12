using System.IO;
using Xunit;
using Xunit.Abstractions;


namespace AdventOfCode.Tests
{
    public class Day12Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day12 solver;

        public Day12Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day12();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day12.txt");
            return input;
        }

        [Theory]
        [InlineData("???.### 1,1,3", 1)]
        [InlineData(".??..??...?##. 1,1,3", 4)]
        [InlineData("?#?#?#?#?#?#?#? 1,3,1,6", 1)]
        [InlineData("????.#...#... 4,1,1", 1)]
        [InlineData("????.######..#####. 1,6,5", 4)]
        [InlineData("?###???????? 3,2,1", 10)]
        public void Part1_SampleInput_ProducesCorrectResponse(string line, int expected)
        {
            var result = solver.Part1([line]);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 7694;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 12 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("???.### 1,1,3", 1)]
        [InlineData(".??..??...?##. 1,1,3", 16384)]
        [InlineData("?#?#?#?#?#?#?#? 1,3,1,6", 1)]
        [InlineData("????.#...#... 4,1,1", 16)]
        [InlineData("????.######..#####. 1,6,5", 2500)]
        [InlineData("?###???????? 3,2,1", 506250)]
        public void Part2_SampleInput_ProducesCorrectResponse(string line, int expected)
        {
            var result = solver.Part2([line]);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            var expected = 5071883216318;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 12 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
