using Aristarete.Basic;

namespace Aristarete
{
    public class VertexProcessor
    {
        public Matrix World2View = Matrix.Identity;
        public Matrix View2Proj = Matrix.Identity;

        public void SetPerspective(float fieldOfView, float aspect, float zNear, float zFar)
        {
            View2Proj = Matrix.CreatePerspective(fieldOfView, aspect, zNear, zFar);
        }

        public void SetLookAt(Float3 eye, Float3 center, Float3 up)
        {
            World2View *= Matrix.CreateLookAt(eye,center,up);
        }

        public void SetIdentityToView()
        {
            World2View = Matrix.Identity;
        }
    }
}