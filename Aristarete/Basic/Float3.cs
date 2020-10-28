using System;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3(Float3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Float3 Zero = new Float3();
        public static readonly Float3 One = new Float3(1, 1, 1);
        public static readonly Float3 Up = new Float3(0f, 1f, 0);
        public static readonly Float3 Down = new Float3(0f, -1f, 0);
        public static readonly Float3 Left = new Float3(-1f, 0, 0);
        public static readonly Float3 Right = new Float3(1f, 0, 0);
        public static readonly Float3 Forward = new Float3(0f, 0f, 1f);
        public static readonly Float3 Back = new Float3(0f, 0f, -1f);

        public static readonly Float3 PositiveInfinity =
            new Float3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Float3 NegativeInfinity =
            new Float3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z);

        public float LengthSquared => X * X + Y * Y + Z * Z;

        #region Operators

        public static Float3 operator +(Float3 v1, Float3 v2)
        {
            return new Float3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Float3 operator -(Float3 v1, Float3 v2)
        {
            return new Float3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Float3 operator *(Float3 v1, Float3 v2)
        {
            return new Float3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        public static Float3 operator *(Float3 v, float scalar)
        {
            return new Float3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Float3 operator *(float scalar, Float3 v)
        {
            return new Float3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Float3 operator /(Float3 v, float scalar)
        {
            var inverse = 1f / scalar;
            return new Float3(v.X * inverse, v.Y * inverse, v.Z * inverse);
        }

        public static Float3 operator /(float scalar, Float3 v)
        {
            return new Float3(scalar / v.X, scalar / v.Y, scalar / v.Z);
        }

        public static Float3 operator -(Float3 v)
        {
            return new Float3(-v.X, -v.Y, -v.Z);
        }

        #endregion

        #region Equality

        public static bool operator ==(Float3 left, Float3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Float3 left, Float3 right)
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

        public override bool Equals(object? obj)
        {
            return obj is Float3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(Float3 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        public Float3 Cross(Float3 v)
        {
            return new Float3(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }

        public Float3 Reflect(Float3 normal)
        {
            // return this - 2 * Dot(normal) * normal;
            //return 2 * normal * Dot(normal) - this;
            return 2 * normal * normal.Dot(this) - this;
        }

        public static Float3 Project(Float3 vector, Float3 onNormal)
        {
            float sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < Epsilon)
                return Zero;
            else
            {
                var dot = Dot(vector, onNormal);
                return new Float3(onNormal.X * dot / sqrMag,
                    onNormal.Y * dot / sqrMag,
                    onNormal.Z * dot / sqrMag);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Lerp(Float3 v, float t)
        {
            t = MathExtensions.Clamp01(t);
            return new Float3(X + t * (v.X - X), Y + t * (v.Y - Y), Z + t * (v.Z - Z));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 LerpUnclamped(Float3 v, float t)
        {
            return new Float3(X + t * (v.X - X), Y + t * (v.Y - Y), Z + t * (v.Z - Z));
        }

        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;
        private const float EpsilonNormalSqrt = 1e-15f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Float3 v1, Float3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static float Angle(Float3 from, Float3 to)
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

        public static float Distance(Float3 v1, Float3 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            var diffZ = v1.Z - v2.Z;
            return MathF.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
        }

        public static Float3 Lerp(Float3 a, Float3 b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Float3 LerpUnclamped(Float3 a, Float3 b, float t)
        {
            return new Float3(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t
            );
        }


        public Vector3 ToBuiltIn()
        {
            return new Vector3(X, Y, Z);
        }

        public static Float3 FromBuiltIn(Vector3 vector3)
        {
            return new Float3(vector3.X, vector3.Y, vector3.Z);
        }

        public static Float3 Transform(Float3 float3, float scale, Float3 position, Float3 rotationAxis,
            float angle)
        {
            var matrixScale = Matrix4x4.CreateScale(scale);
            var matrixPosition = Matrix4x4.CreateTranslation(position.ToBuiltIn());
            var matrixRotation =
                Matrix4x4.CreateFromAxisAngle(rotationAxis.ToBuiltIn(), angle * MathExtensions.Deg2Rad);
            var transformMatrix = matrixScale * matrixRotation * matrixPosition;
            return FromBuiltIn(Vector3.Transform(float3.ToBuiltIn(), transformMatrix));
        }


        public static implicit operator Float3((float x, float y, float z) values) =>
            new Float3(values.x, values.y, values.z);

        public static implicit operator (float x, float y, float z)(Float3 v) => (v.X, v.Y, v.Z);

        public void Deconstruct(out float x, out float y, out float z) => (x, y, z) = (X, Y, Z);

        public Float2 XY => new Float2(X, Y);
    }
}