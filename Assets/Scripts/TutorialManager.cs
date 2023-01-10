using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public TMP_Text tutorialText;
    public PlayerInput playerInput;
    public HordeUIController hordeUI;
    private int tutorialStep = 0;

    public string[] tutorialTexts = {
        "Use <b> WASD </b> to move",
        "Use your <b>mouse to aim</b> and use <b>left click</b> to swing your scythe.",
        "Find resources on the map and hit them with your scythe to harvest them",
        "Harvest 50 <b>Brimstone</b>",
        "Use the <b>Build Menu</b> to build a Summoning Circle",
        "Use the <b>Demons Menu</b> to manage your <b>Demons</b> and change their jobs",
        "Build and harvest to defend yourself from the impending invasion!"
    };
    
    private void Update()
    {
        if (tutorialStep == 0)
        {
            tutorialText.text = tutorialTexts[0];

            if (playerInput.actions["Movement"].WasPerformedThisFrame())
            {
                tutorialStep++;
            }
        }

        if (tutorialStep == 1)
        {
            tutorialText.text = tutorialTexts[1];

            if (playerInput.actions["Attack"].WasPerformedThisFrame())
            {
                tutorialStep++;
            }
        }
        
        if (tutorialStep == 2)
        {
            tutorialText.text = tutorialTexts[2];
            if (ResourceManager.Instance.resourceLedger.ContainsKey("Brimstone") && ResourceManager.Instance.resourceLedger["Brimstone"] > 0)
            {
                tutorialStep++;
            }
            
        }
        
        if (tutorialStep == 3)
        {
            tutorialText.text = tutorialTexts[3];
            if (ResourceManager.Instance.resourceLedger.ContainsKey("Brimstone") && ResourceManager.Instance.resourceLedger["Brimstone"] >= 50)
            {
                tutorialStep++;
            }
            
        }
        
        if (tutorialStep == 4)
        {
            tutorialText.text = tutorialTexts[4];
            if (HordeManager.Instance.hordeMembers.Count > 0)
            {
                tutorialStep++;
            }
            
        }
        
        if (tutorialStep == 5)
        {
            tutorialText.text = tutorialTexts[5];
            if (hordeUI.showPanel)
            {
                tutorialStep++;
            }
            
        }

        if (tutorialStep == 6)
        {
            tutorialText.text = tutorialTexts[6];
            if (InvaderManager.Instance.currentTime <= 15)
            {
                tutorialText.gameObject.SetActive(false);
            }
        }
    }
}
