using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 22
    /// </summary>
    public class Day22
    {
        public int Part1(string[] input)
        {
            var cubes = input.Select(SandCube.Parse).ToArray();

            Dictionary<SandCube, IList<SandCube>> supporting = new();
            Dictionary<SandCube, IList<SandCube>> supportedBy = new();

            for (int i = 0; i < cubes.Length - 1; i++)
            {
                for (int j = i + 1; j < cubes.Length; j++)
                {
                    var a = cubes[i];
                    var b = cubes[j];

                    if (!a.Overlaps(b))
                    {
                        continue;
                    }

                    if (a.TopRightBack.Z < b.BottomLeftFront.Z)
                    {
                        // A supports B
                        supporting.GetOrCreate(a, () => new List<SandCube>()).Add(b);
                        supportedBy.GetOrCreate(b, () => new List<SandCube>()).Add(a);
                    }
                    else
                    {
                        // B supports A
                        supporting.GetOrCreate(b, () => new List<SandCube>()).Add(a);
                        supportedBy.GetOrCreate(a, () => new List<SandCube>()).Add(b);
                    }
                }
            }

            int total = 0;

            foreach (SandCube cube in cubes)
            {
                if (!supporting.TryGetValue(cube, out IList<SandCube> holdingUp))
                {
                    // cube doesn't support anything, safe to remove
                    total++;
                    continue;
                }

                if (holdingUp.All(h => supportedBy.ContainsKey(h) && supportedBy[h].Count > 1))
                {
                    // we're supporting things, but everything we're supporting is itself supported by at least one other thing
                    total++;
                }
            }

            // 1216 -- too high
            // 1210 -- too high

            return total;
        }

        public int Part2(string[] input)
        {
            foreach (string line in input)
            {
                throw new NotImplementedException("Part 2 not implemented");
            }

            return 0;
        }

        private record SandCube
        {
            public Point3D BottomLeftFront { get; init; }
            public Point3D TopRightBack { get; init; }

            public static SandCube Parse(string input)
            {
                int[] numbers = input.Numbers<int>();

                Point3D bottomLeftFront = (Math.Min(numbers[0], numbers[3]),
                                           Math.Min(numbers[1], numbers[4]),
                                           Math.Min(numbers[2], numbers[5]));

                Point3D topRightBack = (Math.Max(numbers[0], numbers[3]),
                                        Math.Max(numbers[1], numbers[4]),
                                        Math.Max(numbers[2], numbers[5]));

                return new SandCube { BottomLeftFront = bottomLeftFront, TopRightBack = topRightBack };
            }

            public bool Overlaps(SandCube other)
            {
                bool overlapX = !(this.BottomLeftFront.X > other.TopRightBack.X
                               || this.TopRightBack.X < other.BottomLeftFront.X);

                bool overlapY = !(this.BottomLeftFront.Y > other.TopRightBack.Y
                               || this.TopRightBack.Y < other.BottomLeftFront.Y);

                return overlapX && overlapY;
            }
        }
    }
}
