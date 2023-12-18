using System.IO;
using Xunit;
using Xunit.Abstractions;


namespace AdventOfCode.Tests
{
    public class Day18Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day18 solver;

        public Day18Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day18();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day18.txt");
            return input;
        }

        private static string[] GetSampleInput()
        {
            return new string[]
            {
                "R 6 (#70c710)",
                "D 5 (#0dc571)",
                "L 2 (#5713f0)",
                "D 2 (#d2c081)",
                "R 2 (#59c680)",
                "D 2 (#411b91)",
                "L 5 (#8ceee2)",
                "U 2 (#caa173)",
                "L 1 (#1b58a2)",
                "U 2 (#caa171)",
                "R 2 (#7807d2)",
                "U 3 (#a77fa3)",
                "L 2 (#015232)",
                "U 2 (#7a21e3)",
            };
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 46394;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 18 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_SampleInput_ProducesCorrectResponse()
        {
            var expected = 952408144115;

            var result = solver.Part2(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_MyInput_ProducesCorrectResponse()
        {
            /*
                01234567891111
                          0123
             0      #####
             1      #   #
             2  #####   #
             3  #       #
             4  #####   ######
             5      #        #
             6      ##########

             */
            long expected = 62;

            var result = this.solver.Part2(new []
            {
                "R 4 (#000040)",
                "D 4 (#000041)",
                "R 5 (#000050)",
                "D 2 (#000021)",
                "L 9 (#000092)",
                "U 2 (#000023)",
                "L 4 (#000042)",
                "U 2 (#000023)",
                "R 4 (#000040)",
                "U 2 (#000023)",
            });

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            var expected = 201398068194715;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 18 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
