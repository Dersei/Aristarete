using System;
using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;
using Aristarete.Cameras;
using Aristarete.Extensions;
using Aristarete.Lighting;
using Aristarete.Meshes;
using Aristarete.Meshes.Models;
using Aristarete.Rendering;
using Daeira;
using Buffer = Aristarete.Rendering.Buffer;

namespace Aristarete.Scenes
{
    public class Castle : Scene
    {
        private readonly PerspectiveCamera _perspectiveCamera =
            new(new Float3(0, 5, 5), new Float3(0, 0, 0), Float3.Up, 45, 2, 0.1f, 100);

        private readonly Random _random = new();

        private readonly PbrMaterial _castleMaterial = new(FloatColor.White,
            new TextureInfo(Texture.LoadFrom("_Resources/castle/textures/color-palette.png")));

        private readonly PbrMaterial _castleMaterial2 = new(FloatColor.White,
            new TextureInfo(Texture.LoadFrom("_Resources/castle/textures/color-palette-2.png")));

        public Castle(Buffer buffer)
        {
            Rasterizer = new ForwardRasterizer(buffer, this,_perspectiveCamera);

            AddMesh(new Plane(5, 5, 5, 5)
                {
                    BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel
                }
                .LoadDiffuseMap("_Resources/castle/textures/grass.png", 1f));
            AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile("_Resources/castle/tower-window.obj",
                    _castleMaterial,
                    0.1f, new Float3(0, 0f, 0)))
                .Translate(new Float3(2, 0, 1)));
            AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile("_Resources/castle/tower-window.obj",
                    _castleMaterial,
                    0.1f, new Float3(0, 0f, 0)))
                .Translate(new Float3(-2, 0, 1)));
            AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile("_Resources/castle/tower-window.obj",
                    _castleMaterial,
                    0.1f, new Float3(0, 0f, 0)))
                .Translate(new Float3(2, 0, -1)));
            AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile("_Resources/castle/tower-window.obj",
                    _castleMaterial,
                    0.1f, new Float3(0, 0f, 0)))
                .Translate(new Float3(-2, 0, -1)));
            AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile("_Resources/castle/castle-base.obj", _castleMaterial,
                    0.1f, new Float3(0, 0f, 0)))
                .Translate(new Float3(0, 0, 0)));
            AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile("_Resources/castle/winter-castle.obj",
                    _castleMaterial,
                    0.1f, new Float3(0, 0f, 0)))
                .Rotate(90, Float3.Up).Translate(new Float3(-1.5f, 0, -0.25f)));
            AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile("_Resources/castle/winter-tower.obj",
                    _castleMaterial,
                    0.1f, new Float3(0, 0f, 0)))
                .Translate(new Float3(1f, 0, -0.75f)));
            AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile("_Resources/castle/winter-tower.obj",
                    _castleMaterial,
                    0.1f, new Float3(0, 0f, 0)))
                .Translate(new Float3(1.25f, 0, 0)));


            for (var i = 0; i < 6; i++)
            {
                for (var j = 0; j < 6; j++)
                {
                    AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile(
                            "_Resources/castle/pine.obj", _castleMaterial2,
                            0.1f, new Float3(0, 0f, 0)))
                        .Translate(new Float3(_random.NextFloat(-2, 2), 0, _random.NextFloat(-2, 2))));
                }
            }

            for (var i = 0; i < 6; i++)
            {
                for (var j = 0; j < 6; j++)
                {
                    AddMesh(new RenderObject(Meshes.Models.Model.LoadFromFile("_Resources/castle/tree.obj",
                            _castleMaterial2,
                            0.1f, new Float3(0, 0f, 0)))
                        .Translate(new Float3(_random.NextFloat(-2, 2), 0,
                            _random.NextFloatWithSpace(-2f, -1.1f, 1.1f, 2.5f))));
                }
            }

            AddMesh(
                new Cube(4, 0.3f, 0.1f)
                    {
                        BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel
                    }
                    .Translate(new Float3(0, 0.15f, 1)));
            
            AddMesh(
                new Cube(4, 0.3f, 0.1f)
                    {
                        BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel
                    }
                    .Translate(new Float3(0, 0.15f, -1)));
            
            AddMesh(
                new Cube(2, 0.3f, 0.1f)
                    {
                        BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel
                    }
                    .Rotate(90, Float3.Up).Translate(new Float3(-2, 0.15f, 0)));
            
            AddMesh(
                new Cube(2, 0.3f, 0.1f)
                    {
                        BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel
                    }
                    .Rotate(90, Float3.Up).Translate(new Float3(2, 0.15f, 0)));
            
            AddLight(new DirectionalLight
            {
                Position = (Float3.Up + Float3.Left + Float3.Forward * 2).Normalize(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.White,
                Specular = FloatColor.White,
                Shininess = 32
            });
        }

        private bool _shadowUpdate = true;

        public  override void Run()
        {
            Rasterizer.CreateShadowMaps(_shadowUpdate);
            _shadowUpdate = false;
            Rasterizer.Render();
        }
    }
}