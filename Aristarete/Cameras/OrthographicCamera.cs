using Aristarete.Basic;

using Matrix = Aristarete.Basic.Matrix;

namespace Aristarete.Cameras
{
    public class OrthographicCamera : Camera
    {
        public OrthographicCamera(Float3Sse position, Float3Sse direction, Float3Sse up, float width, float height, float zNear = 0.1f, float zFar = 100) : base(position, direction, up, zNear, zFar)
        {
            View2Proj = Matrix.CreateOrthographic(width, height, zNear, zFar);
            Matrix.Invert(View2Proj, out View2ProjInv);
        }

        public OrthographicCamera(Float3Sse position, Float3Sse direction, Float3Sse up, float left, float right, float bottom,
            float top, float zNear = 0.1f, float zFar = 100) : base(position, direction, up, zNear, zFar)
        {
            View2Proj = Matrix.CreateOrthographicOffCenter(left, right, bottom, top, zNear, zFar);
            Matrix.Invert(View2Proj, out View2ProjInv);
        }
        
        public override Float3Sse ProjectTransformInv(Float2 pointProjected, float depth) {
            var point = new Float4Sse(pointProjected.X, pointProjected.Y, depth, 1);
            var r = View2ProjInv * point;
            return new Float3Sse(r.X, r.Y, depth);
        }
    }
}