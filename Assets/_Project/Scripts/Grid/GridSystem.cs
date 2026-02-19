using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    private int width;
    private int height;
    private float cellSize;
    private GridObject[,] gridObjectArray;

    public GridSystem(int width, int height, float cellSize, Transform debugPrefab = null, Transform parent = null)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new GridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectArray[x, z] = new GridObject(this, gridPosition);

                if (debugPrefab != null)
                {
                    Transform debugObj = Object.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, parent);
                    debugObj.GetComponent<GridDebugObject>().SetGridObject(gridObjectArray[x, z]);
                }
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        // Convertimos (x, z) lógico a (x * tamaño, 0, z * tamaño) en el mundo 3D
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        // Inverso: convertimos posición de mundo a índice de grid
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
        );
    }

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        if (IsValidGridPosition(gridPosition))
        {
            return gridObjectArray[gridPosition.x, gridPosition.z];
        }
        else
        {
            return null;
        }
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && 
               gridPosition.z >= 0 && 
               gridPosition.x < width && 
               gridPosition.z < height;
    }
}