using System;

public class SubscribesToAttribute : Attribute
{
    public string Topic;

    public SubscribesToAttribute(string topic)
    {
        this.Topic = topic;
    }
}