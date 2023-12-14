namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Text;

partial class Program
{
    static int MirrorFinder(List<string> pattern)
    {
        for (int i = 1;i<pattern.Count;i++)
            {
            bool ismirrored = true;
            for (int j = 1; i-j >= 0 && i+j <= pattern.Count;j++)
            {
                if(pattern[i+j-1] != pattern[i-j])
                {
                    ismirrored = false;
                    break;
                }
            }
            if (ismirrored) return i;
        }
        return 0;
    }

    static BigInteger Day13(int phase, string datafile)
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

        var patterns = new List<List<string>>();
        var newpattern = new List<string>();
        foreach (var line in lines)
        {
            if (line=="")
            {
                patterns.Add(newpattern);
                newpattern = [];
            }
            else newpattern.Add(line);
        }
        patterns.Add(newpattern);

        //Käydään listat läpi
        foreach (var pattern in patterns)
        {
            //Ensin riveittäin (tee tämä metodiksi)
            int mirrorrow = MirrorFinder(pattern);
            result += 100 * mirrorrow;
            if (mirrorrow != 0) continue;  //turha katsoja sarakkeita, jos asia on jo ratkennut

            //sitten sarakkeisiin
            //transponoidaan rivit
            var t_pattern = new List<string>();
            for (int i=0; i<pattern[0].Length; i++)
            {
                var t_rowbuilder = new StringBuilder();
                for (int j=0; j<pattern.Count; j++)
                {
                    t_rowbuilder.Append(pattern[j][i]);
                }
                t_pattern.Add(t_rowbuilder.ToString());
            }
            //sitten sama homma
            result += MirrorFinder(t_pattern);
        }
        return result;
    }

}