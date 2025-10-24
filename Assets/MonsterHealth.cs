using UnityEngine;
using UnityEngine.AI; // เพิ่มเข้ามาเพื่อใช้ NavMeshAgent

// 🟢 บรรทัดนี้สำคัญที่สุด: ต้องมีชื่อคลาสที่ตรงกับชื่อไฟล์ และสืบทอดจาก MonoBehaviour
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
            Debug.LogWarning($"Monster {gameObject.name} does not have the 'Enemy' tag. Wave Manager counting will be incorrect.");
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

        // 1. ปิดการทำงานของ Controller
        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.enabled = false;
        }

        // 2. ปิดการทำงานของ Nav Mesh Agent 
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }
        
        // 3. ทำลาย GameObject
        float destroyDelay = 3f; 
        Destroy(gameObject, destroyDelay);
    }
}