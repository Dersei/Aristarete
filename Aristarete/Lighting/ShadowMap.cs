using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Aristarete.Cameras;
using Aristarete.Rendering;
using Daeira;
using Buffer = Aristarete.Rendering.Buffer;


namespace Aristarete.Lighting
{
    public class ShadowMap : IShadowMap
    {
        private Camera _camera;
        private readonly Rasterizer _rasterizer;
        private readonly Light _light;
        private float[]? _map;
        public bool HardShadows { get; set; }= false;
        private const int ShadowMapWidth = 2048;
        private const int ShadowMapHeight = 2048;
        private const float ShadowMapBias = 0.0015f;
        private const int Area = 16;
        private const int Side = Area / 4;
        private const float AaShade = 0.5f / (Area * Area);
        private readonly Scene _scene;
        
        public ShadowMap(Light light, Scene scene)
        {
            _light = light;
            _scene = scene;
            _camera = _light switch
            {
                DirectionalLight dl => new OrthographicCamera(dl.Position, Float3.Zero, Float3.Up, -10f, 10f, -10f, 10f, -10f, 20f),
                SpotLight sl => new PerspectiveCamera(sl.Position, sl.Direction, Float3.Up, 45, 1, 0.1f, 50),
                _ => throw new ArgumentOutOfRangeException()
            };
            _rasterizer = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene, _camera);
        }

        public void Update()
        {
            _camera = new OrthographicCamera(_light.Position, Float3.Zero, Float3.Up, -10f, 10f, -10f, 10f, -10f, 20f);
            _rasterizer.Clear();
        }

        [MemberNotNull(nameof(_map))]
        public void Render()
        {
            _rasterizer.Render(RenderMode.DepthOnly);
            Parallel.ForEach(_scene.Renderables, mesh => { mesh.IsDirty = true; });

            _map = _rasterizer.ZBuffer;
        }

        public float PointInShadow(Float3 pointWorld)
        {
            // var object2View = _camera.World2View * renderable!.Object2World;
            // var object2Projection = _camera.View2Proj * object2View;
            // var point = object2Projection.MultiplyPoint(pointWorld);
            // var point_light_space = ToBufferCoords(point);
            var point_light_space = _rasterizer.Rasterize(pointWorld);
            // Console.WriteLine(p);
            float shadow = 0;
            var index = (int) (point_light_space.Y * ShadowMapWidth + point_light_space.X);
            if (index < 0 || index >= _map?.Length) return 0;
            var closestDepth = _map![index];
            var currentDepth = point_light_space.Z;
            shadow += currentDepth > closestDepth + ShadowMapBias ? 0.25f : 0.0f;
            if (HardShadows) return shadow;
            shadow = 0;

            for (var x = (int) point_light_space.X - Side; x < point_light_space.X + Side; ++x)
            {
                for (var y = (int) point_light_space.Y - Side; y < point_light_space.Y + Side; ++y)
                {
                    index = (int) (point_light_space.Y * ShadowMapWidth + point_light_space.X);
                    closestDepth = _map[index];
                    currentDepth = point_light_space.Z;
                    shadow += currentDepth > closestDepth + ShadowMapBias ? AaShade : 0.0f;
                }
            }


            return shadow;
        }
    }
}