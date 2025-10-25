using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// ระบบ Inventory Grid UI หลัก
/// - แสดงช่องทั้งหมด (locked / unlocked / expand)
/// - แสดง highlight ขณะวางอาวุธ
/// - ใช้เชื่อมกับ InventoryGrid.cs
/// - รองรับลากวางอาวุธจาก DropPanel (UI_WeaponOption)
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public InventoryGrid grid;

    [Header("UI Layout")]
    public RectTransform gridParent;      // GameObject ที่มี GridLayoutGroup
    public RectTransform highlightParent; // สำหรับแสดง highlight ขณะลาก
    public Image highlightPrefab;         // Prefab ของ highlight

    [Header("Grid Cell Prefab")]
    public InventoryCellUI cellPrefab;    // ช่องแต่ละช่องใน grid

    [Header("Sprites (ช่องพื้นหลัง)")]
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    public Sprite expandSprite;

    [Header("Layout Settings")]
    public int gridW = 9;
    public int gridH = 9;
    public int cellSize = 64;
    public int spacing = 2;

    // ========== INTERNAL ==========
    private InventoryCellUI[,] uiCells;
    private ItemInstance previewItem;
    private Vector2Int previewRoot;
    private Image[] highlights = new Image[0];
    private bool previewValid = false;

    // ===============================

    void Start()
    {
        if (grid == null) grid = FindObjectOfType<InventoryGrid>();
        if (gridParent == null) Debug.LogError("[InventoryUI] gridParent missing!");

        DrawGridBackground();
    }

    // ==========================================
    // 🔹 วาดช่องทั้งหมดแบบถาวร (Persistent Grid)
    // ==========================================
    public void DrawGridBackground()
    {
        if (!gridParent || !cellPrefab) return;

        // ลบของเก่า
        foreach (Transform c in gridParent)
            Destroy(c.gameObject);

        // ตั้งค่า GridLayoutGroup
        var layout = gridParent.GetComponent<GridLayoutGroup>();
        if (!layout) layout = gridParent.gameObject.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(cellSize, cellSize);
        layout.spacing = new Vector2(spacing, spacing);
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = gridW;
        layout.childAlignment = TextAnchor.UpperLeft;

        uiCells = new InventoryCellUI[gridW, gridH];

        for (int y = 0; y < gridH; y++)
        {
            for (int x = 0; x < gridW; x++)
            {
                var ui = Instantiate(cellPrefab, gridParent);
                ui.lockedSprite = lockedSprite;
                ui.unlockedSprite = unlockedSprite;
                ui.expandSprite = expandSprite;

                var cell = grid.GetCell(new Vector2Int(x, y));
                ui.Setup(cell.state);

                uiCells[x, y] = ui;
            }
        }
    }

    // ==========================================
    // 🔹 ลากอาวุธ (Begin / Update / Place)
    // ==========================================

    public void BeginPreview(ItemInstance item)
    {
        ClearPreview();
        previewItem = item;

        var shape = (item.def.shapeOffsets != null && item.def.shapeOffsets.Length > 0)
            ? item.def.shapeOffsets
            : new[] { Vector2Int.zero };

        highlights = new Image[shape.Length];
        for (int i = 0; i < shape.Length; i++)
        {
            var h = Instantiate(highlightPrefab, highlightParent);
            h.rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
            h.color = new Color(1, 1, 1, 0.25f);
            highlights[i] = h;
        }
    }

    public void UpdatePreview(Vector2 screenPos)
    {
        if (previewItem == null || grid == null) return;

        if (!ScreenToGrid(screenPos, out var root))
        {
            SetHighlightsVisible(false);
            previewValid = false;
            return;
        }

        var shape = (previewItem.def.shapeOffsets != null && previewItem.def.shapeOffsets.Length > 0)
            ? previewItem.def.shapeOffsets
            : new[] { Vector2Int.zero };

        bool ok = true;
        for (int i = 0; i < shape.Length; i++)
        {
            var cell = root + shape[i];
            highlights[i].rectTransform.anchoredPosition = GridToLocalCenter(cell);
            if (!grid.IsInside(cell) || grid.GetCell(cell).item != null)
                ok = false;
        }

        previewValid = ok;
        previewRoot = root;
        Color c = ok ? new Color(0, 1, 0, 0.35f) : new Color(1, 0, 0, 0.35f);
        foreach (var h in highlights) h.color = c;
        SetHighlightsVisible(true);
    }

    public bool TryPlacePreview()
    {
        if (previewItem == null || !previewValid)
        {
            ClearPreview();
            return false;
        }

        bool ok = grid.TryPlaceItem(previewItem, previewRoot);
        ClearPreview();
        return ok;
    }

    // ==========================================
    // 🔹 Helper Functions
    // ==========================================
    void ClearPreview()
    {
        if (highlights != null)
            foreach (var h in highlights)
                if (h) Destroy(h.gameObject);
        highlights = new Image[0];
        previewItem = null;
    }

    void SetHighlightsVisible(bool v)
    {
        foreach (var h in highlights)
            if (h) h.gameObject.SetActive(v);
    }

    // แปลงตำแหน่งเมาส์บนจอ -> ตำแหน่งช่องกริด (พิกัดกริด)
    bool ScreenToGrid(Vector2 screenPos, out Vector2Int gridPos)
    {
        gridPos = default;

        // ใช้ RectTransform ของ GridContainer (ไม่ใช่ 'panel')
        var rect = gridParent; // <- สำคัญ!

        if (!rect) return false;

        // รองรับทั้ง Overlay และ ScreenSpace-Camera
        var canvas = GetComponentInParent<Canvas>();
        Camera cam = null;
        if (canvas && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        // screen -> local (อิง UpperLeft)
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, cam, out var local))
            return false;

        // Anchor/Pivot ของ GridContainer = UpperLeft: x ขวา+, y ลง-
        float gx = local.x / cellSize;
        float gy = -local.y / cellSize;

        int ix = Mathf.FloorToInt(gx);
        int iy = Mathf.FloorToInt(gy);

        gridPos = new Vector2Int(ix, iy);
        return ix >= 0 && iy >= 0 && ix < gridW && iy < gridH;
    }

 
    Vector2 GridToLocalCenter(Vector2Int gridPos)
    {
        var cellSize = 64f;
        var spacing = 2f;
        return new Vector2(
            gridPos.x * (cellSize + spacing) + cellSize / 2f,
            -gridPos.y * (cellSize + spacing) - cellSize / 2f
        );
    }



    // ==========================================
    // 🔹 UI Controls
    // ==========================================
    public void ShowInventory() => gameObject.SetActive(true);
    public void HideInventory() => gameObject.SetActive(false);
    public void UpdateGridIcons()
    {
        if (grid == null || uiCells == null) return;

        for (int y = 0; y < gridH; y++)
        {
            for (int x = 0; x < gridW; x++)
            {
                var cell = grid.GetCell(new Vector2Int(x, y));
                var ui = uiCells[x, y];
                if (cell == null || ui == null) continue;

                // ถ้ามี item อยู่ → แสดงไอคอนอาวุธ
                if (cell.item != null && cell.item.def != null && cell.item.def.icon != null)
                {
                    ui.SetIcon(cell.item.def.icon);
                }
                else
                {
                    ui.ClearIcon();
                }
            }
        }
    }
}
