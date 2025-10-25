using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro; // 🟢 ต้องเพิ่มบรรทัดนี้เพื่อใช้ TextMeshPro

public class WaveManager : MonoBehaviour
{
    // --- Prefabs มอนสเตอร์ ---
    // ... (ตัวแปร Prefabs เดิม) ...
    public GameObject monsterPrefab1; 
    public GameObject monsterPrefab2; 
    public GameObject monsterPrefab3; 
    public GameObject bossPrefab;     

    // --- จุด Spawn ---
    // ... (ตัวแปร Spawn เดิม) ...
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float spawnInterval = 0.5f; 
    public int baseEnemyCount = 5;     

    // --- ตรรกะ Wave ---
    [Header("Wave Logic")]
    public int currentWave = 1;
    public const int MAX_WAVE = 100;
    public const int BOSS_WAVE_INTERVAL = 15;

    // 🟢 ตัวแปรใหม่สำหรับ UI
    [Header("UI Display")]
    public TextMeshProUGUI waveText; 
    // ถ้าคุณใช้ Text ธรรมดา ให้ใช้ public UnityEngine.UI.Text waveText; แทน
    
    private int enemiesRemaining;
    private bool isSpawning = false;
    private const string ENEMY_TAG = "Enemy"; 

    void Start()
    {
        if (spawnPoints.Length == 0 || monsterPrefab1 == null)
        {
            Debug.LogError("WaveManager ไม่พร้อม! ตรวจสอบ Spawn Points และ Prefabs");
            enabled = false;
            return;
        }

        // 🟢 อัปเดต UI ทันทีเมื่อเริ่ม
        UpdateWaveDisplay(); 
        StartNextWave();
    }

    void Update()
    {
        if (isSpawning || currentWave > MAX_WAVE) 
            return;

        enemiesRemaining = GameObject.FindGameObjectsWithTag(ENEMY_TAG).Length;

        if (enemiesRemaining <= 0)
        {
            if (currentWave < MAX_WAVE)
            {
                currentWave++;
                Debug.Log($"--- Wave {currentWave} เริ่มแล้ว! ---");
                // 🟢 อัปเดต UI ก่อนเริ่ม Wave ใหม่
                UpdateWaveDisplay(); 
                StartNextWave();
            }
            else
            {
                Debug.Log("เกมจบ! คุณชนะครบ 100 Wave แล้ว!");
                // 🟢 อัปเดต UI เป็นข้อความจบเกม
                if (waveText != null)
                {
                    waveText.text = "Victory!";
                }
                enabled = false;
            }
        }
    }

    // 🟢 ฟังก์ชันใหม่สำหรับอัปเดตข้อความบนหน้าจอ
    void UpdateWaveDisplay()
    {
        if (waveText != null)
        {
            waveText.text = $"WAVE {currentWave}/{MAX_WAVE}";
        }
        else
        {
            Debug.LogWarning("WaveText UI component is missing in the Inspector!");
        }
    }


    // --- โค้ดส่วนอื่นๆ (StartNextWave, SpawnWaveCoroutine, SpawnEnemy) ยังคงเดิม ---

    void StartNextWave()
    {
        isSpawning = true;
        StartCoroutine(SpawnWaveCoroutine());
    }

    // ... (ฟังก์ชัน SpawnWaveCoroutine และ SpawnEnemy ยังคงเดิม) ...
    IEnumerator SpawnWaveCoroutine()
    {
        // ... (โค้ด Spawn Wave เดิม) ...
        bool isBossWave = (currentWave % BOSS_WAVE_INTERVAL == 0);
        
        if (isBossWave)
        {
            Debug.Log($"!!! BOSS WAVE ที่ {currentWave} !!!");
            yield return StartCoroutine(SpawnEnemy(bossPrefab, 1));
        }
        else
        {
            // ... (กำหนดชนิดมอนสเตอร์) ...
            GameObject enemyToSpawn;
            int enemyCount;

            if (currentWave >= 20)
            {
                enemyToSpawn = monsterPrefab3;
            }
            else if (currentWave >= 10)
            {
                enemyToSpawn = monsterPrefab2;
            }
            else 
            {
                enemyToSpawn = monsterPrefab1;
            }

            enemyCount = baseEnemyCount + (currentWave / 5); 
            
            Debug.Log($"Spawn มอนสเตอร์ชนิด: {enemyToSpawn.name} จำนวน: {enemyCount}");
            yield return StartCoroutine(SpawnEnemy(enemyToSpawn, enemyCount));
        }
        
        isSpawning = false;
    }

    IEnumerator SpawnEnemy(GameObject enemyPrefab, int count)
    {
        // ... (โค้ด Spawn Enemy เดิม) ...
        for (int i = 0; i < count; i++)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPosition = randomPoint.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 5f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
            }

            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}