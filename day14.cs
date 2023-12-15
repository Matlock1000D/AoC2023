namespace AoC2023;

using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Xml;

partial class Program
{
    
    static List<string> FlipPatternHor(List<string>orignalpattern)
    {
        var newpattern = new List<string>();
        foreach (var line in orignalpattern)
        {
            newpattern.Add(new string(line.ToCharArray().Reverse().ToArray()));
        }
        return newpattern;
    }

    static List<string> MoveStones(List<string> pattern)
    {
        var newpattern = new List<string>();
        //move stones to the left side of the pattern
        foreach (var origline in pattern)
        {
            string line = origline + '#';
            List<int> fixedrocks = [-1];
            var raipe = line.Select((c, i) => c == '#' ? i : -1).Where(i => i != -1).ToList();
            fixedrocks = fixedrocks.Concat(line.Select((c, i) => c == '#' ? i : -1).Where(i => i != -1).ToList()).ToList();

            var newline = new StringBuilder();
            for (int i=0; i<fixedrocks.Count-1;i++)
            {
                var subline = line[(fixedrocks[i]+1)..fixedrocks[i+1]];
                int rocks = subline.Count(x => x == 'O');
                string newpart = new string('O',rocks) + new string('.',fixedrocks[i+1]-fixedrocks[i]-rocks-1) + '#';
                newline.Append(newpart);
            }
            newpattern.Add(newline.ToString()[..^1]);
        }
        return newpattern;
    }
    static BigInteger Day14(int phase, string datafile)
    {
        string[] lines = [];
        BigInteger result = 0;
        var originalresults = new List<int>();

        if (File.Exists(datafile))
        {
            try
            {
                // Luetaan kaikki rivit
                lines = File.ReadAllLines(datafile);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Lukuvirhe: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Tiedostoa ei löydy: {datafile}");
            return -1;
        }

        //transponoindaan taas!

        if (phase == 1)
        {
            List<string> t_pattern = TransposePattern(lines);

            result = StrainCalculator(t_pattern);
        }
        else
        {
            int targetrotations = 1000000000;
            List<string> pattern = lines.ToList();
            //muistetaan, että pohjoinen on vasemmalla, joten pyöritetään systeemiä 90 astetta vasemmalle
                pattern = FlipPatternHor(pattern);
                pattern = TransposePattern(pattern.ToArray());

            List<List<string>> configurations = [pattern];

            var currentpattern = pattern;

            bool foundpattern = false;
            for (int i = 0;i<targetrotations;i++)
            {
                currentpattern = Rotator(currentpattern);
                //toistuuko kuvio?
                //Debuggero(currentpattern);
                if (!foundpattern)
                {
                    for (int j = 0; j<=i;j++) if (currentpattern.SequenceEqual(configurations[j]))
                    {
                        int sequencelength = i+1-j;
                        int remainingrotations = targetrotations - (i+1);
                        int sequencesleft = remainingrotations%sequencelength;
                        currentpattern = configurations[j+sequencesleft];
                        foundpattern = true;
                        break;
                    }
                    configurations.Add(currentpattern);
                }
                if (foundpattern) break;
            }

            //currentpattern =TransposePattern(currentpattern.ToArray()); //tarvitaanko?
            Debuggero(currentpattern);

            result = StrainCalculator2(currentpattern);
        }

        return result;
    }


    static List<string> Rotator(List<string> pattern)
    {
        var newpattern = pattern;
        
        //Liikutetaan kivet ja pyöräytetään systeemiä neljänneskierros oikealle
        for (int i=0;i<4;i++)
        {
            newpattern = MoveStones(newpattern);
            newpattern = TransposePattern(newpattern.ToArray());
            newpattern = FlipPatternHor(newpattern);
        }
        return newpattern;
    }
    private static BigInteger StrainCalculator(List<string> t_pattern)
    {
        BigInteger result = 0;
        foreach (var column in t_pattern)
        {
            int rocks = column.Count(x => x == 'O') * column.Length;
            List<int> fixedrocks = column.Select((c, i) => c == '#' ? i : -1).Where(i => i != -1).ToList();
            string subcolumn;
            if (fixedrocks.Count > 0) subcolumn = column[0..fixedrocks[0]];
            else subcolumn = column;
            int minusrocks = 0;
            for (int i = 0; i < subcolumn.Count(x => x == 'O'); i++) minusrocks += i;
            for (int i = 0; i < fixedrocks.Count - 1; i++)
            {
                subcolumn = column[fixedrocks[i]..fixedrocks[i + 1]];
                for (int j = 0; j < subcolumn.Count(x => x == 'O'); j++) minusrocks += fixedrocks[i] + 1 + j;
            }
            if (fixedrocks.Count > 0)
            {
                subcolumn = column[fixedrocks[fixedrocks.Count - 1]..];
                for (int j = 0; j < subcolumn.Count(x => x == 'O'); j++) minusrocks += fixedrocks[fixedrocks.Count - 1] + 1 + j;
            }
            result += rocks - minusrocks;
        }

        return result;
    }

        private static BigInteger StrainCalculator2(List<string> t_pattern)
    {
        BigInteger result = 0;
        foreach (var column in t_pattern)
        {
            List<int> fixedrocks = column.Select((c, i) => c == 'O' ? i : -1).Where(i => i != -1).ToList();
            int subresult = fixedrocks.Count * column.Length - fixedrocks.Sum();
            result += subresult;
        }

        return result;
    }


    private static List<string> TransposePattern(string[] lines)
    {
        var t_pattern = new List<string>();
        for (int i = 0; i < lines[0].Length; i++)
        {
            var t_rowbuilder = new StringBuilder();
            for (int j = 0; j < lines.Length; j++)
            {
                t_rowbuilder.Append(lines[j][i]);
            }
            t_pattern.Add(t_rowbuilder.ToString());
        }

        return t_pattern;
    }

    private static void Debuggero(List<string> pattern)
    {
        var rotpat = pattern.ToList();
        rotpat = TransposePattern(rotpat.ToArray());
        rotpat = FlipPatternHor(rotpat);
        foreach(var line in rotpat) Console.WriteLine(line);
        Console.WriteLine();
    }
}