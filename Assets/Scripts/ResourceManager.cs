using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public Dictionary<string, int> resourceLedger = new Dictionary<string, int>();
    public Action<Tuple<string, int>> OnResourceUpdated; 
    public Dictionary<string, Sprite> resourceIcons = new Dictionary<string, Sprite>();

    public ResourceType[] resourceTypes;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
        
        foreach (var resourceType in resourceTypes) 
        {
            resourceLedger.Add(resourceType.name, 0);
            resourceIcons.Add(resourceType.name, resourceType.icon);
            OnResourceUpdated?.Invoke(new Tuple<string, int>(resourceType.name, 0));
        }
        
    }


    public void AddResource(string resource, int amount)
    {
        if (resourceLedger.ContainsKey(resource))
        {
            resourceLedger[resource] += amount;
            OnResourceUpdated?.Invoke(new Tuple<string, int>(resource, resourceLedger[resource]));
            return;
        }
        
        Debug.LogWarning($"ResourceType '{resource}' is not defined!");

    }
    [Serializable]
    public struct ResourceType
    {
        public string name;
        public Sprite icon;
    }

    public void SpendResource(string costResourceType, int costAmount)
    {
        if (resourceLedger.ContainsKey(costResourceType))
        {
            resourceLedger[costResourceType] -= costAmount;
            OnResourceUpdated?.Invoke(new Tuple<string, int>(costResourceType, resourceLedger[costResourceType]));
            return;
        }
        
        Debug.LogWarning($"ResourceType '{costResourceType}' is not defined!");
    }
}

