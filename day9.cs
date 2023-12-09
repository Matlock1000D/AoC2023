namespace AoC2023;

using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

partial class Program
{
    static BigInteger Day9(int phase, string datafile)
    {
        string[] lines = [];

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
        BigInteger result = 0;

        var sequences = new List<List<int>>();

        foreach (var line in lines)
        {
            var sequence = new List<int>();
            foreach (var value in line.Split(' ',StringSplitOptions.TrimEntries))
            {
                sequence.Add(int.Parse(value));
            }
            sequences.Add(sequence);
        }
        
        //käsitellään luvut
        foreach(var sequence in sequences)
        {
            var sequencesolver = new List<List<int>>(){sequence};
            var newsequence = sequence;
            while (newsequence.Count(x => x==0) != newsequence.Count)
            {
                newsequence = new List<int>();
                for(int i=1;i<sequencesolver[sequencesolver.Count-1].Count;i++)
                {
                    newsequence.Add(sequencesolver[sequencesolver.Count-1][i]-sequencesolver[sequencesolver.Count-1][i-1]);
                }
                sequencesolver.Add(newsequence);
            }
            
            //ja takaisinpäin
            if (phase == 1)
            {
                for(int i = sequencesolver.Count-3;i >= 0;i--)
                {
                    int diff = sequencesolver[i+1].Last();
                    sequencesolver[i].Add(sequencesolver[i][sequencesolver[i].Count-1] + diff);
                }
                result += sequencesolver[0][sequencesolver[0].Count-1];
            }
            else
            {
                for(int i = sequencesolver.Count-3;i >= 0;i--)
                {
                    int diff = sequencesolver[i+1].First();
                    sequencesolver[i].Insert(0,sequencesolver[i][0] - diff);
                }
                result += sequencesolver[0][0];
            }
        }
        
        return result;
    }
}