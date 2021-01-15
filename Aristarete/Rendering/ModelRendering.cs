using System;
using System.Collections.Generic;
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
    public class ModelRendering : IRendering
    {
        private float _angleLeft;
        private float _angleUp;
        private float _autoAngle;
        private Float3 _scale = Float3.One;
        private Float3 _translate = Float3.Zero;
        private Float3 _cameraRotation = new(0, 0, 3);
        private Float3 _cameraRotationStart = new(0, 0, 3);
        private Float3 _cameraRotationStop = new(3, 0, 0);
        private int _cameraRotationSwitch;
        private readonly Float3 _eye = new(0, 0, 2);
        private readonly Model2 _model = Model2.LoadFromFile("_Resources/crystal.obj", new PbrMaterial(FloatColor.White, new TextureInfo(Texture.LoadFrom("_Resources/crystal_diffuse.png"))));
        private readonly Model2 _modelGreen = Model2.LoadFromFile("_Resources/crystal.obj", new PbrMaterial(FloatColor.White, new TextureInfo(Texture.LoadFrom("_Resources/crystal_green_diffuse.png"))));
        private readonly VertexProcessor _vertexProcessor = new();
        private readonly List<IRenderable> _renderObjects = new();

        public ModelRendering()
        {
            _vertexProcessor.SetPerspective(60, 2, 0.1f, 100);
            _vertexProcessor.SetLookAt(_eye, new Float3(0, 0, 0), Float3.Up);
            _renderObjects.Add(new RenderObject(_vertexProcessor, _modelGreen));
            _renderObjects.Add(new RenderObject(_vertexProcessor, _model));
            _renderObjects.Add(new RenderObject(_vertexProcessor, _model));
            _renderObjects.Add(new RenderObject(_vertexProcessor, _model));
            _renderObjects.Add(new Cube(_vertexProcessor) {BasicColor = FloatColor.White}.CreateNormals());
            
            
            Statics.Lights.Add(new DirectionalLight
            {
                Position = Float3.Forward.Normalize(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Red,
                Specular = FloatColor.White,
                Shininess = 32
            });
        }

        public void Run(Rasterizer rasterizer)
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
                _translate += Float3.Left / 20f;
            }
            else if (Input.IsCurrentPress(Keys.D))
            {
                _translate += Float3.Right / 20f;
            }

            if (Input.IsCurrentPress(Keys.Space))
            {
                _cameraRotation = Float3.Lerp(_cameraRotationStart, _cameraRotationStop, _cameraRotationSwitch++ / 100f);
                if (_cameraRotation == new Float3(3, 0, 0) && _cameraRotationSwitch > 100)
                {
                    _cameraRotationStart = _cameraRotationStop;
                    _cameraRotationStop = new Float3(0,0,-3);
                    _cameraRotationSwitch = 0;
                }
            }

            _autoAngle++;

            _vertexProcessor.SetIdentityToView();
            _vertexProcessor.SetLookAt(_cameraRotation,
                new Float3(0, 0, 0),
                Float3.Up);
            
            _renderObjects[0]
                .Scale(_scale)
                .Rotate(_angleUp, Float3.Left)
                .Translate(_translate)
                .Rotate(_angleLeft, Float3.Up);
            
            // (_renderObjects[0] as RenderObject)!.Model.ColorAngle = FloatColor.White;
            // _renderObjects[0].Update(rasterizer);
            //
            // (_renderObjects[1] as RenderObject)!.Model.ColorAngle = FloatColor.Blue;
            _renderObjects[1]
                .Scale(Float3.One / 2f)
                .Translate(Float3.Left)
                .Rotate(_autoAngle, Float3.Up);
            _renderObjects[1].Update(rasterizer);
            
            // (_renderObjects[2] as RenderObject)!.Model.ColorAngle = FloatColor.Red;
            _renderObjects[2]
                .Scale(Float3.One)
                .Rotate(_autoAngle, Float3.Forward)
                .Translate(Float3.Right + Float3.Back);
            _renderObjects[2].Update(rasterizer);
            
            // (_renderObjects[3] as RenderObject)!.Model.ColorAngle = FloatColor.White *
            //                                                         MathExtensions.PingPong(
            //                                                             (float) Time.RealGameTime.TotalMilliseconds /
            //                                                             1000, 1);
            _renderObjects[3]
                .Scale(Float3.One / 4)
                .Translate(Float3.Forward / 5 + new Float3(0,MathExtensions.PingPong(
                    (float) Time.RealGameTime.TotalMilliseconds /
                    1000, 2) - 1,0))
                .Rotate(180, Float3.Forward);
            _renderObjects[3].Update(rasterizer);

            var scaleUnit = (MathExtensions.PingPong((float) Time.RealGameTime.TotalMilliseconds / 1000, 1) + 1) / 16;
            var scale = new Float3(scaleUnit);
            _renderObjects[4]
                .Scale(scale)
                .Rotate(20, Float3.Up)
                .Rotate(35f, Float3.Left)
                .Translate((Float3.Left + Float3.Up) / 2);
            _renderObjects[4].Update(rasterizer);
        }
    }
}