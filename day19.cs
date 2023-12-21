namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;

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
            else if (inputphase == 2)
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
}
