using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;
using Aristarete.Cameras;
using Aristarete.Lighting;
using Aristarete.Meshes;
using Aristarete.Meshes.Models;
using Aristarete.Rendering;
using Daeira;


namespace Aristarete.Scenes
{
    public class SolarMap : Scene
    {
        
        private readonly PerspectiveCamera _perspectiveCamera =
            new(new Float3(0, 5, 5), new Float3(0, 0, 0), Float3.Up, 45, 2, 0.1f, 100);

        private readonly Meshes.Models.Model _modelGreen = Meshes.Models.Model.LoadFromFile("_Resources/corvette/corvette.obj",
            new PbrMaterial(FloatColor.White,
                diffuseMap: new TextureInfo(Texture.LoadFrom("_Resources/corvette/textures/corvette-green.tif")),
                specularMap: new TextureInfo(Texture.LoadFrom("_Resources/corvette/textures/corvette-roughness.tif")),
                normalMap: new TextureInfo(Texture.LoadFrom("_Resources/corvette/textures/corvette-normal.tif"))
            ), 0.02f, Float3.Down);

        public SolarMap(Buffer buffer)
        {
            Rasterizer = new ForwardRasterizer(buffer, this,_perspectiveCamera);


            AddMesh(new OctahedronSphere(6, 1)
                {
                    BasicColor = FloatColor.White,
                }
                .LoadDiffuseMap("_Resources/earth/earth-basecolor.jpg", 1)
                .LoadEmissiveMap("_Resources/earth/earth-lights.jpg", 1)
                .LoadSpecularMap("_Resources/earth/earth-specular.jpg", 1)
                .LoadHeightMap("_Resources/earth/earth-bump.jpg", 1)
                .Scale(1f).Rotate(180, Float3.Up).Rotate(180, Float3.Forward).Translate(Float3.Left * 4));

            AddMesh(new OctahedronSphere(4, 1)
                {
                    BasicColor = FloatColor.White,
                    LightingMode = LightingMode.None
                }
                .LoadDiffuseMap("_Resources/sun-basecolor.jpg", 1)
                .LoadEmissiveMap("_Resources/sun-basecolor.jpg", 1, 1)
                .Scale(2).Rotate(90, Float3.Left).Translate(Float3.Right * 4 + Float3.Up));

            AddMesh(new RenderObject(_modelGreen).Rotate(-90, Float3.Up)
                .Translate(Float3.Up + Float3.Left));

            AddMesh(new Plane(12, 12, 64, 64)
                {
                    BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel
                }
                .LoadDiffuseMap("_Resources/wood/wood-basecolor.png", 2)
                .LoadSpecularMap("_Resources/wood/wood-roughness.png", 2)
                .LoadNormalMap("_Resources/wood/wood-normal.png", 2)
                .Translate(Float3.Down)
            );


            AddMesh(new OctahedronSphere(6, 1)
                {
                    BasicColor = FloatColor.White,
                }
                .LoadDiffuseMap("_Resources/stylized-broken-ice/stylized-broken-ice-basecolor.jpg", 2)
                .LoadSpecularMap("_Resources/stylized-broken-ice/stylized-broken-ice-glossiness.jpg", 2)
                .LoadNormalMap("_Resources/stylized-broken-ice/stylized-broken-ice-normal.jpg", 2)
                .LoadHeightMap("_Resources/stylized-broken-ice/stylized-broken-ice-height.jpg", 2)
                .Scale(0.5f).Rotate(90, Float3.Right).Translate(Float3.Left * 2 + Float3.Up * 2 + Float3.Forward * 3));

            AddLight(new DirectionalLight
            {
                Position = (Float3.Forward + Float3.Up / 2).Normalize(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.White,
                Specular = FloatColor.White,
                Shininess = 32
            });
        }

        public override void Run()
        {
            Rasterizer.CreateShadowMaps(_shadowUpdate);
            _shadowUpdate = false;
            Rasterizer.Render();

            //(rasterizer as DeferredRasterizer).RenderToScreen(DeferredRasterizer.BufferMode.Standard);
        }

        private bool _shadowUpdate = true;
    }
}