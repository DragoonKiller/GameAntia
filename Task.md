## The Antia (200 Points)

You're to play a game called Antia.

You play the game on a grid map which has 40 row and 80 columns. Each grid (or so called a Tile in the world) is one of 4 types: **Normal**, **Obstacle**, **Factory** and **Resource**.

Each round, every player can perform 3 types of command: **Attack**, **Defend** and **Build**. All commands should be performed on a grid that is not obstacle.
* **Attack** should be performed on a grid adjacent to your territory that is currently not yours. When the round ends, attacked grids will be yours if there's no **Defend** command applied on the grid. You can perform any amount of Attack command on the same grid. If multiple players perform Attack command on the same grid, the one who performs the most times of Attack takes the grid. If there're multiple players perform the same times of Attack, player with the largest ID takes the grid.
* **Defend** prevents your grid from being taken up by other players. As long as you perform the Defend command on a grid, this grid will never be taken up by others until next round. You can perform multiple Defend command on the same grid, however it has no more effect.
* **Build** can change a grid's type from **Normal** into **Factory**, which should be within your territory.

When you have `a` Factories and `b` Resources and it's your turn, you can perform **at most `a + 2` times of Defend** and **at most `b + 2` times of Attack**. The cost of **Build** command is equivalent to **`7` Attack commands**.

At the beginning of the game, each player has only one grid which is always a Factory. Your goal is to take grids as many as possible. You win if
* Right after 200 rounds, your territory is the largest (or one of the largest territories) amoung all players', **or**
* Your territory has 50% of non-obstacle grids in the world (in the whole map) before 200 rounds.

Write an AI to win the game against 4 other players.

To maximize the portability, the game uses **standard input** and **standard output** to communicate with your AI program. The game program will push information of the whole map into your AI program's stnadard input by using pipes **before every round**, and take your AI program's standard output as commands.

The input contains 3 parts : your player ID in the first line, the tile map below and the owner map followed up.

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

The commands are in syntax (regular expression) `(([aA]ttack|[dD]efend|[bB]uild) +[0-9]+ +[0-9]+\n)*`, e.g.
```
attack 13 25
defend 12 11
build 13 15
attack 22 34
```
commands that are not valid are ignored. Other commands which being affect by those invalid commands may also be ignored.

There are multiple samples written in different programming languages. To use then, run the game from terminal, with their names as parameters, e.g. `./bin/Antia.exe ./Sample1/x.exe "python ./Sample2/x.py"`. 

### Scores

We will run `10` times of game with your AI and 4 of our AIs (may **not** be given samples).

1. You get **at least** 30 scores if your AI works properly, just like samples.
2. You get **at least** 60 points if your AI live for at least 200 rounds every game.
3. You get **at least** 100 scores if your AI win at least `1` times.
4. You get **at least** 160 scores if your AI win at least `5` times.
5. You get full 200 scores if your AI win at least `8` times.

You may get more scores even if you haven't complete requirements described above.
We will go deep into your code to find out what strategy your AI use, and how famillar you are with the programming language you use.

### Hints
1. To run the game, you may need an installation of **.Net Core**, **Mono** or **.Net Framework**. **It's recommanded to install Mono on Linux and macOS, and .Net Core on Windows**. This is not a simple work, check everything carefully.
2. Check `log.txt` if you get something wrong with the game or with your AI. Invalid commands will be reported out there.
3. You can refer given samples' source code to investigate how to interact with the game program.
4. Feel free to use any programming language and its compiler and build system.
5. Your program should exit normally **in `0.2` seconds**. Program will be killed if time limit exceeded.
6. You may need to `flush` the stdout when you have done your output. Search `stdout flush` for more information.
7. Since the game program only give you the current map, you can store previous maps and operations by files or something else.

### Other References
1. https://docs.microsoft.com/en-us/dotnet/core/windows-prerequisites?tabs=netcore21
2. https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=netcore2x
3. https://docs.microsoft.com/en-us/dotnet/core/tutorials/using-on-macos
4. https://www.mono-project.com/
