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

    [Header("Tile Sprites")]
    public List<Tile> plainTiles;
    public List<Tile> forestTiles;
    public List<Tile> mountainTiles;
    public List<Tile> waterTiles;
    public List<Tile> buildingTiles;
    public TileBase selectionTile;
    public Tile forestTile;
    public Tile plainTile;
    public Tile mountainTile;

    [Header("Sprites")]
    public Sprite plainTileSprite;
    public Sprite forestTileSprite;
    public Sprite mountainTileSprite;
    public Sprite foodIcon;
    public Sprite woodIcon;
    public Sprite stoneIcon;

    [Header("Buildings")]
    public GameObject sawmill;
    public GameObject windmill;
}

