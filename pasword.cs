using System;
using System.Collections.Generic;
using System.Security.Cryptography;

// ================== Данные ==================

// Список популярных паролей (из открытых утечек)
string[] popularPasswords =
{
    "123456", "password", "123456789", "12345678", "12345",
    "1234567", "qwerty", "abc123", "football", "monkey",
    "letmein", "111111", "mustang", "access", "shadow",
    "master", "michael", "superman", "696969", "batman",
    "iloveyou", "dragon", "sunshine", "princess", "admin",
    "welcome", "login", "passw0rd", "hello", "charlie"
};

// Словарь для парольных фраз (128 слов — тема «повседневное вокруг»)
string[] phraseWords =
{
    "кот", "пёс", "лиса", "волк", "медведь", "заяц", "ёж", "сова",
    "орёл", "тигр", "лев", "слон", "жираф", "зебра", "крот", "бобр",
    "выдра", "ласка", "куница", "рысь", "гепард", "леопард", "пума", "ягуар",

    "лес", "река", "гора", "море", "озеро", "поле", "луг", "сад",
    "небо", "солнце", "луна", "звезда", "облако", "дождь", "снег", "ветер",
    "гроза", "радуга", "рассвет", "закат", "туман", "иней", "лёд", "пламя",

    "дом", "окно", "дверь", "стол", "стул", "кровать", "шкаф", "полка",
    "лампа", "свеча", "книга", "тетрадь", "ручка", "карандаш", "кисть", "краска",
    "чашка", "ложка", "вилка", "нож", "тарелка", "кастрюля", "чайник", "сковорода",

    "хлеб", "сыр", "молоко", "масло", "мёд", "яблоко", "груша", "слива",
    "вишня", "малина", "смородина", "арбуз", "дыня", "виноград", "банан", "апельсин",
    "лимон", "персик", "абрикос", "гранат", "инжир", "финик", "орех", "каштан",

    "поезд", "самолёт", "корабль", "лодка", "машина", "автобус", "трамвай", "метро",
    "велосипед", "мотоцикл", "ракета", "вертолёт", "паром", "яхта", "катер", "троллейбус",

    "дружба", "радость", "печаль", "надежда", "мечта", "память", "мысль", "слово",
    "песня", "танец", "музыка", "картина", "история", "сказка", "загадка", "шутка"
};

// ================== Основной цикл ==================

while (true)
{
    PrintMenu();
    string choice = Console.ReadLine() ?? "";

    switch (choice)
    {
        case "1":
            CheckPasswordInteractive(popularPasswords);
            break;
        case "2":
            GeneratePasswordInteractive();
            break;
        case "3":
            GeneratePassphraseInteractive(phraseWords);
            break;
        case "4":
            ShowCrackTimeExamples();
            break;
        case "0":
            Console.WriteLine("До свидания!");
            return;
        default:
            Console.WriteLine("Неверный пункт меню. Попробуйте снова.");
            break;
    }
}

// ================== Меню и вспомогательный ввод ==================

static void PrintMenu()
{
    Console.WriteLine();
    Console.WriteLine("===== Пароли: проверка и генерация =====");
    Console.WriteLine("1 — проверить пароль");
    Console.WriteLine("2 — сгенерировать пароль");
    Console.WriteLine("3 — сгенерировать парольную фразу");
    Console.WriteLine("4 — примеры времени перебора (шаг 3)");
    Console.WriteLine("0 — выход");
    Console.Write("Выбор: ");
}

static int ReadInt(string prompt, int min, int max)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int v) && v >= min && v <= max)
            return v;
        Console.WriteLine($"Введите целое число от {min} до {max}.");
    }
}

static bool ReadYesNo(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string s = (Console.ReadLine() ?? "").Trim().ToLower();
        if (s == "y" || s == "yes" || s == "д" || s == "да") return true;
        if (s == "n" || s == "no" || s == "н" || s == "нет") return false;
        Console.WriteLine("Введите y или n.");
    }
}

// ================== Шаг 1. Размер алфавита ==================

static int GetAlphabetSize(string password)
{
    bool lower = false, upper = false, digit = false, other = false, nonAscii = false;

    foreach (char c in password)
    {
        if (c >= 'a' && c <= 'z') lower = true;
        else if (c >= 'A' && c <= 'Z') upper = true;
        else if (c >= '0' && c <= '9') digit = true;
        else if (c >= ' ' && c <= '~') other = true;   // остальные печатные ASCII
        else nonAscii = true;                          // вне таблицы ASCII
    }

    int size = 0;
    if (lower) size += 26;
    if (upper) size += 26;
    if (digit) size += 10;
    if (other) size += 33;
    if (nonAscii) size += 66;

    return size;
}

