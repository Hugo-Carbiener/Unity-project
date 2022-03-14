using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production : MonoBehaviour
{
    private ResourceManager resourceManager;
    [SerializeField] private ResourceType resource;
    [SerializeField] private int amount;
    [SerializeField] private int cooldown;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        InvokeRepeating("ProductionOrder", 5, cooldown);
    }

    private void ProductionOrder()
    {
        resourceManager.ModifyResources(resource, amount);
    }
}
