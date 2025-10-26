using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float damage = 10f;
    public float fireRate = 1f;
    public float range = 15f;

    private float nextShot;

    void Update()
    {
        if (Time.time < nextShot) return;

        EnemyHealth target = FindClosestEnemy();
        if (target == null) return;

        GameObject go = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile p = go.GetComponent<Projectile>();
        if (p != null) p.Init(target.transform, damage);

        nextShot = Time.time + 1f / fireRate;
    }

    EnemyHealth FindClosestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        EnemyHealth closest = null;
        float minDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
             float dist = Vector3.Distance(transform.position, e.transform.position);
             if (dist < minDist && dist <= range)
             {
                minDist = dist;
                 closest = e;
             }
         }
        return closest;
     }
}
