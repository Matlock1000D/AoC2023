namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;
using System.Text;

class Device
{
    public string name;
    public char type;
    Dictionary<string, bool> inputs = new Dictionary<string, bool>();
    public List<string> outputs;
    bool onstate = false;

    public Device(char type, List<string> outputs, string name)  // False == low-tila
    {
        this.type = type;
        this.outputs = outputs;
        this.name = name;
    }

    public void AddInput(string input)
    {
        inputs[input] = false;
    }
    public List<(bool high, string destination, string origin)> Operate(bool high, string input)
    {
        var outputpulses = new List<(bool high, string destination, string origin)>();
        switch (type)
        {
            case '%':
                if (!high)
                    {
                        onstate = !onstate;
                        foreach (var output in outputs)
                        {   
                            outputpulses.Add((onstate, output, name));
                        }                     
                    }
                break;
            case '&':
                inputs[input] = high;
                bool sendhigh;
                if (inputs.Count(x => x.Value == false) > 0) sendhigh = true;
                else sendhigh = false;
                foreach (var output in outputs)
                    {   
                        outputpulses.Add((sendhigh, output, name));
                    }
                break;
            case 'b':
                foreach (var output in outputs)
                {   
                    outputpulses.Add((high, output, name));
                }
                break;
        }
        return outputpulses;
    }
}

partial class Program
{
    static BigInteger Day20(int phase, string datafile)
    {

        string[] lines = [];
        BigInteger result = 1;

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
    
        var circuit = new Dictionary<string, Device>();

        // Kuten aina, parsitaan sisältö
        foreach(string line in lines)
        {
            var splitstring = line.Split(' ');
            char type = splitstring[0][0];
            string label;
            if (type == 'b') label = "broadcaster";
            else
            label = splitstring[0][1..];
            var outputs = new List<string>();
            for (var i=2; i<splitstring.Length; i++)
            {
                outputs.Add(splitstring[i].TrimEnd(','));
            }
            circuit[label] = new Device(type, outputs, label);
        }

        // Täytetään inputit
        foreach(var device in circuit)
        {
            foreach(var output in device.Value.outputs)
            {
                if (circuit.TryGetValue(output, out Device outdevice))
                    if (outdevice.type == '&')
                        outdevice.AddInput(device.Key); 
            }
        }
        var pulses = new List<(bool high, string destination, string origin)>();

        BigInteger lowpulses=0, highpulses = 0;
        // Pannaan homma käyntiin
        if (phase == 1)
        {
            for (var i = 0; i<1000; i++)
            {
                pulses.Add((false,"broadcaster","button"));
                while (pulses.Count > 0)
                {
                    if (circuit.TryGetValue(pulses[0].destination, out Device targetdevice))
                        pulses.AddRange(targetdevice.Operate(pulses[0].high, pulses[0].origin));
                    if (pulses[0].high) highpulses++;
                    else lowpulses++;
                    pulses.RemoveAt(0);
                }
            }
            return lowpulses * highpulses;
        }
        else
        {
        BigInteger i = 0;
        var pulsecounter = new Dictionary<string, int>{{"fg", 0},{"fm", 0},{"dk", 0},{"pq", 0}};    // nämä analysoitu datasta
        var cyclicanalyzer = new List<(int, string, BigInteger)>(); // Monesko korkea pulssi, lähde, painallus

            while (true)
            {
                i++;
                pulses.Add((false,"broadcaster","button"));
                while (pulses.Count > 0)
                {
                    if (circuit.TryGetValue(pulses[0].destination, out Device targetdevice))
                    {
                        if (targetdevice.name == "vr" && pulses[0].high)
                        {
                            pulsecounter[pulses[0].origin] = pulsecounter[pulses[0].origin]+1;      // vaikuttaa siltä, että ensimmäinen ja seuraavat syklit ovat samanmittaiset
                            var j = pulsecounter[pulses[0].origin];
                            cyclicanalyzer.Add((j, pulses[0].origin, i));
                            if (pulsecounter.Values.Min() == 1)
                            {
                                var cyclestarts = cyclicanalyzer.Where(x => x.Item1 == 1).Select(x => x.Item3).ToList();
                                result = CustomMaths.Lcm(cyclestarts);
                                return result;
                            }
                        };
                        if (targetdevice.name == "rx" && !pulses[0].high) return i; 
                        pulses.AddRange(targetdevice.Operate(pulses[0].high, pulses[0].origin));
                    }
                    pulses.RemoveAt(0);
                }
            }
        }
    }
}