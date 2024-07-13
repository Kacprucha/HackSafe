using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIFOQueue<T> 
{
    private Queue<T> fifoQueue = new Queue<T> ();

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
    }

    public void Push (T value)
    {
        fifoQueue.Enqueue (value);
    }

    public T[] GetArryOfObjectsInQueue ()
    {
        return fifoQueue.ToArray ();
    }
}
