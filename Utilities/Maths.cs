using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public static class Maths
    {
        public static double Percent(int number, int percent)
        {
            return (number * percent) / 100D;
        }
        public static decimal Percent(decimal number, decimal percent)
        {
            return (number * percent) / 100m;
        }

        public static decimal PercentOf(decimal numberInQuestion, decimal ofNumber)
        {
            return (numberInQuestion / ofNumber) * 100;
        }

        public static decimal PercentDiff(decimal numberInQuestion, decimal ofNumber)
        {
            var diff = ofNumber - numberInQuestion;
            return (diff / ofNumber) * 100;
        }

        public static double CalculateChange(int previous, int current)
        {
            if (previous == 0)
                throw new InvalidOperationException();

            var change = current - previous;
            return (double)change / previous;
        }
        public static decimal CalculateChange(decimal previous, decimal current)
        {
            if (previous == 0)
                throw new InvalidOperationException();

            var change = current - previous;
            return (decimal)change / previous;
        }
    }
}
