using UnityEngine;
using UnityEngine.AI; // For NavMeshAgent in Die()

// *** FIX: Must derive from MonoBehaviour (image_1989ba.png) ***
public class MonsterHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; 
    private float currentHealth;
    
    public bool isDead { get; private set; } = false;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Disable components to prevent NavMesh errors (image_19119c.png)
        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null) enemyController.enabled = false;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false; // Completely disables the agent
        
        // 2. Notify the Wave Manager that the monster is dead
        // Using FindAnyObjectByType is the modern fix for the obsolete warning (image_12167d.png)
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.OnMonsterDied();
        }

        // 3. Destroy the GameObject
        Destroy(gameObject, 3f); 
    }
}