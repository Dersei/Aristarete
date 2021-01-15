using System;
using Aristarete.Basic;
using Aristarete.Basic.Materials;
using Aristarete.Meshes;

namespace Aristarete.Models
{
    public readonly struct ModelTriangle
    {
        public readonly Float3 Normal;
        public readonly Vertex FV1;
        public readonly Vertex FV2;
        public readonly Vertex FV3;

        public PbrMaterial Material { get; }

        public Vertex this[int index] => index switch
        {
            0 => FV1,
            1 => FV2,
            2 => FV3,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };
            
        
        public ModelTriangle(Vertex fv1 = default,
            Vertex fv2 = default, Vertex fv3 = default, PbrMaterial? material = default)
        {
            FV1 = fv1;
            FV2 = fv2;
            FV3 = fv3;
            Material = material ?? PbrMaterial.Error;
            Normal = (FV2.Position - FV1.Position).Cross(FV3.Position - FV1.Position).Normalize();
        }
    }
}