namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Text;

partial class Program
{
    static BigInteger Day25(int phase, string datafile)
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

        // Käytetään hajautusjoukkojen hajautusjoukkoja, niin järjestyksestä ei tarvitse välittää
        var connections = new HashSet<HashSet<string>>();

        foreach (string line in lines)
        {
            var splitline = line.Split(' ');
            var leftnode = splitline[0].TrimEnd(':');
            foreach (string rightnode in splitline[1..])
            {
                var connection = new HashSet<string> { leftnode, rightnode };
                connections.Add(connection);
            }
        }

        var nodes = new HashSet<string>();
        var connectionList = new List<List<string>>();

        foreach (var connection in connections)
        {
            nodes = nodes.Concat(connection).ToHashSet();
            connectionList.Add(connection.ToList());
        }

        var edges = new Dictionary<string, Dictionary<string, bool>>();

        // lähtösolmu, kohdesolmu, saako käyttää

        foreach (var connection in connectionList)
        {
            foreach (var node in connection)
            {
                foreach (var othernode in connection.Where(x => x != node))
                {
                    if (edges.ContainsKey(node))
                    {
                        if (!edges[node].ContainsKey(othernode))
                            edges[node].Add(othernode, true);
                    }
                    else
                        edges.Add(node, new Dictionary<string, bool> { { othernode, true } });
                    if (edges.ContainsKey(othernode))
                    {
                        if (!edges[othernode].ContainsKey(node))
                            edges[othernode].Add(node, true);
                    }
                    else
                        edges.Add(othernode, new Dictionary<string, bool> { { node, true } });
                }
            }
        }

        var s = edges.First().Key;
        int sSide = 1;
        int otherSide = 0;

        foreach (var goaledge in edges.Where(x => x.Key != s).ToDictionary().Keys)
        {
            if (FindConnectivity(edges, s, goaledge) == 3) otherSide++;
            else sSide++;
        }

        return otherSide * sSide;
    }

    private static int FindConnectivity(Dictionary<string, Dictionary<string, bool>> edges, string s, string goaledge)
    {
        int connectivity = 0;
        // Yhdistettävyys voi olla maksimissaan maalisolmusta lähtevien polkujen määrä.
        int connectivityMax = edges[goaledge].Count;
        // Resetoidaan linkit
        foreach (var edge in edges.Values)
        {
            foreach (var goal in edge.Keys)
                edge[goal] = true;
        }

        // käytetään Fordin–Fulkersonin algoritmia

        while (true)
        {
            var route = new List<List<string>>
            {
                new List<string> { s }
            };
            bool foundroute = false;

            // etsitään BFS-haulla lyhin reitti maalisolmuun
            
            while (!foundroute)
            {
                var nextroutes = new List<List<string>>();
                foreach (var thisroute in route)
                {
                    var newroutes = new List<string>();
                    var laststep = thisroute.LastOrDefault();
                    foreach (var target in edges[laststep].Keys)
                    {
                        if (edges[laststep][target] && !thisroute.Contains(target) && !route.Any(x => x.Contains(target)))
                        {
                            if (target == goaledge)
                            {
                                // Jos on päästy maaliin
                                thisroute.Add(target);
                                for (var i=0; i<thisroute.Count-1;i++)
                                {
                                    edges[thisroute[i]][thisroute[i+1]] = false;
                                    edges[thisroute[i+1]][thisroute[i]] = false;
                                }
                                connectivity++;
                                if (connectivity == connectivityMax)
                                    return connectivity;
                                foundroute = true;
                                break;
                            }
                            newroutes.Add(target);
                        }
                    }
                    if (foundroute)
                        break;
                    foreach (var newroute in newroutes)
                    {
                        var addable = thisroute.ToList();
                        addable.Add(newroute);
                        nextroutes.Add(addable);
                    }
                }
                // Jos ei päästä enää eteenpäin eikä olla maalissakaan, kaikki reitit on tutkittu
                if (foundroute)
                    break;
                if (nextroutes.Count == 0)
                    return connectivity;
                route = nextroutes.ToList();
            }
        }
        return connectivity;
    }
}