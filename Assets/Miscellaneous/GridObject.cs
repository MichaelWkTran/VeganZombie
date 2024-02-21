using UnityEngine;

public class GridObject : MonoBehaviour
{
    GridManager m_gridManager;
    public GridManager m_GridManager
    {
        get
        {
            //Set grid manager if one is not currently assigned
            if (m_gridManager == null) m_GridManager = FindObjectOfType<GridManager>();
            
            //Return grid manager
            return m_gridManager;
        }
        set
        {
            //Do not change grid manager if the new grid manager is the same as the current grid manager
            if (m_gridManager == value) return;

            //If the previous set grid manager is not null, remove the object from that grid
            if (m_gridManager != null) m_gridManager.RemoveGridObject(this);

            //Set new grid manager, add the object to the manager and snap to grid
            m_gridManager = value;
            m_cellPos = m_gridManager.m_grid.WorldToCell(transform.position);
            m_gridManager.AddGridObject(this);
            SnapToGrid();
        }
    }

    Vector3Int m_cellPos; public Vector3Int m_CellPos
    {
        get { return m_cellPos; }
        set
        {
            m_GridManager.RemoveGridObject(this);
            m_cellPos = value;
            m_GridManager.AddGridObject(this);
            SnapToGrid();
        }
    }

    void Start()
    {
        if (m_gridManager == null) m_GridManager = FindObjectOfType<GridManager>();
    }

    public void SnapToGrid()
    {
        transform.position = m_GridManager.m_grid.CellToWorld(m_cellPos);
    }
}