// ================== Шаг 2. Стойкость и оценка ==================

static double CalculateEntropyBits(int length, int alphabetSize)
{
    if (length <= 0 || alphabetSize <= 1) return 0;
    return length * Math.Log2(alphabetSize);
}

static string GetStrengthLabel(double bits)
{
    if (bits < 28) return "очень слабый";
    if (bits < 36) return "слабый";
    if (bits < 60) return "средний";
    if (bits < 128) return "сильный";
    return "очень сильный";
}

// ================== Шаг 3. Время перебора ==================

// По умолчанию — быстрая хеш-функция: 10 миллиардов вариантов в секунду.
// Для медленной (bcrypt/Argon2) передаём 10_000 — в миллион раз меньше.
static double CalculateAverageCrackSeconds(double bits, double speed = 10_000_000_000)
{
    double variants = Math.Pow(2, bits);
    return variants / 2 / speed;   // в среднем — половина вариантов
}

static string FormatTime(double seconds)
{
    const double universeAgeYears = 13_800_000_000;

    if (seconds < 1) return "меньше секунды";
    if (seconds < 60) return $"{seconds:F1} сек.";

    double minutes = seconds / 60;
    if (minutes < 60) return $"{minutes:F1} мин.";

    double hours = minutes / 60;
    if (hours < 24) return $"{hours:F1} ч.";

    double days = hours / 24;
    if (days < 365.25) return $"{days:F1} дн.";

    double years = days / 365.25;
    if (years > universeAgeYears) return "больше возраста Вселенной";
    if (years > 1_000_000_000) return $"{years / 1_000_000_000:F1} млрд лет";
    if (years > 1_000_000) return $"{years / 1_000_000:F1} млн лет";
    if (years > 1_000) return $"{years / 1_000:F1} тыс. лет";
    return $"{years:F1} лет";
}

// ================== Шаг 4. Словарь и шаблоны ==================

static bool IsPopularPassword(string password, string[] popular)
{
    string lower = password.ToLower();
    foreach (string p in popular)
    {
        if (p.ToLower() == lower) return true;
    }
    return false;
}

// Ловит пароли, «придуманные человеком»: один и тот же символ,
// известные последовательности, популярное слово с цифрами в конце.
static bool HasWeakPattern(string password, string[] popular)
{
    if (password.Length == 0) return false;

    // 1) все символы одинаковые
    bool allSame = true;
    for (int i = 1; i < password.Length; i++)
    {
        if (password[i] != password[0]) { allSame = false; break; }
    }
    if (allSame) return true;

    // 2) известные последовательности
    string low = password.ToLower();
    if (low.Contains("12345")) return true;
    if (low.Contains("abcde")) return true;
    if (low.Contains("qwerty")) return true;

    // 3) популярное слово + цифры в конце (например, Password123)
    int end = password.Length;
    while (end > 0 && password[end - 1] >= '0' && password[end - 1] <= '9') end--;
    if (end < password.Length && end > 0)
    {
        string word = password.Substring(0, end);
        if (IsPopularPassword(word, popular)) return true;
    }

    return false;
}

// ================== Шаг 5. Генератор пароля ==================

static string GeneratePassword(int length, bool useLower, bool useUpper, bool useDigits, bool useSpecial)
{
    const string lowerSet = "abcdefghijklmnopqrstuvwxyz";
    const string upperSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string digitSet = "0123456789";
    const string specialSet = "!@#$%^&*()-_=+[]{};:,.?/\\|`~ \"'<>";

    var groups = new List<string>();
    if (useLower) groups.Add(lowerSet);
    if (useUpper) groups.Add(upperSet);
    if (useDigits) groups.Add(digitSet);
    if (useSpecial) groups.Add(specialSet);

    if (groups.Count == 0)
        throw new ArgumentException("Нужно выбрать хотя бы одну группу символов.");
    if (length < groups.Count)
        throw new ArgumentException("Длина меньше числа выбранных групп.");

    char[] chars = new char[length];
    int pos = 0;

    // По одному обязательному символу из каждой выбранной группы
    foreach (string group in groups)
    {
        chars[pos++] = group[RandomNumberGenerator.GetInt32(group.Length)];
    }

    // Остальные символы — из общего алфавита
    string all = string.Concat(groups);
    while (pos < length)
    {
        chars[pos++] = all[RandomNumberGenerator.GetInt32(all.Length)];
    }

    // Обязательные символы не должны всегда быть в начале
    Shuffle(chars);
    return new string(chars);
}

