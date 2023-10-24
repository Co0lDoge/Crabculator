using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Globalization;


namespace S4_Calculator
{
    class Calculator
    {
        private List<double> nums;
        private List<string> textNums;
        private List<string> operations;

        public string currentNum;
        public double triMultiplier;

        // Standart constructor for the Calculator class. It initializes the lists that will be used to store
        // the calculator's history of numbers and operations, and initializes the current input string to an empty string.
        public Calculator()
        {
            nums = new List<double>();
            textNums= new List<string>();
            operations = new List<string>();
            currentNum = "";

            triMultiplier = (Math.PI / 180);
        }

        // This method assembles the complete input or operation string that will be displayed in the calculator's textBox.
        public string assembleString()
        {
            // Initialize the string that will hold the complete operations line.
            string textValue = "";

            // Loop through each number and operation in the calculator's history and append them to the string.
            for (int i = 0; i < nums.Count; i++)
            {
                textValue += textNums[i];
                textValue += operations[i];
            }

            // Append the current input string to the end of the string.
            textValue += currentNum;

            return textValue;
        }

        // This method handles a button click by adding the number to the current input string
        public void handleNum(string num)
        {
            // Do nothing if empty operator is present
            if (isEmptyOperator())
                return;

            currentNum += num;
        }

        // This method handles a button click by adding the current input string to the calculator's
        // number history as a number, and adding the specified operation to the history.
        public void handleOper(string operation)
        {
            // Switcher of operations text
            switch (operation)
            {
                case ("x^y"):
                    operation = "^";
                    break;
                default:
                    break;
            }

            if (isEmptyOperator())
            {
                operations[operations.Count - 1] = operation;
                return;
            }

            // If first operator of emtry line if "^", return function
            if (operation == "^" && assembleString().Length == 0)
                return;
            

            // If the assembledstring is empty, adds operator to current number
            if (assembleString().Length == 0 && operation != "^")
            {
                switch (operation)
                {
                    case ("+"):
                        nums.Add(0);
                        textNums.Add("0");
                        break;
                    case ("-"):
                        nums.Add(0);
                        textNums.Add("0");
                        break;

                    case ("*"):
                        nums.Add(1);
                        textNums.Add("1");
                        break;
                    case (":"):
                        nums.Add(1);
                        textNums.Add("1");
                        break;
                }


                operations.Add(operation);
                Console.WriteLine("aaaa");
            }

            // If the current number string is not empty, add it to the calculator's history as a number and reset the input string.
            if (currentNum.Length > 0)
            {
                double numToArray = double.Parse(currentNum, CultureInfo.InvariantCulture);
                nums.Add(numToArray);
                textNums.Add(numToArray.ToString());

                currentNum = "";

                operations.Add(operation);
            }

            // If the current input string is empty, replace the most recent operation in the operations list with the new operation.
            else
            {
                operations.RemoveAt(operations.Count - 1);
                operations.Add(operation);
            }
        }

