namespace GFunctions.Winforms.Input
{
    /// <summary>
    /// A textbox that nicely allows inputting numerical values, and provides error checking for non-number values
    /// </summary>
    public class NumericalInputTextBox : TextBox
    {
        /// <summary>
        /// Value of the textbox
        /// </summary>
        public double Value
        {
            get { return ConvertInput(Text); }
            set { Text = Convert.ToString(value); }
        }
        
        /// <summary>
        /// True of the text is a valid number
        /// </summary>
        public bool TextValid { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public NumericalInputTextBox()
        {
            TextChanged += TextBox_TextChanged;
            Click += TextBox_MouseClick;
            ValidateText();
        }


        private void TextBox_MouseClick(object? Sender, EventArgs e)
        {
            if (Sender != null)
            {
                TextBox sendTxt = (TextBox)Sender;
                sendTxt.SelectAll();
            }
        }
        private void TextBox_TextChanged(object? Sender, EventArgs e)
        {
            ValidateText();
        }


        private bool ValidateText()
        {
            TextValid = CheckInput(Text);
            BackColor = TextValid ? Color.White : Color.IndianRed;
            return TextValid;
        }
        private static bool CheckInput(string text)
        {
            try
            {
                ConvertInput(text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static double ConvertInput(string text)
        {
            return Convert.ToDouble(text);
        }
    }

}
