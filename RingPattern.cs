using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class RingPattern : NestedPattern
    {
        public RingPattern(Pattern pat1, Pattern pat2, Matrix transform) : base(pat1, pat2, transform)
        {

        }

        public RingPattern(RayColour col1, RayColour col2, Matrix transform) : base(col1, col2, transform)
        {

        }

        public override RayColour PatternAt(Tuple point)
        {
            RayColour col;

            if (Math.Floor(Math.Sqrt((point.x * point.x) + (point.z * point.z))) % 2 == 0)
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
