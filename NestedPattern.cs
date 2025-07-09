using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal abstract class NestedPattern : Pattern
    {
        protected Pattern pat1;
        protected Pattern pat2;

        public NestedPattern(Pattern pat1, Pattern pat2, Matrix transform) : base(transform)
        {
            this.pat1 = pat1;
            this.pat2 = pat2;
        }

        public NestedPattern(RayColour col1, RayColour col2, Matrix transform) : this(new SolidColourPattern(col1, transform), new SolidColourPattern(col2, transform), transform)
        {

        }
    }
}
