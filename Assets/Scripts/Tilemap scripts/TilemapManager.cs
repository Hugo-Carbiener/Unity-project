using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{

    public static TilemapManager Instance { get; private set; }

    [SerializeField]
    private Tilemap groundTilemap;
    [SerializeField]
    private Tilemap buildingsTilemap;
    [SerializeField]
    private Tilemap waterTilemap;

    //[SerializeField]
    private List<TileData> tileDatas;

    public int columns;
    public int rows;
    private List<CellData> cells = new List<CellData>();

    public List<Tile> plainTiles;
    public List<Tile> forestTiles;
    public List<Tile> mountainTiles;
    public List<Tile> waterTiles;
    public List<Tile> buildingTiles;
    private List<Tile> tiles;
    private Tile forestTile;
    private Tile plainTile;
    private Tile mountainTile;

    public bool activateClustering;

    private List<Vector3Int> evenNeighborCoordinates = new List<Vector3Int>() { new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), };
    private List<Vector3Int> oddNeighborCoordinates = new List<Vector3Int>() { new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0), };

    private void Awake()
    {
        // create tile list
        tiles = new List<Tile>();
        foreach (Tile tile in plainTiles)
        {
            tiles.Add(tile);
        }
        foreach (Tile tile in forestTiles)
        {
            tiles.Add(tile);
        }
        foreach (Tile tile in mountainTiles)
        {
            tiles.Add(tile);
        }

        // initiate main tiles
        forestTile = forestTiles[0];
        plainTile = plainTiles[0];
        mountainTile = mountainTiles[0];

        // create instance of tilemapManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Debug.Log("You had to destroy a TilemapManager You fucked up");
        }

        // add offset to building tilemap
        Vector3 offset = new Vector3(0, buildingsTilemap.layoutGrid.cellSize.x / 4, 0);
        buildingsTilemap.transform.position += offset;
    }

    private void Start()
    {
        generateGroundTilemap(columns, rows);
        mergeTiles();
        //setRandomBuilding();
        //generateCastle();
        
        //-------------
        paintTilemap();
    }

    public void generateGroundTilemap(int columns, int rows)
    {
        // Start on a blank grid
        groundTilemap.ClearAllTiles();
        buildingsTilemap.ClearAllTiles();
        waterTilemap.ClearAllTiles();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3Int coordinates = new Vector3Int(x, y, 0);
                CellData cell = new CellData(coordinates);

                if (activateClustering)
                {
                    setTileToCellDependingOnNeighbor(cell);
                }
                else
                {
                    setCellAtRandom(cell);
                }
                cells.Add(cell);
            }
        }
    }

    public Tilemap getGroundTilemap() { return this.groundTilemap; }
    
    public int? getCell(Vector3Int coordinates)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].coordinates == coordinates)
            {
                return i;
            }
        }
        return null;
    }
    public void setCellAtRandom(CellData data)
    {
        int rd = Random.Range(0, 3);
        if (rd == 0)
        {
            setCellToPlain(data);
        }
        else if (rd == 1)
        {
            setCellToForest(data);
        }
        else
        {
            setCellToMountain(data);
        }
    }
    public void setCellToPlain(CellData data) {
        data.setEnvironment(environments.plain);
        data.setGroundTile(plainTile);
    }
    public void setCellToForest(CellData data) {
        data.setEnvironment(environments.forest);
        data.setGroundTile(forestTile);
    }
    public void setCellToMountain(CellData data) {
        data.setEnvironment(environments.mountain);
        data.setGroundTile(mountainTile);
    }

    public void setTileToCellDependingOnNeighbor(CellData data)
    {
        float plainNeighbors = 0;
        float forestNeighbors = 0;
        float mountainNeighbors = 0;

        // get neighbor coordinates depending on if the tile is even or odd
        List<Vector3Int> neighborCoordinates;

        if (data.coordinates.y % 2 == 0)
        {
            neighborCoordinates = evenNeighborCoordinates;
        }
        else
        {
            neighborCoordinates = oddNeighborCoordinates;
        }
        
        foreach (Vector3Int neighbor in neighborCoordinates)
        {   
           
            int? currentCellIndex = getCell(data.coordinates + neighbor);
            if (currentCellIndex != null)
            {
                if (cells[ (int) currentCellIndex].environment == environments.plain)
                {
                    plainNeighbors += 1;
                }
                else if (cells[ (int) currentCellIndex].environment == environments.forest)
                {
                    forestNeighbors += 1;
                }
                else if (cells[ (int) currentCellIndex].environment == environments.mountain)
                {
                    mountainNeighbors += 1;
                }
            }
        }
        float total = plainNeighbors + forestNeighbors + mountainNeighbors;
        float plainProba = (plainNeighbors + 1) / (total + 3);
        float forestProba = (forestNeighbors + 1) / (total + 3);
        float mountainProba = (mountainNeighbors + 1) / (total + 3);
        Debug.Log(plainNeighbors + ", " + forestNeighbors + ", " + mountainNeighbors);
        double random = Random.value;
        if (random < plainProba) {
            setCellToPlain(data);
        } else if (random >= plainProba && random < forestProba +plainProba)
        {
            setCellToForest(data);
        } else
        {
            setCellToMountain(data);
        }
    }

    public void mergeTiles()
    {
        string tileName;
        List<Vector3Int> neighborCoordinates;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3Int currentPosition = new Vector3Int(x, y, 0);
                int? currentCellIndex = getCell(currentPosition);
                environments currentCellEnvironment = cells[ (int) currentCellIndex].environment;
                tileName = currentCellEnvironment.ToString() + "tile";

                // get neighbor coordinates depending on if the tile is even or odd
                if ( y % 2 == 0)
                {
                    neighborCoordinates = evenNeighborCoordinates;
                } else
                {
                    neighborCoordinates = oddNeighborCoordinates;
                }

                for (int i = 0; i < 6; i++)
                {
                    int? neighborCellIndex = getCell(currentPosition + neighborCoordinates[i]);
                    if (neighborCellIndex != null)
                    {
                        environments neighborCellEnvironment = cells[ (int) neighborCellIndex].environment;
                        if (currentCellEnvironment == neighborCellEnvironment)
                        {
                            // if current tile and neighbor are from the same env, we add the current neighbor number (1 to 6) to the end of the tile name.
                            tileName += i + 1;
                            Debug.Log(tileName);
                        }
                    }
                }
                
            foreach(Tile tile in tiles)
            {
                //Debug.Log("merging : tile name is " + tile.name);
                if(tile.name == tileName)
                {
                    cells[ (int) currentCellIndex].groundTile = (TileBase) tile;
                }
            }
            }
        }
    }

    /*public void generateCastle()
    {
        Vector3Int center = new Vector3Int(rows/2, columns/2, 0);
        //groundTilemap.SetTile(center, null);
        int i = 0;
        foreach(Vector3Int neighbor in oddNeighborCoordinates)
        {
            CellData currentCell = getCell(center + neighbor);
            i += 1;
            currentCell.waterTile = waterTiles[0];
        }
    }*/


    /*public void setRandomBuilding()
    {
        for (int i = 0; i < 10; i++)
        {
            getCell(new Vector3Int(Random.Range(0, rows - 1), Random.Range(0, columns - 1), 0)).buildingTile = buildingTiles[0];
        }
    }*/

    public void paintTilemap()
    {
        foreach (CellData cell in cells)
        {
            if (cell.waterTile == null)
            {
                if (cell.buildingTile != null)
                {
                    buildingsTilemap.SetTile(cell.coordinates, cell.buildingTile);
                }
                if (cell.groundTile != null)
                {
                    groundTilemap.SetTile(cell.coordinates, cell.groundTile);
                }
            }
            else
            {
                waterTilemap.SetTile(cell.coordinates, cell.waterTile);
            }

        }
    }
    
}
