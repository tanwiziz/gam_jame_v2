using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    float currentHealth;

    private Enemy enemy; // เชื่อมกับ Enemy ที่มี event OnDied อยู่แล้ว

    void Awake()
    {
        currentHealth = maxHealth;
        enemy = GetComponent<Enemy>();
    }

    /// <summary>
    /// เรียกจาก Projectile หรือ BaseShooter เมื่อโดนยิง
    /// </summary>
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // แจ้งระบบ Enemy ให้ยิง event OnDied
        if (enemy != null)
            enemy.Die(); // ให้ Enemy จัดการ event OnDied เอง

        Destroy(gameObject); // ทำลาย object
    }
}
