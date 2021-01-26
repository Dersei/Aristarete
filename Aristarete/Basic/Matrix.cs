using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Aristarete.Extensions;

namespace Aristarete.Basic
{
    public struct Matrix : IEquatable<Matrix>, IFormattable
    {
        public float M11;
        public float M21;
        public float M31;
        public float M41;
        public float M12;
        public float M22;
        public float M32;
        public float M42;
        public float M13;
        public float M23;
        public float M33;
        public float M43;
        public float M14;
        public float M24;
        public float M34;
        public float M44;

        public Matrix(Float4 column1, Float4 column2, Float4 column3, Float4 column4)
        {
            M11 = column1.X;
            M12 = column2.X;
            M13 = column3.X;
            M14 = column4.X;
            M21 = column1.Y;
            M22 = column2.Y;
            M23 = column3.Y;
            M24 = column4.Y;
            M31 = column1.Z;
            M32 = column2.Z;
            M33 = column3.Z;
            M34 = column4.Z;
            M41 = column1.W;
            M42 = column2.W;
            M43 = column3.W;
            M44 = column4.W;
        }

        public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31,
            float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            M11 = m11;
            M21 = m21;
            M31 = m31;
            M41 = m41;
            M12 = m12;
            M22 = m22;
            M32 = m32;
            M42 = m42;
            M13 = m13;
            M23 = m23;
            M33 = m33;
            M43 = m43;
            M14 = m14;
            M24 = m24;
            M34 = m34;
            M44 = m44;
        }

