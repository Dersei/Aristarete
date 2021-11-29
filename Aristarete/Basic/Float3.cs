using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aristarete.Extensions;

namespace Aristarete.Basic
{
    public readonly struct Float3 : IEquatable<Float3>, IFormattable
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public Float3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Float3(float value) : this(value, value, value)
        {
        }

        public Float3(in Float2 vector, float z) : this(vector.X, vector.Y, z)
        {
        }

        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Float3 Zero = new(0, 0, 0);
        public static readonly Float3 One = new(1, 1, 1);
        public static readonly Float3 Up = new(0f, 1f, 0);
        public static readonly Float3 Down = new(0f, -1f, 0);
        public static readonly Float3 Left = new(-1f, 0, 0);
        public static readonly Float3 Right = new(1f, 0, 0);
        public static readonly Float3 Forward = new(0f, 0f, 1f);
        public static readonly Float3 Back = new(0f, 0f, -1f);
        public static readonly Float3 UnitX = new(1f, 0f, 0f);
        public static readonly Float3 UnitY = new(0f, 1f, 0f);
        public static readonly Float3 UnitZ = new(0f, 0f, 1f);

        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;
        private const float EpsilonNormalSqrt = 1e-15f;

        public static readonly Float3 PositiveInfinity =
            new(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Float3 NegativeInfinity =
            new(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z);

        public float LengthSquared => X * X + Y * Y + Z * Z;

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator +(in Float3 v1, in Float3 v2)
        {
            return new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator -(in Float3 v1, in Float3 v2)
        {
            return new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(in Float3 v1, in Float3 v2)
        {
            return new(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(in Float3 v, float scalar)
        {
            return new(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(float scalar, in Float3 v)
        {
            return new(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator /(in Float3 v, float scalar)
        {
            var inverse = 1f / scalar;
            return new Float3(v.X * inverse, v.Y * inverse, v.Z * inverse);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator /(float scalar, in Float3 v)
        {
            return new(scalar / v.X, scalar / v.Y, scalar / v.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator -(in Float3 v)
        {
            return new(-v.X, -v.Y, -v.Z);
        }

        #endregion

        #region Equality

        public static bool operator ==(in Float3 left, in Float3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in Float3 left, in Float3 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Float3 other)
        {
            var diffX = X - other.X;
            var diffY = Y - other.Y;
            var diffZ = Z - other.Z;
            var sqrMag = diffX * diffX + diffY * diffY + diffZ * diffZ;
            return sqrMag < DoubledEpsilon;
        }

        public static bool Equals(in Float3 left, in Float3 right)
        {
            return left == right;
        }

        public static bool FloatEquals(in Float3 left, in Float3 right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Float3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        #endregion

        #region ToString

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)})";
        }

        public string ToString(string? format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        #endregion

        public Float3 Normalize()
        {
            var length = Length;
            if (length > Epsilon)
            {
                return this / length;
            }

            return Zero;
        }

        public static Float3 Normalize(in Float3 v)
        {
            return v.Normalize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 NormalizeUnsafe() => this / Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(in Float3 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in Float3 v1, in Float3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public Float3 Cross(in Float3 v)
        {
            return new(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }
        
        public static Float3 Cross(in Float3 v1, in Float3 v2)
        {
            return new(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Reflect(in Float3 normal)
        {
            return -2 * Dot(this, normal) * normal + this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 Reflect(in Float3 vector, in Float3 normal)
        {
            return -2 * Dot(vector, normal) * normal + vector;
        }

        public static Float3 Project(in Float3 vector, in Float3 onNormal)
        {
            float sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < Epsilon)
            {
                return Zero;
            }

            var dot = Dot(vector, onNormal);
            return new Float3(onNormal.X * dot / sqrMag,
                onNormal.Y * dot / sqrMag,
                onNormal.Z * dot / sqrMag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 Saturate(in Float3 vector)
        {
            return new(
                MathExtensions.Clamp01(vector.X),
                MathExtensions.Clamp01(vector.Y),
                MathExtensions.Clamp01(vector.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Lerp(in Float3 vector, float t)
        {
            return new(X + t * (vector.X - X), Y + t * (vector.Y - Y), Z + t * (vector.Z - Z));
        }

        public static Float3 Lerp(in Float3 v1, in Float3 v2, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(v1, v2, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Float3 LerpUnclamped(in Float3 v1, in Float3 v2, float t)
        {
            return new(
                v1.X + (v2.X - v1.X) * t,
                v1.Y + (v2.Y - v1.Y) * t,
                v1.Z + (v2.Z - v1.Z) * t
            );
        }

        public static float Angle(in Float3 from, in Float3 to)
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

        public static float SignedAngle(in Float3 from, in Float3 to, in Float3 axis)
        {
            var unsignedAngle = Angle(from, to);

            var crossX = from.Y * to.Z - from.Z * to.Y;
            var crossY = from.Z * to.X - from.X * to.Z;
            var crossZ = from.X * to.Y - from.Y * to.X;
            var sign = MathF.Sign(axis.X * crossX + axis.Y * crossY + axis.Z * crossZ);
            return unsignedAngle * sign;
        }

        public static float Distance(in Float3 v1, in Float3 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            var diffZ = v1.Z - v2.Z;
            return MathF.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
        }

        public static float DistanceSquared(in Float3 v1, in Float3 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            var diffZ = v1.Z - v2.Z;
            return diffX * diffX + diffY * diffY + diffZ * diffZ;
        }

        public static Float3 SquareRoot(in Float3 vector)
        {
            return new(
                MathF.Sqrt(vector.X),
                MathF.Sqrt(vector.Y),
                MathF.Sqrt(vector.Z)
            );
        }

        public static Float3 Abs(in Float3 vector)
        {
            return new(
                MathF.Abs(vector.X),
                MathF.Abs(vector.Y),
                MathF.Abs(vector.Z)
            );
        }

        public Vector3 ToBuiltIn()
        {
            return new(X, Y, Z);
        }

        public static Float3 FromBuiltIn(Vector3 vector3)
        {
            return new(vector3.X, vector3.Y, vector3.Z);
        }

        public static Float3 Transform(in Float3 vector, float scale, in Float3 position, in Float3 rotationAxis,
            float angle)
        {
            var matrixScale = Matrix4x4.CreateScale(scale);
            var matrixPosition = Matrix4x4.CreateTranslation(position.ToBuiltIn());
            var matrixRotation =
                Matrix4x4.CreateFromAxisAngle(rotationAxis.ToBuiltIn(), angle * MathExtensions.Deg2Rad);
            var transformMatrix = matrixScale * matrixRotation * matrixPosition;
            return FromBuiltIn(Vector3.Transform(vector.ToBuiltIn(), transformMatrix));
        }

        public static Float3 Min(in Float3 v1, in Float3 v2)
        {
            return new(
                v1.X < v2.X ? v1.X : v2.X,
                v1.Y < v2.Y ? v1.Y : v2.Y,
                v1.Z < v2.Z ? v1.Z : v2.Z
            );
        }

        public static Float3 Max(in Float3 v1, in Float3 v2)
        {
            return new(
                v1.X > v2.X ? v1.X : v2.X,
                v1.Y > v2.Y ? v1.Y : v2.Y,
                v1.Z > v2.Z ? v1.Z : v2.Z
            );
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 Clamp(in Float3 vector, in Float3 min, in Float3 max)
        {
            // We must follow HLSL behavior in the case user specified min value is bigger than max value.
            return Min(Max(vector, min), max);
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

            if (array.Length - index < 3)
            {
                throw new ArgumentException("array.Length - index < 3");
            }

            array[index] = X;
            array[index + 1] = Y;
            array[index + 2] = Z;
        }
        
        public Float2 XY() => new(X, Y);

        public static implicit operator Float3((float x, float y, float z) values) =>
            new(values.x, values.y, values.z);

        public static implicit operator (float x, float y, float z)(in Float3 v) => (v.X, v.Y, v.Z);

        public void Deconstruct(out float x, out float y, out float z) => (x, y, z) = (X, Y, Z);
    }
}