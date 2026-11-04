using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Выберите действие:");
        Console.WriteLine("1 — Показать приветствие");
        Console.WriteLine("2 — Показать текущее время");
        Console.WriteLine("3 — Покажи рекурсию");
        Console.WriteLine("4 - Выйти");
        Console.Write("\nВаш выбор: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.WriteLine("Привет, пользователь!");
                break;
            case "2":
                Console.WriteLine("Текущее время: ");
                break;
            case "3":
                Console.WriteLine("Рекурсия.");
                break;
            case "4":
                Console.WriteLine("Выход из программы.");
                break;
            default:
                Console.WriteLine("Ошибка: неверный ввод. Выберите от 1 до 3.");
                break;
        }

        Console.WriteLine("\nНажмите любую клавишу для завершения...");
        Console.ReadKey();

    }
}