using UnityEngine;
using TMPro;

public class HUD_WaveLabel : MonoBehaviour
{
    public WaveSystem waveSystem;
    public TextMeshProUGUI waveText;

    void Start()
    {
        if (waveSystem == null) waveSystem = FindObjectOfType<WaveSystem>();
        if (waveText == null) waveText = GetComponent<TextMeshProUGUI>();

        if (waveSystem != null)
        {
            waveSystem.OnWaveStarted += OnWaveStarted;
            waveSystem.OnWaveCleared += OnWaveCleared;
        }

        UpdateWaveText(0);
    }

    void OnDestroy()
    {
        if (waveSystem != null)
        {
            waveSystem.OnWaveStarted -= OnWaveStarted;
            waveSystem.OnWaveCleared -= OnWaveCleared;
        }
    }

    void OnWaveStarted(int waveIndex) => UpdateWaveText(waveIndex);
    void OnWaveCleared(int waveIndex) => waveText.text = $"WAVE {waveIndex} CLEARED!";

    void UpdateWaveText(int wave) { if (waveText) waveText.text = $"WAVE {wave}"; }
}
