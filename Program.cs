namespace AoC2023;

using System;
using System.IO;
using System.Diagnostics;

partial class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 3)    //argumenttien pitäisi olla [päivä] [kohta] [datatiedosto], heitä virhe jos niitä on väärä määrä
        {
            Console.WriteLine("Anna argumentit: päivä kohta datatiedosto");
            return;
        }

        //Virheenhallinta olkoon vähän niin ja näin, koska emmehän tee virheitä.

        var timer = new Stopwatch();
        timer.Start();

        int day = int.Parse(args[0]);
        int phase = int.Parse(args[1]);
        string datafile = args[2];

        switch (day)
        {
            case 1:
                Console.WriteLine(Day1(phase, datafile));
                break;
            case 2:
                Console.WriteLine(Day2(phase, datafile));
                break;
            case 3:
                Console.WriteLine(Day3(phase, datafile));
                break;
        }

        timer.Stop();
        var runtime = timer.Elapsed;
        Console.WriteLine($"Aikaa meni: {runtime}");
    }
}
