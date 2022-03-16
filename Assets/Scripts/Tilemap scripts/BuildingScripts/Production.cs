using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production : MonoBehaviour
{
    private ResourceManager resourceManager;
    private Building building;
    [SerializeField] private ResourceType resource;
    [SerializeField] private int amount;
    [SerializeField] private int cooldown;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        building = gameObject.GetComponent<Building>();
        InvokeRepeating("ProductionOrder", 5, cooldown);
    }

    private void ProductionOrder()
    {
        GainPopUp.Create(building.getWorldCoordinates(), amount, resource);
        resourceManager.ModifyResources(resource, amount);
    }
}
