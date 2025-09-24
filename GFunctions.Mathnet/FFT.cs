using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace GFunctions.Mathnet
{
    /// <summary>
    /// Class for working with fast fourier transforms
    /// </summary>
    public class FFT
    {
        /// <summary>
        /// Calculates the average sampling frequency based on start/end times
        /// </summary>
        /// <param name="timeData">The time data, from earliest to latest</param>
        /// <returns>The average sampling frequency</returns>
        public static double SamplingFrequencyAverage(double[] timeData)
        {
            if (timeData.Length < 2)
                return 0;

            return timeData.Length / (timeData[^1] - timeData[0]);
        }

        /// <summary>
        /// Calculates the max frequency the FFT can compute
        /// </summary>
        /// <param name="samplingRate">The data sampling rate [Hz]</param>
        /// <returns></returns>
        public static double MaxFrequency(double samplingRate)
        {
            return samplingRate / 2.0;
        }

        /// <summary>
        /// Calculates the min frequency the FFT can compute
        /// </summary>
        /// <param name="samplingRate">The data sampling rate [Hz]</param>
        /// <param name="length">The dataset length</param>
        /// <returns></returns>
        public static double MinFrequency(double samplingRate, int length)
        {
            return samplingRate/ length;
        }

        /// <summary>
        /// Calculate the frequencies in an FFT dataset
        /// </summary>
        /// <param name="n">The number of samples</param>
        /// <param name="samplingRate">The sampling frequency in HZ</param>
        /// <returns>The corresponding FFT frequencies for the dataset</returns>
        public static double[] CalculateFrequencies(int n, double samplingRate)
        {
            int numFreqs = (n / 2) - 1; // Number of unique frequencies the fft can compute
            double[] freq = new double[numFreqs];

            for (int i = 0; i < numFreqs; i++)
                freq[i] = 1.0 * i * samplingRate / n; // Compute each frequency

            return freq;
        }

        /// <summary>
        /// Calculate the Power Spectral Density (PSD)
        /// </summary>
        /// <param name="frequencies">The FFT frequencies [Hz]</param>
        /// <param name="FFTData">The raw FFT output data</param>
        /// <param name="frequencyCutoff">Optional frequency limit to remove values below</param>
        /// <returns>The power spectral density [Frequencies, Magnitudes]</returns>
        public static double[,] CalculatePSD(double[] frequencies, Complex32[] FFTData, double? frequencyCutoff = null)
        {
            int nFreqs = frequencies.Length;

            // Trim nFreqs based on the cutoff
            for (int i = 0; i < nFreqs; i++)
            {
                if (frequencyCutoff != null && frequencies[i] > frequencyCutoff)
                {
                    nFreqs = i + 1;
                    break;
                }
            }

            var output = new double[2, nFreqs];

            for (int i = 0; i < nFreqs; i++)
            {
                output[0, i] = frequencies[i]; // Frequencies
                output[1, i] = FFTData[i].Real * FFTData[i].Real + FFTData[i].Imaginary * FFTData[i].Imaginary; // PSD values
            }

            return output;
        }

        /// <summary>
        /// Computes the FFT and calculates the power spectral 
        /// </summary>
        /// <param name="inputData">The data [times, values]</param>
        /// <returns>The complex FFT</returns>
        public static Complex32[] ComputeFFT(double[] inputData)
        {
            int n = inputData.GetLength(0);

            Complex32[] complexArray = new Complex32[n];

            for (int i = 0; i < n; i++)
                complexArray[i] = new Complex32((float)inputData[i], 0); // Fill array with data in real components

            Fourier.Forward(complexArray);

            return complexArray;
        }

        /// <summary>
        /// Performs the FFT, calculating all signalcomponents of the data
        /// </summary>
        /// <param name="inputData">The data to perform the fft on</param>
        /// <param name="samplingRate">The data sampling rate [Hz]</param>
        /// <returns>All signal components for the FFT of the dataset</returns>
        /// <exception cref="ArgumentException">The data was an invalid length</exception>
        public static SignalComponent[] ComputeComponents(double[] inputData, double samplingRate)
        {
            int length = inputData.Length;

            if (!IsPowerOfTwo(length))
                throw new ArgumentException($"Input dataset with {length} entries not a valid size for computing an FFT.");

            // ---------------------------- Calculate FFT ----------------------------------

            Complex32[] spectrum = ComputeFFT(inputData);
            double[] frequencies = CalculateFrequencies(length, samplingRate);

            // ------------------------ Prepare output data -------------------------------------
            var output = new SignalComponent[spectrum.Length];

            for (int i = 0; i < spectrum.Length; i++)
                output[i] = new SignalComponent(frequencies[i], spectrum[i].Real, spectrum[i].Imaginary, i, length);

            SignalComponent.SetContributionFractions(output);
            SignalComponent.UnwrapPhases(output);

            return output;
        }

        /// <summary>
        /// Returns whether the number is a power of two
        /// </summary>
        /// <param name="x">The number</param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int x)
        {
            return (x > 0) && ((x & (x - 1)) == 0);
        }

        /// <summary>
        /// Gets all values that are powers of two between a maximum and minimum
        /// </summary>
        /// <param name="max">The maximum value (inclusive).</param>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <returns></returns>
        public static List<int> PowersOfTwo(int max, int min = 2)
        {
            // Do some validation
            if (min < 2) { min = 2; }
            if (max < 2) { max = 2; }
            if (max < min) { min = max; }

            List<int> output = [];
            int value = 2;

            while (value <= max)
            {
                if (value >= min)
                {
                    output.Add(value);
                }
                value *= 2;
            }

            return output;
        }

        /// <summary>
        /// Generates a random signal for testing based on a sine wave and noise
        /// </summary>
        /// <param name="points">The number of points to generate</param>
        /// <returns>A list of values to input into an FFT</returns>
        public static double[] GenerateTestSignal(int points)
        {
            double[] output = new double[points];

            Random rnd = new();

            double scale = rnd.Next(1, 20);
            double period = rnd.Next(1, 20);

            double scale2 = rnd.Next(1, 20);
            double period2 = rnd.Next(1, 20);

            for (int i = 0; i < points; i++)
            {
                output[i] = scale * Math.Sin(period * i / 100) + scale2 * Math.Cos(period2 * i / 100) + i * i / (points * points);

                // Vary this to add noise
                scale2 = rnd.Next(1, 20);
                period2 = rnd.Next(1, 20);
            }

            return output;
        }
    }
}
