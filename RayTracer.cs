using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Raylib_cs;
using static Raylib_cs.PixelFormat;

namespace Raytracer
{
    public unsafe class RayTracer : PixelPlotter
    {
        RayCanvas canvas;
        Scene scene;

        int sampleSize = 1;

        public static void Main()
        {
            RunTests();

            RayTracer plotter = (RayTracer)ConstructPlotter();
            plotter.SetupPlotter();
            plotter.RunPlotter();
        }

        protected static PixelPlotter ConstructPlotter()
        {
            return new RayTracer();
        }

        protected new void SetupPlotter()
        {
            screenPixelWidth = 1024;
            screenPixelHeight = 1024;
            taskPixelWidth = 64;
            taskPixelHeight = 64;

            base.SetupPlotter();

            canvas = new RayCanvas(screenPixelWidth, screenPixelHeight);

            // create a Scene
            scene = new Scene();
            scene.maxRecursionDepth = 8;

            // set up camera
            Camera cam = new Camera();
            cam.position = Tuple.Point(10, 10, -20);
            cam.lookAt = Tuple.Point(0, 0, 0);
            cam.screenDist = 10;
            cam.screenPixelW = screenPixelWidth;    // from PixelPlotter
            cam.screenPixelH = screenPixelHeight;   // from PixelPlotter
            cam.screenWidth = 10;
            cam.screenHeight = (cam.screenWidth / screenPixelWidth) * screenPixelHeight;    // keep in ratio
            cam.focalDistance = 25.0;
            cam.apertureSize = 0.3;
            cam.apertureSampling = 7;
            cam.Initialise();
            scene.camera = cam;

            // set up light
            Light light = new Light(new RayColour(1, 1, 1), Tuple.Point(40, 40, -40));
            scene.lights.Add(light);
            //light = new Light(new RayColour(0.5, 0.5, 0.5), Tuple.Point(0, 0, -10));
            //scene.lights.Add(light);

            // add objects to scene
            //Matrix transform = Matrix.GetTranslation(0, 0, 0);
            //transform = transform.Multiply(Matrix.GetScaling(4, 4, 4));
            //Material mat = new Material(new RayColour(1, 0.1, 0.1), 0.1, 1, 1, 1000, 0.7, 0, 1.52);
            //Sphere sphere = new Sphere(mat, transform);
            ////scene.AddObject(sphere);

            //transform = Matrix.GetTranslation(0, 4, 0);
            //transform = transform.Multiply(Matrix.GetScaling(2, 2, 2));
            //transform = transform.Multiply(Matrix.GetRotateY(45));
            //mat = new Material(new RayColour(0.1, 1, 0.1), 0.1, 1, 1, 1000, 0.7, 0, 1);
            //Cube cube1 = new Cube(mat, transform);
            ////scene.AddObject(cube1);

            //SceneObject csgObj = new CSG(sphere, cube1, CSG.Operation.Difference, null, Matrix.GetIdentity(4));
            //scene.AddObject(csgObj);


            Matrix transform = Matrix.GetTranslation(0, 0, 10);
            transform = transform.Multiply(Matrix.GetScaling(4, 4, 4));
            Material mat = new Material(new RayColour(0.8, 0.3, 0.1), 0.2, 1, 1, 1000, 0, 0, 1);// 0.8, 0.95, 1.52);
            Sphere sphere1 = new Sphere();
            sphere1.SetMaterial(mat);
            sphere1.SetTransform(transform);
            transform = Matrix.GetTranslation(0, 0, 6);
            transform = transform.Multiply(Matrix.GetScaling(2, 2, 2));
            mat = new Material(new RayColour(0.3, 0.8, 0.1), 0.2, 1, 1, 1000, 0, 0, 1);//0.8, 0.95, 1.52);
            Sphere sphere2 = new Sphere();
            sphere2.SetMaterial(mat);
            sphere2.SetTransform(transform);
            SceneObject csgObj = new CSG(sphere1, sphere2, CSG.Operation.Difference, null, Matrix.GetIdentity(4));
            //scene.objects.Add(csgObj);

            Group group = new Group();
            transform = Matrix.GetIdentity(4);
            group.SetTransform(transform);

            transform = Matrix.GetTranslation(0, 0, 50);
            transform = transform.Multiply(Matrix.GetScaling(1, 1, 1));
            transform = transform.Multiply(Matrix.GetRotateY(30));
            transform = transform.Multiply(Matrix.GetRotateX(0));
            mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.1, 1, 1, 1000, 0, 0, 1);
            Cube cube = new Cube();
            cube.SetMaterial(mat);
            cube.SetTransform(transform);
            group.AddChild(cube);

