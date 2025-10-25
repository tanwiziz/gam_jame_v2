using UnityEngine;
using System.Linq;

public class InventoryMerge : MonoBehaviour
{
    public InventoryGrid grid;

    public void TryMergeSameWeapons()
    {
        if (grid == null) grid = FindObjectOfType<InventoryGrid>();
        if (grid == null || grid.AllItems() == null || !grid.AllItems().Any()) return;

        var groups = grid.AllItems()
            .Where(i => i != null && i.def != null)
            .GroupBy(i => new { id = i.def.weaponName, lvl = i.level })
            .ToList();

        foreach (var g in groups)
        {
            var list = g.ToList();
            while (list.Count >= 2)
            {
                var a = list[0];
                var b = list[1];
                grid.RemoveItem(a);
                grid.RemoveItem(b);
                list.RemoveAt(0);
                list.RemoveAt(0);

                var mergedDef = ScriptableObject.Instantiate(a.def);
                mergedDef.damage *= 1.5f;
                var merged = new ItemInstance(mergedDef);

                grid.TryPlaceItem(merged, Vector2Int.zero);
            }
        }

        var ui = FindObjectOfType<InventoryUI>();
        if (ui) ui.UpdateGridIcons();
    }
}
