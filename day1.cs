namespace AoC2023;

using System.Collections;
using System.Collections.Generic;

partial class Program
{
    static int Day1(int phase, string datafile)
    {
        string[] lines = new string[] {};
        Dictionary<string,char> numberwords = new Dictionary<string, char>();

        numberwords.Add("one",'1');
        numberwords.Add("two",'2');
        numberwords.Add("three",'3');
        numberwords.Add("four",'4');
        numberwords.Add("five",'5');
        numberwords.Add("six",'6');
        numberwords.Add("seven",'7');
        numberwords.Add("eight",'8');
        numberwords.Add("nine",'9');

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

        //varsinainen suoritus
        int result = 0;


        foreach (string line in lines)
        {
            char[] fixedline = line.ToCharArray();
            //parsitaan sanat pois jos niitä on
            if(phase == 2)
            {
                for(int i=0;i<line.Length;i++)
                {
                    foreach(KeyValuePair<string,char> numberword in numberwords)
                    {
                        if(new string(fixedline).ToLower().IndexOf(numberword.Key) == i)
                        {
                            fixedline[i] = numberword.Value;
                            break;
                        }
                    }
                }
            }

            char firstdigit = (char)0, lastdigit = (char)0;
            foreach (char character in fixedline)
            {
                int charnumval = character - '0';
                if(charnumval >= 0 & charnumval <10)
                {
                    firstdigit = character;
                    break;
                }
            }
            for(int i = fixedline.Length-1; i >= 0; i--)
            {
                char character = fixedline[i];
                int charnumval = character - '0';
                if(charnumval >= 0 & charnumval <10)
                {
                    lastdigit = character;
                    break;
                }
            }
            string str_nextnum = firstdigit.ToString() + lastdigit.ToString();
            int nextval = int.Parse(str_nextnum);
            result += nextval;
        }
        return result;
    }
}