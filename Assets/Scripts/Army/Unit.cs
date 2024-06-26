using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * Basic component for game agents
 */
public class Unit : MonoBehaviour, ITargettable
{
    [Header("Modules")]
    [SerializeField] private BehaviorModule behaviorModule;
    [SerializeField] private MovementModule movementModule;
    [SerializeField] private FightModule fightModule;
    [Header("Consumption")]
    [SerializeField] private int foodAmount;

    private void Awake()
    {
        Assert.IsNotNull(behaviorModule);
        Assert.IsNotNull(movementModule);
        Assert.IsNotNull(fightModule);
    }

    public Vector2Int position
    {
        get { return this.position; }

        set
        {
            movementModule.currentCell = value;
        }
    }
    
    public void die()
    {
        gameObject.SetActive(false);
        ArmyManager.Instance.armySize--;
        ArmyManager.Instance.armyUnits.Remove(this);
    }

    public int getFoodConsummed() { return foodAmount; }

    public BehaviorModule GetBehaviorModule() { return behaviorModule; }   
    public MovementModule GetMovementModule() { return movementModule; }
    public FightModule GetFightModule() { return fightModule; }

    public Vector2Int GetPosition() { return position; }
}
