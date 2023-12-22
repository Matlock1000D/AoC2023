namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;

public class MegaXmasPart
{
    public List<(int min,int max)> xranges;
    public List<(int min,int max)> mranges;
    public List<(int min,int max)> aranges;
    public List<(int min,int max)> sranges;

    public string address;

    // Taitaa itse asiassa olla turhaa, että nämä ovat listana, koska epäjatkuvia alueita ei voi tulla
    // TODO refaktoroi yhdeksi
    public MegaXmasPart(int maxrange)
    {
        this.xranges = new List<(int min, int max)>{(0,maxrange)};
        this.mranges = new List<(int min, int max)>{(0,maxrange)};
        this.aranges = new List<(int min, int max)>{(0,maxrange)};
        this.sranges = new List<(int min, int max)>{(0,maxrange)};
        address = "in";
    }

    public MegaXmasPart(List<(int min, int max)> xranges, List<(int min, int max)> mranges, List<(int min, int max)> aranges, List<(int min, int max)> sranges, string address)
    {
        this.xranges = xranges.ToList();
        this.mranges = mranges.ToList();
        this.aranges = aranges.ToList();
        this.sranges = sranges.ToList();
        this.address = address;
    }

    public int MinCount()
    {
        var counts = new List<int>{xranges.Count, mranges.Count, aranges.Count, sranges.Count};
        return counts.Min();
    }

    public BigInteger Combinations()
    {
        return (xranges[0].max-xranges[0].min+1)*(mranges[0].max-mranges[0].min+1)*(aranges[0].max-aranges[0].min+1)*(sranges[0].max-sranges[0].min+1);
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
                newparts.Add(step.MegaRunStep(megaxmaspart));
            if (megaxmaspart.MinCount() == 0)
                break;
        }
        if (megaxmaspart.MinCount() > 0 && megaxmaspart.address != "R")
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
        var newranges = new List<(int min, int max)>();
        var oldranges = new List<(int min, int max)>();

        switch (category)
        {
            case 'x':
                foreach (var xrange in megaxmaspart.xranges)
                {
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
                        newranges.Add(((int)newrangemin,(int)newrangemax));
                    if (oldrangemin != null)
                        oldranges.Add(((int)oldrangemin,(int)oldrangemax));
                }
                megaxmaspart.xranges = oldranges.ToList();
                if (newranges.Count > 0)
                    return new MegaXmasPart(newranges.ToList(), megaxmaspart.mranges.ToList(), megaxmaspart.aranges.ToList(), megaxmaspart.sranges.ToList(), target);
                else
                    return null;
                break;
            case 'm':
                foreach (var mrange in megaxmaspart.mranges)
                {
                    int? newrangemin=null, newrangemax=null;
                    int? oldrangemin=null, oldrangemax=null;
                    if (rule == '>')
                    {
                        if (mrange.max > comparer)
                        {
                            newrangemax = mrange.max;
                            if (mrange.min > comparer)
                            {
                                newrangemin = mrange.min;
                            }
                            else
                            {
                                newrangemin = comparer+1;
                                oldrangemax = comparer;
                                oldrangemin = mrange.min;
                            }
                        }
                    }
                    else if (rule == '<')
                    {
                        if (mrange.min < comparer)
                        {
                            newrangemin = mrange.min;
                            if (mrange.max < comparer)
                            {
                                newrangemax = mrange.max;
                            }
                            else
                            {
                                newrangemax = comparer-1;
                                oldrangemin = comparer;
                                oldrangemax = mrange.max;
                            }
                        }
                    }
                    else megaxmaspart.address = target;

                    if (newrangemin != null)
                        newranges.Add(((int)newrangemin,(int)newrangemax));
                    if (oldrangemin != null)
                        oldranges.Add(((int)oldrangemin,(int)oldrangemax));
                }
                megaxmaspart.mranges = oldranges.ToList();
                if (newranges.Count > 0)
                    return new MegaXmasPart(megaxmaspart.xranges.ToList(), newranges.ToList(), megaxmaspart.aranges.ToList(), megaxmaspart.sranges.ToList(), target);
                else
                    return null;
                break;
            case 'a':
                foreach (var arange in megaxmaspart.aranges)
                {
                    int? newrangemin=null, newrangemax=null;
                    int? oldrangemin=null, oldrangemax=null;
                    if (rule == '>')
                    {
                        if (arange.max > comparer)
                        {
                            newrangemax = arange.max;
                            if (arange.min > comparer)
                            {
                                newrangemin = arange.min;
                            }
                            else
                            {
                                newrangemin = comparer+1;
                                oldrangemax = comparer;
                                oldrangemin = arange.min;
                            }
                        }
                    }
                    else if (rule == '<')
                    {
                        if (arange.min < comparer)
                        {
                            newrangemin = arange.min;
                            if (arange.max < comparer)
                            {
                                newrangemax = arange.max;
                            }
                            else
                            {
                                newrangemax = comparer-1;
                                oldrangemin = comparer;
                                oldrangemax = arange.max;
                            }
                        }
                    }
                    else megaxmaspart.address = target;

                    if (newrangemin != null)
                        newranges.Add(((int)newrangemin,(int)newrangemax));
                    if (oldrangemin != null)
                        oldranges.Add(((int)oldrangemin,(int)oldrangemax));
                }
                megaxmaspart.aranges = oldranges.ToList();
                if (newranges.Count > 0)
                    return new MegaXmasPart(megaxmaspart.xranges.ToList(), megaxmaspart.mranges.ToList(), newranges.ToList(), megaxmaspart.sranges.ToList(), target);
                else
                    return null;
                break;
            case 's':
                foreach (var srange in megaxmaspart.sranges)
                {
                    int? newrangemin=null, newrangemax=null;
                    int? oldrangemin=null, oldrangemax=null;
                    if (rule == '>')
                    {
                        if (srange.max > comparer)
                        {
                            newrangemax = srange.max;
                            if (srange.min > comparer)
                            {
                                newrangemin = srange.min;
                            }
                            else
                            {
                                newrangemin = comparer+1;
                                oldrangemax = comparer;
                                oldrangemin = srange.min;
                            }
                        }
                    }
                    else if (rule == '<')
                    {
                        if (srange.min < comparer)
                        {
                            newrangemin = srange.min;
                            if (srange.max < comparer)
                            {
                                newrangemax = srange.max;
                            }
                            else
                            {
                                newrangemax = comparer-1;
                                oldrangemin = comparer;
                                oldrangemax = srange.max;
                            }
                        }
                    }
                    else megaxmaspart.address = target;

                    if (newrangemin != null)
                        newranges.Add(((int)newrangemin,(int)newrangemax));
                    if (oldrangemin != null)
                        oldranges.Add(((int)oldrangemin,(int)oldrangemax));
                }
                megaxmaspart.sranges = oldranges.ToList();
                if (newranges.Count > 0)
                    return new MegaXmasPart(megaxmaspart.xranges.ToList(), megaxmaspart.mranges.ToList(), megaxmaspart.aranges.ToList(), newranges.ToList(), target);
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
        string firstflow = "in";
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