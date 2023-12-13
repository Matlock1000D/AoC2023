namespace AoC2023;

using System.Collections.Generic;
using System.Collections;
using System.Numerics;
using System.Text;

partial class Program
{

    static Dictionary<(string, int),int> knownresults = [];
    static int hits = 0;

    static int CheckSpring(string orig_springs, List<int> orig_springnums)
    {
        int combinations = 0;

        var springs = orig_springs.TrimStart('.');     //turhaa, mutta olkoon varuiksi
        var springnums = orig_springnums.ToList();
        var springnums_arr = StructuralComparisons.StructuralEqualityComparer.GetHashCode(springnums);

        if (knownresults.TryGetValue((springs, springnums_arr), out int value))
        {
            hits++;
            return value;
        }

        var springnums_length = springnums.Sum() + springnums.Count-1;
        if (springs.Length < springnums_length)
        {
            knownresults.Add((springs,springnums_arr),0);
            return 0; //jäljellä olevat jouset eivät voi mahtua jäljellä olevaan tilaan
        }

        if (springs.Length == springnums_length)
        {
            var perfectstring_builder = new StringBuilder();

            foreach (var springnum in springnums)
            {
                for (int i=0; i< springnum; i++) perfectstring_builder.Append("#");
                perfectstring_builder.Append('.');
            }
                var perfectstring = perfectstring_builder.Remove(perfectstring_builder.Length-1,1).ToString();
                if (perfectstring == springs)
                {
                    knownresults.Add((springs,springnums_arr),1);
                    return 1;
                }
        }

        for (int i = 0; i<springs.Length; i++)
        {
            if (springs[i] == '#')
            {
                string nextstring;
                if (i+springnums[0] > springs.Length)
                {
                    knownresults.Add((springs,springnums_arr),0);
                    return 0; //jouset eivät enää mahdu, tarkastelta jono on mahdoton
                }

                var nextset = springs.Substring(i,springnums[0]);
                if (nextset.Contains('.')) 
                {
                    knownresults.Add((springs,springnums_arr),0);
                    return 0; //jouset eivät enää mahdu, tarkastelta jono on mahdoton
                }
                //jos löytyy #, mutta seuraava määrä jousia ei mahdu, tarkasteltava jono on mahdoton
                //muuten heitetään pois tarkasteltavat jouset
                if (i+springnums[0] + 1 <= springs.Length) 
                {
                    if (springs[i+springnums[0]] == '#')
                    {
                        knownresults.Add((springs,springnums_arr),0);
                        return 0; //jousijono on liian pitkä, tarkasteltava jono on mahdoton
                    } 
                    nextstring = springs.Substring(i+springnums[0]+1);   //jatketaan tarkastelua seuraavasta jonosta
                }
                else nextstring = "";
                // heitetään pois listan ensimmäinen luku
                springnums.RemoveAt(0);

                //jos lista on tyhjä, tarkistetaan, onko jousia vielä jäljellä
                if (springnums.Count == 0)
                {
                    if (nextstring.Contains('#')) 
                    {
                        knownresults.Add((springs,springnums_arr),0);
                        return 0;
                    }
                    else 
                    {
                        knownresults.Add((springs,springnums_arr),1);
                        return 1;
                    }
                }

                //muuten jatketaan
                return CheckSpring(nextstring,springnums);
            } 
            if (springs[i] == '?')
            {
                string nextstring;
                if (i+springnums[0] > springs.Length) nextstring = "";
                else nextstring = springs.Substring(i+1);
                int result = CheckSpring($"#{nextstring}",springnums) + CheckSpring($".{nextstring}",springnums);
                knownresults.Add((springs,springnums_arr),result);
                return result;
            }
        }

        return combinations;
    }


    static BigInteger Day12(int phase, string datafile)
    {
        string[] lines = [];
        BigInteger result = 0;

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

        for (int i1 = 0; i1 < lines.Length; i1++)
        {
            string? line = lines[i1];
            var splitline = line.Split(' ');
            var springs = splitline[0];
            if (phase == 2) springs = $"{springs}?{springs}?{springs}?{springs}?{springs}";
            //tiivistetään pois turhat moninkertaiset pisteet
            while (springs.Contains(".."))
            {
                springs = springs.Replace("..",".");
            }
            var str_springnums = splitline[1].Split(',');
            var springnums = new List<int>();
            foreach(var springnum in str_springnums) springnums.Add(int.Parse(springnum));
            if (phase == 2)
            {
                for (int i = 0;i<4;i++)
                {
                    springnums = springnums.Concat(springnums).ToList();
                }
            }
            springs = springs.TrimStart('.').TrimEnd('.');
            result += CheckSpring(springs,springnums);
            if (phase == 2) Console.WriteLine(i1);
        }

        return result;
    }
}