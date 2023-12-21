namespace AoC2023;

using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

public class CustomMaths
{
    public static BigInteger Gcd(BigInteger a, BigInteger b)    //Eukleideen algoritmi pienimmän yhteisen monikerran löytämiseen
    {
        if (b > a)
        {
            (a, b) = (b, a);
        }
        //nyt a on varmasti suurempi kuin b

        BigInteger c;
        BigInteger d = 1;
        while(d != 0)
        {
            c = a/b;
            d = a%b;

            a = b;
            b = d;
        }

        return a;
    }
    public static int Gcd(List<int> numbers)      //eihän tätä tarvitakaan
    {
        int gcd = int.MaxValue;
        int compare = numbers[0];
        for(int i=1;i<numbers.Count;i++)
        {
            int pair_gcd = (int)Gcd((BigInteger)compare,(BigInteger)numbers[i]);
            if (pair_gcd < gcd) gcd = pair_gcd;
            if (pair_gcd == 1) break;
        }
        return gcd;
    }
    public static BigInteger Lcm(List<BigInteger> numbers)
    {
        BigInteger lcm = numbers[0];
        for (int i=1;i<numbers.Count;i++)
        {
            lcm = (lcm*numbers[i])/Gcd(lcm,numbers[i]);
        }
        return lcm;
    }
}
partial class Program
{
    static BigInteger Day8(int phase, string datafile)
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

        var nodes = new Dictionary<string,(string,string)>();
        for (int i = 2; i < lines.Length; i++)  //luetaan noodidata
        {
            string? line = lines[i];
            string[] split_line = line.Split(' ');
            string node_id = split_line[0];
            string left_node = split_line[2].Substring(1,split_line[2].Length-2);
            string right_node = split_line[3].Substring(0,split_line[3].Length-1);
            nodes.Add(node_id,(left_node,right_node));    
        }

        BigInteger steps = 0;

        if (phase == 1)
        {
            //alustetaan tilanne
            int ruleindex = 0;
            string currentnode_id = "AAA";

            //käydään kartta läpi
            while (currentnode_id != "ZZZ")
            {
                var currentnode = nodes[currentnode_id];
                if (rules[ruleindex] == 'L') currentnode_id = currentnode.Item1;
                else currentnode_id = currentnode.Item2;
                steps++;
                ruleindex++;
                if (ruleindex >= rules.Length) ruleindex = 0;
            }
        }

        if (phase == 2)
        {
            //alustetaan tilanne
            int ruleindex = 0;
            var currentnodes = new List<string>();
            foreach(var node in nodes) if (node.Key.EndsWith('A')) currentnodes.Add(node.Key);

            //käydään kartta läpi reitti kerrallaan            
            var goalsteps = new List<BigInteger>();
            for (int i = 0; i < currentnodes.Count; i++)
            {                  
                var currentnode = currentnodes[i];
                steps = 0;
                while (!currentnode.EndsWith('Z'))
                {
                    if (rules[ruleindex] == 'L') currentnode = nodes[currentnode].Item1;
                    else currentnode = nodes[currentnode].Item2;
                    steps++;
                    ruleindex++;
                    if (ruleindex >= rules.Length) ruleindex = 0;
                }
                goalsteps.Add(steps);
            }
            //haetaan tulosten pienin yhteinen monikerta
            steps = CustomMaths.Lcm(goalsteps);

        }       
        return steps;
    }
}