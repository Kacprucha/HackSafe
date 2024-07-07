using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIFOQueue<T> 
{
    //T[] array;
    //int cap;
    //int begin;
    //int end;

    //int capasity = 0;

    private Queue<T> fifoQueue = new Queue<T> ();

    //public FILOGueue (int capacity)
    //{
    //    cap = capacity + 1;
    //    array = new T[cap];
    //    begin = 0;
    //    end = 0;
    //}

    public int Count
    {
        get { return fifoQueue.Count; }
        
    }

    public T Pop ()
    {
        if (fifoQueue.Count > 0)
        {
            T item = fifoQueue.Dequeue ();
            return item;
        }
        else
        {
            Debug.Log ("Queue is empty");
            return default (T);
        }

        //if (begin == end) 
        //{
        //    Debug.LogError ("No iteams in queue");
        //    return default(T);
        //}
        //else 
        //{ 
        //    begin--;
        //    if (begin < 0)
        //        begin += cap;
        //    capasity--;
        //    return array[begin];
        //}
    }

    public void Push (T value)
    {
        fifoQueue.Enqueue (value);
        //array[begin] = value;
        //begin = (begin + 1) % cap;
        //if (begin == end)
        //    end = (end + 1) % cap;

        //capasity++;
    }
}
