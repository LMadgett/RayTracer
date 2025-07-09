using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.PixelFormat;

namespace Raytracer
{
    public unsafe class RaytracerTaskParams : PixelTaskParams
    {
        public RayCanvas canvas;
        public Color taskColor;
        public Scene scene;

        public RaytracerTaskParams(int pixelX, int pixelY, int pixelWidth, int pixelHeight, RayCanvas canvas, Color* taskPixels, int freeTaskPixelIdx, Scene scene, Color taskColor)
            : base(pixelX, pixelY, pixelWidth, pixelHeight, taskPixels, freeTaskPixelIdx)
        {
            this.canvas = canvas;
            this.taskColor = taskColor;
            this.scene = scene;
        }
    }
}
