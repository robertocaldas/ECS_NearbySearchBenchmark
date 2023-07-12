
using System;
using System.Collections.Generic;
using System.Linq;

public class BehaviourFactory
{
    private IList<string> _behaviourNames;
    public BehaviourFactory(IList<string> behaviourNames)
    {
        _behaviourNames = behaviourNames;
    }

    public IList<IBehaviour> Build()
    {
        var behaviours = new List<IBehaviour>();

        var ibehaviourTypes = System.Reflection.Assembly.GetAssembly(typeof(IBehaviour)).GetTypes()
            .Where(mytype => mytype.GetInterfaces().Contains(typeof(IBehaviour)));

        foreach (var ibehaviourType in ibehaviourTypes)
        {
            if (_behaviourNames.Contains(ibehaviourType.Name))
            {
                behaviours.Add(Activator.CreateInstance(ibehaviourType) as IBehaviour);
            }
        }

        return behaviours;
    }
}
