using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UEntity : MonoBehaviour
{

    // todo: refactor into class?
    private static int _idCounter = 0;

    [SerializeField]
    private int _id;

    public int Id => _id;

    public Entity Entity { get; private set; }

    private void Awake()
    {
        _id = _idCounter++;

        var uEntityData = GetComponent<UEntityData>();

        var behaviours = CreateBehaviours(uEntityData);

        EntityType et = new EntityType();
        et.SetFlags(uEntityData.EntityType);
        EntityStatus es = new EntityStatus();
        es.SetFlags(uEntityData.EntityStatus);

        Entity = new Entity(behaviours, et, es);
        Entity.Destroyed += () => Destroy(gameObject);

        //var physicalData = gameObject.AddComponent<UPhysicalData>();
        var physicalData = new UPhysicalData_KDTree(this);
        var logger = FindObjectOfType<ULogger>();

        InjectThings(uEntityData, behaviours, physicalData, logger);
    }

    private IList<IBehaviour> CreateBehaviours(UEntityData uEntityData)
    {
        var behaviourNames = uEntityData.BehavioursMap.Where(p => p.Value).Select(p => p.Key).ToList();

        BehaviourFactory behaviourFactory = new BehaviourFactory(behaviourNames);
        return behaviourFactory.Build();
    }

    private void InjectThings(UEntityData uEntityData, IList<IBehaviour> behaviours,
        IPhysicalData physicalData, ILogger logger)
    {
        Injector.InjectProperty(_id, Entity, nameof(Entity.Id));
        Injector.InjectProperty(physicalData, Entity, "PhysicalData");
        Injector.Inject(uEntityData.RequiredAttributesMap, Entity);
        Injector.Inject(logger, Entity);

        foreach (var behaviour in behaviours)
        {
            Injector.InjectFloats(uEntityData.FloatsMap, behaviour);

            Injector.InjectProperty(_id, behaviour, nameof(Entity.Id));
            Injector.Inject(uEntityData.RequiredAttributesMap, behaviour);
            Injector.Inject(physicalData, behaviour);
            Injector.Inject(logger, behaviour);
            Injector.Inject(Entity, behaviour);
        }
    }
}

