using UnityEngine;

[CreateAssetMenu(fileName ="New Enemy", menuName ="Enemy")]
public class Enemy_Data : ScriptableObject
{
    public string Enemy_Name;
    public float Speed;
    public int EnemyID;
    public float Health;
    public float Armor;
    public bool Melee_Attack;
    public bool Ranged_Attack;
    public float Damage;
    public float Attack_Rate;
}
