using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Aristarete.Extensions;

namespace Aristarete.Basic
{
    public readonly struct Float4Sse : IEquatable<Float4Sse>, IFormattable
    {
        public readonly Vector128<float> Vector;
        public float X => Vector.GetElement(0);
        public float Y => Vector.GetElement(1);
        public float Z => Vector.GetElement(2);
        public float W => Vector.GetElement(3);


        public Float4Sse(float x, float y, float z, float w)
        {
            Vector = Vector128.Create(x, y, z, w);
        }

        private Float4Sse(in Vector128<float> vector)
        {
            Vector = vector;
        }

        public Float4Sse(float value) : this(value, value, value, value)
        {
        }

        public Float4Sse(in Float3 value, float w) : this(value.X, value.Y, value.Z, w)
        {
        }

        public Float4Sse(in Float3Sse value, float w) : this(value.X, value.Y, value.Z, w)
        {
        }

        
        public Float4Sse(in Float2 value1, in Float2 value2) : this(value1.X, value1.Y, value2.X, value2.Y)
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

        public static readonly Float4Sse Zero = new Float4Sse(0, 0, 0, 0);
        public static readonly Float4Sse One = new Float4Sse(1, 1, 1, 1);
        public static readonly Float4Sse UnitX = new Float4Sse(1f, 0f, 0f, 0f);
        public static readonly Float4Sse UnitY = new Float4Sse(0f, 1f, 0f, 0f);
        public static readonly Float4Sse UnitZ = new Float4Sse(0f, 0f, 1f, 0f);
        public static readonly Float4Sse UnitW = new Float4Sse(0f, 0f, 0f, 1f);

        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;

        public static readonly Float4Sse PositiveInfinity =
            new Float4Sse(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity,
                float.PositiveInfinity);

        public static readonly Float4Sse NegativeInfinity =
            new Float4Sse(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity,
                float.NegativeInfinity);

        public float Length => Sse.SqrtScalar(Sse41.DotProduct(Vector, Vector, 0b11110001)).GetElement(0);

        public float LengthSquared => Sse41.DotProduct(Vector, Vector, 0b11110001).GetElement(0);

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse operator +(in Float4Sse v1, in Float4Sse v2)
        {
            return new(Sse.Add(v1.Vector, v2.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse operator -(in Float4Sse v1, in Float4Sse v2)
        {
            return new(Sse.Subtract(v1.Vector, v2.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse operator *(in Float4Sse v1, in Float4Sse v2)
        {
            return new(Sse.Multiply(v1.Vector, v2.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse operator *(in Float4Sse v, float scalar)
        {
            var scalarVector = Vector128.Create(scalar);
            return new Float4Sse(Sse.Multiply(v.Vector, scalarVector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse operator *(float scalar, in Float4Sse v)
        {
            var scalarVector = Vector128.Create(scalar);
            return new Float4Sse(Sse.Multiply(scalarVector, v.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse operator /(in Float4Sse v, float scalar)
        {
            var scalarVector = Vector128.Create(1f / scalar);
            return new Float4Sse(Sse.Multiply(v.Vector, scalarVector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse operator /(float scalar, in Float4Sse v)
        {
            var scalarVector = Vector128.Create(scalar);
            return new Float4Sse(Sse.Divide(scalarVector, v.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse operator -(in Float4Sse v)
        {
            return new(Sse.Subtract(Vector128<float>.Zero, v.Vector));
        }

        #endregion

        #region Equality

        public static bool operator ==(in Float4Sse left, in Float4Sse right)
        {
            return SimdExtensions.Equal(left.Vector, right.Vector);
        }

        public static bool operator !=(in Float4Sse left, in Float4Sse right)
        {
            return SimdExtensions.NotEqual(left.Vector, right.Vector);
        }

        public bool Equals(Float4Sse other)
        {
            var sqrMag = DistanceSquared(this, other);
            return sqrMag < DoubledEpsilon;
        }

        public static bool Equals(in Float4Sse left, in Float4Sse right)
        {
            return SimdExtensions.Equal(left.Vector, right.Vector);
        }

        public static bool FloatEquals(in Float4Sse left, in Float4Sse right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
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
        public Float4Sse Normalize()
        {
            return new(Sse.Multiply(Vector, Sse.ReciprocalSqrt(Sse41.DotProduct(Vector, Vector, 0b11111111))));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4Sse NormalizeExact()
        {
            return new(Sse.Divide(Vector, Sse.Sqrt(Sse41.DotProduct(Vector, Vector, 0b11111111))));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse Normalize(in Float4Sse vector)
        {
            return vector.Normalize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(Float4Sse v)
        {
            return Sse41.DotProduct(Vector, v.Vector, 0b11110001).GetElement(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in Float4Sse a, in Float4Sse b)
        {
            return Sse41.DotProduct(a.Vector, b.Vector, 0b11110001).GetElement(0);
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse Project(in Float4Sse a, in Float4Sse b)
        {
            return new(Sse.Multiply(b.Vector,
                Sse.Divide(Sse41.DotProduct(a.Vector, b.Vector, 255), Sse41.DotProduct(b.Vector, b.Vector, 255))));
            //return b * (Dot(a, b) / Dot(b, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4Sse Lerp(in Float4Sse v, float t)
        {
            return new(Sse.Add(Vector, Sse.Multiply(Sse.Subtract(v.Vector, Vector), Vector128.Create(t))));
        }

        public static Float4Sse Lerp(in Float4Sse a, in Float4Sse b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Float4Sse LerpUnclamped(in Float4Sse a, in Float4Sse b, float t)
        {
            return new(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            );
        }

        public static float Distance(in Float4Sse a, in Float4Sse b)
        {
            var diff = Sse.Subtract(a.Vector, b.Vector);
            return Sse.SqrtScalar(Sse41.DotProduct(diff, diff, 0b11110001)).GetElement(0);
        }

        public static float DistanceSquared(in Float4Sse a, in Float4Sse b)
        {
            var diff = Sse.Subtract(a.Vector, b.Vector);
            return Sse41.DotProduct(diff, diff, 0b11110001).GetElement(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse SquareRoot(in Float4Sse value)
        {
            return new(Sse.Sqrt(value.Vector));
        }

        public static Float4Sse Abs(in Float4Sse value)
        {
            return new Float4Sse(
                MathF.Abs(value.X),
                MathF.Abs(value.Y),
                MathF.Abs(value.Z),
                MathF.Abs(value.W)
            );
        }

        public Vector4 ToBuiltIn()
        {
            return Vector.AsVector4();
        }

        public static Float4Sse FromBuiltIn(Vector4 vector4)
        {
            return new Float4Sse(vector4.AsVector128());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse Min(in Float4Sse a, in Float4Sse b)
        {
            return new(Sse.Min(a.Vector, b.Vector));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse Max(in Float4Sse a, in Float4Sse b)
        {
            return new(Sse.Max(a.Vector, b.Vector));
        }

        private static readonly Vector128<float> Vector2 = Vector128.Create(2f);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse Reflect(in Float4Sse vector, in Float4Sse normal)
        {
            return new(Sse.Subtract(vector.Vector,
                Sse.Multiply(Vector2,
                    Sse.Multiply(normal.Vector,
                        Sse41.DotProduct(vector.Vector, normal.Vector, 0xff)))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse Saturate(in Float4Sse vector)
        {
            return new(Sse.Max(Sse.Min(vector.Vector, One.Vector), Vector128<float>.Zero));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4Sse Clamp(in Float4Sse value, in Float4Sse min, in Float4Sse max)
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
            array[index + 3] = W;
        }

        public static implicit operator Float4Sse((float x, float y, float z, float w) values) =>
            new Float4Sse(values.x, values.y, values.z, values.w);

        public static implicit operator (float x, float y, float z, float w)(in Float4Sse v) => (v.X, v.Y, v.Z, v.W);

        public void Deconstruct(out float x, out float y, out float z, out float w) => (x, y, z, w) = (X, Y, Z, W);
        
        public Float3Sse XYZ() => new(X, Y, Z);
    }
}