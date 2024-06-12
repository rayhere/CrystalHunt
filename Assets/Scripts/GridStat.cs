using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridStat : MonoBehaviour
{
    public int x;
    public int y;
    public int z;

    private void Start()
    {
        // Assuming you set the initial grid position in the Inspector
        // x = 0;
        // y = 0;

        // Set the initial grid position (0,0) as empty and with checkoutTime = 0f
       /*  CoordinatesTable coordinatesTable = FindObjectOfType<CoordinatesTable>();
        if (coordinatesTable != null && coordinatesTable.IsWithinBounds(x, y))
        {
            CoordinatesTable.GridUnit gridUnit = coordinatesTable.GetGridUnit(x, y);
            gridUnit.isEmpty = true;
            gridUnit.objName = "";
            gridUnit.checkoutTime = Time.deltaTime;
        } */

        /* CoordinatePlane coordinatePlane = FindObjectOfType<CoordinatePlane>();
        if (coordinatePlane != null && coordinatePlane.IsWithinBounds(x, y))
        {
            CoordinatePlane.GridUnit gridUnit = CoordinatePlane.GetGridUnit(x, y);
            gridUnit.isEmpty = true;
            gridUnit.objName = "";
            gridUnit.checkoutTime = Time.deltaTime;
        } */
    }
}