// Алгоритм Фишера — Йетса: каждая перестановка равновероятна
static void Shuffle(char[] chars)
{
    for (int i = chars.Length - 1; i > 0; i--)
    {
        int j = RandomNumberGenerator.GetInt32(i + 1); // 0..i
        (chars[i], chars[j]) = (chars[j], chars[i]);
    }
}

// ================== Шаг 6. Парольная фраза ==================

static string GeneratePassphrase(string[] words, int count)
{
    string[] chosen = new string[count];
    for (int i = 0; i < count; i++)
    {
        chosen[i] = words[RandomNumberGenerator.GetInt32(words.Length)];
    }
    return string.Join("-", chosen);
}

static double CalculatePhraseBits(int wordCount, int dictionarySize)
{
    if (dictionarySize <= 1) return 0;
    return wordCount * Math.Log2(dictionarySize);
}

// ================== Интерактив: проверка пароля ==================

static void CheckPasswordInteractive(string[] popular)
{
    Console.Write("Введите пароль: ");
    string password = Console.ReadLine() ?? "";

    if (password.Length == 0)
    {
        Console.WriteLine("Пароль не введён.");
        return;
    }

    int alphabet = GetAlphabetSize(password);
    double bits = CalculateEntropyBits(password.Length, alphabet);

    Console.WriteLine();
    Console.WriteLine($"Длина:            {password.Length}");
    Console.WriteLine($"Размер алфавита:  {alphabet}");
    Console.WriteLine($"Стойкость:        {bits:F1} бит");

    // Словарная атака: популярные пароли подбираются мгновенно
    if (IsPopularPassword(password, popular))
    {
        PrintColored("Пароль в списке популярных — подбирается мгновенно!", ConsoleColor.Red);
        return;
    }

    string label;
    if (HasWeakPattern(password, popular))
    {
        label = "слабый";
        PrintColored("Обнаружен слабый шаблон — оценка понижена.", ConsoleColor.Yellow);
    }
    else
    {
        label = GetStrengthLabel(bits);
    }

    PrintColored($"Оценка: {label}", GetLabelColor(label));

    double secondsFast = CalculateAverageCrackSeconds(bits);            // 10^10 в сек
    double secondsSlow = CalculateAverageCrackSeconds(bits, 10_000);    // bcrypt/Argon2
    Console.WriteLine($"Среднее время перебора (быстрая хеш-функция): {FormatTime(secondsFast)}");
    Console.WriteLine($"Среднее время перебора (bcrypt/Argon2):       {FormatTime(secondsSlow)}");
}

static ConsoleColor GetLabelColor(string label) => label switch
{
    "очень слабый" => ConsoleColor.Red,
    "слабый" => ConsoleColor.Red,
    "средний" => ConsoleColor.Yellow,
    "сильный" => ConsoleColor.Green,
    "очень сильный" => ConsoleColor.Green,
    _ => ConsoleColor.Gray
};

static void PrintColored(string text, ConsoleColor color)
{
    ConsoleColor old = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ForegroundColor = old;
}

// ================== Интерактив: генерация пароля ==================

