namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;

internal class Beam
{
    public int x,y;
    public (int,int) direction;    //y, x increment
    public Beam(int y, int x, (int,int) direction)
    {
        this.y = y;
        this.x = x;
        this.direction = direction;
    }

    public void Move()
    {
        this.y += direction.Item1;
        this.x += direction.Item2;
    }

    public void HitTile(char tile, List<Beam> beams)
    {
        switch (tile)
        {
            case '/':
                direction = (-direction.Item2,-direction.Item1);
                break;
            case '\\':
                direction = (direction.Item2,direction.Item1);
                break;
            case '|':
                if (direction.Item2 != 0)
                {
                    beams.Add(new Beam(y,x,(-1,0)));
                    direction = (1,0);
                }
                break;
            case '-':
                if (direction.Item1 != 0)
                {
                    beams.Add(new Beam(y,x,(0,-1)));
                    direction = (0,1);
                }
                break;
        }
    }
}

partial class Program
{

    static BigInteger Day16(int phase, string datafile)
    {
        string[] lines = [];
        BigInteger result = 0;

        //suuntasanakirja
        var directiondict = new Dictionary<(int, int), int>
        {
            { (1, 0), 1 },
            { (0, 1), 2 },
            { (-1, 0), 4 },
            { (0, -1), 8 }
        };

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

        var cavemap = new char[maxy, maxx];
        for (int i = 0; i < maxy; i++)
        {
            for (int j = 0; j < maxx; j++)
            {
                cavemap[i, j] = lines[i][j];
            }
        }


        var startconfigurations = new List<(int,int,(int,int))>();
        if (phase == 1) startconfigurations.Add((0,0,(0,1)));
        else
        {
            for (var i=0;i<maxy;i++) startconfigurations.Add((i,0,(0,1)));
            for (var i=0;i<maxx;i++) startconfigurations.Add((maxy-1,i,(-1,0)));
            for (var i=0;i<maxy;i++) startconfigurations.Add((i,maxx-1,(0,-1)));
            for (var i=0;i<maxx;i++) startconfigurations.Add((0,i,(1,0)));
        }

        foreach((int,int,(int,int)) startconfiguration in startconfigurations)
        {
            int[,] energymap = new int[maxy, maxx];
            var beams = new List<Beam>();   //lista säteistä
            beams.Add(new Beam(startconfiguration.Item1,startconfiguration.Item2,startconfiguration.Item3));
            NewMethod(directiondict, maxy, maxx, cavemap, energymap, beams);
            int subresult = energymap.Cast<int>().Count(x => x > 0);
            if (subresult > result) result = subresult;
        }

        return result;

        static void NewMethod(Dictionary<(int, int), int> directiondict, int maxy, int maxx, char[,] cavemap, int[,] energymap, List<Beam> beams)
        {
            while (beams.Count > 0)
            {
                for (int i = 0; i < beams.Count; i++)
                {
                    Beam beam = beams[i];
                    //tarkistetaan ollaanko kartalla
                    if (beam.x < 0 || beam.x >= maxx || beam.y < 0 || beam.y >= maxy)
                    {
                        beams.RemoveAt(i);
                        i--;
                        continue;
                    }

                    //jos tästä on jo menty samaan suuntaan, säteen voi poistaa
                    if ((energymap[beam.y, beam.x] & directiondict[beam.direction]) > 0)
                    {
                        beams.RemoveAt(i);  //ei kaunista, varmaan pitäisi tehdä metodilla
                        i--;
                        continue;
                    }
                    energymap[beam.y, beam.x] = energymap[beam.y, beam.x] | directiondict[beam.direction];
                    beam.HitTile(cavemap[beam.y, beam.x], beams);
                    beam.Move();
                }
            }
        }
    }
}