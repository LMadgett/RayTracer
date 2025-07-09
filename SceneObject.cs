using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public abstract class SceneObject
    {
        protected Matrix transform;
        protected Matrix invTransform;
        protected Matrix transInvTransform;
        protected SceneObject parent = null;
        private Material material = null;
        protected Tuple centrePoint = null;

        protected abstract List<Intersection> LocalIntersect(Ray ray, Intersection closestHitSoFar);

        public List<Intersection> Intersect(Ray ray, Intersection closestHitSoFar)
        {
            // transform ray by inverse of object's transform (will move object in world space, effectively)
            Ray ray2 = ray.Transform(this.invTransform);
            List<Intersection> intersections = LocalIntersect(ray2, closestHitSoFar);

            return intersections;
        }

        public SceneObject()
        {
            //this.material = new Material(new RayColour(1, 1, 1), 1, 1, 1, 10, 0, 0, 1);
            this.material = null;
            SetTransform(Matrix.GetIdentity(4));
        }

        public Tuple GetNormal(Tuple worldPoint, Intersection i)
        {
            Tuple objPoint = MathsUtils.WorldToObject(this, worldPoint);
            Tuple objNormal = GetLocalNormal(objPoint, i);
            Tuple worldNormal = MathsUtils.NormalToWorld(this, objNormal); 
            worldNormal.w = 0;
            worldNormal = worldNormal.Normalise();
            return worldNormal;
        }

        protected abstract Tuple GetLocalNormal(Tuple objPoint, Intersection i);

        public Tuple GetCentrePoint()
        {
            if (centrePoint != null) return centrePoint;
            Tuple localCentre = GetLocalCentrePoint();
            centrePoint = localCentre.Multiply(this.invTransform);
            return centrePoint;
        }

        protected abstract Tuple GetLocalCentrePoint();

        public void SetTransform(Matrix transform)
        {
            this.transform = transform;
            this.invTransform = transform.GetInverse();
            this.transInvTransform = invTransform.GetTranspose();
        }

        public Matrix GetTransform()
        {
            return transform;
        }

        public Matrix GetInvTransform()
        {
            return invTransform;
        }

        public Material GetMaterial()
        {
            //use parent material is we don't have one
            if (this.material == null && this.parent != null)
            {
                return parent.GetMaterial();
            }

            return material;
        }

        public void SetMaterial(Material mat)
        {
            this.material = mat;
        }
        
        public SceneObject GetParent()
        {
            return parent;
        }

        public void SetParent(SceneObject parent)
        {
            this.parent = parent;
        }

        public abstract BoundingBox GetBoundingBox();
    }
}
