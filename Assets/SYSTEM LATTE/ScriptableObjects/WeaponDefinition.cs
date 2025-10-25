using UnityEngine;

public enum WeaponRarity { Common, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "WeaponDefinition", menuName = "Game/Weapon Definition")]
public class WeaponDefinition : ScriptableObject
{
    public string weaponName = "Pistol";
    public Sprite icon;
    public GameObject projectilePrefab;
    public WeaponRarity rarity = WeaponRarity.Common;
    public float damage = 10f;
    public float fireRate = 1f;
    public float range = 10f;
    public float projectileSpeed = 20f;
    public int pierce = 1;
    public int maxLevel = 5;
    public Vector2Int[] shapeOffsets = new Vector2Int[] { Vector2Int.zero };
}

