using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace GFunctions.Mathnet
{
    public class KinematicMath
    {
        public static DenseMatrix RotationMatrixFromPRY(double[] Rot)
        {
            double Pitch = Rot[0] * Math.PI / 180.0;
            double Roll = Rot[1] * Math.PI / 180.0;
            double Yaw = Rot[2] * Math.PI / 180.0;

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
        public static double VectorLength(double[] pos1, double[] pos2)
        {
            double output = 0;

            for (int i = 0; i < pos1.Length; i++)
            {
                output += (pos2[i] - pos1[i]) * (pos2[i] - pos1[i]);
            }

            return Math.Sqrt(output);
        }
        public static double[] RotateVector(double[] vector, double[] rotationPRY)
        {
            DenseVector vectorObj = new DenseVector(vector); //local vector without rotation;
            DenseMatrix rotation = RotationMatrixFromPRY(rotationPRY);

            DenseVector rotatedVector = rotation * vectorObj; //apply rotation

            return rotatedVector.ToArray();
        }


        public static double[] CalcGlobalCoord(double[] LocalCoord, double[] Trans1, double[] Trans2, double[] Rot)
        {
            DenseMatrix LocalCoords = new DenseMatrix(3, 1);
            LocalCoords.SetColumn(0, LocalCoord);

            DenseMatrix TranslationMat = new DenseMatrix(3, 1);
            TranslationMat.SetColumn(0, Trans1);

            DenseMatrix StartingMat = new DenseMatrix(3, 1);
            StartingMat.SetColumn(0, Trans2);

            DenseMatrix GlobalCoords = (KinematicMath.RotationMatrixFromPRY(Rot) * LocalCoords) + TranslationMat + StartingMat;

            return new double[] { GlobalCoords[0, 0], GlobalCoords[1, 0], GlobalCoords[2, 0] };
        }
        public static double[] CalcGlobalCoord2(double[] LocalCoord, double[] Trans1, double[] Trans2, double[] Rot, double[] RelativeRotCenter = null)
        {
            double[] coords = new double[] { 0, 0, 0 };

            if (RelativeRotCenter != null)
                coords = RelativeRotCenter;

            DenseMatrix RotCenter = new DenseMatrix(3, 1);
            RotCenter.SetColumn(0, coords);

            DenseMatrix LocalCoords = new DenseMatrix(3, 1);
            LocalCoords.SetColumn(0, LocalCoord);

            DenseMatrix TranslationMat = new DenseMatrix(3, 1);
            TranslationMat.SetColumn(0, Trans1);

            DenseMatrix TranslationMat2 = new DenseMatrix(3, 1);
            TranslationMat2.SetColumn(0, Trans2);


            DenseMatrix GlobalCoords = (KinematicMath.RotationMatrixFromPRY(Rot) * (LocalCoords - RotCenter + TranslationMat)) + TranslationMat2 + RotCenter; //Rotcenter needs to be added before rotation, then removed after

            return new double[] { GlobalCoords[0, 0], GlobalCoords[1, 0], GlobalCoords[2, 0] };
        }
    }
}
