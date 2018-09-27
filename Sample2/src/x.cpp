#include <cstdio>
#include <cstdlib>
#include <cmath>
#include <iostream>
#include <vector>
#include <ctime>
#include <algorithm>
using namespace std;

#define rep(i,a,b) for(int i=a,lim__lim=b; i<=lim__lim; i++)

vector<string> type;
vector<string> owner;
struct pt{ int x, y; };
vector<pt> atkArr;
vector<pt> defArr;
vector<pt> bldArr;
const int dx[] = { 0, 0, 1, -1 };
const int dy[] = { 1, -1, 0, 0 };

int main()
{
    srand(time(NULL));
    
    int n = 40;
    int m = 80;
    string inp;
    getline(cin, inp);
    int p; sscanf(inp.c_str(), "%d", &p);
    type.resize(n); owner.resize(n);
    rep(i, 0, n-1) getline(cin, type[i]);
    rep(i, 0, n-1) getline(cin, owner[i]);
    
    int atkpt = 2;
    int defpt = 2;
    rep(i, 0, n-1) rep(j, 0, m-1) if(owner[i][j] == '0' + p && type[i][j] == 'x') atkpt++;
    rep(i, 0, n-1) rep(j, 0, m-1) if(owner[i][j] == '0' + p && type[i][j] == 'H') defpt++;
    
    rep(i, 0, n-1) rep(j, 0, m-1)
    {
        if(owner[i][j] != '0' + p) continue;
        bldArr.push_back(pt{ i, j });
        defArr.push_back(pt{ i, j });
        rep(k, 0, 3)
        {
            int nx = i + dx[k];
            int ny = j + dy[k];
            if(!(0 <= nx && nx < n && 0 <= ny && ny < m)) continue;
            if(owner[nx][ny] == '0' + p) continue;
            if(type[nx][ny] == '#') continue;
            atkArr.push_back(pt{ nx, ny });
        }
    }
    
    random_shuffle(atkArr.begin(), atkArr.end());
    random_shuffle(defArr.begin(), defArr.end());
    random_shuffle(bldArr.begin(), bldArr.end());
    
    if(rand() % 10 == 9) rep(i, 0, min(atkpt / 7 - 1, (int)bldArr.size() - 1)) printf("build %d %d\n", bldArr[i].x, bldArr[i].y), atkpt -= 7;
    rep(i, 0, min(defpt - 1, (int)defArr.size() - 1)) printf("defend %d %d\n", defArr[i].x, defArr[i].y);
    rep(i, 0, min(atkpt - 1, (int)atkArr.size() - 1)) printf("attack %d %d\n", atkArr[i].x, atkArr[i].y);
    
    return 0;
}
