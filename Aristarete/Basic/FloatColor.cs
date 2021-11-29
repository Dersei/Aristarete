using System;
using System.Runtime.CompilerServices;
using Aristarete.Extensions;

namespace Aristarete.Basic
{
    public readonly struct FloatColor : IEquatable<FloatColor>, IFormattable
    {
        public readonly float R;
        public readonly float G;
        public readonly float B;
        public readonly float A;

        public FloatColor(float r, float g, float b, float a = 1f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public FloatColor(in Float3 vector, float a = 1f)
        {
            R = vector.X;
            G = vector.Y;
            B = vector.Z;
            A = a;
        }
        
        public FloatColor(in Float3Sse vector, float a = 1f)
        {
            R = vector.X;
            G = vector.Y;
            B = vector.Z;
            A = a;
        }

        public FloatColor(in Float4Sse vector)
        {
            R = vector.X;
            G = vector.Y;
            B = vector.Z;
            A = vector.W;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor operator +(in FloatColor v1, in FloatColor v2)
        {
            return new(v1.R + v2.R, v1.G + v2.G, v1.B + v2.B, v1.A + v2.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor operator -(in FloatColor v1, in FloatColor v2)
        {
            return new(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B, v1.A - v2.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor operator *(in FloatColor color, int value)
        {
            return new(color.R * value, color.G * value, color.B * value, color.A * value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor operator *(in FloatColor color, float value)
        {
            return new(color.R * value, color.G * value, color.B * value, color.A * value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor operator /(in FloatColor color, float value)
        {
            return new(color.R / value, color.G / value, color.B / value, color.A / value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor operator *(in FloatColor color1, in FloatColor color2)
        {
            return new(color1.R * color2.R, color1.G * color2.G, color1.B * color2.B, color1.A * color2.A);
        }

        /// <summary>
        /// Creates a new color from the normal vector after converting its values.
        /// </summary>
        /// <param name="normal">Normal</param>
        /// <returns></returns>
        public static FloatColor FromNormal(in Float3 normal)
        {
            var converted = (normal + Float3.One) / 2;
            return new FloatColor(converted);
        }

        /// <summary>
        /// Creates a new color from the vector.
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns></returns>
        public static FloatColor FromVector(in Float3 vector)
        {
            return new(vector);
        }

        /// <summary>
        /// Creates a new color from the vector.
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns></returns>
        public static FloatColor FromVectorSafe(in Float3 vector)
        {
            var nVector = vector.Normalize();
            nVector = new Float3(MathExtensions.Clamp01(nVector.X), MathExtensions.Clamp01(nVector.Y),
                MathExtensions.Clamp01(nVector.Z));
            return new FloatColor(nVector);
        }

        public static FloatColor FromRgb(float r, float g, float b)
        {
            return new(r, g, b);
        }

        public static FloatColor FromRgba(float r, float g, float b, float a = 1)
        {
            return new(r, g, b, a);
        }

        public static FloatColor FromRgba(int r, int g, int b, int a = 255)
        {
            return new(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        public static FloatColor FromArgb(int a, int r, int g, int b)
        {
            return new(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        public static FloatColor FromRgba(uint color)
        {
            var r = (byte) ((color >> 0) & 255);
            var g = (byte) ((color >> 8) & 255);
            var b = (byte) ((color >> 16) & 255);
            var a = (byte) ((color >> 24) & 255);
            return FromRgba(r, g, b, a);
        }

        public static FloatColor FromArgb(uint color)
        {
            var b = (byte) ((color >> 0) & 255);
            var g = (byte) ((color >> 8) & 255);
            var r = (byte) ((color >> 16) & 255);
            var a = (byte) ((color >> 24) & 255);
            return FromArgb(a, r, g, b);
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ClampValue(float value)
        {
            return value <= 0 ? 0 : value >= 255 ? 255 : (uint) value;
        }

        /// <summary>
        /// Converts <c>FloatColor</c> to <c>uint</c> in ARGB format
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToUint()
        {
            var a = ClampValue(A * 255);
            var r = ClampValue(R * 255);
            var g = ClampValue(G * 255);
            var b = ClampValue(B * 255);
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        /// <summary>
        /// Returns <c>FloatColor</c> as <c>uint</c> in ARGB format
        /// </summary>
        /// <returns></returns>
        public uint ToArgb()
        {
            return ToUint();
        }

        /// <summary>
        /// Returns <c>FloatColor</c> as <c>uint</c> in RGB format with A = 255
        /// </summary>
        /// <returns></returns>
        public uint ToRgb()
        {
            const uint a = 255u;
            var r = ClampValue(R * 255);
            var g = ClampValue(G * 255);
            var b = ClampValue(B * 255);
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        /// <summary>
        /// Converts <c>FloatColor</c> to <c>uint</c>
        /// </summary>
        /// <returns></returns>
        public static implicit operator uint(in FloatColor color)
        {
            return color.ToUint();
        }
        
        /// <summary>
        /// Converts <c>System.Drawing.Color</c> to <c>FloatColor</c>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static implicit operator System.Drawing.Color(in FloatColor color)
        {
            var a = (byte) color.A * 255;
            var r = (byte) color.R * 255;
            var g = (byte) color.G * 255;
            var b = (byte) color.B * 255;
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }
        
        public static implicit operator FloatColor(System.Drawing.Color color)
        {
            return FromRgba(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// (0,0,0,1)
        /// </summary>
        public static readonly FloatColor Black = new(0, 0, 0);
        /// <summary>
        /// (1,1,1,1)
        /// </summary>
        public static readonly FloatColor White = new(1, 1, 1);
        public static readonly FloatColor Grey = new(0.5f, 0.5f, 0.5f);
        public static readonly FloatColor Red = new(1, 0, 0);
        public static readonly FloatColor Blue = new(0, 0, 1);
        public static readonly FloatColor Green = new(0, 1, 0);
        public static readonly FloatColor Yellow = new(1, 1, 0);
        public static readonly FloatColor UnityYellow = new(1, 235f / 255f, 4f / 255f);
        public static readonly FloatColor Cyan = new(0, 1, 1);
        public static readonly FloatColor Magenta = new(1, 0, 1);
        public static readonly FloatColor Purple = new(1, 0, 1);
        public static readonly FloatColor Error = new(1, 0, 0.5647059F);
        public static readonly FloatColor Clear = new(0, 0, 0, 0);

        public float Grayscale => 0.299F * R + 0.587F * G + 0.114F * B;
        public float MaxColorComponent => MathF.Max(MathF.Max(R, G), B);


        public bool Equals(FloatColor other)
        {
            return R.IsAbout(other.R) && G.IsAbout(other.G) && B.IsAbout(other.B) && A.IsAbout(other.A);
        }

        public override bool Equals(object? obj)
        {
            return obj is FloatColor other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }

        public static bool operator ==(in FloatColor left, in FloatColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in FloatColor left, in FloatColor right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"FloatColor - ({R}, {G}, {B}, {A})";
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"FloatColor - ({R.ToString(format, formatProvider)}, {G.ToString(format, formatProvider)}, {B.ToString(format, formatProvider)})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor LerpUnclamped(in FloatColor a, in FloatColor b, float t)
        {
            return new(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t,
                a.A + (b.A - a.A) * t
            );
        }

        public static FloatColor Lerp(in FloatColor a, in FloatColor b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }
        
        public static FloatColor LerpWithoutAlpha(in FloatColor a, in FloatColor b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return new FloatColor(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t
            );
        }
        
        public static FloatColor Lerp(in FloatColor a, in FloatColor b, in FloatColor t)
        {
            var tR = MathExtensions.Clamp01(t.R);
            var tG = MathExtensions.Clamp01(t.G);
            var tB = MathExtensions.Clamp01(t.B);
            return new FloatColor(
                a.R + (b.R - a.R) * tR,
                a.G + (b.G - a.G) * tG,
                a.B + (b.B - a.B) * tB
            );
        }

        public float this[int index] =>
            index switch
            {
                0 => R,
                1 => G,
                2 => B,
                3 => A,
                _ => throw new IndexOutOfRangeException("Invalid FloatColor index(" + index + ")!")
            };
        
        // Convert a set of HSV values to an RGB Color.
        public static FloatColor FromHsv(float h, float s, float v, bool hdr)
        {
            if (s.IsAboutZero())
            {
                return new FloatColor(v, v, v);
            }

            if (v.IsAboutZero())
            {
                return new FloatColor(0, 0, 0);
            }

            //crazy hsv conversion

            var tS = s;
            var tV = v;
            var hToFloor = h * 6.0f;

            var temp = (int) MathF.Floor(hToFloor);
            var t = hToFloor - temp;
            var var1 = tV * (1 - tS);
            var var2 = tV * (1 - tS * t);
            var var3 = tV * (1 - tS * (1 - t));

            var result = temp switch
            {
                0 => new FloatColor(tV, var3, var1),
                1 => new FloatColor(var2, tV, var1),
                2 => new FloatColor(var1, tV, var3),
                3 => new FloatColor(var1, var2, tV),
                4 => new FloatColor(var3, var1, tV),
                5 => new FloatColor(tV, var1, var2),
                6 => new FloatColor(tV, var3, var1),
                -1 => new FloatColor(tV, var1, var2),
                _ => Error
            };


            if (!hdr)
            {
                var r = MathExtensions.Clamp(result.R, 0.0f, 1.0f);
                var g = MathExtensions.Clamp(result.G, 0.0f, 1.0f);
                var b = MathExtensions.Clamp(result.B, 0.0f, 1.0f);
                return new FloatColor(r, g, b);
            }

            return result;
        }

        public static FloatColor ConvertToSRgb(in FloatColor value)
        {
            var r = value.R > 0.0031308f ? 1.055f * MathF.Pow(value.R, 1.0f / 2.4f) - 0.055f : 12.92f * value.R;
            var g = value.G > 0.0031308f ? 1.055f * MathF.Pow(value.G, 1.0f / 2.4f) - 0.055f : 12.92f * value.G;
            var b = value.B > 0.0031308f ? 1.055f * MathF.Pow(value.B, 1.0f / 2.4f) - 0.055f : 12.92f * value.B;
            return new FloatColor(r, g, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor ConvertToGamma(in FloatColor value)
        {
            var r = MathF.Pow(value.R, 1.0f / 2.2f);
            var g = MathF.Pow(value.B, 1.0f / 2.2f);
            var b = MathF.Pow(value.G, 1.0f / 2.2f);
            return new FloatColor(r, g, b);
        }
        
        public static FloatColor ConvertToHdr(in FloatColor color, float gamma = 2.2f, float exposure = 1.0f)
        {
            var r = MathF.Pow(1 - MathF.Exp(-color.R * exposure), 1.0f / gamma);
            var g = MathF.Pow(1 - MathF.Exp(-color.G * exposure), 1.0f / gamma);
            var b = MathF.Pow(1 - MathF.Exp(-color.B * exposure), 1.0f / gamma);
            return new FloatColor(r, g, b);
        }
        
        public static FloatColor AlphaToOne(in FloatColor color) => new(color.R, color.G, color.B);
        public FloatColor AlphaToOne() => new(R, G, B);
        public FloatColor WithAlpha(float a) => new(R, G, B, a);
    }
}