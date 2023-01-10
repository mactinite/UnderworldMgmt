using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform shootFrom;

    public Transform prefab;

    public float interval = 0.5f;
    
    [SerializeField] private float range = 20f;

    private float timer = 0f;

    [SerializeField]
    private LayerMask enemyLayerMask;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        bool enemiesInVicinity = Physics2D.OverlapCircle(transform.position, range, enemyLayerMask);
        if (timer > interval && enemiesInVicinity)
        {
            timer = 0;
            Instantiate(prefab, shootFrom.transform.position, prefab.rotation);
        }
    }
}