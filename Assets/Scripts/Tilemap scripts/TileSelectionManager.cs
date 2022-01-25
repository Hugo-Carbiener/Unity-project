using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelectionManager : MonoBehaviour
{
    Vector3Int oldFrameSelect = new Vector3Int(0, 0, 0);
    Vector3Int frameSelect = new Vector3Int(0, 0, 0);

    TilemapManager tilemapManager;
    Tilemap selectionTilemap;
    
    private void Start()
    {
        tilemapManager = TilemapManager.Instance;
        selectionTilemap = tilemapManager.getSelectionTilemap();
        InputManager.onSelectInput += updateFrameSelect;
    }

    private void updateFrameSelect(Vector2 mousePosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int tilePos = selectionTilemap.WorldToCell(worldPos);
        oldFrameSelect = frameSelect;
        frameSelect = tilePos;

        if (frameSelect == oldFrameSelect)
        {
            tilemapManager.reSelectCell();
        } else
        {
            // generate newly selected cell coordinates
            tilemapManager.SelectCell(tilePos);
        }
    }
}
