using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aristarete.Basic;

namespace Aristarete.Extensions
{
    public static class MathExtensions
    {
        public const float Deg2Rad = MathF.PI * 2F / 360F;
        public const float Rad2Deg = 1F / Deg2Rad;

        public static float Clamp(float value, float min, float max)
        {
            if (value <= min)
                return min;
            return value >= max ? max : value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value <= min)
                return min;
            return value >= max ? max : value;
        }

        public static byte ClampToByte(this int value)
        {
            if (value <= 0)
                value = 0;
            else if (value >= 255)
                value = 255;
            return (byte) value;
        }

        public static byte ClampToByte(this float value) =>
            value <= 0 ? (byte) 0 : value >= 255 ? (byte) 255 : (byte) value;


        public static byte ClampToByte(this double value)
        {
            if (value <= 0)
                value = 0;
            else if (value >= 255)
                value = 255;
            return (byte) value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value)
        {
            if (value <= 0F)
                return 0F;
            return value >= 1F ? 1F : value;
        }


        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }

        public static bool IsAbout(this float first, float second, float precisionMultiplier = 8)
        {
            return MathF.Abs(second - first) < MathF.Max(1E-06f * MathF.Max(MathF.Abs(first), MathF.Abs(second)),
                float.Epsilon * precisionMultiplier);
        }

        public static float Repeat(float t, float length)
        {
            return t - MathF.Floor(t / length) * length;
        }

        public static float PingPong(float t, float length)
        {
            t = Repeat(t, length * 2f);
            return length - MathF.Abs(t - length);
        }

        public static bool IsNotZero(this float value) => MathF.Abs(value) > float.Epsilon;

        public static bool IsAboutZero(this float value) => MathF.Abs(value) < float.Epsilon;

        public static Quaternion AngleAxis(float aAngle, Float3 aAxis)
        {
            aAxis.Normalize();
            var rad = aAngle * Deg2Rad * 0.5f;
            aAxis *= MathF.Sin(rad);
            return new Quaternion(aAxis.X, aAxis.Y, aAxis.Z, MathF.Cos(rad));
        }

        public static Vector3 Rotate(Quaternion rotation, Vector3 point)
        {
            var x = rotation.X * 2F;
            var y = rotation.Y * 2F;
            var z = rotation.Z * 2F;
            var xx = rotation.X * x;
            var yy = rotation.Y * y;
            var zz = rotation.Z * z;
            var xy = rotation.X * y;
            var xz = rotation.X * z;
            var yz = rotation.Y * z;
            var wx = rotation.W * x;
            var wy = rotation.W * y;
            var wz = rotation.W * z;

            Vector3 res;
            res.X = (1F - (yy + zz)) * point.X + (xy - wz) * point.Y + (xz + wy) * point.Z;
            res.Y = (xy + wz) * point.X + (1F - (xx + zz)) * point.Y + (yz - wx) * point.Z;
            res.Z = (xz - wy) * point.X + (yz + wx) * point.Y + (1F - (xx + yy)) * point.Z;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Saturate(float value)
        {
            return Clamp01(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor Saturate(FloatColor value)
        {
            return new(Clamp01(value.R), Clamp01(value.G), Clamp01(value.B), Clamp01(value.A));
        }
    }
}