namespace GFunctions.Winforms.Dialogs
{
    /// <summary>
    /// Class providing a global entry point for displaying message boxes
    /// </summary>
    public static class FormattedMessageBox
    {

        /// <summary>
        /// Displays a user message in a <see cref="MessageBox"/>
        /// </summary>
        /// <param name="message">The user message</param>
        /// <param name="header">Optional header for the messagebox</param>
        /// <returns>Result from the message box button</returns>
        public static DialogResult DisplayMessage(string message, string header = "")
        {
            if (header == "")
                header = "Info";

            return MessageBox.Show(message, header, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays an error message to the user in a <see cref="MessageBox"/>
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>Result from the message box button</returns>
        public static DialogResult DisplayError(string message)
        {
            return MessageBox.Show("An error occurred: " + message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
