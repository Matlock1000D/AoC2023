namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Collections.Specialized;
using System.Collections;

partial class Program
{
    static int Hasher(string str_input)
    {
        int hash=0;
        foreach(var letter in str_input)
        {
            hash += letter;
            hash *= 17;
            hash %= 256;
        }

        return hash;
    }


    static BigInteger Day15(int phase, string datafile)
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

        var sequence = lines[0].Split(',');
        result = 0;
        if (phase == 1) foreach(var step in sequence) result += Hasher(step);
        else
        {
            var boxes = new Dictionary<int,OrderedDictionary>();
            for (int i = 0; i<256;i++)
            {
                boxes[i] = new OrderedDictionary();
            }

            foreach(var instruction in sequence)
            {
                int commandindex = instruction.Select((c, i) => c == '=' || c == '-' ? i : -1).Where(i => i != -1).ToList()[0];
                char command = instruction[commandindex];
                string label = instruction[..commandindex];
                int box = Hasher(label);
                int lens;
                //palaset on parsetettu, operoidaan
                if (command == '=') 
                {
                    lens = instruction[commandindex+1];
                    boxes[box][label] = lens;
                }
                if (command == '-') boxes[box].Remove(label);
            }
        
            //haetaan vastaus
            for (int i=0;i<256;i++)
            {
                var lenslist = boxes[i].Cast<DictionaryEntry>().ToList();
                for (int j=0;j<lenslist.Count;j++)
                {                
                    result += (i+1)*(j+1)*((int) lenslist[j].Value-48);
                }
            }
        }

        return result;
    } 
}