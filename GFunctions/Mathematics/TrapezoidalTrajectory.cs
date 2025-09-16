namespace GFunctions.Mathematics
{
    /// <summary>
    /// Calculations for a trapezoidal velocity motion trajectory
    /// </summary>
    public class TrapezoidalTrajectory
    {
        /// <summary>
        /// Calculate the distance required to accelerate
        /// </summary>
        /// <param name="moveDist">Total trajectory distance</param>
        /// <param name="maxSpeed">Maximum trajectory speed</param>
        /// <param name="accel">Trajectory acceleration</param>
        /// <returns>The distance for acceleration to occur</returns>
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

        /// <summary>
        /// Calculate the time required to accelerate
        /// </summary>
        /// <param name="moveDist">Total trajectory distance</param>
        /// <param name="maxSpeed">Maximum trajectory speed</param>
        /// <param name="accel">Trajectory acceleration</param>
        /// <returns>The time for acceleration to occur</returns>
        public static double AccelTime(double moveDist, double maxSpeed, double accel)
        {
            accel = Math.Abs(accel);
            return Math.Sqrt(2 * AccelDistance(moveDist, maxSpeed, accel) / accel);
        }

        /// <summary>
        /// Calculate the distance at maximum trajectory speed
        /// </summary>
        /// <param name="moveDist">Total trajectory distance</param>
        /// <param name="maxSpeed">Maximum trajectory speed</param>
        /// <param name="accel">Trajectory acceleration</param>
        /// <param name="decel">Trajectory deceleration</param>
        /// <returns>The distance spent at maximum speed</returns>
        public static double MaxSpeedDistance(double moveDist, double maxSpeed, double accel, double decel)
        {
            moveDist = Math.Abs(moveDist);
            var dist = moveDist - AccelDistance(moveDist, maxSpeed, accel) - AccelDistance(moveDist, maxSpeed, decel);

            if (dist < 0)
                dist = 0;

            return dist;
        }

        /// <summary>
        /// Calculate the time at maximum trajectory speed
        /// </summary>
        /// <param name="moveDist">Total trajectory distance</param>
        /// <param name="maxSpeed">Maximum trajectory speed</param>
        /// <param name="accel">Trajectory acceleration</param>
        /// <param name="decel">Trajectory deceleration</param>
        /// <returns>The time spent at maximum speed</returns>
        public static double MaxSpeedTime(double moveDist, double maxSpeed, double accel, double decel)
        {
            maxSpeed = Math.Abs(maxSpeed);
            return MaxSpeedDistance(moveDist, maxSpeed, accel, decel) / maxSpeed;
        }

        /// <summary>
        /// Calculate total time for a trapezoidal trajectory
        /// </summary>
        /// <param name="moveDist">Total trajectory distance</param>
        /// <param name="maxSpeed">Maximum trajectory speed</param>
        /// <param name="accel">Trajectory acceleration</param>
        /// <param name="decel">Trajectory deceleration</param>
        /// <returns>The total trajectory time</returns>
        public static double TrapezoidalProfileTime(double moveDist, double maxSpeed, double accel, double decel)
        {
            return AccelTime(moveDist, maxSpeed, accel) + MaxSpeedTime(moveDist, maxSpeed, accel, decel) + AccelTime(moveDist, maxSpeed, decel);
        }
    }

}
