using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.PixelFormat;

namespace Raytracer
{
    public unsafe class RayCanvas
    {
        int width, height;
        RayColour[,] canvas;

        public RayCanvas (int width, int height)
        {
            this.width = width;
            this.height = height;

            // create a new canvas and populate with new RayColour objects
            InitCanvas();
        }

        // only called when want a whole new canvas, doesn't reuse existing
        private void InitCanvas()
        {
            canvas = new RayColour[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // new RayColour object for each position in canvas when initialised first time
                    canvas[x, y] = new RayColour(0, 0, 0);
                }
            }
        }

        // clears the existing canvas to a given colour
        public void ClearCanvas (RayColour colour)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SetColour(x, y, colour);
                }
            }
        }

        // only used INTERNALLY by this canvas class to directly get RayColour object from canvas
        private RayColour InternalGetColour (int x, int y)
        {
            return canvas[x, y];
        }

        public RayColour GetColour(int x, int y)
        {
            // returns a NEW RayColour, not the one from canvas (to prevent external direct tampering with canvas colours)
            return new RayColour(InternalGetColour(x, y));
        }

        // allows passing in of RayColour to set return colours into, to avoid NEW RayColour creation (=> less GC)
        public RayColour GetColour(int x, int y, RayColour c)
        {
            c.SetColour(InternalGetColour(x, y));
            return c;
        }

        public void SetColour (int x, int y, RayColour colour)
        {
            // NB using SetColour method rather than canvas[x,y]=colour in order to keep existing RayColour object in the canvas => avoid GC
            canvas[x, y].SetColour(colour);
        }

        //
        // Copies a rectangle (x, y) -> (x+width, y+height) into destPixels Color* 1D array
        // This means it can be decoupled from the main render thread (allowing multithreading in chunks)
        //
        // destPixels must be pre-allocated! and big enough (at least width*height Color* big)        
        // destPixels can be allocated by something like :
        //      Color* destPixels = (Color*)Raylib.MemAlloc(width * height * sizeof(Color));
        // You can then use this destPixels to write into an actual Raylib Texture like :
        //      Raylib.UpdateTextureRec(screenTexture, new Rectangle(destinationX, destinationY, width, height), destPixels);
        // This screenTexture can then be written to the actual screen in the update loop like:
        //      Raylib.DrawTexture(screenTexture, 0, 0, Color.WHITE);
        // screenTexture is obtained ONCE (at init time) like :
        //      Texture2D screenTexture = Raylib.LoadTextureFromImage(screenImage);
        // screenImage is created like :
        //      Color* screenPixels = (Color*)Raylib.MemAlloc(screenPixelWidth * screenPixelHeight * sizeof(Color));
        //      Image screenImage = new Image
        //                          {
        //                              Data = screenPixels,
        //                              Width = screenPixelWidth,
        //                              Height = screenPixelHeight,
        //                              Format = PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
        //                              Mipmaps = 1,
        //                          };
        //
        // Colours are converted from 0..1 doubles to 0..255 byte format
        //
        // Returns destPixels Color* (for ease of use)
        public Color* CopyRectangle(int x, int y, int width, int height, Color* destPixels)
        {
            Color col = new Color(0, 0, 0, 0);
            for (int sy = 0; sy < height; sy++)
            {
                for (int sx = 0; sx < width; sx++)
                {
                    RayColour rCol = canvas[x + sx, y + sy];
                    col.R = (byte)(rCol.r * 255);
                    col.G = (byte)(rCol.g * 255);
                    col.B = (byte)(rCol.b * 255);
                    col.A = 255;
                    destPixels[sx + (sy * width)] = col;
                }
            }

            return destPixels;
        }

    }
}
