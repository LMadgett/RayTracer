using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class Cone : SceneObject
    {
        double min, max;
        bool closedBot, closedTop;

        public Cone(double min, double max, bool closedBot, bool closedTop)
        {
            this.min = min;
            this.max = max;
            this.closedBot = closedBot;
            this.closedTop = closedTop;
            SetTransform(Matrix.GetIdentity(4));
        }

        protected override Tuple GetLocalCentrePoint()
        {
            Tuple centre = Tuple.Point(0.0, (min + max) / 2.0, 0.0);
            return centre;
        }

        protected override List<Intersection> LocalIntersect(Ray ray, Intersection closestHitSoFar)
        {
            List<Intersection> intersections = new List<Intersection>();

            double a = Math.Pow(ray.direction.x, 2) - Math.Pow(ray.direction.y, 2) + Math.Pow(ray.direction.z, 2);
            double b = (2.0 * ray.origin.x * ray.direction.x) - (2.0 * ray.origin.y * ray.direction.y) + (2.0 * ray.origin.z * ray.direction.z);
            double c = Math.Pow(ray.origin.x, 2) - Math.Pow(ray.origin.y, 2) + Math.Pow(ray.origin.z, 2);

            if (MathsUtils.Equal(a, 0.0))
            {
                if(!MathsUtils.Equal(b, 0.0))
                {
                    double t = -c / (2.0 * b);
                    Intersection i = new Intersection(t, this);
                    intersections.Add(i);
                }
            }
            else
            {
                double disc = Math.Pow(b, 2) - (4.0 * a * c);
                if (disc >= 0)
                {
                    double t0 = (-b - Math.Sqrt(disc)) / (2 * a);
                    double t1 = (-b + Math.Sqrt(disc)) / (2 * a);

                    if (t0 > t1)
                    {
                        double temp = t0;
                        t0 = t1;
                        t1 = temp;
                    }

                    double y0 = ray.origin.y + (t0 * ray.direction.y);
                    double y1 = ray.origin.y + (t1 * ray.direction.y);

                    if (min < y0 && y0 < max)
                    {
                        Intersection i0 = new Intersection(t0, this);
                        intersections.Add(i0);
                    }
                    if (min < y1 && y1 < max)
                    {
                        Intersection i1 = new Intersection(t1, this);
                        intersections.Add(i1);
                    }
                }
            }
            IntersectCaps(ray, intersections);

            return intersections;
        }

        private bool CheckCap(Ray r, double t, double yRad)
        {
            double x = r.origin.x + (t * r.direction.x);
            double z = r.origin.z + (t * r.direction.z);

            bool isHit = Math.Pow(x, 2) + Math.Pow(z, 2) <= yRad;
            return isHit;
        }

        private void IntersectCaps(Ray r, List<Intersection> intersections)
        {
            if (closedTop == false && closedBot == false) return;
            if (MathsUtils.Equal(r.direction.y, 0.0)) return;

            double t = (min - r.origin.y) / r.direction.y;
            if (CheckCap(r, t, min))
            {
                Intersection i = new Intersection(t, this);
                intersections.Add(i);
            }

            t = (max - r.origin.y) / r.direction.y;
            if (CheckCap(r, t, max))
            {
                Intersection i = new Intersection(t, this);
                intersections.Add(i);
            }
        }

        protected override Tuple GetLocalNormal(Tuple objPoint, Intersection i)
        {
            Tuple objectNormal;
            double dist = Math.Pow(objPoint.x, 2) + Math.Pow(objPoint.z, 2);
            if (dist < 1.0 && objPoint.y >= max - MathsUtils.EPSILON) objectNormal = Tuple.Vector(0.0, 1.0, 0.0);
            else if (dist < 1.0 && objPoint.y <= min + MathsUtils.EPSILON) objectNormal = Tuple.Vector(0.0, -1.0, 0.0);
            else
            {
                double y = Math.Sqrt(Math.Pow(objPoint.x, 2) + Math.Pow(objPoint.x, 2));
                if(objPoint.y > 0.0)
                {
                    y = -y;
                }
                objectNormal = Tuple.Vector(objPoint.x, y, objPoint.z);
            }

            return objectNormal;
        }

        public override BoundingBox GetBoundingBox()
        {
            BoundingBox bB = new BoundingBox(Tuple.Point(-1, min, -1), Tuple.Point(1, max, 1));

            return bB;
        }
    }
}
