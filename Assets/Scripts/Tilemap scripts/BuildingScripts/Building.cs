using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    protected Vector3Int coordinates = Vector3Int.zero;

    public Vector3Int getCoordinates() { return this.coordinates; }
    public void setCoordinates(Vector3Int coordinates) { this.coordinates = coordinates; }

}
