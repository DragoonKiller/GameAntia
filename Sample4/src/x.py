from io import *
from sys import *
from random import *

h = 40 ; w = 80

def Game() :
    me = int(stdin.readline())
    grids = []
    for i in range(h) : grids.append(stdin.readline())
    for i in range(h) : grids[i] = list(zip(grids[i], map(lambda x : int(x), stdin.readline()[0:w])))
    
    atkpt = defpt = 2
    def_targets = []
    atk_targets = []
    
    for i in range(h) :
        for j in range(w) :
            if grids[i][j][1] == me and grids[i][j][0] == 'H' : defpt += 1 ; def_targets.append((i, j))
            if grids[i][j][1] == me and grids[i][j][0] == 'x' : atkpt += 1 ; def_targets.append((i, j))
            if grids[i][j][1] == me :
                for (di, dj) in ((0, 1), (0, -1), (1, 0), (-1, 0)) :
                    (ni, nj) = (i + di, j + dj)
                    if not (0 <= ni and ni < h and 0 <= nj and nj < w) : continue
                    if grids[ni][nj][1] == me : continue
                    if grids[ni][nj][0] == '#' : continue
                    atk_targets.append((ni, nj))
    
    shuffle(def_targets)
    shuffle(atk_targets)
    
    k = 0
    for x in range(len(atk_targets)) :
        (i, j) = atk_targets[x]
        if grids[i][j][1] != 0 :
            (atk_targets[x], atk_targets[k]) = (atk_targets[k], atk_targets[x])
            k += 1
    
    k = 0
    for x in range(len(def_targets)) :
        (i, j) = def_targets[x]
        dangerous = False
        for (di, dj) in ((0, 1), (0, -1), (1, 0), (-1, 0)) :
            (ni, nj) = (i + di, j + dj)
            if not (0 <= ni and ni < h and 0 <= nj and nj < w) : continue
            if grids[ni][nj][1] != 0 and grids[ni][nj][1] != me : dangerous = True
        if dangerous :
            (def_targets[x], def_targets[k]) = (def_targets[k], def_targets[x])
            k += 1
    
    for i in range(0, min(len(def_targets), defpt)) : print("Defend", def_targets[i][0], def_targets[i][1])
    for i in range(0, min(len(atk_targets), atkpt)) : print("Attack", atk_targets[i][0], atk_targets[i][1])
    
if __name__ == "__main__" : Game()
