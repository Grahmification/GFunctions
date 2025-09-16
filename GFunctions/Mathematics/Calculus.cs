namespace GFunctions.Mathematics
{
    /// <summary>
    /// Various calculus functions
    /// </summary>
    public class Calculus
    {
        /// <summary>
        /// Integrate the given area with a trapezoidal zone
        /// </summary>
        /// <param name="X0">Value at T=0</param>
        /// <param name="X1">Value at T=timeStep</param>
        /// <param name="timeStep">Delta t</param>
        /// <returns>The area inside the zone</returns>
        public static double Integrate(double X0, double X1, double timeStep)
        {
            return ((X0 + X1) / 2.0 + Math.Min(Math.Abs(X0), Math.Abs(X1))) * timeStep; //triangular region + square region
        }

        /// <summary>
        /// Integrates the given area with a rectangular zone
        /// </summary>
        /// <param name="X0">Value at T=0</param>
        /// <param name="timeStep">Delta t</param>
        /// <returns>The area inside the zone</returns>
        public static double Integrate(double X0, double timeStep)
        {
            return X0 * timeStep; //square region
        }

        /// <summary>
        /// Takes the numerical derivative
        /// </summary>
        /// <param name="X0">Value at T=0</param>
        /// <param name="X1">Value at T=timeStep</param>
        /// <param name="timeStep">Delta t</param>
        /// <returns>The first order derivative</returns>
        public static double Derivative(double X0, double X1, double timeStep)
        {
            return (X1 - X0) / timeStep;
        }
    }
}
