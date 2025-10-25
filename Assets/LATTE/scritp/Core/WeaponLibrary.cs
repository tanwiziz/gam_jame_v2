using UnityEngine;
using System.Collections.Generic;

public static class WeaponLibrary
{
    static List<WeaponDefinition> pool = new();

    public static void SetPool(IEnumerable<WeaponDefinition> list)
    {
        pool.Clear();
        pool.AddRange(list);
    }

    public static WeaponDefinition GetRandomWeapon()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("WeaponLibrary: pool is empty.");
            return null;
        }

        return pool[Random.Range(0, pool.Count)];
    }
}
