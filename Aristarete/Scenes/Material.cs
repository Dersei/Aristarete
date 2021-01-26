using Aristarete.Basic;
using Aristarete.Cameras;
using Aristarete.Inputting;
using Aristarete.Lighting;
using Aristarete.Meshes;
using Aristarete.Rendering;
using Daeira;

namespace Aristarete.Scenes
{
    public class Material : Scene
    {
        
        private float _angleLeft;
        private float _angleUp;
        private float _oldAngleLeft;
        private float _oldAngleUp;
        private bool _isStopped;
        private readonly PerspectiveCamera _perspectiveCamera = new(new Float3(0, 0, 5), new Float3(0, 0, 0), Float3.Up,60, 2, 0.1f, 100);

      
        public Material(Buffer buffer)
        {
            Rasterizer = new ForwardRasterizer(buffer, this,_perspectiveCamera);


            AddMesh(new OctahedronSphere(6, 1)
                {
                    BasicColor = FloatColor.White,
                }
                .LoadDiffuseMap("_Resources/stylized-broken-ice/stylized-broken-ice-basecolor.jpg", 2)
                .LoadSpecularMap("_Resources/stylized-broken-ice/stylized-broken-ice-glossiness.jpg", 2)
                .LoadNormalMap("_Resources/stylized-broken-ice/stylized-broken-ice-normal.jpg", 2)
                .LoadHeightMap("_Resources/stylized-broken-ice/stylized-broken-ice-height.jpg", 2)
                .Scale(1f).Rotate(230, Float3.Up).Translate(Float3.Left * 2));
            
            AddMesh(new Sphere(1, 24, 24)
                {
                    BasicColor = FloatColor.White,
                    LightingMode = LightingMode.Pixel
                }
                .LoadDiffuseMap("_Resources/lava/lava-albedo.png", 2)
                .LoadSpecularMap("_Resources/lava/lava-smoothness.png", 2)
                .LoadEmissiveMap("_Resources/lava/lava-emission.png", 3, 2)
                .LoadNormalMap("_Resources/lava/lava-normals.png", 2)
                .Scale(0.7f).Translate(Float3.Right * 2));

            AddMesh(new Plane(6, 12, 5, 5)
                    {BasicColor = FloatColor.White, LightingMode = LightingMode.Pixel}.Rotate(-90, Float3.Right).Translate(Float3.Back * 2)
                // .LoadDiffuseMap("_Resources/Stylized_15_Grass/Stylized_15_Grass_basecolor.jpg", 4f)
                // .LoadSpecularMap("_Resources/Stylized_15_Grass/Stylized_15_Grass_glossiness.jpg", 4f)
                // .LoadNormalMap("_Resources/Stylized_15_Grass/Stylized_15_Grass_normal.jpg", 4f));
                // .LoadDiffuseMap("_Resources/castle/grass.png", 1f)
                );
            
            
            AddLight(new DirectionalLight
            {
                Position = Float3.Forward.Normalize(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.White,
                Specular = FloatColor.White,
                Shininess = 32
            });
            //
            // AddLight(new PointLight
            // {
            //     Position = Float3.Back * 5 + Float3.Right * 5,
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.Green,
            //     Specular = FloatColor.White,
            //     Shininess = 32
            // });
            // AddLight(new PointLight()
            // {
            //     Position =  Float3.Zero,
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.White,
            //     Specular = FloatColor.Black,
            //     Shininess = 32
            // });
            // //
            // AddLight(new SpotLight
            // {
            //     Position = new Float3(-2, 0, 5),
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.Red,
            //     Specular = FloatColor.White,
            //     Shininess = 32,
            //     Direction = (Float3.Forward),
            //     Angle = 5
            // });
            
           
            // //
            // // AddLight(new SpotLight
            // // {
            // //     Position = new Float3(0, 0, 5) + Float3.Right * 2,
            // //     Ambient = FloatColor.Black,
            // //     Diffuse = FloatColor.Green,
            // //     Specular = FloatColor.White,
            // //     Shininess = 32,
            // //     Direction = Float3.Forward,
            // //     Angle = 5
            // // });
            // //
            // // AddLight(new SpotLight
            // // {
            // //     Position = new Float3(0, 0, 5),
            // //     Ambient = FloatColor.Black,
            // //     Diffuse = FloatColor.Blue,
            // //     Specular = FloatColor.White,
            // //     Shininess = 32,
            // //     Direction = Float3.Forward,
            // //     Angle = 5
            // // });
            // //
            // // AddLight(new PointLight
            // // {
            // //     Position = Float3.Back * 5 + Float3.Right * 2,
            // //     Ambient = FloatColor.Black,
            // //     Diffuse = FloatColor.Blue,
            // //     Specular = FloatColor.UnityYellow,
            // //     Shininess = 32
            // // });
        }

        public override void Run()
        {
            if (Input.IsNewPress(Keys.A))
            {
                _angleLeft += 5f;
            }
            else if (Input.IsNewPress(Keys.D))
            {
                _angleLeft -= 5f;
            }
            else if (Input.IsNewPress(Keys.W))
            {
                _angleUp += 5f;
            }
            else if (Input.IsNewPress(Keys.S))
            {
                _angleUp -= 5f;
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
            // (Statics.Lights[1] as SpotLight).Direction = (Float3.Forward + Float3.Right * g1);
            // (Statics.Lights[2] as SpotLight).Position = new Float3(0, 0, 5) + 2 * Float3.Right + Float3.Up * g1;
            // (Statics.Lights[3] as SpotLight).Angle = g * 7 + 1;
            Rasterizer.CreateShadowMaps(_shadowUpdate);
            _shadowUpdate = false;
            Rasterizer.Render();
        }
        private bool _shadowUpdate = true;

        
    }
}