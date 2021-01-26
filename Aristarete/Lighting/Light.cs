using Aristarete.Basic;
using Aristarete.Meshes;
using Aristarete.Rendering;
using Daeira;

namespace Aristarete.Lighting
{
    public abstract class Light
    {
        public Float3 Position;
        public FloatColor Ambient;
        public FloatColor Diffuse;
        public FloatColor Specular;
        public float Shininess;
        public Scene? Scene;

        public ShadowMap? ShadowMap;

        public virtual Light CreateShadowMap()
        {
            if (this is DirectionalLight or SpotLight)
                ShadowMap = new ShadowMap(this, Scene!);
            // if (this is PointLight)
            //     ShadowMap = new PointShadowMap(this, Scene);
            return this;
        }

        public virtual Light UpdateShadowMap()
        {
            ShadowMap?.Update();
            ShadowMap?.Render();
            return this;
        }

        public abstract FloatColor Calculate(Vertex vertex, Mesh renderable, Rasterizer rasterizer);

        public abstract FloatColor Calculate(DeferredData data, Rasterizer rasterizer);
    }
}