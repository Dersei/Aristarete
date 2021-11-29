using System;

namespace Aristarete.Meshes
{
    public readonly struct Triangle
    {
        public readonly Vertex First;
        public readonly Vertex Second;
        public readonly Vertex Third;
        //public readonly Float3Sse Normal;

        public Triangle(Vertex first, Vertex second, Vertex third)
        {
            First = first;
            Second = second;
            Third = third;
            //Normal = (Second.Position - First.Position).Cross(Third.Position - First.Position).Normalize();
        }
        
        public Vertex this[int index] =>
            index switch
            {
                0 => First,
                1 => Second,
                2 => Third,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
    }
}