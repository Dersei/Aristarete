using Aristarete.Basic;

using Matrix = Aristarete.Basic.Matrix;

namespace Aristarete.Cameras
{
    public abstract class Camera
    {
        public Matrix World2View;
        public Matrix View2Proj = Matrix.Identity;
        public Matrix View2ProjInv = Matrix.Identity;
        public Matrix LookAt;
        public Matrix LookAtInv;
        public Float3Sse Position;
        public Float3Sse Direction;
        public Float3Sse Up;
        public float Near;
        public float Far;

        protected Camera(Float3Sse position, Float3Sse direction, Float3Sse up, float near, float far)
        {
            Position = position;
            Direction = direction;
            Up = up;
            Near = near;
            Far = far;
            World2View = LookAt = Matrix.CreateLookAt(position, direction, up);
            Matrix.Invert(LookAt, out LookAtInv);
        }

        public void SetLookAt(Float3Sse eye, Float3Sse center, Float3Sse up)
        {
            LookAt = Matrix.CreateLookAt(eye, center, up);
            World2View *= LookAt;
        }

        public void SetIdentityToView()
        {
            World2View = Matrix.Identity;
        }

        public abstract Float3Sse ProjectTransformInv(Float2 pointProjected, float depth);

        public Float3Sse ViewTransformInv(Float3Sse pointCamera)
        {
            return LookAtInv.MultiplyPoint3X4(pointCamera);
        }

        public Float3Sse ProjectTransform(Float3Sse pointCamera)
        {
            return View2Proj.MultiplyPoint(pointCamera);
        }

// World space to camera/view space
        public Float3Sse ViewTransform(Float3Sse pointWorld)
        {
           return LookAt.MultiplyPoint(pointWorld);
        }
    }
}