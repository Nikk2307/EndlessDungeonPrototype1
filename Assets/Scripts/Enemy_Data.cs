using UnityEngine;

[CreateAssetMenu(fileName ="New Enemy", menuName ="Enemy")]
public class Enemy_Data : ScriptableObject
{
    public string enemyName;
    public float moveSpeed;
    public int enemyPos;
    public float health;
    public float armor;
    public float meleeAttack;
    public float rangedAttack;
    public float attackRate;
}
