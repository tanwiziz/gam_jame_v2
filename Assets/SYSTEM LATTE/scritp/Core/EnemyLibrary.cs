using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Enemy Library")]
public class EnemyLibrary : ScriptableObject
{
    public List<Enemy> enemies = new();

    public Enemy GetRandomEnemy()
    {
        if (enemies == null || enemies.Count == 0) return null;
        return enemies[Random.Range(0, enemies.Count)];
    }
}
