using UnityEngine;

public class Enemy : MonoBehaviour
{
    // === การตั้งค่าการเคลื่อนที่และโจมตี ===
    [Header("Movement")]
    public float speed = 2f;
    public float rotationSpeed = 10f; // ความเร็วในการหมุนตัว (ใหม่: เพื่อความนุ่มนวล)

    [Header("Attack")]
    public float attackRange = 2f;      // ระยะที่จะเริ่มโจมตี
    public float attackRate = 1f;       // อัตราการโจมตี (ครั้งต่อวินาที)
    public int damage = 10;             // ดาเมจต่อครั้ง
    
    // === ตัวแปรภายใน ===
    private Transform player;
    private float nextAttackTime = 0f;

    // === ฟังก์ชันเริ่มต้นที่ Wave Spawner เรียกใช้ ===
    // ใช้เพื่อกำหนดเป้าหมายทันทีที่มอนสเตอร์เกิด
    public void Init(Transform playerTransform)
    {
        player = playerTransform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // 🧍‍♂️ เดินเข้าหา Player เมื่ออยู่ไกลเกินระยะโจมตี
            MoveTowardsPlayer();
        }
        else
        {
            // 🛑 หยุดเคลื่อนที่เมื่ออยู่ในระยะโจมตี
            StopMoving(); 
            
            // ⚔️ โจมตีเมื่อถึงเวลา
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                // nextAttackTime = Time.time + 1f / attackRate; // สูตรนี้ถูกต้องแล้ว
                nextAttackTime = Time.time + (1f / attackRate);
            }
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        
        // 1. เคลื่อนที่
        transform.position += direction * speed * Time.deltaTime;
        
        // 2. หมุนตัวให้หันไปทางผู้เล่นอย่างนุ่มนวล (3D)
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    void StopMoving()
    {
        // หากใช้ Rigidbody ควรตั้งค่า velocity เป็น Vector3.zero 
        // แต่เนื่องจากโค้ดนี้ใช้ transform.position โดยตรง จึงไม่ต้องทำอะไรในส่วนนี้
        // มอนสเตอร์จะหยุดเองเพราะไม่มีการเพิ่ม position ใน Update()
    }

    void AttackPlayer()
    {
        // ต้องแน่ใจว่า Player มี Script PlayerHealth
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        
        // ** (ตัวเลือก) หมุนตัวให้หันไปทางผู้เล่นทันทีก่อนโจมตี **
        transform.LookAt(player); 
        
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
    
    // === การจัดการความตาย (ต้องเรียกใช้จากระบบ Health/Damage) ===
    public void OnDeath() 
    {
        // แจ้ง Wave Spawner ว่ามอนสเตอร์ตัวนี้ตายแล้ว
        WaveSpawner spawner = FindObjectOfType<WaveSpawner>();
        if (spawner != null)
        {
            spawner.EnemyDied();
        }
        
        // ทำลาย GameObject ตัวเอง
        Destroy(gameObject);
    }
}