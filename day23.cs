namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Collections.Specialized;

partial class Program
{
    static BigInteger Day23(int phase, string datafile)
    {
        // Oletukset (silmäilty datasta):
        // Kaikki mäet ovat risteyksissä
        // Heti mäen jälkeen ei ole uutta mäkeä tai risteystä
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

        BigInteger result = 0;

        // Kartta merkkijonotaulukoksi.

        int maxy = lines.Length;
        int maxx = lines[0].Length;
        var places = new Dictionary<(int y,int x),bool>();

        var hikingMap = new char[maxy,maxx];
        for (int i=0;i<maxy;i++)
        {
            for (int j=0;j<maxx;j++)
            {
                hikingMap[i,j] = lines[i][j];
                // Etsitään samalla aloituspiste
            }
        }

        // Sanakirja suunnille
        var directions = new Dictionary<char, (int, int)>
        {
            {'^',(-1,0)},
            {'v',(1,0)},
            {'<',(0,-1)},
            {'>',(0,1)}
        };

        var endline = lines.Length-1;
        // Tehdään graafikartta sanakirjana: Solmun osoite, sanakirja solmuista, joihin sieltä pääsee ja matkasta sinne.
        var unknowntrails = new Dictionary<(int y, int x), Dictionary<(int y, int x), int>>();
        var trails = new Dictionary<(int y, int x), Dictionary<(int y, int x), int>>();


        // Etsitään lähtöpiste
        var intx = lines[0].IndexOf('.'); 

        // Ruvetaan etsimään solmuja
        unknowntrails.Add((0,intx),new Dictionary<(int y, int x), int>());

        while (unknowntrails.Count > 0)
        {
            var unknowntrail = unknowntrails.First();
            foreach (var direction in directions)
            {
                var nextstep = TupleSum(unknowntrail.Key, direction.Value);
                if (hikingMap[nextstep.y, nextstep.x] == direction.Key)
                {
                    var addedpath = FindNextNode(unknowntrail.Key,direction.Value,directions,hikingMap,endline);
                    unknowntrail.Value.Add(addedpath.Key, addedpath.Value);
                    if (!unknowntrails.Keys.Contains(addedpath.Key) && !trails.Keys.Contains(addedpath.Key))
                        unknowntrails.Add(addedpath.Key, new Dictionary<(int y, int x), int>());
                }
            }
            unknowntrails.Remove(unknowntrail.Key);
        }
    }

    //Etsi lyhin reitti

    private static KeyValuePair<(int y, int x), int> FindNextNode((int y,int x) startnode, (int y, int x) direction, Dictionary<char, (int, int)> directions, char[,] hikingMap, int endline)
    {
        int length = 2;
        (int y, int x) laststep = TupleSum(direction,TupleSum(startnode,direction));
        while(true)
        {  
            var baddirection = TupleProduct((-1,-1),direction);
            foreach (var newdirection in directions.Values)
            {
                if (newdirection == baddirection)
                    continue;
                (int y, int x) nextstep = TupleSum(laststep,newdirection);
                if (hikingMap [nextstep.y, nextstep.x] == '.')
                {
                    length++;
                    direction = newdirection;
                    laststep = nextstep;
                    if (nextstep.y == endline)
                        return new KeyValuePair<(int y, int x), int>((nextstep.y, nextstep.x),length);
                }
                if (directions.Keys.Contains(hikingMap [nextstep.y, nextstep.x]))
                {
                    length+=2;
                    direction = newdirection;
                    laststep = TupleSum(laststep,directions[hikingMap[nextstep.y, nextstep.x]]);
                    return new KeyValuePair<(int y, int x), int>((laststep.y, laststep.x),length);
                }
            }
        }
    }

    private static (int y, int x) TupleSum((int y, int x) tuple1, (int y, int x) tuple2)
    {
        return (tuple1.y+tuple2.y, tuple1.x+tuple2.x);
    }

    private static (int y, int x) TupleProduct((int y, int x) tuple1, (int y, int x) tuple2)
    {
        return (tuple1.y*tuple2.y, tuple1.x*tuple2.x);
    }

}