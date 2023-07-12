using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class EntityType : Tag<EntityType> { }
public partial class EntityStatus : Tag<EntityStatus> { }

public class Entity
{
    private bool _firstTime = true;

    private IRegularBehaviour _currentRegularBehaviour;
    private readonly IList<IBehaviour> _behaviours;
    private readonly IList<IRegularBehaviour> _regularBehaviours;
    private readonly IList<IAlwaysFirstBehaviour> _alwaysFirstBehaviours;
    private readonly IList<IParallelBehaviour> _parallelBehaviours;
    private readonly IList<IAlwaysLastBehaviour> _alwaysLastBehaviours;
    private readonly Dictionary<Type, IBehaviour> _behaviourMap;

    [Injectable]
    public int Id { get; private set; }

    [Injectable]
    public Dictionary<string, Float> RequiredAttributes { get; protected set; }

    [Injectable]
    private ILogger _logger;

    public event Action Destroyed;

    [Injectable]
    public IPhysicalData PhysicalData { get; private set; }

    //todo: all (most) should be injected
    public EntityType EntityType { get; private set; }
    public EntityStatus EntityStatus { get; private set; }

    public T GetBehaviour<T>() where T : class
    {
        var t = typeof(T);
        return _behaviourMap[t] as T;
    }

    public Entity(IList<IBehaviour> behaviours, EntityType entityType, EntityStatus entityStatus)
    {
        _behaviours = behaviours;
        _regularBehaviours = _behaviours.Where(b => typeof(IRegularBehaviour).IsInstanceOfType(b)).Cast<IRegularBehaviour>().ToList();
        _alwaysFirstBehaviours = _behaviours.Where(b => typeof(IAlwaysFirstBehaviour).IsInstanceOfType(b)).Cast<IAlwaysFirstBehaviour>().ToList();
        _parallelBehaviours = _behaviours.Where(b => typeof(IParallelBehaviour).IsInstanceOfType(b)).Cast<IParallelBehaviour>().ToList();
        _alwaysLastBehaviours = _behaviours.Where(b => typeof(IAlwaysLastBehaviour).IsInstanceOfType(b)).Cast<IAlwaysLastBehaviour>().ToList();
        _behaviourMap = behaviours.ToDictionary(b => b.GetType(), b => b);

        _currentRegularBehaviour = null;

        EntityType = entityType;
        EntityStatus = entityStatus;

        EntitiesManager.Instance.Register(this);
    }

    public void MarkToDestroy()
    {
        EntitiesManager.Instance.Deregister(this, DoDestroy);
    }

    private void DoDestroy()
    {
        Destroyed?.Invoke();
    }

    public void Tick()
    {
        if(_firstTime)
        {
            // TODO: always last and parallel behaviours shouls start after start of regular behaviour
            StartNonRegularBehaviours();
            _firstTime = false;
        }

        for (int i = 0; i < _alwaysFirstBehaviours.Count; i++)
        {
            _alwaysFirstBehaviours[i].Tick();
        }

        var score = float.NegativeInfinity;
        IRegularBehaviour mostScoringBehaviour = null;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < _regularBehaviours.Count; i++)
        {
            var behaviour = _regularBehaviours[i];

            var evaluation = behaviour.Evaluate();
            if (evaluation > score)
            {
                score = evaluation;
                mostScoringBehaviour = behaviour;
            }

            sb.Append($"{behaviour.GetType().Name} = {evaluation:000.0} ");
        }

        sb.Append($"<b>Winner: {mostScoringBehaviour.GetType().Name} = {score:0.#}</b>");

        if (mostScoringBehaviour != _currentRegularBehaviour)
        {
            sb.Append("<b> (starting)</b>");
            _currentRegularBehaviour?.Stop();
            _currentRegularBehaviour = mostScoringBehaviour;
            _currentRegularBehaviour.Start();
        }

        _logger.Watch(Id, "Evaluations", sb.ToString());
        _currentRegularBehaviour?.Tick();


        for (int i = 0; i < _parallelBehaviours.Count; i++)
        {
            _parallelBehaviours[i].Tick();
        }

        for (int i = 0; i < _alwaysLastBehaviours.Count; i++)
        {
            _alwaysLastBehaviours[i].Tick();
        }
    }

    private void StartNonRegularBehaviours()
    {
        Action<IList<IBehaviour>> start = (behaviours) =>
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                behaviours[i].Start();
            }
        };

        start(_alwaysFirstBehaviours.Cast<IBehaviour>().ToList());
        start(_parallelBehaviours.Cast<IBehaviour>().ToList());
        start(_alwaysLastBehaviours.Cast<IBehaviour>().ToList());
    }
}