using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterBehavior : BehaviorModule, ITaskAutoGeneration
{
    private IFightable target;
    private FightModule fightModule;

    private void Awake()
    {
        fightModule = unit.GetFightModule();
    }

    private void Start()
    {
        DayNightCycleManager.OnCyclePhaseEnd.AddListener(Flee);
    }

    private void Update()
    {
        UpdateMovementDestination(assignedTask);
    }

    protected override void InitAction(Vector2Int targetCell)
    {
        if (!fightModule) return;

        ExecuteAction(targetCell);
    }

    protected override void ExecuteAction(Vector2Int targetCell)
    {
        if (fightModule.Attack(targetCell))
        {
            TilemapManager.Instance.GetCellData(targetCell).fight.OnFightEndEvent.AddListener(EndTask);
        }
        else
        {
            EndTask();
        }
    }

    public Task GenerateTask(Unit unit)
    {
        target = GetTarget(unit.GetMovementModule());
        if (target == null) return null;

        Task existingTask = TaskManager.Instance.GetTask(target.GetPosition(), TaskType.MonsterAttack);
        return existingTask != null ? existingTask : new Task(target.GetPosition(), TaskManager.INFINITE_CAPACITY, TaskType.MonsterAttack);
    }

    private IFightable GetTarget(MovementModule movementModule)
    {
        IFightable closestTarget = null;
        int distanceToTarget = TilemapManager.Instance.GetTilemapColumns() * TilemapManager.Instance.GetTilemapRows();

        List<IFightable> buildingTargets = BuildingFactory.Instance.buildingsConstructed
            .Where(target => target.GetFightModule() != null)
            .Where(building => building.IsValidTargetForFight(fightModule.GetFaction()))
            .Select(building => (IFightable) building)
            .ToList();
        List <IFightable> unitTargets = UnitManager.Instance.GetAllActiveUnits()
            .Where(target => target.GetFightModule() != null)
            .Where(unit => unit.IsValidTargetForFight(fightModule.GetFaction()))
            .Select(unit => (IFightable)unit)
            .ToList();
        List<IFightable> targets = Enumerable.Concat<IFightable>(buildingTargets, unitTargets).ToList();
         
        foreach (IFightable currentTarget in targets)
        {
            if (currentTarget == null || !currentTarget.GetFightModule().IsAttackable()) continue;

            int distanceToThisTarget = Utils.GetTileDistance(movementModule.currentCell, currentTarget.GetPosition());
            if (distanceToThisTarget >= distanceToTarget) continue;

            distanceToTarget = distanceToThisTarget;
            closestTarget = currentTarget;
        }
        return closestTarget;
    }

    private void Flee(DayNightCyclePhases phase)
    {
        assignedTask == new Task()
    }

    private void UpdateMovementDestination(Task task)
    {
        if (task == null || target == null || task.location == target.GetPosition()) return;
        
        unit.GetMovementModule().CancelMovement();
        task.location = target.GetPosition();

        if (InitMovement(task))
        {
            ExecuteMovement(task);
        }
        else
        {
            EndTask();
        }   
    }

    public override bool GeneratesOwnTasks() { return true; }
}
