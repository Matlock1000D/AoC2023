namespace AoC2023;

using System;
using System.Collections.Generic;
using System.IO.Compression;
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

        //tehdään graafi
        var graph = new Dictionary<(int,int,int),Dictionary<(int,int,int),int>>();  //kerros 0: vaakasuorat liikkeet, kerros 7, pystysuorat
        int maxstraights = 10+1-(4-1);
        if (phase == 2)
        {
            //vaakasuuntaiset liikkeet
            {
                for (var j=0;j<maxy;j++)
                {
                    for (var k=0;k<maxx;k++)
                    {
                        graph.Add((0,j,k),new Dictionary<(int,int,int),int>());
                        for (var l=4;l<=maxstraights;l++)
                        {   
                            if (GoodNode(maxy, maxx, j, k+l))
                            {
                                graph[(0, j, k)].Add((1, j, k + l),GetCost(cavemap, j, k, l, 1, 'x'));
                            }
                            if (GoodNode(maxy, maxx, j, k-l))
                            {
                                graph[(0, j, k)].Add((1, j, k - l),GetCost(cavemap, j, k, l, -1, 'x'));
                            }
                        }
                    }
                }            
            }
            //pystysuuntaiset liikkeet
                        {
                for (var j=0;j<maxy;j++)
                {
                    for (var k=0;k<maxx;k++)
                    {
                        graph.Add((1,j,k),new Dictionary<(int,int,int),int>());
                        for (var l=4;l<=maxstraights;l++)
                        {   
                            if (GoodNode(maxy, maxx, j+l, k))
                            {
                                graph[(1, j, k)].Add((0, j+l, k),GetCost(cavemap, j, k, l, 1, 'y'));
                            }
                            if (GoodNode(maxy, maxx, j-l, k))
                            {
                                graph[(1, j, k)].Add((0, j-l, k),GetCost(cavemap, j, k, l, -1, 'y'));
                            }
                        }
                    }
                }            
            }
            //lisätään vielä alku- ja loppusolmut
            graph.Add((-1,-1,-1),new Dictionary<(int, int, int), int> {{(0,0,0),0},{(1,0,0),0}});
            graph.Add((-1,0,0),new Dictionary<(int, int, int), int>());
            graph[(0,maxy-1,maxx-1)].Add((-1,0,0),0);
            graph[(1,maxy-1,maxx-1)].Add((-1,0,0),0);
        }

        var graph_route = new List<((int, int, int), int, int)>
        {
            ((-1, -1, -1), 0, maxy + maxx)
        };  //node address, heatloss, f

        //jummijammi, sitten A* käyntiin
        if (phase == 2)
        {
            while (true)
            {
                int minval = graph_route.Min(x => x.Item3);
                int minindex = graph_route.FindIndex(x => x.Item3 == minval);
                var minheat_node = graph_route[minindex];  //(noodin id), heatloss, f 
                graph_route.RemoveAt(minindex);

                //ollaanko tultu maaliin?
                if (minheat_node.Item1 == (-1,0,0))
                {
                    return (BigInteger)minheat_node.Item2;
                }
                //jos ei, jatketaan
                foreach (var nextnode in graph[minheat_node.Item1])
                {
                    var heatloss = minheat_node.Item2 + nextnode.Value;
                    var f = heatloss + GetDist(maxy, maxx, nextnode); //nykyisen noden heatloss + liikkumiskustannus + Manhattan-etäisyys maalista
                    //katsotaan, onko seuraava noodi -tarjokas jo listassa
                    if (!graph_route.Any(x => x.Item1 == nextnode.Key && x.Item3 <= f))
                    {
                        //lisätään noodi listaan
                        var position = nextnode.Key;
                        graph_route.Add((position, heatloss, f));
                    }
                }
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
        route.Add(((0,0),0,0,(0,1),maxx+maxy-2));   //alustetaan lista
        if (phase == 2)  route.Add(((0,0),0,0,(1,0),maxx+maxy-2));   //alustetaan lista


        //ruvetaan menemään A*-algoritmin mukaan
        int straightmax;
        if (phase==1) straightmax = 3;
        else straightmax = 10;
        int straightmin;
        if (phase == 1) straightmin = 0;
        else straightmin = 4;
        while (true)
        {
            //valitaan alkio, jolla on pienin heatloss-arvo
            //poistetaan alkio listasta
            var minheat_node = route[0];
            route.RemoveAt(0);

            //ollaanko tultu maaliin?
            if (minheat_node.Item1 == (maxy-1,maxx-1))
            {
                if (phase == 1 || minheat_node.Item3 >= straightmin)
                {
                    result = minheat_node.Item2;
                    break;
                }
                else continue;

            }
            //jos ei, generoidaan seuraavat noodit
            //suoraan
            if (minheat_node.Item3 < straightmax)
            {
                var direction = minheat_node.Item4;
                var position = minheat_node.Item1;
                var nextpos = (position.Item1 + direction.Item1, position.Item2 + direction.Item2);
                if (phase == 1) NextNode(maxy, maxx, cavemap, route, minheat_node, direction, position, nextpos, true);
                else NextNode2(maxy, maxx, cavemap, route, minheat_node, direction, position, nextpos, true);
            }
            //vasemmalle
            if (minheat_node.Item3 >= straightmin)
            {
                var direction = turn_left[minheat_node.Item4];
                var position = minheat_node.Item1;
                var nextpos = (position.Item1 + direction.Item1, position.Item2 + direction.Item2);
                if (phase == 1) NextNode(maxy, maxx, cavemap, route, minheat_node, direction, position, nextpos, false);
                else NextNode2(maxy, maxx, cavemap, route, minheat_node, direction, position, nextpos, false);
            }
            //oikealle
            if (minheat_node.Item3 >= straightmin)
            {
                var direction = turn_right[minheat_node.Item4];
                var position = minheat_node.Item1;
                var nextpos = (position.Item1 + direction.Item1, position.Item2 + direction.Item2);
                if (phase == 1) NextNode(maxy, maxx, cavemap, route, minheat_node, direction, position, nextpos, false);
                else NextNode2(maxy, maxx, cavemap, route, minheat_node, direction, position, nextpos, false);
            }
        }

        return result;
    }

    private static int GetDist(int maxy, int maxx, KeyValuePair<(int, int, int), int> nextnode)
    {
        return (maxx + maxy - 2 - nextnode.Key.Item2 - nextnode.Key.Item3);
    }

    private static int GetCost(int[,] cavemap, int j, int k, int l, int direction, char axis)
    {
        int cost = 0;
        for (var m = 1; m <= l; m++)
        {
            if (axis == 'x')    cost += cavemap[j, k + (direction * m)];
            else if (axis == 'y')    cost += cavemap[j + (direction * m), k];
            else throw new Exception("Ei kunnollista argumenttia akselille!");
        }
        return cost;
    }

    private static bool GoodNode(int maxy, int maxx, int j, int l)
    {
        return !(j < 0 || l < 0 || j >= maxy || l >= maxx);
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

        private static void NextNode2(int maxy, int maxx, int[,] cavemap, List<((int, int), int, int, (int, int), int)> route, ((int, int), int, int, (int, int), int) minheat_node, (int, int) direction, (int, int) position, (int, int) nextpos, bool isstraight)
    {
        if (Goodpos(nextpos, maxy, maxx))
        {
            var heatval = minheat_node.Item2 + cavemap[nextpos.Item1, nextpos.Item2];
            int straight;
            if (isstraight) straight = minheat_node.Item3 + 1;
            else straight = 1;
            var t = maxy - nextpos.Item1 - 1 + maxx - nextpos.Item2 - 1;
            var f = t + heatval;
            if (!route.Any(x => x.Item1 == nextpos && x.Item4 == x.Item4 && x.Item3 == straight && x.Item2 <= heatval))
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

//901 liikaa