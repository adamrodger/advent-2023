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
            var cubes = input.Select((line, i) => SandCube.Parse(i, line)).ToArray();

            Dictionary<int, ISet<int>> supporting = new();
            Dictionary<int, ISet<int>> supportedBy = new();
            Dictionary<Point3D, int> occupiedSpace = new();

            foreach (SandCube cube in cubes.OrderBy(c => c.BottomLeftFront.Z))
            {
                SandCube current = cube;
                SandCube dropped = cube.Drop();

                // drop until we hit either the ground or a point occupied by another cube
                while (dropped.Points().All(p => p.Z > 0 && !occupiedSpace.ContainsKey(p)))
                {
                    current = dropped;
                    dropped = current.Drop();
                }

                // mark the cubes below as supporting this one
                foreach (Point3D point in dropped.BottomLayer().Where(occupiedSpace.ContainsKey))
                {
                    int supportingId = occupiedSpace[point];
                    supporting.GetOrCreate(supportingId, () => new HashSet<int>()).Add(current.Id);
                    supportedBy.GetOrCreate(current.Id, () => new HashSet<int>()).Add(supportingId);
                }

                // settle this cube in place
                foreach (Point3D point in current.Points())
                {
                    occupiedSpace[point] = current.Id;
                }
            }

            int total = 0;

            foreach (SandCube cube in cubes)
            {
                if (!supporting.TryGetValue(cube.Id, out ISet<int> holdingUp))
                {
                    // cube doesn't support anything, safe to remove
                    total++;
                    continue;
                }

                if (holdingUp.All(h => supportedBy[h].Count > 1))
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

        private record SandCube(int Id, Point3D BottomLeftFront, Point3D TopRightBack)
        {
            public static SandCube Parse(int id, string input)
            {
                int[] numbers = input.Numbers<int>();

                Point3D bottomLeftFront = (Math.Min(numbers[0], numbers[3]),
                                           Math.Min(numbers[1], numbers[4]),
                                           Math.Min(numbers[2], numbers[5]));

                Point3D topRightBack = (Math.Max(numbers[0], numbers[3]),
                                        Math.Max(numbers[1], numbers[4]),
                                        Math.Max(numbers[2], numbers[5]));

                return new SandCube(id, bottomLeftFront, topRightBack);
            }

            public bool Overlaps(SandCube other)
            {
                bool overlapX = !(this.BottomLeftFront.X > other.TopRightBack.X
                               || this.TopRightBack.X < other.BottomLeftFront.X);

                bool overlapY = !(this.BottomLeftFront.Y > other.TopRightBack.Y
                               || this.TopRightBack.Y < other.BottomLeftFront.Y);

                return overlapX && overlapY;
            }

            public SandCube Drop()
            {
                return this with
                {
                    BottomLeftFront = this.BottomLeftFront - (0, 0, 1),
                    TopRightBack = this.TopRightBack - (0, 0, 1)
                };
            }

            public IEnumerable<Point3D> Points()
            {
                for (int x = this.BottomLeftFront.X; x <= this.TopRightBack.X; x++)
                {
                    for (int y = this.BottomLeftFront.Y; y <= this.TopRightBack.Y; y++)
                    {
                        for (int z = this.BottomLeftFront.Z; z <= this.TopRightBack.Z; z++)
                        {
                            yield return (x, y, z);
                        }
                    }
                }
            }

            public IEnumerable<Point3D> BottomLayer()
            {
                for (int x = this.BottomLeftFront.X; x <= this.TopRightBack.X; x++)
                {
                    for (int y = this.BottomLeftFront.Y; y <= this.TopRightBack.Y; y++)
                    {
                        yield return (x, y, this.BottomLeftFront.Z);
                    }
                }
            }
        }
    }
}
