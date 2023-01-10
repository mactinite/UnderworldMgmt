using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTile : MonoBehaviour
{
    public bool invalid = false;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Color32 color;
    [SerializeField] private Color32 invalidColor;
    [SerializeField] private BoxCollider2D collider2D;
    [SerializeField] private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] results = new Collider2D[10];
    private void OnTriggerStay2D(Collider2D other)
    {

    }

    private void FixedUpdate()
    {
        spriteRenderer.color = color;
        invalid = false;
        int count = collider2D.OverlapCollider(contactFilter, results);
        if (count > 0)
        {
            spriteRenderer.color = invalidColor;
            invalid = true;
        }
    }

}
