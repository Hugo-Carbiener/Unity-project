using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using System;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject[] imageContainer;
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private environments env;
    private Vector3Int fixedPosition;
    private Vector2 moveInput;
    private int selectedOption;
    private bool menuIsOpened;
    private Camera cam;
    

    // Start is called before the first frame update
    void Start()
    {
        if(!menu) { menu = GameObject.Find("Radial Menu") as GameObject; }
        CloseMenu();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            menu.SetActive(true);
        }

        if (menu.activeInHierarchy)
        {
            moveInput.x = Input.mousePosition.x - (Screen.width / 2f);
            moveInput.y = Input.mousePosition.y - (Screen.height / 2f);

            moveInput.Normalize();


            if (moveInput != Vector2.zero)
            {
                float angle = Mathf.Atan2(moveInput.y, -moveInput.x) / Mathf.PI * 180;
                angle += 90;
                if (angle < 0)
                {
                    angle += 360;
                }

                for (int i = 0; i < imageContainer.Length; i++)
                {

                    if (angle > i * 72 && angle < (i + 1) * 72)
                    {
                        selectedOption = i;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                switch (selectedOption)
                {
                    case 0:

                        break;

                    case 1:

                        break;
                    case 2:

                        break;
                    case 3:

                        break;
                    case 4:

                        break;
                }
                CloseMenu();
            }
        }
    }

    private void FixedUpdate()
    {
        if (menu.activeInHierarchy)
        {
            UpdateScale();
            transform.position = selectionTilemap.CellToWorld(fixedPosition);
        }
    }

    public void OpenMenu(Vector3Int pos)
    {
        Debug.Log("Menu opened");
        fixedPosition = pos;
        transform.position = selectionTilemap.CellToWorld(fixedPosition);
        menu.SetActive(true);
        menuIsOpened = true;
    }

    public void CloseMenu()
    {
        Debug.Log("Menu closed");
        menu.SetActive(false);
        menuIsOpened = false;
    }

    private void UpdateScale()
    {
        float factor = cam.orthographicSize * 0.032f + 0.263f;
        transform.localScale = Vector3.one * factor;
    }

    public bool MenuIsOpened() { return this.menuIsOpened; }
}
