using Aristarete.Basic;
using Daeira;

namespace Aristarete.Lighting
{
    public interface IShadowMap
    {
        void Update();
        void Render();
        float PointInShadow(Float3 pointWorld);
        bool HardShadows { get; set; }
    }
}