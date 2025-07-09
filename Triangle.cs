using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    internal class Triangle : SceneObject
    {
        private Tuple p1, p2, p3;
        private Tuple e1, e2, normal, n1, n2, n3;
        private bool isSmooth = false;

        public Triangle(Tuple v1, Tuple v2, Tuple v3)
        {
            SetVertices(v1, v2, v3);
            SetTransform(Matrix.GetIdentity(4));
        }

        public Triangle(Tuple v1, Tuple v2, Tuple v3, Tuple n1, Tuple n2, Tuple n3) : this(v1, v2, v3)
        {
            isSmooth = true;
            this.n1 = n1;
            this.n2 = n2;
            this.n3 = n3;
        }

        protected override Tuple GetLocalCentrePoint()
        {
            Tuple centre = Tuple.Point((p1.x + p2.x + p3.x)/3.0, (p1.y + p2.y + p3.y) / 3.0, (p1.z + p2.z + p3.z) / 3.0);
            return centre;
        }

        protected override List<Intersection> LocalIntersect(Ray ray, Intersection closestHitSoFar)
        {
            List<Intersection> intersections = new List<Intersection>();

            Tuple dirCrossE2 = ray.direction.Cross(e2);
            double det = e1.Dot(dirCrossE2);
            if (MathsUtils.Equal(det, 0.0))
            {
                return intersections;
            }

            double f = 1.0 / det;
            Tuple p1ToOrigin = ray.origin.Sub(p1);
            double u = f * p1ToOrigin.Dot(dirCrossE2);
            if(u < 0.0 || u > 1.0)
            {
                return intersections;
            }

            Tuple originCrossE1 = p1ToOrigin.Cross(e1);
            double v = f * ray.direction.Dot(originCrossE1);
            if(v < 0.0 || (u + v) > 1.0)
            {
                return intersections;
            }

            double t = f * e2.Dot(originCrossE1);
            Intersection i;

            if (!isSmooth) i = new Intersection(t, this);
            else i = new Intersection(t, this, u, v);
            
            intersections.Add(i);

            return intersections;
        }

        protected override Tuple GetLocalNormal(Tuple objPoint, Intersection i)
        {
            if(!isSmooth) return normal;

            Tuple n = n2.Multiply(i.u).Add(n3.Multiply(i.v)).Add(n1.Multiply(1.0 - i.u - i.v));

            return n;
        }

        public void SetVertices(Tuple p1, Tuple p2, Tuple p3) 
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            CalculateEdgesAndNormal();
        }

        private void CalculateEdgesAndNormal()
        {
            e1 = p2.Sub(p1);
            e2 = p3.Sub(p1);

            normal = e2.Cross(e1).Normalise();
        }

        public override BoundingBox GetBoundingBox()
        {
            BoundingBox bB = new BoundingBox(Tuple.Point(MathsUtils.Min(p1.x, p2.x, p3.x), MathsUtils.Min(p1.y, p2.y, p3.y), MathsUtils.Min(p1.z, p2.z, p3.z)), Tuple.Point(MathsUtils.Max(p1.x, p2.x, p3.x), MathsUtils.Max(p1.y, p2.y, p3.y), MathsUtils.Max(p1.z, p2.z, p3.z)));

            return bB;
        }
    }
}
