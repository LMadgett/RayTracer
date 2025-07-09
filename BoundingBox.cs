using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class BoundingBox : SceneObject
    {
        public readonly Tuple min;
        public readonly Tuple max;
        public static double INFINITYish = 1000000000.0;
        private List<Tuple> points = null;

        public BoundingBox(Tuple min, Tuple max)
        {
            this.min = min;
            this.max = max;
        }

        protected override Tuple GetLocalCentrePoint()
        {
            Tuple centre = Tuple.Point((min.x + max.x)/2.0, (min.y + max.y) / 2.0, (min.z + max.z) / 2.0);
            return centre;
        }

        public List<Tuple> GetPoints()
        {
            if (points == null)
            {
                points = new List<Tuple>();
                Tuple p = Tuple.Point(min.x, min.y, min.z);
                points.Add(p);
                p = Tuple.Point(min.x, max.y, min.z);
                points.Add(p);
                p = Tuple.Point(min.x, max.y, max.z);
                points.Add(p);
                p = Tuple.Point(max.x, max.y, max.z);
                points.Add(p);
                p = Tuple.Point(max.x, max.y, min.z);
                points.Add(p);
                p = Tuple.Point(min.x, min.y, max.z);
                points.Add(p);
                p = Tuple.Point(max.x, min.y, max.z);
                points.Add(p);
                p = Tuple.Point(max.x, min.y, min.z);
                points.Add(p);
            }
            
            return points;
        }

        public BoundingBox Transform(Matrix transform)
        {
            Tuple tMin = Tuple.Point(0, 0, 0);
            Tuple tMax = Tuple.Point(0, 0, 0);
            List<Tuple> bBPoints = GetPoints();

            List<Tuple> tPoints = new List<Tuple>();

            foreach(Tuple point in bBPoints)
            {
                tPoints.Add(transform.Multiply(point));
            }

            foreach(Tuple tP in tPoints)
            {
                if (tP.x < tMin.x) tMin.x = tP.x;
                if (tP.y < tMin.y) tMin.y = tP.y;
                if (tP.z < tMin.z) tMin.z = tP.z;

                if (tP.x > tMax.x) tMax.x = tP.x;
                if (tP.y > tMax.y) tMax.y = tP.y;
                if (tP.z > tMax.z) tMax.z = tP.z;
            }

            BoundingBox newBB = new BoundingBox(tMin, tMax);

            return newBB;
        }

        protected override List<Intersection> LocalIntersect(Ray ray, Intersection closestHitSoFar)
        {
            List<Intersection> intersections = new List<Intersection>();

            Tuple<double, double> xt = CheckAxis(ray.origin.x, ray.direction.x, min.x, max.x);
            Tuple<double, double> yt = CheckAxis(ray.origin.y, ray.direction.y, min.y, max.y);
            Tuple<double, double> zt = CheckAxis(ray.origin.z, ray.direction.z, min.z, max.z);

            double tmin = MathsUtils.Max(xt.Item1, yt.Item1, zt.Item1);

            double tmax = MathsUtils.Min(xt.Item2, yt.Item2, zt.Item2);

            //did we hit the cube?
            if (tmin <= tmax)
            {
                Intersection int1 = new Intersection(tmin, this);
                Intersection int2 = new Intersection(tmax, this);
                intersections.Add(int1);
                intersections.Add(int2);
            }

            return intersections;
        }

        private Tuple<double, double> CheckAxis(double origin, double direction, double min, double max)
        {
            double tminNum = (min - origin);
            double tmaxNum = (max - origin);
            double tmin;
            double tmax;

            if (Math.Abs(direction) >= MathsUtils.EPSILON)
            {
                tmin = tminNum / direction;
                tmax = tmaxNum / direction;
            }
            else
            {
                tmin = tminNum * MathsUtils.INFINITY;
                tmax = tmaxNum * MathsUtils.INFINITY;
            }

            if (tmin > tmax)
            {
                double temp = tmin;
                tmin = tmax;
                tmax = temp;
            }

            return new Tuple<double, double>(tmin, tmax);
        }

        protected override Tuple GetLocalNormal(Tuple objPoint, Intersection i)
        {
            throw new NotImplementedException();
        }

        public override BoundingBox GetBoundingBox()
        {
            return this;
        }
    }
}
