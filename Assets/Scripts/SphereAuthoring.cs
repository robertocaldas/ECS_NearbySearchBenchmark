using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class SphereAuthoring : MonoBehaviour
{
    public GameObject SpherePrefab;




    public class SphereBaker : Baker<SphereAuthoring>
    {
        public override void Bake(SphereAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Sphere
            {
                Prefab = GetEntity(authoring.SpherePrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct Sphere : IComponentData
{
    public Unity.Entities.Entity Prefab;
}