            transform = Matrix.GetTranslation(0, 0, 40);
            transform = transform.Multiply(Matrix.GetScaling(1, 1, 1));
            transform = transform.Multiply(Matrix.GetRotateY(30));
            transform = transform.Multiply(Matrix.GetRotateX(0));
            mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.1, 1, 1, 1000, 0, 0, 1);
            cube = new Cube();
            cube.SetMaterial(mat);
            cube.SetTransform(transform);
            group.AddChild(cube);

            transform = Matrix.GetTranslation(0, 0, 30);
            transform = transform.Multiply(Matrix.GetScaling(1, 1, 1));
            transform = transform.Multiply(Matrix.GetRotateY(30));
            transform = transform.Multiply(Matrix.GetRotateX(0));
            mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.1, 1, 1, 1000, 0, 0, 1);
            cube = new Cube();
            cube.SetTransform(transform);
            cube.SetMaterial(mat);
            group.AddChild(cube);

            transform = Matrix.GetTranslation(0, 0, 20);
            transform = transform.Multiply(Matrix.GetScaling(1, 1, 1));
            transform = transform.Multiply(Matrix.GetRotateY(30));
            transform = transform.Multiply(Matrix.GetRotateX(0));
            mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.1, 1, 1, 1000, 0, 0, 1);
            cube = new Cube();
            cube.SetMaterial(mat);
            cube.SetTransform(transform);
            group.AddChild(cube);

            transform = Matrix.GetTranslation(0, 0, 10);
            transform = transform.Multiply(Matrix.GetScaling(1, 1, 1));
            transform = transform.Multiply(Matrix.GetRotateY(30));
            transform = transform.Multiply(Matrix.GetRotateX(0));
            mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.1, 1, 1, 1000, 0, 0, 1);
            cube = new Cube();
            cube.SetMaterial(mat);
            cube.SetTransform(transform);
            group.AddChild(cube);

            transform = Matrix.GetTranslation(0, 0, 0);
            transform = transform.Multiply(Matrix.GetScaling(1, 1, 1));
            transform = transform.Multiply(Matrix.GetRotateY(30));
            transform = transform.Multiply(Matrix.GetRotateX(0));
            mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.1, 1, 1, 1000, 0, 0, 1);
            cube = new Cube();
            cube.SetMaterial(mat);
            cube.SetTransform(transform);
            group.AddChild(cube);

            transform = Matrix.GetTranslation(0, 0, -10);
            transform = transform.Multiply(Matrix.GetScaling(1, 1, 1));
            transform = transform.Multiply(Matrix.GetRotateY(30));
            transform = transform.Multiply(Matrix.GetRotateX(0));
            mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.1, 1, 1, 1000, 0, 0, 1);
            cube = new Cube();
            cube.SetMaterial(mat);
            cube.SetTransform(transform);
            group.AddChild(cube);

            transform = Matrix.GetTranslation(0, 0, -20);
            transform = transform.Multiply(Matrix.GetScaling(1, 1, 1));
            transform = transform.Multiply(Matrix.GetRotateY(30));
            transform = transform.Multiply(Matrix.GetRotateX(0));
            mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.1, 1, 1, 1000, 0, 0, 1);
            cube = new Cube();
            cube.SetMaterial(mat);
            cube.SetTransform(transform);
            group.AddChild(cube);

            scene.objects.Add(group);

            //transform = Matrix.GetTranslation(0, 0, -8);
            //transform = transform.Multiply(Matrix.GetScaling(4, 4, 4));
            //mat = new Material(new RayColour(0.1, 0.1, 0.1), 0.1, 1, 1, 1000, 0.8, 0.95, 1.52);
            //sphere1 = new Sphere(mat, transform);
            //transform = Matrix.GetTranslation(0, 0, -1);
            //transform = transform.Multiply(Matrix.GetScaling(4, 4, 4));
            //mat = new Material(new RayColour(0.1, 0.1, 0.1), 0.1, 1, 1, 1000, 0.8, 0.95, 1.52);
            //sphere2 = new Sphere(mat, transform);
            //csgObj = new CSG(sphere1, sphere2, CSG.Operation.Intersection, null, Matrix.GetIdentity(4));
            //scene.AddObject(csgObj);

            //Group group = new Group();
            //transform = Matrix.GetScaling(0.5, 0.5, 0.5);
            ////transform = transform.Multiply(Matrix.GetRotateX(-90));
            //group.SetTransform(transform);
            //group.AddChild(cube1);
            //group.AddChild(cube2);
            //scene.AddObject(group);

            //Group superGroup = new Group();
            //superGroup.AddChild(group);

            //Group cloneGroup = group.Duplicate();
            //transform = Matrix.GetScaling(0.5, 0.5, 0.5);
            //transform = transform.Multiply(Matrix.GetRotateZ(45));
            //cloneGroup.SetTransform(transform);
            //superGroup.AddChild(cloneGroup);

            //cloneGroup = group.Duplicate();
            //transform = Matrix.GetScaling(0.5, 0.5, 0.5);
            //transform = transform.Multiply(Matrix.GetRotateZ(90));
            //cloneGroup.SetTransform(transform);
            //superGroup.AddChild(cloneGroup);

            //cloneGroup = group.Duplicate();
            //transform = Matrix.GetScaling(0.5, 0.5, 0.5);
            //transform = transform.Multiply(Matrix.GetRotateZ(135));
            //cloneGroup.SetTransform(transform);
            //superGroup.AddChild(cloneGroup);

            //cloneGroup = group.Duplicate();
            //transform = Matrix.GetScaling(0.5, 0.5, 0.5);
            //transform = transform.Multiply(Matrix.GetRotateZ(180));
            //cloneGroup.SetTransform(transform);
            //superGroup.AddChild(cloneGroup);

            //cloneGroup = group.Duplicate();
            //transform = Matrix.GetScaling(0.5, 0.5, 0.5);
            //transform = transform.Multiply(Matrix.GetRotateZ(225));
            //cloneGroup.SetTransform(transform);
            //superGroup.AddChild(cloneGroup);

            //cloneGroup = group.Duplicate();
            //transform = Matrix.GetScaling(0.5, 0.5, 0.5);
            //transform = transform.Multiply(Matrix.GetRotateZ(270));
            //cloneGroup.SetTransform(transform);
            //superGroup.AddChild(cloneGroup);

            //cloneGroup = group.Duplicate();
            //transform = Matrix.GetScaling(0.5, 0.5, 0.5);
            //transform = transform.Multiply(Matrix.GetRotateZ(315));
            //cloneGroup.SetTransform(transform);
            //superGroup.AddChild(cloneGroup);

            //scene.AddObject(superGroup);

            //mat = new Material(new RayColour(0.9, 0.1, 0.2), 1.0, 1.0, 1, 1000, 0, 0, 1);
            //transform = Matrix.GetTranslation(-3, 0, 0);
            //transform = transform.Multiply(Matrix.GetScaling(2, 2, 2));
            //Triangle tri = new Triangle(Tuple.Point(-1, 1, 0), Tuple.Point(0, 1, 0), Tuple.Point(0, 0, 0), mat, transform);
            //scene.AddObject(tri);

            //transform = Matrix.GetTranslation(0, 0, 0);
            //transform = transform.Multiply(Matrix.GetScaling(2, 2, 2));
            //mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.2, 1.0, 1, 1000, 0, 0, 1);
            //Cylinder cyl = new Cylinder(0.0, 1.0, true, true, mat, transform);
            //scene.AddObject(cyl);

            //transform = Matrix.GetTranslation(-6, 2, 0);
            //transform = transform.Multiply(Matrix.GetScaling(2, 2, 2));
            //mat = new Material(new RayColour(0.8, 0.1, 0.2), 0.2, 1.0, 1, 1000, 0, 0, 1);
            //Cone cone = new Cone(-1.0, 0.0, true, true, mat, transform);
            //scene.AddObject(cone);

            //transform = Matrix.GetTranslation(0, 0, 0);
            //transform = transform.Multiply(Matrix.GetScaling(4, 4, 4));
            //mat = new Material(new RayColour(0.1, 0.1, 0.1), 0, 0, 0, 0, 0, 1, 1.00029);
            //sphere = new Sphere(mat, transform);
            //scene.AddObject(sphere);

            //transform = Matrix.GetTranslation(0, 0, 12);
            //transform = transform.Multiply(Matrix.GetScaling(3, 3, 3));
            //mat = new Material(new RayColour(0.8, 0.2, 0.1), 0.2, 1.0, 1, 1000, 0.9, 0, 1);
            //sphere = new Sphere(mat, transform);
            //scene.AddObject(sphere);

            //transform = Matrix.GetTranslation(-4, 0, 12);
            //transform = transform.Multiply(Matrix.GetScaling(4, 4, 4));
            //mat = new Material(new RayColour(0.2, 0.8, 0.1), 0.2, 1.0, 1, 1000, 0, 0, 1);
            //Sphere sphere = new Sphere(mat, transform);
            //scene.AddObject(sphere);

            //transform = Matrix.GetTranslation(15, 0, 12);
            //transform = transform.Multiply(Matrix.GetScaling(12, 4, 4));
            //mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.2, 1.0, 1, 1000, 0.9, 0, 1);
            //sphere = new Sphere(mat, transform);
            //scene.AddObject(sphere);

            //Matrix patTransform = Matrix.GetScaling(4, 4, 4);//.Multiply(Matrix.GetRotateX(0));
            //Pattern pattern = new Checker2DPattern(new RayColour(0.3, 0.8, 0.2), new RayColour(0.2, 0.4, 0.9), patTransform);
            //transform = Matrix.GetTranslation(0, -8, 0);
            //mat = new Material(new RayColour(0.2, 0.5, 0.5), 0.1, 1.0, 1, 400, 0, 0, 1, pattern);
            //Plane plane = new Plane();
            //plane.SetMaterial(mat);
            //plane.SetTransform(transform);

            //scene.objects.Add(plane);

            //Group obj = OBJParser.ParsOBJFile("terrain.obj");
            //mat = new Material(new RayColour(0.2, 0.1, 0.8), 0.2, 1.0, 1, 1000, 0.9, 0, 1);
            //obj.SetMaterial(mat);
            //transform = Matrix.GetScaling(0.1, 0.1, 0.1);
            //obj.SetTransform(transform);
            //obj = obj.Subdivide(3);

            //scene.objects.Add(obj);
        }

        public new void RunPlotter()
        {
            base.RunPlotter();
        }

        // overrides PixelPlotter.ConstructTaskParams()
        protected override PixelTaskParams ConstructTaskParams(int pixelX, int pixelY, int pixelWidth, int pixelHeight,
                                                          Color* taskPixels, int freeTaskPixelIdx)
        {
            // bodge bodge! testing
            Color taskColor = Color.RED;
            if ((freeTaskPixelIdx & 1) == 1) taskColor = Color.BLUE;
            // end of bodge bodge testing :)

            return new RaytracerTaskParams(pixelX, pixelY, pixelWidth, pixelHeight, canvas, taskPixels, freeTaskPixelIdx, scene, taskColor);
        }

        // Called via a Task so runs multi-threaded
        // Computes the pixels for the given taskParams rectangle
        // Overrides PixelPlotter.ComputePixels()
        protected override PixelTaskParams ComputePixels(PixelTaskParams taskParams)
        {
            // casting because we "know" this is Raytracer
            RaytracerTaskParams rayParams = (RaytracerTaskParams)taskParams;

            Scene scene = rayParams.scene;
            Camera cam = scene.camera;
            

            double sampleInc = 1.0 / sampleSize;

            for (int y = 0; y < rayParams.pixelHeight; y++)
            {
                for (int x = 0; x < rayParams.pixelWidth; x++)
                {
                    RayColour totCol = new RayColour(0, 0, 0);
                    for (double sY = y; sY < y + 1.0; sY = sY + sampleInc)
                    {
                        for (double sX = x; sX < x + 1.0; sX = sX + sampleInc)
                        {
                            RayColour rcol = new RayColour(0, 0, 0);
                            List<Ray> cameraRays = cam.GetRay(sX + rayParams.pixelX, sY + rayParams.pixelY);
                            int rayCount = 0;

                            foreach(Ray cameraRay in cameraRays)
                            {
                                rcol = rcol.Add(ColourAt(scene, cameraRay, scene.maxRecursionDepth));
                                rayCount++;
                            }
                            //avg rays across aperture and track total for antialiasing
                            rcol = rcol.Divide(rayCount);
                            totCol = totCol.Add(rcol);
                        }
                    }

                    totCol = totCol.Divide(sampleSize * sampleSize).Clamp();

                    canvas.SetColour(x + rayParams.pixelX, y +  rayParams.pixelY, totCol);
                }
            }

            canvas.CopyRectangle(rayParams.pixelX, rayParams.pixelY, rayParams.pixelWidth, rayParams.pixelHeight, rayParams.taskPixels);

            return rayParams;
        }

        private List<Intersection> IntersectScene(Scene scene, Ray ray)
        {
            List<SceneObject> objects = scene.objects;
            List<Intersection> allIntersects = new List<Intersection>();
            Intersection closestHitSoFar = null;

            foreach (SceneObject obj in objects)
            {
                List<Intersection> intersects = obj.Intersect(ray, closestHitSoFar);
                allIntersects.AddRange(intersects);
                Intersection thisClosestHit = Hit(intersects);
                if(thisClosestHit != null && (closestHitSoFar == null || thisClosestHit.t < closestHitSoFar.t))
                {
                    closestHitSoFar = thisClosestHit;
                }
            }

            Intersection.SortIntersections(allIntersects);

            return allIntersects;
        }

        private Comps PrepareComputations(Intersection intersect, Ray ray, List<Intersection> allIntersects)
        {
            Comps comps = new Comps();
            comps.t = intersect.t;
            comps.sceneObject = intersect.sceneObject;
            comps.point = ray.Position(comps.t);
            comps.eyeDir = ray.direction.Negate();
            comps.normal = comps.sceneObject.GetNormal(comps.point, intersect);
            comps.inside = false;
            comps.reflectV = Tuple.Reflect(ray.direction, comps.normal);

            if(comps.normal.Dot(comps.eyeDir) < 0)
            {
                comps.inside = true;
                comps.normal = comps.normal.Negate();
            }

            Tuple miniNormal = comps.normal.Multiply(MathsUtils.EPSILON);
            if (!comps.inside)
            {
                comps.overPoint = comps.point.Add(miniNormal);

                comps.underPoint = comps.point.Sub(miniNormal);
            }
            else
            {
                comps.overPoint = comps.point.Sub(miniNormal);

                comps.underPoint = comps.point.Add(miniNormal);

            }

            FindRIsForHit(comps, intersect, allIntersects);

            return comps;
        }

        //finds the refractive indices of the scene objects either side of the hit (see page 153)
        private void FindRIsForHit(Comps comps, Intersection intersect, List<Intersection> allIntersects) 
        {
            List<SceneObject> intersectedObjects = new List<SceneObject>();
            
            foreach(Intersection i in allIntersects)
            {
                if(i == intersect)
                {
                    if(intersectedObjects.Count == 0)
                    {
                        comps.rI1 = 1.0;
                    }
                    else
                    {
                        comps.rI1 = intersectedObjects[intersectedObjects.Count - 1].GetMaterial().refractiveIndex;
                    }
                }

                if(intersectedObjects.Contains(i.sceneObject))
                {
                    intersectedObjects.Remove(i.sceneObject);
                }
                else
                {
                    intersectedObjects.Add(i.sceneObject);
                }

                if(i == intersect)
                {
                    if(intersectedObjects.Count == 0)
                    {
                        comps.rI2 = 1.0;
                    }
                    else
                    {
                        comps.rI2 = intersectedObjects[intersectedObjects.Count - 1].GetMaterial().refractiveIndex;
                    }

                    break;
                }
            }
        }

        // returns null if all intersections are behind camera (-ve t) OR List is empty
        private Intersection Hit(List<Intersection> intersections)
        {
            Intersection hit = null;

            foreach (Intersection intersection in intersections)
            {
                if (intersection.t >= 0)
                {
                    if (hit == null)
                        hit = intersection;
                    else if (hit.t > intersection.t)
                        hit = intersection;
                }
            }
            return hit;
        }

        private RayColour IsShadowed(Scene scene, Light light, Comps comps) 
        {
            RayColour isShadowed = new RayColour(1, 1, 1);
            Tuple lightDir = light.position.Sub(comps.point);
            double lightDist = lightDir.Magnitude();
            lightDir = lightDir.Normalise();
            Ray lightRay = new Ray(comps.overPoint, lightDir);

            List<Intersection> intersections = IntersectScene(scene, lightRay);

            foreach(Intersection hit in intersections)
            {
                if(hit.t >= 0 && hit.t < lightDist)
                {
                    Material mat = hit.sceneObject.GetMaterial();
                    isShadowed = isShadowed.Multiply(NormaliseColour(mat.colour));
                    isShadowed = isShadowed.Multiply(mat.transparency);
                    if (MathsUtils.Equal(mat.transparency, 0.0))
                    {
                        break;
                    }
                }
            }

            return isShadowed;
        }

        private RayColour NormaliseColour(RayColour col)
        {
            RayColour newCol = col;
            double scale = 0;

            if(col.r >= col.g && col.r >= col.b)
            {
                scale = 1.0 / col.r;
            }
            else if (col.g >= col.r && col.g >= col.b)
            {
                scale = 1.0 / col.g;
            }
            else
            {
                scale = 1.0 / col.b;
            }

            newCol = newCol.Multiply(scale);

            return newCol;
        }

        private RayColour Lighting(Scene scene, Comps comps, int currentDepth)
        {
            Tuple point = comps.point;
            Tuple normal = comps.normal;
            Tuple eyeDir = comps.eyeDir;
            Material mat = comps.sceneObject.GetMaterial();
            List<Light> lights = scene.lights;
            RayColour totalResult = new RayColour(0, 0, 0);

            foreach(Light light in lights) 
            {
                RayColour effective = mat.colour;

                if(mat.pattern != null)
                {
                    effective = mat.pattern.PatternAtObject(point, comps.sceneObject);
                }

                effective = effective.Multiply(light.intensity);

                Tuple lightDir = light.position.Sub(point).Normalise();

                RayColour ambient = effective.Multiply(mat.ambient);

                double lightDotNormal = lightDir.Dot(normal);

                RayColour diffuse = new RayColour(RayColour.BLACK);
                RayColour specular = new RayColour(RayColour.BLACK);

                if (lightDotNormal >= 0)
                {
                    diffuse = effective.Multiply(mat.diffuse).Multiply(lightDotNormal);

                    Tuple reflect = lightDir.Negate().Reflect(normal);
                    double reflectDotEye = reflect.Dot(eyeDir);

                    if (reflectDotEye >= 0)
                    {
                        double factor = Math.Pow(reflectDotEye, mat.shininess);
                        specular = light.intensity.Multiply(factor).Multiply(mat.specular);
                    }
                }
                RayColour result = new RayColour(ambient);

                RayColour isShadowed = IsShadowed(scene, light, comps);
                
                result = ambient.Add(diffuse).Add(specular).Multiply(isShadowed).Clamp();

                totalResult = totalResult.Add(result);
            }

            RayColour reflected = ReflectedColour(scene, comps, currentDepth);

            RayColour refracted = RefractedColour(scene, comps, currentDepth);

            if(mat.reflectivity > 0 && comps.sceneObject.GetMaterial().transparency > 0)
            {
                double reflectance = Schlick(comps);
                reflected = reflected.Multiply(reflectance);
                refracted = refracted.Multiply(1 - reflectance);
            }

            totalResult = totalResult.Add(reflected);
            totalResult = totalResult.Add(refracted);
            totalResult = totalResult.Clamp();

            return totalResult;
        }

        RayColour ReflectedColour(Scene scene, Comps comps, int currentDepth)
        {
            RayColour rCol = new RayColour(0, 0, 0);
            
            if(currentDepth > 0 && !MathsUtils.Equal(comps.sceneObject.GetMaterial().reflectivity, 0))
            {
                Ray reflectRay = new Ray(comps.overPoint, comps.reflectV);
                rCol = ColourAt(scene, reflectRay, currentDepth - 1);
                rCol = rCol.Multiply(comps.sceneObject.GetMaterial().reflectivity);
            }

            return rCol;
        }

        RayColour RefractedColour(Scene scene, Comps comps, int currentDepth)
        {
            RayColour rCol = new RayColour(0, 0, 0);

            //return black if object is black
            if (comps.sceneObject.GetMaterial().transparency == 0)
            {
                return rCol;
            }

            //return black if at max recursive depth
            if(currentDepth == 0)
            {
                return rCol;
            }

            //check for total internal reflection using snel's law, see page 157 in the book
            double nRatio = comps.rI1 / comps.rI2;
            double cosI = Tuple.Dot(comps.eyeDir, comps.normal);
            double sin2T = (nRatio * nRatio) * (1 - (cosI * cosI));

            if(sin2T > 1)
            {
                return rCol;
            }

            double cosT = Math.Sqrt(1 - sin2T);
            Tuple direction = comps.normal.Multiply((nRatio * cosI) - cosT).Sub(comps.eyeDir.Multiply(nRatio));
            Ray refractRay = new Ray(comps.underPoint, direction);
            rCol = ColourAt(scene, refractRay, currentDepth - 1).Multiply(comps.sceneObject.GetMaterial().transparency);

            return rCol;
        }

        double Schlick(Comps comps)
        {
            double cos = Tuple.Dot(comps.eyeDir, comps.normal);
            if(comps.rI2 > comps.rI2)
            {
                double n = comps.rI1 / comps.rI2;
                double sin2T = (n * n) * (1 - (cos * cos));
                if(sin2T > 1)
                {
                    return 1.0;
                }
                double cosT = Math.Sqrt(1 - sin2T);
                cos = cosT;
            }

            double r0 = Math.Pow(((comps.rI1 - comps.rI2) / (comps.rI1 + comps.rI2)), 2);

            double retVal = (r0 + (1 - r0)) * Math.Pow((1 - cos), 5);
            return retVal;
        }

        private RayColour ColourAt(Scene scene, Ray r, int currentDepth)
        {
            List<Intersection> allIntersects = IntersectScene(scene, r);

            RayColour rcol = RayColour.BLACK;

            if (allIntersects.Count > 0)
            {
                //Intersection firstHit = allIntersects[0];
                Intersection firstHit = Hit(allIntersects);
                if(firstHit != null)
                {
                    Material rmat = firstHit.sceneObject.GetMaterial();

                    Comps comps = PrepareComputations(firstHit, r, allIntersects);

                    rcol = Lighting(scene, comps, currentDepth);
                }
            }

            return rcol;
        }

        // overrides PixelPlotter.RunTests()
        public new static void RunTests()
        {
            // make sure we call our parent class RunTests first,
            // can't use "base.RunTests()" as static methods can't use base
            PixelPlotter.RunTests();

            int numTestsFailed = 0;
            Console.WriteLine("Running RayTracer Tests");

            // add tests here

            Console.WriteLine("TOTAL RayTracer numTestsFailed=" + numTestsFailed);
            Console.WriteLine("Finished RayTracer Tests");
        }
    }
}
