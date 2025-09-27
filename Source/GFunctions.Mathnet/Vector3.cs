using MathNet.Numerics.LinearAlgebra.Double;

namespace GFunctions.Mathnet
{
    /// <summary>
    /// A 3 dimensional (XYZ) vector
    /// </summary>
    public class Vector3
    {
        // -------------------- Properties --------------------

        /// <summary>
        /// X Component
        /// </summary>
        public double X { get; set; } = 0;

        /// <summary>
        /// Y Component
        /// </summary>
        public double Y { get; set; } = 0;

        /// <summary>
        /// Z Component
        /// </summary>
        public double Z { get; set; } = 0;

        /// <summary>
        /// Vector length
        /// </summary>
        public double Magnitude => Math.Sqrt(X * X + Y * Y + Z * Z);

        // -------------------- Constructors --------------------

        /// <summary>
        /// Initialize to zero
        /// </summary>
        public Vector3() { }

        /// <summary>
        /// Initialize with values
        /// </summary>
        /// <param name="x">X Component</param>
        /// <param name="y">Y Component</param>
        /// <param name="z">Z Component</param>
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initialize from an array
        /// </summary>
        /// <param name="array">An array of [x, y, z]</param>
        public Vector3(double[] array)
        {
            if (array.Length != 3)
                throw new ArgumentException("Cannot initialize Vector3 with an array other than length of 3");
            
            X = array[0];
            Y = array[1];
            Z = array[2];
        }

        // -------------------- Public Methods --------------------

        /// <summary>
        /// Normalize the vector to a magnitude, while maintaining direction
        /// </summary>
        /// <param name="newMagnitude">Optional magnitude (defaults to 1)</param>
        /// <exception cref="InvalidOperationException">Can't normalize a vector with zero length</exception>
        public void Normalize(double newMagnitude = 1.0)
        {
            double magnitude = Magnitude;
            if (magnitude == 0)
                throw new InvalidOperationException("Cannot normalize a zero-length vector.");

            X *= newMagnitude / magnitude;
            Y *= newMagnitude / magnitude;
            Z *= newMagnitude / magnitude;
        }

        /// <summary>
        /// Gets the unit vector (length = 1) of this vector
        /// </summary>
        /// <returns>The unit vector</returns>
        /// <exception cref="InvalidOperationException">Can't normalize a vector with zero length</exception>
        public Vector3 GetUnitVector()
        {
            double mag = Magnitude;
            if (mag == 0)
                throw new InvalidOperationException("Cannot normalize a zero-length vector.");
            
            return new Vector3(X / mag, Y / mag, Z / mag);
        }

        /// <summary>
        /// Compute the vector dot product
        /// </summary>
        /// <param name="other">Other vector</param>
        /// <returns>Dot product between this vector and the other</returns>
        public double Dot(Vector3 other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        /// <summary>
        /// Compute the length between this vector and the other one
        /// </summary>
        /// <param name="other">Other vector</param>
        /// <returns>The distance between this vector and the other</returns>
        public double LengthBetween(Vector3 other)
        {
            return KinematicMath.VectorLength(ToArray(), other.ToArray());
        }

        /// <summary>
        /// Get the array implementation (X, Y, Z)
        /// </summary>
        /// <returns>The vector in array form</returns>
        public double[] ToArray()
        {
            return [X, Y, Z];
        }

        /// <summary>
        /// Get the matrix with values in a single column
        /// </summary>
        /// <returns>The vector in column maxtrix form</returns>
        public DenseMatrix ToColumnMatrix()
        {
            DenseMatrix matrix = new(3, 1);
            matrix.SetColumn(0, ToArray());
            return matrix;
        }

        /// <summary>
        /// Perform an arbitrary operation on all coordinates of the vector
        /// </summary>
        /// <param name="operation">The function to operate on each coordinate in the vector</param>
        /// <returns>A vector which is the result of the operation on X, Y, and Z</returns>
        public Vector3 Operate(Func<double, double> operation)
        {
            return new Vector3(
                operation(X),
                operation(Y),
                operation(Z)
            );
        }

        /// <summary>
        /// Checks if this vector equals another one
        /// </summary>
        /// <param name="obj">Input object to test</param>
        /// <returns>True of the objects are equivalent</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Vector3 v)
            {
                return X == v.X && Y == v.Y && Z == v.Z;
            }
            return false;
        }

        /// <summary>
        /// Compute the hash code
        /// </summary>
        /// <returns>The object hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        // -------------------- Operator overloads --------------------

        /// <summary>
        /// Add two vectors together
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Sum of the two vectors</returns>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Subtract two vectors
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Difference of the two vectors</returns>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Negate a vector
        /// </summary>
        /// <param name="v">Input vector</param>
        /// <returns>The vector, negated</returns>
        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }

        /// <summary>
        /// Multiply a vector by a value
        /// </summary>
        /// <param name="v">The vector</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>The vector multiplied by the scalar</returns>
        public static Vector3 operator *(Vector3 v, double scalar)
        {
            return new Vector3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        /// <summary>
        /// Multiply a vector by a value
        /// </summary>
        /// <param name="v">The vector</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>The vector multiplied by the scalar</returns>
        public static Vector3 operator *(double scalar, Vector3 v)
        {
            return v * scalar;
        }

        /// <summary>
        /// Divide a vector by a value
        /// </summary>
        /// <param name="v">The vector</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>The vector divided by the scalar</returns>
        public static Vector3 operator /(Vector3 v, double scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero.");

            return new Vector3(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }
    }
}
