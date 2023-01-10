using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomPlacer : MonoBehaviour
{

    public int amount = 100;
    public Transform prefab;

    public int width = 100;
    public int height = 75;

    public Grid grid;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-(width / 2f), (width / 2f)), Random.Range(-(height / 2f), (height / 2f)), transform.position.z);
            if (grid)
            {
                Vector3Int cellPos = grid.WorldToCell(position);
                position = grid.CellToWorld(cellPos);
                position.z = transform.position.z;
            }
            Instantiate(prefab, position, prefab.rotation);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 1f));
    }
}
