using System.Windows.Input;
using Aristarete.Basic;

namespace Aristarete.Rendering
{
    public class BasicRendering : IRendering
    {
        private Vector3 _v = new Vector3(-0.5f, 0.5f, 0.0f);
        
        public void Run(Rasterizer rasterizer)
        {
            if (Keyboard.IsKeyDown(Key.A))
            {
                _v *= 0.9f;
            }
            else if(Keyboard.IsKeyDown(Key.D))
            {
                _v /= 0.9f;
            }
            
            rasterizer.Triangle(new[]
                {
                    _v,
                    new Vector3(0.5f, 0.5f, 0.0f),
                    new Vector3(0.5f, -0.5f, 0.0f)
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Red,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    new Vector3(0.5f, -0.5f, 0.0f),
                    new Vector3(-0.5f, -0.5f, 0.0f),
                    new Vector3(-0.5f, 0.5f, 0.0f)
                },
                new[]
                {
                    FloatColor.Green, 
                    FloatColor.Green,
                    FloatColor.Green
                });

            rasterizer.Triangle(new[]
                {
                    new Vector3(1f, 0.5f, 2.0f),
                    new Vector3(0f, -0.5f, 2.0f),
                    new Vector3(0.8f, -1.5f, -1.0f)
                },
                new[]
                {
                    FloatColor.Blue,
                    FloatColor.Blue,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    new Vector3(0.5f, 0.5f, 0.0f),
                    new Vector3(0.5f, -0.5f, 0.0f),
                    new Vector3(1.5f, -1.5f, 0.0f)
                },
                new[]
                {
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow,
                    FloatColor.UnityYellow
                });
        }
    }
}