        public Float4 GetColumn(int index)
        {
            switch (index)
            {
                case 0: return new Float4(M11, M21, M31, M41);
                case 1: return new Float4(M12, M22, M32, M42);
                case 2: return new Float4(M13, M23, M33, M43);
                case 3: return new Float4(M14, M24, M34, M44);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        // Returns a row of the matrix.
        public Float4 GetRow(int index)
        {
            switch (index)
            {
                case 0: return new Float4(M11, M12, M13, M14);
                case 1: return new Float4(M21, M22, M23, M24);
                case 2: return new Float4(M31, M32, M33, M34);
                case 3: return new Float4(M41, M42, M43, M44);
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
        }

        public void SetColumn(int index, Float4 column)
        {
            this[0, index] = column.X;
            this[1, index] = column.Y;
            this[2, index] = column.Z;
            this[3, index] = column.W;
        }

        // Sets a row of the matrix.
        public void SetRow(int index, Float4 row)
        {
            this[index, 0] = row.X;
            this[index, 1] = row.Y;
            this[index, 2] = row.Z;
            this[index, 3] = row.W;
        }

        public override bool Equals(object? other)
        {
            return other is Matrix matrix && Equals(matrix);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetColumn(0).GetHashCode(), GetColumn(1).GetHashCode(), GetColumn(2).GetHashCode(),
                GetColumn(3).GetHashCode());
        }

        public float this[int index]
        {
            get
            {
                return index switch
                {
                    0 => M11,
                    1 => M21,
                    2 => M31,
                    3 => M41,
                    4 => M12,
                    5 => M22,
                    6 => M32,
                    7 => M42,
                    8 => M13,
                    9 => M23,
                    10 => M33,
                    11 => M43,
                    12 => M14,
                    13 => M24,
                    14 => M34,
                    15 => M44,
                    _ => throw new IndexOutOfRangeException("Invalid matrix index!")
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        M11 = value;
                        break;
                    case 1:
                        M21 = value;
                        break;
                    case 2:
                        M31 = value;
                        break;
                    case 3:
                        M41 = value;
                        break;
                    case 4:
                        M12 = value;
                        break;
                    case 5:
                        M22 = value;
                        break;
                    case 6:
                        M32 = value;
                        break;
                    case 7:
                        M42 = value;
                        break;
                    case 8:
                        M13 = value;
                        break;
                    case 9:
                        M23 = value;
                        break;
                    case 10:
                        M33 = value;
                        break;
                    case 11:
                        M43 = value;
                        break;
                    case 12:
                        M14 = value;
                        break;
                    case 13:
                        M24 = value;
                        break;
                    case 14:
                        M34 = value;
                        break;
                    case 15:
                        M44 = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        public float this[int row, int column]
        {
            get => this[row + column * 4];
            set => this[row + column * 4] = value;
        }

        public bool Equals(Matrix other)
        {
            return GetColumn(0).Equals(other.GetColumn(0))
                   && GetColumn(1).Equals(other.GetColumn(1))
                   && GetColumn(2).Equals(other.GetColumn(2))
                   && GetColumn(3).Equals(other.GetColumn(3));
        }

        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F5";
            return
                @$"{M11.ToString(format, formatProvider)}    {M12.ToString(format, formatProvider)}    {M13.ToString(format, formatProvider)}    {M14.ToString(format, formatProvider)}
{M21.ToString(format, formatProvider)}        {M22.ToString(format, formatProvider)}    {M23.ToString(format, formatProvider)}    {M24.ToString(format, formatProvider)}
{M31.ToString(format, formatProvider)}    {M32.ToString(format, formatProvider)}    {M33.ToString(format, formatProvider)}    {M34.ToString(format, formatProvider)}
{M41.ToString(format, formatProvider)}    {M42.ToString(format, formatProvider)}    {M43.ToString(format, formatProvider)}    {M44.ToString(format, formatProvider)}{Environment.NewLine}";
        }

        public static unsafe Matrix operator *(Matrix left, Matrix right)
        {
            if (Sse.IsSupported)
            {
                var row = Sse.LoadVector128(&right.M11);
                Sse.Store(&right.M11,
                    Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&left.M11)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&left.M12))),
                        Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&left.M13)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&left.M14)))));
                row = Sse.LoadVector128(&right.M12);
                Sse.Store(&right.M12,
                    Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&left.M11)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&left.M12))),
                        Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&left.M13)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&left.M14)))));
                row = Sse.LoadVector128(&right.M13);
                Sse.Store(&right.M13,
                    Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&left.M11)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&left.M12))),
                        Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&left.M13)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&left.M14)))));
                row = Sse.LoadVector128(&right.M14);
                Sse.Store(&right.M14,
                    Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0x00), Sse.LoadVector128(&left.M11)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0x55), Sse.LoadVector128(&left.M12))),
                        Sse.Add(Sse.Multiply(Sse.Shuffle(row, row, 0xAA), Sse.LoadVector128(&left.M13)),
                            Sse.Multiply(Sse.Shuffle(row, row, 0xFF), Sse.LoadVector128(&left.M14)))));
                return right;
            }

            Matrix res;
            res.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
            res.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
            res.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
            res.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

            res.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
            res.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
            res.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
            res.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

            res.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
            res.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
            res.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
            res.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

            res.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
            res.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
            res.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
            res.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;

            return res;
        }

        public static unsafe Float4 operator *(Matrix matrix, Float4 vector)
        {
            if (Sse.IsSupported)
            {
                Float4 result = default;
                var valueX = Vector128.Create(vector.X);
                var valueY = Vector128.Create(vector.Y);
                var valueZ = Vector128.Create(vector.Z);
                var valueW = Vector128.Create(vector.W);
                Sse.Store(&result.X, Sse.Add(Sse.Add(Sse.Add(
                            Sse.Multiply(Sse.LoadVector128(&matrix.M11), valueX),
                            Sse.Multiply(Sse.LoadVector128(&matrix.M12), valueY)),
                        Sse.Multiply(Sse.LoadVector128(&matrix.M13), valueZ)),
                    Sse.Multiply(Sse.LoadVector128(&matrix.M14), valueW)));
                return result;
            }

            var resultX = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14 * vector.W;
            var resultY = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24 * vector.W;
            var resultZ = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34 * vector.W;
            var resultW = matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Z + matrix.M44 * vector.W;
            return new Float4(resultX, resultY, resultZ, resultW);
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            // Returns false in the presence of NaN values.
            return left.GetColumn(0) == right.GetColumn(0)
                   && left.GetColumn(1) == right.GetColumn(1)
                   && left.GetColumn(2) == right.GetColumn(2)
                   && left.GetColumn(3) == right.GetColumn(3);
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            // Returns true in the presence of NaN values.
            return !(left == right);
        }

        public Float3 MultiplyPoint(Float3 point)
        {
            var resultX = M11 * point.X + M12 * point.Y + M13 * point.Z + M14;
            var resultY = M21 * point.X + M22 * point.Y + M23 * point.Z + M24;
            var resultZ = M31 * point.X + M32 * point.Y + M33 * point.Z + M34;
            var w = M41 * point.X + M42 * point.Y + M43 * point.Z + M44;

            w = 1F / w;
            resultX *= w;
            resultY *= w;
            resultZ *= w;
            return new Float3(resultX, resultY, resultZ);
        }

        public Float3 MultiplyPoint3X4(Float3 point)
        {
            var resultX = M11 * point.X + M12 * point.Y + M13 * point.Z + M14;
            var resultY = M21 * point.X + M22 * point.Y + M23 * point.Z + M24;
            var resultZ = M31 * point.X + M32 * point.Y + M33 * point.Z + M34;
            return new Float3(resultX, resultY, resultZ);
        }

        public Float3 MultiplyVector(Float3 vector)
        {
            var resultX = M11 * vector.X + M12 * vector.Y + M13 * vector.Z;
            var resultY = M21 * vector.X + M22 * vector.Y + M23 * vector.Z;
            var resultZ = M31 * vector.X + M32 * vector.Y + M33 * vector.Z;
            return new Float3(resultX, resultY, resultZ);
        }

        public static unsafe Float3 MultiplyVector(Matrix matrix, Float3 vector)
        {
            Unsafe.SkipInit(out Float3 result);

            if (Sse.IsSupported)
            {
                Sse.Store(&result.X, Sse.Add(Sse.Add(
                        Sse.Multiply(Sse.LoadAlignedVector128(&matrix.M11), Vector128.Create(vector.X)),
                        Sse.Multiply(Sse.LoadAlignedVector128(&matrix.M12), Vector128.Create(vector.Y))),
                    Sse.Multiply(Sse.LoadAlignedVector128(&matrix.M13), Vector128.Create(vector.Z))));
                return result;
            }

            var resultX = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z;
            var resultY = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z;
            var resultZ = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z;
            return new Float3(resultX, resultY, resultZ);
        }


        public static Matrix Rotate(float angle, Float3 axis)
        {
            // Matrix result = Identity;
            //
            // float c = MathF.Cos(angle*MathExtensions.Deg2Rad);
            // float s = MathF.Sin(angle*MathExtensions.Deg2Rad);
            //
            // float y = axis.Y * (1 - c) + axis.Z * s;
            // float z = axis.Z * (1 - c) - axis.Y * s;
            //
            // // [  1  0  0  0 ]
            // // [  0  c  s  0 ]
            // // [  0 -s  c  0 ]
            // // [  0  y  z  1 ]
            //
            // result.M22 = c;
            // result.M32 = s;
            // result.M23 = -s;
            // result.M33 = c;
            // result.M24 = y;
            // result.M34 = z;
            //
            // return result;
            var s = MathF.Sin(angle * MathF.PI / 180);
            var c = MathF.Cos(angle * MathF.PI / 180);
            axis = axis.Normalize();
            return new Matrix(
                axis.X * axis.X * (1 - c) + c,
                axis.Y * axis.X * (1 - c) + axis.Z * s,
                axis.X * axis.Z * (1 - c) - axis.Y * s,
                0,
                axis.X * axis.Y * (1 - c) - axis.Z * s,
                axis.Y * axis.Y * (1 - c) + c,
                axis.Y * axis.Z * (1 - c) + axis.X * s,
                0,
                axis.X * axis.Z * (1 - c) + axis.Y * s,
                axis.Y * axis.Z * (1 - c) - axis.X * s,
                axis.Z * axis.Z * (1 - c) + c,
                0,
                0, 0, 0, 1);
        }

        public static Matrix Scale(Float3 vector)
        {
            Matrix m;
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


        public static Matrix Translate(Float3 vector)
        {
            Matrix m;
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

        public static Matrix CreateLookAt(Float3 cameraPosition, Float3 cameraTarget, Float3 cameraUpVector)
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

            result.M14 = -Float3.Dot(xAxis, cameraPosition);
            result.M24 = -Float3.Dot(yAxis, cameraPosition);
            result.M34 = -Float3.Dot(zAxis, cameraPosition);

            return result;
        }

        public static Matrix CreatePerspective(float fieldOfView, float aspectRatio, float nearPlaneDistance,
            float farPlaneDistance)
        {
            fieldOfView *= MathExtensions.Deg2Rad;
            var yScale = 1.0f / MathF.Tan(fieldOfView * 0.5f);
            var xScale = yScale / aspectRatio;

            Matrix result;

            result.M11 = xScale;
            result.M21 = result.M31 = result.M41 = 0.0f;

            result.M22 = yScale;
            result.M12 = result.M32 = result.M42 = 0.0f;

            result.M13 = result.M23 = 0.0f;
            var negFarRange = float.IsPositiveInfinity(farPlaneDistance)
                ? -1.0f
                : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M33 = negFarRange;
            result.M43 = -1.0f;

            result.M14 = result.M24 = result.M44 = 0.0f;
            result.M34 = nearPlaneDistance * negFarRange;

            return result;
        }

        /// <summary>Creates an orthographic perspective matrix from the given view volume dimensions.</summary>
        /// <param name="width">Width of the view volume.</param>
        /// <param name="height">Height of the view volume.</param>
        /// <param name="zNearPlane">Minimum Z-value of the view volume.</param>
        /// <param name="zFarPlane">Maximum Z-value of the view volume.</param>
        /// <returns>The orthographic projection matrix.</returns>
        public static Matrix CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
        {
            Matrix result = Identity;
            float zRange = zNearPlane - zFarPlane;
            result.M11 = 2.0f / width;
            result.M22 = 2.0f / height;
            result.M33 = 1.0f / zRange;
            result.M34 = zNearPlane / zRange;

            return Transpose(result);
        }

        /// <summary>Builds a customized, orthographic projection matrix.</summary>
        /// <param name="left">Minimum X-value of the view volume.</param>
        /// <param name="right">Maximum X-value of the view volume.</param>
        /// <param name="bottom">Minimum Y-value of the view volume.</param>
        /// <param name="top">Maximum Y-value of the view volume.</param>
        /// <param name="zNearPlane">Minimum Z-value of the view volume.</param>
        /// <param name="zFarPlane">Maximum Z-value of the view volume.</param>
        /// <returns>The orthographic projection matrix.</returns>
        public static Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top,
            float zNearPlane, float zFarPlane)
        {
            Matrix result = Identity;

            result.M11 = 2.0f / (right - left);

            result.M22 = 2.0f / (top - bottom);

            result.M33 = 1.0f / (zNearPlane - zFarPlane);

            result.M14 = (left + right) / (left - right);
            result.M24 = (top + bottom) / (bottom - top);
            result.M34 = zNearPlane / (zNearPlane - zFarPlane);

            return result;
        }

        public static unsafe Matrix Transpose(Matrix matrix)
        {
            if (Sse.IsSupported)
            {
                var row1 = Sse.LoadVector128(&matrix.M11);
                var row2 = Sse.LoadVector128(&matrix.M12);
                var row3 = Sse.LoadVector128(&matrix.M13);
                var row4 = Sse.LoadVector128(&matrix.M14);

                var l12 = Sse.UnpackLow(row1, row2);
                var l34 = Sse.UnpackLow(row3, row4);
                var h12 = Sse.UnpackHigh(row1, row2);
                var h34 = Sse.UnpackHigh(row3, row4);

                Sse.Store(&matrix.M11, Sse.MoveLowToHigh(l12, l34));
                Sse.Store(&matrix.M12, Sse.MoveHighToLow(l34, l12));
                Sse.Store(&matrix.M13, Sse.MoveLowToHigh(h12, h34));
                Sse.Store(&matrix.M14, Sse.MoveHighToLow(h34, h12));

                return matrix;
            }

            //
            Matrix result;

            result.M11 = matrix.M11;
            result.M21 = matrix.M12;
            result.M31 = matrix.M13;
            result.M41 = matrix.M14;
            result.M12 = matrix.M21;
            result.M22 = matrix.M22;
            result.M32 = matrix.M23;
            result.M42 = matrix.M24;
            result.M13 = matrix.M31;
            result.M23 = matrix.M32;
            result.M33 = matrix.M33;
            result.M43 = matrix.M34;
            result.M14 = matrix.M41;
            result.M24 = matrix.M42;
            result.M34 = matrix.M43;
            result.M44 = matrix.M44;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector128<float> Permute(Vector128<float> value, byte control)
        {
            if (Avx.IsSupported)
            {
                return Avx.Permute(value, control);
            }

            if (Sse.IsSupported)
            {
                return Sse.Shuffle(value, value, control);
            }

            // Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
            throw new PlatformNotSupportedException();
        }

        public static bool SoftwareFallback(Matrix matrix, out Matrix result)
        {
            float a = matrix.M11, b = matrix.M21, c = matrix.M31, d = matrix.M41;
            float e = matrix.M12, f = matrix.M22, g = matrix.M32, h = matrix.M42;
            float i = matrix.M13, j = matrix.M23, k = matrix.M33, l = matrix.M43;
            float m = matrix.M14, n = matrix.M24, o = matrix.M34, p = matrix.M44;

            var kpLo = k * p - l * o;
            var jpLn = j * p - l * n;
            var joKn = j * o - k * n;
            var ipLm = i * p - l * m;
            var ioKm = i * o - k * m;
            var inJm = i * n - j * m;

            var a11 = +(f * kpLo - g * jpLn + h * joKn);
            var a12 = -(e * kpLo - g * ipLm + h * ioKm);
            var a13 = +(e * jpLn - f * ipLm + h * inJm);
            var a14 = -(e * joKn - f * ioKm + g * inJm);

            var det = a * a11 + b * a12 + c * a13 + d * a14;

            if (MathF.Abs(det) < float.Epsilon)
            {
                result = new Matrix(float.NaN, float.NaN, float.NaN, float.NaN,
                    float.NaN, float.NaN, float.NaN, float.NaN,
                    float.NaN, float.NaN, float.NaN, float.NaN,
                    float.NaN, float.NaN, float.NaN, float.NaN);
                return false;
            }

            var invDet = 1.0f / det;

            result.M11 = a11 * invDet;
            result.M12 = a12 * invDet;
            result.M13 = a13 * invDet;
            result.M14 = a14 * invDet;

            result.M21 = -(b * kpLo - c * jpLn + d * joKn) * invDet;
            result.M22 = +(a * kpLo - c * ipLm + d * ioKm) * invDet;
            result.M23 = -(a * jpLn - b * ipLm + d * inJm) * invDet;
            result.M24 = +(a * joKn - b * ioKm + c * inJm) * invDet;

            var gpHo = g * p - h * o;
            var fpHn = f * p - h * n;
            var foGn = f * o - g * n;
            var epHm = e * p - h * m;
            var eoGm = e * o - g * m;
            var enFm = e * n - f * m;

            result.M31 = +(b * gpHo - c * fpHn + d * foGn) * invDet;
            result.M32 = -(a * gpHo - c * epHm + d * eoGm) * invDet;
            result.M33 = +(a * fpHn - b * epHm + d * enFm) * invDet;
            result.M34 = -(a * foGn - b * eoGm + c * enFm) * invDet;

            var glHk = g * l - h * k;
            var flHj = f * l - h * j;
            var fkGj = f * k - g * j;
            var elHi = e * l - h * i;
            var ekGi = e * k - g * i;
            var ejFi = e * j - f * i;

            result.M41 = -(b * glHk - c * flHj + d * fkGj) * invDet;
            result.M42 = +(a * glHk - c * elHi + d * ekGi) * invDet;
            result.M43 = -(a * flHj - b * elHi + d * ejFi) * invDet;
            result.M44 = +(a * fkGj - b * ekGi + c * ejFi) * invDet;

            return true;
        }

        //TODO: Check
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Invert(Matrix matrix, out Matrix result)
        {
            // This implementation is based on the DirectX Math Library XMMatrixInverse method
            // https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathMatrix.inl

            if (Sse.IsSupported)
            {
                return SseImpl(matrix, out result);
            }

            return SoftwareFallback(matrix, out result);

            static bool SseImpl(Matrix matrix, out Matrix result)
            {
                if (!Sse.IsSupported)
                {
                    // Redundant test so we won't prejit remainder of this method on platforms without SSE.
                    throw new PlatformNotSupportedException();
                }

                // Load the matrix values into rows
                var row1 = Sse.LoadVector128(&matrix.M11);
                var row2 = Sse.LoadVector128(&matrix.M12);
                var row3 = Sse.LoadVector128(&matrix.M13);
                var row4 = Sse.LoadVector128(&matrix.M14);

                // Transpose the matrix
                var vTemp1 = Sse.Shuffle(row1, row2, 0x44); //_MM_SHUFFLE(1, 0, 1, 0)
                var vTemp3 = Sse.Shuffle(row1, row2, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)
                var vTemp2 = Sse.Shuffle(row3, row4, 0x44); //_MM_SHUFFLE(1, 0, 1, 0)
                var vTemp4 = Sse.Shuffle(row3, row4, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)

                row1 = Sse.Shuffle(vTemp1, vTemp2, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
                row2 = Sse.Shuffle(vTemp1, vTemp2, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)
                row3 = Sse.Shuffle(vTemp3, vTemp4, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
                row4 = Sse.Shuffle(vTemp3, vTemp4, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)

                var v00 = Permute(row3, 0x50); //_MM_SHUFFLE(1, 1, 0, 0)
                var v10 = Permute(row4, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)
                var v01 = Permute(row1, 0x50); //_MM_SHUFFLE(1, 1, 0, 0)
                var v11 = Permute(row2, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)
                var v02 = Sse.Shuffle(row3, row1, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
                var v12 = Sse.Shuffle(row4, row2, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)

                var d0 = Sse.Multiply(v00, v10);
                var d1 = Sse.Multiply(v01, v11);
                var d2 = Sse.Multiply(v02, v12);

                v00 = Permute(row3, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)
                v10 = Permute(row4, 0x50); //_MM_SHUFFLE(1, 1, 0, 0)
                v01 = Permute(row1, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)
                v11 = Permute(row2, 0x50); //_MM_SHUFFLE(1, 1, 0, 0)
                v02 = Sse.Shuffle(row3, row1, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)
                v12 = Sse.Shuffle(row4, row2, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)

                // Note:  We use this expansion pattern instead of Fused Multiply Add
                // in order to support older hardware
                d0 = Sse.Subtract(d0, Sse.Multiply(v00, v10));
                d1 = Sse.Subtract(d1, Sse.Multiply(v01, v11));
                d2 = Sse.Subtract(d2, Sse.Multiply(v02, v12));

                // V11 = D0Y,D0W,D2Y,D2Y
                v11 = Sse.Shuffle(d0, d2, 0x5D); //_MM_SHUFFLE(1, 1, 3, 1)
                v00 = Permute(row2, 0x49); //_MM_SHUFFLE(1, 0, 2, 1)
                v10 = Sse.Shuffle(v11, d0, 0x32); //_MM_SHUFFLE(0, 3, 0, 2)
                v01 = Permute(row1, 0x12); //_MM_SHUFFLE(0, 1, 0, 2)
                v11 = Sse.Shuffle(v11, d0, 0x99); //_MM_SHUFFLE(2, 1, 2, 1)

                // V13 = D1Y,D1W,D2W,D2W
                var v13 = Sse.Shuffle(d1, d2, 0xFD); //_MM_SHUFFLE(3, 3, 3, 1)
                v02 = Permute(row4, 0x49); //_MM_SHUFFLE(1, 0, 2, 1)
                v12 = Sse.Shuffle(v13, d1, 0x32); //_MM_SHUFFLE(0, 3, 0, 2)
                var v03 = Permute(row3, 0x12); //_MM_SHUFFLE(0, 1, 0, 2)
                v13 = Sse.Shuffle(v13, d1, 0x99); //_MM_SHUFFLE(2, 1, 2, 1)

                var c0 = Sse.Multiply(v00, v10);
                var c2 = Sse.Multiply(v01, v11);
                var c4 = Sse.Multiply(v02, v12);
                var c6 = Sse.Multiply(v03, v13);

                // V11 = D0X,D0Y,D2X,D2X
                v11 = Sse.Shuffle(d0, d2, 0x4); //_MM_SHUFFLE(0, 0, 1, 0)
                v00 = Permute(row2, 0x9e); //_MM_SHUFFLE(2, 1, 3, 2)
                v10 = Sse.Shuffle(d0, v11, 0x93); //_MM_SHUFFLE(2, 1, 0, 3)
                v01 = Permute(row1, 0x7b); //_MM_SHUFFLE(1, 3, 2, 3)
                v11 = Sse.Shuffle(d0, v11, 0x26); //_MM_SHUFFLE(0, 2, 1, 2)

                // V13 = D1X,D1Y,D2Z,D2Z
                v13 = Sse.Shuffle(d1, d2, 0xa4); //_MM_SHUFFLE(2, 2, 1, 0)
                v02 = Permute(row4, 0x9e); //_MM_SHUFFLE(2, 1, 3, 2)
                v12 = Sse.Shuffle(d1, v13, 0x93); //_MM_SHUFFLE(2, 1, 0, 3)
                v03 = Permute(row3, 0x7b); //_MM_SHUFFLE(1, 3, 2, 3)
                v13 = Sse.Shuffle(d1, v13, 0x26); //_MM_SHUFFLE(0, 2, 1, 2)

                c0 = Sse.Subtract(c0, Sse.Multiply(v00, v10));
                c2 = Sse.Subtract(c2, Sse.Multiply(v01, v11));
                c4 = Sse.Subtract(c4, Sse.Multiply(v02, v12));
                c6 = Sse.Subtract(c6, Sse.Multiply(v03, v13));

                v00 = Permute(row2, 0x33); //_MM_SHUFFLE(0, 3, 0, 3)

                // V10 = D0Z,D0Z,D2X,D2Y
                v10 = Sse.Shuffle(d0, d2, 0x4A); //_MM_SHUFFLE(1, 0, 2, 2)
                v10 = Permute(v10, 0x2C); //_MM_SHUFFLE(0, 2, 3, 0)
                v01 = Permute(row1, 0x8D); //_MM_SHUFFLE(2, 0, 3, 1)

                // V11 = D0X,D0W,D2X,D2Y
                v11 = Sse.Shuffle(d0, d2, 0x4C); //_MM_SHUFFLE(1, 0, 3, 0)
                v11 = Permute(v11, 0x93); //_MM_SHUFFLE(2, 1, 0, 3)
                v02 = Permute(row4, 0x33); //_MM_SHUFFLE(0, 3, 0, 3)

                // V12 = D1Z,D1Z,D2Z,D2W
                v12 = Sse.Shuffle(d1, d2, 0xEA); //_MM_SHUFFLE(3, 2, 2, 2)
                v12 = Permute(v12, 0x2C); //_MM_SHUFFLE(0, 2, 3, 0)
                v03 = Permute(row3, 0x8D); //_MM_SHUFFLE(2, 0, 3, 1)

                // V13 = D1X,D1W,D2Z,D2W
                v13 = Sse.Shuffle(d1, d2, 0xEC); //_MM_SHUFFLE(3, 2, 3, 0)
                v13 = Permute(v13, 0x93); //_MM_SHUFFLE(2, 1, 0, 3)

                v00 = Sse.Multiply(v00, v10);
                v01 = Sse.Multiply(v01, v11);
                v02 = Sse.Multiply(v02, v12);
                v03 = Sse.Multiply(v03, v13);

                var c1 = Sse.Subtract(c0, v00);
                c0 = Sse.Add(c0, v00);
                var c3 = Sse.Add(c2, v01);
                c2 = Sse.Subtract(c2, v01);
                var c5 = Sse.Subtract(c4, v02);
                c4 = Sse.Add(c4, v02);
                var c7 = Sse.Add(c6, v03);
                c6 = Sse.Subtract(c6, v03);

                c0 = Sse.Shuffle(c0, c1, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
                c2 = Sse.Shuffle(c2, c3, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
                c4 = Sse.Shuffle(c4, c5, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
                c6 = Sse.Shuffle(c6, c7, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)

                c0 = Permute(c0, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
                c2 = Permute(c2, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
                c4 = Permute(c4, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
                c6 = Permute(c6, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)

                // Get the determinant
                vTemp2 = row1;
                var det = Vector4.Dot(c0.AsVector4(), vTemp2.AsVector4());

                // Check determinate is not zero
                if (MathF.Abs(det) < float.Epsilon)
                {
                    result = new Matrix(float.NaN, float.NaN, float.NaN, float.NaN,
                        float.NaN, float.NaN, float.NaN, float.NaN,
                        float.NaN, float.NaN, float.NaN, float.NaN,
                        float.NaN, float.NaN, float.NaN, float.NaN);
                    return false;
                }

                // Create Vector128<float> copy of the determinant and invert them.
                var ones = Vector128.Create(1.0f);
                var vTemp = Vector128.Create(det);
                vTemp = Sse.Divide(ones, vTemp);

                row1 = Sse.Multiply(c0, vTemp);
                row2 = Sse.Multiply(c2, vTemp);
                row3 = Sse.Multiply(c4, vTemp);
                row4 = Sse.Multiply(c6, vTemp);

                Unsafe.SkipInit(out result);
                ref var vResult = ref Unsafe.As<Matrix, Vector128<float>>(ref result);

                vResult = row1;
                Unsafe.Add(ref vResult, 1) = row2;
                Unsafe.Add(ref vResult, 2) = row3;
                Unsafe.Add(ref vResult, 3) = row4;

                return true;
            }
        }

        public static readonly Matrix Zero = new Matrix(new Float4(0, 0, 0, 0),
            new Float4(0, 0, 0, 0),
            new Float4(0, 0, 0, 0),
            new Float4(0, 0, 0, 0));

        public static readonly Matrix Identity = new Matrix(new Float4(1, 0, 0, 0),
            new Float4(0, 1, 0, 0),
            new Float4(0, 0, 1, 0),
            new Float4(0, 0, 0, 1));
    }
}