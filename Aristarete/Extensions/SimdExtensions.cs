using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Aristarete.Extensions
{
    public static class SimdExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equal(Vector128<float> vector1, Vector128<float> vector2)
        {
            // This implementation is based on the DirectX Math Library XMVector4Equal method
            // https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathVector.inl

            if (Sse.IsSupported)
            {
                return Sse.MoveMask(Sse.CompareNotEqual(vector1, vector2)) == 0;
            }
            // Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
            throw new PlatformNotSupportedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEqual(Vector128<float> vector1, Vector128<float> vector2)
        {
            // This implementation is based on the DirectX Math Library XMVector4NotEqual method
            // https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathVector.inl

            if (Sse.IsSupported)
            {
                return Sse.MoveMask(Sse.CompareNotEqual(vector1, vector2)) != 0;
            }
            // Redundant test so we won't prejit remainder of this method on platforms without AdvSimd.
            throw new PlatformNotSupportedException();
        }
    }
}