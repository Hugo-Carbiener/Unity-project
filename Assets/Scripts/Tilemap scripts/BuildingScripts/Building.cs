using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{
    private Vector3Int coordinates = Vector3Int.zero;
    private Vector3 worldCoordinates;
    private Tilemap buildingTilemap;

    private void Awake()
    {
        buildingTilemap = GameObject.Find("BuildingTilemap").GetComponent<Tilemap>();
        worldCoordinates = buildingTilemap.CellToWorld(coordinates);
    }

    public Vector3Int getCoordinates() { return this.coordinates; }
    public Vector3 getWorldCoordinates() { return this.worldCoordinates; }
    public void setCoordinates(Vector3Int cellCoordinates) {
        this.coordinates = cellCoordinates; 
        worldCoordinates = buildingTilemap.CellToWorld(cellCoordinates);
    } 
}
