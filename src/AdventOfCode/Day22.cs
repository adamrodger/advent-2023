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
            var tower = SandCubeTower.Build(cubes);

            int total = 0;

            foreach (SandCube cube in cubes)
            {
                if (!tower.Supporting.TryGetValue(cube.Id, out ISet<int> holdingUp))
                {
                    // cube doesn't support anything, safe to remove
                    total++;
                    continue;
                }

                if (holdingUp.All(h => tower.SupportedBy[h].Count > 1))
                {
                    // we're supporting things, but everything we're supporting is itself supported by at least one other thing
                    total++;
                }
            }

            return total;
        }

        public int Part2(string[] input)
        {
            var cubes = input.Select((line, i) => SandCube.Parse(i, line)).ToArray();
            var tower = SandCubeTower.Build(cubes);

            int total = 0;

            foreach (SandCube cube in cubes)
            {
                Queue<int> queue = new();
                HashSet<int> fell = new() { cube.Id };

                queue.Enqueue(cube.Id);

                while (queue.Count > 0)
                {
                    int id = queue.Dequeue();

                    if (!tower.Supporting.TryGetValue(id, out ISet<int> holdingUp))
                    {
                        // not holding anything up, so nothing to fall
                        continue;
                    }

                    foreach (int heldUp in holdingUp)
                    {
                        if (tower.SupportedBy[heldUp].All(fell.Contains))
                        {
                            // everything supporting this one fell down
                            queue.Enqueue(heldUp);
                            fell.Add(heldUp);
                        }
                    }
                }

                total += fell.Count - 1; // don't count the block that we removed
            }

            return total;
        }

        /// <summary>
        /// Tower of sand cubes
        /// </summary>
        /// <param name="Supporting">Lookup of each sand cube to which other sand cubes it is supporting</param>
        /// <param name="SupportedBy">Lookup of each sand cube to which other sand cubes it is supported by</param>
        private record SandCubeTower(IDictionary<int, ISet<int>> Supporting, IDictionary<int, ISet<int>> SupportedBy)
        {
            /// <summary>
            /// Building the tower from the given starting cube positions representing falling cubes
            /// </summary>
            /// <param name="falling">Falling cubes</param>
            /// <returns>Sand cube tower after all cubes have fallen and settled</returns>
            public static SandCubeTower Build(IEnumerable<SandCube> falling)
            {
                Dictionary<int, ISet<int>> supporting = new();
                Dictionary<int, ISet<int>> supportedBy = new();
                Dictionary<Point3D, int> occupiedSpace = new();

                foreach (SandCube cube in falling.OrderBy(c => c.BottomLeftFront.Z))
                {
                    SandCube current = cube;
                    SandCube dropped = cube.Drop();

                    // drop until we hit either the ground or a point occupied by another cube
                    while (dropped.BottomLayer().All(p => p.Z > 0 && !occupiedSpace.ContainsKey(p)))
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

                return new SandCubeTower(supporting, supportedBy);
            }
        }

        /// <summary>
        /// A cube of sand
        /// </summary>
        /// <param name="Id">Cube ID</param>
        /// <param name="BottomLeftFront">The bottom front left corner</param>
        /// <param name="TopRightBack">The top right back corner</param>
        private record SandCube(int Id, Point3D BottomLeftFront, Point3D TopRightBack)
        {
            /// <summary>
            /// Parse a sand cube from input
            /// </summary>
            /// <param name="id">Cube ID</param>
            /// <param name="input">Input line</param>
            /// <returns>Sand cube</returns>
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

            /// <summary>
            /// Drop the current cube by one place
            /// </summary>
            /// <returns>Same cube but space one lower down</returns>
            public SandCube Drop()
            {
                return this with
                {
                    BottomLeftFront = this.BottomLeftFront - (0, 0, 1),
                    TopRightBack = this.TopRightBack - (0, 0, 1)
                };
            }

            /// <summary>
            /// Enumerate all the points of space taken up by this cube
            /// </summary>
            /// <returns>Cube points</returns>
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

            /// <summary>
            /// Enumerate all the points of space taken up by the bottom layer of this cube
            /// </summary>
            /// <returns>Bottom layer</returns>
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
