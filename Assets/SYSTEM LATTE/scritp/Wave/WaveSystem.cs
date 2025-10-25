using System.Collections;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [Header("Enemies")]
    public EnemyLibrary enemyLibrary;
    public Transform[] spawnPoints;

    [Header("Wave Config")]
    public int baseCount = 6;
    public float spawnInterval = 0.35f;

    public int CurrentWave { get; private set; } = 0;
    public event System.Action<int> OnWaveStarted;
    public event System.Action<int> OnWaveCleared;

    int alive;
    bool spawning;

    public void StartWave() => StartNextWave();

    public void StartNextWave()
    {
        if (!spawning) StartCoroutine(Co_SpawnWave());
    }

    IEnumerator Co_SpawnWave()
    {
        if (enemyLibrary == null || enemyLibrary.enemies == null || enemyLibrary.enemies.Count == 0)
        {
            Debug.LogError("[WaveSystem] EnemyLibrary is empty or missing.");
            yield break;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveSystem] No spawnPoints.");
            yield break;
        }

        spawning = true;
        CurrentWave++;
        OnWaveStarted?.Invoke(CurrentWave);

        int count = baseCount + (CurrentWave - 1) * 2;
        alive = count;

        for (int i = 0; i < count; i++)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitUntil(() => alive <= 0);
        OnWaveCleared?.Invoke(CurrentWave);
        spawning = false;
    }

    void SpawnOne()
    {
        var enemyPrefab = enemyLibrary.GetRandomEnemy();
        if (!enemyPrefab) return;

        var p = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var go = Instantiate(enemyPrefab, p.position, Quaternion.identity);
        var e = go.GetComponent<Enemy>();
        if (e != null) e.OnDied += OnEnemyDied;
    }

    void OnEnemyDied()
    {
        alive = Mathf.Max(0, alive - 1);
    }
}
