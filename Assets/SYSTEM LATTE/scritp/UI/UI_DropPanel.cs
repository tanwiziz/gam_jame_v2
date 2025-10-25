using UnityEngine;
using UnityEngine.UI;

public class UI_DropPanel : MonoBehaviour
{
    public InventoryUI inventoryUI;
    public GameManager gameManager;

    public RectTransform optionParent;
    public GameObject optionPrefab;
    public Button confirmButton;

    public int dropCount = 3;

    void Awake()
    {
        if (confirmButton)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                inventoryUI.HideInventory();
                Time.timeScale = 1f;
                gameManager.NextWave();
            });
        }
        gameObject.SetActive(false);
    }

    public void OpenDropPanel(int waveIndex)
    {
        if (!optionParent || !optionPrefab)
        {
            Debug.LogError("[DropPanel] Missing refs: please assign optionParent and optionPrefab in Inspector.");
            return;
        }

        ClearOptions();
        inventoryUI.ShowInventory();

        for (int i = 0; i < dropCount; i++)
        {
            var def = WeaponLibrary.GetRandomWeapon();
            if (!def) continue;

            var go = Instantiate(optionPrefab, optionParent);
            var opt = go.GetComponent<UI_WeaponOption>();
            if (!opt) opt = go.AddComponent<UI_WeaponOption>();
            opt.Init(def, inventoryUI);
        }

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    void ClearOptions()
    {
        for (int i = optionParent.childCount - 1; i >= 0; i--)
            Destroy(optionParent.GetChild(i).gameObject);
    }
}
