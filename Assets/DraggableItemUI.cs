
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Component สำหรับจัดการการลากวาง (Drag & Drop) ของไอเทมจาก Drop Panel ไปยัง Inventory Grid.
/// ต้องถูกแนบไว้กับ GameObject ที่มี Image Component (ไอคอนอาวุธ).
/// </summary>
public class DraggableItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [Tooltip("InventoryUI หลักที่ใช้แสดง Highlight และจัดการการวาง")]
    public InventoryUI inventoryUI;

    [Tooltip("ItemDefinition (ScriptableObject) ของไอเทมที่ต้องการลาก")]
    public ItemDefinition itemDefinition;

    // Internal
    private ItemInstance currentItemInstance;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Image itemImage;
    private bool isDragging = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        itemImage = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();

        if (itemImage && itemDefinition && itemDefinition.icon)
        {
            itemImage.sprite = itemDefinition.icon;
            itemImage.SetNativeSize(); // ตั้งค่าขนาดตาม Sprite
        }
    }

    /// <summary>
    /// เริ่มการลาก: สร้าง ItemInstance ชั่วคราว และเริ่ม Preview ใน InventoryUI
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventoryUI == null || itemDefinition == null) return;

        // 1. สร้าง Instance ของไอเทม (ItemInstance) สำหรับลาก
        currentItemInstance = new ItemInstance(itemDefinition);
        
        // 2. บอก InventoryUI ให้เริ่มแสดง Preview
        inventoryUI.BeginPreview(currentItemInstance);

        // 3. เตรียม UI ของไอคอน:
        // - ให้ไอคอนตามเมาส์
        // - ตั้งค่า Parent เป็น Canvas เพื่อให้อยู่ด้านบนสุด (เหนือ UI อื่นๆ)
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        
        // - ลดความทึบ (Opacity) ขณะลาก
        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 0.5f);
        
        isDragging = true;
    }

    /// <summary>
    /// อัปเดตตำแหน่งของไอคอนตามเมาส์ และอัปเดต Preview ใน InventoryUI
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // 1. ทำให้ไอคอนตามเมาส์
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        else // ScreenSpaceCamera / WorldSpace
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform, eventData.position, 
                canvas.worldCamera, out position);
            rectTransform.localPosition = position;
        }

        // 2. อัปเดต InventoryUI Preview
        inventoryUI.UpdatePreview(eventData.position);

        // 3. ตรวจสอบการหมุน (ถ้ามีการกดปุ่ม 'R')
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentItemInstance.Rotate();
            // ต้องเรียก UpdatePreview ซ้ำหลังการหมุน
            inventoryUI.UpdatePreview(eventData.position); 
        }
    }

    /// <summary>
    /// จบการลาก: พยายามวางไอเทม หากวางไม่ได้จะลบ ItemInstance ทิ้ง และคืนค่าไอคอนกลับไปที่ Drop Panel
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        // 1. พยายามวางไอเทมลงใน Grid
        bool placed = inventoryUI.TryPlacePreview();

        // 2. คืนค่า UI
        // - คืน Parent กลับไปที่ Drop Panel เดิม (สมมติว่า Drop Panel เป็น Parent เดิม)
        transform.SetParent(transform.parent); 
        
        // - ตั้งค่าตำแหน่งและ Opacity ให้กลับเป็นปกติ
        rectTransform.anchoredPosition = Vector2.zero;
        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1f);

        // 3. ถ้าวางไม่สำเร็จ ItemInstance จะถูกทำลายใน TryPlacePreview() แล้ว
        
        currentItemInstance = null;
        isDragging = false;
    }
}
