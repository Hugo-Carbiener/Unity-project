using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{

    public ProductionBuilding(Vector2 coordinates, ResourceType resourceGenerated) : base(coordinates)
    {
        this.buildingType = BuildingFactory.BuildingType.production;
    }
}