using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.PixelFormat;

namespace Raytracer
{
    // used as both input parameters and output results for the pixel Tasks
    public unsafe class PixelTaskParams
    {
        public int pixelX, pixelY;
        public int pixelWidth, pixelHeight;
        public Color* taskPixels;
        public int freeTaskPixelIdx;

        public PixelTaskParams(int pixelX, int pixelY, int pixelWidth, int pixelHeight, Color* taskPixels, int freeTaskPixelIdx)
        {
            this.pixelX = pixelX;
            this.pixelY = pixelY;
            this.pixelWidth = pixelWidth;
            this.pixelHeight = pixelHeight;
            this.taskPixels = taskPixels;
            this.freeTaskPixelIdx = freeTaskPixelIdx;
        }
    }
}
