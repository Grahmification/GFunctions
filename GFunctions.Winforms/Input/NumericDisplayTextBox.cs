using System;
using System.Drawing;
using System.Windows.Forms;

namespace GFunctions.Winforms.Input
{
    public partial class NumericDisplayTextBox : TextBox
    {
        /// <summary>
        /// Stores the default color when the control gets initialized
        /// </summary>
        private Color defaultBackColor = Color.White;

        /// <summary>
        /// The data model for this textbox
        /// </summary>
        public NumericDisplayModel Model { get; set; } = new NumericDisplayModel();

        //------------------------- Public Methods -------------------------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        public NumericDisplayTextBox()
        {
            InitializeComponent();
            defaultBackColor = BackColor;
            this.TextChanged += handleTextChanged;
            this.Click += this.TextBox_MouseClick;
        
            this.DataBindings.Clear();
            this.DataBindings.Add("Text", this, "Model.ValueString", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        /// <summary>
        /// Turns the control red if an error occurred
        /// </summary>
        /// <param name="Error">True if an error occurred</param>
        public void SetErrorStatus(bool Error)
        {
            if (Error)
            {
                BackColor = Color.IndianRed;
            }
            else
            {
                BackColor = defaultBackColor;
            }
        }

        //------------------------- Event Methods -------------------------------------
        private void handleTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Model.Value is null)
                {
                    ForeColor = Color.Red;
                }
                else
                {
                    ForeColor = Color.Black;
                }
            }
            catch (Exception) { }
        }
        private void TextBox_MouseClick(object Sender, EventArgs e)
        {
            TextBox sendTxt = (TextBox)Sender;
            sendTxt.SelectAll();
        }
    }
}
