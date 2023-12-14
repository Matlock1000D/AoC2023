namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Text;

partial class Program
{
    

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
        var t_pattern = new List<string>();
        for (int i=0; i<lines[0].Length; i++)
        {
            var t_rowbuilder = new StringBuilder();
            for (int j=0; j<lines.Length; j++)
            {
                t_rowbuilder.Append(lines[j][i]);
            }
            t_pattern.Add(t_rowbuilder.ToString());
        }

        foreach (var column in t_pattern)
        {
            int rocks = column.Count(x => x == 'O') * column.Length;
            List<int> fixedrocks = column.Select((c,i) => c == '#' ? i : -1).Where(i => i != -1).ToList();
            string subcolumn;
            if (fixedrocks.Count > 0) subcolumn = column[0..fixedrocks[0]];
            else subcolumn = column;
            int minusrocks = 0;
            for (int i=0;i < subcolumn.Count(x => x == 'O'); i++) minusrocks += i; 
            for (int i=0;i<fixedrocks.Count-1;i++)
            {
                subcolumn = column[fixedrocks[i]..fixedrocks[i+1]];
                for (int j=0;j < subcolumn.Count(x => x == 'O'); j++) minusrocks += fixedrocks[i] + 1 + j; 
            }
            if (fixedrocks.Count > 0)
            {
                subcolumn = column[fixedrocks[fixedrocks.Count-1]..];
                for (int j=0;j < subcolumn.Count(x => x == 'O'); j++) minusrocks += fixedrocks[fixedrocks.Count-1] + 1 + j; 
            }          
            result += rocks - minusrocks;
        }
        
        return result;
    }
}