using Aristarete.Basic;
using Daeira;

namespace Aristarete.Meshes
{
    public struct Vertex
    {
        public Float3 Position;
        public Float3 WorldPosition;
        public Float3 Normal;
        public Float2 UV;

        public Vertex(float x, float y, float z)
        {
            Position = new Float3(x, y, z);
            Normal = Float3.Zero;
            UV = Float2.Zero;
            WorldPosition = Float3.Zero;
        }
        
        public Vertex(Float3 position, Float3 normal, Float2 uv)
        {
            Position = position;
            Normal = normal;
            UV = uv;
            WorldPosition = Float3.Zero;
        }

        public static implicit operator Vertex(Float3 v) => new(v.X, v.Y, v.Z);
    }
}