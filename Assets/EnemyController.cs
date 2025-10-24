// 🟢 ต้องมีบรรทัดนี้: ทำให้รู้จัก Vector3, Transform, GameObject, Time
using UnityEngine;
// 🟢 ต้องมีบรรทัดนี้: ทำให้รู้จัก NavMeshAgent
using UnityEngine.AI; 

public class EnemyController : MonoBehaviour
{
    // ตัวแปรที่ปรับใน Inspector
    public float moveSpeed = 3f;     
    public float attackRange = 2f;   
    public float chaseRange = 15f;   

    public float attackCooldown = 1.5f; 
    private float lastAttackTime;        

    private Transform player;
    private NavMeshAgent agent;

    void Awake()
    {
        // ... (โค้ด Awake เดิม) ...
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        agent = GetComponent<NavMeshAgent>();
        
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = false; 
            // ลบบรรทัดที่ผูกค่า Stopping Distance กับ Attack Range ออกแล้ว
        }

        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (player == null || agent == null || !agent.enabled)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        
        // 1. สั่งให้หันหน้าเข้าหาผู้เล่นทันทีเสมอ (แก้ปัญหาไม่มองตอนเริ่มต้น)
        LookAtDirection(player.position - transform.position); 

        // 2. ตรรกะการไล่ตาม (Chase Logic)
        if (distance <= chaseRange)
        {
            if (distance > agent.stoppingDistance)
            {
                agent.SetDestination(player.position);
                agent.isStopped = false;
            }
            else 
            {
                agent.isStopped = true;

                if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    /// <summary>หันศัตรูเข้าหาทิศทางที่กำหนด</summary>
    void LookAtDirection(Vector3 direction)
    {
        // 🟢 Vector3 จะใช้ได้เพราะมีการ using UnityEngine;
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
        
        if (flatDirection.sqrMagnitude > 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); 
        }
    }

    /// <summary>ฟังก์ชันการโจมตี</summary>
    void Attack()
    {
        lastAttackTime = Time.time;
        Debug.Log($"{gameObject.name} กำลังโจมตีผู้เล่น! (Cooldown: {attackCooldown}s)");
    }
}