using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutFit : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    private RectTransform gridRect;
    [SerializeField, Space(5),Range(-2,1)] float spacingProportion = 0;

    private void Awake()
    {
        if (!gridLayoutGroup) gridLayoutGroup = GetComponent<GridLayoutGroup>();
        gridRect = (RectTransform)gridLayoutGroup.transform;
    }

    void Update()
    {
        if(!gridLayoutGroup) return;
        AdjustGridSpacing();
    }
    
    private void AdjustGridSpacing()
    {
        Vector2 totalSpace = gridRect.rect.size;
        totalSpace.x -= gridLayoutGroup.padding.right + gridLayoutGroup.padding.left;
        totalSpace.y -= gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;
        float spacePerCell = (totalSpace.x / gridLayoutGroup.constraintCount);
        Vector2 cellSpacing = Vector2.one * (spacePerCell * spacingProportion) * 2;
        Vector2 cellSize = Vector2.one * spacePerCell - cellSpacing;
        cellSize.y = (totalSpace.y / gridLayoutGroup.constraintCount) - cellSpacing.y;

        gridLayoutGroup.cellSize = cellSize;
        gridLayoutGroup.spacing = cellSpacing;
    }
}
