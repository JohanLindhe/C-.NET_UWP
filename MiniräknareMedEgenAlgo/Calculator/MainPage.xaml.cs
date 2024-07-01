using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Calculator
{
    public sealed partial class MainPage : Page
    {
        private string currentInput = string.Empty;
        private double result = 0;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string buttonText = button.Content.ToString();

            if (buttonText == "=")
            {
                PerformCalculation();
            }
            else if (buttonText == "C")
            {
                ClearCalculator();
            }
            else
            {
                currentInput += buttonText;
                InputTextBox(currentInput);
            }
        }

        private void PerformCalculation()
        {
            try
            {
                result = Utrakning(currentInput);

                // Om man delar med 0 blir svarer oändligt. Om svaret bli oändligt så skrivs det istälelt ut en text
                // https://learn.microsoft.com/en-us/dotnet/api/system.double.isinfinity?view=net-8.0
                
                if (double.IsInfinity(result))
                {
                    InputTextBox("Cant divide with zero");

                    currentInput = string.Empty;
                }
                else
                {
                    InputTextBox(result.ToString());
                    currentInput = result.ToString();
                }
            }
            catch (Exception ex)
            {
                InputTextBox(ex.Message);
                currentInput = string.Empty; 
                
            }
        }


        private void ClearCalculator()
        {
            currentInput = string.Empty;
            
            InputTextBox(currentInput);
        }

        private void InputTextBox(string cInput)
        {
            input.Text = cInput;
        }

        private double Utrakning(string expression)
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.string.split?view=dotnet-uwp-10.0#system-string-split(system-char-system-int32-system-stringsplitoptions)
            string[] inputOperators = expression.Split('+', '*', '-', '/');

            
            // Iden att använda och loopa genom indexPlatser är chatGpt. Att få det att funka det med Switch, case och breake är egen.
            char[] operators = expression.ToCharArray();
            double result = double.Parse(inputOperators[0]);
            
                    
            int indexPlats = 1; 
         
            for (int i = 0; i < operators.Length; i++)
            {
                switch (operators[i])
                {
                    case '+':
                        result += double.Parse(inputOperators[indexPlats]);
                        indexPlats++;
                        break;
                    case '*':
                        result *= double.Parse(inputOperators[indexPlats]);
                        indexPlats++;
                        break;
                    case '/':
                        result /= double.Parse(inputOperators[indexPlats]);
                        indexPlats++;
                        break;
                    case '-':
                        result -= double.Parse(inputOperators[indexPlats]);
                        indexPlats++;
                        break;
                     
                }
            }

            return result;
        }

    }
}
