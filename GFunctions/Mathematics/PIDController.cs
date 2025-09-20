namespace GFunctions.Mathematics
{
    /// <summary>
    /// Model of a PID controller
    /// </summary>
    public class PIDController
    {
        private bool _firstCycle = true;

        private double _prevFeedbackValue = 0;
        private double _previousError = 0;

        private double _target = 0;

        private double _integralSum = 0; //may consider adding limit executed in private set property

        /// <summary>
        /// Global controller gain
        /// </summary>
        public double Gain { get; set; } = 1;

        /// <summary>
        /// Proportional gain
        /// </summary>
        public double P { get; set; } = 1;

        /// <summary>
        /// Integral gain
        /// </summary>
        public double I { get; set; } = 1;

        /// <summary>
        /// Derivative gain
        /// </summary>
        public double D { get; set; } = 1;

        /// <summary>
        /// Limits max/min output value of controller, 0 = no limit
        /// </summary>
        public double SatLimit { get; set; } = 0;

        /// <summary>
        /// If True, d is calculated from feedback change, not error change (isolating it from setpoint changes)
        /// </summary>
        public bool TargetIsolatedDerivative { get; set; } = true;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="p">Proportional gain</param>
        /// <param name="i">Integral gain</param>
        /// <param name="d">Derivative gain</param>
        /// <param name="saturationLimit">Limits max/min output value of controller, 0 = no limit</param>
        public PIDController(double p, double i, double d, double saturationLimit = 0)
        {
            P = p;
            I = i;
            D = d;
            SatLimit = saturationLimit;
        }
        
        /// <summary>
        /// Sets the target input for the controller
        /// </summary>
        /// <param name="target">The value to target</param>
        public void SetTarget(double target)
        {
            _target = target;
        }
        
        /// <summary>
        /// Computes one cycle of the controller, calculating the output value
        /// </summary>
        /// <param name="feedbackValue">The feedback value from the system, used to calculate error</param>
        /// <param name="timeStep">The delta t value</param>
        /// <returns>The controller output</returns>
        public double CalculateOutput(double feedbackValue, double timeStep)
        {
            var error = _target - feedbackValue; // Calculate error

            // No previous error data saved
            if (_firstCycle)
            {
                _previousError = error;
                _prevFeedbackValue = feedbackValue;
                _firstCycle = false;
            }

            _integralSum += Calculus.Integrate(error, timeStep);

            double output = 0;
            output += P * error; // Proportional term
            output += I * _integralSum; // Integral term

            if (TargetIsolatedDerivative)
            {
                output += D * Calculus.Derivative(_prevFeedbackValue, feedbackValue, timeStep); // Derivative from process variable only
            }
            else
            {
                output += D * Calculus.Derivative(_previousError, error, timeStep); // Derivative from error
            }

            output *= Gain; // Apply the gain
            output = CheckSatuationLimit(output, SatLimit); // Check if the output is inside the saturation limit

            // Save current values for next cycle
            _previousError = error;
            _prevFeedbackValue = feedbackValue;

            return output;
        }

        private static double CheckSatuationLimit(double output, double satLimit)
        {
            if (satLimit == 0) // 0 = no saturation limit
                return output;

            if (output > satLimit)
                return satLimit;

            if (output < satLimit * -1)
                return satLimit * -1;

            return output; // Output is inside limit
        }
    }
}
