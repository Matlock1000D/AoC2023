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
        for (var i = 0; i < 3; i++)
        {
            var hail = hails[i];
            var arrayline = ((double)hail.pos.x, (double)hail.vel.x, (double)hail.pos.y, (double)hail.vel.y, (double)hail.pos.z, (double)hail.vel.z);
            hailarraylist.Add(arrayline);
        }
        constanthails = hailarraylist.ToArray();

        if (phase == 2)
        {
            // alkuperäinen arvaus
            double[] F = [100000000000000, 10000, 100000000000000, 10000, 100000000000000,10000,100000,100000,100000]; //x, x', y, y', z, z', t1, t2, t3
            // Funktiovektori
            Func<double, double, double, double, double, double, double, double, double, double>[] funcs = 
            {F1, F2, F3, F4, F5, F6, F7, F8, F9};

            // Osittaisderivaattavektori:
            Func<double, double, double, double, double, double, double, double, double, double>[,] d_funcs = {
                {D1, T1, D0, D0, D0, D0, X01, D0, D0},
                {D0, D0, D1, T1, D0, D0, Y01, D0, D0},
                {D0, D0, D0, D0, D1, T1, Z01, D0, D0},
                {D1, T2, D0, D0, D0, D0, D0, X02, D0},
                {D0, D0, D1, T2, D0, D0, D0, Y02, D0},
                {D0, D0, D0, D0, D1, T2, D0, Z02, D0},
                {D1, T3, D0, D0, D0, D0, D0, D0, X03},
                {D0, D0, D1, T3, D0, D0, D0, D0, Y03},
                {D0, D0, D0, D0, D1, T3, D0, D0, Z03}
                };

            while (true)
            {
                //Lasketaan sattuisiko funcs olemaan nollavektori
                bool foundzero = true;
                foreach (var func in funcs)
                {
                    var funcVal = func(F[0], F[1], F[2], F[3], F[4], F[5], F[6], F[7], F[8]);
                    if (funcVal != 0)
                    {
                        foundzero = false;
                        break;
                    }
                }
                if (foundzero)
                    return (BigInteger)(F[0] + F[2] + F[4]);

                // Käytetään Newtonin menetelmää:
                // On ratkaistava yhtälö d_funcs Δx = -funcs(x0)
                // Lasketaan d_funcs(F)
                const int rank = 9;
                var d_f_x = new double[rank,rank];
                for (var i = 0; i < rank; i++)
                {
                    for (var j = 0; j < rank; j++)
                    {
                        d_f_x[i,j] = d_funcs[i,j](F[0], F[1], F[2], F[3], F[4], F[5], F[6], F[7], F[8]);
                    }
                }

                var funcsAtF = new double[1,rank];
                for (var i=0; i<rank ; i++)
                    funcsAtF[0,i] = funcs[i](F[0], F[1], F[2], F[3], F[4], F[5], F[6], F[7], F[8]);
                //sijoitus funciin!

                // Nyt tarvitaan d_f_x:n käänteismatriisi, jolloin haluttu Δx = -1 * (d_funcs)^-1 * funcs (x0)
                var delta_x = MatrixConstMultiplier(-1,MatrixMultiplier(Inverter(d_f_x),funcsAtF));

                // Päivitetään F:ää
                var new_F = new double[rank];

                for (var i=0; i<rank; i++)
                    new_F[i] = F[i] + delta_x[0,i];
                
                F = new_F;
            }
        }
        return -1;

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

    private static double F1(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return x - constanthails[0].x + vx*t1 -constanthails[0].vx * t1;
    }
    
    private static double F2(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return y - constanthails[0].y + vy*t1 -constanthails[0].vy * t1;
    }

    private static double F3(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return z - constanthails[0].z + vz*t1 -constanthails[0].vz * t1;
    }

    private static double F4(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return x - constanthails[1].x + vx*t2 -constanthails[1].vx * t2;
    }
    
    private static double F5(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return y - constanthails[1].y + vy*t2 -constanthails[1].vy * t2;
    }

    private static double F6(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return z - constanthails[1].z + vz*t2 -constanthails[1].vz * t2;
    }

    private static double F7(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return x - constanthails[2].x + vx*t3 -constanthails[2].vx * t3;
    }
    
    private static double F8(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return y - constanthails[2].y + vy*t3 -constanthails[2].vy * t3;
    }

    private static double F9(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return z - constanthails[2].z + vz*t3 -constanthails[2].vz * t3;
    }

    private static double D0(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return 0;
    }

    private static double D1(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return 1;
    }

    private static double T1(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return t1;
    }

    private static double T2(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return t2;
    }

    private static double T3(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return t3;
    }

    private static double X01(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return vx-constanthails[0].vx;
    }
    private static double X02(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return vx-constanthails[1].vx;
    }
    private static double X03(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return vx-constanthails[2].vx;
    }
    private static double Y01(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return vy-constanthails[0].vy;
    }
    private static double Y02(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return vy-constanthails[1].vy;
    }
    private static double Y03(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return vy-constanthails[2].vy;
    }
    private static double Z01(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return vz-constanthails[0].vz;
    }
    private static double Z02(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return vz-constanthails[1].vz;
    }
    private static double Z03(double x, double vx, double y, double vy, double z, double vz, double t1, double t2, double t3)
    {
        return vz-constanthails[2].vz;
    }

    private static double DetCalculator(double[,] johnMatrix)
    {
        if (johnMatrix.GetLength(0) != johnMatrix.GetLength(1)) throw new Exception("Ei ole neliömatriisi");

        int rank = johnMatrix.GetLength(0);
        if (rank == 1)
            return johnMatrix[0,0];
        
        double determinant = 0;
        for (var i = 0; i < rank; i++)
        {
            var c = johnMatrix[0, i];
            if (c == 0)
                continue;
            double[,] minor = GetMinor(johnMatrix, 0, i);
            determinant += Math.Pow(-1, i) * c * DetCalculator(minor);
        }
        return determinant;
    }

    private static double[,] GetMinor(double[,] johnMatrix, int i, int j)
    {
        if (johnMatrix.GetLength(0) != johnMatrix.GetLength(1)) throw new Exception("Ei ole neliömatriisi");
        int rank = johnMatrix.GetLength(0);
        var minor = new double[rank - 1, rank - 1];
        for (var k = 1; k < rank; k++)
        {
            for (var l = 1; l < rank; l++)
            {
                int mi, mj;
                if (k < i)
                    mi = k;
                else
                    mi = k-1;
                if (l < j)
                    mj = l;
                else
                    mj = l-1;

                minor[mi,mj] = johnMatrix[j, k];
            }
        }
        return minor;
    }

    private static double[,] Comatrixer(double[,] johnMatrix)
    {
        if (johnMatrix.GetLength(0) != johnMatrix.GetLength(1)) throw new Exception("Ei ole neliömatriisi");
        int rank = johnMatrix.GetLength(0);

        var comatrix = new double[rank,rank];

        for (var i = 0; i < rank ; i++)
        {
            for (var j = 0; j < rank; j++)
            {
                var cofactor = Math.Pow(-1,i+j) * DetCalculator(GetMinor(johnMatrix, i, j));
                comatrix[i,j] = cofactor;
            }
        }

        return comatrix;
    }

    private static double[,] Inverter(double[,] johnMatrix)
    {
        if (johnMatrix.GetLength(0) != johnMatrix.GetLength(1)) throw new Exception("Ei ole neliömatriisi");
        int rank = johnMatrix.GetLength(0);

        var invMatrix = new double[rank,rank];

        var det = DetCalculator(johnMatrix);
        var adjungate = Comatrixer(johnMatrix);

        for (var i = 0; i < rank; i++)
        {
            for (var j=0; j < rank; j++)
            {
                invMatrix[i,j] = adjungate[i,j]/det;
            }
        }
        return invMatrix;
    }

    private static double[,] MatrixConstMultiplier (double multiplier, double[,] johnMatrix)
    {
        int maxi = johnMatrix.GetLength(0);
        int maxj = johnMatrix.GetLength(1);

        var newMatrix = new double[maxi,maxj];

        for (var i=0;i<maxi;i++)
            for (var j=0;j<maxj;j++)
                newMatrix[i,j] = multiplier * johnMatrix[i,j];
        
        return newMatrix;
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