using MathNet.Numerics.LinearAlgebra.Double;

namespace GFunctions.Mathnet
{
    /// <summary>
    /// Contains various calculation methods for kinematics
    /// </summary>
    public class KinematicMath
    {
        /// <summary>
        /// Gets a rotation matrix from pitch roll yaw angles
        /// </summary>
        /// <param name="rotation">Pitch, roll, yaw in degrees</param>
        /// <returns>Equivalent rotation matrix</returns>
        public static DenseMatrix RotationMatrixFromPRY(double[] rotation)
        {
            if (rotation.Length != 3)
                throw new ArgumentException("Cannot calculate rotation matrix. Invalid number of arguments provided in array.");
                
            double Pitch = rotation[0] * Math.PI / 180.0;
            double Roll = rotation[1] * Math.PI / 180.0;
            double Yaw = rotation[2] * Math.PI / 180.0;

            //Math: http://planning.cs.uiuc.edu/node102.html

            //------------ Pitch Matrix -----------------------

            DenseMatrix PitchMat = new DenseMatrix(3, 3);

            PitchMat[0, 0] = Math.Cos(Pitch);
            PitchMat[1, 0] = 0;
            PitchMat[2, 0] = -1 * Math.Sin(Pitch);
            PitchMat[0, 1] = 0;
            PitchMat[1, 1] = 1;
            PitchMat[2, 1] = 0;
            PitchMat[0, 2] = Math.Sin(Pitch);
            PitchMat[1, 2] = 0;
            PitchMat[2, 2] = Math.Cos(Pitch);

            //------------ Roll Matrix -----------------------

            var RollMat = new DenseMatrix(3, 3);

            RollMat[0, 0] = 1;
            RollMat[1, 0] = 0;
            RollMat[2, 0] = 0;
            RollMat[0, 1] = 0;
            RollMat[1, 1] = Math.Cos(Roll);
            RollMat[2, 1] = Math.Sin(Roll);
            RollMat[0, 2] = 0;
            RollMat[1, 2] = -1 * Math.Sin(Roll);
            RollMat[2, 2] = Math.Cos(Roll);

            //------------ Yaw Matrix -----------------------

            var YawMat = new DenseMatrix(3, 3);

            YawMat[0, 0] = Math.Cos(Yaw);
            YawMat[1, 0] = Math.Sin(Yaw);
            YawMat[2, 0] = 0;
            YawMat[0, 1] = -1 * Math.Sin(Yaw);
            YawMat[1, 1] = Math.Cos(Yaw);
            YawMat[2, 1] = 0;
            YawMat[0, 2] = 0;
            YawMat[1, 2] = 0;
            YawMat[2, 2] = 1;


            DenseMatrix output = YawMat * PitchMat * RollMat;

            return output;
        }

        /// <summary>
        /// Gets the length between two Nd vectors of the same order
        /// </summary>
        /// <param name="pos1">Position 1 (x,y,....)</param>
        /// <param name="pos2">Position 2 (x,y,....)</param>
        /// <returns>Distance between the two positions</returns>
        public static double VectorLength(double[] pos1, double[] pos2)
        {
            double output = 0;

            for (int i = 0; i < pos1.Length; i++)
            {
                output += (pos2[i] - pos1[i]) * (pos2[i] - pos1[i]);
            }

            return Math.Sqrt(output);
        }

        /// <summary>
        /// Rotates a vector by the given pitch roll yaw angles
        /// </summary>
        /// <param name="vector">The vector to rotate</param>
        /// <param name="rotationPRY">Pitch, roll, yaw rotation in degrees</param>
        /// <returns>The rotated vector (x,y,z)</returns>
        public static Vector3 RotateVector(Vector3 vector, RotationPRY rotationPRY)
        {
            DenseVector vectorObj = new(vector.ToArray()); // Local vector without rotation;
            DenseMatrix rotation = rotationPRY.ToRotationMatrix();

            DenseVector rotatedVector = rotation * vectorObj; // Apply rotation

            return new(rotatedVector[0], rotatedVector[1], rotatedVector[2]);
        }

        /// <summary>
        /// Calculates a coordinates with the applied translations and rotation
        /// </summary>
        /// <param name="localCoord">The local coordinate (x,y,z)</param>
        /// <param name="trans1">First translation distance (x,y,z)</param>
        /// <param name="trans2">Second translation distance (x,y,z)</param>
        /// <param name="rotation">Pitch, roll, yaw rotation in degrees</param>
        /// <returns>Transformed coordinates (x,y,z)</returns>
        public static Vector3 CalcGlobalCoord(Vector3 localCoord, Vector3 trans1, Vector3 trans2, RotationPRY rotation)
        {
            DenseMatrix LocalCoords = localCoord.ToColumnMatrix();
            DenseMatrix TranslationMat = trans1.ToColumnMatrix();
            DenseMatrix StartingMat = trans2.ToColumnMatrix();

            DenseMatrix GlobalCoords = (rotation.ToRotationMatrix() * LocalCoords) + TranslationMat + StartingMat;

            return new(GlobalCoords[0, 0], GlobalCoords[1, 0], GlobalCoords[2, 0]);
        }

        /// <summary>
        /// Calculates a coordinates with the applied translations and rotation
        /// </summary>
        /// <param name="localCoord">The local coordinate (x,y,z)</param>
        /// <param name="trans1">Translation distance (x,y,z) before rotation</param>
        /// <param name="trans2">Translation distance (x,y,z) after rotation</param>
        /// <param name="rotation">Pitch, roll, yaw rotation in degrees</param>
        /// <param name="relativeRotCenter">Coordinates of the rotation center (x,y,z) relative to the local coordinate</param>
        /// <returns>Transformed coordinates (x,y,z)</returns>
        public static Vector3 CalcGlobalCoord2(Vector3 localCoord, Vector3 trans1, Vector3 trans2, RotationPRY rotation, Vector3? relativeRotCenter = null)
        {
            // If no relative center, set the rotation center to (0, 0, 0)
            DenseMatrix RotCenter = (relativeRotCenter ?? new Vector3(0, 0, 0)).ToColumnMatrix();
            DenseMatrix LocalCoords = localCoord.ToColumnMatrix();
            DenseMatrix TranslationMat = trans1.ToColumnMatrix();
            DenseMatrix TranslationMat2 = trans2.ToColumnMatrix();

            DenseMatrix GlobalCoords = (rotation.ToRotationMatrix() * (LocalCoords - RotCenter + TranslationMat)) + TranslationMat2 + RotCenter; //Rotcenter needs to be added before rotation, then removed after

            return new(GlobalCoords[0, 0], GlobalCoords[1, 0], GlobalCoords[2, 0]);
        }
    }
}
