using Aristarete.Basic;


namespace Aristarete.Meshes
{
    public struct Vertex
    {
        public Float3Sse Position;
        public Float3Sse WorldPosition;
        public Float3Sse Normal;
        public Float2 UV;

        public Vertex(float x, float y, float z)
        {
            Position = new Float3Sse(x, y, z);
            Normal = Float3Sse.Zero;
            UV = Float2.Zero;
            WorldPosition = Float3Sse.Zero;
        }
        
        public Vertex(Float3Sse position, Float3Sse normal, Float2 uv)
        {
            Position = position;
            Normal = normal;
            UV = uv;
            WorldPosition = Float3Sse.Zero;
        }

        public static implicit operator Vertex(Float3Sse v) => new(v.X, v.Y, v.Z);
    }
}