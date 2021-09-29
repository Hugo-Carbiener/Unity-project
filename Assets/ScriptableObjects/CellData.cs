using UnityEngine;
using UnityEngine.Tilemaps;
public class CellData : Object
{
    public Vector3Int coordinates;
    public environments environment;
    public TileBase groundTile = null;
    public TileBase buildingTile = null;
    public TileBase waterTile = null;

    public CellData(Vector3Int coordinates)
    {
        this.coordinates = coordinates;

    }
}
