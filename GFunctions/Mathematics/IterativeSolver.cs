using System;

namespace GFunctions.Mathematics
{
    public class IterativeSolver
    {
        public delegate double DblFunction(double param1);

        private bool _solutionValid = false;
        private double _stepSize = 0.01; //initial change step size to test
        private double _maxSteps = 100; //max allowable steps before solution fails
        private double _errorTolerance = 0.001; //error tolerance for successful solution
        private DblFunction _errorFunc; //function which will calculate the error

        private double _maxValue; //max allowable value for solution
        private double _minValue; //min allowable value for solution

        public bool SolutionValid
        {
            get
            {
                return this._solutionValid;
            }
        }

        public IterativeSolver(double stepSize, double maxSteps, double errorTolerance, DblFunction errorFunc, double maxValue, double minValue)
        {
            this._stepSize = stepSize;
            this._maxSteps = maxSteps;
            this._errorTolerance = errorTolerance;
            this._errorFunc = errorFunc;
            this._maxValue = maxValue;
            this._minValue = minValue;

        }

        public double Solve(double previousSolution)
        {
            this._solutionValid = false;

            double ScalingStepSize = this._stepSize; //default starting step size
            double newError = 0;
            double ratio = 0;

            double iterationValue = previousSolution;
            double prevError = this._errorFunc(iterationValue); //calculate the error

            for (int i = 0; i < this._maxSteps; i++)
            {
                if (Math.Abs(prevError) <= this._errorTolerance) //Solution is with error tolerance (valid)
                {
                    if (iterationValue > this._maxValue || iterationValue < this._minValue)
                        break; //soln has failed if out of allowable range, exit loop and go to last pos

                    this._solutionValid = true;
                    return iterationValue;
                }

                iterationValue += ScalingStepSize; //change value by one step size

                newError = this._errorFunc(iterationValue); //calculate the error
                ratio = (prevError - newError) / ScalingStepSize; //ratio between change in k and error
                ScalingStepSize = newError / (ratio * 2.0);
                prevError = newError;

                while (Math.Abs(ScalingStepSize) > (this._maxValue - this._minValue) / 2.0)
                {
                    ScalingStepSize /= 2.0; //helps to prevent value from running away by growing too large
                }
            }

            return previousSolution; //solution has failed. Reset to starting. 
        }

    }
}
