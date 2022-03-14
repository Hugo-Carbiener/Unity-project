using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private GameObject resourceCanvas;
    private int food;
    private int wood;
    private int stone;
    private Text foodText;
    private Text woodText;
    private Text stoneText;

    private void Start()
    {
        food = 0;
        wood = 0;
        stone = 0;
        Text[] texts = resourceCanvas.GetComponentsInChildren<Text>();
        foodText = texts[0];
        woodText = texts[1];
        stoneText = texts[2];
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
        UpdateUI();
    }

    private void UpdateUI()
    {
        foodText.text = food.ToString();
        woodText.text = wood.ToString();
        stoneText.text = stone.ToString();
    }
}

