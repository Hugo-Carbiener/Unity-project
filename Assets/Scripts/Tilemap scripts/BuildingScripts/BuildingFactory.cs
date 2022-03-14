using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingFactory : MonoBehaviour
{
    private static BuildingFactory instance;

    public static BuildingFactory i
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<BuildingFactory>();
            }
            return instance;
        }
    }

    public enum buildings
    {
        sawmill,
        windmill
    }
    private Dictionary<buildings, GameObject> buildingDictionnary = new Dictionary<buildings, GameObject>();
    private TilemapManager tilemapManager;

    private void Start()
    {
        tilemapManager = TilemapManager.Instance;
        buildingDictionnary.Add(buildings.sawmill, GameAssets.i.sawmill);
        buildingDictionnary.Add(buildings.windmill, GameAssets.i.windmill);
    }

    public void build(buildings buildingType)
    {
        // set the buildings values in the selected cell data and the coordinates in the building data
        GameObject building = buildingDictionnary[buildingType];
        CellData selectedCell = tilemapManager.getSelectedCellData();
        building = Instantiate(building);
        building.GetComponent<Building>().setCoordinates(selectedCell.coordinates);
        foreach (Tile tile in GameAssets.i.buildingTiles)
        {
            if (tile.name == building.ToString())
            {
                selectedCell.setBuildingTile(tile);
            } else
            {
                Debug.LogError("Building tile could not be found");
            }
        }        
    }
}
