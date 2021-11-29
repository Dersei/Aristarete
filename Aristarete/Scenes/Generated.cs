using Aristarete.Cameras;
using Daeira.Extensions;
using Aristarete.Inputting;
using Aristarete.Lighting;
using Aristarete.Meshes;
using Aristarete.Rendering;
using Aristarete.Utils;
using Daeira;


namespace Aristarete.Scenes
{
    public class Generated : Scene
    {
       
        private float _angleLeft;
        private float _angleUp;
        private float _oldAngleLeft;
        private float _oldAngleUp;
        private bool _isStopped;
        private readonly PerspectiveCamera _perspectiveCamera = new(new Float3(0, 0, 5), new Float3(0, 0, 0), Float3.Up,45, 2, 0.1f, 100);

        public Generated(Buffer buffer)
        {
            Rasterizer = new ForwardRasterizer(buffer, this,_perspectiveCamera);

            for (var i = -1; i <= 1; i += 2)
            {
                AddMesh(new Sphere(1, 24, 24)
                        {BasicColor = FloatColor.White, LightingMode = (LightingMode) (i + 1)}
                    .LoadDiffuseMap("_Resources/texel_density.png")
                    .Scale(0.3f).Translate(Float3.Up * i + Float3.Left * -2.5f));
                AddMesh(new Torus()
                        {BasicColor = FloatColor.White, LightingMode = (LightingMode) (i + 1)}
                    .LoadDiffuseMap("_Resources/texel_density.png")
                    .Scale(0.3f).Translate(Float3.Up * i + Float3.Left * -1.5f));
                AddMesh(new Cube()
                        {BasicColor = FloatColor.White, LightingMode = (LightingMode) (i + 1)}
                    .LoadDiffuseMap("_Resources/texel_density.png")
                    .Scale(0.3f).Rotate(45, Float3.Up + Float3.Right).Translate(Float3.Up * i + Float3.Left * -0.5f));
                AddMesh(new Tube()
                        {BasicColor = FloatColor.White, LightingMode = (LightingMode) (i + 1)}
                    .LoadDiffuseMap("_Resources/texel_density.png")
                    .Scale(0.3f).Rotate(45, Float3.Right).Translate(Float3.Up * i + Float3.Left * 0.5f));
                AddMesh(new Cone()
                        {BasicColor = FloatColor.White, LightingMode = (LightingMode) (i + 1)}
                    .LoadDiffuseMap("_Resources/texel_density.png")
                    .Scale(0.3f).Rotate(30, Float3.Left).Translate(Float3.Up * i + Float3.Left * 1.5f));
                AddMesh(new Cylinder()
                        {BasicColor = FloatColor.White, LightingMode = (LightingMode) (i + 1)}
                    .LoadDiffuseMap("_Resources/texel_density.png")
                    .Scale(0.3f).Rotate(30, Float3.Left).Translate(Float3.Up * i + Float3.Left * 2.5f));
            }

            AddMesh(new Sphere(1, 24, 24) {BasicColor = FloatColor.White}
                .LoadDiffuseMap("_Resources/circuitry/circuitry-albedo.png", 2)
                .LoadSpecularMap("_Resources/circuitry/circuitry-smoothness.png", 2)
                .LoadEmissiveMap("_Resources/circuitry/circuitry-emission.png", scale: 2)
                .LoadNormalMap("_Resources/circuitry/circuitry-normals.png", 2)
                .Scale(0.3f).Translate(Float3.Left * -2.5f));
            AddMesh(new Torus() {BasicColor = FloatColor.White}
                .LoadDiffuseMap("_Resources/circuitry/circuitry-albedo.png", 2)
                .LoadSpecularMap("_Resources/circuitry/circuitry-smoothness.png", 2)
                .LoadEmissiveMap("_Resources/circuitry/circuitry-emission.png", scale: 2)
                .LoadNormalMap("_Resources/circuitry/circuitry-normals.png", 2)
                .Scale(0.3f).Translate(Float3.Left * -1.5f));
            AddMesh(new Cube() {BasicColor = FloatColor.White}
                .LoadDiffuseMap("_Resources/circuitry/circuitry-albedo.png", 2)
                .LoadSpecularMap("_Resources/circuitry/circuitry-smoothness.png", 2)
                .LoadEmissiveMap("_Resources/circuitry/circuitry-emission.png", scale: 2)
                .LoadNormalMap("_Resources/circuitry/circuitry-normals.png", 2)
                .Scale(0.3f).Rotate(45, Float3.Up + Float3.Right).Translate(Float3.Left * -0.5f));
            AddMesh(new Tube() {BasicColor = FloatColor.White}
                .LoadDiffuseMap("_Resources/circuitry/circuitry-albedo.png", 2)
                .LoadSpecularMap("_Resources/circuitry/circuitry-smoothness.png", 2)
                .LoadEmissiveMap("_Resources/circuitry/circuitry-emission.png", scale: 2)
                .LoadNormalMap("_Resources/circuitry/circuitry-normals.png", 2)
                .Scale(0.3f).Rotate(45, Float3.Right).Translate(Float3.Left * 0.5f));
            AddMesh(new Cone() {BasicColor = FloatColor.White}
                .LoadDiffuseMap("_Resources/circuitry/circuitry-albedo.png", 2)
                .LoadSpecularMap("_Resources/circuitry/circuitry-smoothness.png", 2)
                .LoadEmissiveMap("_Resources/circuitry/circuitry-emission.png", scale: 2)
                .LoadNormalMap("_Resources/circuitry/circuitry-normals.png", 2)
                .Scale(0.3f).Rotate(30, Float3.Left).Translate(Float3.Left * 1.5f));
            AddMesh(new Cylinder() {BasicColor = FloatColor.White}
                .LoadDiffuseMap("_Resources/circuitry/circuitry-albedo.png", 2)
                .LoadSpecularMap("_Resources/circuitry/circuitry-smoothness.png", 2)
                .LoadEmissiveMap("_Resources/circuitry/circuitry-emission.png", scale: 2)
                .LoadNormalMap("_Resources/circuitry/circuitry-normals.png", 2)
                .Scale(0.3f).Rotate(120, Float3.Up).Rotate(30, Float3.Left).Translate(Float3.Left * 2.5f));

            AddLight(new DirectionalLight
            {
                Position = Float3.Left.Normalize(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.White,
                Specular = FloatColor.White,
                Shininess = 32
            });

            AddLight(new SpotLight
            {
                Position = new Float3(0, 0, 5),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Red,
                Specular = FloatColor.White,
                Shininess = 32,
                Direction = Float3.Forward + Float3.Right / 50,
                Angle = 5,
                OuterConeAngle = 7
            });

            AddLight(new SpotLight
            {
                Position = new Float3(0, 0, 5) + Float3.Right * 2,
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Green,
                Specular = FloatColor.White,
                Shininess = 32,
                Direction = Float3.Forward,
                Angle = 5,
                OuterConeAngle = 7
            });

            AddLight(new SpotLight
            {
                Position = new Float3(0, 0, 5),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Blue,
                Specular = FloatColor.White,
                Shininess = 32,
                Direction = Float3.Forward,
                Angle = 5,
                OuterConeAngle = 7
            });

            // AddLight(new PointLight
            // {
            //     Position = Float3.Back * 5 + Float3.Right * 2,
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.Blue,
            //     Specular = FloatColor.UnityYellow,
            //     Shininess = 32
            // });
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

            var g = MathExtensions.PingPong((float) Time.RealGameTime.TotalMilliseconds / 1000, 1);
            var g1 = MathExtensions.Lerp(-1, 1,
                MathExtensions.PingPong((float) Time.RealGameTime.TotalMilliseconds / 1000, 1));
            g1 /= 4f;
            (Lights[1] as SpotLight)!.Direction = Float3.Forward + Float3.Right * g1;
            (Lights[2] as SpotLight)!.Position = new Float3(0, 0, 5) + 2 * Float3.Right + Float3.Up * g1;
            (Lights[3] as SpotLight)!.Angle = g * 7 + 1;
            (Lights[3] as SpotLight)!.OuterConeAngle = g * 7 + 3;

            foreach (var renderable in Renderables)
            {
                renderable.Rotate(_angleLeft, Float3.Left);
                renderable.Rotate(_angleUp, Float3.Up);
            }
            
            Rasterizer.Render();
        }
    }
}