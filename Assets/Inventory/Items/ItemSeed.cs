using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Item", menuName = "Inventory/Create New Seed")]
public class ItemSeed : UsableItem
{
    public Sprite[] m_growthStages; //The stages of the seed as it grows from a seed to a vegtable
    public float m_finalProgress; //If progress reached final progress, the plant is ready to be plucked
    public float m_maxDryness; //The max dryness of the plant
    public float m_minDryness; //If below min dryness, the plant is dead
    public Item m_harvestedItem;
    float m_actionFrame;

    public override IEnumerator UseItem(Collider2D _userCollider, Vector3 _spawnPos = default, Vector2 _lookDir = default)
    {
        yield return new WaitForSeconds(m_actionFrame);

        Vector3Int cellPosition = GameManager.m_current.m_PloughableTilemap.WorldToCell(_spawnPos);
        GridObject plantPatchGridObject = GameManager.m_current.m_GridManager.GetGridObjectsFromPosition(cellPosition)?.Find(i => (PlantPatch)i != null);

        //Check whether a plant patch exists on the cell
        if (plantPatchGridObject == null) yield break;
        
        //Get Plant Patch
        PlantPatch plantPatch = (PlantPatch)plantPatchGridObject;

        //Ensure the Plant Patch does not have a seed planted
        if (plantPatch.m_ItemSeed != null) yield break;

        //Plant the seed and remove it from the inventory
        plantPatch.m_ItemSeed = this;
        GameManager.m_current.m_PlayerInventory.SubtractItem(this, 1);
    }
}
