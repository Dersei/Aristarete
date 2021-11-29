using Aristarete.Basic;


namespace Aristarete.Lighting
{
    public interface IShadowMap
    {
        void Update();
        void Render();
        float PointInShadow(Float3Sse pointWorld);
        bool HardShadows { get; set; }
    }
}