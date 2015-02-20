using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Globalization;

namespace Calculadora
{
    public partial class MainPage : PhoneApplicationPage
    {
        int index;

        List<Double> numList;
        List<String> opeList;
        List<Boolean> opePriorList;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            textBlock_Res.Text = "0";

            index = 0;
            numList = new List<Double>();
            opeList = new List<string>();
            opePriorList = new List<Boolean>();

            History.Text = "Index = " + index;

           /* MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();
            sc.Language = "VBScript";
            string expression = "1 + 2 * 7";
            object result = sc.Eval(expression);
            MessageBox.Show(result.ToString());*/
            
        }

        private void clickOnNumber(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            Button b = (Button)fe;
            String numberS = b.Content.ToString();
            if (textBlock_Res.Text.ToString() == "0")
            {
                textBlock_Res.Text = "";
            }
            textBlock_Res.Text = textBlock_Res.Text+numberS+"";            
        }

        private void clickOnOperator(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement) sender; Button b=(Button) fe;
            String sentence = textBlock_Res.Text.ToString();
            //Si el último caracter del string no es un operador...
            String lastChar = sentence.Substring(sentence.Length - 1);
            
            if (!((lastChar.CompareTo("+")==0) || (lastChar.CompareTo("-")==0) ||
                (lastChar.CompareTo("*")==0) || (lastChar.CompareTo("/")==0) ) )
            {
                sentence = String.Concat(sentence, b.Content.ToString());
                index = sentence.Length;
            }
            textBlock_Res.Text = sentence;
            //
            History.Text = "Index = "+index;
        }


        private void clickOnC(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            Button b = (Button)fe;

            String sentence = textBlock_Res.Text.ToString();
            
            //Si se va a borrar un operador, retroceder el index
            if ((sentence.Substring(sentence.Length - 1).CompareTo("+") == 0) || (sentence.Substring(sentence.Length - 1).CompareTo("-") == 0)
                || (sentence.Substring(sentence.Length - 1).CompareTo("*") == 0) || (sentence.Substring(sentence.Length - 1).CompareTo("/") == 0) )
            {                
                //En caso de que Index no sea 0, se busca regresivamente el index más próximo al actual
                if (index > 0)
                {
                    for (int i = sentence.Length - 2; i >= 0; i--)
                    {                        
                        if ((sentence.Substring(i, 1).CompareTo("+") == 0) || (sentence.Substring(i, 1).CompareTo("-") == 0)
                            || (sentence.Substring(i, 1).CompareTo("*") == 0) || (sentence.Substring(i, 1).CompareTo("/") == 0) )
                        {
                            index = i + 1;                            
                            History.Text = "Index = " + index;
                            break;
                        }
                    }
                }
            }
            //Luego de actualizar el index, se procede a cambiar el texto en textView_Actual                        
            if ( sentence.Length > 1 )
            {
                sentence = sentence.Substring(0, sentence.Length - 1);
                textBlock_Res.Text = sentence;
                Boolean containsOperator = (sentence.Contains("+") || (sentence.Contains("-")) || (sentence.Contains("*")) || (sentence.Contains("/")));
                if(containsOperator == false)
                {
                    index = 0;
                }
                History.Text = "Index = " + index;
            }else{            
            
                textBlock_Res.Text = "0";
                History.Text = "Index = " + index;
            }
        }

        private void clickOnDot(object sender, RoutedEventArgs e)
        {
            String sentence = textBlock_Res.Text.ToString();

            if (!(sentence.Substring(index, sentence.Length - index).Contains(".")) )
            {
                sentence = String.Concat(sentence, ".");
            }
            textBlock_Res.Text = sentence;
        }

        private void clickOnEquals(object sender, RoutedEventArgs e)
        {
            if (verifyIntegrityOnExpression(textBlock_Res.Text.ToString()))
                executeOperations();
            else
                History.Text = "ERROR";
        }
        
        public Boolean verifyIntegrityOnExpression(String sentence)
        {
            numList.Clear();

            Boolean isOk = true;
            //Chequear si el último caractér es un operador
            if ((sentence.Substring(sentence.Length - 1).CompareTo("+") == 0) || (sentence.Substring(sentence.Length - 1).CompareTo("-") == 0)
                || (sentence.Substring(sentence.Length - 1).CompareTo("*") == 0) || (sentence.Substring(sentence.Length - 1).CompareTo("/") == 0))
            {
                isOk = false;
            }else{
                for (int i = 0; i < sentence.Length - 1; i++)
                {
                    for (int j = 1; j < sentence.Length; j++)
                    {
                        //Busco en sentence hasta que se encuentre un operador y obtengo el número antes del operador. No realiza la validación para la última cifra
                        if ((sentence.Substring(j, 1).CompareTo("+") == 0) || (sentence.Substring(j, 1).CompareTo("-") == 0)
                            || (sentence.Substring(j, 1).CompareTo("*") == 0) || (sentence.Substring(j, 1).CompareTo("/") == 0))
                        {
                            String numS = sentence.Substring(i,j-i);
                            Double num = Double.Parse(numS, CultureInfo.InvariantCulture);
                            numList.Add(num);

                            String ope = sentence.Substring(j, 1);
                            opeList.Add(ope);

                            if ((ope.CompareTo("*") == 0) || (ope.CompareTo("/") == 0))
                            {
                                opePriorList.Add(true);
                            }
                            else
                            {
                                opePriorList.Add(false);
                            }
                            i = j + 1;//Avanzo al siguiente operando
                        }
                        else if(j+1 == sentence.Length) //Caso de la ultima cifra de sentence
                        {
                            String numS = sentence.Substring(i, j - i + 1);
                            Double num = Double.Parse( numS, CultureInfo.InvariantCulture);
                            numList.Add(num);
                            
                            //Rompo los Ciclos for
                            i = sentence.Length; break;
                        }
                    }
                }
            }
            return isOk;
        }

        public void executeOperations()
        {
            //Salvo la expresión a ser evaluada en History
            History.Text = textBlock_Res.Text.ToString();
            Double res = 0.0;

            //Si el número de operandos salvados es mayor que 1...
            if (numList.Count > 1)
            {
                while (opeList.Count > 0)
                {
                    //Encuentra la primera operación prioritaria
                    int ind = opePriorList.IndexOf(true);

                    //Si hay al menos una operación de alta prioridad
                    if (ind != -1)
                    {
                        Double num1 = numList.ElementAt(ind);
                        Double num2 = numList.ElementAt(ind + 1);
                        String operador = opeList.ElementAt(ind);

                        switch (operador)
                        {
                            case "*":
                                res = num1 * num2;
                                break;
                            case "/":
                                res = num1 / num2;
                                break;
                        }
                    }
                    else//Si no hay operaciones de alta prioridad
                    {
                        ind = opePriorList.IndexOf(false);
                        Double num1 = numList.ElementAt(ind);
                        Double num2 = numList.ElementAt(ind+1);
                        String operador = opeList.ElementAt(ind);

                        switch (operador)
                        {
                            case "+":
                                res = num1 + num2;
                                break;
                            case "-":
                                res = num1 - num2;
                                break;
                        }
                    }
                    numList[ind] = res; numList.RemoveAt(ind + 1);
                    opeList.RemoveAt(ind);
                    opePriorList.RemoveAt(ind);
                }
                textBlock_Res.Text = "" + res;
            }

        }
        
        
    }
}