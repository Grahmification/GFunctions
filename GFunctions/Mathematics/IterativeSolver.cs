namespace GFunctions.Mathematics
{
    /// <summary>
    /// Uses gradient descent to find an optimal value that minimizes error a function
    /// </summary>
    public class IterativeSolver
    {
        /// <summary>
        /// Template for the function used to calculate the error
        /// </summary>
        /// <param name="param1">Generic input parameter</param>
        /// <returns>The error</returns>
        public delegate double DoubleFunction(double param1);

        private double _stepSize = 0.01; //initial change step size to test
        private double _maxSteps = 100; //max allowable steps before solution fails
        private readonly double _errorTolerance = 0.001; //error tolerance for successful solution
        private DoubleFunction _errorFunction; //function which will calculate the error

        private double _maxValue; //max allowable value for solution
        private double _minValue; //min allowable value for solution

        /// <summary>
        /// Whether a valid solution was found
        /// </summary>
        public bool SolutionValid { get; private set; } = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="stepSize">The step at each iteration</param>
        /// <param name="maxSteps">The max steps before a solution fails</param>
        /// <param name="errorTolerance">The error tolerance for successful solution</param>
        /// <param name="errorFunc">Function which will calculate the error</param>
        /// <param name="maxValue">Solution will fail if this value is exceeded</param>
        /// <param name="minValue">Solution will fail if this value is exceeded</param>
        public IterativeSolver(double stepSize, double maxSteps, double errorTolerance, DoubleFunction errorFunc, double maxValue, double minValue)
        {
            _stepSize = stepSize;
            _maxSteps = maxSteps;
            _errorTolerance = errorTolerance;
            _errorFunction = errorFunc;
            _maxValue = maxValue;
            _minValue = minValue;
        }

        /// <summary>
        /// Find the value that results in the minimum error
        /// </summary>
        /// <param name="startingValue">The value to start iterating at</param>
        /// <returns>The optimal value, or original if a valid solution wasn't found</returns>
        public double Solve(double startingValue)
        {
            SolutionValid = false;

            double ScalingStepSize = _stepSize; //default starting step size
            double newError = 0;
            double ratio = 0;

            double iterationValue = startingValue;
            double prevError = _errorFunction(iterationValue); //calculate the error

            for (int i = 0; i < _maxSteps; i++)
            {
                if (Math.Abs(prevError) <= _errorTolerance) //Solution is with error tolerance (valid)
                {
                    if (iterationValue > _maxValue || iterationValue < _minValue)
                        break; //soln has failed if out of allowable range, exit loop and go to last pos

                    SolutionValid = true;
                    return iterationValue;
                }

                iterationValue += ScalingStepSize; //change value by one step size

                newError = _errorFunction(iterationValue); //calculate the error
                ratio = (prevError - newError) / ScalingStepSize; //ratio between change in k and error
                ScalingStepSize = newError / (ratio * 2.0);
                prevError = newError;

                while (Math.Abs(ScalingStepSize) > (_maxValue - _minValue) / 2.0)
                {
                    ScalingStepSize /= 2.0; //helps to prevent value from running away by growing too large
                }
            }

            return startingValue; //solution has failed. Reset to starting. 
        }
    }
}
