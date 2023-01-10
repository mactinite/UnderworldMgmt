using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPurchaseUI : MonoBehaviour
{
    private BuildingAsset buildingAsset;
    public Button buildButton;
    public Transform costLayout;
    public Transform costPrefab;
    public TMP_Text name;
    public TMP_Text description;

    public void SetData(BuildingAsset asset)
    {
        buildingAsset = asset;
        name.text = asset.buildingName;
        description.text = asset.description;

        foreach (var cost in asset.Costs)
        {
            var obj = Instantiate(costPrefab, costLayout);

            obj.GetChild(0).GetComponent<Image>().sprite =
                ResourceManager.Instance.resourceIcons[cost.ResourceType];
            obj.GetChild(1).GetComponent<TMP_Text>().text = $"{cost.Amount}";
        }
        
        buildButton.onClick.AddListener(SetBuilding);
    }

    private void SetBuilding()
    {
        BuildManager.Instance.testBuilding = buildingAsset;
        BuildManager.Instance.buildingMode = true;
        BuildManager.Instance.newPreviewNeeded = true;
    }

    private void Update()
    {
        if (CanAfford())
        {
            buildButton.interactable = true;
        }
        else
        {
            buildButton.interactable = false;
        }
    }

    private bool CanAfford()
    {
        foreach (var cost in buildingAsset.Costs)
        {
            int current = ResourceManager.Instance.resourceLedger[cost.ResourceType];
            if (current - cost.Amount < 0)
            {
                return false;
            }
        }

        return true;
    }
}
