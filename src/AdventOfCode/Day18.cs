using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 18
    /// </summary>
    public class Day18
    {
        public int Part1(string[] input)
        {
            HashSet<Point2D> points = new();
            Point2D current = (0, 0);

            points.Add(current);

            foreach (string line in input)
            {
                Bearing direction = line[0] switch
                {
                    'U' => Bearing.North,
                    'D' => Bearing.South,
                    'R' => Bearing.East,
                    'L' => Bearing.West,
                    _ => throw new ArgumentOutOfRangeException()
                };

                int steps = line.Numbers<int>().First();

                foreach (Point2D next in Dig(current, direction, steps))
                {
                    points.Add(next);
                    current = next;
                }
            }

            int minX = points.MinBy(p => p.X).X;
            int maxX = points.MaxBy(p => p.X).X;
            int minY = points.MinBy(p => p.Y).Y;
            int maxY = points.MaxBy(p => p.Y).Y;

            StringBuilder s = new();

            for (int y = minY; y <= maxY; y++)
            {
                s.Append($"{y:D4}: ");

                for (int x = minX; x <= maxX; x++)
                {
                    s.Append(points.Contains((x, y)) ? '#' : '.');
                }

                s.AppendLine();
            }

            Debug.WriteLine(s.ToString());

            // from visual inspection, flood fill will work instead of the parity checking style from a previous day

            Point2D point = (Enumerable.Range(minX, maxX - minX).First(x => points.Contains((x, 0)) && !points.Contains((x + 1, 0))) + 1, 0);

            Queue<Point2D> queue = new();
            queue.Enqueue(point);

            while (queue.Count > 0)
            {
                point = queue.Dequeue();

                if (points.Contains(point))
                {
                    continue;
                }

                points.Add(point);

                foreach (Point2D next in point.Adjacent4().Where(n => !points.Contains(n)))
                {
                    queue.Enqueue(next);
                }
            }

            return points.Count;
        }

        public long Part2(string[] input)
        {
            long minX = long.MaxValue;
            long maxX = long.MinValue;
            long minY = long.MaxValue;
            long maxY = long.MinValue;

            long x = 0;
            long y = 0;

            SortedSet<HorizontalRange> horizontals = new();
            SortedSet<VerticalRange> verticals = new();

            foreach (string line in input)
            {
                Bearing direction = line[^2] switch
                {
                    '3' => Bearing.North,
                    '1' => Bearing.South,
                    '0' => Bearing.East,
                    '2' => Bearing.West,
                    _ => throw new ArgumentOutOfRangeException()
                };

                int steps = int.Parse(line[^7..^2], NumberStyles.HexNumber);

                switch (direction)
                {
                    case Bearing.North:
                        verticals.Add(new VerticalRange(x, y - steps, y));
                        y -= steps;
                        break;
                    case Bearing.South:
                        verticals.Add(new VerticalRange(x, y, y + steps));
                        y += steps;
                        break;
                    case Bearing.East:
                        horizontals.Add(new HorizontalRange(y, x, x + steps));
                        x += steps;
                        break;
                    case Bearing.West:
                        horizontals.Add(new HorizontalRange(y, x - steps, x));
                        x -= steps;
                        break;
                }

                minX = Math.Min(x, minX);
                maxX = Math.Max(x, maxX);
                minY = Math.Min(y, minY);
                maxY = Math.Max(y, maxY);
            }

            long total = 0;

            for (y = minY; y < maxY; y++)
            {
                var verticalIntersections = verticals.Where(v => v.Start <= y && v.End >= y).OrderBy(range => range.X).Pairs();

                foreach ((VerticalRange First, VerticalRange Second) pair in verticalIntersections)
                {
                    total += pair.Second.X - pair.First.X + 1;
                }
            }

            // 201397207615723 -- too low

            return total;
        }

        private static IEnumerable<Point2D> Dig(Point2D start, Bearing direction, int steps)
        {
            Point2D current = start;

            for (int i = 0; i < steps; i++)
            {
                current = current.Move(direction);
                yield return current;
            }
        }

        private record VerticalRange(long X, long Start, long End) : IComparable<VerticalRange>
        {
            /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
            /// <param name="other">An object to compare with this instance.</param>
            /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
            /// <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description></item><item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description></item></list></returns>
            public int CompareTo(VerticalRange other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }

                if (ReferenceEquals(null, other))
                {
                    return 1;
                }

                int xComparison = this.X.CompareTo(other.X);
                if (xComparison != 0)
                {
                    return xComparison;
                }

                int startComparison = this.Start.CompareTo(other.Start);
                if (startComparison != 0)
                {
                    return startComparison;
                }

                return this.End.CompareTo(other.End);
            }
        }

        private record HorizontalRange(long Y, long Start, long End) : IComparable<HorizontalRange>
        {
            /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
            /// <param name="other">An object to compare with this instance.</param>
            /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
            /// <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description></item><item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description></item></list></returns>
            public int CompareTo(HorizontalRange other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }

                if (ReferenceEquals(null, other))
                {
                    return 1;
                }

                int yComparison = this.Y.CompareTo(other.Y);
                if (yComparison != 0)
                {
                    return yComparison;
                }

                int startComparison = this.Start.CompareTo(other.Start);
                if (startComparison != 0)
                {
                    return startComparison;
                }

                return this.End.CompareTo(other.End);
            }
        }
    }

    public static class PairExtensions
    {
        public static IEnumerable<(T First, T Second)> Pairs<T>(this IEnumerable<T> items)
        {
            T first = default;

            bool yielded = true;

            foreach (T item in items)
            {
                if (yielded)
                {
                    first = item;
                    yielded = false;
                    continue;
                }

                yield return (first, item);
                yielded = true;
            }
        }
    }
}
