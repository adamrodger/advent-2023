using System;
using System.Linq;
using AdventOfCode.Utilities;
using Microsoft.Z3;
using Optional;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 24
    /// </summary>
    public class Day24
    {
        public int Part1(string[] input)
        {
            var hailstones = input.Select(Hailstone.Parse).ToArray();

            int total = 0;

            // pairwise intersect all hailstones
            foreach ((int i, Hailstone left) in hailstones.Enumerate())
            {
                foreach (Hailstone right in hailstones.Skip(i + 1))
                {
                    Option<(double TimeA, double TimeB, double InterceptX, double InterceptY)> intersection = left.Intersect2d(right);

                    intersection.Filter(x => x is
                                 {
                                     // make sure the intersection happens in the future and within the bounded box we're checking
                                     TimeA: >= 0,
                                     TimeB: >= 0,
                                     InterceptX: >= 200_000_000_000_000, InterceptX: <= 400_000_000_000_000,
                                     InterceptY: >= 200_000_000_000_000, InterceptY: <= 400_000_000_000_000
                                 })
                                .Map(_ => total++);
                }
            }

            return total;
        }

        public long Part2(string[] input)
        {
            var hailstones = input.Select(Hailstone.Parse).Take(3).ToArray();

            // I have no idea how this works and stole the following pretty much verbatim from:
            //      https://www.reddit.com/r/adventofcode/comments/18pnycy/comment/keq0grp/?utm_source=share&utm_medium=web2x&context=3
            //      https://pastebin.com/fkpZWn8X

            var ctx = new Context();
            var solver = ctx.MkSolver();

            // Coordinates of the stone
            var x = ctx.MkIntConst("x");
            var y = ctx.MkIntConst("y");
            var z = ctx.MkIntConst("z");

            // Velocity of the stone
            var vx = ctx.MkIntConst("vx");
            var vy = ctx.MkIntConst("vy");
            var vz = ctx.MkIntConst("vz");

            // For each iteration, we will add 3 new equations and one new condition to the solver.
            // We want to find 9 variables (x, y, z, vx, vy, vz, t0, t1, t2) that satisfy all the equations, so a system of 9 equations is enough.
            foreach ((int i, Hailstone stone) in hailstones.Enumerate())
            {
                var t = ctx.MkIntConst($"t{i}"); // time for the stone to reach the hail

                var px = ctx.MkInt(stone.Position.X);
                var py = ctx.MkInt(stone.Position.Y);
                var pz = ctx.MkInt(stone.Position.Z);

                var pvx = ctx.MkInt(stone.Velocity.X);
                var pvy = ctx.MkInt(stone.Velocity.Y);
                var pvz = ctx.MkInt(stone.Velocity.Z);

                var xLeft = ctx.MkAdd(x, ctx.MkMul(t, vx)); // x + t * vx
                var yLeft = ctx.MkAdd(y, ctx.MkMul(t, vy)); // y + t * vy
                var zLeft = ctx.MkAdd(z, ctx.MkMul(t, vz)); // z + t * vz

                var xRight = ctx.MkAdd(px, ctx.MkMul(t, pvx)); // px + t * pvx
                var yRight = ctx.MkAdd(py, ctx.MkMul(t, pvy)); // py + t * pvy
                var zRight = ctx.MkAdd(pz, ctx.MkMul(t, pvz)); // pz + t * pvz

                solver.Add(t >= 0); // time should always be positive - we don't want solutions for negative time
                solver.Add(ctx.MkEq(xLeft, xRight)); // x + t * vx = px + t * pvx
                solver.Add(ctx.MkEq(yLeft, yRight)); // y + t * vy = py + t * pvy
                solver.Add(ctx.MkEq(zLeft, zRight)); // z + t * vz = pz + t * pvz
            }

            solver.Check();
            Model model = solver.Model;

            Expr rx = model.Eval(x);
            Expr ry = model.Eval(y);
            Expr rz = model.Eval(z);

            return long.Parse(rx.ToString()) + long.Parse(ry.ToString()) + long.Parse(rz.ToString());
        }

        /// <summary>
        /// A hailstone at a position traveling on a certain vector
        /// </summary>
        /// <param name="Position">Current position at time 0</param>
        /// <param name="Velocity">Velocity vector</param>
        private record Hailstone((long X, long Y, long Z) Position, Point3D Velocity)
        {
            /// <summary>
            /// Parse the input
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Hailstone</returns>
            public static Hailstone Parse(string input)
            {
                var numbers = input.Numbers<long>();

                return new Hailstone((numbers[0], numbers[1], numbers[2]),
                                     ((int)numbers[3], (int)numbers[4], (int)numbers[5]));
            }

            /// <summary>
            /// Check if this hailstone intersects another hailstone
            /// </summary>
            /// <param name="other">Other hailstone</param>
            /// <returns>Intersection times and coordinates, otherwise none if they would never intersect</returns>
            public Option<(double TimeA, double TimeB, double InterceptX, double InterceptY)> Intersect2d(Hailstone other)
            {
                // https://www.youtube.com/watch?v=6ZXav6DSmn8
                // need to solve s and t at the same time so need simultaneous equations
                // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection

                // slopes
                double ma = (double)this.Velocity.Y / this.Velocity.X;
                double mb = (double)other.Velocity.Y / other.Velocity.X;
                
                if (Math.Abs(ma - mb) < 0.000001)
                {
                    // won't intersect
                    return Option.None<(double, double, double, double)>();
                }

                double ba = this.Position.Y - ma * this.Position.X;
                double bb = other.Position.Y - mb * other.Position.X;

                // intersection location
                double ix = (bb - ba) / (ma - mb);
                double iy = ma * ix + ba;

                // intersection times
                double t = (ix - this.Position.X) / this.Velocity.X;
                double s = (ix - other.Position.X) / other.Velocity.X;

                return (t, s, ix, iy).Some();
            }
        }
    }
}
