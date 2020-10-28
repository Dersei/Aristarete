using System;
using System.Globalization;
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

        public Matrix(float m11, float m21, float m31, float m41, float m12, float m22, float m32, float m42, float m13,
            float m23, float m33, float m43, float m14, float m24, float m34, float m44)
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

        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            Matrix res;
            res.M11 = lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21 + lhs.M13 * rhs.M31 + lhs.M14 * rhs.M41;
            res.M12 = lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22 + lhs.M13 * rhs.M32 + lhs.M14 * rhs.M42;
            res.M13 = lhs.M11 * rhs.M13 + lhs.M12 * rhs.M23 + lhs.M13 * rhs.M33 + lhs.M14 * rhs.M43;
            res.M14 = lhs.M11 * rhs.M14 + lhs.M12 * rhs.M24 + lhs.M13 * rhs.M34 + lhs.M14 * rhs.M44;

            res.M21 = lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21 + lhs.M23 * rhs.M31 + lhs.M24 * rhs.M41;
            res.M22 = lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22 + lhs.M23 * rhs.M32 + lhs.M24 * rhs.M42;
            res.M23 = lhs.M21 * rhs.M13 + lhs.M22 * rhs.M23 + lhs.M23 * rhs.M33 + lhs.M24 * rhs.M43;
            res.M24 = lhs.M21 * rhs.M14 + lhs.M22 * rhs.M24 + lhs.M23 * rhs.M34 + lhs.M24 * rhs.M44;

            res.M31 = lhs.M31 * rhs.M11 + lhs.M32 * rhs.M21 + lhs.M33 * rhs.M31 + lhs.M34 * rhs.M41;
            res.M32 = lhs.M31 * rhs.M12 + lhs.M32 * rhs.M22 + lhs.M33 * rhs.M32 + lhs.M34 * rhs.M42;
            res.M33 = lhs.M31 * rhs.M13 + lhs.M32 * rhs.M23 + lhs.M33 * rhs.M33 + lhs.M34 * rhs.M43;
            res.M34 = lhs.M31 * rhs.M14 + lhs.M32 * rhs.M24 + lhs.M33 * rhs.M34 + lhs.M34 * rhs.M44;

            res.M41 = lhs.M41 * rhs.M11 + lhs.M42 * rhs.M21 + lhs.M43 * rhs.M31 + lhs.M44 * rhs.M41;
            res.M42 = lhs.M41 * rhs.M12 + lhs.M42 * rhs.M22 + lhs.M43 * rhs.M32 + lhs.M44 * rhs.M42;
            res.M43 = lhs.M41 * rhs.M13 + lhs.M42 * rhs.M23 + lhs.M43 * rhs.M33 + lhs.M44 * rhs.M43;
            res.M44 = lhs.M41 * rhs.M14 + lhs.M42 * rhs.M24 + lhs.M43 * rhs.M34 + lhs.M44 * rhs.M44;

            return res;
        }

        public static Float4 operator *(Matrix lhs, Float4 vector)
        {
            var resx = lhs.M11 * vector.X + lhs.M12 * vector.Y + lhs.M13 * vector.Z + lhs.M14 * vector.W;
            var resy = lhs.M21 * vector.X + lhs.M22 * vector.Y + lhs.M23 * vector.Z + lhs.M24 * vector.W;
            var resz = lhs.M31 * vector.X + lhs.M32 * vector.Y + lhs.M33 * vector.Z + lhs.M34 * vector.W;
            var resw = lhs.M41 * vector.X + lhs.M42 * vector.Y + lhs.M43 * vector.Z + lhs.M44 * vector.W;
            return new Float4(resx, resy, resz, resw);
        }

        public static bool operator ==(Matrix lhs, Matrix rhs)
        {
            // Returns false in the presence of NaN values.
            return lhs.GetColumn(0) == rhs.GetColumn(0)
                   && lhs.GetColumn(1) == rhs.GetColumn(1)
                   && lhs.GetColumn(2) == rhs.GetColumn(2)
                   && lhs.GetColumn(3) == rhs.GetColumn(3);
        }

        //*undoc*
        public static bool operator !=(Matrix lhs, Matrix rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        public Float3 MultiplyPoint(Float3 point)
        {
            float w;
            var resx = M11 * point.X + M12 * point.Y + M13 * point.Z + M14;
            var resy = M21 * point.X + M22 * point.Y + M23 * point.Z + M24;
            var resz = M31 * point.X + M32 * point.Y + M33 * point.Z + M34;
            w = M41 * point.X + M42 * point.Y + M43 * point.Z + M44;

            w = 1F / w;
            resx *= w;
            resy *= w;
            resz *= w;
            return new Float3(resx, resy, resz);
        }

        public Float3 MultiplyPoint3X4(Float3 point)
        {
            var resx = M11 * point.X + M12 * point.Y + M13 * point.Z + M14;
            var resy = M21 * point.X + M22 * point.Y + M23 * point.Z + M24;
            var resz = M31 * point.X + M32 * point.Y + M33 * point.Z + M34;
            return new Float3(resx, resy, resz);
        }

        public Float3 MultiplyVector(Float3 vector)
        {
            var resx = M11 * vector.X + M12 * vector.Y + M13 * vector.Z;
            var resy = M21 * vector.X + M22 * vector.Y + M23 * vector.Z;
            var resz = M31 * vector.X + M32 * vector.Y + M33 * vector.Z;
            return new Float3(resx, resy, resz);
        }

        public static Matrix Rotate(float angle, Float3 axis)
        {
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
            var zaxis = (cameraPosition - cameraTarget).Normalize();
            var xaxis = cameraUpVector.Cross(zaxis).Normalize();
            var yaxis = zaxis.Cross(xaxis);
 
            var result = Identity;
 
            result.M11 = xaxis.X;
            result.M21 = yaxis.X;
            result.M31 = zaxis.X;
 
            result.M12 = xaxis.Y;
            result.M22 = yaxis.Y;
            result.M32 = zaxis.Y;
 
            result.M13 = xaxis.Z;
            result.M23 = yaxis.Z;
            result.M33 = zaxis.Z;
 
            result.M14 = -Float3.Dot(xaxis, cameraPosition);
            result.M24 = -Float3.Dot(yaxis, cameraPosition);
            result.M34 = -Float3.Dot(zaxis, cameraPosition);
 
            return result;
        }
        
        public static Matrix CreatePerspective(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
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
            var negFarRange = float.IsPositiveInfinity(farPlaneDistance) ? -1.0f : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M33 = negFarRange;
            result.M43 = -1.0f;
 
            result.M14 = result.M24 = result.M44 = 0.0f;
            result.M34 = nearPlaneDistance * negFarRange;
 
            return result;
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