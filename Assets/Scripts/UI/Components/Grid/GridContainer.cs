using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridContainer : MonoBehaviour
{
    [SerializeField] private Sprite diamondSprite, bombSprite;

    private readonly Color _grayViewColor = new(255, 255, 255, .5f);
    private readonly Color _highlightColor = Color.white;

    public void SetGrid(List<int> tiles, List<int> clickedTiles)
    {
        ClearGrid();
        var cellAsset = Resources.LoadAsync<Image>("CellHistory").asset as Image;
        for (var i = 0; i < tiles.Count; i++)
        {
            var cell = Instantiate(cellAsset, transform);
            cell.sprite = tiles[i] == 1 ? diamondSprite : bombSprite;
            cell.color = clickedTiles.Contains(i) ? _highlightColor : _grayViewColor;
        }
    }

    public void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
