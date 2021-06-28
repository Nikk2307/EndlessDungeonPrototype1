using UnityEngine;

[CreateAssetMenu(fileName ="New Character", menuName ="Characters")]
public class Character_Data : ScriptableObject
{
    public string characterName;
    public int characterPos;
    public float health;
    public float armor;
    public float healthRegen;
    public float healthRecovery;
    public float meleeAttack;
    public float rangedAttack;
    public float attackRate;
    public float shield;
}
