using Aristarete.Basic;

namespace Aristarete.Rendering
{
    public class BasicRendering : IRendering
    {
        public void Run(Rasterizer rasterizer)
        {
            rasterizer.Triangle(new[]
                {
                    new Float3(-0.5f, 0.5f, 0.0f),
                    new Float3(0.5f, 0.5f, 0.0f),
                    new Float3(0.5f, -0.5f, 0.0f)
                },
                new[]
                {
                    FloatColor.Green,
                    FloatColor.Red,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    new Float3(0.5f, -0.5f, 0.0f),
                    new Float3(-0.5f, -0.5f, 0.0f),
                    new Float3(-0.5f, 0.5f, 0.0f)
                },
                new[]
                {
                    FloatColor.Green, 
                    FloatColor.Green,
                    FloatColor.Green
                });

            rasterizer.Triangle(new[]
                {
                    new Float3(1f, 0.5f, 2.0f),
                    new Float3(0f, -0.5f, 2.0f),
                    new Float3(0.8f, -1.5f, -1.0f)
                },
                new[]
                {
                    FloatColor.Blue,
                    FloatColor.Blue,
                    FloatColor.Blue
                });

            rasterizer.Triangle(new[]
                {
                    new Float3(0.5f, 0.5f, 0.0f),
                    new Float3(0.5f, -0.5f, 0.0f),
                    new Float3(1.5f, -1.5f, 0.0f)
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