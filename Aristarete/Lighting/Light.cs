using Aristarete.Basic;
using Aristarete.Meshes;

namespace Aristarete.Lighting
{
    public abstract class Light
    {
        public Float3 Position;
        public FloatColor Ambient;
        public FloatColor Diffuse;
        public FloatColor Specular;
        public float Shininess;

        public abstract FloatColor Calculate(Vertex vertex, IRenderable renderable);
    }
}