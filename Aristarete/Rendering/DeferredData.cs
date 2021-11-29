using Aristarete.Basic;


namespace Aristarete.Rendering
{
    public readonly struct DeferredData
    {
        public readonly Float3Sse Position;
        public readonly Float3Sse Normal;
        public readonly FloatColor Color;
        public readonly FloatColor Diffuse;
        public readonly FloatColor Specular;
        public readonly FloatColor Emission;

        public DeferredData(Float3Sse position, Float3Sse normal, FloatColor color, FloatColor diffuse, FloatColor specular, FloatColor emission)
        {
            Position = position;
            Normal = normal;
            Color = color;
            Diffuse = diffuse;
            Specular = specular;
            Emission = emission;
        }
    }
}