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
        

        int day = int.Parse(args[0]);
        int phase = int.Parse(args[1]);
        string datafile = args[2];
        int result = -1;
        timer.Start();
        switch (day)
        {
            case 1:
                result = Day1(phase, datafile);
                break;
            case 2:
                result = Day2(phase, datafile);
                break;
            case 3:
                result = Day3(phase, datafile);
                break;
            case 4:
                result = Day4(phase, datafile);
                break;
            case 5:
                result = Day5(phase, datafile);
                break;
            case 6:
                result = Day6(phase, datafile);
                break;
        }

        timer.Stop();
        Console.WriteLine(result);
        var runtime = timer.Elapsed;
        Console.WriteLine($"Aikaa meni: {runtime}");
    }
}
