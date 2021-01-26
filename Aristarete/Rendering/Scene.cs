using System.Collections.Generic;
using Aristarete.Lighting;
using Aristarete.Meshes;

namespace Aristarete.Rendering
{
    public abstract class Scene
    {
        public readonly List<Mesh> Renderables;
        public readonly List<Light> Lights;
        public Rasterizer Rasterizer = null!;

        protected Scene()
        {
            Renderables = new List<Mesh>();
            Lights = new List<Light>();
        }

        protected Scene AddLight(Light light)
        {
            Lights.Add(light);
            return light.Scene = this;
        }

        protected Scene AddMesh(Mesh mesh)
        {
            Renderables.Add(mesh);
            return this;
        }

        public abstract void Run();
    }
}