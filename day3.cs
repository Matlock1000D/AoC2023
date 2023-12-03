namespace AoC2023;

using System.Collections.Generic;
using System.Linq;

partial class Program
{
    static int Day3(int phase, string datafile)
    {
        string[] lines = [];
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

        List<List<char>> schematic = new List<List<char>>();

        foreach(var line in lines)
        {
            schematic.Add(line.ToList());
        }

        //Lähdetään iteroimaan
        int max_x = schematic[0].Count -1;  //oletetaan, että kartta on suorakulmion muotoinen
        int max_y = schematic.Count - 1;

        for(int y = 0;y<=max_y;y++)
        {   
            int curnum = 0;
            int curdigits = 0;
            for(int x = 0;x<=max_x;x++)
            {
                if('0' <= schematic[y][x] & schematic[y][x] <= '9')
                {
                    curnum = 10*curnum + (schematic[y][x] - '0');
                    curdigits++;
                }
                if (curnum > 0 & ('0' > schematic[y][x] | schematic[y][x] > '9'| x==max_x))
                {
                    if (x == max_x & '0' <= schematic[y][x] & schematic[y][x] <= '9') 
                    {
                        x++; //ruma tapa saada kursori seuraavaan ruutuun
                        //curdigits++;
                    }
                    //tarkistetaan, onko luvun ympärillä symboli
                    for(int check_y = y-1;check_y <= y+1;check_y++)
                    {
                        //muista, että x on nyt lukua seuraavassa ruudussa
                        if (check_y < 0 | check_y > max_y) continue;
                        for(int check_x = x-curdigits-1; check_x <= x;check_x++)
                        {
                            if (check_x < 0 | check_x > max_x) continue;
                            if (check_y == y & check_x >= x-curdigits & check_x < x) continue;
                            if (schematic[check_y][check_x] != '.' & (schematic[check_y][check_x] < '0' | schematic[check_y][check_x] > '9'))
                            {
                                //löytyi symboli
                                result += curnum;
                                goto ExitCheck;
                            }
                        }
                    }
                    ExitCheck:;
                    //nollataan curnum
                    curnum = 0;
                    curdigits = 0;
                }
            }
        }

        return result;
    }
}