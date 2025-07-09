using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Raylib_cs;
using static Raylib_cs.PixelFormat;

namespace Raytracer
{
    public abstract unsafe class PixelPlotter
    {
        protected int screenPixelWidth = 1024;
        protected int screenPixelHeight = 1024;
        protected int taskPixelWidth = 64;
        protected int taskPixelHeight = 64;

        Color* screenPixels;
        Texture2D screenTexture;

        // You will need a Main() method in your code that does
        // similar to the below, most importantly:
        // Construct a new Plotter
        // SetupPlotter();
        // RunPlotter();
        //public static void Main()
        //{
        //    RunTests();

        //    PixelPlotter plotter = new Plotter();
        //    plotter.SetupPlotter();
        //    plotter.RunPlotter();
        //}

        protected void SetupPlotter ()
        {
            InitialiseScreen();
        }

        protected void InitialiseScreen ()
        {
            //Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT);
            Raylib.InitWindow(screenPixelWidth, screenPixelHeight, "PixelPlotter");
            //Raylib.SetTargetFPS(60);

            // Sets to fullscreen for current monitor
            //int currentMonitor = Raylib.GetCurrentMonitor();
            //screenPixelWidth = Raylib.GetMonitorWidth(currentMonitor);
            //screenPixelHeight = Raylib.GetMonitorHeight(currentMonitor);
            //Raylib.SetWindowSize(screenPixelWidth, screenPixelHeight);
            //Raylib.SetWindowPosition(0, 0);
            //Raylib.ToggleFullscreen();

            // pixels for the overall picture
            screenPixels = (Color*)Raylib.MemAlloc(screenPixelWidth * screenPixelHeight * sizeof(Color));

            // create an Image of the right size using the pixels array as storage
            Image screenImage = new Image
            {
                Data = screenPixels,
                Width = screenPixelWidth,
                Height = screenPixelHeight,
                Format = PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
                Mipmaps = 1,
            };

            // get the texture from the image
            screenTexture = Raylib.LoadTextureFromImage(screenImage);

            // update the texture with the new pixels
            // TODO do we need this? not sure...
            Raylib.UpdateTexture(screenTexture, screenPixels);
        }

        public void RunPlotter()
        {
            int nextTaskPixelX = 0;
            int nextTaskPixelY = 0;

            // max number of tasks to run in parallel
            int maxTasks = 10;

            List<Task<PixelTaskParams>> pixelTasks = new List<Task<PixelTaskParams>>();
            Color*[] taskPixelsArr = new Color*[maxTasks];
            // keeps track of which entries in taskPixelsArr are free (not currently being used by a pixelTask)
            List<int> freeTaskPixels = new List<int>();

            // allocate the taskPixels textures for each task
            for (int i = 0; i < maxTasks; i++)
            {
                // pixels for a single task to generate into
                Color* taskPixels = (Color*)Raylib.MemAlloc(taskPixelWidth * taskPixelHeight * sizeof(Color));
                taskPixelsArr[i] = taskPixels;
                freeTaskPixels.Add(i);
            }

            bool completed = false;
            long lastParamChangeTime = GetCurrentMilliseconds();

            while (!Raylib.WindowShouldClose())
            {
                //Console.WriteLine("pixelTasks.Count=" + pixelTasks.Count);

                // do we need a new pixel Task? loop for as many as we need
                while (pixelTasks.Count < maxTasks && completed == false)
                {
                    // get the first free taskPixels (and remove from the free list)
                    int freeTaskPixelIdx = freeTaskPixels[0];
                    freeTaskPixels.RemoveAt(0);
                    Color* taskPixels = taskPixelsArr[freeTaskPixelIdx];

                    // make sure we're only calculating up to edge of screen to prevent overrun
                    int thisTaskPixelWidth = taskPixelWidth;
                    int thisTaskPixelHeight = taskPixelHeight;
                    if (nextTaskPixelX + thisTaskPixelWidth >= screenPixelWidth)
                        thisTaskPixelWidth = screenPixelWidth - nextTaskPixelX;
                    if (nextTaskPixelY + thisTaskPixelHeight >= screenPixelHeight)
                        thisTaskPixelHeight = screenPixelHeight - nextTaskPixelY;

                    PixelTaskParams taskParams = ConstructTaskParams(nextTaskPixelX, nextTaskPixelY,
                                                                     thisTaskPixelWidth, thisTaskPixelHeight,
                                                                     taskPixels, freeTaskPixelIdx);

                    // create the task, passing required State into the lambda, as otherwise get race conditions
                    Task<PixelTaskParams> pixelTask = new Task<PixelTaskParams>((Object obj) =>
                                                                            {
                                                                                PixelTaskParams data = obj as PixelTaskParams;
                                                                                if (data == null) return null;
                                                                                return ComputePixels(data);
                                                                            },
                                                                            taskParams);

                    pixelTask.Start();
                    pixelTasks.Add(pixelTask);

                    //Console.WriteLine("starting taskX=" + nextTaskX + " taskY=" + nextTaskY + " taskPixels=" + (int)taskPixels + " freeIdx=" + freeTaskPixelIdx);

                    // move to the next task coordinates
                    nextTaskPixelX += thisTaskPixelWidth;
                    if (nextTaskPixelX >= screenPixelWidth)
                    {
                        nextTaskPixelX = 0;
                        nextTaskPixelY += thisTaskPixelHeight;

                        if (nextTaskPixelY >= screenPixelHeight)
                        {
                            completed = true;
                        }
                    }
                }

                // look at each currently running pixelTask to see if they've finished
                // note we go BACKWARDS through the list so we can remove them as we need
                // - if we went forwards then the counting would break as we removed tasks
                for (int i = pixelTasks.Count-1; i >= 0; i--)
                {
                    Task<PixelTaskParams> pixelTask = pixelTasks[i];

                    // do we have a completed task?
                    if (pixelTask != null && pixelTask.IsCompleted)
                    {
                        PixelTaskParams result = pixelTask.Result;

                        //Console.WriteLine("finished taskX=" + result.taskX + " taskY=" + result.taskY + " taskPixels=" + (int)result.taskPixels +" freeIdx=" + result.freeTaskPixelIdx);

                        // correct the pixelTask output texture for reduced width if we're at the edge of the screen
                        int recW = result.pixelWidth;
                        int recH = result.pixelHeight;
                        if (result.pixelX + recW >= screenPixelWidth)
                        {
                            recW = screenPixelWidth - result.pixelX;

                            // don't need all this jiggery pokery now in Raylib 5? Did need it in Raylib 4 though.
                            //int toPos = 0;
                            //int fromPos = 0;
                            //int fromX = 0;
                            //while (fromPos < result.pixelWidth * result.pixelHeight)
                            //{
                            //    result.taskPixels[toPos] = result.taskPixels[fromPos];
                            //    toPos++;
                            //    fromPos++;
                            //    fromX++;
                            //    if (fromX >= recW)
                            //    {
                            //        fromX = 0;
                            //        fromPos += (result.pixelWidth - recW);
                            //    }
                            //}
                        }
                        if (result.pixelY + recH >= screenPixelHeight)
                        {
                            recH = screenPixelHeight - result.pixelY;
                        }
                        Raylib.UpdateTextureRec(screenTexture, new Rectangle(result.pixelX, result.pixelY, recW, recH), result.taskPixels);

                        // free up the taskPixels to be reused in another task
                        freeTaskPixels.Add(result.freeTaskPixelIdx);

                        // remove this completed task
                        pixelTasks.RemoveAt(i);
                    }
                }

                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.WHITE);

                // draw the main texture to the screen
                Raylib.DrawTexture(screenTexture, 0, 0, Color.WHITE);

                // have less than 3s passed since last change of maxIter?
                long currentTime = GetCurrentMilliseconds();
                if (currentTime - lastParamChangeTime < 3000)
                {
                    // display current maxIter if so
                    Raylib.DrawText("(centreX, centreY): ()", 16, 16, 24, Color.WHITE);
                    Raylib.DrawText("xSize: ySize: ", 16, 64, 24, Color.WHITE);
                    Raylib.DrawText("maxIter: ", 16, 112, 24, Color.WHITE);
                }

                Raylib.EndDrawing();

                // take a screenshot
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                {
                    Image currentScreen = Raylib.LoadImageFromScreen();
                    string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    Raylib.ExportImage(currentScreen, "RayTracer_" + currentDateTime + ".png");
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_F))
                {
                    // reset

                    lastParamChangeTime = GetCurrentMilliseconds();

                    nextTaskPixelX = 0;
                    nextTaskPixelY = 0;
                    completed = false;
                }
            }

