namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Collections.Specialized;

partial class Program
{
    static (double x, double vx, double y, double vy, double z, double vz)[] constanthails = Array.Empty<(double x, double vx, double y, double vy, double z, double vz)>();
    static BigInteger Day24(int phase, string datafile)
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

        // Parsitaan syöte

        var hails = new List<((BigInteger x, BigInteger y, BigInteger z) pos, (BigInteger x, BigInteger y, BigInteger z) vel)>();


        foreach (string line in lines)
        {
            var splitstring = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var pos_x = BigInteger.Parse(splitstring[0].Trim(','));
            var pos_y = BigInteger.Parse(splitstring[1].Trim(','));
            var pos_z = BigInteger.Parse(splitstring[2].Trim(','));
            var vel_x = BigInteger.Parse(splitstring[4].Trim(','));
            var vel_y = BigInteger.Parse(splitstring[5].Trim(','));
            var vel_z = BigInteger.Parse(splitstring[6].Trim(','));
            hails.Add(((pos_x, pos_y, pos_z),(vel_x, vel_y, vel_z)));
        }

        var hailarraylist = new List<(double x, double vx, double y, double vy, double z, double vz)>();
        foreach (var i in new int[]{0,1,2,4})
        {
            var hail = hails[i];
            var arrayline = ((double)hail.pos.x, (double)hail.vel.x, (double)hail.pos.y, (double)hail.vel.y, (double)hail.pos.z, (double)hail.vel.z);
            hailarraylist.Add(arrayline);
        }
        constanthails = hailarraylist.ToArray();

        var hailarraylist_int = new List<(BigInteger x, BigInteger vx, BigInteger y, BigInteger vy, BigInteger z, BigInteger vz)>();
        for (var i = 0; i < hails.Count; i++)
        {
            var hail = hails[i];
            var arrayline = (hail.pos.x, hail.vel.x, hail.pos.y, hail.vel.y, hail.pos.z, hail.vel.z);
            hailarraylist_int.Add(arrayline);
        }
        var constanthails_int = hailarraylist_int.ToArray();


        if (phase == 2)
        {
            // Ratkaisu johdettu kynällä ja paperilla.
        }

        double areamin, areamax;
        if (datafile[^12..] == "demo24-1.txt")
        {
            areamin = 7;
            areamax = 27;
        }
        else
        {
            areamin = 200000000000000;
            areamax = 400000000000000;
        }

