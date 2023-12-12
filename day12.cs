namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;

partial class Program
{

    static int CheckSpring(string orig_springs, List<int> orig_springnums)
    {
        int combinations = 0;

        var springs = orig_springs;     //turhaa, mutta olkoon varuiksi
        var springnums = orig_springnums.ToList();

        var springnums_length = springnums.Sum() + springnums.Count-1;
        if (springs.Length < springnums_length) return 0; //jäljellä olevat jouset eivät voi mahtua jäljellä olevaan tilaan 

        for (int i = 0; i<springs.Length; i++)
        {
            if (springs[i] == '#')
            {
                string nextstring;
                if (i+springnums[0] > springs.Length) return 0; //jouset eivät enää mahdu, tarkastelta jono on mahdoton
                var nextset = springs.Substring(i,springnums[0]);
                if (nextset.Contains(".")) return 0;    //jos löytyy #, mutta seuraava määrä jousia ei mahdu, tarkasteltava jono on mahdoton
                //muuten heitetään pois tarkasteltavat jouset
                if (i+springnums[0] + 1 <= springs.Length) 
                {
                    if (springs[i+springnums[0]] == '#') return 0; //jousijono on liian pitkä, tarkasteltava jono on mahdoton 
                    nextstring = springs.Substring(i+springnums[0]+1);   //jatketaan tarkastelua seuraavasta jonosta
                }
                else nextstring = "";
                // heitetään pois listan ensimmäinen luku
                springnums.RemoveAt(0);

                //jos lista on tyhjä, tarkistetaan, onko jousia vielä jäljellä
                if (springnums.Count == 0)
                {
                    if (nextstring.Contains("#")) return 0;
                    else return 1;
                }

                //muuten jatketaan
                return CheckSpring(nextstring,springnums);
            } 
            if (springs[i] == '?')
            {
                string nextstring;
                if (i+springnums[0] > springs.Length) nextstring = "";
                else nextstring = springs.Substring(i+1);
                return CheckSpring($"#{nextstring}",springnums) + CheckSpring($".{nextstring}",springnums);
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

        foreach (var line in lines)
        {
            var splitline = line.Split(' ');
            var springs = splitline[0];
            var str_springnums = splitline[1].Split(',');
            var springnums = new List<int>();
            foreach(var springnum in str_springnums) springnums.Add(int.Parse(springnum));

            result += CheckSpring(springs,springnums);
        }

        return result;
    }
}