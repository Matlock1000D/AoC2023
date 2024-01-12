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
            BigInteger[][] Vs = new BigInteger[3][];
            BigInteger[][] Ps = new BigInteger[3][]; 
            for (var i = 0; i<3; i++)
            {
                Vs[i] = [hails[i].vel.x,hails[i].vel.y,hails[i].vel.z];
                Ps[i] = [hails[i].pos.x,hails[i].pos.y,hails[i].pos.z];
            }

            double[,] Xmat = new double[3,3];
            BigInteger[,] Dmat = new BigInteger[1,3];

            for (var i=0; i<3; i++)
            {
                int j = (i+1)%3;
                var vdiff = VectorDiff(Vs[i],Vs[j]);
                var pdiff = VectorDiff(Ps[i],Ps[j]);
                var crossVect = CrossProduct(vdiff,pdiff);
                var pVect = CrossProduct(Ps[i],Ps[j]);

                Xmat[0,i] = (double)crossVect[0];
                Xmat[1,i] = (double)crossVect[1];
                Xmat[2,i] = (double)crossVect[2];
                Dmat[0,i] = DotProduct(vdiff, pVect);
            }

            double[,] invVect = Inverter(Xmat);
            BigInteger[,] intVect = new BigInteger[3,3];
            for (var i = 0; i<3 ;i++)
                for (var j=0; j<3; j++)
                    intVect[i,j] = (BigInteger)invVect[i,j];

            var solution = MatrixMultiplierInt(intVect,Dmat);

            return solution[0,0] + solution[0,1] + solution[0,2];

            // Alempana toivottomia ratkaisuyritelmiä


            // Eipäs menekään Newtonin menetelmällä
            // Kaikissa i,j = sarake, rivi, koska olen tyhmä.

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

            // Kerroinmatriisi (analyyttisestä ratkaisusta, kuulemma oikein mutta ei oikein mene laskemalla...

            double[,] coefficients = new double[,]{
                {constanthails[1].vy-constanthails[0].vy,constanthails[2].vy-constanthails[1].vy,constanthails[3].vy-constanthails[2].vy,constanthails[0].vy-constanthails[3].vy},
                {constanthails[0].y-constanthails[1].y,constanthails[1].y-constanthails[2].y,constanthails[2].y-constanthails[3].y,constanthails[3].y-constanthails[0].y},
                {constanthails[0].vx-constanthails[1].vx,constanthails[1].vx-constanthails[2].vx,constanthails[2].vx-constanthails[3].vx,constanthails[3].vx-constanthails[0].vx},
                {constanthails[1].x-constanthails[0].x,constanthails[2].x-constanthails[1].x,constanthails[3].x-constanthails[2].x,constanthails[0].x-constanthails[3].x}
            };

            double[,] constants = new double[,]{{
                -constanthails[0].x*constanthails[0].vy+constanthails[0].vx*constanthails[0].y+constanthails[1].x*constanthails[1].vy-constanthails[1].vx*constanthails[1].y,
                -constanthails[1].x*constanthails[1].vy+constanthails[1].vx*constanthails[1].y+constanthails[2].x*constanthails[2].vy-constanthails[2].vx*constanthails[2].y,
                -constanthails[2].x*constanthails[2].vy+constanthails[2].vx*constanthails[2].y+constanthails[3].x*constanthails[3].vy-constanthails[3].vx*constanthails[3].y,
                -constanthails[3].x*constanthails[3].vy+constanthails[3].vx*constanthails[3].y+constanthails[0].x*constanthails[0].vy-constanthails[0].vx*constanthails[0].y                
            }};

            BigInteger[,] coefficients_int = new BigInteger[,]{
                {constanthails_int[1].vy-constanthails_int[0].vy,constanthails_int[2].vy-constanthails_int[1].vy,constanthails_int[3].vy-constanthails_int[2].vy,constanthails_int[0].vy-constanthails_int[3].vy},
                {constanthails_int[0].y-constanthails_int[1].y,constanthails_int[1].y-constanthails_int[2].y,constanthails_int[2].y-constanthails_int[3].y,constanthails_int[3].y-constanthails_int[0].y},
                {constanthails_int[0].vx-constanthails_int[1].vx,constanthails_int[1].vx-constanthails_int[2].vx,constanthails_int[2].vx-constanthails_int[3].vx,constanthails_int[3].vx-constanthails_int[0].vx},
                {constanthails_int[1].x-constanthails_int[0].x,constanthails_int[2].x-constanthails_int[1].x,constanthails_int[3].x-constanthails_int[2].x,constanthails_int[0].x-constanthails_int[3].x}
            };

            BigInteger[,] constants_int = new BigInteger[,]{{
                -constanthails_int[0].x*constanthails_int[0].vy+constanthails_int[0].vx*constanthails_int[0].y+constanthails_int[1].x*constanthails_int[1].vy-constanthails_int[1].vx*constanthails_int[1].y,
                -constanthails_int[1].x*constanthails_int[1].vy+constanthails_int[1].vx*constanthails_int[1].y+constanthails_int[2].x*constanthails_int[2].vy-constanthails_int[2].vx*constanthails_int[2].y,
                -constanthails_int[2].x*constanthails_int[2].vy+constanthails_int[2].vx*constanthails_int[2].y+constanthails_int[3].x*constanthails_int[3].vy-constanthails_int[3].vx*constanthails_int[3].y,
                -constanthails_int[3].x*constanthails_int[3].vy+constanthails_int[3].vx*constanthails_int[3].y+constanthails_int[0].x*constanthails_int[0].vy-constanthails_int[0].vx*constanthails_int[0].y                
            }};
            for (var i = 0; i < hails.Count; i++)
            {
                for (var j = i+1; j < hails.Count; j++)
                {
                    for (var k = j+1; k < hails.Count; k++)
                    {
                        for (var l = k+1; l < hails.Count; l++)
                        {
                            BigInteger[,] coefficients_temp = new BigInteger[,]{
                {constanthails_int[j].vy-constanthails_int[i].vy,constanthails_int[k].vy-constanthails_int[j].vy,constanthails_int[l].vy-constanthails_int[k].vy,constanthails_int[i].vy-constanthails_int[l].vy},
                {constanthails_int[i].y-constanthails_int[j].y,constanthails_int[j].y-constanthails_int[k].y,constanthails_int[k].y-constanthails_int[l].y,constanthails_int[l].y-constanthails_int[i].y},
                {constanthails_int[i].vx-constanthails_int[j].vx,constanthails_int[j].vx-constanthails_int[k].vx,constanthails_int[k].vx-constanthails_int[l].vx,constanthails_int[l].vx-constanthails_int[i].vx},
                {constanthails_int[j].x-constanthails_int[i].x,constanthails_int[k].x-constanthails_int[j].x,constanthails_int[l].x-constanthails_int[k].x,constanthails_int[i].x-constanthails_int[l].x}
            };
            if (DetCalculatorInt(coefficients_temp) != 0)
                Console.WriteLine($",{i},{j},{k},{l}");
                        }
                    }
                }
            }
            Console.WriteLine(DetCalculatorInt(coefficients_int));

            // vastausvektori = coefficientsin käänteismatriisi * constants
            var solutionMatrix = MatrixMultiplier(Inverter(coefficients),constants);
            result += (BigInteger)Math.Round(solutionMatrix[0,0])+(BigInteger)Math.Round(solutionMatrix[0,2]);

            // Tästä voisi ratkaista elegantimminkin z:n, mutta koska ei säätää, toistetaan ylläoleva prosessi x:lle ja z:lle

            coefficients = new double[,]{
                {constanthails[1].vy-constanthails[0].vy,constanthails[2].vy-constanthails[1].vy,constanthails[3].vy-constanthails[2].vy,constanthails[0].vy-constanthails[3].vy},
                {constanthails[0].y-constanthails[1].y,constanthails[1].y-constanthails[2].y,constanthails[2].y-constanthails[3].y,constanthails[3].y-constanthails[0].y},
                {constanthails[0].vz-constanthails[1].vz,constanthails[1].vz-constanthails[2].vz,constanthails[2].vz-constanthails[3].vz,constanthails[3].vz-constanthails[0].vz},
                {constanthails[1].z-constanthails[0].z,constanthails[2].z-constanthails[1].z,constanthails[3].z-constanthails[2].z,constanthails[0].z-constanthails[3].z}
            };

            constants = new double[,]{{
                -constanthails[0].z*constanthails[0].vy+constanthails[0].vz*constanthails[0].y+constanthails[1].z*constanthails[1].vy-constanthails[1].vz*constanthails[1].y,
                -constanthails[1].z*constanthails[1].vy+constanthails[1].vz*constanthails[1].y+constanthails[2].z*constanthails[2].vy-constanthails[2].vz*constanthails[2].y,
                -constanthails[2].z*constanthails[2].vy+constanthails[2].vz*constanthails[2].y+constanthails[3].z*constanthails[3].vy-constanthails[3].vz*constanthails[3].y,
                -constanthails[3].z*constanthails[3].vy+constanthails[3].vz*constanthails[3].y+constanthails[0].z*constanthails[0].vy-constanthails[0].vz*constanthails[0].y,                
            }};

            solutionMatrix = MatrixMultiplier(Inverter(coefficients),constants);
            result += (BigInteger)Math.Round(solutionMatrix[0,2]);

            return result;
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

    private static BigInteger[,] MatrixMultiplierInt (BigInteger[,] leftMatrix, BigInteger[,] rightMatrix)
    {
        int leftCols = leftMatrix.GetLength(0);
        int rightRows = rightMatrix.GetLength(1);
        if (leftCols != rightRows)
            throw new Exception("Ei voi kertoa!");
        
        int cols = rightMatrix.GetLength(0);
        int rows = leftMatrix.GetLength(1);
        
        var multiMatrix = new BigInteger[cols,rows];

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                BigInteger c = 0;
                for (var k = 0; k < leftCols; k++)
                    c += leftMatrix[k,i] * rightMatrix[j,k];
                multiMatrix[j,i] = c;
            }
        }
        return multiMatrix;
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

    private static BigInteger DetCalculatorInt(BigInteger[,] johnMatrix)
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
            BigInteger[,] minor = GetMinorInt(johnMatrix, 0, i);
            determinant += (BigInteger)Math.Pow(-1, i) * c * DetCalculatorInt(minor);
        }
        return determinant;
    }

    private static BigInteger[,] GetMinorInt(BigInteger[,] johnMatrix, int i, int j)
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

    private static BigInteger[] CrossProduct(BigInteger[] leftMatrix, BigInteger[] rightMatrix)
    {
        BigInteger i1 = leftMatrix[1]*rightMatrix[2] - leftMatrix[2]*rightMatrix[1];
        BigInteger i2 = leftMatrix[2]*rightMatrix[0] - leftMatrix[0]*rightMatrix[2];
        BigInteger i3 = leftMatrix[0]*rightMatrix[1] - leftMatrix[1]*rightMatrix[0];

        return [i1,i2,i3];
    }

    private static BigInteger DotProduct(BigInteger[] leftMatrix, BigInteger[] rightMatrix)
    {
        return leftMatrix[0]*rightMatrix[0] + leftMatrix[1]*rightMatrix[1] + leftMatrix[2]*rightMatrix[2];
    }

    private static BigInteger[] VectorDiff(BigInteger[] left, BigInteger[] right)
    {
        var rank = left.Length;
        
        BigInteger[] result = new BigInteger[rank];
        for (var i=0; i<rank; i++)
        {
            result[i] = left[i]-right[i];
        }
        return result;
    }

}