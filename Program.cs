namespace AoC2023;

using System;
using System.IO;
using System.Diagnostics;
using System.Numerics;

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
        BigInteger result = -1;
        timer.Start();
        switch (day)
        {
            case 1:
                result = (BigInteger)Day1(phase, datafile);
                break;
            case 2:
                result = (BigInteger)Day2(phase, datafile);
                break;
            case 3:
                result = (BigInteger)Day3(phase, datafile);
                break;
            case 4:
                result = (BigInteger)Day4(phase, datafile);
                break;
            case 5:
                result = (BigInteger)Day5(phase, datafile);
                break;
            case 6:
                result = (BigInteger)Day6(phase, datafile);
                break;
            case 7:
                result = (BigInteger)Day7(phase, datafile);
                break;
            case 8:
                result = Day8(phase, datafile);
                break;
            case 9:
                result = Day9(phase, datafile);
                break;
            case 10:
                result = Day10(phase, datafile);
                break;
            case 11:
                result = Day11(phase, datafile);
                break;
            case 12:
                result = Day12(phase, datafile);
                break;
            case 13:
                result = Day13(phase, datafile);
                break;
            case 14:
                result = Day14(phase, datafile);
                break;
            case 15:
                result = Day15(phase, datafile);
                break;
            case 16:
                result = Day16(phase, datafile);
                break;
            case 17:
                result = Day17(phase, datafile);
                break;
            case 18:
                result = Day18(phase, datafile);
                break;
            case 19:
                result = Day19(phase, datafile);
                break;
            case 20:
                result = Day20(phase, datafile);
                break;
            case 21:
                result = Day21(phase, datafile);
                break;
            case 22:
                result = Day22(phase, datafile);
                break;

        }

        timer.Stop();
        Console.WriteLine(result);
        var runtime = timer.Elapsed;
        Console.WriteLine($"Aikaa meni: {runtime}");
    }
}
