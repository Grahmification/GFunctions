namespace GFunctions.Mathematics
{
    public class Calculus
    {
        public static double Integrate(double X0, double X1, double timeStep)
        {
            return ((X0 + X1) / 2.0 + Math.Min(Math.Abs(X0), Math.Abs(X1))) * timeStep; //triangular region + square region
        }
        public static double Integrate(double X0, double timeStep)
        {
            return X0 * timeStep; //square region
        }

        public static double Derivative(double X0, double X1, double timeStep)
        {
            return (X1 - X0) / timeStep;
        }
    }
}
