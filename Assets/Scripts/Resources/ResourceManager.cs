using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private GameObject resourceValuesContainer;
    [SerializeField] private GameObject resourceProductionsContainer;
    private Dictionary<ResourceTypes, int> resources;
    private Dictionary<ResourceTypes, int> resourceProductions;
    private Text foodText;
    private Text woodText;
    private Text stoneText; 
    private Text foodProdText;
    private Text woodProdText;
    private Text stoneProdText;

    private void Awake()
    {
        resources = new Dictionary<ResourceTypes, int>();
        resourceProductions = new Dictionary<ResourceTypes, int>();
        foreach (ResourceTypes resource in ResourceTypes.GetValues(typeof(ResourceTypes)))
        {
            resources.Add(resource, 50);
            resourceProductions.Add(resource, 0);
        }
        // resources[ResourceTypes.Food] = 0;

        Text[] texts = resourceValuesContainer.GetComponentsInChildren<Text>();
        foodText = texts[0];
        woodText = texts[1];
        stoneText = texts[2];
        Text[] prodTexts = resourceProductionsContainer.GetComponentsInChildren<Text>();
        foodProdText = prodTexts[0];
        woodProdText = prodTexts[1];
        stoneProdText = prodTexts[2];
    }

    private void Start()
    {
        UpdateUI();
    }

    public int getResource(ResourceTypes resourceType) { return resources[resourceType]; }

    /**
     * Return whether the given amount of resource is available
     */
    public bool resourceIsAvailable(ResourceTypes resourceType, int amount)
    {
        return resources[resourceType] >= amount;
    }

    /**
     * Add the amount of given resources. Amount can be negative 
     */
    public void modifyResources(ResourceTypes resourceType, int amount)
    {
        int resourceAmount;
        if (resources.TryGetValue(resourceType, out resourceAmount))
        {
            resources[resourceType] = resourceAmount + amount;
        }
        UpdateValues();
    }

    /**
     * Compute the production rates from the list of buildings stored in BuildingFactory
     */
    private void computeProductions()
    {
        List<Building> buildings = BuildingFactory.Instance.buildingsConstructed;

        // reset prod values
        foreach (var resourceType in resourceProductions.Keys.ToList())
        {
            resourceProductions[resourceType] = 0;
        }

        // compute new values
        foreach (Building building in buildings)
        {
            ProductionModule productionModule;
            if (TryGetComponent(out productionModule))
            {
                ResourceTypes resourceType = productionModule.getResource();
                resourceProductions[resourceType] += productionModule.getAmount();
            }
        }
    }

    private void UpdateValues()
    {
        foodText.text = resources[ResourceTypes.Food].ToString();
        woodText.text = resources[ResourceTypes.Wood].ToString();
        stoneText.text = resources[ResourceTypes.Stone].ToString();
    }

    public void UpdateProductions()
    {
        computeProductions();
        foodProdText.text = resourceProductions[ResourceTypes.Food].ToString();
        woodProdText.text = resourceProductions[ResourceTypes.Wood].ToString();
        stoneProdText.text = resourceProductions[ResourceTypes.Stone].ToString();
    }

    private void UpdateUI()
    {
        UpdateValues();
        UpdateProductions();
    }
}

