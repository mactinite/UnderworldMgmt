using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Buildings/New Building")]
public class BuildingAsset : ScriptableObject
{
    public string buildingName = "Brimstone Mine";
    public string description = "A place to mine brimstone";
    public BuildingFootprint buildingPrefab;

    public List<Cost> Costs = new List<Cost>();
    
    [Serializable]
    public struct Cost
    {
        public string ResourceType;
        public int Amount;
    }
}
