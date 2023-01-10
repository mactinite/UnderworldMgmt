using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    public bool reserved = false;
    public string occupantID;
    public string resourceType = "Brimstone";
    public string verb = "Mining";
    public int amountGenerated = 5;
    public MemberJob requiredJob = MemberJob.Mine;
    public float radius = 3f;
    public float energyCost = 1f;
    public float workAmount = 5f;
    public Transform rallyPoint;

    float _workLeft = 5f;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void WorkTick()
    {
        _workLeft -= Time.deltaTime;
        
        if (_workLeft < 0)
        {
            _workLeft = workAmount;
            GenerateResource();
        }
    }

    public void GenerateResource()
    {
        ResourceManager.Instance.AddResource(resourceType, amountGenerated);
    }
    
}
