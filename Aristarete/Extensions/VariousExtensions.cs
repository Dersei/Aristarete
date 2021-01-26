using System;

namespace Aristarete.Extensions
{
    public static class VariousExtensions
    {
        public static float NextFloat(this Random @this, float minValue, float maxValue)
        {
            return (float) (@this.NextDouble() * (maxValue - minValue) + minValue);
        }
        
        public static float NextFloatWithSpace(this Random @this, float minValue1, float maxValue1, float minValue2, float maxValue2)
        {
            var value1 =  (float) (@this.NextDouble() * (maxValue1 - minValue1) + minValue1);
            var value2 =  (float) (@this.NextDouble() * (maxValue2 - minValue2) + minValue2);
            return @this.Next(0, 2) == 0 ? value1 : value2;
        }
    }
}