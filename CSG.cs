using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class CSG : SceneObject
    {   
        public enum Operation
        {
            Union,
            Intersection,
            Difference
        }

        public SceneObject left;
        public SceneObject right;
        Operation operation;

        public CSG() : base()
        {
            SetTransform(Matrix.GetIdentity(4));
        }

        public CSG(SceneObject left, SceneObject right, Operation operation)
        {
            this.left = left;
            this.left.SetParent(this);
            this.right = right;
            this.operation = operation;
            this.right.SetParent(this);
            this.SetMaterial(null);
        }

        public CSG(SceneObject left, SceneObject right, Operation operation, Material mat, Matrix transform) : this(left, right, operation)
        {
            this.SetMaterial(mat);
            this.transform = transform;
        }

        public override BoundingBox GetBoundingBox()
        {
            BoundingBox objBB;

            Matrix leftTransform = left.GetTransform();
            BoundingBox leftBB = left.GetBoundingBox();
            BoundingBox leftTransformedBB = leftBB.Transform(transform);
            Matrix righttransform = left.GetTransform();
            BoundingBox rightBB = left.GetBoundingBox();
            BoundingBox rightTransformedBB = leftBB.Transform(transform);

            Tuple min = Tuple.Point(rightBB.min.x, rightBB.min.y, rightBB.min.z);
            Tuple max = Tuple.Point(rightBB.max.x, rightBB.max.y, rightBB.max.z);

            if (leftBB.min.x < min.x) min.x = leftBB.min.x;
            if (leftBB.min.y < min.y) min.y = leftBB.min.y;
            if (leftBB.min.z < min.z) min.z = leftBB.min.z;

            if (leftBB.max.x > max.x) max.x = leftBB.max.x;
            if (leftBB.max.y > max.y) max.y = leftBB.max.y;
            if (leftBB.max.z > max.z) max.z = leftBB.max.z;

            objBB = new BoundingBox(min, max);

            return objBB;
        }

        protected override Tuple GetLocalCentrePoint()
        {
            Tuple leftCentre = left.GetCentrePoint();
            Tuple rightCentre = right.GetCentrePoint();

            Tuple centre = Tuple.Point((leftCentre.x + rightCentre.x) / 2, (leftCentre.y + rightCentre.y) / 2, (leftCentre.z + rightCentre.z) / 2);

            return centre;
        }

        protected bool IntersectionAllowed(bool leftHit, bool inLeft, bool inRight)
        {
            bool opAllowed = false;
            //leftHit == false means rightHit

            if (operation == Operation.Union)
            {
                if(leftHit && !inRight || !leftHit && !inLeft) opAllowed = true; 
            }
            else if(operation == Operation.Intersection)
            {
                if(leftHit && inRight || !leftHit && inLeft) opAllowed = true;
            }
            else if(operation == Operation.Difference) 
            {
                if (leftHit && !inRight || !leftHit && inLeft) opAllowed = true;
            }

            return opAllowed;
        }

        protected List<Intersection> FilterIntersections(List<Intersection> intersections)
        {
            List<Intersection> result = new List<Intersection>();

            bool inLeft = false;
            bool inRight = false;

            foreach(Intersection intersection in intersections)
            {
                bool leftHit;
                if (intersection.topLevelSceneObject == left)
                {
                    leftHit = true;
                    inLeft = intersection.wasInside;
                }
                else if (intersection.topLevelSceneObject == right)
                {
                    leftHit = false;
                    inRight = intersection.wasInside;
                }
                else throw new Exception("CSG.FilterIntersections did not find a topLevelSceneObject");

                if (IntersectionAllowed(leftHit, inLeft, inRight))
                {
                    result.Add(intersection);
                }

                if (leftHit)
                {
                    inLeft = !inLeft;
                }
                else
                {
                    inRight = !inRight;
                }
            }

            return result;
        }

        protected override Tuple GetLocalNormal(Tuple objPoint, Intersection i)
        {
            //should never get called (hopefully!)
            throw new NotImplementedException();
        }

        protected override List<Intersection> LocalIntersect(Ray ray, Intersection closestHitSoFar)
        {
            List<Intersection> leftInts = left.Intersect(ray, closestHitSoFar);
            foreach(Intersection intersection in leftInts)
            {
                intersection.topLevelSceneObject = left;

                Tuple intPoint = ray.Position(intersection.t);
                Tuple intNorm = intersection.sceneObject.GetNormal(intPoint, intersection);
                bool inside = false;
                if(ray.direction.Dot(intNorm) >= 0) inside = true;
                intersection.wasInside = inside;
            }

            List<Intersection> rightInts = right.Intersect(ray, closestHitSoFar);
            foreach (Intersection intersection in rightInts)
            {
                intersection.topLevelSceneObject = right;

                Tuple intPoint = ray.Position(intersection.t);
                Tuple intNorm = intersection.sceneObject.GetNormal(intPoint, intersection);
                bool inside = false;
                if (ray.direction.Dot(intNorm) >= 0) inside = true;
                intersection.wasInside = inside;
            }

            leftInts.AddRange(rightInts);
            Intersection.SortIntersections(leftInts);

            List<Intersection> result = FilterIntersections(leftInts);

            return result;
        }
    }
}
