using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aristarete.Extensions;

namespace Aristarete.Basic
{
    public readonly struct Float2 : IEquatable<Float2>, IFormattable
    {
        public readonly float X;
        public readonly float Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2(float x = default, float y = default)
        {
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2(Float2 v)
        {
            X = v.X;
            Y = v.Y;
        }

        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Float2 Zero = new Float2();
        public static readonly Float2 One = new Float2(1, 1);
        public static readonly Float2 Up = new Float2(0, 1);
        public static readonly Float2 Down = new Float2(0, -1);
        public static readonly Float2 Left = new Float2(-1F, 0F);
        public static readonly Float2 Right = new Float2(1F, 0F);
        
        public static readonly Float2 PositiveInfinity =
            new Float2(float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Float2 NegativeInfinity =
            new Float2(float.NegativeInfinity, float.NegativeInfinity);

        public float Length => MathF.Sqrt(X * X + Y * Y);

        public float LengthSquared => X * X + Y * Y;

        #region Operators

        public static Float2 operator +(Float2 v1, Float2 v2)
        {
            return new Float2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Float2 operator -(Float2 v1, Float2 v2)
        {
            return new Float2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Float2 operator *(Float2 v1, Float2 v2)
        {
            return new Float2(v1.X * v2.X, v1.Y * v2.Y);
        }

        public static Float2 operator *(Float2 v, float scalar)
        {
            return new Float2(v.X * scalar, v.Y * scalar);
        }

        public static Float2 operator *(float scalar, Float2 v)
        {
            return new Float2(v.X * scalar, v.Y * scalar);
        }

        public static Float2 operator /(Float2 v, float scalar)
        {
            var inverse = 1f / scalar;
            return new Float2(v.X * inverse, v.Y * inverse);
        }

        public static Float2 operator /(float scalar, Float2 v)
        {
            return new Float2(scalar / v.X, scalar / v.Y);
        }

        public static Float2 operator -(Float2 v)
        {
            return new Float2(-v.X, -v.Y);
        }

        #endregion

        #region Equality

        public static bool operator ==(Float2 left, Float2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Float2 left, Float2 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Float2 other)
        {
            var diffX = X - other.X;
            var diffY = Y - other.Y;
            var sqrMag = diffX * diffX + diffY * diffY;
            return sqrMag < DoubledEpsilon;
        }

        public override bool Equals(object? obj)
        {
            return obj is Float2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
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

        #endregion

        public static Float2 Lerp(Float2 a, Float2 b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Float2 LerpUnclamped(Float2 a, Float2 b, float t)
        {
            return new Float2(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }

        public Float2 Normalize()
        {
            var length = Length;
            if (length > Epsilon)
            {
                return this / length;
            }

            return Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(Float2 v)
        {
            return X * v.X + Y * v.Y;
        }

        public Float2 Reflect(Float2 normal)
        {
            var factor = -2F * Dot(normal);
            return new Float2(factor * normal.X + X, factor * normal.Y + Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Lerp(Float2 v, float t)
        {
            t = MathExtensions.Clamp01(t);
            return new Float2(X + t * (v.X - X), Y + t * (v.Y - Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 LerpUnclamped(Float2 v, float t)
        {
            return new Float2(X + t * (v.X - X), Y + t * (v.Y - Y));
        }

        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;
        private const float EpsilonNormalSqrt = 1e-15f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Float2 v1, Float2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static float Angle(Float2 from, Float2 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            var denominator = MathF.Sqrt(from.LengthSquared * to.LengthSquared);
            if (denominator < EpsilonNormalSqrt)
            {
                return 0f;
            }

            var dot = MathExtensions.Clamp(Dot(from, to) / denominator, -1f, 1f);
            return MathF.Acos(dot) * MathExtensions.Rad2Deg;
        }

        public static float Distance(Float2 v1, Float2 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            return MathF.Sqrt(diffX * diffX + diffY * diffY);
        }


        public Vector2 ToBuiltIn()
        {
            return new Vector2(X, Y);
        }

        public static Float2 FromBuiltIn(Vector2 vector2)
        {
            return new Float2(vector2.X, vector2.Y);
        }

        public static Float2 Transform(Float2 vector3, float scale, Float2 position, Float2 rotationAxis,
            float angle)
        {
            var matrixScale = Matrix3x2.CreateScale(scale);
            var matrixPosition = Matrix3x2.CreateTranslation(position.ToBuiltIn());
            var matrixRotation = Matrix3x2.CreateRotation(angle * MathExtensions.Deg2Rad, rotationAxis.ToBuiltIn());
            var transformMatrix = matrixScale * matrixRotation * matrixPosition;
            return FromBuiltIn(Vector2.Transform(vector3.ToBuiltIn(), transformMatrix));
        }

        public static Float2 MinAbsolute(Float2 v1, Float2 v2)
        {
            return new Float2(MathF.Max(0, MathF.Min(v1.X, v2.X)), MathF.Max(0, MathF.Min(v1.Y, v2.Y)));
        }

        public static Float2 MaxClamped(Float2 v1, Float2 v2, Float2 clamp)
        {
            return new Float2(MathF.Min(clamp.X, MathF.Max(v1.X, v2.X)), MathF.Min(clamp.Y, MathF.Max(v1.Y, v2.Y)));
        }

        public static Float2 Min(Float2 v1, Float2 v2)
        {
            return new Float2(MathF.Min(v1.X, v2.X), MathF.Min(v1.Y, v2.Y));
        }

        public static Float2 Max(Float2 v1, Float2 v2)
        {
            return new Float2(MathF.Max(v1.X, v2.X), MathF.Max(v1.Y, v2.Y));
        }

        public static implicit operator Float2((float x, float y, float z) values) =>
            new Float2(values.x, values.y);

        public static implicit operator (float x, float y)(Float2 v) => (v.X, v.Y);

        public void Deconstruct(out float x, out float y) => (x, y) = (X, Y);
    }
}