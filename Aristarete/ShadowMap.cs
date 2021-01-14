using System.Collections.Generic;
using Aristarete.Basic;
using Aristarete.Lighting;

namespace Aristarete
{
    public class ShadowMap
    {
        private readonly VertexProcessor _vertexProcessor = new();
        private readonly List<IRenderable> _meshes;
        private Light? _light;
        private readonly List<float> _map = new List<float>();
        private const int ShadowMapWidth = 800;
        private const int ShadowMapHeight = 600;
        private const float ShadowMapBias = 7.5f;

        public ShadowMap(List<IRenderable> meshes, Light light)
        {
            _meshes = meshes;
            _light = light;
            _vertexProcessor.SetOrthographic(ShadowMapWidth, ShadowMapHeight, 0.1f, 100);
            _vertexProcessor.SetLookAt(light.Position, new Float3(0, 0, 0), Float3.Up);
        }

        public void Render()
        {
            
        }
    }
}