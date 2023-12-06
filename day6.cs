namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Text;

partial class Program
{
    static int Day6(int phase, string datafile)
    {

        string[] lines = [];
        BigInteger result = 1;

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
        var timelimits = new List<BigInteger>();
        var records = new List<BigInteger>();

        //luetaan datat listoihin
        string[] str_timelimits = lines[0].Split(' ',StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (phase == 2)
        {
            var timelimitBuilder = new StringBuilder();
            for (int i = 1; i<str_timelimits.Length;i++) timelimitBuilder.Append(str_timelimits[i]);
            var timelimit2 = timelimitBuilder.ToString();
            str_timelimits = new string[] {"times:",timelimit2};
        }
        for (int i = 1; i < str_timelimits.Length; i++) timelimits.Add(BigInteger.Parse(str_timelimits[i]));

        string[] str_records = lines[1].Split(' ',StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (phase == 2)
        {
            var recordBuilder = new StringBuilder();
            for (int i = 1; i<str_records.Length;i++) recordBuilder.Append(str_records[i]);
            var record2 = recordBuilder.ToString();
            str_records = new string[] {"records:",record2};
        }

        for (int i = 1; i < str_records.Length; i++) records.Add(BigInteger.Parse(str_records[i]));

        //jos aikaraja on t ja napin alaspainoaika x, veneen kulkema matka y = x(t-x) = -x^2+tx
        //2. asteen yhtälön ratkaisukaavalla x = (-t +- sqrt((t)^2-4y))/-2
        //tarpeeksi nopeille ajoille siis x > (t- sqrt(t^2-4y)/)2 ja x < (t + sqrt(t^2-4y))/2

        for (int i=0; i < records.Count;i++)
        {
            double t = (double) timelimits[i];
            double y = (double) records[i];

            BigInteger minx = (BigInteger)((t-Math.Sqrt(t*t-4*y))/2);
            BigInteger maxx = (BigInteger)Math.Ceiling((t+Math.Sqrt(t*t-4*y))/2);

            BigInteger goodrange = maxx-minx-1;
            result *= (BigInteger)goodrange;
        }

        return (int)result;
    }
}