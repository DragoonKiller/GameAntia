dotnet build ./Game/x.csproj

dotnet build ./Sample1/x.csproj

g++ ./Sample2/src/x.cpp -o ./bin/SampleAI2.exe -g -O2 -std=c++11

.\bin\Antia.exe ./bin/SampleAI1.exe ./bin/SampleAI1.exe ./bin/SampleAI2.exe
