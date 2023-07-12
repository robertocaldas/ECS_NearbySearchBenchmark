using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

/// <summary>
/// How to use: inherit a Partial class from Tag and define empty public static
/// integers inside the partial classes. Each integer will become a flag
/// (a bit in a binary mask).
/// Create an instance and use the public methods along with the type-safe
/// flags to set/reset/check the instance's value.
/// </summary>
public abstract class Tag<T> where T : Tag<T>
{
    private int _value;

    static Tag()
    {
        var flags = new List<string>();
        var fields = GetFields();
        for (int i = 0; i < fields.Count; i++)
        {
            fields[i].SetValue(null, 1 << i);
            flags.Add(fields[i].Name);
        }
        Flags = flags;
    }

    public static IReadOnlyList<string> Flags { get; private set; }


    public void SetFlags(int flags)
    {
        _value = flags;
    }

    public bool HasFlags(int flags)
    {
        return (_value & flags) == flags;
    }

    public void AddFlags(int flags)
    {
        _value |= flags;
    }

    public void RemoveFlags(int flags)
    {
        _value &= ~flags;
    }

    public override string ToString()
    {
        var fields = GetFields();
        var binary = Convert.ToString(_value, 2);
        binary = binary.PadLeft(fields.Count, '0');
        StringBuilder sb = new StringBuilder($"{_value} =  0b{binary}\n");
        foreach (var field in GetFields())
        {
            var f = HasFlags((int)field.GetValue(null));
            sb.Append(f ? "(X) " : "(  ) ");
            sb.AppendLine(field.Name);
        }
        return sb.ToString();
    }

    public int ToInteger()
    {
        return _value;
    }

    private static List<FieldInfo> GetFields()
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(int))
            .OrderBy(f => f.Name).ToList();
    }
}
