namespace Assets.Scripts.Common.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx">from left 0..1</param>
        /// <param name="dy">from top 0..1</param>
        /// <param name="rect">TL/TR / BL/BR</param>
        /// <returns></returns>
        public static float Lerp2D(float dx, float dy, params float[] rect)
        {
            var m1 = rect[0] * (1 - dx) + rect[1] * dx;
            var m2 = rect[2] * (1 - dx) + rect[3] * dx;
            return m1 * (1 - dy) + m2 * dy;
        }
        public static int MOD(this int a, int n)
        {
            int result = a % n;
            if (result < 0 && n > 0 || result > 0 && n < 0)
            {
                result += n;
            }
            return result;
        }
    }
}