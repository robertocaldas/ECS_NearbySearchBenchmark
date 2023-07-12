using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMonoBehaviour : MonoBehaviour
{
    public IPhysicalData physicalData;
    void Awake()
    {
        var entity = GetComponent<UEntity>();
        physicalData = new UPhysicalData_KDTree(entity);
    }
}
