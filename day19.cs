namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;

public class MegaXmasPart
{
    public (int min,int max) xrange;
    public (int min,int max) mrange;
    public (int min,int max) arange;
    public (int min,int max) srange;

    public string address;

    // Taitaa itse asiassa olla turhaa, että nämä ovat listana, koska epäjatkuvia alueita ei voi tulla
    public MegaXmasPart(int maxrange)
    {
        this.xrange = (1,maxrange);
        this.mrange = (1,maxrange);
        this.arange = (1,maxrange);
        this.srange = (1,maxrange);
        address = "in";
    }

    public MegaXmasPart((int min, int max) xrange, (int min, int max) mrange, (int min, int max) arange, (int min, int max) srange, string address)
    {
        this.xrange = xrange;
        this.mrange = mrange;
        this.arange = arange;
        this.srange = srange;
        this.address = address;
    }

    public bool MinCount()
    {
        if (xrange.min < 0 || mrange.min < 0 || arange.min < 0 || srange.min < 0) return true;
        else return false;
    }

    public BigInteger Combinations()
    {
        return ((BigInteger)xrange.max-(BigInteger)xrange.min+1)*((BigInteger)mrange.max-(BigInteger)mrange.min+1)*((BigInteger)arange.max-(BigInteger)arange.min+1)*((BigInteger)srange.max-(BigInteger)srange.min+1);
    }
}
public class XmasPart
{
    public int x,m,a,s;

    public XmasPart(int x, int m, int a, int s)
    {
        this.x = x;
        this.m = m;
        this.a = a;
        this.s = s;
    }

    public int Sum()
    {
        return this.x+this.m+this.a+this.s;
    }
}
public class Workflow
{
    public string? Run(XmasPart xmaspart)
    {
        foreach (var step in steps)
        {
            string? stepresult = step.RunStep(xmaspart);
            if (stepresult != null) return stepresult;
        }
        return null;
    }
    List<WorkflowStep> steps;
    public Workflow(List<WorkflowStep> steps)
    {
        this.steps = steps.ToList();
    }

    public List<MegaXmasPart> MegaRun(MegaXmasPart megaxmaspart)
    {        
        var newparts = new List<MegaXmasPart>();
        foreach (var step in steps)
        {
            MegaXmasPart? newpart = step.MegaRunStep(megaxmaspart);
            if (newpart != null)
                newparts.Add(newpart);
            if (megaxmaspart.MinCount())
                break;
        }
        if (!megaxmaspart.MinCount() && megaxmaspart.address != "R")
            newparts.Add(megaxmaspart);

        return newparts;
    }
}

public class WorkflowStep
{
    char category, rule;
    int comparer;
    string target;

    public WorkflowStep(char category, char rule, int comparer, string target)
    {
        this.category = category;
        this.rule = rule;
        this.comparer = comparer;
        this.target = target;
    }

    public MegaXmasPart? MegaRunStep(MegaXmasPart megaxmaspart)
    {
        // Tämän voisi varmaan tehdä tiiviimminkin
        // esim. olion ominaisuuksiin viittaavan sanakirjan
        // avulla
        (int min, int max) newrange = (-1000, -1001);
        (int min, int max) oldrange = (-1000, -1001);

        (int min, int max) xrange = (0,0);
        switch (category)
        {
            case 'x':
                xrange = megaxmaspart.xrange;
                break;
            case 'm':
                xrange = megaxmaspart.mrange;
                break;
            case 'a':
                xrange = megaxmaspart.arange;
                break;
            case 's':
                xrange = megaxmaspart.srange;
                break;
        }

        int? newrangemin=null, newrangemax=null;
        int? oldrangemin=null, oldrangemax=null;
        if (rule == '>')
        {
            if (xrange.max > comparer)
            {
                newrangemax = xrange.max;
                if (xrange.min > comparer)
                {
                    newrangemin = xrange.min;
                }
                else
                {
                    newrangemin = comparer+1;
                    oldrangemax = comparer;
                    oldrangemin = xrange.min;
                }
            }
        }
        else if (rule == '<')
        {
            if (xrange.min < comparer)
            {
                newrangemin = xrange.min;
                if (xrange.max < comparer)
                {
                    newrangemax = xrange.max;
                }
                else
                {
                    newrangemax = comparer-1;
                    oldrangemin = comparer;
                    oldrangemax = xrange.max;
                }
            }
        }
        else megaxmaspart.address = target;

        if (newrangemin != null)
            newrange = ((int)newrangemin,(int)newrangemax);
        if (oldrangemin != null)
            oldrange = ((int)oldrangemin,(int)oldrangemax);

        switch (category)
        {
            case 'x':            
                megaxmaspart.xrange = oldrange;
                if (newrange.min > -1)
                    return new MegaXmasPart(newrange, megaxmaspart.mrange, megaxmaspart.arange, megaxmaspart.srange, target);
                else
                    return null;
                break;
            case 'm':            
                megaxmaspart.mrange = oldrange;
                if (newrange.min > -1)
                    return new MegaXmasPart(megaxmaspart.xrange, newrange, megaxmaspart.arange, megaxmaspart.srange, target);
                else
                    return null;
                break;
            case 'a':            
                megaxmaspart.arange = oldrange;
                if (newrange.min > -1)
                    return new MegaXmasPart(megaxmaspart.xrange, megaxmaspart.mrange, newrange, megaxmaspart.srange, target);
                else
                    return null;
                break;
            case 's':            
                megaxmaspart.srange = oldrange;
                if (newrange.min > -1)
                    return new MegaXmasPart(megaxmaspart.xrange, megaxmaspart.mrange, megaxmaspart.arange, newrange, target);
                else
                    return null;
                break;

        }
        return null;
    }
    
