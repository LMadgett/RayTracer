using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class Cube : SceneObject
    {
        public Cube()
        {
            SetTransform(Matrix.GetIdentity(4));
        }

        protected override Tuple GetLocalCentrePoint()
        {
            Tuple centre = Tuple.Point(0, 0, 0);
            return centre;
        }

        protected override List<Intersection> LocalIntersect(Ray ray, Intersection closestHitSoFar)
        {
            List<Intersection> intersections = new List<Intersection>();

            Tuple<double, double> xt = CheckAxis(ray.origin.x, ray.direction.x);
            Tuple<double, double> yt = CheckAxis(ray.origin.y, ray.direction.y);
            Tuple<double, double> zt = CheckAxis(ray.origin.z, ray.direction.z);

            double tmin = MathsUtils.Max(xt.Item1, yt.Item1, zt.Item1);

            double tmax = MathsUtils.Min(xt.Item2, yt.Item2, zt.Item2);

            //did we hit the cube?
            if(tmin <= tmax)
            {
                Intersection int1 = new Intersection(tmin, this);
                Intersection int2 = new Intersection(tmax, this);
                intersections.Add(int1);
                intersections.Add(int2);
            }

            return intersections;
        }

        private Tuple<double, double> CheckAxis(double origin, double direction)
        {
            double tminNum = (-1.0 - origin);
            double tmaxNum = (1.0 - origin);
            double tmin;
            double tmax;

            if(Math.Abs(direction) >= MathsUtils.EPSILON)
            {
                tmin = tminNum / direction;
                tmax = tmaxNum / direction;
            }
            else
            {
                tmin = tminNum * MathsUtils.INFINITY;
                tmax = tmaxNum * MathsUtils.INFINITY;
            }

            if(tmin > tmax)
            {
                double temp = tmin;
                tmin = tmax;
                tmax = temp;
            }

            return new Tuple<double, double>(tmin, tmax);
        }

        protected override Tuple GetLocalNormal(Tuple objPoint, Intersection i)
        {
            Tuple objectNormal;

            double maxC = MathsUtils.Max(Math.Abs(objPoint.x), Math.Abs(objPoint.y), Math.Abs(objPoint.z));

            if(maxC == Math.Abs(objPoint.x))
            {
                objectNormal = Tuple.Vector(objPoint.x, 0.0, 0.0);
            }
            else if (maxC == Math.Abs(objPoint.y))
            {
                objectNormal = Tuple.Vector(0.0, objPoint.y, 0.0);
            }
            else
            {
                objectNormal = Tuple.Vector(0.0, 0.0, objPoint.z);
            }

            return objectNormal.Normalise();
        }

        public override BoundingBox GetBoundingBox()
        {
            BoundingBox bB = new BoundingBox(Tuple.Point(-1, -1, -1), Tuple.Point(1, 1, 1));

            return bB;
        }
    }
}
