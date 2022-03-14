using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuOptions : MonoBehaviour
{
    [SerializeField] private BuildingFactory.buildings option0;
    [SerializeField] private bool isActive0;
    [SerializeField] private BuildingFactory.buildings option1;
    [SerializeField] private bool isActive1;
    [SerializeField] private BuildingFactory.buildings option2;
    [SerializeField] private bool isActive2;
    [SerializeField] private BuildingFactory.buildings option3;
    [SerializeField] private bool isActive3;
    private BuildingFactory factory;

    private void Start()
    {
        factory = BuildingFactory.Instance;
    }

    public void Option0()
    {
        if (isActive0)
        {
            factory.build(option0);
        }
    }

    public void Option1()
    {
        if (isActive2)
        {
            factory.build(option1);
        }
    }

    public void Option2()
    {
        if (isActive2)
        {
            factory.build(option2);
        }
    }

    public void Option3()
    {
        if (isActive3) 
        {
            factory.build(option3);
        }
    }
}
