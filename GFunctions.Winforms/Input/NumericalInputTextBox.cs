using System;
using System.Drawing;
using System.Windows.Forms;


namespace GFunctions.Winforms.Input
{
    public class NumericalInputTextBox : TextBox
    {
        public double Value
        {
            get
            {
                return this.ConvertInput(this.Text);
            }
            set
            {
                this.Text = Convert.ToString(value);
            }
        }

        public bool TextValid { get; private set; }

        public NumericalInputTextBox()
        {
            this.TextChanged += this.TextBox_TextChanged;
            this.Click += this.TextBox_MouseClick;
            this.ValidateText();
        }


        private void TextBox_MouseClick(object Sender, EventArgs e)
        {
            TextBox sendTxt = (TextBox)Sender;
            sendTxt.SelectAll();
        }
        private void TextBox_TextChanged(object Sender, EventArgs e)
        {
            this.ValidateText();
        }


        private bool ValidateText()
        {
            this.TextValid = CheckInput(this.Text);

            if (this.TextValid)
            {
                this.BackColor = Color.White;
            }
            else
            {
                this.BackColor = Color.IndianRed;
            }

            return this.TextValid;
        }
        private bool CheckInput(string text)
        {
            try
            {
                this.ConvertInput(text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private double ConvertInput(string text)
        {
            return Convert.ToDouble(text);
        }

    }

}
