using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;
using Aristarete.Extensions;
using Aristarete.Inputting;
using Aristarete.Lighting;
using Aristarete.Meshes;

namespace Aristarete.Rendering
{
    public class MaterialRendering : IRendering
    {
        private float _angleLeft;
        private float _angleUp;
        private float _oldAngleLeft;
        private float _oldAngleUp;
        private bool _isStopped;
        private readonly VertexProcessor _vertexProcessor = new();
        private readonly List<IRenderable> _meshes = new();

        public MaterialRendering()
        {
            _vertexProcessor.SetPerspective(45, 2, 0.1f, 100);
            _vertexProcessor.SetLookAt(new Float3(0, 0, 5), new Float3(0, 0, 0), Float3.Up);
            Statics.VertexProcessor = _vertexProcessor;


            _meshes.Add(new Sphere(_vertexProcessor, 1, 24, 24)
                {
                    BasicColor = FloatColor.White, Material = new PbrMaterial(FloatColor.White,
                        new TextureInfo(Texture.LoadFrom("_Resources/circuitry-albedo.png"))
                        ,specularMap: new TextureInfo(Texture.LoadFrom("_Resources/circuitry-smoothness.png"))
                        ,emissiveMap: new TextureInfo(Texture.LoadFrom("_Resources/circuitry-emission.png"))
                        ,normalMap: new TextureInfo(Texture.LoadFrom("_Resources/circuitry-normals.png"))
                     ),
                    LightingMode = LightingMode.Pixel
                }
                .CreateNormals()
                .Scale(0.7f).Translate(Float3.Left * 2));
            
            _meshes.Add(new Sphere(_vertexProcessor, 1, 24, 24)
                {
                    BasicColor = FloatColor.White, Material = new PbrMaterial(FloatColor.White,
                        new TextureInfo(Texture.LoadFrom("_Resources/circuitry-albedo.png"))
                       // ,specularMap: new TextureInfo(Texture.LoadFrom("_Resources/circuitry-smoothness.png"))
                    )
                }
                .CreateNormals()
                .Scale(0.7f).Translate(Float3.Right * 2));

            Statics.Lights.Add(new DirectionalLight
            {
                Position = Float3.Down.Normalize(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.White,
                Specular = FloatColor.Red,
                Shininess = 32
            });
            //
            Statics.Lights.Add(new PointLight()
            {
                Position = Float3.Back * 5 + 5 * Float3.Left,
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.Red,
                Specular = FloatColor.Black,
                Shininess = 32
            });
            // Statics.Lights.Add(new PointLight()
            // {
            //     Position = Float3.Up + Float3.Left*10,
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.White,
            //     Specular = FloatColor.Black,
            //     Shininess = 32
            // });
            //
            // Statics.Lights.Add(new SpotLight
            // {
            //     Position = new Float3(0, 0, 5),
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.Red,
            //     Specular = FloatColor.White,
            //     Shininess = 32,
            //     Direction = (Float3.Forward + Float3.Right / 50),
            //     Angle = 5
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
            //     Angle = 5
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
            //     Angle = 5
            // });
            //
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
            // (Statics.Lights[1] as SpotLight).Direction = (Float3.Forward + Float3.Right * g1);
            // (Statics.Lights[2] as SpotLight).Position = new Float3(0, 0, 5) + 2 * Float3.Right + Float3.Up * g1;
            // (Statics.Lights[3] as SpotLight).Angle = g * 7 + 1;

            Parallel.ForEach(_meshes, mesh =>
            {
                mesh.Rotate(_angleLeft, Float3.Up);
                mesh.Rotate(_angleUp, Float3.Left);
                mesh.Update(rasterizer);
            });
            //
            // foreach (var mesh in _meshes)
            // {
            //     mesh.Rotate(_angleLeft, Float3.Up);
            //     mesh.Rotate(_angleUp, Float3.Left);
            //     mesh.Update(rasterizer);
            // }
        }
    }
}