using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Component for a single cell in the inventory grid.
/// Handles background image based on state (Locked/Unlocked/Expand).
/// </summary>
public class InventoryCellUI : MonoBehaviour
{
    public Image backgroundImage;
    public Image itemIcon; // For displaying a single item icon (mostly for 1x1 or root)
    public enum CellState { Locked, Unlocked, Expand }

    [HideInInspector] public Sprite lockedSprite;
    [HideInInspector] public Sprite unlockedSprite;
    [HideInInspector] public Sprite expandSprite;

    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        if (itemIcon == null)
        {
            // Assumes a child image component exists for the icon
            Transform child = transform.Find("Icon"); 
            if (child) itemIcon = child.GetComponent<Image>();
        }
        ClearIcon();
    }

    public void Setup(CellState state)
    {
        if (!backgroundImage) return;

        switch (state)
        {
            case CellState.Locked:
                backgroundImage.sprite = lockedSprite;
                backgroundImage.color = new Color(0.7f, 0.7f, 0.7f); // สีจางๆ สำหรับช่องล็อค
                break;
            case CellState.Unlocked:
                backgroundImage.sprite = unlockedSprite;
                backgroundImage.color = Color.white;
                break;
            case CellState.Expand:
                backgroundImage.sprite = expandSprite;
                backgroundImage.color = new Color(0.5f, 0.8f, 1f); // สีพิเศษสำหรับการขยาย
                break;
        }
    }

    public void SetIcon(Sprite iconSprite)
    {
        if (itemIcon)
        {
            itemIcon.sprite = iconSprite;
            itemIcon.enabled = true;
        }
    }

    public void ClearIcon()
    {
        if (itemIcon)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
    }
}
