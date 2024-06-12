using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public CoordinatesTable coordinatesTable;
    public GridStat gridStat;

    private void Start()
    {
/*         if (coordinatesTable != null && gridStat != null)
        {
            // Read grid position from GridStat script
            int currentX = gridStat.x;
            int currentY = gridStat.y;

            // Check if the target grid position is within bounds
            if (currentX == 0 && currentY == 1 && coordinatesTable.IsWithinBounds(currentX, currentY))
            {
                CoordinatesTable.GridUnit gridUnit = coordinatesTable.GetGridUnit(currentX, currentY);

                if (gridUnit.isEmpty && gridUnit.checkoutTime == 0f)
                {
                    // Update grid status
                    gridUnit.isEmpty = false;
                    gridUnit.objName = gameObject.name;
                    // You can leave checkoutTime unchanged if it's still 0f
                }
            }
        } */
    }
}
