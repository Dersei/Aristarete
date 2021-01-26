using Aristarete.Basic;

namespace Aristarete.Cameras
{
    public class PerspectiveCamera : Camera
    {
        public PerspectiveCamera(Float3 position, Float3 direction, Float3 up, float fieldOfView, float aspect, float zNear = 0.1f, float zFar = 100) : base(position, direction, up, zNear,zFar)
        {
            View2Proj = Matrix.CreatePerspective(fieldOfView, aspect, zNear, zFar);
            Matrix.Invert(View2Proj, out View2ProjInv);
        }
        
        public override Float3 ProjectTransformInv(Float2 pointProjected, float depth) {
            var point = new Float4(pointProjected.X * depth, pointProjected.Y * depth, depth, 1);
            var r = View2ProjInv * point;
            return new Float3(r.X, r.Y, r.Z * depth);
        }

    }
}