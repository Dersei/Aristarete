using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aristarete.Extensions;

namespace Aristarete.Basic
{
    public readonly struct Float2 : IEquatable<Float2>, IFormattable
    {
        public readonly float X;
        public readonly float Y;

        public Float2(float x = default, float y = default)
        {
            X = x;
            Y = y;
        }
        
        public Float2(float value) : this(value, value)
        {
        }

        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Float2 Zero = new(0, 0);
        public static readonly Float2 One = new(1, 1);
        public static readonly Float2 UnitX = new(1f, 0f);
        public static readonly Float2 UnitY = new(0f, 1f);
        public static readonly Float2 Up = new(0, 1);
        public static readonly Float2 Down = new(0, -1);
        public static readonly Float2 Left = new(-1F, 0F);
        public static readonly Float2 Right = new(1F, 0F);

        public static readonly Float2 PositiveInfinity =
            new(float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Float2 NegativeInfinity =
            new(float.NegativeInfinity, float.NegativeInfinity);
        
                
        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;
        private const float EpsilonNormalSqrt = 1e-15f;

        public float Length => MathF.Sqrt(X * X + Y * Y);

        public float LengthSquared => X * X + Y * Y;

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator +(in Float2 v1, in Float2 v2)
        {
            return new(v1.X + v2.X, v1.Y + v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator -(in Float2 v1, in Float2 v2)
        {
            return new(v1.X - v2.X, v1.Y - v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator *(in Float2 v1, in Float2 v2)
        {
            return new(v1.X * v2.X, v1.Y * v2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator *(in Float2 v, float scalar)
        {
            return new(v.X * scalar, v.Y * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator *(float scalar, in Float2 v)
        {
            return new(v.X * scalar, v.Y * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator /(in Float2 v, float scalar)
        {
            var inverse = 1f / scalar;
            return new Float2(v.X * inverse, v.Y * inverse);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator /(float scalar, in Float2 v)
        {
            return new(scalar / v.X, scalar / v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator -(in Float2 v)
        {
            return new(-v.X, -v.Y);
        }

        #endregion

        #region Equality

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Float2 left, in Float2 right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Float2 left, in Float2 right)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(in Float2 left, in Float2 right)
        {
            return left == right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FloatEquals(in Float2 left, in Float2 right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Float2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        #endregion

        #region ToString

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)})";
        }

        public string ToString(string? format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        #endregion


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
        public Float2 NormalizeUnsafe() => this / Length;
        
        public static Float2 Normalize(in Float2 vector)
        {
            return vector.Normalize();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(in Float2 v)
        {
            return X * v.X + Y * v.Y;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in Float2 v1, in Float2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Reflect(in Float2 normal)
        {
            return -2 * Dot(this, normal) * normal + this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 Reflect(in Float2 vector, in Float2 normal)
        {
            return -2 * Dot(vector, normal) * normal + vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Lerp(in Float2 v, float t)
        {
            t = MathExtensions.Clamp01(t);
            return new Float2(X + t * (v.X - X), Y + t * (v.Y - Y));
        }
        
        
        public static Float2 Lerp(in Float2 a, in Float2 b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Float2 LerpUnclamped(in Float2 a, in Float2 b, float t)
        {
            return new(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }

        public static float Angle(in Float2 from, in Float2 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            var denominator = MathF.Sqrt(@from.LengthSquared * to.LengthSquared);
            if (denominator < EpsilonNormalSqrt)
            {
                return 0f;
            }

            var dot = MathExtensions.Clamp(Dot(from, to) / denominator, -1f, 1f);
            return MathF.Acos(dot) * MathExtensions.Rad2Deg;
        }

        public static float SignedAngle(in Float2 from, in Float2 to)
        {
            var unsignedAngle = Angle(from, to);
            var sign = MathF.Sign(from.X * to.Y - from.Y * to.X);
            return unsignedAngle * sign;
        }

        public static float Distance(in Float2 v1, in Float2 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            return MathF.Sqrt(diffX * diffX + diffY * diffY);
        }
        
        public static float DistanceSquared(in Float2 v1, in Float2 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            return diffX * diffX + diffY * diffY;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 SquareRoot(in Float2 value)
        {
            return new(
                MathF.Sqrt(value.X),
                MathF.Sqrt(value.Y)
            );
        }
        
        public static Float2 Abs(in Float2 value)
        {
            return new(
                MathF.Abs(value.X),
                MathF.Abs(value.Y)
            );
        }

        public Vector2 ToBuiltIn()
        {
            return new(X, Y);
        }

        public static Float2 FromBuiltIn(Vector2 vector2)
        {
            return new(vector2.X, vector2.Y);
        }

        public static Float2 Transform(in Float2 vector3, float scale, in Float2 position, in Float2 rotationAxis, float angle)
        {
            var matrixScale = Matrix3x2.CreateScale(scale);
            var matrixPosition = Matrix3x2.CreateTranslation(position.ToBuiltIn());
            var matrixRotation = Matrix3x2.CreateRotation(angle * MathExtensions.Deg2Rad, rotationAxis.ToBuiltIn());
            var transformMatrix = matrixScale * matrixRotation * matrixPosition;
            return FromBuiltIn(Vector2.Transform(vector3.ToBuiltIn(), transformMatrix));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 Min(in Float2 value1, in Float2 value2)
        {
            return new(
                value1.X < value2.X ? value1.X : value2.X,
                value1.Y < value2.Y ? value1.Y : value2.Y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 Max(in Float2 value1, in Float2 value2)
        {
            return new(
                value1.X > value2.X ? value1.X : value2.X,
                value1.Y > value2.Y ? value1.Y : value2.Y
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 MinAbsolute(Float2 v1, Float2 v2)
        {
            return new(MathF.Max(0, MathF.Min(v1.X, v2.X)), MathF.Max(0, MathF.Min(v1.Y, v2.Y)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 MaxClamped(Float2 v1, Float2 v2, Float2 clamp)
        {
            return new(MathF.Min(clamp.X, MathF.Max(v1.X, v2.X)), MathF.Min(clamp.Y, MathF.Max(v1.Y, v2.Y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 Clamp(in Float2 value1, in Float2 min, in Float2 max)
        {
            // We must follow HLSL behavior in the case user specified min value is bigger than max value.
            return Min(Max(value1, min), max);
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

            if (array.Length - index < 2)
            {
                throw new ArgumentException("array.Length - index < 2");
            }

            array[index] = X;
            array[index + 1] = Y;
        }



        public static implicit operator Float2((float x, float y, float z) values) =>
            new(values.x, values.y);

        public static implicit operator (float x, float y)(in Float2 v) => (v.X, v.Y);

        public void Deconstruct(out float x, out float y) => (x, y) = (X, Y);
    }
}