using System.Collections.Generic;
using UnityEngine;

public class CoordinatesTable : MonoBehaviour
{
    [SerializeField] private int xAxisSize = 11; // Number of units on X axis
    [SerializeField] private int yAxisSize = 11; // Number of units on Y axis
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
            for (int y = 0; y < yAxisSize; y++)
            {
                row.Add(new GridUnit());
            }
            gridUnits.Add(row);
        }
    }

    // Method to get a grid unit at specified coordinates
    public GridUnit GetGridUnit(int x, int y)
    {
        if (IsWithinBounds(x, y))
        {
            return gridUnits[x][y];
        }
        else
        {
            Debug.LogError("Coordinates out of bounds.");
            return null;
        }
    }

    // Method to check if coordinates are within bounds
    public bool IsWithinBounds(int x, int y)
    {
        Debug.Log("IsWithinBounds x is " + x + " and y is " + y);
        return x >= 0 && x < xAxisSize && y >= 0 && y < yAxisSize;
    }

    // Method to check if given coordinates are empty
    public bool IsEmpty(int x, int y)
    {
        return GetGridUnit(x, y).isEmpty;
    }

    // Method to get objName by given coordinates
    public string GetObjName(int x, int y)
    {
        return GetGridUnit(x, y).objName;
    }

    // Method to get checkoutTime by given coordinates
    public float GetCheckoutTime(int x, int y)
    {
        return GetGridUnit(x, y).checkoutTime;
    }

    // Method to get the number of units on the X axis
    public int GetXAxisSize()
    {
        return xAxisSize;
    }

    // Method to get the number of units on the Y axis
    public int GetYAxisSize()
    {
        return yAxisSize;
    }

    // Method to get the size of each grid unit
    public int GetUnitSize()
    {
        return unitSize;
    }

    // Method to set grid unit information at specified coordinates
    public void SetGridUnitInfo(int x, int y, bool isEmpty, string objName, float checkoutTime)
    {
        if (IsWithinBounds(x, y))
        {
            GridUnit gridUnit = GetGridUnit(x, y);
            gridUnit.isEmpty = isEmpty;
            gridUnit.objName = objName;
            gridUnit.checkoutTime = checkoutTime;
        }
        else
        {
            Debug.LogError("Coordinates out of bounds.");
            Debug.LogError("IsWithinBounds x is " + x + " and y is " + y);
        }
    }

    // Method to set grid unit information at specified coordinates
    public void SetGridUnitInfo(int x, int y, bool isEmpty, string objName)
    {
        if (IsWithinBounds(x, y))
        {
            GridUnit gridUnit = GetGridUnit(x, y);
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
        int y = useZAxisInsteadOfY ? Mathf.FloorToInt(position.z / unitSize) : Mathf.FloorToInt(position.y / unitSize);
        return new Vector2Int(x, y);
    }

    // Method to convert grid coordinates to world position
    public Vector3 GridToWorldCoordinates(int x, int y)
    {
        float posX = x * unitSize;
        float posY = useZAxisInsteadOfY ? 0f : y * unitSize;
        float posZ = useZAxisInsteadOfY ? y * unitSize : 0f;
        return new Vector3(posX, posY, posZ);
    }

    // Method to log information for each grid unit
    public void LogGridInformation()
    {
        for (int x = 0; x < xAxisSize; x++)
        {
            for (int y = 0; y < yAxisSize; y++)
            {
                GridUnit gridUnit = GetGridUnit(x, y);
                Debug.Log("Grid Unit at (" + x + ", " + y + "): isEmpty = " + gridUnit.isEmpty + ", objName = " + gridUnit.objName + ", checkoutTime = " + gridUnit.checkoutTime);
            }
        }
    }
}
