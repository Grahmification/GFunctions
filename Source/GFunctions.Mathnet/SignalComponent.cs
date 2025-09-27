namespace GFunctions.Mathnet
{
    /// <summary>
    /// Represents an individual sinusoidal signal from the decomposed fft
    /// </summary>
    public class SignalComponent
    {
        /// <summary>
        /// Whether this component is the DC offset
        /// </summary>
        public bool DCOffset { get; private set; } = false;

        /// <summary>
        /// The index of this component in the fft
        /// </summary>
        public int Index { get; private set; } = 0;

        /// <summary>
        /// The real portion of the signal
        /// </summary>
        public double RealComponent { get; private set; } = 0;

        /// <summary>
        /// The imaginary portion of the signal
        /// </summary>
        public double ImaginaryComponent { get; private set; } = 0;

        /// <summary>
        /// The signal's frequency
        /// </summary>
        public double Frequency { get; private set; } = 0;

        /// <summary>
        /// The signal's period
        /// </summary>
        public double Period => Frequency == 0 ? 0 : 1.0 / Frequency;

        /// <summary>
        /// What fraction of the total fft signal this component contributes to
        /// </summary>
        public double ContributionFraction { get; private set; } = 0;

        /// <summary>
        /// The signal's magnitude
        /// </summary>
        public double Magnitude { get; private set; } = 0;

        /// <summary>
        /// The signal's magnitude in decibels
        /// </summary>
        public double MagnitudeDb => 20 * Math.Log10(Magnitude);

        /// <summary>
        /// The signal's phase in radians
        /// </summary>
        public double Phase { get; private set; } = 0;

        /// <summary>
        /// Construct from FFT outputs
        /// </summary>
        /// <param name="freqency">The frequency in hz</param>
        /// <param name="real">FFT real component</param>
        /// <param name="imaginary">FFT imaginary component</param>
        /// <param name="index">Index in the FFT</param>
        /// <param name="datasetSize">Number of datapoints used to compute the FFT</param>
        public SignalComponent(double freqency, double real, double imaginary, int index, int datasetSize)
        {
            Frequency = freqency;
            RealComponent = real;
            ImaginaryComponent = imaginary;
            Index = index;

            // If the imaginary component is 0 and frequency is 0 this signal is the DC offset
            if (imaginary == 0 && freqency == 0)
            {
                DCOffset = true;
                Frequency = 0;
                Magnitude = real / datasetSize;
                Phase = 0;
            }
            else
            {
                DCOffset = false;
                Magnitude = Math.Sqrt((real * real) + (imaginary * imaginary)) * 2 / datasetSize; // Doubled to account for combined positive and negative frequencies
                Phase = Math.Atan2(imaginary, real);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SignalComponent() { }

        /// <summary>
        /// Gets the sinusoidal value of the signal
        /// </summary>
        /// <param name="xValue">The x axis time value</param>
        /// <returns>The sinusoidal Y value of the signal</returns>
        public double GetYValue(double xValue)
        {
            if (DCOffset)
            {
                return Magnitude;
            }
            else
            {
                return Magnitude * Math.Cos(2 * Math.PI * Frequency * xValue + Phase);
            }

        }

        /// <summary>
        /// Populates the <see cref="ContributionFraction"/> property for a list of SignalComponents
        /// </summary>
        /// <param name="components">The list of SignalComponents to populate</param>
        public static void SetContributionFractions(SignalComponent[] components)
        {
            var magnitudeSum = components.Sum(s => s.Magnitude);

            for (int i = 0; i < components.Length; i++)
            {
                components[i].SetContributionFraction(magnitudeSum);
            }
        }

        /// <summary>
        /// Unwraps all phases for a list of signal components so there are no jumps in phase due to arctan limitations
        /// </summary>
        /// <param name="components">The list of SignalComponents to populate</param>
        public static void UnwrapPhases(SignalComponent[] components)
        {
            for (int i = 1; i < components.Length; i++)
            {
                components[i].UnwrapPhase(components[i - 1].Phase);
            }
        }


        private void SetContributionFraction(double totalMagnitude)
        {
            ContributionFraction = Magnitude / totalMagnitude;
        }

        private void UnwrapPhase(double previousPhase)
        {
            // Normalize the angle to be between 0 and 2*PI
            Phase = Math.IEEERemainder(Phase, Math.PI * 2);

            // Calculate the difference between the angles
            double difference = Phase - previousPhase;

            // Adjust the difference to be within -PI to PI range
            difference = Math.IEEERemainder(difference, Math.PI * 2);

            // Add the adjusted difference back to the reference angle
            Phase = previousPhase + difference;
        }
    }
}
