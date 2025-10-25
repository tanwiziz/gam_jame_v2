using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    public int width = 9;
    public int height = 9;

    [Header("Start Area (unlocked)")]
    public int startW = 5; // กว้าง 5
    public int startH = 3; // สูง 3

    public InventoryCell[,] cells;

    void Awake()
    {
        if (cells == null || cells.GetLength(0) != width || cells.GetLength(1) != height)
        {
            cells = new InventoryCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new InventoryCell();

                    // ✅ ปลดล็อกช่องตรงกลาง 5x3
                    if (x >= 2 && x <= 6 && y >= 3 && y <= 5)
                        cells[x, y].state = CellState.Unlocked;
                    else
                        cells[x, y].state = CellState.Locked;
                }
            }
        }
    }

    public bool IsInside(Vector2Int p) => p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
    public InventoryCell GetCell(Vector2Int p) => IsInside(p) ? cells[p.x, p.y] : null;

    public IEnumerable<ItemInstance> AllItems()
    {
        var set = new HashSet<ItemInstance>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (cells[x, y].item != null)
                    set.Add(cells[x, y].item);
        return set;
    }

    public void RemoveItem(ItemInstance item)
    {
        if (item == null) return;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (cells[x, y].item == item) cells[x, y].item = null;
    }

    public bool TryPlaceItem(ItemInstance item) // auto-fit
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (TryPlaceItem(item, new Vector2Int(x, y))) return true;
        return false;
    }

    public bool TryPlaceItem(ItemInstance item, Vector2Int root)
    {
        if (item == null || item.def == null) return false;
        var shape = (item.def.shapeOffsets != null && item.def.shapeOffsets.Length > 0)
                    ? item.def.shapeOffsets : new[] { Vector2Int.zero };

        // ตรวจทุกช่องต้องอยู่ในเขตปลดล็อกและว่าง
        foreach (var ofs in shape)
        {
            var p = root + ofs;
            if (!IsInside(p)) return false;
            var c = cells[p.x, p.y];
            if (c.state != CellState.Unlocked) return false;
            if (c.item != null) return false;
        }

        // วางจริง
        foreach (var ofs in shape)
        {
            var p = root + ofs;
            cells[p.x, p.y].item = item;
        }
        item.gridPosition = root; // <<< บันทึกรากของไอเทม (เพิ่มฟิลด์นี้ใน ItemInstance)
        return true;
    }

}
