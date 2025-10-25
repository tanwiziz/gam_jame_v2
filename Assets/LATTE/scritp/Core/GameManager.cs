using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Systems")]
    public WaveSystem waveSystem;
    public BaseHealth baseHealth;

    [Header("Inventory / UI")]
    public InventoryUI inventoryUI;
    public UI_DropPanel dropPanel;
    public GameObject gameOverUI;
    public InventoryGrid inventoryGrid;
    public WeaponDefinition startingPistol;


    [Header("Weapons Pool (SO)")]
    public List<WeaponDefinition> allWeapons = new();

    void Awake()
    {
        if (waveSystem) waveSystem.OnWaveCleared += OnWaveCleared;
        if (baseHealth) baseHealth.OnBaseDestroyed += OnBaseDestroyed;
    }

    void OnDestroy()
    {
        if (waveSystem) waveSystem.OnWaveCleared -= OnWaveCleared;
        if (baseHealth) baseHealth.OnBaseDestroyed -= OnBaseDestroyed;
    }

    void Start()
    {
        if (allWeapons != null && allWeapons.Count > 0)
            WeaponLibrary.SetPool(allWeapons);

        if (inventoryUI) inventoryUI.HideInventory();
        if (dropPanel) dropPanel.gameObject.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);

        if (waveSystem) waveSystem.StartWave();
    }

    void OnWaveCleared(int w)
    {
        if (!dropPanel || !inventoryUI) return;
        dropPanel.OpenDropPanel(w);
    }

    void OnBaseDestroyed() => ShowGameOverUI();

    public void NextWave()
    {
        if (waveSystem) waveSystem.StartNextWave();
    }

    public void ShowGameOverUI()
    {
        if (!gameOverUI) { Debug.LogError("GameOverUI not assigned"); return; }
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
