using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aristarete.Cameras;
using Aristarete.Rendering;
using Daeira;
using Buffer = Aristarete.Rendering.Buffer;


namespace Aristarete.Lighting
{
    public class PointShadowMap : IShadowMap
    {
        private readonly Rasterizer _rasterizerUp;
        private readonly Rasterizer _rasterizerDown;
        private readonly Rasterizer _rasterizerForward;
        private readonly Rasterizer _rasterizerBack;
        private readonly Rasterizer _rasterizerLeft;
        private readonly Rasterizer _rasterizerRight;

        private float[] _mapUp = null!;
        private float[] _mapDown = null!;
        private float[] _mapForward = null!;
        private float[] _mapBack = null!;
        private float[] _mapLeft = null!;
        private float[] _mapRight = null!;
        
        private readonly Light _light;
        public bool HardShadows { get; set; }= false;
        private const int ShadowMapWidth = 2048;
        private const int ShadowMapHeight = 2048;
        private const float ShadowMapBias = 0.0015f;
        private const int Area = 16;
        private const int Side = Area / 4;
        private const float AaShade = 0.5f / (Area * Area);
        private readonly Scene _scene;

        public PointShadowMap(Light light, Scene scene)
        {
            _light = light;
            _scene = scene;
            _rasterizerUp = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
                new OrthographicCamera(light.Position, (light.Position + Float3.Up).Normalize(), Float3.Forward, -10f, 10f, -10f, 10f, -10f, 20f));
            _rasterizerDown = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
                new OrthographicCamera(light.Position, (light.Position + Float3.Down).Normalize(), Float3.Back, -10f, 10f, -10f, 10f, -10f, 20f));
            _rasterizerForward = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
                new OrthographicCamera(light.Position, (light.Position + Float3.Forward).Normalize(), Float3.Down, -10f, 10f, -10f, 10f, -10f, 20f));
            _rasterizerBack = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
                new OrthographicCamera(light.Position, (light.Position + Float3.Back).Normalize(), Float3.Down, -10f, 10f, -10f, 10f, -10f, 20f));
            _rasterizerLeft = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
                new OrthographicCamera(light.Position, (light.Position + Float3.Left).Normalize(), Float3.Down, -10f, 10f, -10f, 10f, -10f, 20f));
            _rasterizerRight = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
                new OrthographicCamera(light.Position, (light.Position + Float3.Right).Normalize(), Float3.Down, -10f, 10f, -10f, 10f, -10f, 20f));
            // _rasterizerUp = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
            //     new PerspectiveCamera(light.Position, (light.Position + Float3.Up).Normalize(), Float3.Back, 90, 1, 0.1f, 50));
            // _rasterizerDown = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
            //     new PerspectiveCamera(light.Position, (light.Position + Float3.Down).Normalize(), Float3.Forward, 90, 1, 0.1f, 50));
            // _rasterizerForward = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
            //     new PerspectiveCamera(light.Position, (light.Position + Float3.Forward).Normalize(), Float3.Up, 90, 1, 0.1f, 50));
            // _rasterizerBack = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
            //     new PerspectiveCamera(light.Position, (light.Position + Float3.Back).Normalize(), Float3.Up, 90, 1, 0.1f, 50));
            // _rasterizerLeft = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
            //     new PerspectiveCamera(light.Position, (light.Position + Float3.Left).Normalize(), Float3.Up, 90, 1, 0.1f, 50));
            // _rasterizerRight = new ForwardRasterizer(new Buffer(ShadowMapWidth, ShadowMapHeight), scene,
            //     new PerspectiveCamera(light.Position, (light.Position + Float3.Right).Normalize(), Float3.Up, 90, 1, 0.1f, 50));
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        // [MemberNotNull(nameof(_mapUp))]
        // [MemberNotNull(nameof(_mapDown))]
        // [MemberNotNull(nameof(_mapForward))]
        // [MemberNotNull(nameof(_mapBack))]
        // [MemberNotNull(nameof(_mapLeft))]
        // [MemberNotNull(nameof(_mapRight))]
        public void Render()
        {
            _rasterizerUp.Render(RenderMode.DepthOnly);
            _rasterizerDown.Render(RenderMode.DepthOnly);
            _rasterizerForward.Render(RenderMode.DepthOnly);
            _rasterizerBack.Render(RenderMode.DepthOnly);
            _rasterizerLeft.Render(RenderMode.DepthOnly);
            _rasterizerRight.Render(RenderMode.DepthOnly);

            Parallel.ForEach(_scene.Renderables, mesh => { mesh.IsDirty = true; });

            _mapUp = _rasterizerUp.ZBuffer;
            _mapDown = _rasterizerDown.ZBuffer;
            _mapForward = _rasterizerForward.ZBuffer;
            _mapBack = _rasterizerBack.ZBuffer;
            _mapLeft = _rasterizerLeft.ZBuffer;
            _mapRight = _rasterizerRight.ZBuffer;
        }


        private (float[] map, Rasterizer rasterizer) MapChooser(Float3 point)
        {
            List<(float, float[], Rasterizer)> list = new List<(float, float[], Rasterizer)>();
            list.Add((Float3.Dot(point, _rasterizerBack.Camera.Direction), _mapBack, _rasterizerBack));
            list.Add((Float3.Dot(point, _rasterizerDown.Camera.Direction), _mapDown, _rasterizerDown));
            list.Add((Float3.Dot(point, _rasterizerUp.Camera.Direction), _mapUp, _rasterizerUp));
            list.Add((Float3.Dot(point, _rasterizerForward.Camera.Direction), _mapForward, _rasterizerForward));
            list.Add((Float3.Dot(point, _rasterizerLeft.Camera.Direction), _mapLeft, _rasterizerLeft));
            list.Add((Float3.Dot(point, _rasterizerRight.Camera.Direction), _mapRight, _rasterizerRight));
            list = list.OrderBy(item => item.Item1).ToList();
            return (list[0].Item2, list[0].Item3);
            // var ax = MathF.Abs(point.X);
            // var ay = MathF.Abs(point.Y);
            // var az = MathF.Abs(point.Z);
            //
            // if (ax > ay && ax > az)
            // {
            //     if (point.X > 0.0f) return (_mapRight, _rasterizerRight);
            //     return (_mapLeft, _rasterizerLeft);
            // }
            //
            // if (ay > ax && ay > az)
            // {
            //     if (point.Y > 0.0f) return (_mapUp, _rasterizerUp);
            //     return (_mapDown, _rasterizerDown);
            // }
            //
            // if (az > ay && az > ax)
            // {
            //     if (point.Z > 0.0f) return (_mapForward, _rasterizerForward);
            //     return (_mapBack, _rasterizerBack);
            // }
            //
            // return (_mapForward, _rasterizerForward);
        }

        public float PointInShadow(Float3 pointWorld)
        {
            var direction = _light.Position - pointWorld;
            //var point_light_space = _rasterizer.Rasterize(pointWorld);
            var (map, rasterizer) = MapChooser(direction.Normalize());
            var point_light_space = rasterizer.Rasterize(pointWorld);
            float shadow = 0;
            var index = (int) (point_light_space.Y * ShadowMapWidth + point_light_space.X);
            if (index < 0 || index >= map.Length) return 0;
            var closestDepth = map![index];
            var currentDepth = point_light_space.Z;
            shadow += currentDepth > closestDepth + ShadowMapBias ? 0.25f : 0.0f;
            if (HardShadows) return shadow;
            shadow = 0;

            for (var x = (int) point_light_space.X - Side; x < point_light_space.X + Side; ++x)
            {
                for (var y = (int) point_light_space.Y - Side; y < point_light_space.Y + Side; ++y)
                {
                    index = (int) (point_light_space.Y * ShadowMapWidth + point_light_space.X);
                    closestDepth = map[index];
                    currentDepth = point_light_space.Z;
                    shadow += currentDepth > closestDepth + ShadowMapBias ? AaShade : 0.0f;
                }
            }


            return shadow;
        }
    }
}