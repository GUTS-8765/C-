using System;
using System.Diagnostics;

namespace Haynoskie;

class Program
{
    static long moves = 0;

    static void Main()
    {
        // Задание 7: проверка для n = 3
        moves = 0;
        Console.WriteLine("--- Последовательность ходов для 3 дисков ---");
        Hanoi(3, 'A', 'C', 'B');
        Console.WriteLine("Всего ходов: " + moves + "\n");

        // Задание 8: Замеры для n = 5, 10, 15, 20, 25
        Console.WriteLine("--- Замеры для разного количества дисков ---");
        int[] sizes = { 5, 10, 15, 20, 25 };
        foreach (int n in sizes)
        {
            moves = 0;
            var sw = Stopwatch.StartNew();
            Hanoi(n, 'A', 'C', 'B');
            sw.Stop();
            Console.WriteLine(n + " дисков: " + moves + " ходов, " + sw.ElapsedMilliseconds + " мс");
        }
    }

    static void Hanoi(int n, char from, char to, char spare)
    {
        if (n == 0) return;

        Hanoi(n - 1, from, spare, to);

        moves++;
        // Раскомментируйте строку ниже, если хотите видеть каждый ход (для Задания 7)
        // Console.WriteLine(from + " -> " + to); 

        Hanoi(n - 1, spare, to, from);
    }
}