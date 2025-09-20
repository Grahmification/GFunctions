/*Licence Declaration:

This code uses the Oxyplot libary under the MIT License

MIT License

Copyright (c) 2014 OxyPlot contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using GFunctions.Timing;


namespace GFunctions.Plotting
{
    /// <summary>
    /// A function that will run continuously to generate plot points
    /// </summary>
    public class PlotTargetFunction
    {
        /// <summary>
        /// Definition for a function that runs continuously for each tick of the plot
        /// </summary>
        /// <param name="sw">Stopwatch for time of the plot</param>
        /// <param name="args">Additional arguments</param>
        /// <returns>[X,Y] coordinates for the new plot point</returns>
        public delegate double[,] TargetFunction(StopWatchPrecision sw, params object[] args);

        private readonly TargetFunction _func;
        private readonly object[] _args;

        /// <summary>
        /// Initialize the class
        /// </summary>
        /// <param name="func">The function to get data</param>
        /// <param name="args">Optional input arguments to the function</param>
        public PlotTargetFunction(TargetFunction func, params object[] args)
        {
            _func = func;
            _args = args;
        }

        /// <summary>
        /// Executes the function
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        public double[,] RunFunction(StopWatchPrecision sw)
        {
            return _func(sw, _args);
        }
    }
    
    /// <summary>
    /// A function that runs once when added to the plot loop
    /// </summary>
    public class PlotSingleRunFunction
    {
        /// <summary>
        /// Definition of the function
        /// </summary>
        /// <param name="args">Any arguments to pass</param>
        public delegate void SingleRunFunction(params object[] args);

        private readonly SingleRunFunction _func;
        private readonly object[] _args;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func">The function to call</param>
        /// <param name="args">Optional input arguments to the function</param>
        public PlotSingleRunFunction(SingleRunFunction func, params object[] args)
        {
            _func = func;
            _args = args;
        }

        /// <summary>
        /// Executes the function
        /// </summary>
        public void RunFunction()
        {
            _func(_args);
        }
    }

    /// <summary>
    /// A plot based on <see cref="PlotModel"/> that calls functions to update itself in real time
    /// </summary>
    public class LivePlot
    {
        private double _refreshRate = -1; //refresh time in seconds
        private double _cutOffPeriod = -1; //max period to keep data, in seconds
        private readonly StopWatchPrecision _sw = new(); //keeps track of the starting time, what time data is added
        private bool _finishedStop = false; //flag to notify that all running loops have ended

        private readonly List<PlotTargetFunction> _funcs = []; //holds the target functions to get data for each line
        private readonly List<LineSeries> _lineSeriesList = []; //holds the plot line objects

        private readonly List<PlotSingleRunFunction> _singleRunFuncs = []; //holds pointers for functions that will be run once then deleted (allow for thread safe function calls while running)

        /// <summary>
        /// The underlying plot model to control the plot
        /// </summary>
        public PlotModel Model { get; private set; } = new() { Title = "Live Plot" };
        
        /// <summary>
        /// True if the plot is actively updating
        /// </summary>
        public bool Running { get; private set; } = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="title">The plot title</param>
        /// <param name="XAxisTitle">The plot x-axis title</param>
        /// <param name="YAxisTitle">The plot y-axis title</param>
        public LivePlot(string title, string XAxisTitle, string YAxisTitle)
        {
            Model = new PlotModel { Title = title };

            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = XAxisTitle });
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = YAxisTitle });
        }

        /// <summary>
        /// Adds a plot line that will automatically update with the plot
        /// </summary>
        /// <param name="line">The series to display</param>
        /// <param name="func">The function to generate points for the series</param>
        public void AddPlotLine(LineSeries line, PlotTargetFunction func)
        {
            Model.Series.Add(line);
            _lineSeriesList.Add(line);
            _funcs.Add(func);
        }
        
        /// <summary>
        /// Adds a function that will run once with the next tick of the plot
        /// </summary>
        /// <param name="func">The function to run</param>
        public void AddSingleRunFunction(PlotSingleRunFunction func)
        {
            _singleRunFuncs.Add(func);
        }

        /// <summary>
        /// Starts the plot automatically updating data
        /// </summary>
        /// <param name="cutOffPeriod">The max time to keep data in seconds, or 0 to refresh the whole plot each update</param>
        /// <param name="refreshRate">The refresh rate in seconds</param>
        /// <param name="FPSCallBack">A callback to get the actual plot refresh rate</param>
        public void Start(double cutOffPeriod, double refreshRate, Progress<int> FPSCallBack)
        {
            ClearData();

            _cutOffPeriod = cutOffPeriod; //0 means that whole plot updates each time
            _refreshRate = refreshRate;
            _sw.StartNew(); //log starting time
            Running = true;
            _finishedStop = false;

            if (_cutOffPeriod > 0)
            {
                Task.Factory.StartNew(() => UpdatePlotIncremental(_lineSeriesList, FPSCallBack), TaskCreationOptions.LongRunning);
            }
            else
            {
                Task.Factory.StartNew(() => UpdatePlot(_lineSeriesList, FPSCallBack), TaskCreationOptions.LongRunning);
            }
        }
        
        /// <summary>
        /// Stops the plot from updating
        /// </summary>
        public void Stop()
        {
            Running = false; //set flag to stop

            while (!_finishedStop) //loops still running, need to wait            
                Thread.Sleep((int)(_refreshRate * 500)); //sleep for half a refresh period (converts to ms)

            //----------- now safely run clean up code -------------
            _sw.Stop();
        }
        
        /// <summary>
        /// Clears all the lines on the plot
        /// </summary>
        public void ClearData()
        {
            for (int i = 0; i < _lineSeriesList.Count; i++)
            {
                _lineSeriesList[i].Points.Clear();
            }
            Model.InvalidatePlot(true);
        }


        private void UpdatePlotIncremental(List<LineSeries> lines, IProgress<int> progress)
        {
            while (Running)
            {
                double loopStartTime = (double)_sw.ElapsedMilliseconds;
                double loopEndTime = loopStartTime + _refreshRate * 1000.0; //time loop should be ending in ms

                //-------------- Delete old points from lists --------------
                double deleteCutoff = (_sw.ElapsedMilliseconds / 1000.0) - _cutOffPeriod; //any points with time less than this should be deleted

                for (int i = 0; i < lines.Count; i++)
                {
                    LineSeries L = lines[i];

                    for (int j = 0; j < L.Points.Count; j++)
                    {
                        if (L.Points[j].X < deleteCutoff) { L.Points.RemoveAt(j); } //remove point 
                        else { break; } // if a good point is reached stop searching
                    }
                }

                // ------------- Run + Delete single run functions -----------------------------------
                for (int i = 0; i < _singleRunFuncs.Count; i++)
                {
                    _singleRunFuncs[i].RunFunction(); //run each function
                }
                _singleRunFuncs.Clear(); //clear all values

                // ------------- Grab new values -----------------------------------
                for (int i = 0; i < _funcs.Count; i++)
                {
                    double[,] data = _funcs[i].RunFunction(_sw);

                    int count2 = data.GetLength(1);

                    for (int j = 0; j < count2; j++)
                        lines[i].Points.Add(new DataPoint(data[0, j], data[1, j]));
                }

                // ------------- Update plot -----------------------------------
                Model.InvalidatePlot(true);

                // ------------- sleep if needed ------------------------------
                if (_sw.ElapsedMilliseconds < loopEndTime)
                { Thread.Sleep((int)(loopEndTime - _sw.ElapsedMilliseconds)); }

                // ------------- report operating frequency ------------------------------
                double freq = 1000.0 / ((double)_sw.ElapsedMilliseconds - loopStartTime);
                progress.Report((int)freq);
            }

            _finishedStop = true; //all running loops have ended
        }
        private void UpdatePlot(List<LineSeries> lines, IProgress<int> progress)
        {
            while (Running)
            {
                double loopStartTime = _sw.ElapsedMilliseconds;
                double loopEndTime = loopStartTime + _refreshRate * 1000.0; //time loop should be ending in ms

                //-------------- Delete all points from lists --------------
                for (int i = 0; i < lines.Count; i++)
                {
                    lines[i].Points.Clear();
                }

                // ------------- Run + Delete single run functions -----------------------------------
                for (int i = 0; i < _singleRunFuncs.Count; i++)
                {
                    _singleRunFuncs[i].RunFunction(); //run each function
                }
                _singleRunFuncs.Clear(); //clear all values

                // ------------- Grab new values -----------------------------------
                for (int i = 0; i < _funcs.Count; i++)
                {
                    double[,] data = _funcs[i].RunFunction(_sw);

                    int count2 = data.GetLength(1);

                    for (int j = 0; j < count2; j++)
                        lines[i].Points.Add(new DataPoint(data[0, j], data[1, j]));
                }

                // ------------- Update plot -----------------------------------
                Model.InvalidatePlot(true);

                // ------------- sleep if needed ------------------------------
                if (_sw.ElapsedMilliseconds < loopEndTime)
                { Thread.Sleep((int)(loopEndTime - _sw.ElapsedMilliseconds)); }

                // ------------- report operating frequency ------------------------------
                double freq = 1000.0 / (_sw.ElapsedMilliseconds - loopStartTime);
                progress.Report((int)freq);
            }

            _finishedStop = true; //all running loops have ended
        }
    }
}
