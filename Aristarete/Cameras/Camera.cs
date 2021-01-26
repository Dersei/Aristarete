using Aristarete.Basic;
using Daeira;

namespace Aristarete.Cameras
{
    public abstract class Camera
    {
        public Matrix World2View;
        public Matrix View2Proj = Matrix.Identity;
        public Matrix View2ProjInv = Matrix.Identity;
        public Matrix LookAt;
        public Matrix LookAtInv;
        public Float3 Position;
        public Float3 Direction;
        public Float3 Up;
        public float Near;
        public float Far;

        protected Camera(Float3 position, Float3 direction, Float3 up, float near, float far)
        {
            Position = position;
            Direction = direction;
            Up = up;
            Near = near;
            Far = far;
            World2View = LookAt = Matrix.CreateLookAt(position, direction, up);
            Matrix.Invert(LookAt, out LookAtInv);
        }

        public void SetLookAt(Float3 eye, Float3 center, Float3 up)
        {
            LookAt = Matrix.CreateLookAt(eye, center, up);
            World2View *= LookAt;
        }

        public void SetIdentityToView()
        {
            World2View = Matrix.Identity;
        }

        public abstract Float3 ProjectTransformInv(Float2 pointProjected, float depth);

        public Float3 ViewTransformInv(Float3 pointCamera)
        {
            return LookAtInv.MultiplyPoint3X4(pointCamera);
        }

        public Float3 ProjectTransform(Float3 pointCamera)
        {
            return View2Proj.MultiplyPoint(pointCamera);
        }

// World space to camera/view space
        public Float3 ViewTransform(Float3 pointWorld)
        {
           return LookAt.MultiplyPoint(pointWorld);
        }
    }
}