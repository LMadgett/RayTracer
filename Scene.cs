using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    // Scene contains the definition of the whole scene to be raytraced
    // Camera, all the objects, etc
    public class Scene
    {
        public Camera camera;
        public List<SceneObject> objects = new List<SceneObject>();
        public List<Light> lights = new List<Light>();
        public int maxRecursionDepth = 8;
    }
}
