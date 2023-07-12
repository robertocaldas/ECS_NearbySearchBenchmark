using System;
using System.Reflection;

/// <summary>
/// Non-Unity generic singleton with early instantiation.
/// </summary>
public class Singleton<T> where T : class
{
    private Singleton() { }
    static Singleton()
    {
        ConstructorInfo constructorInfo = typeof(T).GetConstructor(BindingFlags.Instance |
               BindingFlags.NonPublic, Type.DefaultBinder, Type.EmptyTypes, null);

        Instance = constructorInfo.Invoke(null) as T;

    }
    public static T Instance { get; private set; }
}