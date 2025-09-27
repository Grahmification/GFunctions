using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using GFunctions.Timing;

namespace GFunctions.Plotting
{
    /// <summary>
    /// A plot XY datapoint that also includes a time value
    /// </summary>
    /// <param name="xValue">The X plot value</param>
    /// <param name="yValue">The Y plot value</param>
    /// <param name="time">The point create time [s]</param>
    public class TimedDataPoint(double xValue, double yValue, double time): IDataPointProvider
    {
        /// <summary>
        /// The X plot value
        /// </summary>
        public double X { get; set; } = xValue;

        /// <summary>
        /// The Y plot value
        /// </summary>
        public double Y { get; set; } = yValue;

        /// <summary>
        /// The point create time [s]
        /// </summary>
        public double Time { get; set; } = time;

        /// <summary>
        /// Get the datapoint implementation of the class for <see cref="IDataPointProvider"/>
        /// </summary>
        /// <returns>DataPoint associated with X and Y</returns>
        public DataPoint GetDataPoint()
        {
            return new DataPoint(X, Y);
        }
    }

    /// <summary>
    /// Definition for a function that runs continuously for each tick of the plot
    /// </summary>
    /// <param name="stopWatch">Stopwatch containing the time of the plot</param>
    /// <returns>An array of points to add to the plot</returns>
    public delegate TimedDataPoint[] PlotDataFunction(ICurrentTimeProvider stopWatch);

    /// <summary>
    /// Wrapper to hold points for a plot series and an associated function to get new data
    /// </summary>
    /// <param name="function">The function to get new data points for the series</param>
    internal class PlotDataPointCollection(PlotDataFunction function)
    {
        /// <summary>
        /// Points to be plotted with a connected plot series, ordered from oldest to newest
        /// </summary>
        public List<TimedDataPoint> Points { get; private set; } = [];

        /// <summary>
        /// The function to get new datapoints
        /// </summary>
        public PlotDataFunction DataFunction { get; private set; } = function;

        /// <summary>
        /// Gets new datapoints from the function and appends them to the points
        /// </summary>
        /// <param name="stopWatch">Stopwatch containing the time of the plot</param>
        public void GetNewPoints(ICurrentTimeProvider stopWatch)
        {
            TimedDataPoint[] data = DataFunction(stopWatch);
            Points.AddRange(data);
        }

