using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FILOGueue<T> : IEnumerable<T>
{
    T[] array;
    int cap;
    int begin;
    int end;

    int capasity = 0;

    public FILOGueue (int capacity)
    {
        cap = capacity + 1;
        array = new T[cap];
        begin = 0;
        end = 0;
    }

    public int Count
    {
        get { return capasity; }
        
    }

    public T Pop ()
    {
        if (begin == end) 
        {
            Debug.LogError ("No iteams in queue");
            return default(T);
        }
        else 
        { 
            begin--;
            if (begin < 0)
                begin += cap;
            capasity--;
            return array[begin];
        }
    }

    public void Push (T value)
    {
        array[begin] = value;
        begin = (begin + 1) % cap;
        if (begin == end)
            end = (end + 1) % cap;

        capasity++;
    }

    public IEnumerator<T> GetEnumerator ()
    {
        int i = begin - 1;
        while (i != end - 1)
        {
            yield return array[i];
            i--;
            if (i < 0)
                i += cap;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
    {
        return this.GetEnumerator ();
    }
}
