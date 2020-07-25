using System;

namespace GFunctions.Mathematics
{
    public class TrapezoidalTrajectory
    {
        public static double AccelDistance(double moveDist, double maxSpeed, double accel)
        {
            moveDist = Math.Abs(moveDist);
            maxSpeed = Math.Abs(maxSpeed);
            accel = Math.Abs(accel);

            var accelDist = maxSpeed * maxSpeed / (2.0 * accel);

            if (accelDist > moveDist / 2.0)
                accelDist = moveDist / 2.0;

            return accelDist;
        }
        public static double AccelTime(double moveDist, double maxSpeed, double accel)
        {
            accel = Math.Abs(accel);
            return Math.Sqrt(2 * AccelDistance(moveDist, maxSpeed, accel) / accel);
        }
        public static double MaxSpeedDistance(double moveDist, double maxSpeed, double accel, double decel)
        {
            moveDist = Math.Abs(moveDist);
            var dist = moveDist - AccelDistance(moveDist, maxSpeed, accel) - AccelDistance(moveDist, maxSpeed, decel);

            if (dist < 0)
                dist = 0;

            return dist;
        }
        public static double MaxSpeedTime(double moveDist, double maxSpeed, double accel, double decel)
        {
            maxSpeed = Math.Abs(maxSpeed);
            return MaxSpeedDistance(moveDist, maxSpeed, accel, decel) / maxSpeed;
        }
        public static double TrapezoidalProfileTime(double moveDist, double maxSpeed, double accel, double decel)
        {
            return AccelTime(moveDist, maxSpeed, accel) + MaxSpeedTime(moveDist, maxSpeed, accel, decel) + AccelTime(moveDist, maxSpeed, decel);
        }
    }

}
