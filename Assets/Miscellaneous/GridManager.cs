using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridManager : MonoBehaviour
{
    public Grid m_grid;
    Dictionary<Vector3Int, List<GridObject>> m_gridObjects = new Dictionary<Vector3Int, List<GridObject>>();

    void OnValidate()
    {
        //Get Grid
        if (m_grid == null) m_grid = GetComponent<Grid>();
    }

    public void AddGridObject(GridObject _gridObject)
    {
        if (!m_gridObjects.TryGetValue(_gridObject.m_CellPos, out List<GridObject> gridObjectList))
        {
            gridObjectList = new List<GridObject>();
            m_gridObjects.Add(_gridObject.m_CellPos, gridObjectList);
        }
        gridObjectList.Add(_gridObject);
    }

    public void RemoveGridObject(GridObject _gridObject)
    {
        if (m_gridObjects.TryGetValue(_gridObject.m_CellPos, out List<GridObject> gridObjectList))
        {
            gridObjectList.Remove(_gridObject);
            if (gridObjectList.Count <= 0) m_gridObjects.Remove(_gridObject.m_CellPos);
        }
    }

    public List<GridObject> GetGridObjectsFromPosition(Vector3Int _cellPosition)
    {
        m_gridObjects.TryGetValue(_cellPosition, out var gridObjectList);
        return gridObjectList;
    }
}
