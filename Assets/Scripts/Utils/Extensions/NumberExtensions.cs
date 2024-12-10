using System;

namespace Utils.Extensions
{
    public static class NumberExtensions
    {
        public static decimal TruncateDecimal(this decimal value, int precision)
        {
            var step = (decimal) Math.Pow(10, precision);
            var tmp = Math.Truncate(step * value);
            return tmp / step;
        }
    }
}