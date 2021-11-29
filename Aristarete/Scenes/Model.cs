﻿using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;
using Aristarete.Cameras;
using Aristarete.Extensions;
using Aristarete.Inputting;
using Aristarete.Lighting;
using Aristarete.Meshes;
using Aristarete.Meshes.Models;
using Aristarete.Rendering;
using Aristarete.Utils;


namespace Aristarete.Scenes
{
    public class Model : Scene
    {
        
        private float _angleLeft;
        private float _angleUp;
        private float _autoAngle;
        private Float3Sse _scale = Float3Sse.One;
        private Float3Sse _translate = Float3Sse.Zero;
        private Float3Sse _cameraRotation = new(0, 0, 3);
        private Float3Sse _cameraRotationStart = new(0, 0, 3);
        private Float3Sse _cameraRotationStop = new(3, 0, 0);
        private int _cameraRotationSwitch;
        private readonly Float3Sse _eye = new(0, 0, 2);

        private readonly Meshes.Models.Model _model = Meshes.Models.Model.LoadFromFile("_Resources/crystal/crystal.obj",
            new PbrMaterial(FloatColor.White, new TextureInfo(Texture.LoadFrom("_Resources/crystal/textures/crystal_diffuse.png"))));

        private readonly Meshes.Models.Model _modelGreen = Meshes.Models.Model.LoadFromFile("_Resources/crystal/crystal.obj",
            new PbrMaterial(FloatColor.White, new TextureInfo(Texture.LoadFrom("_Resources/crystal/textures/crystal_diffuse.png"))));

        public Model(Buffer buffer)
        {
            PerspectiveCamera perspectiveCamera = new(_eye, new Float3Sse(0, 0, 0), Float3Sse.Up, 60, 2, 0.1f, 100);

            Rasterizer = new ForwardRasterizer(buffer, this,perspectiveCamera);

            AddMesh(new RenderObject(_modelGreen));
            AddMesh(new RenderObject(_model));
            AddMesh(new RenderObject(_model));
            AddMesh(new RenderObject(_model));
            AddMesh(new Cube() {BasicColor = FloatColor.White, LiveUpdate = true}
                );


            AddLight(new DirectionalLight
            {
                Position = Float3Sse.Left.NormalizeExact(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.White,
                Specular = FloatColor.White,
                Shininess = 32
            });

            AddLight(new SpotLight
            {
                Position = new Float3Sse(0, 0, 5),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Blue,
                Specular = FloatColor.White,
                Shininess = 32,
                Direction = Float3Sse.Forward,
                Angle = 5,
                OuterConeAngle = 7
            });
        }

        public override void Run()
        {
            if (Input.IsCurrentPress(Keys.Q))
            {
                _angleLeft += 15f;
            }
            else if (Input.IsCurrentPress(Keys.E))
            {
                _angleLeft -= 15f;
            }
            else if (Input.IsCurrentPress(Keys.W))
            {
                _angleUp += 15f;
            }
            else if (Input.IsCurrentPress(Keys.S))
            {
                _angleUp -= 15f;
            }
            else if (Input.IsNewPress(Keys.Z))
            {
                _scale /= 2;
            }
            else if (Input.IsNewPress(Keys.X))
            {
                _scale *= 2;
            }
            else if (Input.IsCurrentPress(Keys.A))
            {
                _translate += Float3Sse.Left / 20f;
            }
            else if (Input.IsCurrentPress(Keys.D))
            {
                _translate += Float3Sse.Right / 20f;
            }

            if (Input.IsCurrentPress(Keys.Space))
            {
                _cameraRotation =
                    Float3Sse.Lerp(_cameraRotationStart, _cameraRotationStop, _cameraRotationSwitch++ / 100f);
                if (_cameraRotation == new Float3Sse(3, 0, 0) && _cameraRotationSwitch > 100)
                {
                    _cameraRotationStart = _cameraRotationStop;
                    _cameraRotationStop = new Float3Sse(0, 0, -3);
                    _cameraRotationSwitch = 0;
                }
            }

            _autoAngle++;

            // _perspectiveCamera.SetIdentityToView();
            // _perspectiveCamera.SetLookAt(_cameraRotation,
            //     new Float3Sse(0, 0, 0),
            //     Float3Sse.Up);

            Renderables[0]
                .Scale(_scale)
                .Rotate(_angleUp, Float3Sse.Left)
                .Translate(_translate)
                .Rotate(_angleLeft, Float3Sse.Up);

            Renderables[1]
                .Scale(Float3Sse.One / 2f)
                .Translate(Float3Sse.Left)
                .Rotate(_autoAngle, Float3Sse.Up);

            Renderables[2]
                .Scale(Float3Sse.One)
                .Rotate(_autoAngle, Float3Sse.Forward)
                .Translate(Float3Sse.Right + Float3Sse.Back);

            Renderables[3]
                .Scale(Float3Sse.One / 4)
                .Translate(Float3Sse.Forward / 5 + new Float3Sse(0, MathExtensions.PingPong(
                    (float) Time.RealGameTime.TotalMilliseconds /
                    1000, 2) - 1, 0))
                .Rotate(180, Float3Sse.Forward);

            var scaleUnit = (MathExtensions.PingPong((float) Time.RealGameTime.TotalMilliseconds / 1000, 1) + 1) / 4;
            var scale = new Float3Sse(scaleUnit);
            Renderables[4]
                .Scale(scale)
                .Rotate(20, Float3Sse.Up)
                .Rotate(35f, Float3Sse.Left)
                .Translate((Float3Sse.Left + Float3Sse.Up) / 2);

            Rasterizer.Render();
        }
    }
}