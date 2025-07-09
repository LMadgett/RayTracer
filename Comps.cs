using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class Comps
    {
        public Tuple point;
        public Tuple eyeDir;
        public Tuple normal;
        public double t;
        public SceneObject sceneObject;
        public bool inside;
        public Tuple overPoint;
        public Tuple reflectV;
        public double rI1;
        public double rI2;
        public Tuple underPoint;

        public Comps(Tuple point, Tuple eyeDir, Tuple normal, double t, SceneObject obj, bool inside, Tuple reflectV, double rI1, double rI2)
        {
            this.point = point;
            this.eyeDir = eyeDir;
            this.normal = normal;
            this.t = t;
            this.sceneObject = obj;
            this.inside = inside;
            this.reflectV = reflectV;
            this.rI1 = rI1;
            this.rI2 = rI2;
        }   

        public Comps()
        {

        }
    }
}
