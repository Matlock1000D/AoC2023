namespace AoC2023;

using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Xml;

partial class Program
{
    static int Hasher(string str_input)
    {
        int hash=0;
        foreach(var letter in str_input)
        {
            hash += letter;
            hash *= 17;
            hash %= 256;
        }

        return hash;
    }


    static BigInteger Day15(int phase, string datafile)
    {
        string[] lines = [];
        BigInteger result = 0;
        var originalresults = new List<int>();

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

        var sequence = lines[0].Split(',');
        result = 0;
        foreach(var step in sequence) result += Hasher(step);

        return result;
    } 
}