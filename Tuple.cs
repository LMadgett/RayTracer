using System;

namespace Raytracer
{
	public class Tuple
	{
		public double x, y, z;
		public int w;

		public static Tuple Origin = Tuple.Point(0, 0, 0);

		public Tuple(double x, double y, double z, int w)
		{
			CheckValidW(w);

			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Tuple(Tuple tuple) : this(tuple.x, tuple.y, tuple.z, tuple.w)
		{
			// nothing
		}

		public override String ToString ()
		{
			return new string("(" + x + "," + y + "," + z + "," + w + ")");
		}

        public bool IsVector()
		{
			return (w == 0 ? true : false);
		}

		public bool IsPoint()
		{
			return (w == 1 ? true : false);
		}

		public Tuple Add(Tuple b)
		{
			return Tuple.Add(this, b);
		}

		public Tuple Sub(Tuple b)
		{
			return Tuple.Sub(this, b);
		}

		public static Tuple Add(Tuple a, Tuple b)
		{
			Tuple c = new Tuple(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
			return c;
		}

		public static Tuple Sub(Tuple a, Tuple b)
		{
			Tuple c = new Tuple(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
			return c;
		}

		public Tuple Negate()
		{
			if (w == 1) throw new Exception("Can't negate a point!");
			// NB does not negate w! just the vector direction in x,y,z
			return new Tuple(-x, -y, -z, w);
		}

		public Tuple Multiply(double scalar)
		{
			return Tuple.Multiply(this, scalar);
		}

		public static Tuple Multiply(Tuple tuple, double scalar)
		{
			Tuple mTuple = new Tuple(tuple);
            mTuple.x *= scalar;
            mTuple.y *= scalar;
            mTuple.z *= scalar;
            return mTuple;
		}

		public Tuple Multiply (Matrix m)
		{ 
			return m.Multiply(this);
		}

		public Tuple Divide(double divisor)
		{
			return Tuple.Divide(this, divisor);
		}

		public static Tuple Divide(Tuple tuple, double divisor)
		{
			Tuple dTuple = new Tuple(tuple);
			return Tuple.Multiply(dTuple, 1.0 / divisor);
		}

		public double Magnitude()
		{
			return Math.Sqrt(x * x + y * y + z * z);
		}

		public Tuple Normalise()
		{
			return Tuple.Normalise(this);
		}

		public static Tuple Normalise(Tuple tuple)
		{
			Tuple nTuple = new Tuple(tuple);
			return Tuple.Divide(nTuple, nTuple.Magnitude());
		}

		public double Dot(Tuple tuple)
		{
			return (this.x * tuple.x + this.y * tuple.y + this.z * tuple.z + this.w * tuple.w);
		}

		public static double Dot(Tuple tuple1, Tuple tuple2)
		{
			return tuple1.Dot(tuple2);
		}

		public Tuple Cross(Tuple tuple)
		{
			Tuple cross = Tuple.Vector(0, 0, 0);
			cross.x = y * tuple.z - z * tuple.y;
			cross.y = z * tuple.x - x * tuple.z;
			cross.z = x * tuple.y - y * tuple.x;

			return cross;
		}

		public static Tuple Cross(Tuple a, Tuple b)
		{
			return a.Cross(b);
		}

		public Tuple Reflect(Tuple normal)  //return reflection of this tuple across given normal
		{
			Tuple reflect = this.Sub(normal.Multiply(2 * this.Dot(normal)));
			return reflect;
		}

		public static Tuple Reflect(Tuple vec, Tuple normal)
		{
			return vec.Reflect(normal);
		}


		private static void CheckValidW(int w)
		{
			if (w != 0 && w != 1)
			{
				throw new Exception("Tuple.w should be 0 or 1");
			}
		}

		public bool IsEqual(Tuple a)
		{
			if (!MathsUtils.Equal(this.x, a.x) ||
				!MathsUtils.Equal(this.y, a.y) ||
				!MathsUtils.Equal(this.z, a.z) ||
				this.w != a.w)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static Tuple Vector(double x, double y, double z)
		{
			return new Tuple(x, y, z, 0);
		}

		public static Tuple Point(double x, double y, double z)
		{
			return new Tuple(x, y, z, 1);
		}

		public static int TestTuple()
		{
			Console.WriteLine("Running TestTuple");
			int numTestsFailed = 0;

			// Test 0
			bool exceptionCaught = false;
			try
			{
				Tuple e = new Tuple(1, 1, 1, 2);
			}
			catch (Exception e)
			{
				exceptionCaught = true;
			}
			if (!exceptionCaught)
			{
				Console.WriteLine("TestTuple Test 0 failed");
				numTestsFailed++;
			}

			// Test 1
			Tuple a = Tuple.Point(4.3, -4.2, 3.1);
			if (!a.IsEqual(new Tuple(4.3, -4.2, 3.1, 1)) || !a.IsPoint() || a.IsVector())
			{
				Console.WriteLine("TestTuple Test 1 failed");
				numTestsFailed++;
			}

			// Test 2
			a = Tuple.Vector(4.3, -4.2, 3.1);
			if (!a.IsEqual(new Tuple(4.3, -4.2, 3.1, 0)) || a.IsPoint() || !a.IsVector())
			{
				Console.WriteLine("TestTuple Test 2 failed");
				numTestsFailed++;
			}

			// Test 3
			a = Tuple.Vector(1.0, 1.0, 1.0);
			Tuple b = Tuple.Vector(2.0, 3.0, 4.0);
			Tuple c = Tuple.Add(a, b);
			if (!c.IsEqual(Tuple.Vector(3.0, 4.0, 5.0)))
			{
				Console.WriteLine("TestTuple Test 3 failed");
				numTestsFailed++;
			}

			// Test 4
			a = Tuple.Vector(1.0, 1.0, 1.0);
			b = Tuple.Vector(2.0, 3.0, 4.0);
			a = a.Add(b);
			if (!a.IsEqual(Tuple.Vector(3.0, 4.0, 5.0)))
			{
				Console.WriteLine("TestTuple Test 4 failed");
				numTestsFailed++;
			}

			// Test 5
			a = Tuple.Vector(1.0, 1.0, 1.0);
			b = Tuple.Point(2.0, 3.0, 4.0);
			c = Tuple.Add(a, b);
			if (!c.IsEqual(Tuple.Point(3.0, 4.0, 5.0)))
			{
				Console.WriteLine("TestTuple Test 5 failed");
				numTestsFailed++;
			}

			// Test 6
			a = Tuple.Vector(1.0, 1.0, 1.0);
			b = Tuple.Point(2.0, 3.0, 4.0);
			a = a.Add(b);
			if (!a.IsEqual(Tuple.Point(3.0, 4.0, 5.0)))
			{
				Console.WriteLine("TestTuple Test 6 failed");
				numTestsFailed++;
			}

			// Test 7
			a = Tuple.Vector(1.0, 1.0, 1.0);
			b = Tuple.Vector(2.0, 3.0, 4.0);
			c = Tuple.Sub(a, b);
			if (!c.IsEqual(Tuple.Vector(-1.0, -2.0, -3.0)))
			{
				Console.WriteLine("TestTuple Test 7 failed");
				numTestsFailed++;
			}

			// Test 8
			a = Tuple.Vector(1.0, 1.0, 1.0);
			b = Tuple.Vector(2.0, 3.0, 4.0);
			a = a.Sub(b);
			if (!a.IsEqual(Tuple.Vector(-1.0, -2.0, -3.0)))
			{
				Console.WriteLine("TestTuple Test 8 failed");
				numTestsFailed++;
			}

			// Test 9 - can't subtract a point from a vector
			exceptionCaught = false;
			try
			{
				a = Tuple.Vector(1.0, 1.0, 1.0);
				b = Tuple.Point(2.0, 3.0, 4.0);
				c = Tuple.Sub(a, b);
			}
			catch (Exception e)
			{
				exceptionCaught = true;
			}
			if (!exceptionCaught)
			{
				Console.WriteLine("TestTuple Test 9 failed");
				numTestsFailed++;
			}

			// Test 10 - can't subtract a point from a vector
			exceptionCaught = false;
			try
			{
				a = Tuple.Vector(1.0, 1.0, 1.0);
				b = Tuple.Point(2.0, 3.0, 4.0);
				a = a.Sub(b);
			}
			catch (Exception e)
			{
				exceptionCaught = true;
			}
			if (!exceptionCaught)
			{
				Console.WriteLine("TestTuple Test 10 failed");
				numTestsFailed++;
			}

			// Test 11
			a = Tuple.Point(1.0, 1.0, 1.0);
			b = Tuple.Vector(2.0, 3.0, 4.0);
			c = Tuple.Sub(a, b);
			if (!c.IsEqual(Tuple.Point(-1.0, -2.0, -3.0)))
			{
				Console.WriteLine("TestTuple Test 11 failed");
				numTestsFailed++;
			}

			// Test 12
			a = Tuple.Point(1.0, 1.0, 1.0);
			b = Tuple.Vector(2.0, 3.0, 4.0);
			a = a.Sub(b);
			if (!a.IsEqual(Tuple.Point(-1.0, -2.0, -3.0)))
			{
				Console.WriteLine("TestTuple Test 12 failed");
				numTestsFailed++;
			}

			// Test 13
			a = Tuple.Vector(1, -1, 2);
			a = a.Negate();
			if (!a.IsEqual(Tuple.Vector(-1, 1, -2)))
			{
				Console.WriteLine("TestTuple Test 13 failed");
				numTestsFailed++;
			}

			// Test 14 - can't negate a Point
			exceptionCaught = false;
			try
			{
				a = Tuple.Point(1, -1, 2);
				a = a.Negate();
			}
			catch (Exception e)
			{
				exceptionCaught = true;
			}
			if (!exceptionCaught)
			{
				Console.WriteLine("TestTuple Test 14 failed");
				numTestsFailed++;
			}

			// Test 15
			a = Tuple.Vector(1, -2, 3);
			a = a.Multiply(3.5);
			if (!a.IsEqual(Tuple.Vector(3.5, -7, 10.5)))
			{
				Console.WriteLine("TestTuple Test 15 failed");
				numTestsFailed++;
			}

			// Test 16
			a = Tuple.Vector(1, -2, 3);
			a = a.Divide(2);
			if (!a.IsEqual(Tuple.Vector(0.5, -1, 1.5)))
			{
				Console.WriteLine("TestTuple Test 16 failed");
				numTestsFailed++;
			}

			// Test 17
			a = Tuple.Vector(1, 0, 0);
			double mag = a.Magnitude();
			if (!MathsUtils.Equal(mag, 1.0))
			{
				Console.WriteLine("TestTuple Test 17 failed");
				numTestsFailed++;
			}

			// Test 18
			a = Tuple.Vector(1, 2, 3);
			mag = a.Magnitude();
			if (!MathsUtils.Equal(mag, Math.Sqrt(14.0)))
			{
				Console.WriteLine("TestTuple Test 18 failed");
				numTestsFailed++;
			}

			// Test 19
			a = Tuple.Vector(4, 0, 0);
			a = a.Normalise();
			if (!a.IsEqual(Tuple.Vector(1, 0, 0)))
			{
				Console.WriteLine("TestTuple Test 19 failed");
				numTestsFailed++;
			}

			// Test 20
			a = Tuple.Vector(1, 2, 3);
			a = a.Normalise();
			if (!a.IsEqual(Tuple.Vector(1 / Math.Sqrt(14.0), 2 / Math.Sqrt(14.0), 3 / Math.Sqrt(14.0))))
			{
				Console.WriteLine("TestTuple Test 20 failed");
				numTestsFailed++;
			}

			// Test 21
			a = Tuple.Vector(1, 2, 3);
			b = Tuple.Vector(2, 3, 4);
			double dot = a.Dot(b);
			if (!MathsUtils.Equal(dot, 20))
			{
				Console.WriteLine("TestTuple Test 21 failed");
				numTestsFailed++;
			}

			// Test 22
			a = Tuple.Vector(1, 2, 3);
			b = Tuple.Vector(2, 3, 4);
			Tuple cross = a.Cross(b);
			if (!cross.IsEqual(Tuple.Vector(-1, 2, -1)))
			{
				Console.WriteLine("TestTuple Test 22 failed");
				numTestsFailed++;
			}

			// Test 23
			a = Tuple.Vector(1, 2, 3);
			b = Tuple.Vector(2, 3, 4);
			cross = b.Cross(a);
			if (!cross.IsEqual(Tuple.Vector(1, -2, 1)))
			{
				Console.WriteLine("TestTuple Test 23 failed");
				numTestsFailed++;
			}

			Console.WriteLine("TestTuple complete numTestsFailed=" + numTestsFailed);
			return numTestsFailed;
		}
	}
}
