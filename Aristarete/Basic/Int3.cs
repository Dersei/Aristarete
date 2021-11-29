using System;
using System.Runtime.CompilerServices;

namespace Aristarete.Basic
{
    public readonly struct Int3 : IEquatable<Int3>, IFormattable
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Int3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static readonly Int3 Zero = new Int3();
        public static readonly Int3 One = new Int3(1, 1, 1);
        public static readonly Int3 Up = new Int3(0, 1, 0);
        public static readonly Int3 Down = new Int3(0, -1, 0);
        public static readonly Int3 Left = new Int3(-1, 0, 0);
        public static readonly Int3 Right = new Int3(1, 0, 0);
        public static readonly Int3 Forward = new Int3(0, 0, 1);
        public static readonly Int3 Back = new Int3(0, 0, -1);
        
        public int this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
        
        public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z);

        public float LengthSquared => X * X + Y * Y + Z * Z;
        
        public static float Distance(Int3 a, Int3 b) { return (a - b).Length; }

        public Int3 Clamp(Int3 min, Int3 max)
        {
            var x = Math.Max(min.X, X);
            x = Math.Min(max.X, x);
            var y = Math.Max(min.Y, Y);
            y = Math.Min(max.Y, y);
            var z = Math.Max(min.Z, Z);
            z = Math.Min(max.Z, z);
            return new Int3(x, y, z);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override bool Equals(object? other)
        {
            return other is Int3 int3 && Equals(int3);
        }

        public bool Equals(Int3 other)
        {
            return this == other;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Int3 left, Int3 right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Int3 left, Int3 right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator +(Int3 a, Int3 b)
        {
            return new Int3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator -(Int3 a, Int3 b)
        {
            return new Int3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator *(Int3 a, Int3 b)
        {
            return new Int3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator -(Int3 a)
        {
            return new Int3(-a.X, -a.Y, -a.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator *(Int3 a, int b)
        {
            return new Int3(a.X * b, a.Y * b, a.Z * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator *(int a, Int3 b)
        {
            return new Int3(a * b.X, a * b.Y, a * b.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int3 operator /(Int3 a, int b)
        {
            return new Int3(a.X / b, a.Y / b, a.Z / b);
        }
        
        public static Int3 Min(Int3 v1, Int3 v2)
        {
            return new Int3(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y), Math.Min(v1.Z, v2.Z));
        }

        public static Int3 Max(Int3 v1, Int3 v2)
        {
            return new Int3(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y), Math.Max(v1.Z, v2.Z));
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)})";
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}