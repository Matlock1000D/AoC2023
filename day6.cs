namespace AoC2023;

using System.Collections.Generic;
using System.Linq;

partial class Program
{
    static int Day6(int phase, string datafile)
    {

        string[] lines = [];
        int result = 1;

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

        //alustetaan lista ajoista ja ennätyksistä
        var timelimits = new List<int>();
        var records = new List<int>();

        //luetaan datat listoihin
        string[] str_timelimits = lines[0].Split(' ',StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < str_timelimits.Length; i++) timelimits.Add(int.Parse(str_timelimits[i]));

        string[] str_records = lines[1].Split(' ',StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < str_records.Length; i++) records.Add(int.Parse(str_records[i]));

        //jos aikaraja on t ja napin alaspainoaika x, veneen kulkema matka y = x(t-x) = -x^2+tx
        //2. asteen yhtälön ratkaisukaavalla x = (-t +- sqrt((t)^2-4y))/-2
        //tarpeeksi nopeille ajoille siis x > (t- sqrt(t^2-4y)/)2 ja x < (t + sqrt(t^2-4y))/2

        for (int i=0; i < records.Count;i++)
        {
            double t = (double) timelimits[i];
            double y = (double) records[i];

            int minx = (int)((t-Math.Sqrt(t*t-4*y))/2);
            int maxx = (int)Math.Ceiling((t+Math.Sqrt(t*t-4*y))/2);

            int goodrange = maxx-minx-1;
            result *= (int)goodrange;
        }

        return result;
    }
}