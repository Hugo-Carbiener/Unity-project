using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    protected BuildingFactory.BuildingType buildingType;
    protected int level;
    protected int capacity;
    protected Vector2 coordinates;

    public Building(Vector2 coordinates)
    {
        this.coordinates = coordinates;
        this.level = 1;
    }
}
