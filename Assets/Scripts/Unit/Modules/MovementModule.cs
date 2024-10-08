using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

/**
 * Unit module in charge of executing movement after being given a destination cell.
 */
public class MovementModule : MonoBehaviour
{
    // speed in seconds per cell
    [SerializeField] private float timeToCrossCell;
    public Vector2Int destination { get; private set; }
    public Vector2Int currentCell { get; set; }
    public List<CellData> path { get; private set; }
    public Status status { get; private set; } = Status.ToBeProgrammed;

    public UnityEvent<Vector2Int> OnArrivalEvent { get; private set; } = new UnityEvent<Vector2Int>();

    private void Start()
    {
        destination = currentCell;
    }

    public void SetDestination(Vector2Int destination)
    {
        this.destination = destination;
        status = Status.Pending;
    }

    /**
     * Acts as a teleportation and cancels the current movement if there is one
     */
    public void setCurrentCell(Vector2Int position)
    {
        currentCell = position;
        if (status != Status.Done || status != Status.ToBeProgrammed)
        {
            destination = currentCell;
            path = null;
            status = Status.ToBeProgrammed;
        }
    }
    
    /**
     * Triggers the start of the programmed movement if a valid one is programmed.
     */
    public void StartMovement()
    {
        if (destination == currentCell || !Utils.CellCoordinatesAreValid(destination)) return;

        if (status != Status.Pending)
        {
            Debug.LogError("Unit " + gameObject.name + " : Movement was started while in status " + status);
            return;
        }

        status = Status.InProgress;
        path = Pathfinder.GetPath(currentCell, destination);
        if (path == null) return;

        StartCoroutine("MovementLoop");
    }

    /**
     * Graceful terminaison of the movement procedure.
     */
    private void EndMovement()
    {
        if (currentCell != path[^1].coordinates && currentCell != destination)
        {
            Debug.LogError("Unit " + gameObject.name + " : Movement ended on wrong cell : end of path " + path[^1].coordinates + " | current cell " + currentCell);
            return;
        }

        path = null;
        status = Status.Done;
        OnArrivalEvent.Invoke(destination);
    }

    /**
     * Disgraceful terminaison of the movement procedure. Changes status and does not trigger the arrival event.
     */
    public void CancelMovement()
    {
        StopAllCoroutines();
        status = Status.Cancelled;

        path = null;
        destination = currentCell;
    }

    private IEnumerator MovementLoop()
    {
        int currentCellIndex = 0;
        Vector2Int targetCell = path[currentCellIndex + 1].coordinates;

        while (currentCell != destination)
        {
            float timer = 0;
            while (timer < timeToCrossCell)
            {
                timer += Time.deltaTime;
                Vector3 currentCellPosition = TilemapManager.Instance.grid.CellToWorld((Vector3Int) currentCell);
                Vector3 targetCellPosition = TilemapManager.Instance.grid.CellToWorld((Vector3Int) targetCell);

                // orient sprite according to movement
                if ((targetCellPosition - currentCellPosition).x > 0)
                {
                    transform.localScale = new Vector3(-1, 1, 0);
                } else
                {
                    transform.localScale = new Vector3(1, 1, 0);
                }

                transform.position = Vector3.Lerp(currentCellPosition, targetCellPosition, timer / timeToCrossCell);
                yield return null;
            }
            currentCellIndex++;
            currentCell = targetCell;
            if (currentCellIndex + 1 < path.Count)
            {
                targetCell = path[currentCellIndex + 1].coordinates;
            }
        }
        EndMovement();
    }
}
