using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

using static Logger;

public class Config
{
    /// Step delay in ms.
    public int delay;
    
    /// Time limit for one operation of player.
    public int timeLimit;
    
    
    /// Read code from config.
    public void ReadFrom(string file)
    {
        var s = File.ReadAllText(file).Split('\n');
        var ex = new Regex(@" *([a-zA-Z_]+) *= *([0-9]+) *");
        foreach(var c in s)
        {
            var g = ex.Match(c);
            if(g.Success == false) { continue; }
            var name = g.Groups[1].Value;
            var val = g.Groups[2].Value;
            
            WriteLine("config : " + name + " = " + val);
            try
            {
                int p = int.Parse(val);
                foreach(var f in typeof(Config).GetFields())
                {
                    if(f.Name.ToLower() == name.ToLower())
                    {
                        f.SetValue(this, p);
                    }
                }
            }
            catch(FormatException)
            {
                throw new Exception("Config file has a wrong format.");
            }
        }
    }
}
