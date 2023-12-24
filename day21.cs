namespace AoC2023;

using System;
using System.Collections.Generic;
using System.Numerics;

partial class Program
{

    static BigInteger Day21(int phase, string datafile)
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

        //askelmäärä
        int maxsteps;
        if (datafile[^12..] == "demo21-1.txt")
            maxsteps = 6;
        else
            maxsteps = 64;

        //kartta merkkijonotaulukoksi

        int maxy = lines.Length;
        int maxx = lines[0].Length;
        var places = new Dictionary<(int y,int x),bool>();

        var gardenmap = new char[maxy,maxx];
        for (int i=0;i<maxy;i++)
        {
            for (int j=0;j<maxx;j++)
            {
                gardenmap[i,j] = lines[i][j];
                // Etsitään samalla aloituspiste
                if (gardenmap[i,j] == 'S')
                    places[(i,j)] = false;
            }
        }

        // Etsitään paikat
        // Suunnat
        List<(int,int)> directions = new List<(int y,int x)>{(1,0),(-1,0),(0,1),(0,-1)};
        for (var i=0; i<maxsteps; i+= 2)
        {
            var newplaces = places.Where(x => !x.Value).ToDictionary();
            foreach ((int y, int x) place in newplaces.Keys)
            {
                if (places[place]) continue;
                places[place] = true;
                foreach ((int y, int x) direction in directions)
                { 
                    int newy = place.y + direction.y;
                    int newx = place.x + direction.x;
                    if (GoodPlace(newy, newx, maxy, maxx, gardenmap))
                        foreach ((int y, int x) direction2 in directions)
                        {
                            // Periaatteessa turhaa tarkastaa tulosuunta, mutta varmaankin merkittävää nopeuseroa ei ole.
                            int newy2 = newy + direction2.y;
                            int newx2 = newx + direction2.x;
                            if (GoodPlace(newy2, newx2, maxy, maxx, gardenmap))
                                places.TryAdd((newy2, newx2), false);
                        }
                }       
            }
        }

        result = places.Count;
        return result;
    }

    private static bool GoodPlace(int newy, int newx, int maxy, int maxx, char[,] gardenmap)
    {
        if (newy < 0 || newy >= maxy || newx < 0 || newx >= maxx)
            return false;
        if (gardenmap [newy, newx] == '#')
            return false;
        return true;
    }
}