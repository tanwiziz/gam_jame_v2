using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;       // พรีแฟบของศัตรู
    public Transform[] spawnPoints;      // จุดเกิดศัตรู

    [Header("Spawn Timing")]
    public float spawnInterval = 3f;     // เวลาระหว่างการเกิดแต่ละรอบ
    public bool spawnLoop = true;        // ให้เกิดซ้ำเรื่อยๆไหม

    private void Start()
    {
        if (spawnLoop)
            InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
        else
            SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null)
        {
            Debug.LogWarning("⚠️ ไม่มีจุดเกิดหรือ Enemy Prefab ยังไม่ได้ตั้งค่า");
            return;
        }

        // สุ่มเลือกจุดเกิด
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // สร้าง Enemy ที่จุดนั้น
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
