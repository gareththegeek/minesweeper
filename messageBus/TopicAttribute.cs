using System;

public class TopicAttribute : Attribute
{
    public string Topic;

    public TopicAttribute(string topic)
    {
        this.Topic = topic;
    }
}