using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class ReflectionTools
{

    public static List<Type> GetConcreteTypes(Type interfaceType)
    {
        return Assembly.GetAssembly(interfaceType).GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(interfaceType))
            .OrderBy(type => type.Name)
            .ToList();
    }

    /// <summary>
    /// Gets all fields with the InjectableAttribute from the list of types passed as parameter.
    /// </summary>
    /// <returns> Field name, type and whether field is Required.</returns>
    public static Dictionary<string, Tuple<Type, bool>> GetInjectableFields(IEnumerable<Type> types, bool onlyRequired = false)
    {
        var fields = new Dictionary<string, Tuple<Type, bool>>();

        foreach (var type in types)
        {
            var injectableAttributeFields = type.GetFields(BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance).Where(f => f.IsDefined(typeof(InjectableAttribute), false));

            foreach (var injectableAttributeFieldInfo in injectableAttributeFields)
            {
                var isFieldRequired = injectableAttributeFieldInfo.GetCustomAttribute<InjectableAttribute>().Required;
                if(onlyRequired && !isFieldRequired)
                {
                    continue;
                }
                var name = injectableAttributeFieldInfo.Name;

                // this is for making sure an attribute is marked as required if in one of the classes it is marked as
                // required, since the same attribute can be used in multiple classes with or without the "Required" option.
                if (fields.TryGetValue(name, out var tuple))
                {
                    isFieldRequired |= tuple.Item2;
                }

                fields[name] = new Tuple<Type, bool>(injectableAttributeFieldInfo.FieldType, isFieldRequired);
            }
        }

        return fields;
    }
}
