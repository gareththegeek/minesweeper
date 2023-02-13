using Godot;
using System;

public interface IBus
{
    ITopic Get(Type messageType, string topicName);
    ITopic<T> Get<T>(string topicName) where T: struct;
}