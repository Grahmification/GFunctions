namespace GFunctions.Mathematics
{
    public class PIDController
    {
        private bool _firstCycle = true;

        private double _prevProcessVar = 0;
        private double _previousError = 0;

        private double _target = 0;

        private double _integralSum = 0; //may consider adding limit executed in private set property

        public double Gain { get; set; }
        public double P { get; set; }
        public double I { get; set; }
        public double D { get; set; }
        public double SatLimit { get; set; }  //limits max/min output value of controller, 0 = no limit
        public bool TargetIsolatedDeriv { get; set; } //if True, d is calculated from output change, not error change (isolating it from setpoint changes)

        public PIDController(double gain, double p, double i, double d, double satLimit = 0)
        {
            this.Gain = gain;
            this.P = p;
            this.I = i;
            this.D = d;
            this.SatLimit = satLimit;
            this.TargetIsolatedDeriv = true;
        }
        public void SetTarget(double Target)
        {
            _target = Target;
        }
        public double CalcOutput(double ProcessVar, double TimeStep)
        {
            var error = _target - ProcessVar; //calculate error

            if (_firstCycle)
            {
                this._previousError = error; //no previous error data saved
                this._prevProcessVar = ProcessVar;
            }


            this._integralSum += Calculus.Integrate(error, TimeStep);

            double output = 0;
            output += this.P * error; //proportional term
            output += this.I * this._integralSum; //integral term 

            if (this.TargetIsolatedDeriv)
            {
                output += this.D * Calculus.Derivative(this._prevProcessVar, ProcessVar, TimeStep); //derivative from process variable
            }
            else
            {
                output += this.D * Calculus.Derivative(this._previousError, error, TimeStep); //derivative from process variable
            }


            output *= this.Gain; //apply the gain
            output = CheckSatLimit(output, this.SatLimit); //check if the output is inside the saturation limit

            this._previousError = error; //save current value for next cycle
            this._prevProcessVar = ProcessVar;
            this._firstCycle = false;

            return output;
        }

        private double CheckSatLimit(double output, double satLimit)
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
