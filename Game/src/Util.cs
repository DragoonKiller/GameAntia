using System;

public class Ref<T>
{
    public T v;
    public Ref(T v) { this.v = v; }
    public static implicit operator T(Ref<T> g) => g.v;
}


public static class Functional
{
    public static void Foreach<T>(this T[,] t, Func<int,int,T,T> F)
    {
        for(int i=0; i<t.GetLength(0); i++) for(int j=0; j<t.GetLength(1); j++) t[i,j] = F(i, j, t[i, j]);
    }
    
    public static int Count<T>(this T[] arr, Predicate<T> F)
    {
        int cnt = 0;
        foreach(var i in arr) if(F(i)) cnt++;
        return cnt;
    }
    
    public static T[] Filter<T>(this T[] arr, Predicate<T> F)
    {
        int cnt = arr.Count(F);
        T[] x = new T[cnt];
        int t = 0;
        for(int i=0; i<arr.Length; i++) if(F(arr[i])) x[t++] = arr[i];
        return x;
    }
    
}
