namespace AdventOfCode.Utilities
{
    /// <summary>
    /// Maths utilities
    /// </summary>
    internal static class Maths
    {
        /// <summary>
        /// http://eddiejackson.net/wp/?page_id=19876
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Lowest common multiple of the two numbers</returns>
        public static int LowestCommonMultiple(this int a, int b)
        {
            int num1 = a;
            int num2 = b;

            while (num1 != num2)
            {
                if (num1 > num2)
                {
                    num1 -= num2;
                }
                else
                {
                    num2 -= num1;
                }
            }

            return (a * b) / num1;
        }

        /// <summary>
        /// http://eddiejackson.net/wp/?page_id=19876
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Lowest common multiple of the two numbers</returns>
        public static long LowestCommonMultiple(this long a, long b)
        {
            long num1 = a;
            long num2 = b;

            while (num1 != num2)
            {
                if (num1 > num2)
                {
                    num1 -= num2;
                }
                else
                {
                    num2 -= num1;
                }
            }

            return (a * b) / num1;
        }
    }
}
