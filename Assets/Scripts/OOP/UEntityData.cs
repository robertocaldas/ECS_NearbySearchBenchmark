using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Awake must run before UEntity's & UBaseEntityView's Awake.
[DefaultExecutionOrder(-10)]
public class UEntityData : MonoBehaviour
{

    /// <summary>
    /// Serialized fields are stored in file; changes in values during runtime are not
    /// synchronized back to SerializableDictionaries.
    /// </summary>
    public StringFloatMap SerializedFloatsMap;
    public StringBoolMap BehavioursMap;

    private Dictionary<string, Float> floatsMap = null;
    private Dictionary<string, Float> requiredAttributesMap = null;

    public Dictionary<string, Float> FloatsMap => floatsMap;
    public Dictionary<string, Float> RequiredAttributesMap => requiredAttributesMap;

    public int EntityType;
    public int EntityStatus;

    [Serializable] public class StringFloatMap : USerializableDictionary<string, float> {}
    [Serializable] public class StringBoolMap : USerializableDictionary<string, bool> {}

    private void Awake()
    {
        // Instantiate all ValueTypes and add to dictionary:
        floatsMap = SerializedFloatsMap.ToDictionary(p => p.Key, p => new Float(p.Value));

        // Get all atributes from all types
        var behaviourTypes = ReflectionTools.GetConcreteTypes(typeof(IBehaviour));
        var attributeNamesAndTypes = ReflectionTools.GetInjectableFields(behaviourTypes);

        // Filter only the required attributes 
        requiredAttributesMap = floatsMap
            .Where(p => attributeNamesAndTypes.TryGetValue(p.Key, out var tuple) && tuple.Item2)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}

