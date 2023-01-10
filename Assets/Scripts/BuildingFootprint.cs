using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFootprint : MonoBehaviour
{
    public float cellSize = 0.5f;

    public int width = 3;
    public int height = 3;


    private void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var collider = gameObject.AddComponent<BoxCollider2D>();

                float xOddOffset = width % 2 == 0 ? ((float) cellSize / 2) : 0f;
                float yOddOffset = height % 2 == 0 ? cellSize / 2 : 0f;

                Vector3 offset = new Vector3(xOddOffset, yOddOffset, 0);
                
                collider.size = new Vector2(cellSize, cellSize);
                collider.offset = new Vector2(offset.x + x - (width / 2), offset.y + y - (height / 2));
                collider.isTrigger = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xOddOffset = width % 2 == 0 ? ((float) cellSize / 2) : 0f;
                float yOddOffset = height % 2 == 0 ? cellSize / 2 : 0f;

                Vector3 offset = new Vector3(xOddOffset, yOddOffset, 0);
                Gizmos.DrawWireCube( ((transform.position) + offset) + new Vector3(x - (width / 2), y - (height / 2)),
                    new Vector3(cellSize, cellSize, 0.1f));
            }
        }
    }
}