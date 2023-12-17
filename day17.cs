namespace AoC2023;

using System;
using System.Collections.Generic;
using System.Numerics;

partial class Program
{

    static BigInteger Day17(int phase, string datafile)
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

        //alustetaan "käännösmatriisit"

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


        var route = new List<((int,int), int, int, (int, int), int)>();  //position, heatloss, straightsteps, direction, f
        route.Add(((0,0),0,0,(1,0),maxx+maxy-2));   //alustetaan lista

        //ruvetaan menemään A*-algoritmin mukaan

        while (true)
        {
            //valitaan alkio, jolla on pienin heatloss-arvo
            //poistetaan alkio listasta
            var minheat_node = route[0];
            route.RemoveAt(0);

            //ollaanko tultu maaliin?
            if (minheat_node.Item1 == (maxy-1,maxx-1))
            {
                result = minheat_node.Item2;
                break;
            }
            //jos ei, generoidaan seuraavat noodit
            //suoraan
            if (minheat_node.Item3 < 3)
            {
                var direction = minheat_node.Item4;
                var position = minheat_node.Item1;
                var nextpos = (position.Item1 + direction.Item1, position.Item2 + direction.Item2);
                NextNode(maxy, maxx, cavemap, route, minheat_node, direction, position, nextpos, true);
            }
            //vasemmalle
            {
                var direction = turn_left[minheat_node.Item4];
                var position = minheat_node.Item1;
                var nextpos = (position.Item1 + direction.Item1, position.Item2 + direction.Item2);
                NextNode(maxy, maxx, cavemap, route, minheat_node, direction, position, nextpos, false);
            }
            //oikealle
            {
                var direction = turn_right[minheat_node.Item4];
                var position = minheat_node.Item1;
                var nextpos = (position.Item1 + direction.Item1, position.Item2 + direction.Item2);
                NextNode(maxy, maxx, cavemap, route, minheat_node, direction, position, nextpos, false);
            }
        }

        return result;
    }

    private static void NextNode(int maxy, int maxx, int[,] cavemap, List<((int, int), int, int, (int, int), int)> route, ((int, int), int, int, (int, int), int) minheat_node, (int, int) direction, (int, int) position, (int, int) nextpos, bool isstraight)
    {
        if (Goodpos(nextpos, maxy, maxx))
        {
            var heatval = minheat_node.Item2 + cavemap[nextpos.Item1, nextpos.Item2];
            int straight;
            if (isstraight) straight = minheat_node.Item3 + 1;
            else straight = 1;
            var t = maxy - nextpos.Item1 - 1 + maxx - nextpos.Item2 - 1;
            var f = t + heatval;
            if (!route.Any(x => x.Item1 == nextpos && x.Item4 == x.Item4 && x.Item3 <= straight && x.Item2 <= heatval))
            {
                bool notbiggest = false;
                for (var i = 0; i < route.Count; i++)
                {
                    if (f < route[i].Item5)
                    {
                        route.Insert(i, (nextpos, heatval, straight, direction, f));
                        notbiggest = true;
                        break;
                    }
                    else if(f == route[i].Item5 && heatval > route[i].Item2)
                    {
                        route.Insert(i, (nextpos, heatval, straight, direction, f));
                        notbiggest = true;
                        break;
                    }
                }
                if (!notbiggest) route.Add((nextpos, heatval, straight, direction, f));
            }
        }
    }

    private static bool Goodpos((int, int) nextpos, int maxy, int maxx)
    {
        if (nextpos.Item1 >= maxy || nextpos.Item2 >= maxx || nextpos.Item1 < 0 || nextpos.Item2 < 0) return false;
        return true;
    }
}

//748 liian korkea
//741 tietysti liian korkea
//732 liian korkea
//729 liian korkea
//727 liian korkea