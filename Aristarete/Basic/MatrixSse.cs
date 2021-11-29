using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Aristarete.Basic
{
    public partial struct Matrix
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Float3Sse MultiplyVector(Float3Sse vector)
        {
            if (Sse.IsSupported)
            {
                fixed (Aristarete.Basic.Matrix* m = &this)
                {
                    return new Float3Sse(Sse.Add(Sse.Add(
                            Sse.Multiply(Sse.LoadVector128(&(*m).M11),
                                Vector128.Create(*(float*) &vector.Vector)),
                            Sse.Multiply(Sse.LoadVector128(&(*m).M12),
                                Vector128.Create(*((float*) &vector.Vector + 1)))),
                        Sse.Multiply(Sse.LoadVector128(&(*m).M13),
                            Vector128.Create(*((float*) &vector.Vector + 2)))));
                }
            }

            var resultX = M11 * vector.X + M12 * vector.Y + M13 * vector.Z;
            var resultY = M21 * vector.X + M22 * vector.Y + M23 * vector.Z;
            var resultZ = M31 * vector.X + M32 * vector.Y + M33 * vector.Z;
            return new Float3Sse(resultX, resultY, resultZ);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Float3Sse MultiplyVector(Aristarete.Basic.Matrix matrix, Float3Sse vector)
        {
            if (Sse.IsSupported)
            {
                return new Float3Sse(Sse.Add(Sse.Add(
                        Sse.Multiply(Sse.LoadAlignedVector128(&matrix.M11), Vector128.Create(*(float*) &vector.Vector)),
                        Sse.Multiply(Sse.LoadAlignedVector128(&matrix.M12),
                            Vector128.Create(*((float*) &vector.Vector + 1)))),
                    Sse.Multiply(Sse.LoadAlignedVector128(&matrix.M13),
                        Vector128.Create(*((float*) &vector.Vector + 2)))));
            }

            var resultX = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z;
            var resultY = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z;
            var resultZ = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z;
            return new Float3Sse(resultX, resultY, resultZ);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3Sse MultiplyPoint(Float3Sse point)
        {
            var x = point.X;
            var y = point.Y;
            var z = point.Z;

            var resultX = M11 * x + M12 * y + M13 * z + M14;
            var resultY = M21 * x + M22 * y + M23 * z + M24;
            var resultZ = M31 * x + M32 * y + M33 * z + M34;
            var w = M41 * x + M42 * y + M43 * z + M44;

            w = 1F / w;
            resultX *= w;
            resultY *= w;
            resultZ *= w;
            return new Float3Sse(resultX, resultY, resultZ);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3Sse MultiplyPoint3X4(Float3Sse point)
        {
            var x = point.X;
            var y = point.Y;
            var z = point.Z;
            var resultX = M11 * x + M12 * y + M13 * z + M14;
            var resultY = M21 * x + M22 * y + M23 * z + M24;
            var resultZ = M31 * x + M32 * y + M33 * z + M34;
            return new Float3Sse(resultX, resultY, resultZ);
        }

        public static Aristarete.Basic.Matrix Rotate(float angle, Float3Sse axis)
        {
            var s = MathF.Sin(angle * MathF.PI / 180);
            var c = MathF.Cos(angle * MathF.PI / 180);

            axis = axis.Normalize();
            var x = axis.X;
            var y = axis.Y;
            var z = axis.Z;

            return new Aristarete.Basic.Matrix(
                x * x * (1 - c) + c,
                y * x * (1 - c) + z * s,
                x * z * (1 - c) - y * s,
                0,
                x * y * (1 - c) - z * s,
                y * y * (1 - c) + c,
                y * z * (1 - c) + x * s,
                0,
                x * z * (1 - c) + y * s,
                y * z * (1 - c) - x * s,
                z * z * (1 - c) + c,
                0,
                0, 0, 0, 1);
        }

        public static Aristarete.Basic.Matrix Scale(Float3Sse vector)
        {
            Aristarete.Basic.Matrix m;
            m.M11 = vector.X;
            m.M12 = 0F;
            m.M13 = 0F;
            m.M14 = 0F;
            m.M21 = 0F;
            m.M22 = vector.Y;
            m.M23 = 0F;
            m.M24 = 0F;
            m.M31 = 0F;
            m.M32 = 0F;
            m.M33 = vector.Z;
            m.M34 = 0F;
            m.M41 = 0F;
            m.M42 = 0F;
            m.M43 = 0F;
            m.M44 = 1F;
            return m;
        }

        public static Aristarete.Basic.Matrix Translate(Float3Sse vector)
        {
            Aristarete.Basic.Matrix m;
            m.M11 = 1F;
            m.M12 = 0F;
            m.M13 = 0F;
            m.M14 = vector.X;
            m.M21 = 0F;
            m.M22 = 1F;
            m.M23 = 0F;
            m.M24 = vector.Y;
            m.M31 = 0F;
            m.M32 = 0F;
            m.M33 = 1F;
            m.M34 = vector.Z;
            m.M41 = 0F;
            m.M42 = 0F;
            m.M43 = 0F;
            m.M44 = 1F;
            return m;
        }

        public static Aristarete.Basic.Matrix CreateLookAt(Float3Sse cameraPosition, Float3Sse cameraTarget, Float3Sse cameraUpVector)
        {
            var zAxis = (cameraPosition - cameraTarget).Normalize();
            var xAxis = cameraUpVector.Cross(zAxis).Normalize();
            var yAxis = zAxis.Cross(xAxis);

            var result = Identity;

            result.M11 = xAxis.X;
            result.M21 = yAxis.X;
            result.M31 = zAxis.X;

            result.M12 = xAxis.Y;
            result.M22 = yAxis.Y;
            result.M32 = zAxis.Y;

            result.M13 = xAxis.Z;
            result.M23 = yAxis.Z;
            result.M33 = zAxis.Z;

            result.M14 = -Float3Sse.Dot(xAxis, cameraPosition);
            result.M24 = -Float3Sse.Dot(yAxis, cameraPosition);
            result.M34 = -Float3Sse.Dot(zAxis, cameraPosition);

            return result;
        }


        public static unsafe Float4Sse operator *(Aristarete.Basic.Matrix left, Float4Sse vector)
        {
            if (Sse.IsSupported)
            {
                Unsafe.SkipInit(out Float4Sse result);
                var valueX = Vector128.Create(vector.X);
                var valueY = Vector128.Create(vector.Y);
                var valueZ = Vector128.Create(vector.Z);
                var valueW = Vector128.Create(vector.W);
                Sse.Store((float*)&result, Sse.Add(Sse.Add(Sse.Add(
                            Sse.Multiply(Sse.LoadVector128(&left.M11), valueX),
                            Sse.Multiply(Sse.LoadVector128(&left.M12), valueY)),
                        Sse.Multiply(Sse.LoadVector128(&left.M13), valueZ)),
                    Sse.Multiply(Sse.LoadVector128(&left.M14), valueW)));
                return result;
            }

            var resultX = left.M11 * vector.X + left.M12 * vector.Y + left.M13 * vector.Z + left.M14 * vector.W;
            var resultY = left.M21 * vector.X + left.M22 * vector.Y + left.M23 * vector.Z + left.M24 * vector.W;
            var resultZ = left.M31 * vector.X + left.M32 * vector.Y + left.M33 * vector.Z + left.M34 * vector.W;
            var resultW = left.M41 * vector.X + left.M42 * vector.Y + left.M43 * vector.Z + left.M44 * vector.W;
            return new Float4Sse(resultX, resultY, resultZ, resultW);
        }
    }
}