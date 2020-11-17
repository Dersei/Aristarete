using Aristarete.Basic;

namespace Aristarete
{
    public class VertexProcessor
    {
        public Matrix Object2World = Matrix.Identity;
        public Matrix World2View = Matrix.Identity;
        public Matrix View2Proj = Matrix.Identity;
        public Matrix Object2Projection = Matrix.Identity;
        public Matrix Object2View = Matrix.Identity;

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
        
        public void SetIdentity()
        {
            Object2World = Matrix.Identity;
        }

        public void Rotate(float angle, Float3 v) => Object2World = Matrix.Rotate(angle, v) * Object2World;

        public void Translate(Float3 v) => Object2World = Matrix.Translate(v) * Object2World;

        public void Scale(Float3 v) => Object2World = Matrix.Scale(v) * Object2World;

        public void Transform()
        {
            Object2View = World2View * Object2World;
            Object2Projection = View2Proj * Object2View;
        }

        public Float3 Apply(Float3 f) => Object2Projection.MultiplyPoint(f);
    }
}