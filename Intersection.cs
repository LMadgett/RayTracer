using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class Intersection
    {
        public double t;
        public SceneObject sceneObject;
        public SceneObject topLevelSceneObject; //used in CSG to track if left or right scene object was top level intersect
        public bool uvSet = false;
        public double u;
        public double v;
        public bool wasInside = false;

        public Intersection (double t, SceneObject sceneObject)
        {
            this.t = t;
            this.sceneObject = sceneObject;
        }

        public Intersection(double t, SceneObject sceneObject, double u, double v) : this (t, sceneObject)
        {
            this.u = u;
            this.v = v;
        }

        public static void SortIntersections(List<Intersection> intersections)
        {
            intersections.Sort((x, y) => x.t.CompareTo(y.t));
        }
    }
}
