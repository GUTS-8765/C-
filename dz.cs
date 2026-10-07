using System;
public partial class Program
{
    private static object nameLength;

    static void Main()
    {
        Console.WriteLine("Введите ваше имя: ");
        string? name = Console.ReadLine();
        Console.WriteLine($"Привет, {name}!");
        string curretnDate = DateTime.Now.ToString("dd,MM,yyyy");
        Console.WriteLine($"Сегодня на календаре: { curretnDate}");
        Console.WriteLine($"В вашем имени {name.Length} символов.");
    }
}
