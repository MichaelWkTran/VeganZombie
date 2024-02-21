using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Hoe", menuName = "Tool/Hoe", order = 1)]
public class ItemHoe : UsableItem
{
    public float m_animationSpeed;
    float m_actionFrame;
    public PlantPatch m_plantPatchPrefab;

    public override IEnumerator UseItem(Collider2D _userCollider, Vector3 _spawnPos = default, Vector2 _lookDir = default)
    {
        yield return new WaitForSeconds(m_animationSpeed * m_actionFrame);
        
        Vector3Int cellPosition = GameManager.m_current.m_PloughableTilemap.WorldToCell(_spawnPos);
        
        //Check whether the grid cell can be ploughed
        UnityEngine.Tilemaps.Tilemap ploughableTilemap = GameManager.m_current.m_PloughableTilemap;
        if (!ploughableTilemap.HasTile(ploughableTilemap.WorldToCell(cellPosition))) yield break;
        
        //Check whether the the area has not been ploughed
        if (GameManager.m_current.m_GridManager.GetGridObjectsFromPosition(cellPosition)?.Find(i => (PlantPatch)i != null) != null) yield break;

        //Place the plant patch
        PlantPatch plantPatch = Instantiate(m_plantPatchPrefab);
        plantPatch.m_CellPos = cellPosition;
    }
}
