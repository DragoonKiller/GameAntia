## The Antia (200 Points)

You are to play the game on a grid map which has 40 row and 80 columns. Each grid (or so called a Tile) is one of 4 types: **Normal**, **Obstacle**, **Factory** and **Resource**.

Each round, every player can perform 3 types of command: **Attack**, **Defend** and **Build**. All commands should be performed on a grid that is not obstacle.
* **Attack** should be performed on a grid adjacent to your territory which is currently not yours. Attacked grids will be yours the next round if there's no **Defend** command applied on the grid. You can perform any amount of Attack command on the same grid. If multiple players perform Attack command on the same grid, the one who performs the most times of Attack takes the grid. If there're multiple players perform the same times of Attack, player with the largest ID takes the grid.
* **Defend** prevents your grid from being taken by others. As long as you perform the Defend command on a grid, this grid will never be taken by others until next round. You can perform multiple Defend command on the same grid, however it has no more effect.
* **Build** can change a grid's type from **Normal** into **Factory**, which should be within your territory.

When you have `a` Factories and `b` Resources, you can perform **at most `a + 2` times of Defend** and **at most `b + 2` times of Attack** each round. The cost of **Build** command is equivalent to **`7` Attack commands**.

At the beginning, each player has only one grid which is always a Factory. Your goal is to take grids as many as possible. You win if right after 200 rounds your territory is the largest (or one of the largest territories) amoung all players.

Write an AI to win the game against 4 other players.

To maximize the portability, the game uses **standard input** and **standard output** to communicate with your AI program. The game program will push information of the whole map into your AI program's stnadard input by using pipes **before every round**, and take your AI program's standard output as commands.

The input consists of 3 parts : your player ID in the first line, the tile map below and the owner map following up.

The map is represented as two char matrix of 40 raws and 80 columns. The first matrix represents grid types. We use `#` as Obstacles, `x` as Resource, `H` as Factory and ` ` (space) as Normal. The second matrix represents the owner of grids, using numbers from `1` to `5` to represent the owner's player ID, while `0` represents neutral grid which is not owned by any player. The map (part of it) looks like this:
```
1
#  # #  x...
x ##    #...
 # H  ## ...
##     # ...
...
000010111...
000011110...
001111003...
001112203...
...
```

The commands are in syntax (regular expression) `^(([aA]ttack|[dD]efend|[bB]uild) +[0-9]+ +[0-9]+\n)* *$`, e.g.
```
attack 13 25
Defend 12 11
build 13 15
Attack 22 34
```
The game may ignore all your commands if one of your command is invalid. Check `log.txt` if your find anything wrong.

There are samples written in different programming languages. To use them, run the game from terminal, with their names as parameters, e.g. `./bin/Antia.exe ./Sample1/x.exe "python ./Sample2/x.py"`. The game supports at most `5` plyers.

### Score

We will run `10` times of game with your AI and 4 of our AIs (which may be more powerful than given samples).

1. You get +30 points if your AI works properly, just like samples.
2. You get +30 points if your AI still have grids at 200 rounds **every game**.
3. You get +40 points if your AI win at least `2` times.
4. You get +50 points if your AI win at least `5` times.
5. You get +50 points if your AI win at least `8` times.

You may get more score even if your AI doesn't meet requirements.  
However, if your AI is too similar to samples, you'll have less score.

### Hints
1. You can download the source code of game and samples from [from here](https://github.com/DragoonKiller/GameAntia).
2. The game is written in C#. To run the game, you may need an installation of **.Net Core**, **Mono** or **.Net Framework**. **It's recommanded to install Mono on Linux and macOS, and .Net Core on Windows**.
3. Samples are written in different languages. You may need to install other dependencies if you want to use them.
4. Resize the console for the best experience when running the game.
5. You can refer to given samples' source code to learn how to interact with the game program.
6. Feel free to use any programming language for your AI.
7. Your program should exit **in `0.5` seconds**. Program will be killed if time limit exceeded.
8. You may need to `flush` stdout when you have done your output.
9. Since the game only gives you the current map, you can store previous maps and operations by files or something else.
10. Have fun!
