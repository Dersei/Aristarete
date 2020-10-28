using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aristarete.Extensions;

namespace Aristarete.Basic
{
    public readonly struct Float4
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float W;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4(Float3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4(Float3 v, float w)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4(Float4 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = v.W;
        }

        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            3 => W,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Float4 Zero = new Float4();
        public static readonly Float4 One = new Float4(1, 1, 1, 1);
        public static readonly Float4 Up = new Float4(0f, 1f, 0, 0);
        public static readonly Float4 Down = new Float4(0f, -1f, 0, 0);
        public static readonly Float4 Left = new Float4(-1f, 0, 0, 0);
        public static readonly Float4 Right = new Float4(1f, 0, 0, 0);
        public static readonly Float4 Forward = new Float4(0f, 0f, 1f, 0);
        public static readonly Float4 Back = new Float4(0f, 0f, -1f, 0);

        public static readonly Float4 PositiveInfinity =
            new Float4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Float4 NegativeInfinity =
            new Float4(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);

        public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

        #region Operators

        public static Float4 operator +(Float4 v1, Float4 v2)
        {
            return new Float4(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
        }

        public static Float4 operator -(Float4 v1, Float4 v2)
        {
            return new Float4(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
        }

        public static Float4 operator *(Float4 v1, Float4 v2)
        {
            return new Float4(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
        }

        public static Float4 operator *(Float4 v, float scalar)
        {
            return new Float4(v.X * scalar, v.Y * scalar, v.Z * scalar, v.W * scalar);
        }

        public static Float4 operator *(float scalar, Float4 v)
        {
            return new Float4(v.X * scalar, v.Y * scalar, v.Z * scalar, v.W * scalar);
        }

        public static Float4 operator /(Float4 v, float scalar)
        {
            var inverse = 1f / scalar;
            return new Float4(v.X * inverse, v.Y * inverse, v.Z * inverse, v.W * inverse);
        }

        public static Float4 operator /(float scalar, Float4 v)
        {
            return new Float4(scalar / v.X, scalar / v.Y, scalar / v.Z, scalar / v.W);
        }

        public static Float4 operator -(Float4 v)
        {
            return new Float4(-v.X, -v.Y, -v.Z, -v.W);
        }

        #endregion

        #region Equality

        public static bool operator ==(Float4 left, Float4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Float4 left, Float4 right)
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

        public override bool Equals(object? obj)
        {
            return obj is Float4 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}, {W.ToString(format, formatProvider)})";
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }

        #endregion

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
        public float Dot(Float4 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z + W * v.W;
        }


        public static Float4 Project(Float4 a, Float4 b)
        {
            return b * (Dot(a, b) / Dot(b, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Lerp(Float4 v, float t)
        {
            t = MathExtensions.Clamp01(t);
            return new Float4(X + t * (v.X - X), Y + t * (v.Y - Y), Z + t * (v.Z - Z), W + t * (v.W - W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 LerpUnclamped(Float4 v, float t)
        {
            return new Float4(X + t * (v.X - X), Y + t * (v.Y - Y), Z + t * (v.Z - Z), W + t * (v.W - W));
        }

        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;
        private const float EpsilonNormalSqrt = 1e-15f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Float4 v1, Float4 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
        }


        public static float Distance(Float4 v1, Float4 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            var diffZ = v1.Z - v2.Z;
            var diffW = v1.W - v2.W;
            return MathF.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ + diffW * diffW);
        }

        public static Float4 Lerp(Float4 a, Float4 b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Float4 LerpUnclamped(Float4 a, Float4 b, float t)
        {
            return new Float4(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            );
        }


        public Vector4 ToBuiltIn()
        {
            return new Vector4(X, Y, Z, W);
        }

        public static Float4 FromBuiltIn(Vector4 vector4)
        {
            return new Float4(vector4.X, vector4.Y, vector4.Z, vector4.W);
        }

        public static implicit operator Float4((float x, float y, float z, float w) values) =>
            new Float4(values.x, values.y, values.z, values.w);

        public static implicit operator (float x, float y, float z, float w)(Float4 v) => (v.X, v.Y, v.Z, v.W);

        public void Deconstruct(out float x, out float y, out float z, out float w) => (x, y, z, w) = (X, Y, Z, W);

        public Float2 XY => new Float2(X, Y);

        public Float3 XYZ => new Float3(X, Y, Z);
    }
}