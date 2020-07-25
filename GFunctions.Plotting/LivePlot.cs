using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using GFunctions.Timing;


namespace GFunctions.Plotting
{
    public class PlotTargetFunction
    {
        public delegate double[,] targetFunction(StopWatchPrecision sw, params object[] args);

        private targetFunction _func;
        private object[] _args;

        public PlotTargetFunction(targetFunction Func, params object[] args)
        {
            _func = Func;
            _args = args;
        }

        public double[,] RunFunction(StopWatchPrecision sw)
        {
            return _func(sw, _args);
        }

    }
    public class PlotSingleRunFunction
    {
        public delegate void FunctionFormat(params object[] args);

        private FunctionFormat _func;
        private object[] _args;

        public PlotSingleRunFunction(FunctionFormat Func, params object[] args)
        {
            _func = Func;
            _args = args;
        }
        public void RunFunction()
        {
            _func(_args);
        }

    }


    public class LivePlot
    {
        public PlotModel _model = new PlotModel { Title = "Live Plot" };
        private string _title = "Live Plot";
        private double _refreshRate = -1; //refresh time in seconds
        private double _cutOffPeriod = -1; //max period to keep data, in seconds
        private StopWatchPrecision _sw = new StopWatchPrecision(); //keeps track of the starting time, what time data is added
        private bool _running = false;
        private bool _finishedStop = false; //flag to notify that all running loops have ended

        private List<PlotTargetFunction> _funcs = new List<PlotTargetFunction>(); //holds the target functions to get data for each line
        private List<LineSeries> _lineSeriesList = new List<LineSeries>(); //holds the plot line objects

        private List<PlotSingleRunFunction> _singleRunFuncs = new List<PlotSingleRunFunction>(); //holds pointers for functions that will be run once then deleted (allow for thread safe function calls while running)

        public PlotModel model { get { return this._model; } }
        public bool running { get { return this._running; } }

        public LivePlot(string title, string XAxisTitle, string YAxisTitle)
        {
            this._title = title;
            this._model = new PlotModel { Title = this._title };

            _model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = XAxisTitle });
            _model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = YAxisTitle });
        }

        public void AddPlotLine(LineSeries line, PlotTargetFunction func)
        {
            _model.Series.Add(line);
            _lineSeriesList.Add(line);
            _funcs.Add(func);
        }
        public void AddSingleRunFunction(PlotSingleRunFunction func)
        {
            _singleRunFuncs.Add(func);
        }

        public void Start(double cutOffPeriod, double refreshRate, Progress<int> FPSCallBack)
        {
            ClearData();

            _cutOffPeriod = cutOffPeriod; //0 means that whole plot updates each time
            _refreshRate = refreshRate;
            _sw.StartNew(); //log starting time
            _running = true;
            _finishedStop = false;

            if (this._cutOffPeriod > 0)
            {
                Task.Factory.StartNew(() => updateGUI_incremental(_lineSeriesList, FPSCallBack), TaskCreationOptions.LongRunning);
            }
            else
            {
                Task.Factory.StartNew(() => updateGUI(_lineSeriesList, FPSCallBack), TaskCreationOptions.LongRunning);
            }
        }
        public void Stop()
        {
            this._running = false; //set flag to stop

            while (this._finishedStop == false) //loops still running, need to wait            
                Thread.Sleep((int)(_refreshRate * 500)); //sleep for half a refresh period (converts to ms)

            //----------- now safely run clean up code -------------

            _sw.Stop();

        }
        public void ClearData()
        {
            for (int i = 0; i < _lineSeriesList.Count; i++)
            {
                _lineSeriesList[i].Points.Clear();
            }
            _model.InvalidatePlot(true);
        }


        private void updateGUI_incremental(List<LineSeries> lines, IProgress<int> progress)
        {
            while (this._running == true)
            {
                double loopStartTime = (double)_sw.ElapsedMilliseconds;
                double loopEndTime = loopStartTime + this._refreshRate * 1000.0; //time loop should be ending in ms

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

                this._model.InvalidatePlot(true);

                // ------------- sleep if needed ------------------------------

                if (_sw.ElapsedMilliseconds < loopEndTime)
                { Thread.Sleep((int)(loopEndTime - _sw.ElapsedMilliseconds)); }

                // ------------- report operating frequency ------------------------------

                double freq = 1000.0 / ((double)_sw.ElapsedMilliseconds - loopStartTime);
                progress.Report((int)freq);
            }

            _finishedStop = true; //all running loops have ended
        }
        private void updateGUI(List<LineSeries> lines, IProgress<int> progress)
        {
            while (this._running == true)
            {
                double loopStartTime = _sw.ElapsedMilliseconds;
                double loopEndTime = loopStartTime + this._refreshRate * 1000.0; //time loop should be ending in ms

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

                this._model.InvalidatePlot(true);

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
