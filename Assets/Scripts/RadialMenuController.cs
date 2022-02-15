using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuController : MonoBehaviour
{
    public GameObject theMenu;

    public GameObject objRed, objBlue, objGreen, objYellow;

    public Vector2 moveInput;

    public Text[] options;

    public Color normalColor, highlightColor;

    public int selectedOption;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            theMenu.SetActive(true);
        }

        if (theMenu.activeInHierarchy)
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


                Debug.Log(angle);
                for (int i = 0; i < options.Length; i++)
                {

                    if(angle > i * 72 && angle < (i + 1) * 72)
                    {
                        options[i].color = highlightColor;
                        selectedOption = i;
                    } else
                    {
                        options[i].color = normalColor;
                    }
                }
            }

            if(Input.GetMouseButtonDown(0))
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
                theMenu.SetActive(false);
            }
        }
        

        if(Input.GetKeyDown(KeyCode.Q))
        {
            SwitchRed();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SwitchBlue();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchGreen();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchYellow();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchAll();
        }
    }

    public void SwitchRed()
    {
        Debug.Log("Red");
        objRed.SetActive(!objRed.activeInHierarchy);
    }

    public void SwitchBlue()
    {
        objBlue.SetActive(!objBlue.activeInHierarchy);
    }

    public void SwitchGreen()
    {
        objGreen.SetActive(!objGreen.activeInHierarchy);
    }

    public void SwitchYellow()
    {
        objYellow.SetActive(!objYellow.activeInHierarchy);
    }

    public void SwitchAll()
    {
        SwitchRed();
        SwitchBlue();
        SwitchGreen();
        SwitchYellow();
    }
}
