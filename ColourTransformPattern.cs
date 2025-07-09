using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class ColourTransformPattern : Pattern
    {
        public ColourTransformPattern(Matrix transform) : base(transform)
        {

        }

        public override RayColour PatternAt(Tuple point)
        {
            RayColour col = new RayColour(Math.Abs(point.x) / 10, Math.Abs(point.y) / 10, Math.Abs(point.z) / 10);

            return col;
        }
    }
}
