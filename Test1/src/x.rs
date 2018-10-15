#[allow(unused)] use std::io::*;
#[allow(unused)] use std::collections::*;
#[allow(unused)] use std::mem::*;
#[allow(unused)] use std::num::*;
#[allow(unused)] use std::cmp::*;
#[allow(unused)] use std::time::*;

///////////////////////////////////////////////////////////////////////////////

// Authored by : DragoonKiller

// This is the (only) AI for final test.

// Strategy:
// we firstly encounter all resources 'x'.
// factories will be protected if factory is adjacent to enemy.
// resources will be protected if resource is adjacent to enemy, and we have more defence points.
// attacking is preferred to take up the path to resource.
// it's preferred to take up neuture resources other than enemies' resources.

fn main()
{
    // setup a random engine.
    let mut rd_x : u64 = 12357;
    let rd_mod = 1e9 as u64 + 7;
    let rd_mul : u64 = 13313;
    let mut rand = || { rd_x = rd_x * rd_mul % rd_mod; rd_x };
    fn shuffle<T>(rd : &mut FnMut()->u64, target : &mut Vec<T>) where T : Copy {
        for _ in 1..target.len() {
            let a = rd() as usize % target.len();
            let b = rd() as usize % target.len();
            let t = target[b];
            target[b] = target[a];
            target[a] = t;
        }
    };
    
    fn scan_str() -> String
    {
        let mut inp = String::new();
        stdin().read_line(&mut inp).ok();
        inp
    }
    
    const H : usize = 40;
    const W : usize = 80;
    
    let me = scan_str().trim().parse::<char>().ok().unwrap();
    let mut tile = Vec::<Vec<char>>::new();
    let mut owner = Vec::<Vec<char>>::new();
    for _ in 0..H { tile.push(scan_str().chars().collect()); }
    for _ in 0..H { owner.push(scan_str().chars().collect()); }
    
    // >>> Tool functions <<<
    
    let inside = |x : i32, y : i32| { 0 <= x && x < H as i32 && 0 <= y && y < W as i32 };
    
    let for_grids = |f : &mut FnMut(usize,usize,char,char)->()| {
        for i in 0..H {
            for j in 0..W {
                f(i, j, tile[i][j], owner[i][j]);
            }
        }
    };
    
    let for_adjacents = |x : usize, y : usize, f : &mut FnMut(usize,usize,char,char)->()| {
        let dx = [0, 0, 1, -1];
        let dy = [1, -1, 0, 0];
        for t in 0..4 {
            let nx = x as i32 + dx[t];
            let ny = y as i32 + dy[t];
            if !inside(nx, ny) { continue; }
            let (nx, ny) = (nx as usize, ny as usize);
            if tile[nx][ny] == '#' { continue; }
            f(nx, ny, tile[nx][ny], owner[nx][ny]);
        }
    };
    
    let bfs = |initial : & Fn(usize,usize,char,char)->bool,
            cond : & Fn(usize,usize,char,char)->bool,
            f : &mut FnMut(usize,usize,char,char),
            tr : &mut FnMut(usize,usize,usize,usize)| {
        let mut q = VecDeque::new();
        let mut used = vec![vec![false; W]; H];
        for_grids(&mut |x, y, t, c| {
            if !initial(x, y, t, c) { return; }
            q.push_back((x, y));
            used[x][y] = true;
        });
        while !q.is_empty()
        {
            let (cx, cy) = q.pop_front().unwrap();
            f(cx, cy, tile[cx][cy], owner[cx][cy]);
            for_adjacents(cx, cy, &mut |nx, ny, t, c| {
                if !used[nx][ny] && cond(nx, ny, t, c) {
                    q.push_back((nx, ny));
                    used[nx][ny] = true;
                    tr(cx, cy, nx, ny);
                }
            });
        }
    };
    
    // >>> Procedure begin <<<
    
    const INF : i32 = 1e8 as i32;
    const BASE_ATK : i32 = 2;
    const BASE_DEF : i32 = 2;
    const ATK_COST : i32 = 1;
    const DEF_COST : i32 = 1;
    const BUD_COST : i32 = 7;
    
    // distance to territory.
    let mut res_targets = Vec::new();
    let mut tdist = vec![vec![INF; W]; H];
    {
        for_grids(&mut |x ,y , _, c| { if c == me { tdist[x][y] = 0; } });
        
        bfs(& |_, _, _, c| { c == me },
            & |_, _, t, c| { t != '#' && c != me },
            &mut |x, y, t, c| { if t == 'x' && c != me { res_targets.push((x, y)); }; },
            &mut |x, y, nx, ny| { tdist[nx][ny] = tdist[x][y] + 1; }
        );
    }   
    
    // filter build targets.
    let mut bud_targets = Vec::new();
    // distance to factory, inside our own territory.
    {
        let mut fdist = vec![vec![INF; W]; H];
        for_grids(&mut |x, y, t, c| { if t == 'H' && c == me { fdist[x][y] = 0; } });
        
        bfs(& |_, _, t, c| { (t == 'H' || t == 'x') && c == me },
            & |_, _, t, c| { t == ' ' && c == me },
            &mut |_, _, _, _| { },
            &mut |x, y, nx, ny| { fdist[nx][ny] = fdist[x][y] + 1; }
        );
        
        for_grids(&mut |x, y, _, _| {
            if fdist[x][y] == 1 {
                bud_targets.push((x, y));
            }
        });
        
        shuffle(&mut rand, &mut bud_targets);
    }
    
    // filter attack targets.
    // targets are adjacent to our territory and on the path to a resource.
    res_targets.sort_by(|& (ax, ay), & (bx, by)| { tdist[ax][ay].cmp(&tdist[bx][by]) });
    let mut atk_targets = Vec::new();
    {
        let mut atk_used = vec![vec![false; W]; H];
        for (sx, sy) in res_targets {
            let (mut x, mut y) = (sx, sy);
            while tdist[x][y] != 1 {
                for_adjacents(x, y, &mut |nx, ny, _, _| {
                    if tdist[x][y] == 1 { return; }
                    if tdist[nx][ny] == tdist[x][y] - 1 {
                        x = nx;
                        y = ny;
                    }
                });
            }
            if !atk_used[x][y] {
                atk_used[x][y] = true;
                atk_targets.push((x, y));
            }
        }
    }
    
    // filter defend targets.
    let mut def_targets = Vec::new();
    {
        let mut def_not_important = Vec::new();
        
        // defend targets are adjacent to enemies' territory, and is factory or resource.
        for_grids(&mut |x, y, t, c| {
            if c == me && (t == 'x' || t == 'H') { 
                let mut in_danger = false;
                for_adjacents(x, y, &mut |_, _, _, nc| { if nc != me && nc != '0' { in_danger = true; } });
                if in_danger { def_targets.push((x, y)); }
            }
        });
        
        shuffle(&mut rand, &mut def_targets);
        
        // second priority to use defend.
        for_grids(&mut |x, y, _, c| {
            if c == me { 
                let mut in_danger = false;
                for_adjacents(x, y, &mut |_, _, _, nc| { if nc != me && nc != '0' { in_danger = true; } });
                if in_danger { def_not_important.push((x, y)); }
            }
        });
        
        shuffle(&mut rand, &mut def_not_important);
        
        def_targets.append(&mut def_not_important);
    }
    
    // count atk points and def points.
    let mut atk_points = BASE_ATK;
    let mut def_points = BASE_DEF;
    {
        for_grids(&mut |_, _, t, c| { if c == me && t == 'x' { atk_points += 1; } });
        for_grids(&mut |_, _, t, c| { if c == me && t == 'H' { def_points += 1; } });
    }
    
    // factory can only be built when atk_points - BUILD_COST >= factory + BASE_ATK.
    let mut fac_count = def_points - BASE_DEF;
    for & (x, y) in &bud_targets {
        if !((atk_points - BUD_COST) >= fac_count + BASE_ATK) { break; }
        atk_points -= BUD_COST;
        fac_count += 1;
        println!("Build {} {}", x, y);
    }
    
    // for the rest of attack points, attack all targets.
    let mut attacked = Vec::new();
    for &(x, y) in &atk_targets {
        if atk_points < ATK_COST { break; }
        atk_points -= ATK_COST;
        attacked.push((x, y));
        println!("Attack {} {}", x, y);
    }
    
    // for the rest of attack points, attack anywhere that can be attacked.
    let mut atk_random = HashSet::new();
    for_grids(&mut |x, y, _, _| {
        for_adjacents(x, y, &mut |nx, ny, _, c| {
            if c != me { atk_random.insert((nx, ny)); }
        });
    });
    let mut atk_random : Vec<(usize, usize)> = atk_random.iter().filter(|&a| !attacked.contains(a)).map(|&a| a).collect();
    
    shuffle(&mut rand, &mut atk_random);
    
    for (x, y) in atk_random {
        if atk_points < ATK_COST { break; }
        if tdist[x][y] == 1 && !attacked.contains(&(x, y)) {
            atk_points -= ATK_COST;
            println!("Attack {} {}", x, y);
        }
    }
    
    // random defend.
    for & (x, y) in &def_targets {
        if def_points < DEF_COST { break; }
        def_points -= DEF_COST;
        println!("Defend {} {}", x, y);
    }
    
    // >>> Procedure end <<<
}
