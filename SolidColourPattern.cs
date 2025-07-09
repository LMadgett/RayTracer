using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class SolidColourPattern : Pattern
    {
        RayColour col;

        public SolidColourPattern(RayColour col, Matrix transform) : base(transform)
        {
            this.col = col;
        }

        public override RayColour PatternAt(Tuple point)
        {
            return col;
        }
    }
}
