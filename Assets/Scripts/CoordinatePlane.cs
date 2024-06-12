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
            Debug.Log("GridUnit GetGridUnit(int x, int z): x is " + x + ", z is "+z);
            return gridUnits[x + xAxisSize / 2][z + zAxisSize / 2];
        }
        else
        {
            Debug.LogError("Coordinates out of bounds.");
            Debug.Log("IsWithinBounds(x, z) is " + IsWithinBounds(x, z));
            Debug.Log("x is " + x + ", z is " + z);
            return null;
        }
    }

    // Method to check if coordinates are within bounds
    public bool IsWithinBounds(int x, int z)
    {
        // Error when x is 0, z is 5
        // 0 >= -5.5 && 0 < 5.5 && 5 >= -5.5 && && 5 < 5.5
        // return x >= -xAxisSize / 2 && x < xAxisSize / 2 && z >= -zAxisSize / 2 && z < zAxisSize / 2;

        // Return false is correct because range of z is -5.5 to 5.5
        return x >= -xAxisSize / 2 && x < xAxisSize / 2 && z >= -zAxisSize / 2 && z < zAxisSize / 2;
    }

    // Method to check if given coordinates are empty
    public bool IsEmpty(int x, int z)
    {
        return GetGridUnit(x, z).isEmpty;
    }

    // Method to get objName by given coordinates
    public string GetObjName(int x, int z)
    {
        return GetGridUnit(x, z).objName;
    }

    // Method to get checkoutTime by given coordinates
    public float GetCheckoutTime(int x, int z)
    {
        return GetGridUnit(x, z).checkoutTime;
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
            gridUnit.isEmpty = isEmpty;
            gridUnit.objName = objName;
            gridUnit.checkoutTime = checkoutTime;
        }
        else
        {
            Debug.LogError("Coordinates out of bounds.");
            Debug.LogError("IsWithinBounds x is " + x + " and z is " + z);
        }
    }

    // Method to set grid unit information at specified coordinates
    public void SetGridUnitInfo(int x, int z, bool isEmpty, string objName)
    {
        if (IsWithinBounds(x, z))
        {
            GridUnit gridUnit = GetGridUnit(x, z);
            gridUnit.isEmpty = isEmpty;
            gridUnit.objName = objName;
            //gridUnit.checkoutTime = 0.0f;
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
        int y = Mathf.FloorToInt(position.y / unitSize);
        int z = Mathf.FloorToInt(position.z / unitSize);
        return new Vector2Int(x, z);
    }

    // Method to convert grid coordinates to world position
    public Vector3 GridToWorldCoordinates(int x, int z)
    {
        float posX = (x - xAxisSize / 2) * unitSize;
        //float posY = (y - zAxisSize / 2) * unitSize;
        float posY = 0f;
        float posZ = (z - zAxisSize / 2) * unitSize;
        return new Vector3(posX, posY, posZ);
    }

    // Method to log information for each grid unit
    public void LogGridInformation()
    {
        // Iterate over the entire grid
        for (int x = -xAxisSize / 2; x < xAxisSize / 2; x++)
        {
            for (int z = -zAxisSize / 2; z < zAxisSize / 2; z++)
            {
                // Get the grid unit at the current coordinates
                GridUnit gridUnit = GetGridUnit(x, z);

                // Calculate the relative coordinates for logging
                int relativeX = x + xAxisSize / 2;
                int relativeZ = z + zAxisSize / 2;

                // Log the information for the grid unit
                Debug.Log("Grid Unit at (" + x + ", " + z + "): isEmpty = " + gridUnit.isEmpty + ", objName = " + gridUnit.objName + ", checkoutTime = " + gridUnit.checkoutTime);
            }
        }
    }
}
