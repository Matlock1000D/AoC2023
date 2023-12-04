namespace AoC2023;

using System.Collections.Generic;
using System.Linq;

partial class Program
{
    static int Day4(int phase, string datafile)
    {

        string[] lines = [];
        int result = 0;

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
        }

        int[] copies = Enumerable.Repeat(1, lines.Length).ToArray();

        for (int i = 0; i < lines.Length; i++)
        {
            string? line = lines[i];
            var split_line = line.Split(':');   //itse data on split_line[1]
            var game = split_line[1].Split('|'); //game[0] = voittonumerot, game[1] = pelinumerot
            var winning_numbers = new HashSet<int>();
            var my_numbers = new HashSet<int>();
            int hits = 0;
            foreach(var str_number in game[0].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                winning_numbers.Add(int.Parse(str_number));
            }
            foreach(var str_number in game[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                my_numbers.Add(int.Parse(str_number));
            }
            //tarkastetaan tulokset
            foreach(var mynum in my_numbers)
            {
                foreach(var winnum in winning_numbers)
                {
                    if (winnum == mynum)
                    {
                        hits++;
                        break;
                    }
                }
            }
            int value=0;
            if (hits > 0) value = (int)Math.Pow(2,(hits-1));
            if (phase == 2)
            {
                for(int j=1;j<=hits;j++) copies[i+j] += copies[i];
            }
            else result += value;
        }

        if (phase == 2)
        {
            for(int i = 0; i<copies.Length;i++) result+=copies[i];
        }

        return result;
    }
}