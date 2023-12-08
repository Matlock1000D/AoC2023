namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Text;


partial class Program
{
    static int Day8(int phase, string datafile)
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

        char[] rules = lines[0].ToCharArray();

        var nodes = new HashSet<(string,string,string)>();  //tähän olisi kenties joku järkevämpikin rakenne
        for (int i = 2; i < lines.Length; i++)  //luetaan noodidata
        {
            string? line = lines[i];
            string[] split_line = line.Split(' ');
            string node_id = split_line[0];
            string left_node = split_line[2].Substring(1,split_line[2].Length-2);
            string right_node = split_line[3].Substring(0,split_line[3].Length-1);
            (string,string,string) node = (node_id,left_node,right_node);
            nodes.Add(node);    
        }

        //alustetaan tilanne
        int ruleindex = 0;
        int steps = 0;
        string currentnode_id = "AAA";

        //käydään kartta läpi
        while (currentnode_id != "ZZZ")
        {
            var currentnode = nodes.FirstOrDefault(id => id.Item1 == currentnode_id);
            if (rules[ruleindex] == 'L') currentnode_id = currentnode.Item2;
            else currentnode_id = currentnode.Item3;
            steps++;
            ruleindex++;
            if (ruleindex >= rules.Length) ruleindex = 0;
        }

        return steps;
    }
}