using System.Collections.Generic;
using UnityEngine;

public class CoordinatePlane : MonoBehaviour
{
    [SerializeField] private int xAxisSize = 11; // Number of units on X axis
    [SerializeField] private int zAxisSize = 11; // Number of units on Z axis
    [SerializeField] private bool useZAxisInsteadOfY = false; // Toggle to use Z instead of Y
    [SerializeField] private int unitSize = 10; // Size of each grid unit

    // Definition of a single grid unit
    public class GridUnit
    {
        public bool isEmpty = true; // Is the grid unit empty
        public string objName = ""; // Object name occupying the grid unit
        public float checkoutTime = 0f; // Time when the grid unit was checked out
    }

    // 2D list to store grid units
    private List<List<GridUnit>> gridUnits = new List<List<GridUnit>>();

    private void Start()
    {
        InitializeGridUnits();
        LogGridInformation();
    }

    // Method to initialize the grid units
    private void InitializeGridUnits()
    {
        gridUnits.Clear(); // Clear existing grid units

        for (int x = 0; x < xAxisSize; x++)
        {
            List<GridUnit> row = new List<GridUnit>();
            for (int z = 0; z < zAxisSize; z++)
            {
                row.Add(new GridUnit());
            }
            gridUnits.Add(row);
        }
    }

    // Method to get a grid unit at specified coordinates
    public GridUnit GetGridUnit(int x, int z)
    {
        if (IsWithinBounds(x, z))
        {
            return gridUnits[x + xAxisSize / 2][z + zAxisSize / 2];
        }
        else
        {
            Debug.LogError("Coordinates out of bounds.");
            return null;
        }
    }

    // Method to check if coordinates are within bounds
    public bool IsWithinBounds(int x, int z)
    {
        int offset = xAxisSize % 2 > 0 ? 1 : 0;
        return x >= -xAxisSize / 2 && x < (xAxisSize / 2) + offset && z >= -zAxisSize / 2 && z < (zAxisSize / 2) + offset;
    }

    // Method to check if given coordinates are empty
    public bool IsEmpty(int x, int z)
    {
        GridUnit gridUnit = GetGridUnit(x, z);
        return gridUnit != null && gridUnit.isEmpty;
    }

    // Method to get objName by given coordinates
    public string GetObjName(int x, int z)
    {
        GridUnit gridUnit = GetGridUnit(x, z);
        return gridUnit != null ? gridUnit.objName : null;
    }

    // Method to get checkoutTime by given coordinates
    public float GetCheckoutTime(int x, int z)
    {
        GridUnit gridUnit = GetGridUnit(x, z);
        return gridUnit != null ? gridUnit.checkoutTime : 0f;
    }

    // Method to get the number of units on the X axis
    public int GetXAxisSize()
    {
        return xAxisSize;
    }

    // Method to get the number of units on the Z axis
    public int GetZAxisSize()
    {
        return zAxisSize;
    }

    // Method to get the size of each grid unit
    public int GetUnitSize()
    {
        return unitSize;
    }

    // Method to set grid unit information at specified coordinates
    public void SetGridUnitInfo(int x, int z, bool isEmpty, string objName, float checkoutTime)
    {
        if (IsWithinBounds(x, z))
        {
            GridUnit gridUnit = GetGridUnit(x, z);
            if (gridUnit != null)
            {
                gridUnit.isEmpty = isEmpty;
                gridUnit.objName = objName;
                gridUnit.checkoutTime = checkoutTime;
            }
        }
        else
        {
            Debug.LogError("Coordinates out of bounds.");
        }
    }

    // Overloaded method to set grid unit information at specified coordinates without checkoutTime
    public void SetGridUnitInfo(int x, int z, bool isEmpty, string objName)
    {
        if (IsWithinBounds(x, z))
        {
            GridUnit gridUnit = GetGridUnit(x, z);
            if (gridUnit != null)
            {
                gridUnit.isEmpty = isEmpty;
                gridUnit.objName = objName;
            }
        }
        else
        {
            Debug.LogError("Coordinates out of bounds.");
        }
    }

    // Method to convert world position to grid coordinates
    public Vector2Int WorldToGridCoordinates(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / unitSize);
        int z = Mathf.FloorToInt(position.z / unitSize);
        return new Vector2Int(x, z);
    }

    // Method to convert grid coordinates to world position
    public Vector3 GridToWorldCoordinates(int x, int z)
    {
        float posX = (x - xAxisSize / 2) * unitSize;
        float posY = 0f;
        float posZ = (z - zAxisSize / 2) * unitSize;
        return new Vector3(posX, posY, posZ);
    }

    // Method to log information for each grid unit
    public void LogGridInformation()
    {
        int offset = xAxisSize % 2 > 0 ? 1 : 0;
        for (int x = -xAxisSize / 2; x < (xAxisSize / 2) + offset; x++)
        {
            for (int z = -zAxisSize / 2; z < (zAxisSize / 2) + offset; z++)
            {
                GridUnit gridUnit = GetGridUnit(x, z);
                if (gridUnit != null)
                {
                    Debug.Log($"Grid Unit at ({x}, {z}): isEmpty = {gridUnit.isEmpty}, objName = {gridUnit.objName}, checkoutTime = {gridUnit.checkoutTime}");
                }
            }
        }
    }

    // Method to set a grid unit as empty
    public void SetEmpty(int x, int z)
    {
        SetGridUnitInfo(x, z, true, "", 0f);
    }

    // Method to set a grid unit as busy
    public void SetBusy(int x, int z)
    {
        SetGridUnitInfo(x, z, false, "", Time.time);
    }

    // Method to set a grid unit as busy
    public void SetBusy(int x, int z, string objName)
    {
        SetGridUnitInfo(x, z, false, objName, Time.time);
    }
    
    // Method to set the checkout time for a grid unit
    public void CheckoutTime(int x, int z, float time)
    {
        if (IsWithinBounds(x, z))
        {
            GridUnit gridUnit = GetGridUnit(x, z);
            if (gridUnit != null)
            {
                gridUnit.checkoutTime = time;
            }
        }
        else
        {
            Debug.LogError("Coordinates out of bounds.");
        }
    }
}
