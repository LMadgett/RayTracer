using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class GradientPattern : NestedPattern
    {
        public GradientPattern(Pattern pat1, Pattern pat2, Matrix transform) : base(pat1, pat2, transform)
        {

        }

        public GradientPattern(RayColour col1, RayColour col2, Matrix transform) : base(col1, col2, transform)
        {

        }

        public override RayColour PatternAt(Tuple point)
        {
            RayColour col;
            RayColour col1 = pat1.PatternAt(point);
            RayColour col2 = pat2.PatternAt(point);

            RayColour colourDist = col2.Sub(col1);
            double fraction = point.x - Math.Floor(point.x);

            col = colourDist.Multiply(fraction).Add(col1);

            return col;
        }
    }
}
