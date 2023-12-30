using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Pathfinder
{
    public static readonly int DEFAULT_NEIGHBOR_TRAVEL_WEIGHT = 1;
    private static List<PathfindingCellData> accessibleCells = new List<PathfindingCellData>();
    private static List<PathfindingCellData> visitedCells = new List<PathfindingCellData>();

    struct PathfindingCellData
    {
        public readonly Vector2Int coordinates;
        public readonly int currentMovementCost;
        public readonly int estimatedMovementCost;
        public readonly int movementProgression;

        public PathfindingCellData(CellData currentCell, CellData targetCell, int currentMovementCost)
        {
            coordinates = currentCell.coordinates;
            this.currentMovementCost = currentMovementCost;
            this.estimatedMovementCost = Utils.GetTileDistance(currentCell.coordinates, targetCell.coordinates);
            this.movementProgression = this.currentMovementCost + estimatedMovementCost;
        }
    }

    public static List<CellData> GetPath(Vector2Int from, Vector2Int to)
    {
        CellData start = TilemapManager.Instance.GetCellData(from);
        CellData destination = TilemapManager.Instance.GetCellData(to);
        if (start == null || destination == null)
        {
            Debug.LogError(string.Format($"Path ({from.x}, {from.y}) to ({to.x}, {to.y}) - Starting/Ending cell does not exist."));
            return new List<CellData>();
        }

        return GetPath(from, to);
    }

    public static List<CellData> GetPath(CellData from, CellData to)
    {
        if (from.coordinates == to.coordinates) return new List<CellData>();

        if (!CellIsProperDestination(from) || !CellIsProperDestination(to))
        {
            Debug.LogError(string.Format($"Path ({from.coordinates.x}, {from.coordinates.y}) to ({to.coordinates.x}, {to.coordinates.y}) - Starting/Ending cell is not a proper destination"));
            return new List<CellData>();
        }

        int currentMovementCost = 0;
        CellData currentCell = from;
        while (currentCell != to)
        {
            visitedCells.Add(new PathfindingCellData(currentCell, to, currentMovementCost));
            SetNeighborTilesAsAccessible(currentCell, to, currentMovementCost);
            CellData newCurrentCell = GetNewCurrentCell(currentCell);
            currentMovementCost += currentCell.GetNeighborTravelWeight(newCurrentCell.coordinates);

            if (newCurrentCell == to)
            {
                visitedCells.Add(new PathfindingCellData(currentCell, to, currentMovementCost));
            }
        }

        List<CellData> path = Backtrack(from, to);
        return path;
    }

    private static void SetNeighborTilesAsAccessible(CellData currentCell, CellData destination, int currentMovementCost)
    {
        List<Vector2Int> neighborOffsets = Utils.GetNeighborOffsetVectors(currentCell.coordinates);
        List<CellData> unvisitedNeighborCells = neighborOffsets.Select(coordinateOffset => TilemapManager.Instance.GetCellData(currentCell.coordinates + coordinateOffset))
                                                               .Where(cellData => cellData != null)
                                                               .Where(cellData => CellIsWalkable(cellData) || (CellIsProperDestination(cellData) && cellData == destination))
                                                               .Where(cellData => !accessibleCells.Exists(pfCellData => pfCellData.coordinates == cellData.coordinates))
                                                               .Where(cellData => !visitedCells.Exists(pfCellData => pfCellData.coordinates == cellData.coordinates))
                                                               .ToList();
            
        unvisitedNeighborCells.ForEach(neighborCellData => accessibleCells.Add(new PathfindingCellData(neighborCellData, destination, currentMovementCost + currentCell.GetNeighborTravelWeight(neighborCellData.coordinates))));
    }

    private static CellData GetNewCurrentCell(CellData currentCell)
    {
        return accessibleCells.OrderBy(pfCellData => pfCellData.movementProgression)
                              .ThenByDescending(pfCellData => Utils.CellsAreNeighbors(currentCell.coordinates, pfCellData.coordinates))
                              .Select(pfCellData => TilemapManager.Instance.GetCellData(pfCellData.coordinates))
                              .First(cellData => cellData != null);
    }

    private static List<CellData> Backtrack(CellData from, CellData to)
    {
        List<PathfindingCellData> backtrackedPath = new List<PathfindingCellData>();
        List<PathfindingCellData> reversedVisitedCells = visitedCells;
        reversedVisitedCells.Reverse();

        PathfindingCellData currentCell = reversedVisitedCells[0];
        backtrackedPath.Add(currentCell);
        while (!backtrackedPath.Contains(visitedCells[0]))
        {
            currentCell = reversedVisitedCells.OrderByDescending(pfCellData => Utils.CellsAreNeighbors(pfCellData.coordinates, currentCell.coordinates)).ThenBy(pfCellData => pfCellData.movementProgression < currentCell.movementProgression).First();
            reversedVisitedCells.Remove(currentCell);
            backtrackedPath.Add(currentCell);
        }

        List<CellData> path = backtrackedPath.Select(pfCellData => TilemapManager.Instance.GetCellData(pfCellData.coordinates))
                                             .Where(cellData => cellData != null)
                                             .Reverse()
                                             .ToList();
        if (path[0] != from)
        {
            Debug.LogError(string.Format($"Starting cell ({reversedVisitedCells[^1].coordinates.x}, {reversedVisitedCells[^1].coordinates.y}) is invalid because it does not correspond to the expected start ({from.coordinates.x}, {from.coordinates.y})."));
            return new List<CellData>();
        }

        if (path[^1] != to)
        {
            Debug.LogError(string.Format($"Destination cell ({reversedVisitedCells[0].coordinates.x}, {reversedVisitedCells[0].coordinates.y}) is invalid because it does not correspond to the expected destination ({to.coordinates.x}, {to.coordinates.y})."));
            return new List<CellData>();
        }

        return path;
    }

    public static bool CellIsWalkable(CellData cell)
    {
        return cell.environment != Environment.water && cell.building == null;
    }

    public static bool CellIsProperDestination(CellData cell)
    {
        return cell.environment != Environment.water;
    }
}
