using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance { get; private set; }

    public Grid grid { get; private set; }
    public Tilemap groundTilemap { get; private set; }
    public Tilemap buildingsTilemap { get; private set; }
    public Tilemap waterTilemap { get; private set; }
    public Tilemap selectionTilemap { get; private set; }

    [SerializeField] private int columns;
    [SerializeField] private int rows;
    private List<CellData> cells = new List<CellData>();

    private bool displaySelection = false;
    // true when the value of selectedCell just changed
    private CellData selectedCell;

    public bool activateClustering;
    public bool activateIsolatedCellsRemoval;

    private List<Vector2Int> evenNeighborCoordinates = new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), };
    private List<Vector2Int> oddNeighborCoordinates = new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1), };

    private void Awake()
    {
        // define tilemaps
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        groundTilemap = grid.transform.Find("GroundTilemap").GetComponent<Tilemap>();
        buildingsTilemap = grid.transform.Find("BuildingTilemap").GetComponent<Tilemap>();
        waterTilemap = grid.transform.Find("WaterTilemap").GetComponent<Tilemap>();
        selectionTilemap = grid.transform.Find("SelectionTilemap").GetComponent<Tilemap>();

        // event to refresh tiles
        BuildingFactory.Instance.updateBuildingTilemapEvent += DispatchBuildingTilemap;

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
        if (activateIsolatedCellsRemoval)
        {
        removeIsolatedCells();
        }
        //generateCastle();

        // --------------
        initialPaintTilemap();
    }

    private void Update()
    {
        DispatchSelectionTilemap();
    }

    public int? getCell(Vector2Int coordinates)
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

    public CellData getCellData(Vector2Int coordinates)
    {
        foreach (CellData data in cells)
        {
            if (data.coordinates == coordinates)
            {
                return data;
            }
        }
        return null;
    }

    public int getTilemapColumns() { return columns; }
    public int getTilemapRows() { return rows; }
    public bool selectionIsDisplayed() { return displaySelection; }
    public CellData getSelectedCellData() { return selectedCell; }

    public void generateGroundTilemap(int columns, int rows)
    {
        // Start on a blank grid
        groundTilemap.ClearAllTiles();
        buildingsTilemap.ClearAllTiles();
        waterTilemap.ClearAllTiles();
        selectionTilemap.ClearAllTiles();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
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
    public void SelectCell(Vector2Int coordinates)
    {
        displaySelection = true;
        int? cellIndex = getCell(coordinates);
        selectedCell = cells[(int)cellIndex];
    }

    public void reSelectCell ()
    {
        displaySelection = !displaySelection;
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
        data.environment = environments.plain;
        data.groundTile = GameAssets.i.plainTile;
    }
    public void setCellToForest(CellData data) {
        data.environment = environments.forest;
        data.groundTile = GameAssets.i.forestTile;
    }
    public void setCellToMountain(CellData data) {
        data.environment = environments.mountain;
        data.groundTile = GameAssets.i.mountainTile;
    }

    public void setTileToCellDependingOnNeighbor(CellData data)
    {
        float plainNeighbors = 0;
        float forestNeighbors = 0;
        float mountainNeighbors = 0;

        // get neighbor coordinates depending on if the tile is even or odd
        List<Vector2Int> neighborCoordinates;

        if (data.coordinates.y % 2 == 0)
        {
            neighborCoordinates = evenNeighborCoordinates;
        }
        else
        {
            neighborCoordinates = oddNeighborCoordinates;
        }
        
        foreach (Vector2Int neighbor in neighborCoordinates)
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

    public void removeIsolatedCells()
    {
        List<Vector2Int> neighborCoordinates;

        // iterate on each cells
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // for each cell, create a new dictionnary that states how many neighbors of each environement
                Dictionary<environments, int> neighborAmount = new Dictionary<environments, int>();
                foreach (environments environment in environments.GetValues(typeof(environments)))
                {
                    // initiate the dictionnary with every environment
                    neighborAmount.Add(environment, 0);
                }


                Vector2Int currentCellCoordinates = new Vector2Int(x, y);
                int? currentCellIndex = getCell(currentCellCoordinates);
                CellData currentCell = cells[(int)currentCellIndex];
                environments currentCellEnvironment = cells[(int)currentCellIndex].environment;

                // get neighbor coordinates depending on if the tile is even or odd
                if (y % 2 == 0)
                {
                    neighborCoordinates = evenNeighborCoordinates;
                }
                else
                {
                    neighborCoordinates = oddNeighborCoordinates;
                }

                foreach (Vector2Int coordinates in neighborCoordinates)
                {
                    int? neighborCellIndex = getCell(currentCellCoordinates + coordinates);
                    if (neighborCellIndex != null)
                    {
                    environments neighborCellEnvironment = cells[(int)neighborCellIndex].environment;
                    neighborAmount[neighborCellEnvironment] += 1;
                    }
                }
                if (neighborAmount[currentCellEnvironment] == 0) // cell is isolated we need to replace it by the environement that is the most present around the cell
                {
                    // get the key of the max value i.e. get the environment that is the most present around the cell
                    KeyValuePair<environments, int> max = new KeyValuePair<environments, int>();
                    foreach (KeyValuePair<environments, int> entry in neighborAmount)
                    {
                        if (entry.Value > max.Value)
                        {
                            max = entry;
                        }
                    }

                    // replace the initial environment by the most present one
                    if (max.Key == environments.plain)
                    {
                        setCellToPlain(currentCell);
                    }
                    else if (max.Key == environments.forest)
                    {
                        setCellToForest(currentCell);
                    }
                    else if (max.Key == environments.mountain)
                    {
                        setCellToMountain(currentCell);
                    }
                }
            }
        }
    }

    public void generateCastle()
    {
        Vector2Int center = new Vector2Int(rows/2, columns/2);
        int? centerCellIndex = getCell(center);
        CellData centerCell = cells[(int)centerCellIndex];
        //centerCell.waterTile = GameAssets.i.waterTiles[7];
        List<Vector2Int> neighborCoordinates;
        int i = 0;
        

        if (center.y % 2 == 0)
        {
            neighborCoordinates = evenNeighborCoordinates;
        }
        else
        {
            neighborCoordinates = oddNeighborCoordinates;
        }

        foreach (Vector2Int neighbor in neighborCoordinates)
        {
            int? currentCellIndex = getCell(center + neighbor);
            CellData currentCell = cells[(int)currentCellIndex];
            i += 1;
            //currentCell.waterTile = GameAssets.i.waterTiles[i];
        }
    }

    public void initialPaintTilemap()
    // if a water tile is referred to in the CellData it will be painted and the ground/building tile ignored
    {
        foreach (CellData cell in cells)
        {
            if (cell.waterTile == null)
            {
                if (cell.buildingTile != null)
                {
                    buildingsTilemap.SetTile(cell.GetVector3Coordinates(), cell.buildingTile);
                }
                if (cell.groundTile != null)
                {
                    groundTilemap.SetTile(cell.GetVector3Coordinates(), cell.groundTile);
                }
            }
            else
            {
                waterTilemap.SetTile(cell.GetVector3Coordinates(), cell.waterTile);
            }
        }
    }

    public void DispatchSelectionTilemap()
    {
        selectionTilemap.ClearAllTiles();
        if(displaySelection)
        {
            selectionTilemap.SetTile(selectedCell.GetVector3Coordinates(), GameAssets.i.selectionTile); 
        }
    }

    private void DispatchBuildingTilemap()
    {
        foreach (CellData cell in cells)
        {
            if (cell.buildingTile)
            {
                buildingsTilemap.SetTile(cell.GetVector3Coordinates(), cell.buildingTile);
            }
        }
    }

    public void DispatchTile(Vector2Int coordinates)
    {
        CellData data = getCellData(coordinates);
        groundTilemap.SetTile(data.GetVector3Coordinates(), data.groundTile);
        buildingsTilemap.SetTile(data.GetVector3Coordinates(), data.buildingTile);
    }
}