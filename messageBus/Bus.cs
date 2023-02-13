using Godot;
using System;
using System.Collections.Generic;

public class Bus : IBus
{
    private static IBus instance;
    public static IBus Instance { get => instance; }
    static Bus()
    {
        instance = new Bus();
    }

    private Dictionary<string, ITopic> topics = new Dictionary<string, ITopic>();

    public ITopic Get(Type messageType, string topicName)
    {
        ITopic result;
        if (topics.TryGetValue(topicName, out result))
        {
            return result;
        }

        var openType = typeof(Topic<>);
        var closedType = openType.MakeGenericType(new Type[] { messageType });
        var newTopic = (ITopic)Activator.CreateInstance(closedType);
        topics[topicName] = newTopic;
        return newTopic;
    }

    public ITopic<T> Get<T>(string topicName) where T: struct
    {
        ITopic result;
        if (topics.TryGetValue(topicName, out result))
        {
            return result as ITopic<T>;
        }

        var newTopic = new Topic<T>();
        topics[topicName] = newTopic;
        return newTopic;
    }
}
