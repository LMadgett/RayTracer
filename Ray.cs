using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class Ray
    {
        public Tuple origin;
        public Tuple direction;

        public Ray(Tuple origin, Tuple direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public Tuple Position (double t)
        {
            Tuple pos = Tuple.Add(origin, Tuple.Multiply(direction, t));
            return pos;
        }

        public Ray Transform (Matrix trans)
        {
            Tuple newOrigin = trans.Multiply(origin);
            Tuple newDirection = trans.Multiply(direction);
            Ray newRay = new Ray(newOrigin, newDirection);
            return newRay;
        }

        public static int TestRay()
        {
            Console.WriteLine("Running TestTuple");
            int numTestsFailed = 0;

            // Test 1
            Ray r = new Ray(Tuple.Point(1, 2, 3), Tuple.Vector(0, 1, 0));
            Matrix m = Matrix.GetTranslation(3, 4, 5);
            Ray r2 = r.Transform(m);
            if (!MathsUtils.Equal(r2.origin.x, 4) ||
                !MathsUtils.Equal(r2.origin.y, 6) ||
                !MathsUtils.Equal(r2.origin.z, 8) ||
                !MathsUtils.Equal(r2.origin.w, 1) ||
                !MathsUtils.Equal(r2.direction.x, 0) ||
                !MathsUtils.Equal(r2.direction.y, 1) ||
                !MathsUtils.Equal(r2.direction.z, 0) ||
                !MathsUtils.Equal(r2.direction.w, 0))
            {
                Console.WriteLine("TestRay Test 1 failed");
                numTestsFailed++;
            }

            // Test 2
            r = new Ray(Tuple.Point(1, 2, 3), Tuple.Vector(0, 1, 0));
            m = Matrix.GetScaling(2, 3, 4);
            r2 = r.Transform(m);
            if (!MathsUtils.Equal(r2.origin.x, 2) ||
                !MathsUtils.Equal(r2.origin.y, 6) ||
                !MathsUtils.Equal(r2.origin.z, 12) ||
                !MathsUtils.Equal(r2.origin.w, 1) ||
                !MathsUtils.Equal(r2.direction.x, 0) ||
                !MathsUtils.Equal(r2.direction.y, 3) ||
                !MathsUtils.Equal(r2.direction.z, 0) ||
                !MathsUtils.Equal(r2.direction.w, 0))
            {
                Console.WriteLine("TestRay Test 1 failed");
                numTestsFailed++;
            }

            Console.WriteLine("TestRay complete numTestsFailed=" + numTestsFailed);
            return numTestsFailed;
        }
    }
}
