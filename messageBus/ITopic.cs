using Godot;
using System;
using System.Reflection;

public interface ITopic
{
    void Subscribe(MethodInfo method, object instance);
    void Unsubscribe(MethodInfo method, object instance);
}

public interface ITopic<T> : ITopic where T : struct
{
    void Subscribe(Action<T> handler);
    void Unsubscribe(Action<T> handler);
    void Publish(T message);
}
