using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Aristarete.Extensions;

namespace Aristarete.Basic
{
    public readonly struct Float3Sse : IEquatable<Float3Sse>, IFormattable
    {
        public readonly Vector128<float> Vector;
        public float X => Vector.GetElement(0);
        public float Y => Vector.GetElement(1);
        public float Z => Vector.GetElement(2);


        public Float3Sse(float x, float y, float z)
        {
            Vector = Vector128.Create(x, y, z, 1);
        }

        public Float3Sse(in Vector128<float> vector)
        {
            Vector = vector;
        }

        public Float3Sse(float value) : this(value, value, value)
        {
        }

        public Float3Sse(in Float2 value1, float z) : this(value1.X, value1.Y, z)
        {
        }

        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Float3Sse Zero = new(0, 0, 0);
        public static readonly Float3Sse One = new(1, 1, 1);
        public static readonly Float3Sse Up = new(0f, 1f, 0);
        public static readonly Float3Sse Down = new(0f, -1f, 0);
        public static readonly Float3Sse Left = new(-1f, 0, 0);
        public static readonly Float3Sse Right = new(1f, 0, 0);
        public static readonly Float3Sse Forward = new(0f, 0f, 1f);
        public static readonly Float3Sse Back = new(0f, 0f, -1f);
        public static readonly Float3Sse UnitX = new(1f, 0f, 0f);
        public static readonly Float3Sse UnitY = new(0f, 1f, 0f);
        public static readonly Float3Sse UnitZ = new(0f, 0f, 1f);

        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;

        public static readonly Float3Sse PositiveInfinity =
            new(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Float3Sse NegativeInfinity =
            new(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public float Length => Sse.SqrtScalar(Sse41.DotProduct(Vector, Vector, 0b01110001)).GetElement(0);

        public float LengthSquared => Sse41.DotProduct(Vector, Vector, 0b01110001).GetElement(0);

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse operator +(in Float3Sse v1, in Float3Sse v2)
        {
            return new(Sse.Add(v1.Vector, v2.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse operator -(in Float3Sse v1, in Float3Sse v2)
        {
            return new(Sse.Subtract(v1.Vector, v2.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse operator *(in Float3Sse v1, in Float3Sse v2)
        {
            return new(Sse.Multiply(v1.Vector, v2.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse operator *(in Float3Sse v, float scalar)
        {
            return new(Sse.Multiply(v.Vector, Vector128.Create(scalar)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse operator *(float scalar, in Float3Sse v)
        {
            return new(Sse.Multiply(Vector128.Create(scalar), v.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse operator /(in Float3Sse v, float scalar)
        {
            return new (Sse.Multiply(v.Vector, Vector128.Create(1f / scalar)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse operator /(float scalar, in Float3Sse v)
        {
            return new (Sse.Divide(Vector128.Create(scalar), v.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse operator -(in Float3Sse v)
        {
            return new(Sse.Subtract(Vector128<float>.Zero, v.Vector));
        }

        #endregion

        #region Equality

        public static bool operator ==(in Float3Sse left, in Float3Sse right)
        {
            return SimdExtensions.Equal(left.Vector, right.Vector);
        }

        public static bool operator !=(in Float3Sse left, in Float3Sse right)
        {
            return SimdExtensions.NotEqual(left.Vector, right.Vector);
        }

        public bool Equals(Float3Sse other)
        {
            var sqrMag = DistanceSquared(this, other);
            return sqrMag < DoubledEpsilon;
        }

        public static bool Equals(in Float3Sse left, in Float3Sse right)
        {
            return SimdExtensions.Equal(left.Vector, right.Vector);
        }

        public static bool FloatEquals(in Float3Sse left, in Float3Sse right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Float4Sse other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Vector.GetHashCode();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3Sse Normalize()
        {
            return new(Sse.Multiply(Vector, Sse.ReciprocalSqrt(Sse41.DotProduct(Vector, Vector, 0b01110111))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3Sse NormalizeExact()
        {
            return new(Sse.Divide(Vector, Sse.Sqrt(Sse41.DotProduct(Vector, Vector, 0b01110111))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse Normalize(in Float3Sse vector)
        {
            return vector.Normalize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(Float3Sse v)
        {
            return Sse41.DotProduct(Vector, v.Vector, 0b01110001).GetElement(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in Float3Sse a, in Float3Sse b)
        {
            return Sse41.DotProduct(a.Vector, b.Vector, 0b01110001).GetElement(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte Shuffle(int z, int y, int x, int w)
        {
            return (byte) ((z << 6) | (y << 4) | (x << 2) | w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3Sse Cross(in Float3Sse v)
        {
            var x1 = X;
            var y1 = Y;
            var z1 = Z;
            var x2 = v.X;
            var y2 = v.Y;
            var z2 = v.Z;
            return new Float3Sse(y1 * z2 - z1 * y2, z1 * x2 - x1 * z2, x1 * y2 - y1 * x2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse Cross(in Float3Sse v1, in Float3Sse v2)
        {
            var x1 = v1.X;
            var y1 = v1.Y;
            var z1 = v1.Z;
            var x2 = v2.X;
            var y2 = v2.Y;
            var z2 = v2.Z;
            return new Float3Sse(y1 * z2 - z1 * y2, z1 * x2 - x1 * z2, x1 * y2 - y1 * x2);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse Project(in Float3Sse a, in Float3Sse b)
        {
            return new(Sse.Multiply(b.Vector,
                Sse.Divide(Sse41.DotProduct(a.Vector, b.Vector, 255), Sse41.DotProduct(b.Vector, b.Vector, 255))));
            //return b * (Dot(a, b) / Dot(b, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3Sse Lerp(Float3Sse v, float t)
        {
            return new(Sse.Add(Vector, Sse.Multiply(Sse.Subtract(v.Vector, Vector), Vector128.Create(t))));
        }

        public static Float3Sse Lerp(in Float3Sse a, in Float3Sse b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Float3Sse LerpUnclamped(in Float3Sse a, in Float3Sse b, float t)
        {
            return new(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t
            );
        }

        public static float Distance(in Float3Sse a, in Float3Sse b)
        {
            var diff = Sse.Subtract(a.Vector, b.Vector);
            return Sse.SqrtScalar(Sse41.DotProduct(diff, diff, 0b01110001)).GetElement(0);
        }

        public static float DistanceSquared(in Float3Sse a, in Float3Sse b)
        {
            var diff = Sse.Subtract(a.Vector, b.Vector);
            return Sse41.DotProduct(diff, diff, 0b01110001).GetElement(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse SquareRoot(in Float3Sse value)
        {
            return new(Sse.Sqrt(value.Vector));
        }

        public static Float3Sse Abs(in Float3Sse value)
        {
            return new(
                MathF.Abs(value.X),
                MathF.Abs(value.Y),
                MathF.Abs(value.Z)
            );
        }

        public Vector3 ToBuiltIn()
        {
            return Vector.AsVector3();
        }

        public static Float3Sse FromBuiltIn(Vector3 vector4)
        {
            return new(vector4.AsVector128());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse Min(in Float3Sse a, in Float3Sse b)
        {
            return new(Sse.Min(a.Vector, b.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse Max(in Float3Sse a, in Float3Sse b)
        {
            return new(Sse.Max(a.Vector, b.Vector));
        }

        private static readonly Vector128<float> Vector2 = Vector128.Create(2f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse Reflect(in Float3Sse vector, in Float3Sse normal)
        {
            return new(Sse.Subtract(vector.Vector,
                Sse.Multiply(Vector2,
                    Sse.Multiply(normal.Vector,
                        Sse41.DotProduct(vector.Vector, normal.Vector, 0b01110111)))));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3Sse Reflect(in Float3Sse normal)
        {
            return new(Sse.Subtract(Vector,
                Sse.Multiply(Vector2,
                    Sse.Multiply(normal.Vector,
                        Sse41.DotProduct(Vector, normal.Vector, 0b01110111)))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse Saturate(in Float3Sse vector)
        {
            return new(Sse.Max(Sse.Min(vector.Vector, One.Vector), Vector128<float>.Zero));
        }
        
        public static Float3Sse Transform(in Float3Sse vector, float scale, in Float3Sse position, in Float3Sse rotationAxis,
            float angle)
        {
            var matrixScale = Matrix4x4.CreateScale(scale);
            var matrixPosition = Matrix4x4.CreateTranslation(position.ToBuiltIn());
            var matrixRotation =
                Matrix4x4.CreateFromAxisAngle(rotationAxis.ToBuiltIn(), angle * MathExtensions.Deg2Rad);
            var transformMatrix = matrixScale * matrixRotation * matrixPosition;
            return FromBuiltIn(Vector3.Transform(vector.ToBuiltIn(), transformMatrix));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3Sse Clamp(in Float3Sse value, in Float3Sse min, in Float3Sse max)
        {
            return new(Sse.Min(Sse.Max(value.Vector, min.Vector), max.Vector));
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
        }

        public static implicit operator Float3Sse((float x, float y, float z) values) =>
            new(values.x, values.y, values.z);

        public Float2 XY() => new(X, Y);
        
        public static implicit operator (float x, float y, float z)(in Float3Sse v) => (v.X, v.Y, v.Z);

        public void Deconstruct(out float x, out float y, out float z) => (x, y, z) = (X, Y, Z);
    }
}