using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCrystal : MonoBehaviour
{
    public int health = 300;
    public int maxHealth = 300;
    public ProgressBar healthBar;
    private float iTime = 0.1f;
    private float iTimer = 0f;

    private void Update()
    {
        if (iTimer > 0)
        {
            iTimer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (iTimer <= 0)
        {
            health--;
            iTimer = iTime;
        }
        healthBar.SetValue((float)health / maxHealth);
        if (health <= 0)
        {
            Destroyed();
        }
    }

    public void Destroyed()
    {
        Debug.Log("Game Over: Crystal Destroyed");
        InvaderManager.Instance.Lose();
    }


}
