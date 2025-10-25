using UnityEngine;
using System.Linq;

public class BaseShooter : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float range = 18f;
    public float fireRate = 1.2f;
    public float damage = 10f;

    float nextShot;

    void Update()
    {
        if (!projectilePrefab || !firePoint) return;
        if (Time.time < nextShot) return;

        var enemies = FindObjectsOfType<Enemy>();
        if (enemies.Length == 0) return;

        var e = enemies.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
        float dist = Vector3.Distance(transform.position, e.transform.position);
        if (dist > range) return;

        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var p = go.GetComponent<Projectile>();
        if (p) p.Init(e.transform, damage);
        nextShot = Time.time + 1f / fireRate;
    }
}