        /// <summary>
        /// Removes points with times older than the specified cutoff time
        /// </summary>
        /// <param name="deleteCutoff">Points older than this time (seconds) will be removed</param>
        public void RemoveOldPoints(double deleteCutoff)
        {
            int removeCount = 0;

            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].Time < deleteCutoff)
                    removeCount++;
                else
                    break; // If a good point is reached stop searching
            }
            
            // Remove all at once for efficiency
            Points.RemoveRange(0, removeCount);
            
        }
    }

    /// <summary>
    /// A plot based on <see cref="PlotModel"/> that calls functions to update itself in real time
    /// </summary>
    public class LivePlot
    {
        /// <summary>
        /// How long the plot will retain old data, in seconds. Zero means all data will refresh each update.
        /// </summary>
        private double _cutOffPeriod = 0;

        /// <summary>
        /// Handles automatically updating the plot at cyclic intervals
        /// </summary>
        private readonly TimeSimulation _simulation = new();

        /// <summary>
        /// Holds plot points and data retrieval functions
        /// </summary>
        private readonly List<PlotDataPointCollection> _pointCollections = [];

        /// <summary>
        /// Holds the functions that will be run once then deleted (allow for thread safe function calls while running)
        /// </summary>
        private readonly List<Action> _singleRunFuncs = [];

        // --------------------------- Events -------------------------

        /// <summary>
        /// Fires to indicate the plot update frequency has been changed. Int value is the current frequency [Hz].
        /// </summary>
        public event EventHandler<int>? RunFrequencyUpdated;

        // --------------------------- Public Properties -------------------------

        /// <summary>
        /// The underlying plot model to control the plot
        /// </summary>
        public PlotModel Model { get; private set; } = new() { Title = "Live Plot" };
        
        /// <summary>
        /// True if the plot is actively updating
        /// </summary>
        public bool Running => _simulation.Running;

        // --------------------------- Public Methods -------------------------

        /// <summary>
        /// Construct the class using a <see cref="PlotModel"/> with predefined title and axes
        /// </summary>
        /// <param name="title">The plot title</param>
        /// <param name="XAxisTitle">The plot x-axis title</param>
        /// <param name="YAxisTitle">The plot y-axis title</param>
        public LivePlot(string title, string XAxisTitle, string YAxisTitle)
        {
            var model = new PlotModel
            {
                Title = title
            };

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = XAxisTitle });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = YAxisTitle });

            Initialize(model);
        }

        /// <summary>
        /// Construct the class with any <see cref="PlotModel"/>
        /// </summary>
        /// <param name="model">The model to use for the LivePlot</param>
        public LivePlot(PlotModel model)
        {
            Initialize(model);
        }

        /// <summary>
        /// Common initialization logic
        /// </summary>
        private void Initialize(PlotModel model)
        {
            Model = model;

            _simulation.RunFrequencyUpdated += (s, e) => RunFrequencyUpdated?.Invoke(this, e); // Relay event upwards
            _simulation.SimulationDoWorkRequest += UpdatePlot;  // Called cyclically when the simulation is running
        }

        /// <summary>
        /// Adds a plot line that will automatically update with the plot
        /// </summary>
        /// <param name="line">The series to display</param>
        /// <param name="func">The function to generate points for the series</param>
        public void AddPlotLine(LineSeries line, PlotDataFunction func)
        {
            if (Running)
                throw new InvalidOperationException("Cannot add plot lines while the plot is updating.");

            PlotDataPointCollection collection = new(func);
            line.ItemsSource = collection.Points; // Link the LineSeries to the point collection
            
            Model.Series.Add(line);
            _pointCollections.Add(collection);
        }
        
        /// <summary>
        /// Adds a function that will run once with the next tick of the plot
        /// </summary>
        /// <param name="func">The function to run</param>
        public void AddSingleRunFunction(Action func)
        {
            _singleRunFuncs.Add(func);
        }

        /// <summary>
        /// Starts the plot automatically updating data
        /// </summary>
        /// <param name="cutOffPeriod">The max time to keep data in seconds, or 0 to refresh the whole plot each update</param>
        /// <param name="refreshRate">The refresh rate in seconds</param>
        public void Start(double cutOffPeriod, double refreshRate)
        {
            ClearData();

            _cutOffPeriod = cutOffPeriod; // 0 means that whole plot updates each time
            _simulation.Start(refreshRate * 1000); // Convert to ms
        }

        /// <summary>
        /// Stops the plot from updating
        /// </summary>
        public void Stop()
        {
            _simulation.Stop();
        }

        /// <summary>
        /// Clears all the lines on the plot
        /// </summary>
        public void ClearData()
        {
            // If we're running, make sure the methdo runs in the background process
            if (Running)
                AddSingleRunFunction(ClearDataDoWork);
            else
                ClearDataDoWork();
        }

        // --------------------------- Private Helpers -------------------------

        private void UpdatePlot(object? sender, TimeSimulationStepEventArgs e)
        {
            //-------------- Delete points from lists --------------

            // If we have a real cutoff period
            if (_cutOffPeriod > 0)
            {
                // Delete old points beyond the cutoff
                double deleteCutoff = (_simulation.Time.ElapsedSeconds / 1000.0) - _cutOffPeriod; //any points with time less than this should be deleted

                foreach (PlotDataPointCollection collection in _pointCollections)
                    collection.RemoveOldPoints(deleteCutoff);
            }
            else
            {
                // Delete all points
                for (int i = 0; i < _pointCollections.Count; i++)
                    _pointCollections[i].Points.Clear();
            }

            // ------------- Run + Delete single run functions -----------------------------------
            for (int i = 0; i < _singleRunFuncs.Count; i++)
            {
                _singleRunFuncs[i](); // Run each function
            }
            _singleRunFuncs.Clear(); // Clear all values

            // ------------- Grab new values -----------------------------------
            for (int i = 0; i < _pointCollections.Count; i++)
            {
                _pointCollections[i].GetNewPoints(_simulation.Time);
            }

            // ------------- Update plot -----------------------------------
            Model.InvalidatePlot(true);

            // Flag the simulation to continue
            e.FlagWorkDone();
        }

        private void ClearDataDoWork()
        {
            for (int i = 0; i < _pointCollections.Count; i++)
            {
                _pointCollections[i].Points.Clear();
            }

            Model.InvalidatePlot(true);
        }
    }
}
