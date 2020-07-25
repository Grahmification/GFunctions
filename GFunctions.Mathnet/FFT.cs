using System;
using System.Threading;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using GFunctions.Timing;

namespace GFunctions.Mathnet
{
    class FFT
    {
        static double[] CalculateFrequencies(int n, double samplingHZ)
        {
            int numFreqs = (n / 2) - 1; //number of unique frequencies the fft can compute
            double[] Freqs = new double[numFreqs];

            for (int i = 0; i < numFreqs; i++)
                Freqs[i] = 1.0 * i * samplingHZ / n; //compute each frequency

            return Freqs;
        }
        static double[,] CalculatePSD(double[] Freqs, Complex32[] FFTData, double freqCutOff = -1)
        {
            int nFreqs = Freqs.Length;

            for (int i = 0; i < nFreqs; i++)
            {
                if (freqCutOff != -1 && Freqs[i] > freqCutOff)
                {
                    nFreqs = i + 1;
                    break;
                }
            }

            

            var output = new double[2, nFreqs];


            for (int i = 0; i < nFreqs; i++)
            {
                output[0, i] = Freqs[i]; //frequencies
                output[1, i] = FFTData[i].Real * FFTData[i].Real + FFTData[i].Imaginary * FFTData[i].Imaginary; //psd values
            }

            return output;
        }
        public static double[,] computeFFT(StopWatchPrecision sw, params object[] args)
        {
            int n = Convert.ToInt32(args[0]);

            if (IsPowerOfTwo(n) == false)
                throw new Exception("FFT Can only capture values in powers of 2.");


            // ---------------------------- Gather Data ----------------------------------

            double startTime = (double)sw.ElapsedMilliseconds; //get starting time for data recording
            double[] inputData = generateSignal(n);
            double samplingFreq = 1000.0 * n / ((double)sw.ElapsedMilliseconds - startTime); //compute average sampling frequency based on start/end times

            // ---------------------------- Compute Frequencies ----------------------------------

            double[] Freqs = CalculateFrequencies(n, samplingFreq); //occasionally can be infinity if inputdata takes >1ms

            // ---------------------------- Calculate FFT ----------------------------------

            Complex32[] CompArray = new Complex32[n];

            for (int i = 0; i < n; i++)
                CompArray[i] = new Complex32((float)inputData[i], 0); //fill array with data in real components

            Fourier.Forward(CompArray);

            return CalculatePSD(Freqs, CompArray, 15000);
        } //old
        public static double[,] computeFFT(double[,] inputData)
        {
            int n = inputData.GetLength(0);
            double samplingFreq = n / (inputData[0, n - 1] - inputData[0, 0]); //compute average sampling frequency based on start/end times

            // ---------------------------- Compute Frequencies ----------------------------------

            double[] Freqs = CalculateFrequencies(n, samplingFreq); //occasionally can be infinity if inputdata takes >1ms

            // ---------------------------- Calculate FFT ----------------------------------

            Complex32[] CompArray = new Complex32[n];

            for (int i = 0; i < n; i++)
                CompArray[i] = new Complex32((float)inputData[1, i], 0); //fill array with data in real components


            Fourier.Forward(CompArray);

            return CalculatePSD(Freqs, CompArray);
        }


        private static double[] generateSignal(int counter)
        {
            double[] output = new double[counter];

            Random rnd = new Random();

            double scale = rnd.Next(1, 20);
            double period = rnd.Next(1, 20);

            double scale2 = rnd.Next(1, 20);
            double period2 = rnd.Next(1, 20);

            for (int i = 0; i < counter; i++)
            {
                output[i] = scale * Math.Sin(period * i / 100) + scale2 * Math.Cos(period2 * i / 100) + i * i / (counter * counter);
                scale2 = rnd.Next(1, 20);
                period2 = rnd.Next(1, 20);

            }
            Thread.Sleep(1);
            return output;

        } //generates a random signal for testing
        private static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

    }
}
