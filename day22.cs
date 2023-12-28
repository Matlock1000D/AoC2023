namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;

partial class Program
{
    static BigInteger Day22(int phase, string datafile)
    {
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

        var blocks = new List<((int min, int max) x, (int min, int max) y, (int min, int max) z)>();

        foreach (string line in lines)
        {
            var splitline = line.Split('~');
            var endLeft = splitline[0].Split(',');
            var endRight = splitline[1].Split(',');
            List<int> xs = [int.Parse(endLeft[0]), int.Parse(endRight[0])];
            List<int> ys = [int.Parse(endLeft[1]), int.Parse(endRight[1])];
            List<int> zs = [int.Parse(endLeft[2]), int.Parse(endRight[2])];
            (int min, int max) xTuple = (xs.Min(), xs.Max());
            (int min, int max) yTuple = (ys.Min(), ys.Max());
            (int min, int max) zTuple = (zs.Min(), zs.Max());
            ((int min, int max) x, (int min, int max) y, (int min, int max) z) block = (xTuple, yTuple, zTuple);
            blocks.Add(block);
        }

        // Järjestetään palikat yläreunan mukaiseen korkeusjärjestykseen
        blocks = blocks.OrderBy(x => x.z.max).ToList();

        // Tehdään toinen lista, jossa ne ovat alareunan mukaisessa järjestyksessä
        var blocksByLower = OrderBlocks(blocks);

        // Pudota palikat
        for (int i = 0; i < blocksByLower.Count; i++)
        {
            int index = blocksByLower[i].Item1;
            var dropBlock = blocksByLower[i].block;
            int height = blocksByLower[i].block.z.min;
            var possibleHits = blocks.Where(block => block.z.max < height).ToList();
            bool hit = false;
            for (int j = possibleHits.Count-1; j >= 0; j--)
            {
                ((int min, int max) x, (int min, int max) y, (int min, int max) z) hitBlock = possibleHits[j];
                // Katsotaan törmätäänkö
                if (CheckOverlap (dropBlock, hitBlock))
                    {
                        int dropDistance = dropBlock.z.min - hitBlock.z.max - 1;
                        dropBlock.z.max -= dropDistance;
                        dropBlock.z.min -= dropDistance;
                        hit = true;
                        break;
                    }
            }
            if (!hit)
            {
                int dropDistance = dropBlock.z.min - 1;
                dropBlock.z.min -= dropDistance;
                dropBlock.z.max -= dropDistance;
            }
            blocks[i] = dropBlock;
        }

        // Tarkista, mitä voi poistaa:
        // Uudelleenjärjestä listat
        blocks = blocks.OrderBy(x => x.z.max).ToList();
        blocksByLower = OrderBlocks(blocks);
        // Käydään läpi alhaalta ylös
        for (int i = 0; i < blocks.Count; i++)
        {
            var block = blocks[i];
            // Katso, onko välittömästi tarkasteltavan palikan yläpuolella siinä kiinni palikkaa
            var possibleHits = blocks.Where(upperBlock => upperBlock.z.min == block.z.max + 1).ToList();    
            possibleHits = possibleHits.Where(upperBlock => CheckOverlap(upperBlock, block)).ToList();  // (1,4),(0,0),(2,2) on, samoin (0,0) (0,0) (2,4), vaikuttaa toimivan ok

            // Otetaan kiinni olevat palikat tarkasteluun
            bool removable = true;
            foreach (var upperBlock in possibleHits)    // (1,4) (0,0) (2,2):ta ei voi poistaa, kuten pitäisikin
            {
                bool unsupportable = false;
                if (upperBlock.z.min != upperBlock.z.max) unsupportable = true;
                // katsotaan, tukeeko joku muu palikka
                List<((int min, int max) x, (int min, int max) y, (int min, int max) z)> supportBlocks = new List<((int min, int max) x, (int min, int max) y, (int min, int max) z)>();
                if (!unsupportable)
                {
                    supportBlocks = blocks.Select((value, index) => (Value: value, Index: index)).Where(support => support.Value.z.max == upperBlock.z.min - 1 && support.Index != i).Select(tuple => tuple.Value).ToList();                  
                    supportBlocks = supportBlocks.Where(sup => CheckOverlap(sup, upperBlock)).ToList();
                }
                if (supportBlocks.Count() == 0)
                {
                    // Jos mikään ei tue, tätä palikkaa ei voi poistaa.
                    removable = false;
                    break;
                }
            }
            if (removable) result++;
        }

        return result;
    }

    private static bool CheckOverlap(((int min, int max) x, (int min, int max) y, (int min, int max) z) upperBlock, ((int min, int max) x, (int min, int max) y, (int min, int max) z) block)
    {
        return ((upperBlock.x.min >= block.x.min && upperBlock.x.min <= block.x.max) || (upperBlock.x.max >= block.x.min && block.x.max >= upperBlock.x.max) || (upperBlock.x.min >= block.x.min && upperBlock.x.max <= block.x.max) || (block.x.min >= upperBlock.x.min && block.x.max <= upperBlock.x.max)) && ((upperBlock.y.min >= block.y.min && upperBlock.y.min <= block.y.max) || (upperBlock.y.max >= block.y.min && block.y.max >= upperBlock.y.max) || (upperBlock.y.min >= block.y.min && upperBlock.y.max <= block.y.max) || (block.y.min >= upperBlock.y.min && block.y.max <= upperBlock.y.max));
    }

    private static List<(int label, ((int min, int max) x, (int min, int max) y, (int min, int max) z) block)> OrderBlocks(List<((int min, int max) x, (int min, int max) y, (int min, int max) z)> blocks)
    {
        var blocksByLower = new List<(int label, ((int min, int max) x, (int min, int max) y, (int min, int max) z) block)>();
        for (int i = 0; i < blocks.Count; i++)
        {
            ((int min, int max) x, (int min, int max) y, (int min, int max) z) block = blocks[i];
            blocksByLower.Add((i, block));
        }
        blocksByLower = blocksByLower.OrderBy(x => x.block.z.min).ToList();
        return blocksByLower;
    }
}

// 449 too high
// 445 too high