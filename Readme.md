## 2018 年联创 game组秋招熬夜测试题 *The Antia* 游戏源代码

Welcome to the source repository of Game Antia!

The game is inspired by *Tsinghua Artificial Intelligence Programming Contest*. Its official site is https://ai.net9.org/ .

![picture](https://raw.githubusercontent.com/DragoonKiller/GameAntia/master/docs/scst.png)

To run the game you need one of these installation :
* Net Framework 4.7.1 or newer.
* .Net Core 2.1 or newer.
* Any platforms that supports .net 4.7.1 and so on.

To run the game with samples, make sure you have dependency installed and access is valid.
Simply run the game in terminal, with *commands* to run samples followed:
```
./bin/Antia.exe ./bin/SampleAI1.exe ./bin/SampleAI2.exe ./bin/SampleAI3.exe "python ./Sample4/src/x.py" "java -classpath ./bin/Sample5 Sample5"
```
One parameter corresponds one player; players are numbered 1 to 5 from left to right. You can duplicate usable AIs:
```
./bin/Antia.exe ./bin/SampleAI1.exe ./bin/SampleAI1.exe ./bin/SampleAI1.exe
```
or
```
./bin/Antia.exe "python ./Sample4/src/x.py" "python ./Sample4/src/x.py"
```

Samples and tests are written in different languages which may need further dependency and configuration:
* Sample 1 is written in C# 7.3 using .Net build system.
* Sample 2 is written in C++ 11.
* Sample 3 is written in rust 1.28.0, using cargo as build system.
* Sample 4 is written in python 3.
* Sample 5 is written in java 10 (should supports java 8).
* Test 1 is written in rust 1.28.0, using cargo as build system.

To build the game you may need **one of** those installations :
* .Net SDK 4.7.1 and its targeting pack.
* .Net Core 2.1 and mono. **You may need to change project configurations to match the .net core**.
* The lastest version of Mono. It is not sure mono supports this target, but it worth a try ssince I successfully built C# 7.1+ projects on Linux. **You may need to change project configurations to match the .net core**.
