using System;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using System.Threading.Tasks;
using static Logger;


public static class __Main__
{
    public static string GetProgramOutput(string command, string input)
    {
        const double timeout = 0.2f; // in seconds.
        
        Process proc = new Process();
        try
        {
            LogLine("cmd : " + command);
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = command,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };
            proc.Start();
            var stdin = proc.StandardInput;
            var stdout = proc.StandardOutput;
            stdin.Write(input);
            
            var timer = new System.Threading.Timer(
                (e) =>
                {
                    LogLine("Timeout!");
                    proc.Kill();
                }, 
                null,
                new TimeSpan(0, 0, 0, 0, (int)(timeout * 1000)),
                new TimeSpan(1));
            
            proc.WaitForExit();
            timer.Dispose();
            return stdout.ReadToEnd();
        }
        catch(Exception e)
        {
            LogLine("Error : " + e.Message);
            try { if(proc.HasExited) proc.Kill(); }
            catch(Exception) { }
        }
        return "";
    }
    
    public static void Main(string[] args)
    {
        Config config = new Config();
        config.ReadFrom("./config.ini");
        
        Game game = new Game(args.Length);
        
        for(int i=1; i<=200; i++)
        {
            const double time = 1000; // in seconds.
            var t = new System.Timers.Timer(time);
            
            int c = 0;
            foreach(var s in args)
            {
                c++;
                // WriteLine(string.Format("Player {0}", c));
                var cmd = GetProgramOutput(s, game.map.OutputString(c));
                // WriteLine(cmd);
                game.TakeOperation(c, cmd);
            }
            
            game.Roll();
            
            game.map.OutputConsole(i);
        }
    }
}