static void GeneratePasswordInteractive()
{
    const int minLen = 8;
    const int maxLen = 64;

    int length = ReadInt($"Длина пароля ({minLen}-{maxLen}): ", minLen, maxLen);

    bool lower = ReadYesNo("Строчные латинские? (y/n): ");
    bool upper = ReadYesNo("Заглавные латинские? (y/n): ");
    bool digits = ReadYesNo("Цифры? (y/n): ");
    bool special = ReadYesNo("Спецсимволы? (y/n): ");

    if (!lower && !upper && !digits && !special)
    {
        Console.WriteLine("Не выбрано ни одной группы символов.");
        return;
    }

    int groupCount = (lower ? 1 : 0) + (upper ? 1 : 0) + (digits ? 1 : 0) + (special ? 1 : 0);
    if (length < groupCount)
    {
        Console.WriteLine($"Длина должна быть не меньше числа выбранных групп ({groupCount}).");
        return;
    }

    Console.Write("Сколько паролей сгенерировать (1-20)? ");
    if (!int.TryParse(Console.ReadLine(), out int count) || count < 1 || count > 20)
        count = 1;

    for (int i = 0; i < count; i++)
    {
        string pwd = GeneratePassword(length, lower, upper, digits, special);
        int alphabet = GetAlphabetSize(pwd);
        double bits = CalculateEntropyBits(pwd.Length, alphabet);
        double seconds = CalculateAverageCrackSeconds(bits);

        Console.WriteLine();
        Console.WriteLine($"#{i + 1}: {pwd}");
        Console.WriteLine($"   Длина {pwd.Length}, алфавит {alphabet}, стойкость {bits:F1} бит");
        Console.WriteLine($"   Среднее время перебора: {FormatTime(seconds)}");
        PrintColored($"   Оценка: {GetStrengthLabel(bits)}", GetLabelColor(GetStrengthLabel(bits)));
    }
}

// ================== Интерактив: парольная фраза ==================

static void GeneratePassphraseInteractive(string[] words)
{
    int wordCount = ReadInt("Сколько слов во фразе (3-10): ", 3, 10);

    Console.Write("Сколько фраз сгенерировать (1-20)? ");
    if (!int.TryParse(Console.ReadLine(), out int count) || count < 1 || count > 20)
        count = 1;

    double perWord = Math.Log2(words.Length);

    Console.WriteLine();
    Console.WriteLine($"Словарь: {words.Length} слов, каждое слово даёт {perWord:F2} бит.");

    for (int i = 0; i < count; i++)
    {
        string phrase = GeneratePassphrase(words, wordCount);
        double bits = CalculatePhraseBits(wordCount, words.Length);
        double seconds = CalculateAverageCrackSeconds(bits);

        Console.WriteLine();
        Console.WriteLine($"#{i + 1}: {phrase}");
        Console.WriteLine($"   Стойкость {bits:F1} бит, время перебора: {FormatTime(seconds)}");
    }

    // Сколько слов нужно, чтобы догнать 12-символьный случайный пароль (71,5 бит)
    const double targetBits = 71.5;
    int forOurs = (int)Math.Ceiling(targetBits / perWord);
    int forDice = (int)Math.Ceiling(targetBits / Math.Log2(7776));

    Console.WriteLine();
    Console.WriteLine($"Для {targetBits} бит:");
    Console.WriteLine($"  в нашем словаре ({words.Length} слов) нужно {forOurs} слов");
    Console.WriteLine($"  в Diceware (7776 слов)              нужно {forDice} слов");
    Console.WriteLine("Вывод: чем больше словарь, тем меньше слов нужно для той же стойкости.");
}

// ================== Шаг 3 в наглядном виде ==================

static void ShowCrackTimeExamples()
{
    Console.WriteLine();
    Console.WriteLine("--- Примеры из теории ---");
    Demonstrate("8 цифр", "12345678");
    Demonstrate("8 строчных латинских", "abcdefgh");
    Demonstrate("8 латиница + цифры", "Abcd1234");
    Demonstrate("12 латиница + цифры", "Abcdef123456");
    Demonstrate("16 символов всех групп", "Abcd1234!@#$xyz9");

    Console.WriteLine();
    Console.WriteLine("--- Влияние медленной хеш-функции на 8-символьный пароль ---");
    string pwd = "Abcd1234";
    int alpha = GetAlphabetSize(pwd);
    double bits = CalculateEntropyBits(pwd.Length, alpha);
    Console.WriteLine($"Пароль: {pwd}, {bits:F1} бит");
    Console.WriteLine($"Быстрая хеш-функция:  {FormatTime(CalculateAverageCrackSeconds(bits))}");
    Console.WriteLine($"bcrypt/Argon2:        {FormatTime(CalculateAverageCrackSeconds(bits, 10_000))}");
    Console.WriteLine("Вывод: даже короткий пароль при медленной функции живёт в тысячи раз дольше.");
}

static void Demonstrate(string label, string password)
{
    int alpha = GetAlphabetSize(password);
    double bits = CalculateEntropyBits(password.Length, alpha);
    double seconds = CalculateAverageCrackSeconds(bits);
    Console.WriteLine($"{label,-28} алфавит {alpha,3}, стойкость {bits,6:F1} бит, перебор: {FormatTime(seconds)}");
}
