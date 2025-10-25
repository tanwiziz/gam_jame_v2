using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_WeaponOption : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI label;

    InventoryUI inv;
    WeaponDefinition def;
    RectTransform ghost;
    Canvas root;

    public void Init(WeaponDefinition d, InventoryUI inventoryUI)
    {
        def = d;
        inv = inventoryUI;
        if (!root) root = GetComponentInParent<Canvas>();
        if (label) label.text = d ? d.weaponName : "Unknown";
        if (icon && d && d.icon)
        {
            icon.enabled = true;
            icon.sprite = d.icon;
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (!def || !inv) return;
        inv.BeginPreview(new ItemInstance(def));  // ✅ ใช้ได้แล้ว

        ghost = new GameObject("Ghost").AddComponent<RectTransform>();
        ghost.SetParent(root.transform, false);
        var img = ghost.gameObject.AddComponent<Image>();
        img.raycastTarget = false;
        img.sprite = def.icon;
        img.color = new Color(1, 1, 1, 0.85f);
        ghost.sizeDelta = new Vector2(64, 64);
        ghost.position = e.position;
    }

    public void OnDrag(PointerEventData e)
    {
        if (ghost) ghost.position = e.position;
        inv?.UpdatePreview(e.position);  // ✅ ใช้ได้แล้ว
    }

    public void OnEndDrag(PointerEventData e)
    {
        inv?.TryPlacePreview();  // ✅ ใช้ได้แล้ว
        if (ghost) Destroy(ghost.gameObject);
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (!def || !inv) return;
        inv.BeginPreview(new ItemInstance(def));
        inv.UpdatePreview(e.position);
        inv.TryPlacePreview();
    }
}
