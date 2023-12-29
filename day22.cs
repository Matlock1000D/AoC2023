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
            int dropDistance = dropDistance = dropBlock.z.min - 1;
            for (int j = possibleHits.Count-1; j >= 0; j--)
            {
                ((int min, int max) x, (int min, int max) y, (int min, int max) z) hitBlock = possibleHits[j];
                // Katsotaan törmätäänkö
                if (CheckOverlap (dropBlock, hitBlock))
                    {
                        int thisdrop = dropBlock.z.min - hitBlock.z.max - 1;
                        if (thisdrop < dropDistance) dropDistance = thisdrop;
                    }
            }
            dropBlock.z.min -= dropDistance;
            dropBlock.z.max -= dropDistance;
            blocks[i] = dropBlock;
        }

        // Käydään läpi alhaalta ylös
        if (phase == 1)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                // Katso, onko välittömästi tarkasteltavan palikan yläpuolella siinä kiinni palikkaa
                var possibleHits = blocks.Where(upperBlock => upperBlock.z.min == block.z.max + 1).ToList();    
                possibleHits = possibleHits.Where(upperBlock => CheckOverlap(upperBlock, block)).ToList();

                // Otetaan kiinni olevat palikat tarkasteluun
                bool removable = true;
                foreach (var upperBlock in possibleHits)
                {
                    bool unsupportable = false;
                    if (upperBlock.z.min != upperBlock.z.max) unsupportable = true;
                    // katsotaan, tukeeko joku muu palikka
                    List<((int min, int max) x, (int min, int max) y, (int min, int max) z)> supportBlocks = new List<((int min, int max) x, (int min, int max) y, (int min, int max) z)>();
                    if (!unsupportable)
                    {
                        supportBlocks = blocks.Where(support => support.z.max == upperBlock.z.min - 1 && support != block).ToList();
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
        }
        else
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                result += RemoveChecker(blocks, i);
            }

        }

        return result;
    }

    private static BigInteger RemoveChecker(List<((int min, int max) x, (int min, int max) y, (int min, int max) z)> blocks, int i)
    {
        BigInteger result = 0;
        var block = blocks[i];
        // Katso, onko välittömästi tarkasteltavan palikan yläpuolella siinä kiinni palikkaa
        var possibleHits = blocks.Where(upperBlock => upperBlock.z.min == block.z.max + 1).ToList();
        possibleHits = possibleHits.Where(upperBlock => CheckOverlap(upperBlock, block)).ToList();

        // Otetaan kiinni olevat palikat tarkasteluun
        bool removable = true;
        foreach (var upperBlock in possibleHits)
        {
            bool unsupportable = false;
            if (upperBlock.z.min != upperBlock.z.max) unsupportable = true;
            // katsotaan, tukeeko joku muu palikka
            List<((int min, int max) x, (int min, int max) y, (int min, int max) z)> supportBlocks = new List<((int min, int max) x, (int min, int max) y, (int min, int max) z)>();
            if (!unsupportable)
            {
                supportBlocks = blocks.Where(support => support.z.max == upperBlock.z.min - 1 && support != block).ToList();
                supportBlocks = supportBlocks.Where(sup => CheckOverlap(sup, upperBlock)).ToList();
            }
            if (supportBlocks.Count() == 0)
            {
                // Jos mikään ei tue, tätä palikkaa ei voi poistaa.
                result += 1;
                // tarkista, mitkä muut palikat tippuvat
                result += RemoveChecker(blocks, blocks.IndexOf(upperBlock));
            }
        }
        if (removable) result++;
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