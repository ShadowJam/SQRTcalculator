using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//SQRTcalculator Main file ver1.2
public class Manager : MonoBehaviour {


	//объявление основных переменных
	public VerticalLayoutGroup buttonGroup;
	public HorizontalLayoutGroup bottomRow;
	public RectTransform canvasRect;
	CalcButton[] bottomButtons;

	public Text digitLabel;
	public Text operatorLabel;
	bool errorDisplayed;
	bool displayValid;
	bool specialAction;
	double currentVal;
	double storedVal;
	double result;
	char storedOperator;
	int ccoma;

	bool canvasChanged;


	//далее функции
	private void Awake() //для интерфейса
	{
		bottomButtons = bottomRow.GetComponentsInChildren<CalcButton>();
	}


	// Use this for initialization
	void Start () {
		bottomRow.childControlWidth = false;
		canvasChanged = true;
		buttonTapped('c');
	}
	
	// Update is called once per frame
	void Update () {
		if (canvasChanged)
		{
			canvasChanged = false;
			adjustButtons();
		}
		
	}

	private void OnRectTransformDimensionsChange() //для интерфейса
	{
		canvasChanged = true;
	}

	void adjustButtons() //функция для объединения кнопок 0 //для интерфейса
	{
		if (bottomButtons == null || bottomButtons.Length == 0)
			return;
		float buttonSize = canvasRect.sizeDelta.x / 4;
		float bWidth = buttonSize - bottomRow.spacing;
		for (int i = 1; i < bottomButtons.Length;i++)
		{
			bottomButtons[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
																	 bWidth);
		}
		bottomButtons[0].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
																 bWidth * 2 + bottomRow.spacing);
	}

	void clearCalc() //очистка поля ввода вывода
	{
		digitLabel.text = "0";
		operatorLabel.text = "";
		specialAction = displayValid = errorDisplayed = false;
		currentVal = result = storedVal = 0;
		storedOperator = ' ';
		ccoma = 0;
	}
	void updateDigitLabel() //обновление поля ввода вывода 
	{
		if (!errorDisplayed)
			digitLabel.text = currentVal.ToString();
		displayValid = false;
		ccoma = 0;
	}
	/*
	public static double mabs(double x) { return (x < 0) ? -x : x; }
	public static double squareDots(double sV, double cV) //число под корнем, "точность"
    {
		int cVv = (int)System.Math.Round(cV);


		double eps = System.Math.Pow(0.1, cVv);             //допустимая погрешность
		double root = sV / 2;								//начальное приближение корня
		double rn = sV;										//значение корня последовательным делением
		while (mabs(root - rn) >= eps)
		{
			rn = sV;
			for (int i = 1; i < 2; i++)
			{
				rn = rn / root;
			}
			root = 0.5 * (rn + root);
		}
		return root; //результат
	}
	*/
	public static double squareDots(double sV, double cV) // функция квадратного корня с задаваемой точностью (число под корнем, "точность")
	{
		int cVv = (int)System.Math.Round(cV);

		double root = sV / 2;                               //начальное приближение корня
		double rn = sV;                                     //значение корня последовательным делением
		if (cVv <= 0)
			root = 0;
		else
			for (int j = 1; j <=cVv; j++)
			{
				rn = sV;
				for (int i = 1; i < 2; i++)
				{
					rn = rn / root;
				}
				root = 0.5 * (rn + root);
			}
		return root; //результат
	}

	void calcResult(char activeOp) //операторы
	{
		switch (activeOp)
		{
			case '=':
				result = currentVal;
				break;
			case '+':
				result = storedVal + currentVal;
				break;
			case '-':
				result = storedVal - currentVal;
				break;
			case 'x':
				result = storedVal * currentVal;
				break;
            case '^':
				result = System.Math.Pow(storedVal, currentVal);
				break;
			case '√': //квадратный корень с задаваемой точностью
				result = squareDots(storedVal, currentVal);
				break;
			case '÷':
				if (currentVal!=0)
				{
					result = storedVal / currentVal;
				}
				else //деление на 0
				{
					errorDisplayed = true;
					digitLabel.text = "ERROR";
				}
				break;
			default:
				Debug.Log("unknown: " + activeOp);
				break;
		}
		currentVal = result;
		updateDigitLabel();
		if (currentVal > System.Math.Pow(10, 300)) //определение СЛИШКОМ большого числа
		{
			errorDisplayed = true;
			digitLabel.text = ">10^300";
		}
		ccoma = 0;
	}

	public void buttonTapped(char caption) //нажатие на кнопку
	{
		if (errorDisplayed)
			clearCalc();

		if ((caption>='0' && caption<='9')||caption==',') //1-9 и ','
		{
			if (caption != ',' || ccoma <1)
			{
				if (digitLabel.text.Length < 25 || !displayValid)
				{
					if (!displayValid)
						digitLabel.text = (caption == ',' ? "0" : "");
					else if (digitLabel.text == "0" && caption != ',')
						digitLabel.text = "";
					digitLabel.text += caption;
					displayValid = true;
				}
			}
			if (caption == ',')
				ccoma++;

		}
		else if (caption=='c')
		{
			clearCalc();
		}
        else if (caption == 'π')
        {
            if (double.Parse(digitLabel.text) == 0)
                currentVal = System.Math.PI;
            else
                currentVal = double.Parse(digitLabel.text) * System.Math.PI;
            updateDigitLabel();
			specialAction = true;
        }
        else if (caption == 'e')
        {
            if (double.Parse(digitLabel.text) == 0)
                currentVal = System.Math.E;
            else
                currentVal = double.Parse(digitLabel.text) * System.Math.E;
            updateDigitLabel();
			specialAction = true;
        }
		else if (caption == '±')
		{
			currentVal = -double.Parse(digitLabel.text);
			updateDigitLabel();
			specialAction = true;
		}
		else if (caption == '%')
		{
			currentVal = double.Parse(digitLabel.text) / 100.0d;
			updateDigitLabel();
			specialAction = true;
		}
		else if (caption == '&') //обычный квадратный корень
        {
            if (double.Parse(digitLabel.text) >= 0)  //число положительное
			{
                currentVal = System.Math.Sqrt(double.Parse(digitLabel.text)); 
                updateDigitLabel();
				digitLabel.text = "±" + digitLabel.text;

				specialAction = true;
            }
            else if (double.Parse(digitLabel.text) < 0) //число отрицательное, вывод комплексного вида
			{
				currentVal = System.Math.Sqrt(System.Math.Abs(double.Parse(digitLabel.text)));
				updateDigitLabel();
				digitLabel.text = "±" + digitLabel.text + "i";
			}
		}
        else if (caption == '∿') //sin
		{
            currentVal = System.Math.Sin(double.Parse(digitLabel.text));
			if (currentVal < 0.0000001)
			{
				currentVal = 0;
			}
            updateDigitLabel();
            specialAction = true;
        }
        else if (caption == '∽') //cos
		{
            currentVal = System.Math.Cos(double.Parse(digitLabel.text));
			if (currentVal < 0.0000001)
			{
				currentVal = 0;
			}
			updateDigitLabel();
            specialAction = true;
        }
        else if (caption == '≀') //tang
		{
            currentVal = System.Math.Tan(double.Parse(digitLabel.text));
			if (currentVal < 0.0000001)
			{
				currentVal = 0;
			}
			updateDigitLabel();
            specialAction = true;
        }
		else if (displayValid || storedOperator == '=' || specialAction)
		{
			currentVal = double.Parse(digitLabel.text);
			displayValid = false;
			if (storedOperator!=' ')
			{
				calcResult(storedOperator);
				storedOperator = ' ';
			}
			operatorLabel.text = caption.ToString();
			storedOperator = caption;
			storedVal = currentVal;
			updateDigitLabel();
			specialAction = false;
			ccoma = 0;
		}
	}
}