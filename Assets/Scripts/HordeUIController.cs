using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HordeUIController : MonoBehaviour
{
    private HordeManager hordeManager;
    [SerializeField] private Transform hordePanel;

    [SerializeField] private Transform layoutGroup;
    [SerializeField] private HordeMemberUIController uiEntryPrefab;
    [SerializeField]private Dictionary<string, HordeMemberUIController> memberUIMapping = new ();

    [SerializeField]
    public bool showPanel = false;
    private void Start()
    {
        hordeManager = HordeManager.Instance;
        hordeManager.OnHordeChange += OnHordeChange;
        hordePanel.gameObject.SetActive(showPanel);

    }

    private void OnDestroy()
    {
        HordeManager.Instance.OnHordeChange -= OnHordeChange;
    }

    private void OnHordeChange(string id, Member udpatedMember)
    {
        if (memberUIMapping.TryGetValue(id, out var uiElement))
        {
            uiElement.id = id;
            uiElement.name.text = $"{udpatedMember.name}";
            uiElement.status.text = $"{udpatedMember.status}";
            uiElement.energy.text = $"{udpatedMember.energy:F1}/{udpatedMember.maxEnergy:F1}";
            if (udpatedMember.job == MemberJob.Mine)
            {
                uiElement.mineJobButton.interactable = false;
                uiElement.harvestJobButton.interactable = true;
                uiElement.chopJobButton.interactable = true;
            }
            
            if (udpatedMember.job == MemberJob.Harvest)
            {
                uiElement.mineJobButton.interactable = true;
                uiElement.harvestJobButton.interactable = false;
                uiElement.chopJobButton.interactable = true;
            }
            
            if (udpatedMember.job == MemberJob.Chop)
            {
                uiElement.chopJobButton.interactable = false;
                uiElement.mineJobButton.interactable = true;
                uiElement.harvestJobButton.interactable = true;

            }
        }
        else
        {
            var textPrefabInstance = Instantiate(uiEntryPrefab, layoutGroup);
            textPrefabInstance.id = id;
            textPrefabInstance.name.text = $"{udpatedMember.name}";
            textPrefabInstance.status.text = $"{udpatedMember.status}";
            textPrefabInstance.energy.text = $"{udpatedMember.energy:F1}/{udpatedMember.maxEnergy:F1}";

            memberUIMapping.Add(id, textPrefabInstance);
        }
    }

    public void TogglePanel()
    {
        showPanel = !showPanel;
        
        hordePanel.gameObject.SetActive(showPanel);
    }
}