namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Globalization;

partial class Program
{
    static BigInteger Day18(int phase, string datafile)
    {
        // Toivotaan kovasti, ettei kaivanto mene ristiin tai pakita

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
            char direction = ' ';
            if (phase == 1) direction = splitline[0][0];
            else
                switch (splitline[2][^2..^1])
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
            else length = int.Parse(splitline[2][2..7],NumberStyles.HexNumber);
            digcommands.Add((direction,length));
        }
        //tehdään kaivuukartta
        int maxx = 0;
        int curx = 0;
        int maxy = 0;
        int cury = 0;
        int origx = 0; // origon sijainti
        int origy = 0;

        // Tässä ei kartan piirtämisestä tule mitään, koska
        // se ei rasterina mahdu mihinkään muistiin
        if (phase == 2)
        {
            var startcorners = new List <(int,int)> {(0,0),(1,0),(0,1),(1,1)};
            var areas = new List<BigInteger>();

            foreach (var startcorner in startcorners)
            {
                // Missä kursori on viimeksi kaivetun kolon suhteen
                int cornerx = startcorner.Item2;
                int cornery = startcorner.Item1;

                // Lista monikulmion kulmista (y, x)
                var nodes = new List<(BigInteger,BigInteger)> {(cornery,cornerx)};

                // Arvataan, että lähtöpiste on vasen yläkulma, ja
                // että se kuuluu ulkoreunaan
                for (var i = 0; i < digcommands.Count-1; i++)
                {
                    switch(digcommands[i].direction)
                    {
                        case 'U':
                            cury += digcommands[i].length;
                            if (cornerx == 0 && digcommands[i+1].direction == 'L')
                                cornery = 0;
                            if (cornerx == 1 && digcommands[i+1].direction == 'L')
                                cornery = 1;
                            if (cornerx == 1 && digcommands[i+1].direction == 'R')
                                cornery = 0;
                            if (cornerx == 0 && digcommands[i+1].direction == 'R')
                                cornery = 1;
                            break;
                        case 'D':
                            cury -= digcommands[i].length;
                            if (cornerx == 0 && digcommands[i+1].direction == 'L')
                                cornery = 1;
                            if (cornerx == 1 && digcommands[i+1].direction == 'L')
                                cornery = 0;
                            if (cornerx == 1 && digcommands[i+1].direction == 'R')
                                cornery = 1;
                            if (cornerx == 0 && digcommands[i+1].direction == 'R')
                                cornery = 0;
                            break;
                        case 'R':
                            curx += digcommands[i].length;
                            if (cornery == 0 && digcommands[i+1].direction == 'U')
                                cornerx = 1;
                            if (cornery == 1 && digcommands[i+1].direction == 'U')
                                cornerx = 0;
                            if (cornery == 1 && digcommands[i+1].direction == 'D')
                                cornerx = 1;
                            if (cornery == 0 && digcommands[i+1].direction == 'D')
                                cornerx = 0;
                            break;
                        case 'L':
                            curx -= digcommands[i].length;
                            if (cornery == 0 && digcommands[i+1].direction == 'U')
                                cornerx = 0;
                            if (cornery == 1 && digcommands[i+1].direction == 'U')
                                cornerx = 1;
                            if (cornery == 1 && digcommands[i+1].direction == 'D')
                                cornerx = 0;
                            if (cornery == 0 && digcommands[i+1].direction == 'D')
                                cornerx = 1;
                            break;
                    }
                    nodes.Add((cury+cornery,curx+cornerx));
                }
                // Lopuksi maaliin
                int j = digcommands.Count-1;
                switch(digcommands[j].direction)
                {
                case 'U':
                    cury += digcommands[j].length;
                    if (cornerx == 0 && digcommands[0].direction == 'L')
                        cornery = 0;
                    if (cornerx == 1 && digcommands[0].direction == 'L')
                        cornery = 1;
                    if (cornerx == 1 && digcommands[0].direction == 'R')
                        cornery = 0;
                    if (cornerx == 0 && digcommands[0].direction == 'R')
                        cornery = 1;
                    break;
                case 'D':
                    cury -= digcommands[j].length;
                    if (cornerx == 0 && digcommands[0].direction == 'L')
                        cornery = 1;
                    if (cornerx == 1 && digcommands[0].direction == 'L')
                        cornery = 0;
                    if (cornerx == 1 && digcommands[0].direction == 'R')
                        cornery = 1;
                    if (cornerx == 0 && digcommands[0].direction == 'R')
                        cornery = 0;
                    break;
                case 'R':
                    curx += digcommands[j].length;
                    if (cornery == 0 && digcommands[0].direction == 'U')
                        cornerx = 1;
                    if (cornery == 1 && digcommands[0].direction == 'U')
                        cornerx = 0;
                    if (cornery == 1 && digcommands[0].direction == 'D')
                        cornerx = 1;
                    if (cornery == 0 && digcommands[0].direction == 'D')
                        cornerx = 0;
                    break;
                case 'L':
                    curx -= digcommands[j].length;
                    if (cornery == 0 && digcommands[0].direction == 'U')
                        cornerx = 0;
                    if (cornery == 1 && digcommands[0].direction == 'U')
                        cornerx = 1;
                    if (cornery == 1 && digcommands[0].direction == 'D')
                        cornerx = 0;
                    if (cornery == 0 && digcommands[0].direction == 'D')
                        cornerx = 1;
                    break;
                }
                nodes.Add((cury+cornery,curx+cornerx));
                nodes.Add((startcorner.Item1,startcorner.Item2));

                // Lasketaan monikulmion ala

                BigInteger area = 0;
                for (var i = 0; i<nodes.Count-1; i++)
                    area += nodes[i].Item2 * nodes[i+1].Item1 - nodes[i+1].Item2 * nodes[i].Item1;
                area += nodes[nodes.Count-1].Item2 * nodes[0].Item1 - nodes[0].Item2 * nodes[nodes.Count-1].Item1;
                areas.Add(BigInteger.Abs(area));
            }
            return areas.Max()/2;
        }

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
        int[,] lagoonmap = new int[maxy,maxx];
        curx = origx;
        cury = origy;
        lagoonmap[cury,curx] = 1;
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
                lagoonmap[cury,curx] = 1;
            }
        }
        
        // flood-fill ulkoreunaan
        for (var i=0;i<maxx;i++)
            if (lagoonmap[0,i] == 0) lagoonmap[0,i] = -1;
        for (var i=0;i<maxy;i++)
            if (lagoonmap[i,0] == 0) lagoonmap[i,0] = -1;
        for (var i=0;i<maxx;i++)
            if (lagoonmap[maxy-1,i] == 0) lagoonmap[maxy-1,i] = -1;
        for (var i=0;i<maxy;i++)
            if (lagoonmap[i,maxx-1] == 0) lagoonmap[i,maxx-1] = -1;
        
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
                    if (lagoonmap[i,j] == 0)
                    {
                        if (lagoonmap[i-1,j] == -1 || lagoonmap[i+1,j] == -1 || lagoonmap[i,j+1] == -1 || lagoonmap[i,j-1] == -1)   // jos vierestä löytyy ulkopuolta
                        {
                            lagoonmap[i,j] = -1;
                            filled = true;
                        }
                    }
                }
            }
        }
        // lasketaan ykkös- ja nollaruudut
        result = lagoonmap.Cast<int>().Count(x => x != -1);
        return result;
    }
}

//62848 liian iso
//katso inputtia
//piirrä tuloskartta