using System;

public interface IBehaviour
{
    int Id { get; }
    void Start();
    void Tick();
}

public interface IRegularBehaviour : IBehaviour
{
    float Evaluate();
    void Stop();
}

public interface IAlwaysFirstBehaviour : IBehaviour { }
public interface IAlwaysLastBehaviour : IBehaviour { }
public interface IParallelBehaviour : IBehaviour { }
