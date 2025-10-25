using UnityEngine;

public class Projectile : MonoBehaviour
{
    Transform target;
    float damage;
    public float speed = 24f;

    public void Init(Transform t, float dmg) { target = t; damage = dmg; }

    void Update()
    {
        if (!target) { Destroy(gameObject); return; }
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.position) < 0.25f)
        {
            var e = target.GetComponent<Enemy>();
            if (e) e.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
