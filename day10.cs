namespace AoC2023;

using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Numerics;
using System.Text;

partial class Program
{
    internal static readonly char[] sourceArray = ['-', 'L', 'J','.'];
    internal static readonly char[] sourceArray0 = ['-', 'F', '7','.'];
    internal static readonly char[] sourceArray1 = ['|', 'L', 'F','.'];
    internal static readonly char[] sourceArray2 = ['|', '7', 'J','.'];

    static BigInteger Day10(int phase, string datafile)
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

        //muunnetaan rivit merkkitaulukoksi
        var pipemap = new char[lines.Length][];

        for (int i = 0; i < lines.Length; i++)
        {
            string? line = lines[i];
            pipemap[i] = line.ToCharArray();
        }

        var goodloopmap = new char[lines.Length,lines[0].Length];     //tehdään kartta siitä, missä hyvä silmukka menee

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j=0; j<lines[0].Length; j++)
            goodloopmap[i,j] = '.';
        }

        var nestpos = new char[lines.Length,lines[0].Length];     //tehdään kartta siitä, missä pesä voi olla

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j=0; j<lines[0].Length; j++)
            nestpos[i,j] = ' ';
        }


        //etsitään aloituspaikka (y,x)
        (int,int)start_point = (0,0);

        for (int i = 0; i<lines.Length; i++)
        {
            for (int j=0; j<lines[0].Length; j++)
            {
                if (pipemap[i][j] == 'S') start_point = (i,j);
            }
        }
        int maxy = lines.Length-1;
        int maxx = lines[0].Length-1;
        //alustetaan suuntavektori
        int[] dir = [1,0];
        int looplength = 0;
        int[] position = [start_point.Item1,start_point.Item2];
        int tries = 0;
        goodloopmap[start_point.Item1,start_point.Item2] = 'S';

        //lähdetään seuraamaan silmukkaa
        while(true)
        {
            position = position.Zip(dir, (a,b) => a+b).ToArray();
            looplength++;
            //tarkistetaan, mihin on tultu
            bool badroute = false;
            int y = position[0];
            int x = position[1];
            char hereitem = '.';
            if (x < 0 || y < 0 || y > maxy || x >maxx) badroute = true; //on jouduttu pois kartalta
            else hereitem = pipemap[y][x];

            goodloopmap[y,x] = hereitem;   //tehdään tätä nyt samalla vaivalla, tarvitsee kakkoskohdassa
            
            if (dir[0] == -1)
            {
                if (sourceArray.Contains(hereitem)) badroute = true;
                else if (hereitem == '7')
                {
                    dir[0] = 0;
                    dir[1] = -1;
                }
                else if (hereitem == 'F')
                {
                    dir[0] = 0;
                    dir[1] = 1;
                }
                goodloopmap[y,x] = 'O';
            }
            else if (dir[0] == 1)
            {
                if (sourceArray0.Contains(hereitem)) badroute = true;
                else if (hereitem == 'J')
                {
                    dir[0] = 0;
                    dir[1] = -1;
                }
                else if (hereitem == 'L')
                {
                    dir[0] = 0;
                    dir[1] = 1;
                }
                goodloopmap[y,x] = 'V';
            }
            else if (dir[1] == 1)
            {
                if (sourceArray1.Contains(hereitem)) badroute = true;
                else if (hereitem == '7')
                {
                    dir[0] = 1;
                    dir[1] = 0;
                    goodloopmap[y,x] = 'V';

                }
                else if (hereitem == 'J')
                {
                    dir[0] = -1;
                    dir[1] = 0;
                    goodloopmap[y,x] = 'O';
                }
            }
            else if (dir[1] == -1)
            {
                if (sourceArray2.Contains(hereitem)) badroute = true;
                else if (hereitem == 'F')
                {
                    dir[0] = 1;
                    dir[1] = 0;
                    goodloopmap[y,x] = 'O';
                }
                else if (hereitem == 'L')
                {
                    dir[0] = -1;
                    dir[1] = 0;
                    goodloopmap[y,x] = 'V';
                }
            }

            if (hereitem == 'S') 
            {
                if (phase == 1) return looplength/2; //jos on palattu alkuun = voitto!
                //Muuten selvitetään mahdolliset pesäpaikat
                for (int i=0; i<=maxy; i++)
                {
                    for (int j=0; j<=maxx; j++) if (goodloopmap[i,j] != '.') nestpos[i,j] = ' ';
                }
                char currentside = '?';   //kumpi puoli on ulkopuoli
                for (int i=0; i<=maxy; i++)
                {
                    for (int j=0; j<=maxx; j++)
                    {
                        if (goodloopmap[i,j] == '.') nestpos[i,j] = currentside;
                        if (goodloopmap[i,j] == 'V' || goodloopmap[i,j] == 'O') currentside = goodloopmap[i,j];
                    }
                }
                char inside = ' ';
                if (currentside == 'V') inside = 'O';
                else inside = 'V';
                for (int i=0; i<=maxy; i++)
                {
                    for (int j=0; j<=maxx; j++) Console.Write(goodloopmap[i,j]);
                    Console.WriteLine("");
                    for (int j=0; j<=maxx; j++) if (nestpos[i,j] == inside) result++;
                }
                Console.WriteLine("");
                for (int i=0; i<=maxy; i++)
                {
                    for (int j=0; j<=maxx; j++) Console.Write(nestpos[i,j]);
                    Console.WriteLine("");
                    //for (int j=0; j<maxx; j++) if (nestpos[i,j] == inside) result++;
                }
                return result;
            }

            if (badroute)   //resetoidaan tilanne ja yritetään eri suuntaa, jos valittu reitti ei ollut silmukka
            {
                position[0] = start_point.Item1;
                position[1] = start_point.Item2;
                looplength = 0;
                tries++;
                if (phase == 2) //resetoidaan hyvän luupin kartta
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        for (int j=0; j<lines[0].Length; j++)
                        goodloopmap[i,j] = '.';
                    }
                    goodloopmap[start_point.Item1,start_point.Item2] = 'S';

                }
                switch (tries)
                {
                    case 1:
                        dir[0] = 0;
                        dir[1] = 1;
                        break;
                    case 2:
                        dir[0] = -1;
                        dir[1] =0;
                        break;
                    case 3:
                        return -1;
                }
            }
        }
    }
}