
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class GameObjectExtensions
{

    public static void SetLayerRecursively(this GameObject inst, int layer)
    {
        inst.layer = layer;
        foreach (Transform child in inst.transform)
            child.gameObject.SetLayerRecursively(layer);
    }


    public static float FastDistance(this GameObject from, Vector3 to)
    {
        return Mathf.Abs((from.transform.position - to).sqrMagnitude);
    }

}