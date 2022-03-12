using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuOptions : MonoBehaviour
{
    [SerializeField] private GameObject option0Building;
    [SerializeField] private GameObject option1Building;
    [SerializeField] private GameObject option2Building;
    [SerializeField] private GameObject option3Building;
    public void Option0()
    {
        Debug.Log("Select option 0");
    }

    public void Option1()
    {
        Debug.Log("Select option 1");
    }

    public void Option2()
    {
        Debug.Log("Select option 2");
    }

    public void Option3()
    {
        Debug.Log("Select option 3");
    }
}
