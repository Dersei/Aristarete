namespace Aristarete.Meshes
{
    public readonly struct Triangle
    {
        public readonly Vertex First;
        public readonly Vertex Second;
        public readonly Vertex Third;

        public Triangle(Vertex first, Vertex second, Vertex third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }
}