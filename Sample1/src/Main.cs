using System;
using System.Text;
using System.Collections.Generic;
using static System.Console;

public class Map
{
    public const int h = 40;
    public const int w = 80;
    public enum TileType { None, Fac, Res, Obs, }
    
    public TileType[,] type = new TileType[h, w];
    public int[,] owner = new int[h, w];
    
    public (TileType type, int owner) this[int x, int y]
        => (type[x, y], owner[x, y]);
    
    public void ReadFromStdin()
    {
        // Read 40 x 80 tile map.
        for(int i=0; i<h; i++)
        {
            var s = Console.ReadLine();
            for(int j=0; j<w; j++)
            {
                switch(s[j])
                {
                    case '#': type[i,j] = TileType.Obs; break;
                    case 'x': type[i,j] = TileType.Res; break;
                    case 'H': type[i,j] = TileType.Fac; break;
                    case ' ': type[i,j] = TileType.None; break;
                    default: break;
                }
            }
        }
        
        // Read 40x80 tile owner map.
        for(int i=0; i<h; i++)
        {
            var s = ReadLine();
            s = s.TrimEnd();
            for(int j=0; j<w; j++) owner[i, j] = s[j] - '0';
        }
    }
}

public static class __Main__
{
    
    public static void Main(string[] args)
    {
        Random rd = new Random((int)DateTime.Now.Ticks);
        Map map = new Map();
        int me = int.Parse(Console.ReadLine().Trim());
        map.ReadFromStdin();
        
        // >>> tool functions <<<
        bool Inside(int x, int y) => 0 <= x && x < Map.h && 0 <= y && y < Map.w;
        bool Owned(int x, int y) => map[x, y].owner == me;
            
        // >>> find attackable and defendable grids <<< 
        var canAtk = new bool[Map.h, Map.w];
        var canDef = new bool[Map.h, Map.w];
        
        {
            var di = new int[]{ 0, 0, 1, -1 };
            var dj = new int[]{ 1, -1, 0, 0 };
            for(int i=0; i<Map.h; i++) for(int j=0; j<Map.w; j++) if(Owned(i, j))
            {
                canDef[i, j] = true;
                for(int d=0; d<4; d++)
                {
                    int ni = i + di[d];
                    int nj = j + dj[d];
                    if(!Inside(ni, nj)) continue;
                    if(Owned(ni, nj)) continue;
                    if(map[ni, nj].type == Map.TileType.Obs) continue;
                    canAtk[ni, nj] = true;
                }
            }
        }
        
        // >>> build attackable and defendable grid list <<<
        var atk = new List<(int x, int y)>();
        var def = new List<(int x, int y)>();
        {
            for(int i=0; i<Map.h; i++) for(int j=0; j<Map.w; j++)
            {
                if(canAtk[i, j]) atk.Add((i, j));
                if(canDef[i, j]) def.Add((i, j));
            }
        }
        
        // >>> Count factories and resources. <<<
        var atkCount = 2;
        var defCount = 2;
        var k = 0;
        for(int i=0; i<Map.h; i++) for(int j=0; j<Map.w; j++) if(Owned(i, j))
        {
            k++;
            if(map.type[i, j] == Map.TileType.Fac) defCount++;
            if(map.type[i, j] == Map.TileType.Res) atkCount++;
        }
        
        // >>> generate command sequences randomly  <<<
        var sb = new StringBuilder();
        {
            void Shuffle<T>(IList<T> list)
            {
                for(int i=0; i<list.Count; i++)
                {
                    int r = rd.Next(0, list.Count-1);
                    (list[i], list[r]) = (list[r], list[i]);
                }
            }
            
            Shuffle(atk);
            Shuffle(def);
            
            for(int i=0; i < Math.Min(atk.Count, atkCount); i++)
            {
                sb.AppendFormat("Attack {0} {1}\n", atk[i].x, atk[i].y);
            }
            
            for(int i=0; i < Math.Min(def.Count, defCount); i++)
            {
                sb.AppendFormat("Defend {0} {1}\n", def[i].x, def[i].y);
            }
        }
        
        // >>> output <<<
        Console.WriteLine(sb.ToString());
    }
}
