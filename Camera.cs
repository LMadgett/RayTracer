using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Raytracer
{
    public class Camera
    {
        public Tuple position;              // 3D pos of the camera in world space
        public Tuple lookAt;                // 3D pos the camera is looking at in world space
        public double rollAngle;            // roll angle of screen around the (lookAt-position) axis
        //public double screenDistFraction;   // 0..1 fraction of |(lookAt-position)| distance the screen centre should be at
        public int screenPixelW;            // pixel width of screen
        public int screenPixelH;            // pixel height of screen
        public double screenWidth;          // Should calculate from (screenHeight / screenPixelH) * screenPixelW - if Height is set first
        public double screenHeight;         // Should calculate from (screenWidth / screenPixelW) * screenPixelH - if Width is set first

        public double focalDistance = 0; //distance from position, where in focus point should be
        public double apertureSize; //length of one side of the aperture in world units, 0=point aperture
        public int apertureSampling; //number of samples along side of aperture

        public double screenDist;          // actual distance of position->screenCentre
        private Tuple screenXAxis;          // Normalised vector along screen x axis, in world coords
        private Tuple screenYAxis;          // Normalised vector along screen y axis, in world coords
        private Tuple screenPixelWInc;      // calculated, vector for 1 pixel in X axis of the screen in 3D world space
        private Tuple screenPixelHInc;      // calculated, vector for 1 pixel in Y axis of the screen in 3D world space
        private Tuple screenCentre;         // calculated by ((lookAt - position) * screenDistFraction) + position
        private Tuple screenTopLeft;        // calculated from screenCentre using screenPixelWInc and screenPixelHInc

        //DOF vars
        Tuple centToTL;
        Tuple sampleXInc;
        Tuple sampleYInc;

        // return a Ray from the camera through the given pixel coords on the "3D" screen
        public List<Ray> GetRay(double pixelX, double pixelY)
        {
            List<Ray> rays = new List<Ray>();
            Ray cameraRay = new Ray(position, Tuple.Vector(0,0,0));

            //pixel pos on screen in world coords
            Tuple pixelPos = screenTopLeft.Add(screenPixelWInc.Multiply(pixelX)).Add(screenPixelHInc.Multiply(pixelY));
            Tuple direction = pixelPos.Sub(position).Normalise();
            cameraRay.direction = direction;

            //position of the focal point in the world
            Tuple focalPoint = position.Add(cameraRay.direction.Multiply(focalDistance));

            //start rayPos at the top left of the aperture on the screen at pixelPos
            Tuple rayStartPos = pixelPos.Add(centToTL);

            for (int sampleY = 0; sampleY < apertureSampling; sampleY++) 
            {
                for (int sampleX = 0; sampleX < apertureSampling; sampleX++)
                {
                    if (apertureSampling > 1 && !IsSampleInsideCircle(sampleX, sampleY)) continue;

                    Tuple rayPos = rayStartPos.Add(sampleXInc.Multiply(sampleX)).Add(sampleYInc.Multiply(sampleY));
                    cameraRay = new Ray(rayPos, Tuple.Vector(0,0,0));
                    direction = focalPoint.Sub(rayPos).Normalise();
                    cameraRay.direction = direction;
                    rays.Add(cameraRay);
                }
            }

            return rays;
        }

        public void Initialise()
        {
            CalcCameraVectors();
        }

        private bool IsSampleInsideCircle(int sampleX, int sampleY)
        {
            double radius = apertureSampling / 2.0;
            bool inside = false;

            double x = sampleX - radius;
            double y = sampleY - radius;

            if (x * x + y * y <= radius * radius) inside = true;
            return inside;
        }

        private void CalcCameraVectors ()
        {
            // right and up calculations from https://www.scratchapixel.com/lessons/mathematics-physics-for-computer-graphics/lookat-function/framing-lookat-function.html
            Tuple forward = lookAt.Sub(position).Normalise();
            Tuple tempVec = Tuple.Vector(0, 1, 0);
            Tuple right = tempVec.Cross(forward).Normalise();
            Tuple up = forward.Cross(right).Normalise();

            screenXAxis = right;
            screenYAxis = up;

            screenPixelWInc = screenXAxis.Multiply(screenWidth);
            screenPixelWInc = screenPixelWInc.Divide(screenPixelW);
            screenPixelHInc = screenYAxis.Multiply(-screenHeight);   // "minus" because we want to go DOWN y-axis with increasing y-pixel number
            screenPixelHInc = screenPixelHInc.Divide(screenPixelH);

            //screenDist = lookAt.Sub(position).Magnitude() * screenDistFraction;
            screenCentre = position.Add(forward.Multiply(screenDist));
            // get TL corner of screen by subtracting (screenW/2) and adding (screenY/2) - NB ADD on Y as want TOP not bottom left
            screenTopLeft = screenCentre.Sub(screenXAxis.Multiply(screenWidth / 2.0)).Add(screenYAxis.Multiply(screenHeight / 2.0));

            //DOF vars
            centToTL = screenXAxis.Multiply(-apertureSize / 2.0).Add(screenYAxis.Multiply(apertureSize/2.0));
            sampleXInc = screenXAxis.Multiply(apertureSize).Divide(apertureSampling);
            sampleYInc = screenYAxis.Multiply(-apertureSize).Divide(apertureSampling);
        }

        //public static void TestCamera ()
        //{
        //    Camera cam = new Camera();
        //    cam.position = Tuple.Point(0, 0, -10);
        //    cam.lookAt = Tuple.Point(0, 0, 0);
        //    cam.screenDistFraction = 0.5;
        //    cam.screenPixelW = 1024;
        //    cam.screenPixelH = 1024;
        //    cam.screenWidth = 10;
        //    cam.screenHeight = 10;
        //    cam.CalcCameraVectors();
        //    cam.DebugOut();

        //    cam = new Camera();
        //    cam.position = Tuple.Point(0, 10, -10);
        //    cam.lookAt = Tuple.Point(0, 0, 0);
        //    cam.screenDistFraction = 0.5;
        //    cam.screenPixelW = 1024;
        //    cam.screenPixelH = 1024;
        //    cam.screenWidth = 10;
        //    cam.screenHeight = 10;
        //    cam.CalcCameraVectors();
        //    cam.DebugOut();
        //}

        public void DebugOut()
        {
            Console.WriteLine("screenDist=" + screenDist);
            Console.WriteLine("screenCentre=" + screenCentre.ToString());
            Console.WriteLine("screenTopLeft=" + screenTopLeft.ToString());
            Console.WriteLine("screenXAxis=" + screenXAxis.ToString());
            Console.WriteLine("screenYAxis=" + screenYAxis.ToString());
            Console.WriteLine("screenPixelWInc=" + screenPixelWInc.ToString());
            Console.WriteLine("screenPixelHInc=" + screenPixelHInc.ToString());
        }
    }
}
