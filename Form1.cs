using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using static System.Windows.Forms.AxHost;

using System.Globalization;

namespace S4_Calculator
{
    public partial class Form1 : Form
    {
        Calculator calculator;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            calculator = new Calculator();
            Initialize_buttons(
                ref Globals.numbers,
                ref Globals.operations,
                ref Globals.functions,
                ref Globals.support,
                ref Globals.scientifics
                );

            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
        }

        private void Initialize_buttons(
            ref Button[] numbers,
            ref Button[] operations,
            ref Button[] functions,
            ref Button[] supports,
            ref Button[] scientifics
            )
        {
            numbers = new Button[]
            {
                button0, button1, button2, button3, button4,
                button5, button6, button7, button8, button9,
                buttonDot
            };

            supports = new Button[]
            {
                // TODO: Maybe E and Pi should be in numbers category
                buttonEquals, buttonErase, buttonC, buttonE, buttonPi
            };

            operations = new Button[]
            {

                buttonPlus, buttonMinus, buttonMultiply,
                buttonDivide, buttonXpowY
                
            };

            functions = new Button[]
            {
                // Suggestion: Brackets will be function that will do nothing
                // to numbers in brackets, all other function will calculate
                // something with numbers in brackets
                buttonRadical, buttonXpowY, buttonLg, buttonLn,
                buttonLBracket, buttonRBracket, buttonSecond,
                buttonDegRad, buttonSin, buttonCos, buttonTan,
                buttonFactorial, buttonReverse, buttonPercent
            };

            scientifics = new Button[]
            {
                buttonE, buttonPi, buttonReverse, buttonFactorial,
                buttonRadical, buttonXpowY, buttonLg, buttonLn,
                buttonLBracket, buttonRBracket, buttonSecond,
                buttonDegRad, buttonSin, buttonCos, buttonTan,
                
            };
        }

        private void buttonNum_Click(object sender, EventArgs e)
        {
            int num = Array.IndexOf(Globals.numbers, sender);
            //calculator.currentNum += num.ToString();
            calculator.handleNum(num.ToString());

            textBox1.Text = calculator.assembleString();
            textBox2.Text = calculator.currentNum;
        }

        private void buttonOper_Click(object sender, EventArgs e)
        {
            int index = Array.IndexOf(Globals.operations, sender);
            calculator.handleOper(Globals.operations[index].Text);

            textBox1.Text = calculator.assembleString();
            textBox2.Text = calculator.currentNum;
        }

        private void buttonFunc_Click(object sender, EventArgs e)
        {
            int index = Array.IndexOf(Globals.functions, sender);
            try 
            {
                calculator.handleFunc(Globals.functions[index].Text);

                textBox1.Text = calculator.assembleString();
                textBox2.Text = calculator.currentNum;
            } 

            catch (ArgumentException ex)
            {
                textBox1.Text = ex.Message;
                textBox2.Text = ex.Message;
                DisableButtons();
                Globals.support[2].Enabled = true;
            }
        }

        private void buttonSupport_Click(object sender, EventArgs e)
        {
            int index = Array.IndexOf(Globals.support, sender);
            switch(index)
            {
                // Equals button
                case 0:
                    textBox1.Text = "";
                    try
                    {
                        textBox2.Text = calculator.calculate().ToString();
                    }
                    catch(ArgumentException ex)
                    {
                        textBox1.Text = ex.Message;
                        textBox2.Text = ex.Message;
                        DisableButtons();
                        Globals.support[2].Enabled = true;
                    }
                    
                    break;

                // Erase button
                case 1:
                    calculator.removeLast();

                    textBox1.Text = calculator.assembleString();
                    textBox2.Text = calculator.currentNum;
                    break;

                // Clear button
                case 2:
                    calculator.clearAll();
                    

                    textBox1.Text = calculator.assembleString();
                    textBox2.Text = calculator.currentNum;
                    EnableButtons();
                    break;
            }
        }

        private void buttonE_Click(object sender, EventArgs e)
        {
            calculator.currentNum = "";
            calculator.handleNum(Math.E.ToString());

            textBox1.Text = calculator.assembleString();
        }

        private void buttonPi_Click(object sender, EventArgs e)
        {
            calculator.currentNum = "";
            calculator.handleNum(Math.PI.ToString());

            textBox1.Text = calculator.assembleString();
        }

        private void buttonSwitchMode_Click(object sender, EventArgs e)
        {
            // Checks state only for one button,
            // because all buttons will change state simultaneously
            bool state = !Globals.scientifics[0].Visible;

            for (int i = 0; i < Globals.scientifics.Length; i++)
            {
                //Globals.scientifics[i].Enabled = state;
                Globals.scientifics[i].Visible = state;
            }
        }

        private void buttonDot_Click(object sender, EventArgs e)
        {
            // Allow only one dot in number
            if(calculator.currentNum.IndexOf('.') == -1)
                calculator.currentNum += '.'.ToString();

            textBox1.Text = calculator.assembleString();
        }

        private void DisableButtons()
        {
            for (int i = 0; i < Globals.scientifics.Length; i++)
            {
                Globals.scientifics[i].Enabled = false;
            }
            for (int i = 0; i < Globals.operations.Length; i++)
            {
                Globals.operations[i].Enabled = false;
            }
            for (int i = 0; i < Globals.support.Length; i++)
            {
                Globals.support[i].Enabled = false;
            }
            for (int i = 0; i < Globals.numbers.Length; i++)
            {
                Globals.numbers[i].Enabled = false;
            }
            for (int i = 0; i < Globals.functions.Length; i++)
            {
                Globals.functions[i].Enabled = false;
            }
        }

        private void EnableButtons()
        {
            for (int i = 0; i < Globals.scientifics.Length; i++)
            {
                Globals.scientifics[i].Enabled = true;
            }
            for (int i = 0; i < Globals.operations.Length; i++)
            {
                Globals.operations[i].Enabled = true;
            }
            for (int i = 0; i < Globals.support.Length; i++)
            {
                Globals.support[i].Enabled = true;
            }
            for (int i = 0; i < Globals.numbers.Length; i++)
            {
                Globals.numbers[i].Enabled = true;
            }
            for (int i = 0; i < Globals.functions.Length; i++)
            {
                Globals.functions[i].Enabled = true;
            }
        }

        private void buttonDegRad_Click(object sender, EventArgs e)
        {
            if (buttonDegRad.Text == "deg")
            {
                buttonDegRad.Text = "rad";
                calculator.triMultiplier = 1;
                return;
            }

            buttonDegRad.Text = "deg";
            calculator.triMultiplier = (Math.PI / 180);
            return;
        }

        private void buttonPlusMinus_Click(object sender, EventArgs e)
        {
            calculator.PlusMinus();
            textBox1.Text = calculator.assembleString();
        }
    }
}
