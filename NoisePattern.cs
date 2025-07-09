using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class NoisePattern : Pattern
    {
        int seed;
        FastNoise noise;
        Matrix transform;
        Pattern pattern;

        public NoisePattern(int seed, double frequency, Pattern pat, Matrix transform) : base(transform)
        {
            this.transform = transform;
            this.seed = seed;
            noise = new FastNoise(seed);
            noise.SetNoiseType(FastNoise.NoiseType.Simplex);
            this.pattern = pat;
            noise.SetFrequency((float)frequency);
        }

        public override RayColour PatternAt(Tuple point)
        {
            RayColour col;

            //didn't really know why it wouldn't return 3 values, so getting each different value from simply putting the x, y, z coords in a different order for each one
            float xNoise = (noise.GetNoise((float)point.x, (float)point.y, (float)point.z));
            float yNoise = (noise.GetNoise((float)point.y, (float)point.z, (float)point.x));
            float zNoise = (noise.GetNoise((float)point.z, (float)point.x, (float)point.y));

            Tuple peturbedPoint = Tuple.Point(point.x + xNoise, point.y + yNoise, point.z + zNoise);

            col = pattern.PatternAt(peturbedPoint);

            return col;
        }
    }
}
