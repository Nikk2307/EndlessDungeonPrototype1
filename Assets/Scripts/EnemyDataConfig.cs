using UnityEngine;

public enum EnemyAttackRange
{
    Melee,
    Range
}

public enum EnemyType
{
    SkeletonWarrior,
    SkeletonArcher
}

[CreateAssetMenu(fileName = "New Enemy Base Data", menuName = "Create Enemy Base Data")]
public class EnemyDataConfig : ScriptableObject
{
    public float health;
    public float attackDamage;
    public EnemyAttackRange enemyAttackRange;
    public EnemyType enemyType;
    public GameObject enemyPrefab;
}
