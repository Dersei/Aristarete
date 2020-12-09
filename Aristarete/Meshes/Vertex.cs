using Aristarete.Basic;

namespace Aristarete.Meshes
{
    public struct Vertex
    {
        public Float3 Position;
        public Float3 Normal;

        public Vertex(float x, float y, float z)
        {
            Position = new Float3(x, y, z);
            Normal = Float3.Zero;
        }

        public static implicit operator Vertex(Float3 v) => new(v.X, v.Y, v.Z);
    }
}