            // look at each currently running pixelTask to see if they've finished
            // note we go BACKWARDS through the list so we can remove them as we need
            // - if we went forwards then the counting would break as we removed tasks
            for (int i = pixelTasks.Count - 1; i >= 0; i--)
            {
                Task<PixelTaskParams> pixelTask = pixelTasks[i];

                // do we have a completed task?
                if (pixelTask != null && !pixelTask.IsCompleted)
                {
                    pixelTask.Wait();
                }
            }

            // Pixel and Texture unloading
            for (int i = 0; i < maxTasks; i++)
            {
                Raylib.MemFree(taskPixelsArr[i]);
            }
            Raylib.MemFree(screenPixels);
            Raylib.UnloadTexture(screenTexture);  

            Raylib.CloseWindow();
        }

        protected abstract PixelTaskParams ConstructTaskParams(int pixelX, int pixelY, int pixelWidth, int pixelHeight,
                                                   Color* taskPixels, int freeTaskPixelIdx);

        private long GetCurrentMilliseconds()
        {
            return System.DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        // set a colour in the main pixels
        private void SetColor(Color* screenPixels, int x, int y, Color color)
        {
            int index = x + (y * screenPixelWidth);

            screenPixels[index] = color;
        }

        // Called via a Task so runs multi-threaded
        // Computes the pixels for the given taskParams rectangle
        protected abstract PixelTaskParams ComputePixels(PixelTaskParams taskParams);
        //{
        //    // Implement something like the below
        //    for (int y = 0; y < taskParams.pixelHeight; y++)
        //    {
        //        for (int x = 0; x < taskParams.pixelWidth; x++)
        //        {
        //            // DO STUFF and plot the pixel into taskPixels using something like
        //            // taskPixels[x + (y * taskParams.pixelWidth)] = color
        //        }
        //    }

        //    return taskParams;
        //}


        public static void RunTests()
        {
            int numTestsFailed = 0;
            Console.WriteLine("Running PixelPlotter Tests");

            numTestsFailed += Tuple.TestTuple();
            numTestsFailed += MathsUtils.TestMathsUtils();
            numTestsFailed += RayColour.TestRayColour();
            numTestsFailed += Matrix.TestMatrix();
            numTestsFailed += Ray.TestRay();
            //Camera.TestCamera();

            Console.WriteLine("TOTAL PixelPlotter numTestsFailed=" + numTestsFailed);
            Console.WriteLine("Finished PixelPlotter Tests");
        }
    }
}