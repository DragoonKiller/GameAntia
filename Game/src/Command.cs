using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using static System.Console;
using static Logger;

public class Command
{
    public static Command Parse(string cmd)
    {
        string[] s = cmd.Split(new char[]{ ' ', '\t', ',' });
        
        // Remove empty strings.
        s = s.Filter(v => v != "");
        
        if(s.Length < 3)
        {
            return new InvalidCommand("{0} : parameters not enough.", cmd);
        }
        
        if(s.Length > 3)
        {
            return new InvalidCommand("{0} : too many parameters.", cmd);
        }
        
        for(int i=0; i<3; i++) s[i] = s[i].ToLower();
        
        // Pre-parse parameters since we need to parse them whatever command is.
        
        int x = 0, y = 0;
        
        try { x = int.Parse(s[1]); } catch(Exception)
        {
            return new InvalidCommand(
                "{0} : Parameter {1} should be 32bit signed integer.",
                cmd, s[1]);
        }
        
        try { y = int.Parse(s[2]); } catch(Exception)
        {
            return new InvalidCommand(
                "{0} : Parameter {1} should be 32bit signed integer.",
                cmd, s[2]);
        }
        
        switch(s[0].ToLower())
        {
            case "attack": case "Attack":
                return new AttackCommand() { x = x, y = y };
            
            case "defend": case "Defend":
                return new DefendCommand() { x = x, y = y };
            
            case "build": case "Build":
                return new BuildCommand() { x = x, y = y };
            
            default:
                return new InvalidCommand("Unknown commnad " + s[0].ToLower());
        }
    }
}

public class PosCommand : Command
{
    public int x { get; set; }
    public int y { get; set; }
}

public sealed class InvalidCommand : Command
{
    public string info = "Invalid command!";
    public InvalidCommand(){}
    public InvalidCommand(string s) => info = s;
    public InvalidCommand(string s, params object[] args) => info = string.Format(s, args);
    public override string ToString() => info;
}

public sealed class AttackCommand : PosCommand
{
    public override string ToString() => string.Format("Attack {0} {1}", x, y);
}
public sealed class DefendCommand : PosCommand
{
    public override string ToString() => string.Format("Defend {0} {1}", x, y);
}
public sealed class BuildCommand : PosCommand
{
    public override string ToString() => string.Format("Build {0} {1}", x, y);
}
