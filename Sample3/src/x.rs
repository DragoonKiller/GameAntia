#[allow(unused)] use std::io::*;
#[allow(unused)] use std::collections::*;
#[allow(unused)] use std::mem::*;
#[allow(unused)] use std::num::*;
#[allow(unused)] use std::cmp::*;

///////////////////////////////////////////////////////////////////////////////

fn main()
{
    fn scan_str () -> String
    {
        let mut inp = String::new();
        stdin().read_line(&mut inp).ok();
        inp
    }
    
    const H : usize = 40;
    const W : usize = 80;
    const INF : i32 = 100000;
    
    let me = scan_str().trim().parse::<char>().ok().unwrap();
    let mut tile = Vec::<Vec<char>>::new();
    let mut owner = Vec::<Vec<char>>::new();
    for _ in 0..H { tile.push(scan_str().chars().collect()); }
    for _ in 0..H { owner.push(scan_str().chars().collect()); }
    
    // >>> Tool functions <<<
    
    let inside = |x, y| { 0 <= x && x < H as i32 && 0 <= y && y < W as i32 };
    
    let for_grids = |f : &mut FnMut(i32,i32,char,char)->()| {
        for i in 0..H {
            for j in 0..W {
                f(i as i32, j as i32, tile[i][j], owner[i][j]);
            }
        }
    };
    
    let for_adjacents = |x : i32, y : i32, f : &mut FnMut(i32,i32,char,char)->()| {
        let dx = [0, 0, 1, -1];
        let dy = [1, -1, 0, 0];
        for t in 0..4 {
            let nx = x + dx[t];
            let ny = y + dy[t];
            if !inside(nx, ny) { continue; }
            if tile[nx as usize][ny as usize] == '#' { continue; }
            f(nx, ny, tile[nx as usize][ny as usize], owner[nx as usize][ny as usize]);
        }
    };
    
    // >>> Procedure begin <<<
    
    // Find all reachable points.
    let mut dist = Vec::<Vec<i32>>::new();
    let mut used = Vec::<Vec<bool>>::new();
    let mut res = Vec::<(i32,i32)>::new();
    {
        // Allocate distance array.
        for _ in 0..H { let mut t = Vec::new(); for _ in 0..W { t.push(0); } dist.push(t); }
        for _ in 0..H { let mut t = Vec::new(); for _ in 0..W { t.push(false); } used.push(t); }
        
        let mut q = VecDeque::<(i32,i32)>::new();
        
        // Starting points.
        for_grids(&mut |x, y, _, c| {
            if c == me {
                q.push_back((x, y));
                dist[x as usize][y as usize] = 0;
                used[x as usize][y as usize] = true;
            }
            else { dist[x as usize][y as usize] = INF; }
        });
        
        // BFS.
        while !q.is_empty() {
            let (x, y) = q.pop_front().unwrap();
            if tile[x as usize][y as usize] == 'x' || tile[x as usize][y as usize] == 'H' {
                res.push((x, y));
            }
            for_adjacents(x, y, &mut |nx, ny, _, _| {
                if !used[nx as usize][ny as usize] {
                    dist[nx as usize][ny as usize] = dist[x as usize][y as usize] + 1;
                    used[nx as usize][ny as usize] = true;
                    q.push_back((nx, ny));
                }
            });
        }
    }
    
    // Find the closest path to attack targets.
    let mut targets = HashSet::<(i32, i32)>::new();
    {
        for (sx, sy) in res {
            if owner[sx as usize][sy as usize] == me { continue; }
            let (mut cx, mut cy) = (sx, sy);
            while dist[cx as usize][cy as usize] != 1 {
                for_adjacents(cx, cy, &mut |x, y, _, _| {
                    if dist[x as usize][y as usize] == dist[cx as usize][cy as usize] - 1 {
                        if dist[x as usize][y as usize] >= 1 {
                            cx = x;
                            cy = y;
                        }
                    }
                });
            }
            targets.insert((cx, cy));
        }
    }
    
    // Count attack points.
    let mut atkpt = 2;
    for_grids(&mut |_, _, r, c| if r == 'x' && c == me { atkpt += 1; });
    
    // Attack all targets.
    {
        for (x, y) in targets {
            if atkpt <= 0 { break; }
            atkpt -= 1;
            println!("Attack {} {}", x, y);
        }
    }
}
