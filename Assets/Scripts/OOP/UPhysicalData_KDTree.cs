using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
using UnityEngine;
using System.Linq;

/// <summary>
/// For now it's 2D but it can be 3D with small changes. KDTree is natively 3D.
/// </summary>
public class UPhysicalData_KDTree : IPhysicalData
{
    private const int MaxPointsPerLeafNode = 8;

    private static readonly KDTree _tree = new KDTree(MaxPointsPerLeafNode);
    private static readonly KDQuery _query = new KDQuery();
    private static readonly List<UEntity> _entities = new List<UEntity>();
    private static bool _isDirty;

    private UEntity _entity;
    private Transform _transform;

    private float _x, _y;


    public UPhysicalData_KDTree(UEntity entity)
    {
        _entity = entity;
        _entities.Add(entity);
        _entity.Entity.Destroyed += OnDestroy;

        _transform = entity.transform;
        _x = _transform.position.x;
        _y = _transform.position.y;
        _isDirty = true;
        var count = _tree.Count;
        _tree.SetCount(count + 1);
        _tree.Points[count] = new Vector3(_x, _y);
    }

    private void OnDestroy()
    {
        _entities.Remove(_entity);
        _isDirty = true;
        CheckTreeIntegrity();
    }

    public float X
    {
        get => _x;
        set
        {
            _x = value;
            _transform.position = new Vector3(value, _y, _transform.position.z);
            _isDirty = true;
        }
    }

    public float Y
    {
        get => _y;
        set
        {
            _y = value;
            _transform.position = new Vector3(_x, value, _transform.position.z);
            _isDirty = true;
        }
    }

    public void SetPosition(float x, float y)
    {
        _x = x;
        _y = y;
        _transform.position = new Vector3(x, y, _transform.position.z);
        _isDirty = true;
    }

    public void AddPosition(float x, float y)
    {
        SetPosition(_x + x, _y + y);
    }

    public float Distance(Entity target)
    {
        //return (new Vector3(target.PhysicalData.X, target.PhysicalData.Y, transform.position.z) - transform.position).magnitude;
        var pd = target.PhysicalData;
        float x = pd.X - _x;
        float y = pd.Y - _y;
        return Mathf.Sqrt(x * x + y * y);
    }

    public (float, float) DirectionTo(Entity target)
    {
        var pd = target.PhysicalData;
        var d = new Vector2(pd.X - _x, pd.Y - _y).normalized;
        return (d.x, d.y);
    }

    public IReadOnlyList<Entity> GetNearbyEntities(float radius)
    {
        CheckTreeIntegrity();
        var indices = new List<int>();
        _query.Radius(_tree, _transform.position, radius, indices);
        return indices.Where(i => _entities[i].Entity != _entity.Entity).Select(i => _entities[i].Entity).ToList();
    }

    private void CheckTreeIntegrity()
    {
        if(_isDirty)
        {
            _isDirty = false;
            _tree.Build(_entities.Select(e => new Vector3(e.Entity.PhysicalData.X,
                e.Entity.PhysicalData.Y)).ToArray(), MaxPointsPerLeafNode);
            _tree.Rebuild();
        }
    }
}
