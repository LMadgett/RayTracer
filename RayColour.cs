using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
    public class RayColour
    {
        public static RayColour BLACK = new RayColour(0, 0, 0);
        public static RayColour WHITE = new RayColour(1, 1, 1);
        public double r;
        public double g;
        public double b;

        public RayColour (double r, double g, double b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public RayColour (RayColour c)
        {
            this.r = c.r;
            this.g = c.g;
            this.b = c.b;
        }

        public void SetColour (RayColour c)
        {
            this.r = c.r;
            this.g = c.g;
            this.b = c.b;
        }

        public RayColour Multiply (double scalar)
        {
            return new RayColour(r * scalar, g * scalar, b * scalar);
        }

        public RayColour Divide(double divisor)
        {
            return new RayColour(r / divisor, g / divisor, b / divisor);
        }

        public static RayColour Multiply (RayColour rc, double scalar)
        {
            return rc.Multiply(scalar);
        }

        public static RayColour Divide(RayColour rc, double divisor)
        {
            return rc.Divide(divisor);
        }

        public RayColour Multiply (RayColour rc)
        {
            return new RayColour(r * rc.r, g * rc.g, b * rc.b);
        }

        public static RayColour Multiply (RayColour rc1, RayColour rc2)
        {
            return rc1.Multiply(rc2);
        }

        public RayColour Add(RayColour rc)
        {
            return new RayColour(r + rc.r, g + rc.g, b + rc.b);
        }

        public RayColour Sub(RayColour rc)
        {
            return new RayColour(r - rc.r, g - rc.g, b - rc.b);
        }

        public static RayColour Add(RayColour rc1, RayColour rc2)
        {
            return rc1.Add(rc2);
        }

        public static RayColour Sub(RayColour rc1, RayColour rc2)
        {
            return rc1.Sub(rc2);
        }

        public bool IsEqual(RayColour a)
        {
            if (!MathsUtils.Equal(this.r, a.r) ||
                !MathsUtils.Equal(this.g, a.g) ||
                !MathsUtils.Equal(this.b, a.b))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public RayColour Clamp()
        {
            this.r = Math.Clamp(this.r, 0, 1);
            this.g = Math.Clamp(this.g, 0, 1);
            this.b = Math.Clamp(this.b, 0, 1);

            return this;
        }

        public static int TestRayColour()
        {
            Console.WriteLine("Running TestRayColour");
            int numTestsFailed = 0;

            // Test 1
            RayColour a = new RayColour(0.3, 0.4, 0.5);
            if (!a.IsEqual(new RayColour(0.3, 0.4, 0.5)))
            {
                Console.WriteLine("TestRayColour Test 1 failed");
                numTestsFailed++;
            }

            // Test 2
            a = new RayColour(0.3, 0.4, 0.5);
            a = a.Multiply(2.0);
            if (!a.IsEqual(new RayColour(0.6, 0.8, 1.0)))
            {
                Console.WriteLine("TestRayColour Test 2 failed");
                numTestsFailed++;
            }

            // Test 3
            a = new RayColour(0.3, 0.4, 0.5);
            RayColour b = new RayColour(0.1, 0.1, 0.1);
            a = a.Multiply(b);
            if (!a.IsEqual(new RayColour(0.03, 0.04, 0.05)))
            {
                Console.WriteLine("TestRayColour Test 3 failed, a=("+a.r+","+a.g+","+a.b+")");
                numTestsFailed++;
            }

            Console.WriteLine("TestRayColour complete numTestsFailed=" + numTestsFailed);
            return numTestsFailed;
        }
    }
}
