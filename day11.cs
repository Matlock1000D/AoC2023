namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;

partial class Program
{

    static BigInteger Day11(int phase, string datafile)
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

        //kartta merkkijonotaulukoksi

        int maxy = lines.Length;
        int maxx = lines[0].Length;

        var starmap = new char[maxy,maxx];
        for (int i=0;i<maxy;i++)
        {
            for (int j=0;j<maxx;j++)
            {
                starmap[i,j] = lines[i][j];
            }
        }

        //etsitään tähdet
        var starlist = new List<(int,int)>();   //olkoon tässä koordinaattijärjestys (y,x) yhtenäisyyden vuoksi
        for (int i=0;i<maxy;i++)
        {
            for (int j=0;j<maxx;j++)
            {
                if (starmap[i,j] == '#') starlist.Add((i,j));
            }
        }

        //etsitään tyhjät rivit
        var emptyrows = new List<int>();
        for (int i=0;i<maxy;i++)
        {
            bool emptyrow = true;
            for (int j=0;j<maxx;j++)
            {
                if (starmap[i,j]== '#')
                {
                    emptyrow = false;
                    break;
                }
            }
            if (emptyrow) emptyrows.Add(i);
        }

        //etsitään tyhjät sarakkeet
        var emptycols = new List<int>();
        for (int i=0;i<maxx;i++)
        {
            bool emptycol = true;
            for (int j=0;j<maxy;j++)
            {
                if (starmap[j,i]== '#')
                {
                    emptycol = false;
                    break;
                }
            }
            if (emptycol) emptycols.Add(i);
        }

        //lasketaan kysytty etäisyys
        //käydään ensin läpi kaikki tähdet
        for (int i=0;i<starlist.Count;i++)
        {
            for (int j=i+1;j<starlist.Count;j++)
            {
                result += Math.Abs(starlist[i].Item1-starlist[j].Item1);
                result += Math.Abs(starlist[i].Item2-starlist[j].Item2);
            }
        }

        //huomioidaan tyhjät sarakkeet
        foreach (var emptycol in emptycols)
        {
            //yhden tyhjän sarakkeen vaikutus kokonaisuuteen on (sarakkeen vasemmalla puolella olevat tähdet) * (sarakkeen oikealla puolella olevat tähdet)
            BigInteger expansionco = starlist.Count(x => x.Item2 < emptycol) * starlist.Count(x => x.Item2 > emptycol);
            if (phase == 1) result += expansionco;
            else result += 999999*expansionco;
        }

        //huomioidaan tyhjät rivit
        foreach (var emptyrow in emptyrows)
        {
            //yhden tyhjän rivin vaikutus kokonaisuuteen on (sarakkeen vasemmalla puolella olevat tähdet) * (sarakkeen oikealla puolella olevat tähdet)
            BigInteger expansionco = starlist.Count(x => x.Item1 < emptyrow) * starlist.Count(x => x.Item1 > emptyrow);
            if (phase == 1) result += expansionco;
            else result += 999999*expansionco;

        }
    
        return result;
    }
}