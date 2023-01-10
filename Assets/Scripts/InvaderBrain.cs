using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class InvaderBrain : MonoBehaviour
{
    public int health = 5;
    [SerializeField] private AIPath pathfinder;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject spriteParent;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer bodySprite;

    [SerializeField] private Material flashMaterial;
    private Material baseMaterial;
    
    private SoulCrystal soulCrystal;
    
    
    private float iTime = 0.3f;
    private float iTimer = 0f;

    [SerializeField]
    private float speed= 4;
    [SerializeField]
    private float slowSpeed = 1;

    public bool dead;

    public bool slowed;

    // Start is called before the first frame update
    void Start()
    {
        soulCrystal = GameObject.FindObjectOfType<SoulCrystal>();
        baseMaterial = sprite.material;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Slow"))
        {
            slowed = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Slow"))
        {
            slowed = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dead) return;
        
        
        iTimer -= Time.deltaTime;
        
        if (pathfinder.desiredVelocity.magnitude > 0f)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        pathfinder.maxSpeed = slowed ? slowSpeed : speed;

        spriteParent.transform.localScale = new Vector3(pathfinder.desiredVelocity.x < 0 ? -1 : 1, 1, 1);


        pathfinder.destination = soulCrystal.transform.position;

        if (gameObject.FastDistance(soulCrystal.transform.position) < 3f)
        {
            pathfinder.canMove = false;
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsAttacking", true);
            soulCrystal.Attack();
        }
        else
        {
            pathfinder.canMove = true;
            animator.SetBool("IsAttacking", false);

        }
    }

    public void Damage(int amount)
    {
        if (iTimer <= 0 && !dead)
        {
            health -= amount;
            iTimer = iTime;
            StartCoroutine(FlashMaterial());
            if (health < 0)
            {
                Die();
            }
        }
    }

    IEnumerator FlashMaterial()
    {
        sprite.material = flashMaterial;
        bodySprite.material = flashMaterial;
        yield return new WaitForSeconds(iTime);
        sprite.material = baseMaterial;
        bodySprite.material = baseMaterial;
    }

    public void Die()
    {
        dead = true;
        pathfinder.enabled = false;
        animator.SetTrigger("Death");
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
        Destroy(gameObject, 5f);
    }
}
