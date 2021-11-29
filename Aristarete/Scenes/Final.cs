using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;
using Aristarete.Cameras;
using Aristarete.Extensions;
using Aristarete.Inputting;
using Aristarete.Lighting;
using Aristarete.Meshes;
using Aristarete.Rendering;
using Daeira;


namespace Aristarete.Scenes
{
    public class Final : Scene
    {
        
        private float _angleLeft;
        private float _angleUp;
        private float _oldAngleLeft;
        private float _oldAngleUp;
        private bool _isStopped;
        private readonly PerspectiveCamera _perspectiveCamera = new(new Float3(0, 0, 5), new Float3(0, 0, 0), Float3.Up,45, 2, 0.1f, 100);

        public Final(Buffer buffer)
        {

            var circuitryMaterial = new PbrMaterial(FloatColor.White,
                Texture.LoadFrom("_Resources/circuitry/circuitry-albedo.png").ToInfo(1),
                Texture.LoadFrom("_Resources/circuitry/circuitry-emission.png").ToInfo(1),
                Texture.LoadFrom("_Resources/circuitry/circuitry-smoothness.png").ToInfo(1),
                Texture.LoadFrom("_Resources/circuitry/circuitry-normals.png").ToInfo(1));
            Rasterizer = new DeferredRasterizer(buffer, this,_perspectiveCamera);
            
            AddMesh(new Cube {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}
                .SetMaterial(circuitryMaterial)
                .Scale(0.3f).Rotate(45, Float3.Up).Translate(Float3.Left + Float3.Up));
            AddMesh(new Cube {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}
                .SetMaterial(circuitryMaterial)
                .Scale(0.3f).Rotate(45, Float3.Up).Translate(Float3.Left + Float3.Down));
            AddMesh(new Cube {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}
                .SetMaterial(circuitryMaterial)
                .Scale(0.3f).Rotate(45, Float3.Up).Translate(Float3.Right + Float3.Up));
            AddMesh(new Cube {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}
                .SetMaterial(circuitryMaterial)
                .Scale(0.3f).Rotate(45, Float3.Up).Translate(Float3.Right + Float3.Down));
            AddMesh(new Sphere {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}
                .LoadDiffuseMap("_Resources/lava/lava-albedo.png", 2)
                .LoadSpecularMap("_Resources/lava/lava-smoothness.png", 2)
                .LoadEmissiveMap("_Resources/lava/lava-emission.png", 3, 2)
                .LoadNormalMap("_Resources/lava/lava-normals.png", 2)
                .Scale(0.3f).Rotate(45, Float3.Up));
            AddMesh(new Cube {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}
                .SetMaterial(circuitryMaterial)
                .Scale(0.3f).Rotate(45, Float3.Up).Translate(Float3.Left * 2 + Float3.Up));
            AddMesh(new Cube {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}
                .SetMaterial(circuitryMaterial)
                .Scale(0.3f).Rotate(45, Float3.Up).Translate(Float3.Left * 2 + Float3.Down));
            AddMesh(new Cube {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}
                .SetMaterial(circuitryMaterial)
                .Scale(0.3f).Rotate(45, Float3.Up).Translate(Float3.Right * 2 + Float3.Up));
            AddMesh(new Cube {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}
                .SetMaterial(circuitryMaterial)
                .Scale(0.3f).Rotate(45, Float3.Up).Translate(Float3.Right * 2 + Float3.Down));

            AddMesh(new Plane(3, 3, 10, 10)
                {
                    BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel
                }
                .LoadDiffuseMap("_Resources/stylized-broken-ice/stylized-broken-ice-basecolor.jpg", 2)
                .LoadSpecularMap("_Resources/stylized-broken-ice/stylized-broken-ice-glossiness.jpg", 2)
                .LoadNormalMap("_Resources/stylized-broken-ice/stylized-broken-ice-normal.jpg", 2)
                .LoadHeightMap("_Resources/stylized-broken-ice/stylized-broken-ice-height.jpg", 2)
                .Rotate(-90, Float3.Right).Translate(Float3.Back));
            
            // AddLight(new DirectionalLight
            // {
            //     Position = Float3.Forward.Normalize(),
            //     Ambient = FloatColor.White/100f,
            //     Diffuse = FloatColor.Black,
            //     Specular = FloatColor.Black,
            //     Shininess = 32
            // });
            
            AddLight(new SpotLight
            {
                Position = new Float3(-7, 0, 0),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Red,
                Specular = FloatColor.White,
                Shininess = 32,
                Direction = (Float3.Left),
                Angle = 18,
                OuterConeAngle = 20
            });
            // //
            AddLight(new SpotLight
            {
                Position = new Float3(5,0,5),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Green,
                Specular = FloatColor.Green,
                Shininess = 32,
                Direction = (Float3.Forward + Float3.Right).Normalize(),
                Angle = 5,
                OuterConeAngle = 7
            });
            
            AddLight(new SpotLight
            {
                Position = new Float3(3,8,0),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.UnityYellow,
                Specular = FloatColor.UnityYellow,
                Shininess = 32,
                Direction = (Float3.Up).Normalize(),
                Angle = 8,
                OuterConeAngle = 10
            });
            //
            
            // AddLight(new PointLight
            // {
            //     Position =  Float3.Right * 2 + Float3.Forward * 3,
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.Green,
            //     Specular = FloatColor.Green,
            //     Shininess = 32
            // });
            //
            // AddLight(new PointLight
            // {
            //     Position =  Float3.Left * 2,
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.UnityYellow,
            //     Specular = FloatColor.UnityYellow,
            //     Shininess = 32
            // });

        }


