namespace GFunctions.Winforms.Input
{
    /// <summary>
    /// A textbox that nicely displays numeric output values with error display
    /// </summary>
    public partial class NumericDisplayTextBox : TextBox
    {
        /// <summary>
        /// Stores the default color when the control gets initialized
        /// </summary>
        private readonly Color defaultBackColor = Color.White;

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
            TextChanged += HandleTextChanged;
            Click += TextBox_MouseClick;
        
            DataBindings.Clear();
            DataBindings.Add("Text", this, "Model.ValueString", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        /// <summary>
        /// Turns the control red if an error occurred
        /// </summary>
        /// <param name="error">True if an error occurred</param>
        public void SetErrorStatus(bool error)
        {
            BackColor = error ? Color.IndianRed : defaultBackColor;
        }

        //------------------------- Event Methods -------------------------------------
        private void HandleTextChanged(object? sender, EventArgs e)
        {
            try
            {
                ForeColor = Model.Value is null ? Color.Red : Color.Black;
            }
            catch (Exception) { }
        }
        private void TextBox_MouseClick(object? Sender, EventArgs e)
        {
            if (Sender != null)
            {
                TextBox sendTxt = (TextBox)Sender;
                sendTxt.SelectAll();
            }
        }
    }
}
