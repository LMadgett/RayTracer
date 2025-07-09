using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class Sphere : SceneObject
    {
        public Sphere ()
        {
            SetTransform(Matrix.GetIdentity(4));
        }

        protected override Tuple GetLocalCentrePoint()
        {
            Tuple centre = Tuple.Point(0.0, 0.0, 0.0);
            return centre;
        }

        protected override List<Intersection> LocalIntersect(Ray ray, Intersection closestHitSoFar)
        {
            List<Intersection> intersections = new List<Intersection>();

            Tuple sphereToRay = Tuple.Sub(ray.origin, Tuple.Origin);
            double a = Tuple.Dot(ray.direction, ray.direction);
            double b = 2 * Tuple.Dot(ray.direction, sphereToRay);
            double c = Tuple.Dot(sphereToRay, sphereToRay) - 1;
            double discriminant = (b * b) - (4 * a * c);

            if (discriminant >= 0)
            {
                double t1 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                double t2 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                if (t1 > 0) intersections.Add(new Intersection(t1, this));
                if (t2 > 0) intersections.Add(new Intersection(t2, this));
            }

            return intersections;
        }

        protected override Tuple GetLocalNormal(Tuple objPoint, Intersection i)
        {
            Tuple objectNormal = objPoint.Sub(Tuple.Origin);

            return objectNormal;
        }

        public override BoundingBox GetBoundingBox()
        {
            BoundingBox bB = new BoundingBox(Tuple.Point(-1, -1, -1), Tuple.Point(1, 1, 1));

            return bB;
        }
    }
}
