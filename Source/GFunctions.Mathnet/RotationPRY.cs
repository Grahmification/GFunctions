using MathNet.Numerics.LinearAlgebra.Double;

namespace GFunctions.Mathnet
{
    /// <summary>
    /// Combined Pitch Roll Yaw Rotation
    /// </summary>
    public class RotationPRY
    {
        // -------------------- Properties --------------------

        /// <summary>
        /// Pitch angle in degrees
        /// </summary>
        public double Pitch { get; set; } = 0;

        /// <summary>
        /// Roll angle in degrees
        /// </summary>
        public double Roll { get; set; } = 0;

        /// <summary>
        /// Yaw angle in degrees
        /// </summary>
        public double Yaw { get; set; } = 0;

        // -------------------- Constructors --------------------

        /// <summary>
        /// Initialize to zero
        /// </summary>
        public RotationPRY() { }

        /// <summary>
        /// Initialize with values
        /// </summary>
        /// <param name="pitch">Pitch angle in degrees</param>
        /// <param name="roll">Roll angle in degrees</param>
        /// <param name="yaw">Yaw angle in degrees</param>
        public RotationPRY(double pitch, double roll, double yaw)
        {
            Pitch = pitch;
            Roll = roll;
            Yaw = yaw;
        }

        /// <summary>
        /// Initialize from an array
        /// </summary>
        /// <param name="array">An array of [pitch, roll, yaw] in degrees</param>
        public RotationPRY(double[] array)
        {
            if (array.Length != 3)
                throw new ArgumentException("Cannot initialize RotationPRY with an array other than length of 3");

            Pitch = array[0];
            Roll = array[1];
            Yaw = array[2];
        }

        // -------------------- Public Methods --------------------

        /// <summary>
        /// Get the equivalent rotation matrix for the rotation
        /// </summary>
        /// <returns>The rotation matrix</returns>
        public DenseMatrix ToRotationMatrix()
        {
            return KinematicMath.RotationMatrixFromPRY([Pitch, Roll, Yaw]);
        }

        /// <summary>
        /// Perform an arbitrary operation on all coordinates of the Rotation
        /// </summary>
        /// <param name="operation">The function to operate on each coordinate in the rotation</param>
        /// <returns>A rotation which is the result of the operation on Pitch, Roll, and Yaw</returns>
        public RotationPRY Operate(Func<double, double> operation)
        {
            return new RotationPRY(
                operation(Pitch),
                operation(Roll),
                operation(Yaw)
            );
        }

        /// <summary>
        /// Get the array implementation [Pitch, Roll, Yaw]
        /// </summary>
        /// <returns>The rotation in array form</returns>
        public double[] ToArray()
        {
            return [Pitch, Roll, Yaw];
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
                return Pitch == v.X && Roll == v.Y && Yaw == v.Z;
            }
            return false;
        }

        /// <summary>
        /// Compute the hash code
        /// </summary>
        /// <returns>The object hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Pitch, Roll, Yaw);
        }

        // -------------------- Operator overloads --------------------

        /// <summary>
        /// Add two rotations together
        /// </summary>
        /// <param name="a">Rotation a</param>
        /// <param name="b">Rotation b</param>
        /// <returns>Sum of the two rotations</returns>
        public static RotationPRY operator +(RotationPRY a, RotationPRY b)
        {
            return new RotationPRY(a.Pitch + b.Pitch, a.Roll + b.Roll, a.Yaw + b.Yaw);
        }

        /// <summary>
        /// Subtract two rotations
        /// </summary>
        /// <param name="a">Rotation a</param>
        /// <param name="b">Rotation b</param>
        /// <returns>Difference of the two rotations</returns>
        public static RotationPRY operator -(RotationPRY a, RotationPRY b)
        {
            return new RotationPRY(a.Pitch - b.Pitch, a.Roll - b.Roll, a.Yaw - b.Yaw);
        }

        /// <summary>
        /// Multiply the rotation by a value
        /// </summary>
        /// <param name="v">The rotation</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>The rotation multiplied by the scalar</returns>
        public static RotationPRY operator *(RotationPRY v, double scalar)
        {
            return new RotationPRY(v.Pitch * scalar, v.Roll * scalar, v.Yaw * scalar);
        }

        /// <summary>
        /// Multiply the rotation by a value
        /// </summary>
        /// <param name="v">The rotation</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>The rotation multiplied by the scalar</returns>
        public static RotationPRY operator *(double scalar, RotationPRY v)
        {
            return v * scalar;
        }

        /// <summary>
        /// Divide a rotation by a value
        /// </summary>
        /// <param name="v">The rotation</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>The rotation divided by the scalar</returns>
        public static RotationPRY operator /(RotationPRY v, double scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero.");

            return new RotationPRY(v.Pitch / scalar, v.Roll / scalar, v.Yaw / scalar);
        }
    }
}
