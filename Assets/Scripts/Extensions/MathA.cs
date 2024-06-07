using System;
using UnityEngine;

namespace Extensions
{
    public static class MathA
    {
        // ==== LONG ====

        public static long Clamp(long value, long min, long max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static long Max(long l1, long l2)
        {
            return l1 > l2 ? l1 : l2;
        }

        public static long Min(params long[] values)
        {
            var length = values.Length;
            if (length == 0)
                return 0;
            var num = values[0];
            for (var index = 1; index < length; ++index)
                if (values[index] < num)
                    num = values[index];
            return num;
        }

        public static long Round(double d)
        {
            return (long) Math.Round(d);
        }

        public static long Round(float f)
        {
            return (long) Math.Round(f);
        }

        public static long Ceil(double d)
        {
            return (long) decimal.Ceiling(new decimal(d));
        }

        public static long Ceil(float f)
        {
            return (long) decimal.Ceiling(new decimal(f));
        }

        public static long Floor(double d)
        {
            return (long) decimal.Floor(new decimal(d));
        }

        public static long Floor(float f)
        {
            return (long) decimal.Floor(new decimal(f));
        }

        public static long Fibonacci(long n)
        {
            long a = 0;
            long b = 1;

            for (long i = 0; i < n; i++)
            {
                var temp = a;
                a = b;
                b = temp + b;
            }

            return a;
        }

        // ==== INTEGER ====

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static int Max(int i1, int i2)
        {
            return i1 > i2 ? i1 : i2;
        }

        public static int Min(int i1, int i2)
        {
            return i1 < i2 ? i1 : i2;
        }

        public static int Factorial(int n)
        {
            if (n >= 2)
                return n * Factorial(n - 1);
            return 1;
        }

        // ==== BOOL ====

        public static bool Random()
        {
            return UnityEngine.Random.Range(0, 2) > 0;
        }

        // ==== DOUBLE ====

        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static double Min(params double[] values)
        {
            var length = values.Length;
            if (length == 0)
                return 0;
            var num = values[0];
            for (var index = 1; index < length; ++index)
                if (values[index] < num)
                    num = values[index];
            return num;
        }

        public static double Lerp(double a, double b, float t) => a + (b - a) * Mathf.Clamp01(t);

        // ==== VECTOR ====

        public static Vector2 Bezier2(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            var ab = Vector2.Lerp(a, b, t);
            var bc = Vector2.Lerp(b, c, t);
            return Vector2.Lerp(ab, bc, t);
        }

        public static Vector3 Bezier3(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            var ab = Vector3.Lerp(a, b, t);
            var bc = Vector3.Lerp(b, c, t);
            return Vector3.Lerp(ab, bc, t);
        }
    }
}