    public string? RunStep(XmasPart xmaspart)
    {
        int partvalue = 0;
        switch (category)
        {
            case 'x':
                partvalue = xmaspart.x;
                break;
            case 'm':
                partvalue = xmaspart.m;
                break;
            case 'a':
                partvalue = xmaspart.a;
                break;
            case 's':
                partvalue = xmaspart.s;
                break;
        }
        if (rule == '>')
        {
            if (partvalue > comparer) return target;
            else
            return null;
        }
        else if (rule == '<')
        {
            if (partvalue < comparer) return target;
            else
            return null;
        }
        else return target;
    }
}
partial class Program
{
    static BigInteger Day19(int phase, string datafile)
    {
        string[] lines = [];
        BigInteger result = 0;

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

        // Parsetetaan syöte
        var workflows = new Dictionary<string,Workflow>();
        var parts = new List<XmasPart>();
        int inputphase = 1;
        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length ==0)
            {
                inputphase = 2;
                continue;
            }
            else if (inputphase == 1)
            {
                var splitline = lines[i].Split('{');
                string label = splitline[0];
                // Toisesta osasta säännöt
                splitline = splitline[1].Trim('}').Split(',');
                // nyt jokainen splitlinen elementti on säännöstö
                var stepslist = new List<WorkflowStep>();
                for (var j = 0; j < splitline.Length-1; j++)
                {
                    // Parsetaan säännöt.
                    char category = splitline[j][0];
                    char rule = splitline[j][1];
                    var ruletarget = splitline[j].Split(':');
                    int comparer = int.Parse(ruletarget[0][2..]);
                    string target = ruletarget[1];
                    // tee sääntöjen alustus
                    stepslist.Add(new WorkflowStep(category,rule,comparer,target));
                }
                    stepslist.Add(new WorkflowStep('0','!',0,splitline[splitline.Length-1]));
                    workflows[label] = new Workflow(stepslist);
            }
            else if (inputphase == 2 && phase == 1)
            {
                char[] trimmers = ['{','}'];
                string[] xmaspartstring = lines[i].Trim(trimmers).Split(',');
                int x = int.Parse(xmaspartstring[0][2..]);
                int m = int.Parse(xmaspartstring[1][2..]);
                int a = int.Parse(xmaspartstring[2][2..]);
                int s = int.Parse(xmaspartstring[3][2..]);
                parts.Add(new XmasPart(x,m,a,s));
            }
        }

        //Suoritetaan arvonta
        if (phase == 1)
        {
            foreach (XmasPart xmaspart in parts)
            {
                string nextflow = "in";
                while(nextflow != "R" && nextflow != "A")
                {
                    nextflow = workflows[nextflow].Run(xmaspart);
                }
                if (nextflow == "A")
                    result += xmaspart.Sum();
            }
            return result;
        }

        // B-kohdan logiikka

        int maxrange = 4000;
        var megaxmasparts = new List<MegaXmasPart>{new MegaXmasPart(maxrange)};
        var goodparts = new List<MegaXmasPart>();

        while (megaxmasparts.Count > 0)
        {
            var nextpart = megaxmasparts[0];
            var newparts = workflows[nextpart.address].MegaRun(nextpart);
            foreach (var newpart in newparts)
            {
                if (newpart.address == "A")
                    goodparts.Add(newpart);
                else if (newpart.address != "R")
                    megaxmasparts.Add(newpart);
            }
            megaxmasparts.RemoveAt(0);
        }

        // Lasketaan lopputulos
        foreach (var goodpart in goodparts)
            result += goodpart.Combinations();
        
        return result;
    }
}

// TODO: Lisää MegaRunStepiin muutkin kuin xranges