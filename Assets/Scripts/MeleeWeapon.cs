using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public Transform weaponGraphics;
    public Transform weaponCenter;
    public Animator weaponAnimator;
    
    
    public float rotation = 0;
    private float currentRotation;
    public float rotationSpeed = 1f;
    private float vel;


    public void LateUpdate()
    {
        // billboard weapon;
        weaponGraphics.forward = Camera.main.transform.forward;

        var newRotation = Quaternion.Euler(new Vector3(-60, weaponGraphics.rotation.eulerAngles.y, weaponGraphics.rotation.eulerAngles.z));

        weaponGraphics.rotation = newRotation;

        rotation = Mathf.Clamp(rotation, 0, 360);

        currentRotation = Mathf.SmoothDampAngle(currentRotation, rotation, ref vel, rotationSpeed);
        
        weaponCenter.rotation = Quaternion.AngleAxis(currentRotation, Vector3.forward);
        
        
        
    }


    public void Attack()
    {
        weaponAnimator.SetTrigger("Attack");
    }
}
