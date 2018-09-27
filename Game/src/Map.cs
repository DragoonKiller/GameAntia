using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using static Logger;

public struct Tile
{
    public enum Type
    {
        Normal,
        Obstacle,
        Factory,
        Resource,
    }
    
    public Type type;
    public int owner;
}

public class Map
{
    public int height;
    public int width;
    public int playerCount;
    public readonly Tile[,] map;
    bool hasOutput = false;
    
    public Map(int playerCount, int w, int h)
    {
        this.playerCount = playerCount;
        width = w;
        height = h;
        map = new Tile[h, w];
    }
    
    public bool Inside(int x, int y) => 0 <= x && x < height && 0 <= y && y < width;
    public ref Tile this[int x, int y] => ref map[x, y];
    
    public void Init()
    {
        ///
        /// Notice:
        /// This map generator may have issues when edge case occurs.
        /// It'll be manually taken into account so that you don't have to care about that.
        // 
        
        Random rd = new Random((int)DateTime.Now.Ticks);
        
        // Step 1 : Randomly generate obstacles.
        {
            map.Foreach((i, j, r) =>
            {
                const double rate = 1.0 / 5.0; // 1 obstacle per 7 tiles.
                if(rd.NextDouble() < rate) r.type = Tile.Type.Obstacle;
                return r;
            });
        }
        
        List<(int x, int y)> vq = null;
        // Step 2 : Floodfill to find the largest connected componenet in the map.
        {
            bool[,] used = new bool[height, width];
            Ref<int> res = new Ref<int>(0);
            map.Foreach((i, j, r) =>
            {
                if(used[i, j]) return r;
                
                int cnt = 0;
                int[] di = new int[]{ 1, -1, 0, 0 };
                int[] dj = new int[]{ 0, 0, 1, -1 };
                
                var q = new List<(int x, int y)>();
                q.Add((i, j));
                used[i, j] = true;
                
                while(cnt != q.Count)
                {
                    var (ci, cj) = q[cnt++];
                    for(int t=0; t<4; t++)
                    {
                        var (ni, nj) = (ci + di[t], cj + dj[t]);
                        if(Inside(ni, nj) && map[ni, nj].type != Tile.Type.Obstacle)
                        {
                            if(!used[ni, nj])
                            {
                                q.Add((ni, nj));
                                used[ni, nj] = true;
                            }
                        }
                    }
                }
                
                if(res.v < cnt)
                {
                    res.v = cnt;
                    vq = q;
                }
                return r;
            });
            LogLine("Max connected componenet size: " + vq.Count);
        }
        
        // Generate resources. Remove collected resources.
        {
            const double rate = 1.0 / 80.0; // one resource site per 80 grids.
            for(int i=0; i < vq.Count; i++)
            {
                if(rd.NextDouble() < rate)
                {
                    map[vq[i].x, vq[i].y].type = Tile.Type.Resource;
                    (vq[i], vq[vq.Count-1]) = (vq[vq.Count-1], vq[i]);
                    vq.RemoveAt(vq.Count-1);
                }
            }
        }
        
        // Generate Factories.
        {
            for(int i=1; i <= playerCount; i++)
            {
                var (x, y) = vq[rd.Next(0, vq.Count-1)];
                vq.Remove((x, y));
                map[x, y].type = Tile.Type.Factory;
                map[x, y].owner = i;
            }
        }
        
        LogLine("Initial map:");
        LogLine(OutputString(0));
    }
    
    public string OutputString(int player)
    {
        var sb = new StringBuilder();
        
        // Plyer ID.
        sb.AppendFormat("{0}\n", player);
        
        // Tile type info.
        for(int i=0; i<height; i++)
        {
            for(int j=0; j<width; j++)
            {
                char c = ' ';
                if(map[i, j].type == Tile.Type.Factory) c = 'H';
                if(map[i, j].type == Tile.Type.Resource) c = 'x';
                if(map[i, j].type == Tile.Type.Obstacle) c = '#';
                sb.Append(c);
            }
            sb.Append("\n");
        }
        
        // Tile owner info.
        for(int i=0; i<height; i++)
        {
            for(int j=0; j<width; j++) sb.Append(map[i, j].owner);
            sb.Append('\n');
        }
        
        return sb.ToString();
    }
    
    
    
    public void OutputConsole(int round)
    {
        Logger.consoleLock.WaitOne();
        
        if(hasOutput) Console.SetCursorPosition(0, Console.CursorTop - height - 1);
        
        Console.WriteLine("Round " + round);
        
        var prevBack = Console.BackgroundColor;
        var prevFront = Console.ForegroundColor;
        
        ConsoleColor[] playerForegroundColor = new ConsoleColor[6];
        ConsoleColor[] playerBackgroundColor = new ConsoleColor[6];
        
        playerBackgroundColor[0] = ConsoleColor.Black;
        playerBackgroundColor[1] = ConsoleColor.DarkRed;
        playerBackgroundColor[2] = ConsoleColor.DarkYellow;
        playerBackgroundColor[3] = ConsoleColor.DarkGreen;
        playerBackgroundColor[4] = ConsoleColor.DarkBlue;
        playerBackgroundColor[5] = ConsoleColor.Gray;
        
        playerForegroundColor[0] = ConsoleColor.DarkGray;
        playerForegroundColor[1] = ConsoleColor.Black;
        playerForegroundColor[2] = ConsoleColor.Black;
        playerForegroundColor[3] = ConsoleColor.Black;
        playerForegroundColor[4] = ConsoleColor.Black;
        playerForegroundColor[5] = ConsoleColor.Black;
        
        void SetColor(ConsoleColor back, ConsoleColor front)
        {
            Console.BackgroundColor = back;
            Console.ForegroundColor = front;
        }
        
        for(int i=0; i<map.GetLength(0); i++)
        {
            for(int j=0; j<map.GetLength(1); j++)
            {
                char c = ' ';
                if(map[i, j].type == Tile.Type.Factory) c = 'H';
                if(map[i, j].type == Tile.Type.Resource) c = 'x';
                if(map[i, j].type == Tile.Type.Obstacle) c = '#';
                ConsoleColor back = playerBackgroundColor[map[i, j].owner];
                ConsoleColor front = playerForegroundColor[map[i, j].owner];
                SetColor(back, front);
                Console.Write(c);
                
            }
            SetColor(prevBack, prevFront);
            Console.WriteLine();
        }
        
        hasOutput = true;
        Logger.consoleLock.ReleaseMutex();
    }
}