        // Toivotaan, että ensimmäiset kolme raetta ovat lineaarisesti riippumattomia:
        // Silmukoidaan raeparit
        for (var i=0; i<hails.Count - 1; i++)
        {
            for (var j=i+1; j<hails.Count; j++)
            {
                // Etsitään leikkauspiste (kaava johdettu kynällä ja paperilla...)
                
                double px1 = (double)hails[i].pos.x;
                double py1 = (double)hails[i].pos.y;
                double vx1 = (double)hails[i].vel.x;
                double vy1 = (double)hails[i].vel.y;
                double px2 = (double)hails[j].pos.x;
                double py2 = (double)hails[j].pos.y;
                double vy2 = (double)hails[j].vel.y;
                double vx2 = (double)hails[j].vel.x;

                // Leikkauspisteen koordinaatit:
                double x, y;

                if (vx1 == 0 && vx2 == 0)
                {
                    // Joko eivät leikkaa tai ovat ikuisesti samassa tilassa.
                    // Oletetaan, että jälkimmäisiä tilanteita ei ole.
                    continue;
                }

                // Tapaukset, joissa joku nopeuksista on 0 (oletetaan tässä, että max. yksi kerrallaan on):

                if (vx1 == 0)
                {
                    y = vy1*(px2-px1)/vx1 + py1;
                    x = px1;
                }
                else if (vx2 == 0)
                {
                    y = vy2*(px1-px2)/vx2 + py2;
                    x = px2;
                }
                else if (vy1 == 0)
                {
                    x = vx1*(py2-py1)/vy1 + px1;
                    y = py1;
                }
                else if (vy2 == 0)
                {
                    x = vx2*(py1-py2)/vy2 + px2;
                    y = py2;
                }
                else
                {
                    var divisor = vy1/vx1 - vy2/vx2;
                    // Samansuuntaisten rakeiden radat eivät leikkaa:
                    if (divisor == 0)
                        continue;
                    x = (px1*vy1/vx1 - py1 - px2*vy2/vx2 + py2)/divisor;
                    y = (x-px1)*vy1/vx1+py1;
                }
                
                // Katsotaan, onko leikkauspiste testialueella:
                if (x < areamin || x > areamax || y < areamin || y > areamax)
                    continue;

                // Katsotaan, onko leikkauspiste tulevaisuudessa kummankin rakeen kannalta
                double t;
                if (vx1 != 0)
                    t = (x-px1)/vx1;
                else
                    t = (y-py1)/vy1;
                if (t<0)
                    continue;
                if (vx2 != 0)
                    t = (x-px2)/vx2;
                else
                    t = (y-py2)/vy2;
                if (t<0)
                    continue;

                // Jos kaikki ok, pari kelpaa
                result++;
            }
        }
        return result;
    }

    private static BigInteger DetCalculator(BigInteger[,] johnMatrix)
    {
        if (johnMatrix.GetLength(0) != johnMatrix.GetLength(1)) throw new Exception("Ei ole neliömatriisi");

        int rank = johnMatrix.GetLength(0);
        if (rank == 1)
            return johnMatrix[0,0];
        
        BigInteger determinant = 0;
        for (var i = 0; i < rank; i++)
        {
            var c = johnMatrix[0, i];
            if (c == 0)
                continue;
            BigInteger[,] minor = GetMinor(johnMatrix, 0, i);
            determinant += (BigInteger)Math.Pow(-1, i) * c * DetCalculator(minor);
        }
        return determinant;
    }

    private static BigInteger[,] GetMinor(BigInteger[,] johnMatrix, int i, int j)
    {
        if (johnMatrix.GetLength(0) != johnMatrix.GetLength(1)) throw new Exception("Ei ole neliömatriisi");
        int rank = johnMatrix.GetLength(0);
        var minor = new BigInteger[rank - 1, rank - 1];
        for (var k = 0; k < rank; k++)
        {
            if (k == i) continue;
            for (var l = 0; l < rank; l++)
            {
                if (l == j) continue;
                int mi, mj;
                if (k < i)
                    mi = k;
                else
                    mi = k-1;
                if (l < j)
                    mj = l;
                else
                    mj = l-1;

                minor[mi,mj] = johnMatrix[k, l];
            }
        }
        return minor;
    }

    private static BigInteger[,] Comatrixer(BigInteger[,] johnMatrix)
    {
        if (johnMatrix.GetLength(0) != johnMatrix.GetLength(1)) throw new Exception("Ei ole neliömatriisi");
        int rank = johnMatrix.GetLength(0);

        var comatrix = new BigInteger[rank,rank];

        for (var i = 0; i < rank ; i++)
        {
            for (var j = 0; j < rank; j++)
            {
                var cofactor = (BigInteger)Math.Pow(-1,i+j) * DetCalculator(GetMinor(johnMatrix, i, j));
                comatrix[i,j] = cofactor;
            }
        }

        return comatrix;
    }

    private static (BigInteger denumenator, BigInteger[,] adjungate) Inverter(BigInteger[,] johnMatrix)
    {
        if (johnMatrix.GetLength(0) != johnMatrix.GetLength(1)) throw new Exception("Ei ole neliömatriisi");
        int rank = johnMatrix.GetLength(0);

        var invMatrix = new BigInteger[rank,rank];

        var det = DetCalculator(johnMatrix);
        var adjungate = Comatrixer(johnMatrix);

        for (var i = 0; i < rank; i++)
        {
            for (var j=0; j < rank; j++)
            {
                invMatrix[i,j] = adjungate[i,j];
            }
        }
        return (det, invMatrix);
    }

    private static double[,] MatrixMultiplier (double[,] leftMatrix, double[,] rightMatrix)
    {
        int leftCols = leftMatrix.GetLength(0);
        int rightRows = rightMatrix.GetLength(1);
        if (leftCols != rightRows)
            throw new Exception("Ei voi kertoa!");
        
        int cols = rightMatrix.GetLength(0);
        int rows = leftMatrix.GetLength(1);
        
        var multiMatrix = new double[cols,rows];

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                double c = 0;
                for (var k = 0; k < leftCols; k++)
                    c += leftMatrix[k,i] * rightMatrix[j,k];
                multiMatrix[j,i] = c;
            }
        }
        return multiMatrix;
    }
}