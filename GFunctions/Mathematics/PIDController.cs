namespace GFunctions.Mathematics
{
    /// <summary>
    /// Model of a PID controller
    /// </summary>
    public class PIDController
    {
        private bool _firstCycle = true;

        private double _prevProcessVar = 0;
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
        /// If True, d is calculated from output change, not error change (isolating it from setpoint changes)
        /// </summary>
        public bool TargetIsolatedDerivative { get; set; } = true;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="gain">Global controller gain</param>
        /// <param name="p">Proportional gain</param>
        /// <param name="i">Integral gain</param>
        /// <param name="d">Derivative gain</param>
        /// <param name="saturationLimit">Limits max/min output value of controller, 0 = no limit</param>
        public PIDController(double gain, double p, double i, double d, double saturationLimit = 0)
        {
            Gain = gain;
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
        /// <param name="processVar">The feedback value from the system, used to calculate error</param>
        /// <param name="timeStep">The delta t value</param>
        /// <returns>The controller output</returns>
        public double CalculateOutput(double processVar, double timeStep)
        {
            var error = _target - processVar; //calculate error

            if (_firstCycle)
            {
                _previousError = error; //no previous error data saved
                _prevProcessVar = processVar;
            }

            _integralSum += Calculus.Integrate(error, timeStep);

            double output = 0;
            output += P * error; //proportional term
            output += I * _integralSum; //integral term 

            if (TargetIsolatedDerivative)
            {
                output += D * Calculus.Derivative(_prevProcessVar, processVar, timeStep); //derivative from process variable
            }
            else
            {
                output += D * Calculus.Derivative(_previousError, error, timeStep); //derivative from process variable
            }


            output *= Gain; //apply the gain
            output = CheckSatuationLimit(output, SatLimit); //check if the output is inside the saturation limit

            _previousError = error; //save current value for next cycle
            _prevProcessVar = processVar;
            _firstCycle = false;

            return output;
        }

        private static double CheckSatuationLimit(double output, double satLimit)
        {
            if (satLimit == 0) //0 = no saturation limit
                return output;

            if (output > satLimit)
                return satLimit;

            if (output < satLimit * -1)
                return satLimit * -1;

            return output; //output is inside limit
        }
    }
}
