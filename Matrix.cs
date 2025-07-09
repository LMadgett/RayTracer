using System;

namespace Raytracer
{
    public class Matrix
    {
        // index into this matrix array as (x,y) coordinate i.e. columnID first then rowID
        double[,] matrix;
        int size;

        public Matrix (int size)
        {
            if (size < 2 || size > 4)
            {
                throw new Exception("Matrix size must be 2<=size<=4");
            }
            this.size = size;

            matrix = new double[size, size];
        }

        public static Matrix GetIdentity (int size)
        {
            return new Matrix(size).SetIdentity();
        }

        // set this matrix to be the identity
        public Matrix SetIdentity ()
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (x == y)
                        matrix[x, y] = 1.0;
                    else
                        matrix[x, y] = 0.0;
                }
            }

            return this;
        }

        // Translation matrices will always be size 4
        public static Matrix GetTranslation(double x, double y, double z)
        {
            return new Matrix(4).SetTranslation(x, y, z);
        }

        public Matrix SetTranslation (double x, double y, double z)
        {
            SetIdentity();
            matrix[3, 0] = x;
            matrix[3, 1] = y;
            matrix[3, 2] = z;

            return this;
        }

        // Scaling matrices will always be size 4
        public static Matrix GetScaling(double x, double y, double z)
        {
            return new Matrix(4).SetScaling(x, y, z);
        }

        public static Matrix GetRotation(double rotX, double rotY, double rotZ)
        {
            Matrix returnMatrix = Matrix.GetRotateX(rotX).Multiply(Matrix.GetRotateY(rotY)).Multiply(Matrix.GetRotateZ(rotZ));
            return returnMatrix;
        }

        public Matrix SetScaling(double x, double y, double z)
        {
            SetIdentity();
            matrix[0, 0] = x;
            matrix[1, 1] = y;
            matrix[2, 2] = z;

            return this;
        }

        // Rotate matrices will always be size 4
        public static Matrix GetRotateX(double rot)
        {
            return new Matrix(4).SetRotateX(rot);
        }

        public Matrix SetRotateX(double rot)
        {
            rot = MathsUtils.DegToRad(rot);

            SetIdentity();
            matrix[1, 1] = Math.Cos(rot);
            matrix[2, 1] = -Math.Sin(rot);
            matrix[1, 2] = Math.Sin(rot);
            matrix[2, 2] = Math.Cos(rot);

            return this;
        }

        // Rotate matrices will always be size 4
        public static Matrix GetRotateY(double rot)
        {
            return new Matrix(4).SetRotateY(rot);
        }

        public Matrix SetRotateY(double rot)
        {
            rot = MathsUtils.DegToRad(rot);

            SetIdentity();
            matrix[0, 0] = Math.Cos(rot);
            matrix[2, 0] = Math.Sin(rot);
            matrix[0, 2] = -Math.Sin(rot);
            matrix[2, 2] = Math.Cos(rot);

            return this;
        }

        // Rotate matrices will always be size 4
        public static Matrix GetRotateZ(double rot)
        {
            return new Matrix(4).SetRotateZ(rot);
        }

        public Matrix SetRotateZ(double rot)
        {
            rot = MathsUtils.DegToRad(rot);

            SetIdentity();
            matrix[0, 0] = Math.Cos(rot);
            matrix[1, 0] = -Math.Sin(rot);
            matrix[0, 1] = Math.Sin(rot);
            matrix[1, 1] = Math.Cos(rot);

            return this;
        }

        public static Matrix GetShear(double xy, double xz, double yx, double yz, double zx, double zy)
        {
            return new Matrix(4).SetShear(xy, xz, yx, yz, zx, zy);
        }

        public Matrix SetShear(double xy, double xz, double yx, double yz, double zx, double zy)
        {
            SetIdentity();
            matrix[1, 0] = xy;
            matrix[2, 0] = xz;
            matrix[0, 1] = yx;
            matrix[2, 1] = yz;
            matrix[0, 2] = zx;
            matrix[1, 2] = zy;

            return this;
        }

        public double GetValue (int x, int y)
        {
            return matrix[x, y];
        }

        public void SetValue(int x, int y, double value)
        {
            matrix[x, y] = value;
        }

        public bool Equal (Matrix other)
        {
            bool isEqual = true;
            if (other == null)
            {
                return false;
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (!MathsUtils.Equal(GetValue(x,y), other.GetValue(x, y)))
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (!isEqual) break;
            }

            return isEqual;
        }

        public Matrix GetTranspose()
        {
            Matrix trans = new Matrix(size);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    trans.matrix[x, y] = matrix[y, x];
                    trans.matrix[y, x] = matrix[x, y];

                    if (x == y) break;
                }
            }

            return trans;
        }

        public Matrix Multiply (Matrix other)
        {
            return Multiply(other, new Matrix(size));
        }

        public Matrix Multiply (Matrix other, Matrix result)
        {
            if (other.size != size)
            {
                throw new Exception("Matrix multiply must be between two equal size matrices");
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    double tot = 0;
                    for (int i = 0; i < size; i++)
                    {
                        tot = tot + matrix[i, y] * other.matrix[x, i];
                    }
                    result.matrix[x, y] = tot;
                }
            }

            return result;
        }

        public Tuple Multiply (Tuple other)
        {
            if (size != 4)
            {
                throw new Exception("Matrix.Multiply(Tuple) must only be with 4x4 matrix");
            }

            Tuple result = new Tuple(0, 0, 0, 0);

            for (int y = 0; y < size; y++)
            {
                double tot = 0;
                for (int i = 0; i < size; i++)
                {
                    double tupleVal = 0;
                    if (i == 0)
                        tupleVal = other.x;
                    else if (i == 1)
                        tupleVal = other.y;
                    else if (i == 2)
                        tupleVal = other.z;
                    else if (i == 3)
                        tupleVal = other.w;
                    tot = tot + matrix[i, y] * tupleVal;
                }
                if (y == 0)
                    result.x = tot;
                else if (y == 1)
                    result.y = tot;
                else if (y == 2)
                    result.z = tot;
                else if (y == 3)
                    result.w = (int)tot;
            }

            return result;
        }

        public double Determinant ()
        {
            double determinant = 0.0;

            if (size == 2)
            {
                determinant = Determinant2x2();
            }
            else
            {
                for (int x = 0; x < size; x++)
                {
                    determinant += matrix[x, 0] * Cofactor(x, 0);
                }
            }

            return determinant;
        }

        private double Determinant2x2 ()
        {
            if (size != 2)
            {
                throw new Exception("Determinant2x2() must only be called on 2x2 matrix");
            }

            // determinant = ad-bc
            double determinant = (matrix[0, 0] * matrix[1, 1]) - (matrix[1, 0] * matrix[0, 1]);
            return determinant;
        }

        private Matrix SubMatrix (int col, int row)
        {
            if (size <= 2)
            {
                throw new Exception("SubMatrix() must only be called on matrix >2 size");
            }

            Matrix rMatrix = new Matrix(size - 1);

            int rX = 0, rY = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (x != col && y != row)
                    {
                        rMatrix.matrix[rX, rY] = matrix[x, y];
                        rX++;
                    }
                }
                rX = 0;
                if (y != row) rY++;
            }

            return rMatrix;
        }

        private double Minor (int col, int row)
        {
            return SubMatrix(col, row).Determinant();
        }

        private double Cofactor (int col, int row)
        {
            double minor = Minor(col, row);
            // is minor odd?
            if (((col + row) & 1) == 1)
            {
                minor = -minor;
            }

            return minor;
        }

        public Matrix GetInverse ()
        {
            Matrix inverse = new Matrix(size);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    inverse.matrix[x, y] = this.Cofactor(x, y);
                }
            }

            inverse = inverse.GetTranspose();

            double determinant = this.Determinant();

            if (MathsUtils.Equal(determinant, 0))
            {
                inverse = null;
            }
            else
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        inverse.matrix[x, y] = inverse.matrix[x, y] / determinant;
                    }
                }
            }

            return inverse;
        }

        public void PrintMatrix ()
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // NOTE we are doing (y,x) here not (x,y) in order to transpose matrix to
                    // match what we normally expect to see vs how we're storing internally (which is flipped)
                    Console.Write(matrix[y, x]);
                    if (x < size - 1) Console.Write(",");
                }
                Console.WriteLine("");
            }
        }

        public static int TestMatrix()
        {
            Console.WriteLine("Running TestMatrix");
            int numTestsFailed = 0;

            // Test 1 test compare and multiply
            Matrix a = new Matrix(2);
            a.SetValue(0, 0, 1);
            a.SetValue(0, 1, 2);
            a.SetValue(1, 0, 3);
            a.SetValue(1, 1, 4);
            //Console.WriteLine("a=");
            //a.PrintMatrix();
            Matrix b = new Matrix(2);
            b.SetValue(0, 0, 1);
            b.SetValue(0, 1, 2);
            b.SetValue(1, 0, 3);
            b.SetValue(1, 1, 4);
            //Console.WriteLine("b=");
            //b.PrintMatrix();
            Matrix result = a.Multiply(b);
            //Console.WriteLine("result=");
            //result.PrintMatrix();
            Matrix c = new Matrix(2);
            c.SetValue(0, 0, 7);
            c.SetValue(0, 1, 10);
            c.SetValue(1, 0, 15);
            c.SetValue(1, 1, 22);
            if (!result.Equal(c))
            {
                Console.WriteLine("TestMatrix Test 1 failed");
                numTestsFailed++;
            }

            // Test 2 transpose
            a.SetValue(0, 0, 1);
            a.SetValue(0, 1, 2);
            a.SetValue(1, 0, 3);
            a.SetValue(1, 1, 4);
            a = a.GetTranspose();
            c.SetValue(0, 0, 1);
            c.SetValue(0, 1, 3);
            c.SetValue(1, 0, 2);
            c.SetValue(1, 1, 4);
            if (!a.Equal(c))
            {
                Console.WriteLine("TestMatrix Test 2 failed");
                numTestsFailed++;
            }

            // Test 3 submatrix
            a = new Matrix(3);
            a.SetValue(0, 0, 1);
            a.SetValue(0, 1, 2);
            a.SetValue(0, 2, 3);
            a.SetValue(1, 0, 4);
            a.SetValue(1, 1, 5);
            a.SetValue(1, 2, 6);
            a.SetValue(2, 0, 7);
            a.SetValue(2, 1, 8);
            a.SetValue(2, 2, 9);
            b = a.SubMatrix(1, 1);
            c = new Matrix(2);
            c.SetValue(0, 0, 1);
            c.SetValue(0, 1, 3);
            c.SetValue(1, 0, 7);
            c.SetValue(1, 1, 9);
            if (!b.Equal(c))
            {
                Console.WriteLine("TestMatrix Test 3 failed");
                numTestsFailed++;
            }

            // Test 4 minor
            a = new Matrix(3);
            a.SetValue(0, 0, 3);
            a.SetValue(1, 0, 5);
            a.SetValue(2, 0, 0);
            a.SetValue(0, 1, 2);
            a.SetValue(1, 1, -1);
            a.SetValue(2, 1, -7);
            a.SetValue(0, 2, 6);
            a.SetValue(1, 2, -1);
            a.SetValue(2, 2, 5);
            double minor = a.Minor(0, 1);
            if (minor != 25)
            {
                Console.WriteLine("TestMatrix Test 4 failed");
                numTestsFailed++;
            }

            // Test 5 cofactor
            a = new Matrix(3);
            a.SetValue(0, 0, 3);
            a.SetValue(1, 0, 5);
            a.SetValue(2, 0, 0);
            a.SetValue(0, 1, 2);
            a.SetValue(1, 1, -1);
            a.SetValue(2, 1, -7);
            a.SetValue(0, 2, 6);
            a.SetValue(1, 2, -1);
            a.SetValue(2, 2, 5);
            double cofactor = a.Cofactor(0, 0);
            if (cofactor != -12)
            {
                Console.WriteLine("TestMatrix Test 5 pt 1 failed");
                numTestsFailed++;
            }
            cofactor = a.Cofactor(0, 1);
            if (cofactor != -25)
            {
                Console.WriteLine("TestMatrix Test 5 pt 2 failed");
                numTestsFailed++;
            }

            // Test 6 determinant 3x3
            a = new Matrix(3);
            a.SetValue(0, 0, 1);
            a.SetValue(1, 0, 2);
            a.SetValue(2, 0, 6);
            a.SetValue(0, 1, -5);
            a.SetValue(1, 1, 8);
            a.SetValue(2, 1, -4);
            a.SetValue(0, 2, 2);
            a.SetValue(1, 2, 6);
            a.SetValue(2, 2, 4);
            cofactor = a.Cofactor(0, 0);
            double determinant = a.Determinant();
            if (cofactor != 56 || determinant != -196)
            {
                Console.WriteLine("TestMatrix Test 6 failed");
                numTestsFailed++;
            }

            // Test 7 determinant 4x4
            a = new Matrix(4);
            a.SetValue(0, 0, -2);
            a.SetValue(1, 0, -8);
            a.SetValue(2, 0, 3);
            a.SetValue(3, 0, 5);
            a.SetValue(0, 1, -3);
            a.SetValue(1, 1, 1);
            a.SetValue(2, 1, 7);
            a.SetValue(3, 1, 3);
            a.SetValue(0, 2, 1);
            a.SetValue(1, 2, 2);
            a.SetValue(2, 2, -9);
            a.SetValue(3, 2, 6);
            a.SetValue(0, 3, -6);
            a.SetValue(1, 3, 7);
            a.SetValue(2, 3, 7);
            a.SetValue(3, 3, -9);
            cofactor = a.Cofactor(0, 0);
            determinant = a.Determinant();
            if (cofactor != 690 || determinant != -4071)
            {
                Console.WriteLine("TestMatrix Test 7 failed");
                numTestsFailed++;
            }

            // Test 8 inverse 4x4
            a = new Matrix(4);
            a.SetValue(0, 0, -5);
            a.SetValue(1, 0, 2);
            a.SetValue(2, 0, 6);
            a.SetValue(3, 0, -8);
            a.SetValue(0, 1, 1);
            a.SetValue(1, 1, -5);
            a.SetValue(2, 1, 1);
            a.SetValue(3, 1, 8);
            a.SetValue(0, 2, 7);
            a.SetValue(1, 2, 7);
            a.SetValue(2, 2, -6);
            a.SetValue(3, 2, -7);
            a.SetValue(0, 3, 1);
            a.SetValue(1, 3, -3);
            a.SetValue(2, 3, 7);
            a.SetValue(3, 3, 4);
            Matrix inverse = a.GetInverse();
            c = new Matrix(4);
            c.SetValue(0, 0, 0.21805);
            c.SetValue(1, 0, 0.45113);
            c.SetValue(2, 0, 0.24060);
            c.SetValue(3, 0, -0.04511);
            c.SetValue(0, 1, -0.80827);
            c.SetValue(1, 1, -1.45677);
            c.SetValue(2, 1, -0.44361);
            c.SetValue(3, 1, 0.52068);
            c.SetValue(0, 2, -0.07895);
            c.SetValue(1, 2, -0.22368);
            c.SetValue(2, 2, -0.05263);
            c.SetValue(3, 2, 0.19737);
            c.SetValue(0, 3, -0.52256);
            c.SetValue(1, 3, -0.81391);
            c.SetValue(2, 3, -0.30075);
            c.SetValue(3, 3, 0.30639);
            if (inverse == null || !inverse.Equal(c))
            {
                Console.WriteLine("TestMatrix Test 8 failed");
                numTestsFailed++;
            }

            // Test 9 multiply matrix by tuple
            a = new Matrix(4);
            a.SetValue(0, 0, 1);
            a.SetValue(1, 0, 2);
            a.SetValue(2, 0, 3);
            a.SetValue(3, 0, 4);
            a.SetValue(0, 1, 1);
            a.SetValue(1, 1, 2);
            a.SetValue(2, 1, 3);
            a.SetValue(3, 1, 4);
            a.SetValue(0, 2, 1);
            a.SetValue(1, 2, 2);
            a.SetValue(2, 2, 3);
            a.SetValue(3, 2, 4);
            a.SetValue(0, 3, 0);
            a.SetValue(1, 3, 0);
            a.SetValue(2, 3, 0);
            a.SetValue(3, 3, 1);
            Tuple t = new Tuple(1, 2, 3, 1);
            Tuple r = a.Multiply(t);
            if (!MathsUtils.Equal(r.x, 18) || !MathsUtils.Equal(r.y, 18) || !MathsUtils.Equal(r.z, 18) || !MathsUtils.Equal(r.w, 1))
            {
                Console.WriteLine("TestMatrix Test 9 failed");
                numTestsFailed++;
            }

            Console.WriteLine("TestMatrix complete numTestsFailed=" + numTestsFailed);
            return numTestsFailed;
        }

    }
}
