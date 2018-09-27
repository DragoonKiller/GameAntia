using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using static System.Console;
using static Logger;

public class Game
{
    public readonly Map map;
    public int curRound; // only for display purpose.
    public const int baseAttack = 2;
    public const int baseDefend = 2;
    
    List<Command>[] commands;
    
    public Game(int playerCount)
    {
        map = new Map(playerCount, w: 80, h: 40);
        map.Init();
        commands = new List<Command>[map.playerCount + 1];
        
        for(int i=1; i <= map.playerCount; i++)
        {
            commands[i] = new List<Command>();
        }
    }
    
    public void TakeOperation(int player, string cmds)
    {
        string[] lines = cmds.Split(new char[]{'\r', '\n'}).Filter((x) => x != "");
        commands[player].Clear();
        
        foreach(var line in lines)
        {
            var cmd = Command.Parse(line);
            if(cmd is InvalidCommand c) LogFmtLine("Command {0} is invalid. Ignored.", line);
            else commands[player].Add(cmd);
        }
    }
    
    public void CheckCommands()
    {
        Command CheckOutOfRange(PosCommand c)
        {
            if(map.Inside(c.x, c.y)) return c;
            return new InvalidCommand("{0} Position {1} {2} is not within the board!", c, c.x, c.y);
        }
        
        Command CheckOwned(PosCommand c, int player)
        {
            if(map[c.x, c.y].owner == player) return c;
            return new InvalidCommand(
                "{0} : Position {1} {2} is not within your territory!", c, c.x, c.y);
        }
        
        Command CheckNotOwned(PosCommand c, int player)
        {
            if(map[c.x, c.y].owner != player) return c;
            return new InvalidCommand("{0} : Position {1} {2} is within your territory!", c, c.x, c.y);
        }
        
        Command CheckAdjacentOwned(PosCommand c, int player)
        {
            int[] dx = new int[]{ 1, -1, 0, 0 };
            int[] dy = new int[]{ 0, 0, 1, -1 };
            for(int i=0; i<4; i++)
            {
                int nx = c.x + dx[i];
                int ny = c.y + dy[i];
                if(map.Inside(nx, ny) && map[nx, ny].owner == player) return c;
            }
            return new InvalidCommand("{0} : Position {1} {2} is not adjacent to your territory.", c, c.x, c.y);
        }
        
        Command CheckObstacle(PosCommand c)
        {
            if(map[c.x, c.y].type != Tile.Type.Obstacle) return c;
            return new InvalidCommand("{0} : Grid {0} {1} is an obstacle!", c, c.x, c.y);
        }
        
        // If command is invalid, change it to InvalidCommand.
        for(int player=1; player <= map.playerCount; player++)
        {
            for(int i=0; i<commands[player].Count; i++)
            {
                var cmds = commands[player];
                
                { if(cmds[i] is PosCommand g) cmds[i] = CheckObstacle(g); }
                { if(cmds[i] is PosCommand g) cmds[i] = CheckOutOfRange(g); }
                { if(cmds[i] is AttackCommand g) cmds[i] = CheckAdjacentOwned(g, player); }
                { if(cmds[i] is AttackCommand g) cmds[i] = CheckNotOwned(g, player); }
                { if(cmds[i] is DefendCommand g) cmds[i] = CheckOwned(g, player); }
                { if(cmds[i] is BuildCommand g) cmds[i] = CheckOwned(g, player); }
            }
        }
        
        // Output information about invalid commands.
        for(int player = 1; player <= map.playerCount; player++)
        {
            foreach(var i in commands[player])
            {
                if(i is InvalidCommand c)
                {
                    LogLine(i.ToString());
                }
            }
        }
        
        // Remove all invalid commands.
        for(int player = 1; player <= map.playerCount; player++)
        {
            commands[player].RemoveAll(x => x is InvalidCommand);
        }
    }
    
    public void Roll()
    {
        CheckCommands();
        
        curRound++;
        LogFmtLine("Round {0} begin.", curRound);
        
        // Check commands' count.
        for(int player = 1; player <= map.playerCount; player++)
        {
            Ref<int> atkPoint = new Ref<int>(0);
            Ref<int> defPoint = new Ref<int>(0);
            
            map.map.Foreach((i, j, r) =>
            {
                if(player != r.owner) return r;
                if(r.type == Tile.Type.Factory) defPoint.v += 1;
                if(r.type == Tile.Type.Resource) atkPoint.v += 1;
                return r;
            });
            
            int atkPointReq = 0;
            int defPointReq = 0;
            foreach(var i in commands[player])
            {
                const int atkPointPerBuilding = 7;
                switch(i)
                {
                    case AttackCommand c: atkPointReq++; break;
                    case DefendCommand c: defPointReq++; break;
                    case BuildCommand c: atkPointReq += atkPointPerBuilding; break;
                    default: break;
                }
            }
            
            if(atkPoint.v + baseAttack < atkPointReq)
            {
                LogFmtLine("Attack point not enough! You have {0} but {1} is required.", atkPoint.v + baseAttack, atkPointReq);
            }
            
            if(defPoint.v + baseDefend < defPointReq)
            {
                LogFmtLine("Defence point not enough! You have {0} but {1} is required.", defPoint.v + baseDefend, defPointReq);
            }
        }
        
        // Log commands.
        for(int player = 1; player <= map.playerCount; player++)
        {
            LogFmtLine("Player {0} :", player);
            foreach(var c in commands[player]) LogLine("  " + c);
        }
        
        // Count and record attack effect.
        byte[,] atkPlayer = new byte[map.height, map.width];
        short[,] atkCount = new short[map.height, map.width];
        for(byte player = 1; player <= map.playerCount; player++)
        {
            foreach(var i in commands[player])
            {
                if(i is AttackCommand c)
                {
                    atkCount[c.x, c.y]--;
                    if(atkCount[c.x, c.y] <= 0)
                    {
                        atkCount[c.x, c.y] *= -1;
                        atkPlayer[c.x, c.y] = player;
                    }
                }
                
                if(i is BuildCommand b)
                {
                    map[b.x, b.y].type = Tile.Type.Factory;
                }
            }
        }
        
        // Remove attacks on a grid for each defend operation.
        for(byte player = 1; player <= map.playerCount; player++)
        {
            foreach(var i in commands[player])
            {
                if(i is DefendCommand d)
                {
                    atkCount[d.x, d.y] = atkPlayer[d.x, d.y] = 0;
                }
            }
        }
        
        // Apply attack effect.
        for(int i = 0; i<map.height; i++) for(int j=0; j<map.width; j++)
        {
            if(atkPlayer[i, j] != 0)
            {
                map[i, j].owner = atkPlayer[i, j];
            }
        }
        
        GC.Collect();
        LogFmtLine("Round {0} end.", curRound);
    }
    
    public string CurrentOperationList
    {
        get 
        {
            var sb = new StringBuilder();
            for(int p=1; p <= map.playerCount; p++)
            {
                sb.AppendFormat("[Player{0}]\n", p);
                foreach(var i in commands[p])
                {
                    switch(i)
                    {
                        case AttackCommand c: sb.Append("Attack {0} {1}\n", c.x, c.y); break;
                        case DefendCommand c: sb.Append("Defend {0} {1}\n", c.x, c.y); break;
                        case BuildCommand c: sb.Append("Build {0} {1}\n", c.x, c.y); break;
                    }
                }
            }
            return sb.ToString();
        }
    }
}
