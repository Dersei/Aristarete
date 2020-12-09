using Aristarete.Basic;

namespace Aristarete.Meshes
{
    public class Pyramid : Mesh
    {
        public Pyramid(VertexProcessor vertexProcessor) : base(vertexProcessor)
        {
            Create(1, 1, 1);
        }
        
        public Pyramid(VertexProcessor vertexProcessor, float length, float width, float height) : base(vertexProcessor)
        {
            Create(length, width, height);
        }

        private void Create(float length, float width, float height)
        {
            var widthOffset = width * 0.5f;
            var lengthOffset = length * 0.5f;
            var points = new Float3[]
            {
                new(-widthOffset, 0, -lengthOffset),
                new(widthOffset, 0, -lengthOffset),
                new(widthOffset, 0, lengthOffset),
                new(-widthOffset, 0, lengthOffset),
                new(0, height, 0)
            };

            Vertices = new Vertex[]
            {
                points[0], points[1], points[2],
                points[0], points[2], points[3],
                points[0], points[1], points[4],
                points[1], points[2], points[4],
                points[2], points[3], points[4],
                points[3], points[0], points[4]
            };

            Indices = new Int3[]
            {
                new(0, 1, 2),
                new(3, 4, 5),
                new(8, 7, 6),
                new(11, 10, 9),
                new(14, 13, 12),
                new(17, 16, 15)
            };
        }
    }
}