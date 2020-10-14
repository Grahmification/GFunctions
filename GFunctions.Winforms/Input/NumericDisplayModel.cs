using System;
using System.ComponentModel;


namespace GFunctions.Winforms.Input
{
    /// <summary>
    /// A data model providing conversion between a bindable text value and a double value
    /// </summary>
    public class NumericDisplayModel : INotifyPropertyChanged
    {
        private string valueString = "";

        //------------------------- Public Properties -------------------------------------

        /// <summary>
        /// The amount of decimal places to display (-1 = all)
        /// </summary>
        public int DecimalPlaces { get; set; } = -1;

        /// <summary>
        /// The value in text form - used for binding
        /// </summary>
        public string ValueString
        {
            get { return valueString; }
            set
            {
                valueString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueString)));
            }
        }

        /// <summary>
        /// The numeric value
        /// </summary>
        public double? Value
        {
            get
            {
                var val = StringToNullDouble(valueString);

                //conversion won't work if the value is null
                if (val is null)
                    return val;

                return convertInput(val.Value);
            }
            set
            {
                if (value is null)
                    ValueString = NullDoubleToString(value, DecimalPlaces);
                else
                    ValueString = NullDoubleToString(convertOutput(value.Value), DecimalPlaces);
            }
        }

        /// <summary>
        /// True if the textbox value is a valid double
        /// </summary>
        bool ValueValid { 
            get 
            {
                if (Value is null)
                    return false;
                else
                    return true;
            } }

        /// <summary>
        /// The conversion ratio for the <see cref="Value"/>. Gets multiplied to the input
        /// </summary>
        public double ConversionRatio { get; set; } = 1;

        //------------------------- Events -------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;

        //------------------------- Public Methods -------------------------------------

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NumericDisplayModel()
        {
            Value = null;
        }

        //------------------------- Private Methods -------------------------------------

        private double convertInput(double input)
        {
            return input *= ConversionRatio;
        }
        private double convertOutput(double output)
        {
            return output /= ConversionRatio;
        }

        //------------------------- Static Methods -------------------------------------

        /// <summary>
        /// Converts a string to double, or null if invalid input
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns></returns>
        public static double? StringToNullDouble(string input)
        {
            try
            {
                return Convert.ToDouble(input);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// Safely converts a nullable double to formatted string
        /// </summary>
        /// <param name="input">The value to convert</param>
        /// <param name="decimals">The decimal places in the string</param>
        /// <returns></returns>
        public static string NullDoubleToString(double? input, int decimals = -1)
        {
            if (input is null)
            {
                return "";
            }
            else
            {
                if (decimals >= 0)
                {
                    input = Math.Round(input.Value, decimals);
                }

                return Convert.ToString(input);
            }
        }
    }
}
