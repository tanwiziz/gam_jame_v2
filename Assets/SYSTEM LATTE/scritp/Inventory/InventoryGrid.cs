using UnityEngine;

/// <summary>
/// The backend logic for the inventory grid, managing item placement and cell states.
/// </summary>
public class InventoryGrid : MonoBehaviour
{
    // ใช้ enum จาก InventoryCellUI เพื่อความง่าย
    public InventoryCellUI.CellState defaultState = InventoryCellUI.CellState.Unlocked;

    private class GridCell
    {
        public InventoryCellUI.CellState state;
        public ItemInstance item; // null if cell is empty

        public GridCell(InventoryCellUI.CellState initialState)
        {
            state = initialState;
            item = null;
        }
    }

    [Header("Grid Dimensions")]
    public int gridW = 9;
    public int gridH = 9;

    private GridCell[,] grid;

    private void Awake()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        grid = new GridCell[gridW, gridH];
        for (int y = 0; y < gridH; y++)
        {
            for (int x = 0; x < gridW; x++)
            {
                // TODO: โหลดสถานะล็อค/ปลดล็อคจากไฟล์บันทึกจริง
                grid[x, y] = new GridCell(defaultState); 
            }
        }
    }

    public GridCell GetCell(Vector2Int pos)
    {
        if (!IsInside(pos)) return null;
        return grid[pos.x, pos.y];
    }

    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridW && pos.y >= 0 && pos.y < gridH;
    }

    /// <summary>
    /// Checks if an item can be placed at the given root position and rotation.
    /// </summary>
    public bool CanPlaceItem(ItemInstance item, Vector2Int root)
    {
        if (item.def == null) return false;

        // คำนวณเซลล์ทั้งหมดที่ไอเทมจะครอบครอง
        Vector2Int[] occupiedCells = item.GetOccupiedCells(root);

        foreach (var cellPos in occupiedCells)
        {
            // 1. ตรวจสอบขอบเขต
            if (!IsInside(cellPos))
            {
                Debug.LogWarning($"Placement failed: Out of bounds at {cellPos}");
                return false;
            }

            // 2. ตรวจสอบสถานะและว่ามีไอเทมอื่นอยู่หรือไม่
            var cell = GetCell(cellPos);
            if (cell.state == InventoryCellUI.CellState.Locked)
            {
                Debug.LogWarning($"Placement failed: Cell at {cellPos} is locked.");
                return false;
            }
            if (cell.item != null)
            {
                // ตรวจสอบว่าไอเทมที่อยู่ในช่องนั้นคือไอเทมที่เรากำลังจะวางหรือไม่
                // (กรณีลากวางทับตัวเอง) - ในตัวอย่างนี้ เราถือว่าเป็นการวางไอเทมใหม่เสมอ
                Debug.LogWarning($"Placement failed: Cell at {cellPos} is already occupied by an item.");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Attempts to place the item into the grid.
    /// </summary>
    public bool TryPlaceItem(ItemInstance item, Vector2Int root)
    {
        if (!CanPlaceItem(item, root)) return false;

        // ลบไอเทมเก่าออกก่อน ถ้าไอเทมนี้ถูกย้ายมาจากที่อื่น
        // (สำหรับการใช้งานจริง คุณต้องเพิ่มฟังก์ชัน RemoveItem ก่อน)

        Vector2Int[] occupiedCells = item.GetOccupiedCells(root);
        item.rootPosition = root;

        foreach (var cellPos in occupiedCells)
        {
            GetCell(cellPos).item = item;
        }
        
        // ควรแจ้งเตือน UI ให้อัปเดตหลังการวางสำเร็จ
        // (ใน UI_Inventory.cs จะมีการเรียก UpdateGridIcons() หลังวางสำเร็จ)
        return true;
    }
}
