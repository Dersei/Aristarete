using System.Collections.Generic;
using System.Threading.Tasks;
using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;
using Aristarete.Extensions;
using Aristarete.Inputting;
using Aristarete.Lighting;
using Aristarete.Meshes;
using Aristarete.Models;

namespace Aristarete.Rendering
{
    public class CastleRendering : IRendering
    {
        private readonly VertexProcessor _vertexProcessor = new();
        private readonly List<IRenderable> _meshes = new();

        private readonly PbrMaterial _material = new(FloatColor.White,
            new TextureInfo(Texture.LoadFrom("_Resources/castle/ColorPaletteNormal.png")));

        public CastleRendering()
        {
            _vertexProcessor.SetPerspective(45, 2, 0.1f, 100);
            _vertexProcessor.SetLookAt(new Float3(0, 2, -5), new Float3(0, 0, 0), Float3.Up);
            Statics.VertexProcessor = _vertexProcessor;

            _meshes.Add(new Plane(_vertexProcessor, 10, 10, 2, 2)
                {BasicColor = FloatColor.White, LightingMode = LightingMode.None});
            
            _meshes.Add(new RenderObject(_vertexProcessor,Model.LoadFromFile("_Resources/castle/Tower_Window_W.obj", _material, 
                    0.1f, new Float3(0,0.2f,0)))
                .Translate(new Float3(2,0,0)));
            _meshes.Add(new RenderObject(_vertexProcessor,Model.LoadFromFile("_Resources/castle/Tower_Window_W.obj", _material, 
                    0.1f, new Float3(0,0.2f,0)))
                .Translate(new Float3(-2,0,0)));
            _meshes.Add(new RenderObject(_vertexProcessor,Model.LoadFromFile("_Resources/castle/Tower_Window_W.obj", _material, 
                    0.1f, new Float3(0,0.2f,0)))
                .Translate(new Float3(2,0,-2)));
            _meshes.Add(new RenderObject(_vertexProcessor,Model.LoadFromFile("_Resources/castle/Tower_Window_W.obj", _material, 
                    0.1f, new Float3(0,0.2f,0)))
                .Translate(new Float3(-2,0,-2)));
            
            Statics.Lights.Add(new DirectionalLight
            {
                Position = (Float3.Forward + Float3.Left + Float3.Down).Normalize(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.White,
                Specular = FloatColor.White,
                Shininess = 32
            });

            // Statics.Lights.Add(new SpotLight
            // {
            //     Position = new Float3(0, 0, 5),
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.Red,
            //     Specular = FloatColor.White,
            //     Shininess = 32,
            //     Direction = (Float3.Forward + Float3.Right / 50),
            //     Angle = 5,
            //     OuterConeAngle = 7
            // });
            //
            // Statics.Lights.Add(new SpotLight
            // {
            //     Position = new Float3(0, 0, 5) + Float3.Right * 2,
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.Green,
            //     Specular = FloatColor.White,
            //     Shininess = 32,
            //     Direction = Float3.Forward,
            //     Angle = 5,
            //     OuterConeAngle = 7
            // });
            //
            // Statics.Lights.Add(new SpotLight
            // {
            //     Position = new Float3(0, 0, 5),
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.Blue,
            //     Specular = FloatColor.White,
            //     Shininess = 32,
            //     Direction = Float3.Forward,
            //     Angle = 5,
            //     OuterConeAngle = 7
            // });

            // Statics.Lights.Add(new PointLight
            // {
            //     Position = Float3.Back * 5 + Float3.Right * 2,
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.Blue,
            //     Specular = FloatColor.UnityYellow,
            //     Shininess = 32
            // });
        }

        public void Run(Rasterizer rasterizer)
        {
            Parallel.ForEach(_meshes, mesh =>
            {
                mesh.Update(rasterizer);
            });
        }
    }
}