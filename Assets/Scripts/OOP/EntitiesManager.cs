using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

public class EntitiesManager
{
    private readonly Dictionary<Entity, Action> _entitiesToDeregister = new Dictionary<Entity, Action>();
    public List<Entity> Entities { get; } = new List<Entity>();
    public static EntitiesManager Instance => Singleton<EntitiesManager>.Instance;

    private EntitiesManager() { }

    public void Register(Entity entity)
    {
        Entities.Add(entity);
    }

    public void Deregister(Entity entity, Action onDeregister)
    {
        _entitiesToDeregister.Add(entity, onDeregister);
    }

    public void Step()
    {
        Profiler.BeginSample("EntitiesManager.Step");
        foreach (var entity in Entities)
        {
            entity.Tick();
        }
        Profiler.EndSample();

        foreach(var pair in _entitiesToDeregister)
        {
            Entities.Remove(pair.Key);
            pair.Value?.Invoke();
        }
        _entitiesToDeregister.Clear();
    }
}
