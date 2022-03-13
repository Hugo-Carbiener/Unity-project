using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ResourceManager>();
            }

            return _instance;
        }
    }

    private int food;
    private int wood;
    private int stone;

    private void Start()
    {
        food = 0;
        wood = 0;
        stone = 0;
    }

    public void ModifyResources(ResourceType resource, int amount)
    {
        switch(resource)
        {
            case ResourceType.Food:
                food += amount;
                break;
            case ResourceType.Wood:
                wood += amount;
                break;
            case ResourceType.Stone:
                stone += amount;
                break;
        }
    }
}

