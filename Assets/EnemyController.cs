// ต้องมี 🟢 using UnityEngine.AI;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // ตัวแปรสาธารณะที่ตั้งค่าใน Inspector
    public float moveSpeed = 5f;     
    public float attackRange = 2f;   
    public float chaseRange = 20f;   
    public float attackCooldown = 1.5f; 
    
    private float lastAttackTime;        
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator; 

    void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); 

        if (agent != null)
        {
            // 🟢 โค้ดแก้ปัญหา Spawn นอก NavMesh (สำคัญมาก)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }


            
            agent.speed = moveSpeed;
            agent.updateRotation = false; // ให้โค้ด LookAtDirection จัดการการหมุน
        }

        lastAttackTime = -attackCooldown    ;
    }

    void Update()
    {
        // หยุดทำงานหาก Agent ถูกปิด (เช่น เมื่อมอนสเตอร์ตาย)
        if (player == null || agent == null || !agent.enabled || animator == null) 
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        
        // สั่งให้หันหน้าเข้าหาผู้เล่นเสมอ
        LookAtDirection(player.position - transform.position); 

        if (distance <= chaseRange)
        {
            if (distance > agent.stoppingDistance)
            {
                // สถานะ วิ่งไล่ตาม (CHASE/RUN)
                agent.SetDestination(player.position);
                agent.isStopped = false;

                animator.SetBool("IsAttacking", false);
                // ใช้ค่า moveSpeed เพื่อควบคุม Animation Parameter "Speed"
                animator.SetFloat("Speed", agent.velocity.magnitude > 0.1f ? moveSpeed : 0f); 
            }
            else 
            {
                // สถานะ โจมตี (ATTACK)
                agent.isStopped = true;

                animator.SetBool("IsAttacking", true);
                animator.SetFloat("Speed", 0f); // หยุดวิ่ง

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
        }
        else // อยู่นอกระยะ Chase Range (IDLE)
        {
            agent.isStopped = true;
            animator.SetBool("IsAttacking", false);
            animator.SetFloat("Speed", 0f); 
        }
    }

    // 🟢 ฟังก์ชัน LookAtDirection (แก้ปัญหา CS0103)
    void LookAtDirection(Vector3 direction)
    {
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
        
        if (flatDirection.sqrMagnitude > 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); 
        }
    }

    // 🟢 ฟังก์ชัน Attack
    void Attack()
    {
        lastAttackTime = Time.time;
        // ... (ใส่โค้ดทำความเสียหายให้ผู้เล่นตรงนี้) ...
    }
}