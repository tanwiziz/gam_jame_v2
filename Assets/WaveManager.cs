using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // สำหรับ NavMesh.SamplePosition

public class WaveManager : MonoBehaviour
{
    // --- Prefabs มอนสเตอร์ (ต้องลากมาใส่ใน Inspector) ---
    [Header("Monster Prefabs")]
    public GameObject monsterPrefab1; // Monster Type 1 (Wave 1-9)
    public GameObject monsterPrefab2; // Monster Type 2 (Wave 10-19)
    public GameObject monsterPrefab3; // Monster Type 3 (Wave 20+)
    public GameObject bossPrefab;     // Boss Monster (ทุกๆ 15 Wave)

    // --- จุด Spawn (ลาก Transform ของ SpawnPoint มาใส่) ---
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float spawnInterval = 0.5f; // ระยะเวลาระหว่างการ Spawn มอนสเตอร์แต่ละตัว
    public int baseEnemyCount = 5;     // จำนวนมอนสเตอร์เริ่มต้นใน Wave 1

    // --- ตรรกะ Wave ---
    [Header("Wave Logic")]
    public int currentWave = 1;
    public const int MAX_WAVE = 100;
    public const int BOSS_WAVE_INTERVAL = 15;

    private int enemiesRemaining;
    private bool isSpawning = false;
    private const string ENEMY_TAG = "Enemy"; // Tag ที่ใช้สำหรับนับศัตรู

    void Start()
    {
        // ตรวจสอบความพร้อม
        if (spawnPoints.Length == 0 || monsterPrefab1 == null)
        {
            Debug.LogError("WaveManager ไม่พร้อม! ตรวจสอบ Spawn Points และ Prefabs ใน Inspector");
            enabled = false;
            return;
        }

        // เริ่ม Wave แรกทันที
        StartNextWave();
    }

    void Update()
    {
        // ถ้ากำลัง Spawn หรือ Wave ยังไม่ถึง 100 ให้ข้ามการตรวจสอบ
        if (isSpawning || currentWave > MAX_WAVE) 
            return;

        // นับศัตรูที่เหลืออยู่
        enemiesRemaining = GameObject.FindGameObjectsWithTag(ENEMY_TAG).Length;

        // ถ้าศัตรูหมดแล้ว และยังไม่ถึง Wave สุดท้าย
        if (enemiesRemaining <= 0)
        {
            if (currentWave < MAX_WAVE)
            {
                currentWave++;
                Debug.Log($"--- Wave {currentWave} เริ่มแล้ว! ---");
                StartNextWave();
            }
            else
            {
                Debug.Log("เกมจบ! คุณชนะครบ 100 Wave แล้ว!");
                // ใส่โค้ดจบเกมที่นี่
                enabled = false;
            }
        }
    }

    void StartNextWave()
    {
        isSpawning = true;
        StartCoroutine(SpawnWaveCoroutine());
    }

    IEnumerator SpawnWaveCoroutine()
    {
        // ตรวจสอบว่าเป็น Boss Wave หรือไม่
        bool isBossWave = (currentWave % BOSS_WAVE_INTERVAL == 0);
        
        if (isBossWave)
        {
            // --- Boss Wave: Spawn แค่บอสตัวเดียว ---
            Debug.Log($"!!! BOSS WAVE ที่ {currentWave} !!!");
            yield return StartCoroutine(SpawnEnemy(bossPrefab, 1));
        }
        else
        {
            // --- Normal Wave ---
            GameObject enemyToSpawn;
            int enemyCount;

            // 1. กำหนดชนิดมอนสเตอร์ตามเงื่อนไข
            if (currentWave >= 20)
            {
                enemyToSpawn = monsterPrefab3;
            }
            else if (currentWave >= 10)
            {
                enemyToSpawn = monsterPrefab2;
            }
            else // Wave 1-9
            {
                enemyToSpawn = monsterPrefab1;
            }

            // 2. กำหนดจำนวนศัตรู (เพิ่มขึ้นตาม Wave)
            // ตัวอย่าง: Wave 1 = 5 ตัว, Wave 20 = 5 + 20/5 = 9 ตัว
            enemyCount = baseEnemyCount + (currentWave / 5); 
            
            Debug.Log($"Spawn มอนสเตอร์ชนิด: {enemyToSpawn.name} จำนวน: {enemyCount}");
            yield return StartCoroutine(SpawnEnemy(enemyToSpawn, enemyCount));
        }
        
        isSpawning = false;
    }

    /// <summary>
    /// สั่ง Spawn ศัตรูจำนวนหนึ่งตัวตามชนิดที่กำหนด
    /// </summary>
    IEnumerator SpawnEnemy(GameObject enemyPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // สุ่มเลือกจุด Spawn
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPosition = randomPoint.position;

            // ใช้ NavMesh.SamplePosition เพื่อให้มั่นใจว่า Spawn บน NavMesh (แก้ Error เก่า)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 5f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
            }
            // ถ้า SamplePosition ล้มเหลว ให้ใช้ตำแหน่งเดิมไปก่อน

            // Spawn มอนสเตอร์
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}