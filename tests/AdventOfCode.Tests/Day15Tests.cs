using System.IO;
using Xunit;
using Xunit.Abstractions;


namespace AdventOfCode.Tests
{
    public class Day15Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day15 solver;

        public Day15Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day15();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day15.txt");
            return input;
        }

        private static string[] GetSampleInput()
        {
            return new string[]
            {
                "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7"
            };
        }

        [Fact]
        public void Part1_SampleInput_ProducesCorrectResponse()
        {
            var expected = 1320;

            var result = solver.Part1(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 506869;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 15 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_SampleInput_ProducesCorrectResponse()
        {
            var expected = 145;

            var result = solver.Part2(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            var expected = 271384;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 15 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
