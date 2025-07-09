using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class StripePattern : NestedPattern
    {
        public StripePattern(Pattern pat1, Pattern pat2, Matrix transform) : base(pat1, pat2, transform)
        {

        }

        public StripePattern(RayColour col1, RayColour col2, Matrix transform) : base(col1, col2, transform)
        {

        }

        public override RayColour PatternAt(Tuple point)
        {
            RayColour col;

            if (Math.Floor(point.x) % 2 == 0)
            {
                col = pat1.PatternAt(point);
            }
            else
            {
                col = pat2.PatternAt(point);
            }

            return col;
        }
    }
}
