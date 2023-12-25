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
        if (phase == 2)
            maxsteps = 5000; //26501365 tositilanteessa;
        else if (datafile[^12..] == "demo21-1.txt")
            maxsteps = 6;
        else
            maxsteps = 64;

        //kartta merkkijonotaulukoksi

        int maxy = lines.Length;
        int maxx = lines[0].Length;
        var places = new Dictionary<(int y,int x),bool>();
        (int y, int x) startspace = (0,0);

        var gardenmap = new char[maxy,maxx];
        for (int i=0;i<maxy;i++)
        {
            for (int j=0;j<maxx;j++)
            {
                gardenmap[i,j] = lines[i][j];
                // Etsitään samalla aloituspiste
                if (gardenmap[i,j] == 'S')
                {
                    places[(i,j)] = false;
                    startspace = (i,j);
                }
            }
        }

        int spaces = 0;
        // Montako tyhjää ruutua ylipäätään on?
        if (phase == 2)
        {   
            spaces = gardenmap.Cast<char>().Count(x => x == '.');
        }

        var timetofill = new Dictionary<(int,int),int>();

        // Etsitään paikat
        // Suunnat
        List<(int,int)> directions = new List<(int y,int x)>{(1,0),(-1,0),(0,1),(0,-1)};
        if (phase == 1)
        {
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
        }

        var timetosidedict = TimeToSides(maxy, maxx, startspace, gardenmap, directions);
        // Demokartta: ylös (0,8): 8; alas (10,3): 7; vasemmalle (4,0): 6; oikealle (4,10): 8
        // Toisella kierroksella päädytään jo kulmiin
        // Kulmat tulevat dominoimaan kehitystä
        {
            timetofill[startspace] = GetFillTime(maxy, maxx, startspace, gardenmap, spaces, directions);
        }

        // demokartalla menee 14 askelta koko kartan täyttämiseen

        // Katsotaan, kauanko menee täyttää kulmista
        List<(int y, int x)> corners = [(0,0),(0,maxx-1),(maxy-1,0),(maxy-1,maxx-1)];
        foreach(var corner in corners)
        {
            places = new Dictionary<(int y, int x), bool>();
            places[corner] = false;
            timetofill[corner] = GetFillTime(maxy, maxx, corner, gardenmap, spaces, directions);
        }

        var cornertimes = TimeToCorners(maxy, maxx,startspace, gardenmap, directions);
        // Demossa pääsee muihin kulmiin 10 vuorossa, mutta oikeaan alakulmaan (10,10) menee 14
        // Entä sivut?

        // Nyt voidaan yhdistellä palikat.
        // Jaetaan ongelma osiin:
        // 1. keskusruutu (joka ehtii kyllä tällä askelmäärällä täyttyä),
        // 2. keskusruudusta suoraan etenevät linjat, jotka täyttyvät mahdollisesti jonkun syklin mukaan. Näistä jää täyttymättä 1–2 kussakin suunnassa.
        // 3. keskusruudusta vinoon sijaitsevat alueet. Nämä täyttyvät aritmeettisen sarjan mukaisesti: 1 + 2 + 3... aina jokaista maxx*maxy-aikaa kohden.
        //      Koska askeleet voivat edetä reunoja pitkin, kulma-alueet täyttyvät tasaisesti ja kaikki vajaat alueet ovat samassa vaiheessa täyttymässä.
        //      Siksi riittää tarkastaa vain 1–2 ruudukkoa per kvadrantti.
        // Summataan nämä kustakin suunnasta, niin saadaan haluttu lopputulos.

        // 1. Keskusruutu
        result += spaces;

        // 2. Suorat suunnat
        {
            // täydet
            foreach (var direction in directions)
            {
                var relevantcorners = new List<(int,int)>();
                if (direction == (1,0)) relevantcorners = new List<(int,int)>{(maxy-1,0),(maxy-1,maxx-1)};
                if (direction == (-1,0)) relevantcorners = new List<(int,int)>{(0,0),(0,maxx-1)};
                if (direction == (0,1)) relevantcorners = new List<(int,int)>{(0,maxx-1),(maxy-1,maxx-1)};
                if (direction == (0,-1)) relevantcorners = new List<(int,int)>{(0,0),(maxy-1,0)};
                var fastcorner = cornertimes.Where(x => relevantcorners.Contains(x.Key)).Min().Value;
                result += (maxsteps-fastcorner)/maxy;

                // vajaa(t)
                var stepmaps = new List<bool[,]>();
                for (int i = 0; i < relevantcorners.Count; i++)
                {
                    (int, int) relevantcorner = relevantcorners[i];
                    var timeremaining = (maxsteps-cornertimes[relevantcorner])%maxy;
                    stepmaps.Add(GetStepMap(maxy, maxx, startspace, gardenmap, directions));
                }
            }
            
        }
        

        return result;
    }

        private static bool[,] GetStepMap(int maxy, int maxx, (int y, int x) startspace, char[,] gardenmap, List<(int, int)> directions)
    {
        // TODO jatka tästä
        int i = 0;
        Dictionary<(int y, int x), bool> places = new Dictionary<(int y, int x), bool> {{startspace,false}};
        while (true)
        {
            i++;
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
                        places.TryAdd((newy, newx), false);
                }
            }
            if (places.Count == spaces + 1)
                return i;
        }
    }


    private static int GetFillTime(int maxy, int maxx, (int y, int x) startspace, char[,] gardenmap, int spaces, List<(int, int)> directions)
    {
        int i = 0;
        Dictionary<(int y, int x), bool> places = new Dictionary<(int y, int x), bool> {{startspace,false}};
        while (true)
        {
            i++;
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
                        places.TryAdd((newy, newx), false);
                }
            }
            if (places.Count == spaces + 1)
                return i;
        }
    }

    private static Dictionary<(int y, int x),int> TimeToCorners(int maxy, int maxx, (int y, int x) startspace, char[,] gardenmap, List<(int, int)> directions)
    {
        int i = 0;
        Dictionary<(int y, int x), bool> places = new Dictionary<(int y, int x), bool> {{startspace,false}};
        List<(int y, int x)> corners = [(0,0),(0,maxx-1),(maxy-1,0),(maxy-1,maxx-1)];
        var cornertimes = new Dictionary<(int y, int x),int>();
        while (cornertimes.Count < 4)
        {
            i++;
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
                        if (places.TryAdd((newy, newx), false) && corners.Contains((newy,newx)))
                            cornertimes[(newy,newx)] = i;
                }
            }
        }
        return cornertimes;
    }

    private static Dictionary<(int y, int x), int> TimeToSides(int maxy, int maxx, (int y, int x) startspace, char[,] gardenmap, List<(int, int)> directions)
    {   
        int i = 0;
        Dictionary<(int y, int x), bool> places = new Dictionary<(int y, int x), bool> {{startspace,false}};
        var resultdict = new Dictionary<(int y, int x), int>();
        bool up=false, down=false, left=false, right=false;
        // TODO viimeistele
        while (resultdict.Count < 4)
        {
            i++;
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
                    {
                        places.TryAdd((newy, newx), false);
                        if (!up && newy == 0)
                        {
                            resultdict[(newy,newx)] = i;
                            up = true;
                        }
                        if (!down && newy == maxy-1)
                        {
                            resultdict[(newy,newx)] = i;
                            down = true;
                        }
                        if (!left && newx == 0)
                        {
                            resultdict[(newy,newx)] = i;
                            left = true;
                        }
                        if (!right && newx == maxx-1)
                        {
                            resultdict[(newy,newx)] = i;
                            right = true;
                        }
                    }
                }
            }
        }
        return resultdict;
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