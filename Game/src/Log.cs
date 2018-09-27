using System;
using System.IO;
using System.Threading;

public static class Logger
{
    public static readonly  StreamWriter wt = new StreamWriter(File.Create("log.txt"));
    public static readonly Mutex consoleLock = new Mutex();
    public static readonly Mutex fileLock = new Mutex();
    
    public static void Log(string s)
    {
        fileLock.WaitOne();
        wt.Write(s);
        wt.Flush();
        fileLock.ReleaseMutex();
    }
    
    public static void LogLine(string s = "")
    {
        fileLock.WaitOne();
        wt.WriteLine(s);
        wt.Flush();
        fileLock.ReleaseMutex();
    }
    
    public static void LogFmt(string s, params object[] args)
    {
        fileLock.WaitOne();
        wt.Write(string.Format(s, args));
        wt.Flush();
        fileLock.ReleaseMutex();
    }
    
    public static void LogFmtLine(string s, params object[] args)
    {
        fileLock.WaitOne();
        wt.WriteLine(string.Format(s, args));
        wt.Flush();
        fileLock.ReleaseMutex();
    }
    
    public static void Write(string s)
    {
        consoleLock.WaitOne();
        Console.Write(s);
        Console.OpenStandardOutput().Flush();
        consoleLock.ReleaseMutex();
    }
    
    public static void WriteLine(string s = "")
    {
        consoleLock.WaitOne();
        Console.WriteLine(s);
        Console.OpenStandardOutput().Flush();
        consoleLock.ReleaseMutex();
    }
    
    public static void WriteFmt(string s, params object[] args)
    {
        consoleLock.WaitOne();
        Console.Write(string.Format(s, args));
        Console.OpenStandardOutput().Flush();
        consoleLock.ReleaseMutex();
    }
    
    public static void WriteFmtLine(string s, params object[] args)
    {
        consoleLock.WaitOne();
        Console.WriteLine(string.Format(s, args));
        Console.OpenStandardOutput().Flush();
        consoleLock.ReleaseMutex();
    }
}
