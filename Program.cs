using System;
using System.Linq;
using System.Numerics;
using Raylib_cs;

namespace RayTracer
{
    class Program
    {
        int recWidth = 128;
        int recHeight = 128;
        int maxIter = 1024;
        int screenWidth = 512;
        int screenHeight = 512;
        Vector2 iCoord = new Vector2(-1.125f, 0.225f);
        int currentFrame = 0;
        int maxFrames = 150;

        public static void Main()
        {
            //Program program = new Program();
            //program.DrawMandelbrot();

            RunTests();
        }

        static Color[] colours = (from c in Enumerable.Range(0, 256) select new Color((c >> 5) * 36, (c >> 3 & 7) * 36, (c & 3) * 85, 255)).ToArray();

        public static void RunTests()
        {
            int numTestsFailed = 0;
            Console.WriteLine("Running Tests");

            numTestsFailed += Tuple.TestTuple();
            numTestsFailed += RayColour.TestRayColour();
            numTestsFailed += MathsUtils.TestMathsUtils();

            Console.WriteLine("TOTAL NumTestsFailed = " + numTestsFailed);
            Console.WriteLine("Finished Tests");
        }

        public void DrawMandelbrot()
        {
            Raylib.InitWindow(screenWidth, screenHeight, "Mandelbrot");

            int currentMonitor = Raylib.GetCurrentMonitor();
            screenWidth = Raylib.GetMonitorWidth(currentMonitor);
            screenHeight = Raylib.GetMonitorHeight(currentMonitor);

            Raylib.SetWindowSize(screenWidth, screenHeight);
            Raylib.SetWindowPosition(0, 0);

            double centreX = 0;
            double centreY = 0;
            double xSize = 4;
            double ySize = (xSize / screenWidth) * screenHeight;
            double minX = centreX - (xSize / 2);
            double minY = centreY - (ySize / 2);

            double xInc = xSize / screenWidth;
            double yInc = ySize / screenHeight;           
  
            bool completed = false;
            Image currentImage = Raylib.GenImageColor(screenWidth, screenHeight, Color.BLACK);

            Vector2 curLineCoords = iCoord;
            while (!Raylib.WindowShouldClose() && currentFrame < maxFrames)
            {
                int currentPixelX = 0;
                int currentPixelY = 0;
                double currentX = minX;
                double currentY = minY;
                while (!completed)
                {
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.WHITE);

                    if (!completed)
                    {
                        for (int y = currentPixelY; y < currentPixelY + recHeight; y++)
                        {
                            for (int x = currentPixelX; x < currentPixelX + recWidth; x++)
                            {
                                //Raylib.DrawPixel(x, y, Color.GREEN);

                                currentX = minX + (x * xInc);
                                currentY = minY + (y * yInc);

                                int i = CalJulPix(currentX, currentY, curLineCoords.X, curLineCoords.Y);
                                //Color col = Color.WHITE;
                                //if (i == maxIter)
                                //{
                                //    col = Color.BLACK;
                                //}

                                i = i % colours.Length;
                                Color col = colours[i];

                                Raylib.ImageDrawPixel(ref currentImage, x, y, col);
                            }
                        }
                    }

                    Texture2D tex = Raylib.LoadTextureFromImage(currentImage);
                    Raylib.DrawTexture(tex, 0, 0, Color.WHITE);

                    Raylib.DrawText("frame:" + currentFrame + "maxIter:" + maxIter, 16, 16, 16, Color.WHITE);

                    Raylib.EndDrawing();
                    currentPixelX = currentPixelX + recWidth;

                    if (currentPixelX >= screenWidth)
                    {
                        currentPixelY = currentPixelY + recHeight;
                        currentPixelX = 0;

                        if (currentPixelY >= screenHeight)
                        {
                            completed = true;
                        }
                    }

                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                    {
                        Vector2 mousePos = Raylib.GetMousePosition();

                        centreX = minX + (mousePos.X * xInc);
                        centreY = minY + (mousePos.Y * yInc);
                        xSize = xSize / 2;
                        ySize = (xSize / screenWidth) * screenHeight;
                        minX = centreX - (xSize / 2);
                        minY = centreY - (ySize / 2);

                        xInc = xSize / screenWidth;
                        yInc = ySize / screenHeight;
                        currentX = minX;
                        currentY = minY;

                        currentPixelX = 0;
                        currentPixelY = 0;
                        completed = false;
                    }

                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                    {
                        Vector2 mousePos = Raylib.GetMousePosition();

                        centreX = minX + (mousePos.X * xInc);
                        centreY = minY + (mousePos.Y * yInc);
                        xSize = xSize * 2;
                        ySize = (xSize / screenWidth) * screenHeight;
                        minX = centreX - (xSize / 2);
                        minY = centreY - (ySize / 2);

                        xInc = xSize / screenWidth;
                        yInc = ySize / screenHeight;
                        currentX = minX;
                        currentY = minY;

                        currentPixelX = 0;
                        currentPixelY = 0;
                        completed = false;
                    }

                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_KP_ADD))
                    {
                        maxIter *= 2;
                        currentX = minX;
                        currentY = minY;
                        currentPixelX = 0;
                        currentPixelY = 0;
                        completed = false;
                    }

                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_KP_SUBTRACT))
                    {
                        maxIter /= 2;
                        currentX = minX;
                        currentY = minY;
                        currentPixelX = 0;
                        currentPixelY = 0;
                        completed = false;
                    }

                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                    {
                        SaveImage();
                    }

                    Raylib.UnloadTexture(tex);
                }
                SaveImage();
                curLineCoords = CalNextLineCoords(curLineCoords);
                completed = false;
                currentFrame++;
            }

            Raylib.CloseWindow();
        }

        private Vector2 CalNextLineCoords(Vector2 curLinePos)
        {
            return new Vector2((float)(curLinePos.X + 0.00005), (float)(curLinePos.Y - 0.00005));
        }

        private int CalMandPix(double x, double y)
        {
            int i = 0;
            double cX = x;
            double cY = y;

            while ((x * x) + (y * y) < 4 && i < maxIter)
            {
                double tmp = (x * x) - (y * y) + cX;
                y = (2.0 * x * y) + cY;
                x = tmp;
                i = i + 1;
            }

            return i;
        }

        private int CalJulPix(double x, double y, double cX, double cY)
        {
            int i = 0;

            while ((x * x) + (y * y) < 4 && i < maxIter)
            {
                double tmp = (x * x) - (y * y) + cX;
                y = (2.0 * x * y) + cY;
                x = tmp;
                i = i + 1;
            }

            return i;
        }

        private void SaveImage()
        {
            Image screenImage = Raylib.LoadImageFromScreen();
            string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            Raylib.ExportImage(screenImage, "Julia_" + currentDateTime + "_FrameNum_" + currentFrame + "_MaxIter_" + maxIter + ".png");
            Raylib.UnloadImage(screenImage);
        }

    }
}