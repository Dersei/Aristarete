using System;
using System.Collections.Generic;
using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Basic.Textures;
using Aristarete.Inputting;
using Aristarete.Lighting;
using Aristarete.Models;

namespace Aristarete.Rendering
{
    public class BookRendering : IRendering
    {
        private Float3 _cameraRotation = new(0, 0, 3);
        private Float3 _cameraRotationStart = new(0, 0, 3);
        private Float3 _cameraRotationStop = new(3, 0, 0);
        private readonly Float3 _eye = new(0, 0, 2);

        private readonly Model _model = Model.LoadFromFile("_Resources/paladins-book/paladins_book_fewest.obj",
            new PbrMaterial(FloatColor.White,
                new TextureInfo(Texture.LoadFrom("_Resources/paladins-book/paladins_book_Base_Color.png")),
                emissiveMap:new TextureInfo(Texture.LoadFrom("_Resources/paladins-book/paladins_book_Emissive.png")),
                specularMap:new TextureInfo(Texture.LoadFrom("_Resources/paladins-book/paladins_book_Smoothness.png"))
                , normalMap:new TextureInfo(Texture.LoadFrom("_Resources/paladins-book/paladins_book_Normal_DirectxL.png"))
                ),0.05f);
        private readonly VertexProcessor _vertexProcessor = new();
        private readonly List<IRenderable> _renderObjects = new();

        public BookRendering()
        {
            _vertexProcessor.SetPerspective(60, 2, 0.1f, 100);
            _vertexProcessor.SetLookAt(_eye, new Float3(0, 0, 0), Float3.Up);
            _renderObjects.Add(new RenderObject(_vertexProcessor, _model));
            
            
            Statics.Lights.Add(new DirectionalLight
            {
                Position = Float3.Down.Normalize(),
                Ambient = FloatColor.Black,
                Diffuse = FloatColor.White,
                Specular = FloatColor.White,
                Shininess = 32
            });
            
            // Statics.Lights.Add(new SpotLight
            // {
            //     Position = new Float3(0, 0, 5),
            //     Ambient = FloatColor.Black,
            //     Diffuse = FloatColor.White,
            //     Specular = FloatColor.White,
            //     Shininess = 32,
            //     Direction = Float3.Forward,
            //     Angle = 5,
            //     OuterConeAngle = 7
            // });

        }

        private int _counter;
        private bool _isFlipped;
        
        public void Run(Rasterizer rasterizer)
        {
          
            if (_counter%20==0)
            {
                _cameraRotation = (_isFlipped = !_isFlipped) ? _cameraRotationStop : _cameraRotationStart;
            }

            _counter++;
            _vertexProcessor.SetIdentityToView();
            _vertexProcessor.SetLookAt(_cameraRotation,
                new Float3(0, 0, 0),
                Float3.Up);
            
             _renderObjects[0].Update(rasterizer);
            
        }
    }
}