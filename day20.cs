namespace AoC2023;

using System.Collections.Generic;
using System.Numerics;

class Device
{
    public string name;
    public char type;
    public Dictionary<string, bool> inputs = new Dictionary<string, bool>();
    public List<string> outputs;
    bool onstate = false;
    public BigInteger delay = -1;
    public BigInteger firsthigh = -1;
    public BigInteger wavenumber = -1;

    public Device(char type, List<string> outputs, string name)  // False == low-tila
    {
        this.type = type;
        this.outputs = outputs;
        this.name = name;
        if (name == "broadcaster")
        {
            delay = 0;
            wavenumber = 1;
        }
    }

    public bool FindDelayAndFreq(List<Device> inputdevices)
    {
        if (type == '%')
        {
            if (inputdevices[0].type != '%')
                wavenumber = inputdevices[0].wavenumber * 2;
            else wavenumber = inputdevices[0].wavenumber;
            delay = inputdevices[0].delay + 1;
            firsthigh = delay - wavenumber/2;
        }
        if (type == '&')
        {
            if (inputdevices.Count(x => x.delay == -1) > 0)
                return false;
            BigInteger mindelay = 0;
            for (var i = 0; i < inputdevices.Count; i++)
                if (inputdevices[i].firsthigh > mindelay)
                    mindelay = inputdevices[i].firsthigh;
            // Frekvenssin maksimi

            var wavenumbers = new List<BigInteger>();
            for (var i = 0; i < inputdevices.Count; i++)
                wavenumbers.Add(inputdevices[i].wavenumber);
            wavenumber = CustomMaths.Lcm(wavenumbers);

            // Selvitetään ensimmäinen kerta, kun pulssi lähtee
            BigInteger time = mindelay;
            while (delay == -1)
            {
                if (CheckSendLow(inputdevices, time))
                    delay = time;
                else
                    time = NextTime(time, inputdevices);
            }
        }
        return false;

        BigInteger NextTime(BigInteger time, List<Device> inputdevices)
        {
            BigInteger nexttime = inputdevices[0].wavenumber-(time - inputdevices[0].firsthigh)/inputdevices[0].wavenumber;
            for (var i=1;i<inputdevices.Count;i++)
            {
                BigInteger trynext = inputdevices[i].wavenumber-(time - inputdevices[i].firsthigh)/inputdevices[i].wavenumber;
                if (trynext < nexttime)
                    nexttime = trynext;
            }
            return nexttime;
        }

        bool CheckSendLow(List<Device> inputdevices, BigInteger time)
        {
            bool sendlow = true;
            foreach (var inputdevice in inputdevices)
            {
                if ((time - firsthigh) % wavenumber < wavenumber / 2)
                {
                    sendlow = false;
                    break;
                }
            }
            return sendlow;
        }
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
            var uncleardevices = circuit.Where(x => x.Value.delay == -1).ToList();
            while (true)
            {
                Device device = uncleardevices[0].Value;
                if (device.delay == -1)
                {
                    var inputdevices = new List<Device>();
                    foreach (var inputdevicename in device.inputs)
                        inputdevices.Add(circuit[inputdevicename.Key]);

                    if (device.FindDelayAndFreq(inputdevices))
                    {
                        if (device.outputs.Count(x => x == "rx") > 0)
                            return device.delay;
                    }
                    else
                    uncleardevices.Add(uncleardevices[0]);
                    uncleardevices.RemoveAt(0);
                }
                else
                uncleardevices.RemoveAt(0);
            }
        }
    }
}

// Pohdintaa b-kohtaan:
// Brute forcellahan tämä ei tule menemään. 
// Seurataan piiriä rx:stä eteenpäin ja tarkastellaan syklisyyttä

// Lähdetään seuraamaan maalista alkuun.
// Jokainen konjunkitomoduuli vaatii pym(syötteet-1) työntääkseen ulos matalan signaalin.
// Jokainen flip-flop-ketju vaatii pituutensa verran pulsseja, että ensimmäinen pulssi menee siitä läpi. Sitten se puolittaa vastaanottamiensa matalien pulssien taajuuden.

// Esimerkki: nappi -> %0 -> %0 -> %0. Neljä painallusta.
// 1. -> %1 -> %0 -> %0
// 2. -> %0 -> %1 -> %0
// 3. -> %1 -> %0 -> %1 
// 4. -> %0 -> %1 -> %0 -> !
// tästä eteenpäin ketjusta tulee matala pulssi läpi joka 2. painallus

// Eli jos joka n:s pulssi sisääntuloon on matala, joka 2n:s painallus ketju lähettää matalan pulssin, samoin 2n:s painallus niiden välillä korkean

// Konjunktiomoduuli antaa matalan pulssin saatuaan jokaiselta syötteeltään korkean pulssin aina siihen asti, että saa taas...

// ELI jokaisella moduulilla on 
// 1. viive siihen, jolloin se ensimmäisen kerran vastaanottaa pulssin, ja
// 2. frekvenssi

// Flip-flopit:
// ketjun ensimmäinen flip-flop puolittaa taajuuden
// jokainen flip-flop lisää viivettä yhdellä

// Konjunktiomoduulit:
// Lähettävät matalan pulssin, kun saavat korkean pulssin
// muista, että 1. ensimmäinen korkea pulssi tulee 1/2 sykliä ennen 1. matalaa sykliä
// eli jos lähdetään siitä, että ensimmäisen matalan pulssin delay on yksi
// time >= min (delay-0.5*freq) ja (time-0.5*freq)%freq = 0 jollekin inputeista ja muille (time-delay&freq) < freq/2  
