using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public abstract class Pattern
    {
        protected Matrix transform;
        protected Matrix invTransform;

        public Pattern(Matrix transform)
        {
            this.transform = transform;
            this.invTransform = transform.GetInverse();
        }

        public abstract RayColour PatternAt(Tuple point);

        public RayColour PatternAtObject(Tuple point, SceneObject obj)
        {
            RayColour col;

            Tuple objPoint = MathsUtils.WorldToObject(obj, point);
            Tuple patPoint = objPoint.Multiply(this.invTransform);
            col = PatternAt(patPoint);

            return col;
        }
    }
}
