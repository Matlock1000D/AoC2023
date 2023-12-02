namespace AoC2023;

using System.Collections.Generic;

partial class Program
{
     static int Day2(int phase, string datafile)
    {
        Dictionary<string,int> maxcubes = new Dictionary<string, int>();

        maxcubes.Add("red",12);
        maxcubes.Add("green",13);
        maxcubes.Add("blue",14);

        HashSet<Cubegame> set_cubegames = new HashSet<Cubegame>();

        string[] lines = new string[] {};
        int result = 0;

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
        }

        //luetaan data sisään
        foreach(string line in lines)
        {
            string[] inputline = line.Split(':');
            int game_id = int.Parse(inputline[0].Split(' ')[1]);

            Cubegame cubegame = new Cubegame(game_id);
            string[] shown_games = inputline[1].Split(';');
            foreach(string game in shown_games)
            {
                string[] cubes = game.Split(',');
                HashSet<(string, int)> set_cubes = new HashSet<(string colour, int cubeamount)>();
                foreach(string cubepair in cubes)
                {
                    string[] str_pair = cubepair.Trim().Split(' '); 
                    set_cubes.Add((str_pair[1],int.Parse(str_pair[0])));
                }
                cubegame.AddCubes(set_cubes);
            }
            set_cubegames.Add(cubegame);
        }

        //käydään pelit läpi
        foreach(Cubegame cubegame1 in set_cubegames)
        {
            foreach(HashSet<(string, int)> cubeset in cubegame1.cubesets)
            {
                foreach((string, int) cubenum in cubeset)
                {
                    //Onko liikaa kuutioita
                    if(cubenum.Item2 > maxcubes[cubenum.Item1])
                    {
                        goto Escape;    //paetaan silmukoista jos kuutioita on liikaa
                    }
                }
            }
            //jos virheitä ei ole löytynyt, kasvatetaan tulosta
            result += cubegame1.Id;
            Escape:;

        }
        return result;
    }
}

public class Cubegame
{
    public int Id { get; set; }
    public HashSet<HashSet<(string colour, int cubeamount)>> cubesets { get; set; }
    
    public Cubegame(int id)
    {
        Id = id;
        cubesets = new HashSet<HashSet<(string colour, int cubeamount)>>();
    }
    public void AddCubes(HashSet<(string colour, int cubeamount)> shown_cubes)
    {
        cubesets.Add(shown_cubes);
    }
}