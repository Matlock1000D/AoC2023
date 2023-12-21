namespace AoC2023;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Numerics;
using System.Text;

partial class Program
{
    static BigInteger Day18(int phase, string datafile)
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

        var digcommands = new List<(char direction,int length)>(); // kaivuuohjeet
        //parsitaan komennot
        foreach (string line in lines)
        {
            var splitline = line.Split(' ');
            char direction='U';
            if (phase == 1) direction = splitline[0][0];
            else
            {
                switch(splitline[2][^2..^1])
                {
                    case "0":
                        direction = 'R';
                        break;
                    case "1":
                        direction = 'D';
                        break;
                    case "2":
                        direction = 'L';
                        break;
                    case "3":
                        direction = 'U';
                        break;
                }
            int length;
            if (phase == 1) length = int.Parse(splitline[1]);
            else length = int.Parse(splitline[2][2..^2],NumberStyles.HexNumber);
            digcommands.Add((direction,length));
            }
        }

        // Suurimmat yhteiset jakajat
        int xdivisor = 1;
        int ydivisor = 1;

        // yritä skaalata kaivuuohjeita
        if (phase == 2)
        {
            // Jaetaan omiin listoihinsa vaaka- ja pystysuorat liikkeet
            var xlist = new List<int>();
            var ylist = new List<int>();

            foreach (var digcommand in digcommands)
            {
                if (new List<char> {'U','D'}.Contains(digcommand.direction))
                    ylist.Add(digcommand.direction);
                else
                    xlist.Add(digcommand.direction);
            }

            // Etsitään kummallekin suurin yhteinen jakaja.
            xdivisor = CustomMaths.Gcd(xlist);
            ydivisor = CustomMaths.Gcd(ylist);

            // Jaetaan pituudet jakajilla.

            for (int i=0; i<digcommands.Count; i++)
            {
                if (new List<char> {'U','D'}
                .Contains(digcommands[i].direction)) 
                digcommands[i] = 
                (digcommands[i].direction, digcommands[i].length/ydivisor);
                else digcommands[i] = 
                (digcommands[i].direction, digcommands[i].length/xdivisor);
            }
        }
        //tehdään kaivuukartta
        int maxx = 0;
        int curx = 0;
        int maxy = 0;
        int cury = 0;
        int origx = 0; // origon sijainti
        int origy = 0;

        //lasketaan, paljonko tilaa kartalle tarvitaan
        foreach(var digcommand in digcommands)
        {
            switch(digcommand.direction)
            {
                case 'U':
                    cury += digcommand.length;
                    break;
                case 'D':
                    cury -= digcommand.length;
                    break;
                case 'R':
                    curx += digcommand.length;
                    break;
                case 'L':
                    curx -= digcommand.length;
                    break;
            }
            if (curx < 0)   //lisää maxia, aseta nolla uudestaan
            {
                maxx += -1*curx;
                origx += -1*curx;
                curx = 0;
            }
            else if (curx > maxx)   //lisää maxia
            {
                maxx = curx;
            }
            if (cury < 0)   //lisää maxia, aseta nolla uudestaan
            {
                maxy += -1*cury;
                origy += -1*cury;
                cury = 0;
            }
            if (cury > maxy)   //lisää maxia
            {
                maxy = cury;
            }
        }
        maxx++; // muista, että yläreuna ei kuulu alueeseen
        maxy++;
        
        //luodaan kartta, pitäisi alustua nollaan
        bool?[,] lagoonmap = null;
        
        lagoonmap = new bool?[maxy,maxx];
        curx = origx;
        cury = origy;
        lagoonmap[cury,curx] = true;
        //piirretään kartta ohjeiden mukaan
        foreach (var digcommand in digcommands)
        {
            for (var i=0;i<digcommand.length;i++)
            {
                switch (digcommand.direction)
                {
                    case 'U':
                        cury++;
                        break;
                    case 'D':
                        cury--;
                        break;
                    case 'R':
                        curx++;
                        break;
                    case 'L':
                        curx--;
                        break;
                }
                lagoonmap[cury,curx] = true;
            }
        }
        
        // flood-fill ulkoreunaan
        for (var i=0;i<maxx;i++)
            if (lagoonmap[0,i] == null) lagoonmap[0,i] = false;
        for (var i=0;i<maxy;i++)
            if (lagoonmap[i,0] == null) lagoonmap[i,0] = false;
        for (var i=0;i<maxx;i++)
            if (lagoonmap[maxy-1,i] == null) lagoonmap[maxy-1,i] = false;
        for (var i=0;i<maxy;i++)
            if (lagoonmap[i,maxx-1] == null) lagoonmap[i,maxx-1] = false;
        
        // flood-fill
        bool filled = true; // lippu jolla katsotaan, onko jotain täytetty
                            // jatketaan täyttösilmukkaa, kunnes uutta täytettävää ei löydy
                            // JOS ON HIDAS, VOITAISIIN VARIOIDA TÄYTTÖJÄRJESTYSTÄ
        while (filled)
        {
            filled = false;
            for (var i=1;i<maxy-1;i++)
            {
                for (var j=1;j<maxx-1;j++)
                {
                    if (lagoonmap[i,j] == null)
                    {
                        if (lagoonmap[i-1,j] == false || lagoonmap[i+1,j] == false || lagoonmap[i,j+1] == false || lagoonmap[i,j-1] == false)   // jos vierestä löytyy ulkopuolta
                        {
                            lagoonmap[i,j] = false;
                            filled = true;
                        }
                    }
                }
            }
        }
        // lasketaan ykkös- ja nollaruudut
        result = lagoonmap.Cast<int>().Count(x => x != -1) *
        xdivisor * ydivisor;
        return result;
    }
}

//TODO selvitä, voiko taulukon indeksikokoa kasvattaa