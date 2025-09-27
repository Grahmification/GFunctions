namespace GFunctions.Mathematics
{
    /// <summary>
    /// Uses gradient descent to find an optimal value that minimizes error of a function with 1 input and 1 output value
    /// </summary>
    public class IterativeSolver
    {
        /// <summary>
        /// Template for the function used to calculate the error
        /// </summary>
        /// <param name="iterationValue">Input value being optimized</param>
        /// <returns>The error</returns>
        public delegate double ErrorFunction1D(double iterationValue);

        // ----------------------- Private Fields ---------------------------

        /// <summary>
        /// Function which calculates the error
        /// </summary>
        private readonly ErrorFunction1D _errorFunction;

        /// <summary>
        /// Keeps track of solution values as the solver runs
        /// </summary>
        private readonly List<double> _iterationValues = [];

        /// <summary>
        /// Keeps track of solution values as the solver runs
        /// </summary>
        private readonly List<double> _errorValues = [];

        // ----------------------- Public Properties ---------------------------

        /// <summary>
        /// The max allowable iteration steps before solution fails
        /// </summary>
        public double MaxSteps { get; set; } = 100;

        /// <summary>
        /// Max allowable output value before the solution fails
        /// </summary>
        public double MaxSolutionValue { get; set; } = double.MaxValue;

        /// <summary>
        /// Min allowable output value before the solution fails
        /// </summary>
        public double MinSolutionValue { get; set; } = double.MinValue;

        /// <summary>
        /// Whether max/min solution limits are enabled
        /// </summary>
        public bool SolutionLimitsEnabled => !double.IsInfinity(MaxSolutionValue - MinSolutionValue);

        /// <summary>
        /// The initial step size to test when solving
        /// </summary>
        public double InitialStepSize { get; set; } = 0.01;

        /// <summary>
        /// A solution is considered valid if the absolute error is less than this value
        /// </summary>
        public double SuccessErrorThreshold { get; set; } = 0.001;

        /// <summary>
        /// Whether a valid solution was found
        /// </summary>
        public bool SolutionValid { get; private set; } = false;

        /// <summary>
        /// True if the solver is actively running a solution
        /// </summary>
        public bool Solving { get; private set; } = false;

        /// <summary>
        /// The list of iterated input values from earliest to latest as the solver runs
        /// </summary>
        public List<double> IterationValues => _iterationValues;

        /// <summary>
        /// The list of iterated error values from earliest to latest as the solver runs
        /// </summary>
        public List<double> ErrorValues => _errorValues;

        // ----------------------- Public Methods ---------------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="errorFunc">Function which will calculate the error</param>
        public IterativeSolver(ErrorFunction1D errorFunc)
        {
            _errorFunction = errorFunc;
        }

        /// <summary>
        /// Find the value that results in the minimum error
        /// </summary>
        /// <param name="startingValue">The value to start iterating at</param>
        /// <returns>The optimal value, or original if a valid solution wasn't found</returns>
        public double Solve(double startingValue)
        {
            SolutionValid = false;
            Solving = true;
            _iterationValues.Clear();
            _errorValues.Clear();

            double scalingStepSize = InitialStepSize; //default starting step size
            double newError = 0;
            double ratio = 0;

            double iterationValue = startingValue;
            double currentError = _errorFunction(iterationValue); // Calculate starting error

            for (int i = 0; i < MaxSteps; i++)
            {
                // Log the iteration and error values
                _iterationValues.Add(iterationValue);
                _errorValues.Add(currentError);

                // Soln has failed if out of allowable range, exit loop and go to last pos
                if (iterationValue > MaxSolutionValue || iterationValue < MinSolutionValue)
                    break;

                // Solution is within error tolerance (valid)
                if (Math.Abs(currentError) <= SuccessErrorThreshold) 
                {
                    SolutionValid = true;
                    Solving = false;
                    return iterationValue;
                }

                iterationValue += scalingStepSize; // Change value by one step size

                newError = _errorFunction(iterationValue); // Calculate the error
                ratio = (currentError - newError) / scalingStepSize; // Ratio between change in k and error
                scalingStepSize = newError / (ratio * 2.0);
                currentError = newError;

                // If the step size is approaching the limit range, divide it to make sure we don't exceed too easily
                if (SolutionLimitsEnabled)
                {
                    while (Math.Abs(scalingStepSize) > (MaxSolutionValue - MinSolutionValue) / 2.0)
                    {
                        scalingStepSize /= 2.0; // Helps to prevent value from running away by growing too large
                    }
                }
            }

            // Solution has failed. Reset to starting value.
            Solving = false;
            return startingValue;
        }
    }
}
