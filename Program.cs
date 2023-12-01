namespace AoC2023;

using System;
using System.IO;

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

        int day = int.Parse(args[0]);
        int phase = int.Parse(args[1]);
        string datafile = args[2];

        switch (day)
        {
            case 1:
                Console.WriteLine(Day1(phase, datafile));
                break;
        }
    }
}
