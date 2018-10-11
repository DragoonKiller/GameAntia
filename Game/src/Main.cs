using System;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using System.Threading.Tasks;
using System.Text;
using static Logger;


public static class __Main__
{
    public static string GetProgramOutput(string command, string input)
    {
        Process proc = new Process();
        try
        {
            var arr = command.Split(' ');
            LogLine("cmd : " + arr[0]);
            var param = new StringBuilder();
            for(int i=1; i<arr.Length; i++) param.Append(arr[i] + " ");
            LogLine("params : " + param.ToString());
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = arr[0],
                Arguments = param.ToString(),
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
                new TimeSpan(0, 0, 0, 0, Config.inst.timeLimit),
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
        Config.inst.ReadFrom("./config.ini");
        
        Game game = new Game(args.Length);
        
        for(int round=1; round<=Config.inst.maxRound; round++)
        {
            int c = 0;
            
            string[] cmds = new string[Config.inst.maxPlayer + 1];
            
            Parallel.ForEach(args, (s) =>
            {
                c++;
                if(c > 5) { throw new InvalidOperationException("This game only supports at most " + Config.inst.maxPlayer + " players!"); }
                cmds[c] = GetProgramOutput(s, game.map.OutputString(c));
            });
            
            for(int i=1; i <= c; i++)
            {
                game.TakeOperation(i, cmds[i]);
            }
            
            game.Roll();
            
            game.map.OutputConsole(round);
        }
    }
}
