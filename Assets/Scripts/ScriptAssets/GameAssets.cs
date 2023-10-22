#pragma warning disable S1104 // Fields should not have public accessibility
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;

    public static GameAssets i
    {
        get
        {
            if (!instance) {
                instance = (Resources.Load("GameAssets") as GameObject).GetComponent<GameAssets>();
            } 
            return instance;
        }
    }

    [Header("Tiles")]
    public Tile plainTile;
    public Tile forestTile;
    public Tile mountainTile;
    public Tile waterTile;
    public Tile cityTile;
    public List<Tile> buildingTiles;
    public TileBase selectionTile;

    [Header("Sprites")]
    [Header("Ground Sprites")]
    public Sprite plainTileSprite;
    public Sprite forestTileSprite;
    public Sprite mountainTileSprite;

    [Header("Resources Sprites")]
    public Sprite foodIcon;
    public Sprite woodIcon;
    public Sprite stoneIcon;

    [Header("Building Sprites")]
    public Sprite sawmill;
    public Sprite farm;

}

