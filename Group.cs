using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class Group : SceneObject
    {
        List<SceneObject> children = new List<SceneObject>();
        BoundingBox objBB = null;
        Object bbLock = new Object();
        
        public void AddChild(SceneObject child)
        {
            children.Add(child);
            child.SetParent(this);
        }

        protected override Tuple GetLocalNormal(Tuple objPoint, Intersection i)
        {
            //should never get called (hopefully!)
            throw new NotImplementedException();
        }

        protected override Tuple GetLocalCentrePoint()
        {
            Tuple totCentre = Tuple.Point(0, 0, 0);
            int numChildren = 0;
            foreach (SceneObject child in children)
            {
                Tuple childCentre = child.GetCentrePoint();
                Tuple childCentreVector = Tuple.Vector(childCentre.x, childCentre.y, childCentre.z);
                totCentre = totCentre.Add(childCentreVector);
                numChildren++;
            }
            Tuple centre = totCentre.Divide(numChildren);
            return centre;
        }

        protected override List<Intersection> LocalIntersect(Ray ray, Intersection closestHitSoFar)
        {
            List<Intersection> groupInts = new List<Intersection>();

            BoundingBox bb = GetBoundingBox();
            List<Intersection> intersects = bb.Intersect(ray, closestHitSoFar);
            if (intersects.Count > 0)
            {
                bool doHits = false;
                if (closestHitSoFar == null)
                {
                    doHits = true;
                }
                else if (intersects.Count == 1 && intersects[0].t <= closestHitSoFar.t)
                {
                    doHits = true;
                }
                else if (intersects.Count == 2)
                {
                    if (intersects[0].t < intersects[1].t && intersects[0].t <= closestHitSoFar.t)
                    {
                        doHits = true;
                    }
                    else if (intersects[1].t <= intersects[0].t && intersects[1].t <= closestHitSoFar.t)
                    {
                        doHits = true;
                    }
                }

                if (doHits)
                {
                    foreach (SceneObject child in children)
                    {
                        List<Intersection> childInts = child.Intersect(ray, closestHitSoFar);
                        if (childInts.Count > 0)
                        {
                            if (closestHitSoFar == null)
                            {
                                if (childInts.Count == 1)
                                {
                                    closestHitSoFar = childInts[0];
                                }
                                else if (childInts.Count == 2)
                                {
                                    if (childInts[0].t < childInts[1].t)
                                    {
                                        closestHitSoFar = childInts[0];
                                    }
                                    else if (childInts[1].t <= childInts[0].t)
                                    {
                                        closestHitSoFar = childInts[1];
                                    }
                                }
                            }
                            else if (childInts.Count == 1 && childInts[0].t <= closestHitSoFar.t)
                            {
                                closestHitSoFar = childInts[0];
                            }
                            else if (childInts.Count == 2)
                            {
                                if (childInts[0].t < childInts[1].t && childInts[0].t <= closestHitSoFar.t)
                                {
                                    closestHitSoFar = childInts[0];
                                }
                                else if (childInts[1].t <= childInts[0].t && childInts[1].t <= closestHitSoFar.t)
                                {
                                    closestHitSoFar = childInts[1];
                                }
                            }
                        }
                        groupInts.AddRange(childInts);
                    }

                    Intersection.SortIntersections(groupInts);
                }
            }

            return groupInts;
        }

        public Group Subdivide(int recursion)
        {
            Group parentGroup = new Group();
            parentGroup.parent = this.parent;
            parentGroup.SetMaterial(this.GetMaterial());
            parentGroup.SetTransform(this.GetTransform());

            BoundingBox bb = GetBoundingBox();
            List<SceneObject> childrenToAdd = new List<SceneObject>();
            childrenToAdd.AddRange(children);
            Console.WriteLine("r=" + recursion + " childrenToAdd.count=" + childrenToAdd.Count);

            double minx = bb.min.x;
            double maxx = bb.max.x;
            double halfx = (maxx - minx) / 2.0;
            double miny = bb.min.y;
            double maxy = bb.max.y;
            double halfy = (maxy - miny) / 2.0;
            double minz = bb.min.z;
            double maxz = bb.max.z;
            double halfz = (maxz - minz) / 2.0;

            double x = minx;
            double y = miny;
            double z = minz;
            for (int iz = 0; iz < 2; iz++)
            {
                y = miny;
                for (int iy = 0; iy < 2; iy++)
                {
                    x = minx;
                    for (int ix = 0; ix < 2; ix++)
                    {
                        Group subGroup = new Group();

                        for (int i = childrenToAdd.Count - 1; i >= 0; i--)
                        {
                            SceneObject child = childrenToAdd[i];
                            Tuple childCentre = child.GetCentrePoint();
                            if (childCentre.x >= x && childCentre.x < x + halfx &&
                                childCentre.y >= y && childCentre.y < y + halfy &&
                                childCentre.z >= z && childCentre.z < z + halfz)
                            {
                                // subdivide any child groups
                                if (child is Group && recursion > 0)
                                {
                                    child = ((Group)child).Subdivide(recursion - 1);
                                }

                                subGroup.AddChild(child);
                                childrenToAdd.RemoveAt(i);
                            }
                        }

                        if (subGroup.children.Count > 0)
                        {
                            Console.WriteLine("r=" + recursion + " subgroup (ix,iy,iz)=(" + ix + "," + iy + "," + iz + ") child count=" + subGroup.children.Count);
                            if (recursion > 0)
                            {
                                subGroup = subGroup.Subdivide(recursion - 1);
                            }
                            parentGroup.AddChild(subGroup);
                        }

                        x += halfx;
                    }

                    y += halfy;
                }

                z += halfz;
            }

            return parentGroup;
        }

        public override BoundingBox GetBoundingBox()
        {
            if (objBB != null) return objBB;

            lock (bbLock)
            {
                if (objBB == null)
                {
                    List<BoundingBox> tCBBs = new List<BoundingBox>();

                    foreach (SceneObject child in children)
                    {
                        Matrix transform = child.GetTransform();
                        BoundingBox cBB = child.GetBoundingBox();
                        BoundingBox tBB = cBB.Transform(transform);
                        tCBBs.Add(tBB);
                    }

                    Tuple min = Tuple.Point(0, 0, 0);
                    Tuple max = Tuple.Point(0, 0, 0);

                    foreach (BoundingBox tCBB in tCBBs)
                    {
                        if (tCBB.min.x < min.x) min.x = tCBB.min.x;
                        if (tCBB.min.y < min.y) min.y = tCBB.min.y;
                        if (tCBB.min.z < min.z) min.z = tCBB.min.z;

                        if (tCBB.max.x > max.x) max.x = tCBB.max.x;
                        if (tCBB.max.y > max.y) max.y = tCBB.max.y;
                        if (tCBB.max.z > max.z) max.z = tCBB.max.z;
                    }

                    objBB = new BoundingBox(min, max);
                }
            }

            return objBB;
        }
    }
}
