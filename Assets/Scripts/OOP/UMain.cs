using System;
using UnityEngine;
using UnityEngine.Profiling;

// Awake must run before everything to set the RANDOM SEED
[DefaultExecutionOrder(-100)]
public class UMain : MonoBehaviour
{
    [SerializeField]
    private int _systemSeed;


    [SerializeField]
    private bool _paused;

    [SerializeField]
    private int _tick = 0;

    [SerializeField]
    private int _steps;

    public int Steps => _steps;
    public int Tick => _tick;
    public bool Paused => _paused;


    private Unity.Mathematics.Random _random;

    public GameObject EntityPrefab;

    private void Awake()
    {
        UnityEngine.Random.InitState(_systemSeed);
        _random = Unity.Mathematics.Random.CreateFromIndex((uint)_systemSeed);

    }

    private void Start()
    {
        var now = DateTime.Now;
        for(int i = 0; i < 15000; i++)
        {
            var pos = _random.NextFloat2(500f); 
            Instantiate(EntityPrefab, (Vector2) pos, Quaternion.identity);
        }
        $"Created instances in {(DateTime.Now - now).TotalMilliseconds} milliseconds.".Log();

        int neighbours = 0;
        now = DateTime.Now;

        foreach(var entity in EntitiesManager.Instance.Entities)
        {
            neighbours += entity.PhysicalData.GetNearbyEntities(100f).Count;
        }

        $"Neighbours = {neighbours}. Counted neighbours in  {(DateTime.Now - now).TotalMilliseconds} milliseconds.".Log();
    }

}
