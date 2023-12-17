namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Text;

partial class Program
{

    static BigInteger Day17(int phase, string datafile)
    {
        string[] lines = [];
        BigInteger result = 0;
        //tähänkin olisi elegantimpi ratkaisu matriiseilla, mutta kokeillaan nyt näin
        var turn_left = new Dictionary<(int, int), (int, int)>
        {
            {(1,0),(0,1)},
            {(0,1),(-1,0)},
            {(-1,0),(0,-1)},
            {(0,-1),(1,0)}
        };

        var turn_right = new Dictionary<(int, int), (int, int)>
        {
            {(1,0),(0,-1)},
            {(0,-1),(-1,0)},
            {(-1,0),(0,1)},
            {(0,1),(1,0)}
        };


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

        //luetaan kartta merkkitaulukoksi

        int maxy = lines.Length;
        int maxx = lines[0].Length;

        var cavemap = new int[maxy, maxx];
        for (int i = 0; i < maxy; i++)
        {
            for (int j = 0; j < maxx; j++)
            {
                cavemap[i, j] = lines[i][j]-48;
            }
        }

        //suoraan menevän reitin maksimi = (maxy+maxx-2)*9, tätä kalliimpia reittejä ei kannata jatkaa

        (int,int) position =(0,0);  //(y,x)
        int straight = 0;
        (int,int) direction = (0,1);
        int heatloss = 0;
        bool goal = false;
        int minheat = 0;

        //haetaan minheatille joku yläraja. Ei välttämättä tismalleen oikea, mutta varmaan riittävän hyvä.
        //tällä voidaan lakata seuraamasta turhan kiemuraisia reittejä alussa
        for (var i=0;i<maxx;i++) if (i%4 != 3) minheat += cavemap[0,i];
        for (var i=2;i<maxx;i++) if (i%4 != 1) minheat += cavemap[1,i];
        for (var i=0;i<maxy;i++) if (i%4 != 3) minheat += cavemap[i,maxx-1];
        for (var i=2;i<maxy;i++) if (i%4 != 1) minheat += cavemap[i,maxx-2];

        //Koostukoon reitti kirjaimista (E)teen, (L)eft ja (R)ight
        //jos nämä järjestetään aakkosjärjestykseen, ensimmäinen mahdollinen reitti on
        //EEELREEEREEELEEER...
        //U-käännökset ovat turhia, eli LL- ja RR-päätteisiä reittejä ei kannata tutkia
        
        //konstruoidaan ensimmäinen reitti

        char forbidden = 'A';
        var routebuilder = new List<(char, (int, int), int, (int,int))> //komento, (position), heatloss, (direction)
        {
            ('E', (0, 0), 0, direction)
        };
        List<(char, (int, int), int, (int,int))> bestroute;    //debuggaustarkoituksiin

        while (!goal)
        {
            bool nextroute = false;
            
            var newpos = (position.Item1 + direction.Item1,position.Item2 + direction.Item2);
            var newpos_left = (position.Item1 + turn_left[direction].Item1,position.Item2 + turn_left[direction].Item2);
            var newpos_right = (position.Item1 + turn_right[direction].Item1,position.Item2 + turn_right[direction].Item2);
            if (forbidden < 'E' && straight < 3 && newpos.Item1 >= 0 && newpos.Item1 < maxy && newpos.Item2 >= 0 && newpos.Item2 < maxx)
            {
                heatloss += cavemap[newpos.Item1,newpos.Item2];
                routebuilder.Add(('E',newpos, heatloss, direction));
                position = newpos;
                forbidden = 'A';
                straight++;
                if (heatloss >= minheat) nextroute = true;
            }
            else if (forbidden < 'L' && (routebuilder.Count == 0 || routebuilder.Last().Item1 != 'L') && position.Item2 < maxx-1 && newpos_left.Item1 >= 0 && newpos_left.Item1 < maxy && newpos_left.Item2 >= 0 && newpos_left.Item2 < maxx)
            {
                heatloss += cavemap[newpos_left.Item1,newpos_left.Item2];
                direction = turn_left[direction];
                routebuilder.Add(('L',newpos_left, heatloss, direction));
                position = newpos_left;
                forbidden = 'A';
                straight = 1;
                if (heatloss >= minheat) nextroute = true;

            }
            else if (forbidden < 'R' && (routebuilder.Count == 0 || routebuilder.Last().Item1 != 'R') && position.Item1 < maxy-1&& newpos_right.Item1 >= 0 && newpos_right.Item1 < maxy && newpos_right.Item2 >= 0 && newpos_right.Item2 < maxx)
            {
                heatloss += cavemap[newpos_right.Item1,newpos_right.Item2];
                direction = turn_right[direction];
                routebuilder.Add(('R',newpos_right, heatloss, direction));
                position = newpos_right;
                forbidden = 'A';
                straight = 1;
                if (heatloss >= minheat) nextroute = true;
            }
            else
            {
                nextroute = true;
            }
            //tarkista, ollaanko menossa jo vierailtuun ruutuun = (todennäköisesti) turha reitti
            //mennään takaperin, koska on todennäköisempää törmätä lähellä kuin kaukana jonossa oleviin ruutuihin
            if (!nextroute)
            {
                for (var i=routebuilder.Count-4; i>=0;i--)
                {
                    if (position == routebuilder[i].Item2)
                    {
                        nextroute = true;
                        break;
                    }
                }
            }
            //ollaanko maalissa?
            if (position.Item1 == maxy-1 && position.Item2 == maxx-1)
            {
                if (heatloss < minheat) 
                {
                    minheat = heatloss;
                    bestroute = [.. routebuilder];
                }
                nextroute = true;
            }

            if (nextroute)
            {
                if (heatloss != routebuilder.Last().Item3) throw new Exception("Heatloss desync!");
                forbidden = routebuilder.Last().Item1;
                heatloss -= cavemap[position.Item1,position.Item2];
                routebuilder.RemoveAt(routebuilder.Count-1);
                try
                {
                position = routebuilder.Last().Item2;
                direction = routebuilder.Last().Item4;
                }
                catch
                {
                    goal = true;
                }
                if (routebuilder.Count == 0 && forbidden == 'R') goal = true; 
            }
        }

        return minheat;
    }
}