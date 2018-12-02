using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionProcessor : MonoBehaviour
{
    private Queue<Action> _delegateQueue;

    public void Start()
    {
        _delegateQueue = new Queue<Action>();
    }

    public void AddFuncToQueue(Action delegateMethod)
    {
        lock (_delegateQueue)
        {
            _delegateQueue.Enqueue(delegateMethod);
        }
    }

    private void Update()
    {
        lock (_delegateQueue)
        {
            for (var i = 0; i < _delegateQueue.Count; i++)
            {
                _delegateQueue.Dequeue().Invoke();
            }
        }
    }
}