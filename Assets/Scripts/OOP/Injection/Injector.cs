
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// todo: maybe shouldnt be static and could run only 1 getfields for all injections
public static class Injector 
{
    // todo: should be "ref object obj" but I got a compilation error if I do that.
    public static void InjectFloats(IDictionary<string, Float> floatsDictionary, object obj)
    {


        var fields = obj.GetType().GetFields(BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance)
            .Where(f => f.IsDefined(typeof(InjectableAttribute), false))
            .Where(f => f.FieldType == typeof(Float));
        foreach (var field in fields)
        {
            var name = field.Name;
            if(floatsDictionary.TryGetValue(name, out var value))
            {
                field.SetValue(obj, value);
            }
            else
            {
                throw new System.Exception($"{nameof(floatsDictionary)} does not contain a key \"{field.Name}\" required by object of type {obj.GetType().Name}.");
            }
        }
    }

    /// <summary>
    /// Inject variableObj into obj in the first field that uses the attribute Injectable and matches the type of variableObj.
    /// variableObj can be either field or property.
    /// No action is taken if the field/property is not found.
    /// </summary>
    public static void Inject<T>(T variableObj, object obj)
    {
        const BindingFlags bindingFlags = BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance;

        var field = obj.GetType().GetFields(bindingFlags)
            .Where(f => f.IsDefined(typeof(InjectableAttribute), false))
            .Where(f => f.FieldType == typeof(T)).FirstOrDefault();
        field?.SetValue(obj, variableObj);

        if(field == null) // if field null then search properties
        {
            var property = obj.GetType().GetProperties(bindingFlags)
            .Where(p => p.IsDefined(typeof(InjectableAttribute), false))
            .Where(p => p.PropertyType == typeof(T)).FirstOrDefault();
            property?.SetValue(obj, variableObj);
        }
    }

    //todo: this can be refactored to work with both field and property, see method above
    public static void InjectProperty<T>(T fieldObj, object obj, string fieldName)
    {
        try
        {
            var field = obj.GetType().GetProperty(fieldName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            if (field != null && field.IsDefined(typeof(InjectableAttribute), false))
            {
                field.SetValue(obj, fieldObj);
            }
            else
            {
                throw new InvalidOperationException();
            }
        } catch (Exception e)
        {
            throw new Exception($"Something went wrong when trying to inject {fieldName} into {obj}.", e);
        }
    }
}
