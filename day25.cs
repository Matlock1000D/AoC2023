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

        var edges = new Dictionary<string, HashSet<string>>();

        foreach (var connection in connectionList)    // Tätä varmaan voisi optimoida
        {
            foreach (var node in connection)
            {
                foreach (var othernode in connection.Where(x => x != node))
                {
                    if (edges.ContainsKey(node))
                        edges[node].Add(othernode);
                    else
                        edges.Add(node,[othernode]);
                    if (edges.ContainsKey(othernode))
                        edges[othernode].Add(node);
                    else
                        edges.Add(othernode,[node]);
                }
            }
        }

        for (var i = 0; i<connectionList.Count; i++)
        {
            for (var j = i+1; j<connectionList.Count; j++)
            {
                for (var k = j+1; k<connectionList.Count; k++)
                {

                    var linkednodes = new HashSet<string>();
                    linkednodes.Add(edges.First().Key);
                    HashSet<string> newnodes = [linkednodes.First()];

                    while (newnodes.Count > 0)
                    {
                        var new_newnodes = new HashSet<string>();
                        foreach (var newnode in newnodes)
                        {
                            foreach (var linkednode in edges[newnode])
                            {
                                if ((newnode == connectionList[i][0] && linkednode == connectionList[i][1]) || (newnode == connectionList[i][1] && linkednode == connectionList[i][0]) || (newnode == connectionList[j][0] && linkednode == connectionList[j][1]) || (newnode == connectionList[j][1] && linkednode == connectionList[j][0]) || (newnode == connectionList[k][0] && linkednode == connectionList[k][1]) || (newnode == connectionList[k][1] && linkednode == connectionList[k][0]))
                                    continue;
                                if (!linkednodes.Contains(linkednode))
                                    new_newnodes.Add(linkednode);
                                linkednodes.Add(linkednode);
                            }
                        }
                        newnodes = new_newnodes.ToHashSet();
                    }
                    if (linkednodes.Count < nodes.Count)
                        return linkednodes.Count * (nodes.Count-linkednodes.Count);
                }
            }
        }

        return result;
    }
}