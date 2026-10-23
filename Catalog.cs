using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

class program
{
    static List<string> names = new List<string>();
    static List<int> years = new List<int>();
    static List<double> rating = new List<double>();
    static List<bool> done = new List<bool>();
    static List<string> genres = new List<string>();
    private static object ratings;
    const string FILE = "catalog.csv";
    const char SEP = ';';
    
    static object Main()
    {
        Load();

        while (true) 
        {
            Console.WriteLine();
            Console.WriteLine("==CATALOG GTAMES==");
            Console.WriteLine("==1.See games==");
            Console.WriteLine("==2.Add recording==");
            Console.WriteLine("==3.Search in Name==");
            Console.WriteLine("==4.Filter in Rating==");
            Console.WriteLine("==5.Top 3 is Rating==");
            Console.WriteLine("==6.Statistics");
            Console.WriteLine("==0.Save and Exite");

            string cmd = Console.ReadLine();
            switch (cmd)
            {
                case "1": ShowAll(); break;
                case "2": AddRecord(); break;
                case "3": Search(); break;
                case "4": Filter(); break;
                case "5": Top3(); break;
                case "6": Stats(); break;
                case "0":
                    Save();
                    Console.WriteLine("Сохранено. Пока!");
                    return;
                default:
                    Console.WriteLine("Нет такого пункта.");
                    break;
            }
        }
    }

    static void PrintHeader()
    {
        Console.WriteLine("{0,-25} {1,6} {2,8: F1} {3,9} {4,-12}",
            "Название", "Год", "Рейтинг", "Пройдено", "Жанр");
        Console.WriteLine(new string('-', 65));
    }
    static void PrintRow(int i)
    {
        Console.WriteLine("{0,-25} {1,6} {2,8:F1} {3,9} {4,-12}",
            names[i], years[i], ratings[i],
            done[i] ? "да" : "нет", genres[i]);
    }
    static void ShowAll()
    {
        if (names.Count == 0) { Console.WriteLine("Каталог пуст."); return; }
        PrintHeader();
        for (int i = 0; i < names.Count; i++) PrintRow(i);
    }
    static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int v)) return v;
            Console.WriteLine("Это не целое число. Ещё раз.");
        }
    }

    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (double.TryParse(Console.ReadLine(), out double v)) return v;
            Console.WriteLine("Это не число. Ещё раз.");
        }
    }

    static bool ReadBool(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " (да/нет): ");
            string s = Console.ReadLine().Trim().ToLower();
            if (s == "да" || s == "yes" || s == "y" || s == "1") return true;
            if (s == "нет" || s == "no" || s == "n" || s == "0") return false;
            Console.WriteLine("Ответьте «да» или «нет».");
        }
    }

    static string ReadName(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string s = Console.ReadLine().Trim();
            if (s.Length == 0) { Console.WriteLine("Пустое название не годится."); continue; }
            if (s.Contains(SEP)) 
            {
                Console.WriteLine("Название не должно содержать '" + SEP + "'.");
                continue;
            }
            return s;
        }
    }
    static void AddRecord()
    {
        string n = ReadName("Название: ");
        int y = ReadInt("Год выпуска: ");
        double r = ReadDouble("Рейтинг (например 9,1): ");
        bool d = ReadBool("Пройдено?");
        Console.Write("Жанр: ");
        string g = Console.ReadLine().Trim();

        names.Add(n); years.Add(y); ratings.Add(r); done.Add(d); genres.Add(g);
        Console.WriteLine("Запись добавлена.");
    }
    static void Search()
    {
        Console.Write("Часть названия: ");
        string q = Console.ReadLine().Trim().ToLower();
        bool found = false;
        PrintHeader();
        for (int i = 0; i < names.Count; i++)
        {
            if (names[i].ToLower().Contains(q))
            {
                PrintRow(i);
                found = true;
            }
        }
        if (!found) Console.WriteLine("Ничего не найдено.");
    }
    static void Filter()
    {
        double min = ReadDouble("Рейтинг больше чем: ");
        bool found = false;
        PrintHeader();
        for (int i = 0; i < names.Count; i++)
        {
            if (ratings[i] > min) { PrintRow(i); found = true; }
        }
        if (!found) Console.WriteLine("Подходящих записей нет.");
    }
    static void Top3()
    {
        if (names.Count == 0) { Console.WriteLine("Каталог пуст."); return; }

        var idx = Enumerable.Range(0, names.Count).ToList();
        idx.Sort((a, b) => ratings[b].CompareTo(ratings[a])); 

        PrintHeader();
        int top = Math.Min(3, idx.Count);
        for (int k = 0; k < top; k++) PrintRow(idx[k]);
    }
    static void Stats()
    {
        int cnt = names.Count;
        if (cnt == 0) { Console.WriteLine("Каталог пуст."); return; }

        int sum = 0, yes = 0;
        for (int i = 0; i < cnt; i++)
        {
            sum += years[i];
            if (done[i]) yes++;
        }
        double avgYear = (double)sum / cnt; 
        double pctDone = 100.0 * yes / cnt;

        Console.WriteLine("Всего записей: {0}", cnt);
        Console.WriteLine("Средний год выпуска: {0:F2}", avgYear);
        Console.WriteLine("Пройдено: {0} из {1} ({2:F1}%)", yes, cnt, pctDone);
    }
    static void Save()
    {
        var lines = new List<string>();
        for (int i = 0; i < names.Count; i++)
        {
            lines.Add(string.Join(SEP.ToString(),
                names[i], years[i], ratings[i], done[i] ? "да" : "нет", genres[i]));
        }
        File.WriteAllLines(FILE, lines);
    }

    static void Load()
    {
        if (!File.Exists(FILE))
        {
            Console.WriteLine("Файл не найден — начинаем с пустого каталога.");
            return;
        }

        string[] lines = File.ReadAllLines(FILE);
        int ok = 0, bad = 0;
        foreach (string line in lines)
        {
            if (line.Trim().Length == 0) continue;
            string[] p = line.Split(SEP);
            if (p.Length != 5) { bad++; continue; }

            if (!int.TryParse(p[1], out int y) ||
                !double.TryParse(p[2], out double r))
            {
                bad++;
                continue;
            }

            bool d = p[3].Trim().ToLower() == "да";
            names.Add(p[0]); years.Add(y); ratings.Add(r);
            done.Add(d); genres.Add(p[4]);
            ok++;
        }
        Console.WriteLine("Загружено записей: {0}, пропущено битых: {1}", ok, bad);
        }
    }
