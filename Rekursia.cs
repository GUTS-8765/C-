using System;
using System.Diagnostics;

namespace Rekurisa;

class Program
{
    static long calls = 0;

    static void Main()
    {
        int n = 35; 
        calls = 0;

        var sw = Stopwatch.StartNew();
        long naive = FibNaive(n);
        sw.Stop();
        long naiveMs = sw.ElapsedMilliseconds;
        long naiveCalls = calls;

        calls = 0;
        sw = Stopwatch.StartNew();
        long[] cache = new long[n + 1];
        long fast = FibMemo(n, cache);
        sw.Stop();

        Console.WriteLine(n + ": наивный " + naiveCalls + " вызовов, " + naiveMs + " мс" +
                          " | кэш: " + calls + " вызовов, " + sw.ElapsedMilliseconds + " мс" +
                          " | результат " + naive + " = " + fast);
    }

    static long FibNaive(int k)
    {
        calls++;
        if (k <= 1) return k;
        return FibNaive(k - 1) + FibNaive(k - 2);
    }

    static long FibMemo(int k, long[] cache)
    {
        calls++;
        if (k <= 1) return k;
        if (cache[k] != 0) return cache[k];
        cache[k] = FibMemo(k - 1, cache) + FibMemo(k - 2, cache);
        return cache[k];
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }
}