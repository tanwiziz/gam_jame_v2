using UnityEngine;
using UnityEngine.UI;

public class InventoryCellUI : MonoBehaviour
{
    public Image icon;
    public Image background;

    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    public Sprite expandSprite;

    public void Setup(CellState state)
    {
        if (background == null)
            background = GetComponent<Image>();

        switch (state)
        {
            case CellState.Locked:
                background.sprite = lockedSprite;
                background.color = new Color(1, 1, 1, 0.7f);
                break;
            case CellState.Unlocked:
                background.sprite = unlockedSprite;
                background.color = Color.white;
                break;
            case CellState.Expand:
                background.sprite = expandSprite;
                background.color = new Color(0.6f, 1f, 0.6f, 0.8f);
                break;
        }
    }

    public void SetIcon(Sprite iconSprite)
    {
        if (icon != null)
        {
            icon.sprite = iconSprite;
            icon.enabled = (iconSprite != null);
        }
    }

    public void ClearIcon()
    {
        if (icon != null)
            icon.enabled = false;
    }
}