        public override void Run()
        {
            if (Input.IsNewPress(Keys.A))
            {
                _angleLeft += 0.1f;
            }
            else if (Input.IsNewPress(Keys.D))
            {
                _angleLeft -= 0.1f;
            }
            else if (Input.IsNewPress(Keys.W))
            {
                _angleUp += 1f;
            }
            else if (Input.IsNewPress(Keys.S))
            {
                _angleUp -= 1f;
            }
            else if (Input.IsNewPress(Keys.Space))
            {
                if (_isStopped)
                {
                    _angleLeft = _oldAngleLeft;
                    _angleUp = _oldAngleUp;
                    _isStopped = false;
                }
                else
                {
                    _oldAngleLeft = _angleLeft;
                    _oldAngleUp = _angleUp;
                    _angleLeft = 0;
                    _angleUp = 0;
                    _isStopped = true;
                }
            }

            // var g = MathExtensions.PingPong((float) Time.RealGameTime.TotalMilliseconds / 1000, 1);
            // var g1 = MathExtensions.Lerp(-1, 1,
            //     MathExtensions.PingPong((float) Time.RealGameTime.TotalMilliseconds / 1000, 1));
            // g1 /= 4f;
            // (Statics.Lights[1] as SpotLight)!.Direction = Float3.Forward + Float3.Right * g1;
            // (Statics.Lights[2] as SpotLight)!.Position = new Float3(0, 0, 5) + 2 * Float3.Right + Float3.Up * g1;
            // (Statics.Lights[3] as SpotLight)!.Angle = g * 7 + 1;
            // (Statics.Lights[3] as SpotLight)!.OuterConeAngle = g * 7 + 3;
            // _perspectiveCamera.SetIdentityToView();
            // _perspectiveCamera.SetLookAt(_perspectiveCamera.Position, new Float3(0, _angleLeft, 0),Float3.Up);
           Rasterizer.CreateShadowMaps(_shadowUpdate);
            _shadowUpdate = false;
            
            // Parallel.ForEach(Statics.Renderables, mesh =>
            // {
            //    // mesh.Rotate(_angleLeft, Float3.Up);
            //     //mesh.Rotate(_angleUp, Float3.Left);
            //     //mesh.Transform();
            //     mesh.Update(rasterizer);
            // });
            Rasterizer.Render();


            // for (int i = 0; i < Statics.Renderables.Count; i++)
            // {
            //     Statics.Renderables[i].Update(rasterizer);
            // }
        }
        
        private bool _shadowUpdate = true;
    }
}