        public void handleFunc(string funcName)
        {
            if(isEmptyOperator())
            {
                removeEmptyOperator();
            }

            string textValue = assembleString();

            if (textValue.Length == 0)
                return;

            double[] savedNums = new double[nums.Count];
            string[] savedTextNums = new string[textNums.Count];
            string[] savedOperations = new string[operations.Count];
            bool copyRequired = false;


            // If last symbol in string is operator, saves all content to be reused before string
            if (currentNum.Length == 0 && operations.Count > 0)
            {
                copyRequired = true;
                nums.CopyTo(savedNums, 0);
                textNums.CopyTo(savedTextNums, 0);
                operations.CopyTo(savedOperations, 0);

                // Removes last operation to allow calculations
                textValue = textValue.Remove(textValue.Length - 1);
                operations.RemoveAt(operations.Count - 1);
            }

            double numValue = calculate();

            switch (funcName)
            {
                case "√x":
                    textValue = $"√({textValue})";
                    numValue = Math.Sqrt(numValue);
                    break;

                // TODO: No function in math library, require questioning
                case "!":
                    if (numValue % 1 != 0)
                        throw new ArgumentException("Cannot get factorial of non-int");

                    if (numValue > 170)
                        throw new ArgumentException("Number is too big");

                    textValue = $"!({textValue})";
                    numValue = (double)factorial((int)numValue);
                    break;


                case "1/x":
                    if (numValue == 0)
                        throw new ArgumentException("Cannot divide by zero");

                    textValue = $"1/({textValue})";
                    numValue = 1/(numValue);
                    break;

                case "lg":
                    if (numValue <= 0)
                    {
                        throw new ArgumentException("Invalid input: lg(<=0) doesn't exist");
                    }

                    textValue = $"lg({textValue})";
                    numValue = Math.Log10(numValue);
                    break;

                case "ln":
                    if (numValue <= 0)
                    {
                        throw new ArgumentException("Invalid input: ln(<=0) doesn't exist");
                    }
                    textValue = $"lg({textValue})";
                    numValue = Math.Log(numValue);
                    break;

                case "(":
                    textValue = $"({textValue})";
                    break;

                case ")":
                    textValue = $"({textValue})";
                    break;

                case "%":
                    textValue = $"%({textValue})";
                    numValue /= 100;
                    break;

                case "sin":
                    textValue = $"sin({textValue})";
                    numValue = Math.Sin(numValue * triMultiplier);
                    break;

                case "cos":
                    textValue = $"cos({textValue})";
                    numValue = Math.Cos(numValue * triMultiplier);
                    break;

                case "tan":
                    if (Math.Round(Math.Cos(numValue % 180)) == 0)
                        throw new ArgumentException("Invalid input (cos is 0)");

                    textValue = $"tan({textValue})";
                    numValue = Math.Tan(numValue * triMultiplier);
                    break;
            }

            // Rounding the number
            numValue = Math.Round(numValue, 10);

            clearAll();

            // If last symbol was operator, copies previous values before function value
            if (copyRequired)
            {
                nums = new List<double>(savedNums);
                textNums = new List<string>(savedTextNums);
                operations = new List<string>(savedOperations);
            }

            nums.Add(numValue);
            textNums.Add(textValue);
            operations.Add("");

            currentNum = "";
        }

        // This method calculates the result of the current input string by performing any multiplication
        // or division operations, then any addition or subtraction operations.
        public double calculate()
        {
            double result;

            if (isEmptyOperator()) 
            {
                removeEmptyOperator();
            };

            // If the current input string is not empty, add it to the nums list.
            if (currentNum.Length > 0)
            {
                double numToArray = double.Parse(currentNum, CultureInfo.InvariantCulture);
                nums.Add(numToArray);
            }

            
            if (nums.Count != operations.Count + 1)
            {
                return double.NaN;
            }

            // Multiply and divide operations.
            bool hasMultiOrDiv = true;
            while (hasMultiOrDiv)
            {
                hasMultiOrDiv = false;

                // Find the index of the first multiplication and division operation.
                // Then calculates first of this operations
                int powIndex = operations.IndexOf("^");
                int multiIndex = operations.IndexOf("*");
                int divIndex = operations.IndexOf(":");

                // If a power operation was found, perform it.
                // Perform pow firstly
                if (powIndex != -1)
                {
                    hasMultiOrDiv = true;
                    nums[powIndex] = Math.Pow(nums[powIndex], nums[powIndex + 1]);

                    // Throws exeption result is NaN.
                    if (double.IsNaN(nums[powIndex]))
                        throw new ArgumentException("Negative fraction can't be raised to the power of fraction");

                    // Remove the right number and the operation from their respective lists.
                    nums.RemoveAt(powIndex + 1);
                    operations.RemoveAt(powIndex);

                    // Continue to the next iteration of the loop.
                    continue;
                }

                // If a multiplication operation was found, perform it.
                // Perform multiplication, if it stands before all divisions or there is no divisions
                if (multiIndex != -1 && (multiIndex < divIndex || divIndex == -1))
                {
                    hasMultiOrDiv = true;
                    nums[multiIndex] *= nums[multiIndex + 1];

                    // Remove the right number and the operation from their respective lists.
                    nums.RemoveAt(multiIndex + 1);
                    operations.RemoveAt(multiIndex);

                    // Continue to the next iteration of the loop.
                    continue;
                }

                // If a division operation was found, perform it.
                // Perform division, if it stands before all multiplication or there is no multiplication
                if (divIndex != -1 && (divIndex < multiIndex || multiIndex == -1))
                {
                    hasMultiOrDiv = true;

                    // Divide the left number by the right number and replace the left number with the result.
                    nums[divIndex] /= nums[divIndex + 1];
                    if (nums[divIndex + 1] == 0)
                        throw new ArgumentException("Cannot divide by zero");

                    // Remove the right number and the operation from their respective lists.
                    nums.RemoveAt(divIndex + 1);
                    operations.RemoveAt(divIndex);

                    // Continue to the next iteration of the loop.
                    continue;
                }
            }

            // Addition and subtra operations.
            result = nums[0];
            for (int i = 0; i < nums.Count - 1; i++)
            {
                if (operations[i] == "+")
                    result += nums[i + 1];
                if (operations[i] == "-")
                    result -= nums[i + 1];
            }
            
            clearAll();

            currentNum = result.ToString();
            return result;
        }

