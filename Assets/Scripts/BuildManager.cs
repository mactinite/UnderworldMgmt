using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviour
{
    public List<BuildingAsset> buildings;

    public Transform layoutGroup;
    public Transform buildMenu;

    public BuildingPurchaseUI UIPrefab;

    public BuildingAsset testBuilding;
    public PreviewTile previewPrefab;
    public bool buildingMode;
    public Grid buildGrid;
    public Tilemap groundTilemap;
    public PlayerControllerInput input;
    public static BuildManager Instance;
    private Vector3 CursorGridPosition;

    public bool newPreviewNeeded = true;
    private GameObject previewParent;

    public List<PreviewTile> previewPrefabInstances = new List<PreviewTile>();
    public List<PreviewTile> inactivePreviewPrefabInstances = new List<PreviewTile>();
    private Vector3 CursorGridPositionLastFrame;
    public TMP_Text inputDescriptor;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            previewParent = new GameObject("PreviewParent");
        }
    }

    private void Start()
    {
        RenderUI();
    }

    private void OnDestroy()
    {
        
    }

    private void Update()
    {
        Vector3 worldCursorPosition = input.cursor.position;

        var cellPos = buildGrid.WorldToCell(worldCursorPosition);
        CursorGridPositionLastFrame = CursorGridPosition;
        CursorGridPosition = buildGrid.CellToWorld(cellPos);

        if (buildingMode)
        {
            previewParent.SetActive(true);
            RenderPreview(testBuilding.buildingPrefab);
            inputDescriptor.gameObject.SetActive(true);
        }
        else
        {
            inputDescriptor.gameObject.SetActive(false);
            previewParent.SetActive(false);
        }
    }

    public void RenderPreview(BuildingFootprint buildingFootprint)
    {
        if (newPreviewNeeded)
        {
            for (int i = previewPrefabInstances.Count - 1; i >= 0; i--)
            {
                previewPrefabInstances[i].gameObject.SetActive(false);
                inactivePreviewPrefabInstances.Add(previewPrefabInstances[i]);
                previewPrefabInstances.RemoveAt(i);
            }

            previewParent.transform.position = CursorGridPosition;

            for (int x = 0; x < buildingFootprint.width; x++)
            {
                for (int y = 0; y < buildingFootprint.height; y++)
                {
                    float xOddOffset = buildingFootprint.width % 2 == 0 ? ((float) buildingFootprint.cellSize / 2) : 0f;
                    float yOddOffset = buildingFootprint.height % 2 == 0 ? buildingFootprint.cellSize / 2 : 0f;

                    Vector2 offset = new Vector2(xOddOffset, yOddOffset);
                    
                    Vector3 previewPosition = (CursorGridPosition + (Vector3)offset) +  new Vector3(x - (buildingFootprint.width / 2),
                        y - (buildingFootprint.height / 2), 0.5f);

                    if (inactivePreviewPrefabInstances.Count <= 0)
                    {
                        PreviewTile tile = Instantiate(previewPrefab, previewPosition, Quaternion.identity);
                        tile.transform.SetParent(previewParent.transform, true);
                        previewPrefabInstances.Add(tile);
                    }
                    else
                    {
                        inactivePreviewPrefabInstances[0].gameObject.SetActive(true);
                        inactivePreviewPrefabInstances[0].transform.position = previewPosition;
                        previewPrefabInstances.Add(inactivePreviewPrefabInstances[0]);
                        inactivePreviewPrefabInstances.RemoveAt(0);
                    }
                }
            }

            newPreviewNeeded = false;
        }
        else if(CursorGridPositionLastFrame != CursorGridPosition)
        {
            previewParent.transform.position = CursorGridPosition;
        }
    }
    
    private bool CanAfford(BuildingAsset buildingAsset)
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

    public bool IsValidPlacement()
    {
        foreach (var preview in previewPrefabInstances)
        {
            if (preview.invalid) return false;
        }

        return true;
    }

    public void Build()
    {
        if (buildingMode && !EventSystem.current.IsPointerOverGameObject() && IsValidPlacement() && CanAfford(testBuilding))
        {
            Instantiate(testBuilding.buildingPrefab, CursorGridPosition + Vector3.forward,
                testBuilding.buildingPrefab.gameObject.transform.rotation);
            
            foreach (var cost in testBuilding.Costs)
            {
                ResourceManager.Instance.SpendResource(cost.ResourceType, cost.Amount);
            }
            
            AstarPath.active.Scan();

        }
    }

    public void CancelBuild()
    {
        buildingMode = false;
    }

    public void ToggleBuildMode()
    {
        buildingMode = !buildingMode;
    }
    
    public void ToggleBuildPanel()
    {
        buildMenu.gameObject.SetActive(!buildMenu.gameObject.activeSelf);
    }

    public void RenderUI()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            var element = Instantiate(UIPrefab, layoutGroup);
            element.SetData(buildings[i]);
        }
    }
}