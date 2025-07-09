using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class Plane : SceneObject
    {
        public Plane()
        {
            SetTransform(Matrix.GetIdentity(4));
        }

        protected override List<Intersection> LocalIntersect(Ray ray, Intersection closestHitSoFar)
        {
            List<Intersection> intersections = new List<Intersection>();

            // is the ray coplanar with the plane? (plane is in xz LOCAL COORDS)
            if(!MathsUtils.Equal(Math.Abs(ray.direction.y), 0.0))
            {
                double t = -ray.origin.y / ray.direction.y;
                Intersection intersect = new Intersection(t, this);
                if(t > 0)
                    intersections.Add(intersect);
            }

            return intersections;
        }

        protected override Tuple GetLocalCentrePoint()
        {
            Tuple centre = Tuple.Point(0.0, 0.0, 0.0);
            return centre;
        }

        protected override Tuple GetLocalNormal(Tuple objPoint, Intersection i)
        {
            Tuple objectNormal = Tuple.Vector(0, 1, 0);

            return objectNormal;
        }

        public override BoundingBox GetBoundingBox()
        {
            BoundingBox bB = new BoundingBox(Tuple.Point(-BoundingBox.INFINITYish, -MathsUtils.EPSILON, -BoundingBox.INFINITYish), Tuple.Point(BoundingBox.INFINITYish, MathsUtils.EPSILON, BoundingBox.INFINITYish));

            return bB;
        }
    }
}
