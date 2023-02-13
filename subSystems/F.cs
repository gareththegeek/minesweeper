using System;
using System.Linq;
using System.Reflection;

public class F
{
    private static void AttachSubscriptions(object instance, bool bind)
    {
        var type = instance.GetType();

        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => x.IsDefined(typeof(SubscribesToAttribute), true))
            .ToList();

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttributes(typeof(SubscribesToAttribute), true).Single() as SubscribesToAttribute;
            var messageParameterInfo = method.GetParameters().Single();

            if (bind)
            {
                Bus.Instance.Get(messageParameterInfo.ParameterType, attribute.Topic).Subscribe(method, instance);
            }
            else
            {
                Bus.Instance.Get(messageParameterInfo.ParameterType, attribute.Topic).Unsubscribe(method, instance);
            }
        }
    }

    public static void Bind(object instance)
    {
        var type = instance.GetType();

        AttachSubscriptions(instance, true);

        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => x.IsDefined(typeof(TopicAttribute), true))
            .ToList();

        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttributes(typeof(TopicAttribute), true).Single() as TopicAttribute;
            var messageType = field.FieldType.GenericTypeArguments.Single();
            var topic = Bus.Instance.Get(messageType, attribute.Topic);
            field.SetValue(field.IsStatic ? null : instance, topic);
        }
    }

    public static void Unbind(object instance)
    {
        AttachSubscriptions(instance, false);
    }

    public static object Create(Type objectType, params object[] constructorParams)
    {
        var result = Activator.CreateInstance(objectType, constructorParams);

        Bind(result);

        return result;
    }

    public static T Create<T>(params object[] constructorParams) where T : class
    {
        var type = typeof(T);
        var result = (T)Activator.CreateInstance(type, constructorParams);

        Bind(result);

        return result;
    }
}