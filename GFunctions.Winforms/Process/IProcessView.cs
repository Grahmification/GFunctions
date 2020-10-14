using System;

namespace GFunctions.Winforms.Process
{
    /// <summary>
    /// Views that allow starting and stopping long running processes
    /// </summary>
    public interface IProcessView
    {
        /// <summary>
        /// The model for this view
        /// </summary>
        //T Model { get; set; }

        string ToggleButtonText { get; set; }

        /// <summary>
        /// Fired when the user interacts with the view to read a setting
        /// </summary>
        event EventHandler StartRequest;

        /// <summary>
        /// Fired when the user interacts with the view to read a setting
        /// </summary>
        event EventHandler StopRequest;

        /// <summary>
        /// Fired when the user interacts with the view to read a setting
        /// </summary>
        event EventHandler ToggleRequest;

        /// <summary>
        /// Allows or disables user interaction with the view
        /// </summary>
        /// <param name="enabled">True if the user is allowed to interact</param>
        void AllowUse(bool enabled);

        /// <summary>
        /// Sets the status text of the view
        /// </summary>
        /// <param name="status">The status text to display</param>
        void SetStatus(string status);

        /// <summary>
        /// Sets the progress of the view
        /// </summary>
        /// <param name="percentProgress">Percentage progress from 0 to 100</param>
        void SetProgress(int percentProgress);
    }
}
