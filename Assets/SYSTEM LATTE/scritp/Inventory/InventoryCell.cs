public enum CellState { Locked, Expand, Unlocked }

[System.Serializable]
public class InventoryCell
{
    public ItemInstance item;
    public CellState state = CellState.Unlocked;
}
