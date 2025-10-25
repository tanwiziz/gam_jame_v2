using UnityEngine;
using System.Collections.Generic;

public class UI_ShopPanel : MonoBehaviour
{
    public WeaponDefinition[] candidateWeapons; // ลิสต์อาวุธที่ใช้สุ่ม
    public InventoryGrid inventoryGrid;
    public Transform optionParent;
    public GameObject optionPrefab;

    private readonly List<GameObject> spawned = new();

    public void GenerateOptions()
    {
        Clear();

        for (int i = 0; i < 3; i++)
        {
            var def = candidateWeapons != null && candidateWeapons.Length > 0
                ? candidateWeapons[Random.Range(0, candidateWeapons.Length)]
                : null;

            if (def == null) continue;

            var go = Instantiate(optionPrefab, optionParent);
            var opt = go.GetComponent<UI_WeaponOption>();
            if (opt != null)
            {
                opt.Init(def, null); // null เพราะใน Shop ยังไม่ต้อง OnSelectWeapon
            }
        }
    }

    public void OnSelectWeapon(WeaponDefinition def)
    {
        if (def == null || inventoryGrid == null) return;
        var item = new ItemInstance(def);
        if (!inventoryGrid.TryPlaceItem(item, Vector2Int.zero))
            Debug.Log("[Shop] No space to place item.");
    }

    private void Clear()
    {
        foreach (var go in spawned) if (go) Destroy(go);
        spawned.Clear();
        if (optionParent != null)
        {
            for (int i = optionParent.childCount - 1; i >= 0; i--)
                Destroy(optionParent.GetChild(i).gameObject);
        }
    }
}
