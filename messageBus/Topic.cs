using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public class Topic<T> : ITopic<T> where T : struct
{
    private List<Action<T>> handlers = new List<Action<T>>();

    public void Publish(T message)
    {
        foreach(var handler in handlers)
        {
            handler(message);
        }
    }
    
    public void Subscribe(Action<T> handler)
    {
        handlers.Add(handler);
    }

    public void Subscribe(MethodInfo method, object instance)
    {
        var action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), instance, method);
        handlers.Add(action);
    }

    public void Unsubscribe(Action<T> handler)
    {
        handlers.Remove(handler);
    }

    public void Unsubscribe(MethodInfo method, object instance)
    {
        var action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), instance, method);
        var x = handlers.IndexOf(action);
        handlers.Remove(action);
    }
}
