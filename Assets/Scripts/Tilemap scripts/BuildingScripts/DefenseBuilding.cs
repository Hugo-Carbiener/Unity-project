using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseBuilding : Building
{
    public DefenseBuilding(Vector2 coordinates) : base(coordinates)
    {
        buildingType = BuildingFactory.BuildingType.defense;
    }
}