        // This method removes the last input or operation that was added to the calculator.
        public void removeLast()
        {

            // Check if there is any input or operation in the calculator's number history.
            if (assembleString().Length == 0)
                return;

            // If the current input string is not empty, remove the last character from it.
            if (currentNum.Length > 0)
            {
                currentNum = currentNum.Remove(currentNum.Length - 1);

                // If last symbol is "-", remove it
                if (currentNum == "-") currentNum = "";
                return;
            }

            // If the current input string is empty, remove the last complete number or operation from the calculator's history.
            else
            {
                // If last input was function, removes last operator from
                if (textNums[nums.Count - 1].Contains(")") && operations[operations.Count - 1] == "")
                {
                    nums.RemoveAt(nums.Count - 1);
                    textNums.RemoveAt(textNums.Count - 1);
                    operations.RemoveAt(operations.Count - 1);
                    return;
                }
                else if (textNums[nums.Count - 1].Contains(")"))
                {
                    operations[operations.Count - 1] = "";
                    return;
                }


                currentNum = nums[nums.Count - 1].ToString();

                nums.RemoveAt(nums.Count - 1);
                textNums.RemoveAt(textNums.Count - 1);
                operations.RemoveAt(operations.Count - 1);
            }
        }

        // This method will clear any data stored in calculator's object
        public void clearAll()
        {
            nums.Clear();
            textNums.Clear();
            operations.Clear();
            currentNum = "";
        }

        // Checks if empty operator is present
        private bool isEmptyOperator()
        {
            
            if (operations.IndexOf("") != -1)
            {
                return true;
            }

            return false;
        }

        public void removeEmptyOperator()
        {
            currentNum = nums[nums.Count - 1].ToString();

            nums.RemoveAt(nums.Count - 1);
            textNums.RemoveAt(textNums.Count - 1);
            operations.RemoveAt(operations.Count - 1);
        }

        private BigInteger factorial(int num)
        {
            BigInteger res = 1;
            for (int i = 1; i < num + 1; i++)
            {
                res *= i;
            }

            return res;
        }

        public void PlusMinus()
        {
            if (currentNum.Length > 0 && !currentNum.Contains("-"))
                currentNum = "-" + currentNum;

            else if (currentNum.Length > 0 && currentNum.Contains("-"))
                currentNum = currentNum.Replace("-", "");
        }
    }
}
