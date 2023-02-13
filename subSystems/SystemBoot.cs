using System;
using System.Linq;
using System.Reflection;

public class SystemBoot
{
    public void Boot()
    {
        var subSystemTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsDefined(typeof(SubSystemAttribute)))
            .ToList();

        foreach (var subSystemType in subSystemTypes)
        {
            var subSystem = F.Create(subSystemType) as IBootable;
            if (subSystem == null)
            {
                continue;
            }
            subSystem.Boot();
        }
    }
}