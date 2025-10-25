using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hp = 40f, speed = 2f, dmg = 7f, attackRange = 2f, cd = 1f;
    float nextAtk;
    Transform baseT;
    BaseHealth baseHP;

    public System.Action OnDied;

    void Start()
    {
        var b = GameObject.FindWithTag("Base");
        if (b) { baseT = b.transform; baseHP = b.GetComponent<BaseHealth>(); }
    }

    void Update()
    {
        if (!baseT) return;
        float d = Vector3.Distance(transform.position, baseT.position);
        if (d > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, baseT.position, speed * Time.deltaTime);
        }
        else if (Time.time >= nextAtk)
        {
            baseHP?.TakeDamage(dmg);
            nextAtk = Time.time + cd;
        }
    }

    public void TakeDamage(float x)
    {
        hp -= x;
        if (hp <= 0f)
        {
            OnDied?.Invoke();
            Destroy(gameObject);
        }
    }
    public void Die()
    {
        OnDied?.Invoke();  // แจ้ง WaveSystem ว่าตายแล้ว
        // ไม่ต้อง Destroy ที่นี่ เพราะ EnemyHealth ทำลาย object ให้อยู่แล้ว
        // ถ้าคุณอยากให้ Enemy.cs เป็นคน Destroy เอง ก็ย้าย Destroy(gameObject) มาที่นี่แทน
    }
}
