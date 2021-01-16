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

        public static FloatColor operator +(FloatColor v1, FloatColor v2)
        {
            return new FloatColor(v1.R + v2.R, v1.G + v2.G, v1.B + v2.B, v1.A + v2.A);
        }

        public static FloatColor operator -(FloatColor v1, FloatColor v2)
        {
            return new FloatColor(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B, v1.A - v2.A);
        }

        public static FloatColor operator *(FloatColor color, int value)
        {
            return new FloatColor(color.R * value, color.G * value, color.B * value, color.A * value);
        }

        public static FloatColor operator *(FloatColor color, float value)
        {
            return new FloatColor(color.R * value, color.G * value, color.B * value, color.A * value);
        }

        public static FloatColor operator /(FloatColor color, float value)
        {
            return new FloatColor(color.R / value, color.G / value, color.B / value, color.A / value);
        }

        public static FloatColor operator *(FloatColor color1, FloatColor color2)
        {
            var r = color1.R * color2.R;
            var g = color1.G * color2.G;
            var b = color1.B * color2.B;
            var a = color1.A * color2.A;
            return new FloatColor(r, g, b, a);
        }

        public static FloatColor FromRgb(float r, float g, float b)
        {
            return new FloatColor(r, g, b);
        }

        public static FloatColor FromRgba(float r, float g, float b, float a = 1)
        {
            return new FloatColor(r, g, b, a);
        }

        public static FloatColor FromRgba(int r, int g, int b, int a = 255)
        {
            return new FloatColor(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        public static FloatColor FromArgb(int a, int r, int g, int b)
        {
            return new FloatColor(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        public static FloatColor FromRgba(uint color)
        {
            var r = (byte) ((color >> 0) & 255);
            var g = (byte) ((color >> 8) & 255);
            var b = (byte) ((color >> 16) & 255);
            var a = (byte) ((color >> 24) & 255);
            return FromRgba(r, g, b, a);
        }

        public static FloatColor LerpWithAlpha(FloatColor a, FloatColor b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return new FloatColor(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t,
                a.A + (b.A - a.A) * t
            );
        }
        
        public static FloatColor Lerp(FloatColor a, FloatColor b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return new FloatColor(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t
            );
        }
        
        public static FloatColor Lerp(FloatColor a, FloatColor b, FloatColor t)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToUint()
        {
            var a = ClampValue(A * 255);
            var r = ClampValue(R * 255);
            var g = ClampValue(G * 255);
            var b = ClampValue(B * 255);
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        public static implicit operator uint(FloatColor color)
        {
            return color.ToUint();
        }

        public static implicit operator System.Drawing.Color(FloatColor color)
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
        public static FloatColor Black = new FloatColor(0, 0, 0);

        /// <summary>
        /// (1,1,1,1)
        /// </summary>
        public static FloatColor White = new FloatColor(1, 1, 1);

        public static FloatColor Grey = new FloatColor(0.5f, 0.5f, 0.5f);
        public static FloatColor Red = new FloatColor(1, 0, 0);
        public static FloatColor Blue = new FloatColor(0, 0, 1);
        public static FloatColor Green = new FloatColor(0, 1, 0);
        public static FloatColor Yellow = new FloatColor(1, 1, 0);
        public static FloatColor UnityYellow = new FloatColor(1, 235f / 255f, 4f / 255f);
        public static FloatColor Cyan = new FloatColor(0, 1, 1);
        public static FloatColor Magenta = new FloatColor(1, 0, 1);
        public static FloatColor Purple = new FloatColor(1, 0, 1);
        public static FloatColor Error = new FloatColor(1, 0, 0.5647059F);
        public static FloatColor Clear = new FloatColor(0, 0, 0, 0);

        public float Grayscale => 0.299F * R + 0.587F * G + 0.114F * B;
        public float MaxColorComponent => MathF.Max(MathF.Max(R, G), B);
        public uint Uint => ToUint();


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

        public static bool operator ==(FloatColor left, FloatColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FloatColor left, FloatColor right)
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

        public float this[int index] =>
            index switch
            {
                0 => R,
                1 => G,
                2 => B,
                3 => A,
                _ => throw new IndexOutOfRangeException("Invalid Color index(" + index + ")!")
            };

        public static FloatColor FromNormal(Float3 normal)
        {
            var converted = (normal + Float3.One) / 2;
            return new FloatColor(converted.X, converted.Y, converted.Z);
        }

        public static FloatColor AlphaToOne(FloatColor color) => new(color.R, color.G, color.B);
        public FloatColor AlphaToOne() => new(R, G, B);
        public FloatColor WithAlpha(float a) => new(R, G, B, a);

        public Float3 ToFloat3()
        {
            return new Float3(R, G, B);
        }
    }
}