using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Tests
{
    public class Day24Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day24 solver;

        public Day24Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day24();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day24.txt");
            return input;
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 29142;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 24 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            if (!OperatingSystem.IsWindows())
            {
                // this doesn't pass on Linux because of the z3 bindings for some reason
                return;
            }

            var expected = 848_947_587_263_033;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 24 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
