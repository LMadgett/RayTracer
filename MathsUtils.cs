using System;
using System.Net;

namespace Raytracer
{
	public class MathsUtils
	{
		public const double EPSILON = 0.00001;
		public const double INFINITY = Double.MaxValue;

		public static bool Equal(double a, double b)
		{
			if (Math.Abs(a - b) < EPSILON)
				return true;
			else
				return false;
		}

		public static int TestMathsUtils()
		{
			Console.WriteLine("Running TestMathsUtils");
			int numTestsFailed = 0;

			// TestEqual 1
			double a = 1.0;
			double b = 1.0000100001;
			if (MathsUtils.Equal(a, b))
			{
				Console.WriteLine("TestMathsUtils TestEqual Test 1 failed");
				numTestsFailed++;
			}

			// TestEqual 2
			a = 1.0;
			b = 1.0000099999;
			if (!MathsUtils.Equal(a, b))
			{
				Console.WriteLine("TestMathsUtils TestEqual Test 2 failed");
				numTestsFailed++;
			}

			Console.WriteLine("TestMathsUtils complete numTestsFailed=" + numTestsFailed);
			return numTestsFailed;
		}

		public static double DegToRad(double deg)
		{
			return deg * (Math.PI / 180.0);
		}

		public static double RadToDeg(double rad)
		{
			return rad * (180.0 / Math.PI);
		}

		public static double Max(double a, double b, double c)
		{
            double ret = a >= b ? a : b;
            ret = ret >= c ? ret : c;

			return ret;
        }

        public static double Min(double a, double b, double c)
        {
            double ret = a < b ? a : b;
            ret = ret < c ? ret : c;

            return ret;
        }

		public static Tuple WorldToObject(SceneObject obj, Tuple point)
		{
			Tuple retPt = new Tuple(point);

			if (obj.GetParent() != null)
			{
				retPt = WorldToObject(obj.GetParent(), point);
			}

			return (obj.GetInvTransform().Multiply(retPt));
		}

		public static Tuple NormalToWorld(SceneObject obj, Tuple normal)
		{
			Tuple retNormal = obj.GetInvTransform().GetTranspose().Multiply(normal);
			retNormal.w = 0;
			retNormal = retNormal.Normalise();

			if(obj.GetParent() != null)
			{
				retNormal = NormalToWorld(obj.GetParent(), retNormal);
			}

			return retNormal;
		}
    }
}
