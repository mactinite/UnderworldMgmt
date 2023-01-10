using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableResource : MonoBehaviour
{
    public MemberJob jobRequired = MemberJob.Mine;
    public string resourceType;
    public string verb = "Gathering Brimstone";
    public int amount = 10;
    [SerializeField] private SpriteRenderer sprite;
    public int health;
    private float iTime = 0.3f;
    private float iTimer = 0f;
    
    [SerializeField] private Material flashMaterial;
    private Material baseMaterial;

    [SerializeField] private ParticleSystem particleSystem;
    
    void Start()
    {
        baseMaterial = sprite.material;
    }

    private void Update()
    {
        iTimer -= Time.deltaTime;
    }

    public void Damage(int amount)
    {
        if (iTimer <= 0)
        {
            health -= amount;
            iTimer = iTime;
            StartCoroutine(FlashMaterial());
            particleSystem.Emit(1);
            if (health < 0)
            {
                ResourceManager.Instance.AddResource(resourceType, this.amount);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator FlashMaterial()
    {
        sprite.material = flashMaterial;
        yield return new WaitForSeconds(iTime);
        sprite.material = baseMaterial;
    }

}
