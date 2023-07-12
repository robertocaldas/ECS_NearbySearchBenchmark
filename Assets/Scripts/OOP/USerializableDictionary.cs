using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class USerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception($"There are {keys.Count} keys and {values.Count} values after deserialization. Make sure that both key and value types are serializable.");

        for (int i = 0; i < keys.Count; i++)
        {
            //$"{i}: {keys[i]}, {values[i]}".Log();
            this.Add(keys[i], values[i]);
        }
    }
}


//// This was never tested: ////
[Serializable]
public class USerializableDictionary<TKey, TValue1, TValue2> : Dictionary<TKey, Tuple<TValue1, TValue2>>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<Tuple<TValue1, TValue2>> values = new List<Tuple<TValue1, TValue2>>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        "Saving...".LogWarning();
        keys.Clear();
        values.Clear();
        foreach (var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(new Tuple<TValue1, TValue2>(pair.Value.Item1, pair.Value.Item2));
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

        for (int i = 0; i < keys.Count; i++)
        {
            //$"{i}: {keys[i]}, {values[i]}".Log();
            this.Add(keys[i], values[i]);
        }
    }
}