using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Простой калькулятор на C#");
        Console.Write("Введите первое число: ");
        double num1 = Convert.ToDouble(Console.ReadLine());
        Console.Write("Введите оператор (+, -, *, /): ");
        char op = Convert.ToChar(Console.ReadLine()); Console.Write("Введите второе число: ");
        double num2 = Convert.ToDouble(Console.ReadLine());

        double result = 0;
        bool validOperation = true;
        switch (op)
        {
            case '+':
                result = num1 + num2;
                break;
            case '-':
                result = num1 - num2;
                break;
            case '*':
                result = num1 * num2;
                break;
            case '/':
                if (num2 != 0)
                    result = num1 / num2;
                else
                {
                    Console.WriteLine("Ошибка: деление на ноль!");
                    validOperation = false;
                }
                break;
            default:
                Console.WriteLine("Ошибка: неверный оператор!");
                validOperation = false;
                break;
        }
        if (validOperation)
        {
            Console.WriteLine($"Результат: {num1} {op} {num2} = {result}");
        }
    }
}