namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Text;

partial class Program
{
    static int MirrorFinder(List<string> pattern, int nogoodresult = -1)
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
            if (ismirrored && nogoodresult != i) return i;
        }
        return 0;
    }

    static int FindMirrorrow(List<string> pattern, int nogoodresult = -1)
    {
            //Ensin riveittäin
            int mirrorrow = MirrorFinder(pattern, nogoodresult/100);
            if (mirrorrow != 0) return mirrorrow * 100;  //turha katsoja sarakkeita, jos asia on jo ratkennut

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
            mirrorrow = MirrorFinder(t_pattern, nogoodresult);
            return mirrorrow; 
    }

    static BigInteger Day13(int phase, string datafile)
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
            int mirrorrow = FindMirrorrow(pattern);
            if (phase == 1) result += mirrorrow;
            else originalresults.Add(mirrorrow);
        }
        if (phase == 2)
        {
            for (int i1 = 0; i1 < patterns.Count; i1++)
            {
                List<string>? pattern = patterns[i1];
                bool patterdone = false;
                for (int i = 0; i < pattern.Count;i++)
                {
                    for (int j=0; j<pattern[0].Length;j++)
                    {
                        var temppattern = pattern.ToList();
                        var arr_temppattern = temppattern[i].ToCharArray();
                        if (arr_temppattern[j] == '.') arr_temppattern[j] = '#';
                        else arr_temppattern[j] = '.';
                        temppattern[i] = new string(arr_temppattern);
                        int newmirrorline = FindMirrorrow(temppattern, originalresults[i1]);

                        if (newmirrorline != 0)
                        {
                            result += newmirrorline;
                            patterdone = true;
                            break;
                        }
                    }
                    if (patterdone) break;
                }
            }
        }
        return result;
    }
}

//403 too low