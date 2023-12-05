namespace AoC2023;

using System.Collections.Generic;
using System.Linq;
using System.Numerics;

partial class Program
{
    static int Day5(int phase, string datafile)
    {

        string[] lines = [];
        BigInteger result = 9999999999999999; //alustetaan tarpeeksi isoon lukuun

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

        var seeds = new HashSet<BigInteger>();
        var maps = new List<List<(BigInteger,BigInteger,BigInteger)>>(); //lista kartoista
        var seedranges = new List<(BigInteger,BigInteger)>();   //Tämä kakkosvaihetta varten

        for (int i = 0; i < lines.Length; i++)
        {
            string? line = lines[i];

            if (i==0)
            {
                string[] str_seed = line.Split(' ',StringSplitOptions.TrimEntries);
                if (phase == 1)
                {
                    for(int j=1;j< str_seed.Length; j++)
                    {
                        seeds.Add(BigInteger.Parse(str_seed[j]));
                    }
                    // Nyt siementen numerot ovat seeds-joukossa
                }
                else
                {
                    for(int j=1;j< str_seed.Length; j+=2) seedranges.Add((BigInteger.Parse(str_seed[j]),BigInteger.Parse(str_seed[j+1])));
                }
            }
            else
            {
                if (line == "") continue;

                if (line[0] >= 'a' & line[0] <= 'z')
                {
                    maps.Add(new List<(BigInteger, BigInteger, BigInteger)>());  //jos rivi alkaa kirjaimella, alkaa uusi kartta
                }
                else if (line[0] >= '0' & line[0] <= '9')
                {
                    var mapnumbers = line.Split(' ',StringSplitOptions.TrimEntries);
                    maps[maps.Count-1].Add((BigInteger.Parse(mapnumbers[0]),BigInteger.Parse(mapnumbers[1]),BigInteger.Parse(mapnumbers[2])));
                }
            }
        }

        //käsitellään kartat
        if (phase == 1)
        {
        foreach(var seed in seeds)
            {
                BigInteger loc = seed;
                for(int j=0;j < maps.Count;j++)
                {
                    for(int k=0;k<maps[j].Count;k++)
                    {
                        if(loc >= maps[j][k].Item2 & loc <maps[j][k].Item2+maps[j][k].Item3)
                        {
                            loc = maps[j][k].Item1 + (loc-maps[j][k].Item2);
                            break;
                        }
                    }
                }
                if (result > loc) result=loc;
            }
        }
        else //kakkosvaiheen toteutus
        {
            for(int j=0;j < maps.Count;j++) //käydään karttakerrokset läpi yksi kerrallaan
            {
                for(int l=0;l<seedranges.Count;l++)
                {
                    for(int k=0;k<maps[j].Count;k++)
                    {
                        if(maps[j][k].Item2 > seedranges[l].Item1 & maps[j][k].Item2 <seedranges[l].Item1 + seedranges[l].Item2) //jos siemenalueen etupää ei mahdu karttaelementtiin, mutta osa mahtuu...
                        {
                            seedranges.Add((seedranges[l].Item1,maps[j][k].Item2-seedranges[l].Item1)); //tehdään alkupäästä oma siemenalueensa
                            seedranges[l] = (maps[j][k].Item2,seedranges[l].Item2-(maps[j][k].Item2-seedranges[l].Item1)); //siirretään mitä voi
                        }
                        if(seedranges[l].Item1 >= maps[j][k].Item2 & seedranges[l].Item1 <maps[j][k].Item2+maps[j][k].Item3)    //jos tarkasteltava siemenalue osuu kartan rangelle niin, että karttaelementin alku <= siemenalueen alku.
                        {
                            BigInteger availablerange = maps[j][k].Item3 - (seedranges[l].Item1 - maps[j][k].Item2); //paljonko tilaa on käytettävissä
                            if (availablerange >= seedranges[l].Item2)  //jos koko alue mahtuu
                            {
                                seedranges[l] = (maps[j][k].Item1 + (seedranges[l].Item1-maps[j][k].Item2),seedranges[l].Item2);
                            }
                            else{
                                seedranges.Add((maps[j][k].Item2+maps[j][k].Item3,seedranges[l].Item2-availablerange));   //tehdään uusi siemenalue niistä siemenistä, jotka eivät mahdu karttaelementin alueelle
                                seedranges[l] = (maps[j][k].Item1 + (seedranges[l].Item1-maps[j][k].Item2),availablerange); //siirretään mitä voi
                            }
                            break;
                        }
                    }
                }

            }
        }



        if (phase == 2)
        {
            foreach(var seedrange in seedranges)
            {
                if (seedrange.Item1 < result) result = seedrange.Item1;
            }
        }

        int result_int = (int)result;
        return result_int;
    }
}