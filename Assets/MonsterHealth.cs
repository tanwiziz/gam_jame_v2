using UnityEngine;
using UnityEngine.AI; // จำเป็นสำหรับ NavMeshAgent ใน Die()

// *** แก้ไขตรงนี้: ต้องมี : MonoBehaviour ***
public class MonsterHealth : MonoBehaviour 
{
    [Header("Health Settings")]
    public float maxHealth = 100f; 
    private float currentHealth;
    
    public bool isDead { get; private set; } = false;

    void Awake()
    {
        currentHealth = maxHealth;
        if (!gameObject.CompareTag("Enemy"))
        {
            Debug.LogWarning($"Monster {gameObject.name} does not have the 'Enemy' tag.");
        }
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

        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null) enemyController.enabled = false;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;
        
        // ทำให้ GameObject หายไปจาก Scene และ WaveManager จะนับลดลง
        Destroy(gameObject, 3f); 
    }
}