using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    private GameObject child;

    [SerializeField]
    private TilemapManager tilemapManager;
    private CellData cellData;
    private CellData cellDataCheck;
    private bool selectionIsDisplayed;
    private bool boolCheck;

    [Header("Sprites")]
    public Sprite plainTileSprite;
    public Sprite forestTileSprite;
    public Sprite mountainTileSprite;

    private void Start()
    {
        child = gameObject.transform.GetChild(0).gameObject;
        child.SetActive(false);
        tilemapManager = TilemapManager.Instance;

        selectionIsDisplayed = tilemapManager.selectionIsDisplayed();
        boolCheck = selectionIsDisplayed;

        cellData = tilemapManager.getSelectedCellData();
        cellDataCheck = cellData;

    }

    private void Update()
    {
        if (tilemapManager.selectionIsUpdated())
        {
            updateData();
        } 

        // enable the selection box if a cell is selected, otherwise disables it
        selectionIsDisplayed = tilemapManager.selectionIsDisplayed();
        if (selectionIsDisplayed != boolCheck)
        // check if a new cell is selected or if the previous one was unselected
        {

            if (tilemapManager.selectionIsDisplayed())
            {
                child.SetActive(true);
            }
            else
            {
                child.SetActive(false);
            }

            boolCheck = selectionIsDisplayed;
            // reset checker
        }
    }

    private void updateData()
    {
        cellData = tilemapManager.getSelectedCellData();
        Text[] textObjects = gameObject.GetComponentsInChildren<Text>(true);
        Image[] img = gameObject.GetComponentsInChildren<Image>(true);
        // bool in GetComponent required to get inactive component
        textObjects[1].text = "Environment: " + cellData.environment.ToString();
        textObjects[2].text = "Coordinates: " + cellData.coordinates.x + ", " + cellData.coordinates.y;
        textObjects[3].text = "Building has not been implemented yet";

        if (cellData.environment == environments.plain)
        {
            img[1].sprite = plainTileSprite;
        } else if (cellData.environment == environments.forest)
        {
            img[1].sprite = forestTileSprite;
        } else
        {
            img[1].sprite = mountainTileSprite;
        }
    }
}
