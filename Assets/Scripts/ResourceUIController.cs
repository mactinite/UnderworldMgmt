using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIController : MonoBehaviour
{
    [SerializeField]
    private Transform resourceUILayout;
    [SerializeField]
    private Transform resourceUIPrefab;
    private Dictionary<string, Transform> resourceUIMapping = new Dictionary<string, Transform>();
    [SerializeField]
    private bool showPanel = true;

    private void Start()
    {
        ResourceManager.Instance.OnResourceUpdated += OnResourceUpdated;
        resourceUILayout.gameObject.SetActive(showPanel);
    }

    private void OnResourceUpdated(Tuple<string, int> delta)
    {
        string resourceType = delta.Item1;
        int newAmount = delta.Item2;
        
        if (resourceUIMapping.TryGetValue(delta.Item1, out var uiElement))
        {
            var tmp = uiElement.GetChild(1).GetComponent<TMP_Text>();

            tmp.text = $"{resourceType}: {newAmount}";
        }
        else
        {
            var newUiElement = Instantiate(resourceUIPrefab, resourceUILayout);
            var tmp = newUiElement.GetChild(1).GetComponent<TMP_Text>();
            var image = newUiElement.GetChild(0).GetComponent<Image>();
            image.sprite = ResourceManager.Instance.resourceIcons[resourceType];
            tmp.text = $"{resourceType}: {newAmount}";
            
            resourceUIMapping.Add(resourceType, newUiElement);
        }
    }
    public void TogglePanel()
    {
        showPanel = !showPanel;
        
        resourceUILayout.gameObject.SetActive(showPanel);
    }

}
