using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aristarete.Extensions;

namespace Aristarete.Basic
{
    public readonly struct Float4 : IEquatable<Float4>, IFormattable
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float W;

        public Float4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Float4(float value) : this(value, value, value, value)
        {
        }

        public Float4(in Float3 value, float w) : this(value.X, value.Y, value.Z, w)
        {
        }

        public Float4(in Float2 value1, in Float2 value2) : this(value1.X, value1.Y, value2.X, value2.Y)
        {
        }

        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            3 => W,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Float4 Zero = new(0, 0, 0, 0);
        public static readonly Float4 One = new(1, 1, 1, 1);
        public static readonly Float4 UnitX = new(1f, 0f, 0f, 0f);
        public static readonly Float4 UnitY = new(0f, 1f, 0f, 0f);
        public static readonly Float4 UnitZ = new(0f, 0f, 1f, 0f);
        public static readonly Float4 UnitW = new(0f, 0f, 0f, 1f);

        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;

        public static readonly Float4 PositiveInfinity =
            new(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Float4 NegativeInfinity =
            new(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);

        public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 operator +(in Float4 v1, in Float4 v2)
        {
            return new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 operator -(in Float4 v1, in Float4 v2)
        {
            return new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 operator *(in Float4 v1, in Float4 v2)
        {
            return new(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 operator *(in Float4 v, float scalar)
        {
            return new(v.X * scalar, v.Y * scalar, v.Z * scalar, v.W * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 operator *(float scalar, in Float4 v)
        {
            return new(v.X * scalar, v.Y * scalar, v.Z * scalar, v.W * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 operator /(in Float4 v, float scalar)
        {
            var inverse = 1f / scalar;
            return new Float4(v.X * inverse, v.Y * inverse, v.Z * inverse, v.W * inverse);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 operator /(float scalar, in Float4 v)
        {
            return new(scalar / v.X, scalar / v.Y, scalar / v.Z, scalar / v.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 operator -(in Float4 v)
        {
            return new(-v.X, -v.Y, -v.Z, -v.W);
        }

        #endregion

        #region Equality

        public static bool operator ==(in Float4 left, in Float4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in Float4 left, in Float4 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Float4 other)
        {
            var diffX = X - other.X;
            var diffY = Y - other.Y;
            var diffZ = Z - other.Z;
            var diffW = W - other.W;
            var sqrMag = diffX * diffX + diffY * diffY + diffZ * diffZ + diffW * diffW;
            return sqrMag < DoubledEpsilon;
        }

        public static bool Equals(in Float4 left, in Float4 right)
        {
            return left.Equals(right);
        }

        public static bool FloatEquals(in Float4 left, in Float4 right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
        }

        public override bool Equals(object? obj)
        {
            return obj is Float4 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        #endregion

        #region ToString

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}, {W.ToString(format, formatProvider)})";
        }

        public string ToString(string? format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Normalize()
        {
            var length = Length;
            if (length > Epsilon)
            {
                return this / length;
            }

            return Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 Normalize(in Float4 vector)
        {
            return vector.Normalize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(in Float4 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z + W * v.W;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in Float4 a, in Float4 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 Project(in Float4 a, in Float4 b)
        {
            return b * (Dot(a, b) / Dot(b, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 Reflect(in Float4 vector, in Float4 normal)
        {
            return -2 * Dot(vector, normal) * normal + vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 Saturate(in Float4 vector)
        {
            return new(
                MathExtensions.Clamp01(vector.X), 
                MathExtensions.Clamp01(vector.Y),
                MathExtensions.Clamp01(vector.Z), 
                MathExtensions.Clamp01(vector.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Lerp(in Float4 v, float t)
        {
            return new(X + t * (v.X - X), Y + t * (v.Y - Y), Z + t * (v.Z - Z), W + t * (v.W - W));
        }

        public static Float4 Lerp(in Float4 a, in Float4 b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Float4 LerpUnclamped(in Float4 a, in Float4 b, float t)
        {
            return new(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            );
        }

        public static float Distance(in Float4 a, in Float4 b)
        {
            var diffX = a.X - b.X;
            var diffY = a.Y - b.Y;
            var diffZ = a.Z - b.Z;
            var diffW = a.W - b.W;
            return MathF.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ + diffW * diffW);
        }

        public static float DistanceSquared(in Float4 a, in Float4 b)
        {
            var diffX = a.X - b.X;
            var diffY = a.Y - b.Y;
            var diffZ = a.Z - b.Z;
            var diffW = a.W - b.W;
            return diffX * diffX + diffY * diffY + diffZ * diffZ + diffW * diffW;
        }

        public static Float4 SquareRoot(in Float4 value)
        {
            return new(
                MathF.Sqrt(value.X),
                MathF.Sqrt(value.Y),
                MathF.Sqrt(value.Z),
                MathF.Sqrt(value.W)
            );
        }

        public static Float4 Abs(in Float4 value)
        {
            return new(
                MathF.Abs(value.X),
                MathF.Abs(value.Y),
                MathF.Abs(value.Z),
                MathF.Abs(value.W)
            );
        }

        public Vector4 ToBuiltIn()
        {
            return new(X, Y, Z, W);
        }

        public static Float4 FromBuiltIn(Vector4 vector4)
        {
            return new(vector4.X, vector4.Y, vector4.Z, vector4.W);
        }

        public static Float4 Min(in Float4 a, in Float4 b)
        {
            return new(
                a.X < b.X ? a.X : b.X,
                a.Y < b.Y ? a.Y : b.Y,
                a.Z < b.Z ? a.Z : b.Z,
                a.W < b.W ? a.W : b.W
            );
        }

        public static Float4 Max(in Float4 a, in Float4 b)
        {
            return new(
                a.X > b.X ? a.X : b.X,
                a.Y > b.Y ? a.Y : b.Y,
                a.Z > b.Z ? a.Z : b.Z,
                a.W > b.W ? a.W : b.W
            );
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 Clamp(in Float4 value, in Float4 min, in Float4 max)
        {
            // We must follow HLSL behavior in the case user specified min value is bigger than max value.
            return Min(Max(value, min), max);
        }


        /// <summary>Copies the contents of the vector into the given array, starting from index.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(float[] array, int index = 0)
        {
            if (array is null)
            {
                throw new NullReferenceException(nameof(array));
            }

            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (array.Length - index < 4)
            {
                throw new ArgumentException("array.Length - index < 4");
            }

            array[index] = X;
            array[index + 1] = Y;
            array[index + 2] = Z;
            array[index + 3] = W;
        }


        public Float3 XYZ() => new(X, Y, Z);

        public static implicit operator Float4((float x, float y, float z, float w) values) =>
            new(values.x, values.y, values.z, values.w);

        public static implicit operator (float x, float y, float z, float w)(in Float4 v) => (v.X, v.Y, v.Z, v.W);

        public void Deconstruct(out float x, out float y, out float z, out float w) => (x, y, z, w) = (X, Y, Z, W);
    }
}