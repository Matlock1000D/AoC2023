namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Text;

public class HandComparer : IComparer<(string,int)>
{
    private int comparetype;
    
    static List<char> cardlist = new List<char> {'A','K','Q','J','T','9','8','7','6','5','4','3','2'};  //ei varmaan tyylikästä määritellä tätä täällä, mutta olkoon

    public HandComparer(int phase)
    {
        comparetype = phase;
        if (comparetype == 2) cardlist = new List<char> {'A','K','Q','T','9','8','7','6','5','4','3','2','J'};  //ei varmaan tyylikästä määritellä tätä täällä, mutta olkoon
    }

    static int ClassifyHand(string hand, int comparetype) //luokittelee kädet
        {
            var cards = new List<char>();
            foreach(var character in hand) cards.Add(character);

            //Kädet:
            //viisi samaa = 0
            //neljä samaa = 1
            //täyskäsi = 2
            //kolme samaa = 3
            //kaksi paria = 4
            //pari = 5
            //hai = 6

            if (comparetype == 2)
            {
                //mitä ei-jokerikorttia on eniten?
                (char,int) mostcommon = ('J',0);
                foreach(var card in cardlist)
                {
                    if (card == 'J') continue;
                    int handcard = cards.Count(value => value == card);   //mitä ei-jokerikorttia on eniten?
                    if (handcard > mostcommon.Item2) mostcommon = (card, handcard);
                }
                cards = string.Concat(cards).Replace('J',mostcommon.Item1).ToList<char>();
            }

            int cardtypes = cards.Distinct().Count();   //montako erilaista korttia on

            if (cardtypes == 1) return 0;    //viisi samaa
            if (cardtypes == 2)
            {
                foreach (char card in cards.Distinct()) if (cards.Count(item => item == card) == 3) return 2; //täyskäsi
                return 1;    //neljä samaa, jos ei löydy täyskättä
            }
            if (cardtypes == 3)
            {
                foreach (char card in cards.Distinct()) 
                {
                    int amount = cards.Count(item => item == card);
                    if (amount == 3) return 3; //kolme samaa
                    if (amount == 2) return 4; //kaksi paria
                }
            }
            if (cardtypes == 4) return 5;    //pari
            return 6; 
        }

        public int Compare((string, int) handtuple1, (string, int) handtuple2)  //lajittelee heikoimman käden ensiksi
        {
            int class1, class2;
            string hand1 = handtuple1.Item1;
            string hand2 = handtuple2.Item1;
            class1 = ClassifyHand(hand1, comparetype);
            class2 = ClassifyHand(hand2, comparetype);

            if (class1 > class2) return -1;
            if (class2 > class1) return 1;

            for(int i=0;i<5;i++)    //jos tasapeli, verrataan kortteja
            {
                if (cardlist.IndexOf(hand1[i]) > cardlist.IndexOf(hand2[i])) return -1;
                else if (cardlist.IndexOf(hand1[i]) < cardlist.IndexOf(hand2[i])) return 1;
            }

            return 0;
        }
}

partial class Program
{
    static int Day7(int phase, string datafile)
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
            return -1;
        }

        //Käsitellään rivit
        var list_cards = new List<(string,int)>();

        foreach(var line in lines)
        {
            string[] split_line = line.Split(' ');
            list_cards.Add((split_line[0],int.Parse(split_line[1])));
        }

        list_cards.Sort(new HandComparer(phase));    //lajittelee heikoimman ensiksi

        for (int i = 0; i<list_cards.Count;i++) result += list_cards[i].Item2 * (i+1);

        return result;
    }
}