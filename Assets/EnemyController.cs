// *** แก้ไขตรงนี้: ต้องมี using UnityEngine และ UnityEngine.AI ***
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // ... (ตัวแปรเดิม) ...
    public float moveSpeed = 3f;     
    public float attackRange = 2f;   
    public float chaseRange = 15f;   
    public float attackCooldown = 1.5f; 
    
    private float lastAttackTime;        
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator; // 🟢 ตัวแปร Animator

    void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // 🟢 ต้องหา Animator Component

        if (agent != null)
        {
            // 🟢 โค้ดแก้ปัญหา Spawn นอก NavMesh (CS0246)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }

            agent.speed = moveSpeed;
            agent.updateRotation = false; 
        }

        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (player == null || agent == null || !agent.enabled || animator == null) 
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        LookAtDirection(player.position - transform.position); 

        // 1. วิ่งไล่ตาม (นอกระยะ Stopping Distance)
        if (distance <= chaseRange)
        {
            if (distance > agent.stoppingDistance)
            {
                agent.SetDestination(player.position);
                agent.isStopped = false;

                // 🟢 ANIMATION: วิ่ง (Run/Walking)
                animator.SetBool("IsAttacking", false);
                animator.SetFloat("Speed", agent.velocity.magnitude > 0.1f ? moveSpeed : 0f);
            }
            // 2. โจมตี (อยู่ในระยะ Stopping Distance)
            else 
            {
                agent.isStopped = true;

                // 🟢 ANIMATION: โจมตี (Attack)
                animator.SetBool("IsAttacking", true);
                animator.SetFloat("Speed", 0f); // หยุดวิ่ง

                if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
        }
        // 3. อยู่นอกระยะ Chase Range
        else
        {
            agent.isStopped = true;
            animator.SetBool("IsAttacking", false);
            animator.SetFloat("Speed", 0f); // 🟢 ANIMATION: Idle
        }
    }

    // *** แก้ไขตรงนี้: ต้องมีฟังก์ชันนี้ (CS0103) ***
    void LookAtDirection(Vector3 direction)
    {
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
        
        if (flatDirection.sqrMagnitude > 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); 
        }
    }

    // *** แก้ไขตรงนี้: ต้องมีฟังก์ชันนี้ (หากหายไป) ***
    void Attack()
    {
        lastAttackTime = Time.time;
        Debug.Log($"{gameObject.name} กำลังโจมตีผู้เล่น!");
    }
}