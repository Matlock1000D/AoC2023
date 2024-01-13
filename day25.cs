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
                var connection = new HashSet<string>{leftnode, rightnode};
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

        foreach (var connection in connectionList)    // Tätä varmaan voisi optimoida
        {
            foreach (var node in connection)
            {
                foreach (var othernode in connection.Where(x => x != node))
                {
                    if (edges.ContainsKey(node))
                        edges[node].Add(othernode, true);
                    else
                        edges.Add(node,new Dictionary<string, bool>{{othernode, true}});
                    if (edges.ContainsKey(othernode))
                        edges[othernode].Add(node, true);
                    else
                        edges.Add(othernode,new Dictionary<string, bool>{{node, true}});
                }
            }
        }

        var s = edges.First().Key;

        var route = new List<string>();
        // Koko kara
        foreach (var goaledge in edges.Where(x => x.Key != s).ToDictionary())
        {
            // käytetään Fordin–Fulkersonin algoritmia
            var thisstep = s;
            
            foreach (var nextstep in edges[s].Keys)
            {
                if (edges[s][nextstep])
                    newpath = oldpath.Append(nextstep);
            }
            
        }
        return result;
    }
}