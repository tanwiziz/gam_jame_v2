[System.Serializable]
public class ItemInstance
{
    public WeaponDefinition def;
    public int level = 1;

    // ตำแหน่งรากบนกริด (ใช้ตอน Merge/Refresh)
    public UnityEngine.Vector2Int gridPosition;

    public ItemInstance() { }
    public ItemInstance(WeaponDefinition def) { this.def = def; level = 1; }
}