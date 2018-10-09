import java.util.*;

interface MapFunc { void Run(int x, int y, Tile tile); }

/*
 * This sample has a bug.
 * Correct the bug so that you don't have penalty for using this sample directly.
*/
public class Sample5
{
    public static void main(String[] args)
    {
        Scanner sc = new Scanner(System.in);
        char me = sc.nextLine().charAt(0);
        Map map = new Map().read(sc);
        
        Box<Integer> atkcnt = new Box<>(); atkcnt.value = 2;
        Box<Integer> defcnt = new Box<>(); defcnt.value = 2;
        
        map.foreach((x, y, t) -> { if(t.owner == me && t.type == 'x') atkcnt.value += 1; });
        map.foreach((x, y, t) -> { if(t.owner == me && t.type == 'H') defcnt.value += 1; });
        
        ArrayDeque<Point> qx = new ArrayDeque<>();
        
        map.foreach((x, y, t) ->
        {
            if(t.owner != me) return;
            final int[] dx = new int[]{ 0, 0, 1, -1 };
            final int[] dy = new int[]{ 1, -1, 0, 0 };
            for(int i=0; i<4; i++)
            {
                int nx = dx[i] + x;
                int ny = dy[i] + y;
                if(0 <= nx && nx < Map.h && 0 <= ny && ny <= Map.w && map.tile(nx,  ny).owner != me)
                {
                    Point p = new Point();
                    p.x = nx;
                    p.y = ny;
                    qx.addFirst(p);
                }
            }
        });
    
        HashSet<Point> st = new HashSet<>();
        while(!qx.isEmpty())
        {
            Point v = qx.peekFirst(); qx.pop();
            if(st.contains(v)) continue;
            st.add(v);
            System.out.printf("Attack %d %d\n", v.x, v.y);
        }
    }
}

class Point implements Comparable<Point>
{
    int x, y;
    @Override public int compareTo(Point c)
    {
        if(x == c.x && y == c.y) return 0;
        if(x < c.x || (x == c.x && y < c.y)) return -1;
        return 1;
    }
    @Override public boolean equals(Object c)
    {
        Point g = (Point)c;
        return x == g.x && y == g.y;
    }
}

class Box<T> { T value; }

class Map
{
    static final int h = 40;
    static final int w = 80;
    private Tile[] tiles = new Tile[h * w];
    Tile tile(int x, int y) { return tiles[x * w + y]; }
    
    Map read(Scanner sc)
    {
        for(int i=0; i<h; i++)
            for(int j=0; j<w; j++)
                tiles[i * w + j] = new Tile();
        
        for(int i=0; i<h; i++)
        {
            String s = sc.nextLine();
            for(int j = 0; j < s.length(); j++)
                tile(i, j).type = s.charAt(j);
        }
    
        for(int i=0; i<h; i++)
        {
            String s = sc.nextLine();
            for(int j = 0; j < s.length(); j++)
                tile(i, j).owner = s.charAt(j);
        }
        
        return this;
    }
    
    void foreach(MapFunc f)
    {
        for(int i = 0; i < h; i++)
            for(int j = 0; j < w; j++)
                f.Run(i, j, tile(i, j));
    }
}

class Tile
{
    char type;
    char owner;
}
