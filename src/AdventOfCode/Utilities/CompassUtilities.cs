using System;

namespace AdventOfCode.Utilities
{
    /// <summary>
    /// Compass bearing
    /// </summary>
    public enum Bearing { North, South, East, West };

    /// <summary>
    /// Turn direction
    /// </summary>
    public enum TurnDirection { Left = 0, Right = 1 };

    /// <summary>
    /// Extensions methods to do with moving around a grid
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Turn in the given direction
        /// </summary>
        /// <param name="bearing">Current bearing</param>
        /// <param name="turn">Turn direction</param>
        /// <returns>New bearing</returns>
        public static Bearing Turn(this Bearing bearing, TurnDirection turn)
        {
            return bearing switch
            {
                Bearing.North => turn == TurnDirection.Left ? Bearing.West : Bearing.East,
                Bearing.South => turn == TurnDirection.Left ? Bearing.East : Bearing.West,
                Bearing.East => turn == TurnDirection.Left ? Bearing.North : Bearing.South,
                Bearing.West => turn == TurnDirection.Left ? Bearing.South : Bearing.North,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        /// <summary>
        /// Move from one position to another in the given direction and number of steps
        /// </summary>
        /// <param name="position">Starting position</param>
        /// <param name="bearing">Move direction</param>
        /// <param name="steps">Move steps</param>
        /// <returns>New position</returns>
        public static (int x, int y) Move(this (int x, int y) position, Bearing bearing, int steps = 1)
        {
            return bearing switch
            {
                Bearing.North => (position.x, position.y + steps),
                Bearing.South => (position.x, position.y - steps),
                Bearing.East => (position.x + steps, position.y),
                Bearing.West => (position.x - steps, position.y),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
