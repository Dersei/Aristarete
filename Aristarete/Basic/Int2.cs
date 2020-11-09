using System;
using System.Runtime.CompilerServices;

namespace Aristarete.Basic
{
    public readonly struct Int2 : IEquatable<Int2>, IFormattable
    {
        public readonly int X;
        public readonly int Y;

        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static readonly Int2 Zero = new Int2();
        public static readonly Int2 One = new Int2(1, 1);
        public static readonly Int2 Up = new Int2(0, 1);
        public static readonly Int2 Down = new Int2(0, -1);
        public static readonly Int2 Left = new Int2(-1, 0);
        public static readonly Int2 Right = new Int2(1, 0);

        public int this[int index] => index switch
        {
            0 => X,
            1 => Y,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public float Length => MathF.Sqrt(X * X + Y * Y);

        public float LengthSquared => X * X + Y * Y;

        public static float Distance(Int2 a, Int2 b)
        {
            return (a - b).Length;
        }

        public Int2 Clamp(Int2 min, Int2 max)
        {
            var x = Math.Max(min.X, X);
            x = Math.Min(max.X, x);
            var y = Math.Max(min.Y, Y);
            y = Math.Min(max.Y, y);
            return new Int2(x, y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override bool Equals(object? other)
        {
            return other is Int2 int2 && Equals(int2);
        }

        public bool Equals(Int2 other)
        {
            return this == other;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Int2 left, Int2 right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Int2 left, Int2 right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator +(Int2 a, Int2 b)
        {
            return new Int2(a.X + b.X, a.Y + b.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator -(Int2 a, Int2 b)
        {
            return new Int2(a.X - b.X, a.Y - b.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator *(Int2 a, Int2 b)
        {
            return new Int2(a.X * b.X, a.Y * b.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator -(Int2 a)
        {
            return new Int2(-a.X, -a.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator *(Int2 a, int b)
        {
            return new Int2(a.X * b, a.Y * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator *(int a, Int2 b)
        {
            return new Int2(a * b.X, a * b.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 operator /(Int2 a, int b)
        {
            return new Int2(a.X / b, a.Y / b);
        }

        public static Int2 Min(Int2 v1, Int2 v2)
        {
            return new Int2(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y));
        }

        public static Int2 Max(Int2 v1, Int2 v2)
        {
            return new Int2(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y));
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)})";
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}