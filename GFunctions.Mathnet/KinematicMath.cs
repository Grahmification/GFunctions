/*Licence Declaration:

This code uses the Math.NET Numerics library under the MIT License

Copyright (c) 2002-2020 Math.NET

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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
        /// <param name="vector">The vector to rotate (x,y,z)</param>
        /// <param name="rotationPRY">Pitch, roll, yaw rotation in degrees</param>
        /// <returns>The rotated vector (x,y,z)</returns>
        public static double[] RotateVector(double[] vector, double[] rotationPRY)
        {
            DenseVector vectorObj = new(vector); //local vector without rotation;
            DenseMatrix rotation = RotationMatrixFromPRY(rotationPRY);

            DenseVector rotatedVector = rotation * vectorObj; //apply rotation

            return [.. rotatedVector];
        }

        /// <summary>
        /// Calculates a coordinates with the applied translations and rotation
        /// </summary>
        /// <param name="localCoord">The local coordinate (x,y,z)</param>
        /// <param name="trans1">First translation distance (x,y,z)</param>
        /// <param name="trans2">Second translation distance (x,y,z)</param>
        /// <param name="rotation">Pitch, roll, yaw rotation in degrees</param>
        /// <returns>Transformed coordinates (x,y,z)</returns>
        public static double[] CalcGlobalCoord(double[] localCoord, double[] trans1, double[] trans2, double[] rotation)
        {
            DenseMatrix LocalCoords = new(3, 1);
            LocalCoords.SetColumn(0, localCoord);

            DenseMatrix TranslationMat = new(3, 1);
            TranslationMat.SetColumn(0, trans1);

            DenseMatrix StartingMat = new(3, 1);
            StartingMat.SetColumn(0, trans2);

            DenseMatrix GlobalCoords = (RotationMatrixFromPRY(rotation) * LocalCoords) + TranslationMat + StartingMat;

            return [GlobalCoords[0, 0], GlobalCoords[1, 0], GlobalCoords[2, 0]];
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
        public static double[] CalcGlobalCoord2(double[] localCoord, double[] trans1, double[] trans2, double[] rotation, double[]? relativeRotCenter = null)
        {
            double[] coords = [0, 0, 0];

            if (relativeRotCenter != null)
                coords = relativeRotCenter;

            DenseMatrix RotCenter = new(3, 1);
            RotCenter.SetColumn(0, coords);

            DenseMatrix LocalCoords = new(3, 1);
            LocalCoords.SetColumn(0, localCoord);

            DenseMatrix TranslationMat = new(3, 1);
            TranslationMat.SetColumn(0, trans1);

            DenseMatrix TranslationMat2 = new(3, 1);
            TranslationMat2.SetColumn(0, trans2);


            DenseMatrix GlobalCoords = (RotationMatrixFromPRY(rotation) * (LocalCoords - RotCenter + TranslationMat)) + TranslationMat2 + RotCenter; //Rotcenter needs to be added before rotation, then removed after

            return [GlobalCoords[0, 0], GlobalCoords[1, 0], GlobalCoords[2, 0]];
        }
    }
}
