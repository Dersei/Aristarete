using Aristarete.Basic;
using Daeira;

namespace Aristarete.Rendering
{
    public class VertexProcessor
    {
        public Matrix World2View = Matrix.Identity;
        public Matrix View2Proj = Matrix.Identity;
        public Matrix LookAt = Matrix.Identity;

        public void SetPerspective(float fieldOfView, float aspect, float zNear, float zFar)
        {
            View2Proj = Matrix.CreatePerspective(fieldOfView, aspect, zNear, zFar);
        }

        public void SetOrthographic(float width, float height, float zNear, float zFar)
        {
            View2Proj = Matrix.CreateOrthographic(width, height, zNear, zFar);
        }

        public void SetOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane,
            float zFarPlane)
        {
            View2Proj = Matrix.CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane);
        }

        public void SetLookAt(Float3 eye, Float3 center, Float3 up)
        {
            LookAt = Matrix.CreateLookAt(eye, center, up);
            World2View = LookAt;
        }

        public void SetIdentityToView()
        {
            World2View = Matrix.Identity;
        }
    }
}