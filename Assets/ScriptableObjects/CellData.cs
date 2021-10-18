using UnityEngine;
using UnityEngine.Tilemaps;
public class CellData : Object
{
    public Vector3Int coordinates;
    public environments environment;
    public TileBase groundTile = null;
    public TileBase buildingTile = null;
    public TileBase waterTile = null;
    public bool isSelected = false;

    public CellData(Vector3Int coordinates)
    {
        this.coordinates = coordinates;
    }

    public void setEnvironment(environments environment) { this.environment = environment; }
    public void setGroundTile(TileBase tile) { this.groundTile = tile; }
    public void setBuildingTile(TileBase tile) { this.buildingTile = tile; }
    public void setEnvironment(TileBase tile) { this.waterTile = tile; }
}
