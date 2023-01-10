using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fillImage;
    public bool shakeOnChange = false;
    public float shakeAmount = 0.2f;
    public float dampingSpeed;
    private float _currentValue = 1;
    private float _velocity = 0;

    private void Update()
    {
        fillImage.fillAmount = Mathf.SmoothDamp(fillImage.fillAmount, _currentValue, ref _velocity, dampingSpeed);
    }

    public void SetValue(float val)
    {
        if (shakeOnChange)
        {
            Vector3 shakeAxis = Vector3.one * shakeAmount;
            iTween.PunchScale(gameObject,shakeAxis, 0.4f);
        }

        _currentValue = val;
    }
}