using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // ... (Other existing variables for Spawning) ...
    
    [Header("Wave Logic")]
    public int currentWave = 1;
    private int enemiesRemaining = 0; // The counter we track
    private bool isSpawning = false; // Flag to prevent starting a new wave while the current one is still being spawned

    // ... (Your other methods like StartNextWave, SpawnMonster, etc.) ...
    
    // *** The function called by MonsterHealth.cs when an enemy dies ***
    public void OnMonsterDied()
    {
        enemiesRemaining--; 
        
        Debug.Log($"ศัตรูเหลือ: {enemiesRemaining}");

        // Check if the current wave is truly finished
        if (enemiesRemaining <= 0 && !isSpawning)
        {
            // Start logic for next wave
            currentWave++;
            Debug.Log($"--- Wave {currentWave} เริ่มแล้ว! ---");
            // You should call your method to start the next wave here
            // Example: StartNextWave(); 
        }
    }
    
    // *** CRITICAL: You must increment enemiesRemaining when spawning! ***
    // Ensure this line is inside your spawning loop/coroutine:
    // enemiesRemaining++;
}