using System.IO;
using Xunit;
using Xunit.Abstractions;


namespace AdventOfCode.Tests
{
    public class Day19Tests
    {
        private readonly ITestOutputHelper output;
        private readonly Day19 solver;

        public Day19Tests(ITestOutputHelper output)
        {
            this.output = output;
            this.solver = new Day19();
        }

        private static string[] GetRealInput()
        {
            string[] input = File.ReadAllLines("inputs/day19.txt");
            return input;
        }

        private static string[] GetSampleInput()
        {
            return new string[]
            {
                "px{a<2006:qkq,m>2090:A,rfg}",
                "pv{a>1716:R,A}",
                "lnx{m>1548:A,A}",
                "rfg{s<537:gd,x>2440:R,A}",
                "qs{s>3448:A,lnx}",
                "qkq{x<1416:A,crn}",
                "crn{x>2662:A,R}",
                "in{s<1351:px,qqz}",
                "qqz{s>2770:qs,m<1801:hdj,R}",
                "gd{a>3333:R,R}",
                "hdj{m>838:A,pv}",
                "",
                "{x=787,m=2655,a=1222,s=2876}",
                "{x=1679,m=44,a=2067,s=496}",
                "{x=2036,m=264,a=79,s=2244}",
                "{x=2461,m=1339,a=466,s=291}",
                "{x=2127,m=1623,a=2188,s=1013}",
            };
        }

        [Fact]
        public void Part1_SampleInput_ProducesCorrectResponse()
        {
            var expected = 19114;

            var result = solver.Part1(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part1_RealInput_ProducesCorrectResponse()
        {
            var expected = 263678;

            var result = solver.Part1(GetRealInput());
            output.WriteLine($"Day 19 - Part 1 - {result}");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_SampleInput_ProducesCorrectResponse()
        {
            var expected = 167_409_079_868_000;

            var result = solver.Part2(GetSampleInput());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Part2_RealInput_ProducesCorrectResponse()
        {
            var expected = 125_455_345_557_345;

            var result = solver.Part2(GetRealInput());
            output.WriteLine($"Day 19 - Part 2 - {result}");

            Assert.Equal(expected, result);
        }
    }
}
