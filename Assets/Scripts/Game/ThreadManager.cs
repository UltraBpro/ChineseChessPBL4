using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly Queue<Action> executeOnMainThread = new Queue<Action>();
    private static bool actionToExecuteOnMainThread = false;

    private void Update()
    {
        if (actionToExecuteOnMainThread)
        {
            ExecuteQueuedActions();
        }
    }

    public static void ExecuteOnMainThread(Action _action)
    {
        if (_action == null)
        {
            Debug.Log("No action to execute on main thread!");
            return;
        }

        lock (executeOnMainThread)
        {
            executeOnMainThread.Enqueue(_action);
            actionToExecuteOnMainThread = true;
        }
    }

    private void ExecuteQueuedActions()
    {
        lock (executeOnMainThread)
        {
            while (executeOnMainThread.Count > 0)
            {
                Action action = executeOnMainThread.Dequeue();
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Debug.Log("An action threw an exception: " + ex.Message);
                }
            }

            actionToExecuteOnMainThread = false;
        }
    }
}
