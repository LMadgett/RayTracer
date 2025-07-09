using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class Light
    {
        public RayColour intensity;
        public Tuple position;

        public Light(RayColour intensity, Tuple position)
        {
            this.intensity = intensity;
            this.position = position;
        }
    }
}
