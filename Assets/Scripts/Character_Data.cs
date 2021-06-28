using UnityEngine;

[CreateAssetMenu(fileName ="New Character", menuName ="Characters")]
public class Character_Data : ScriptableObject
{
    public string Character_Name;
    public int CharacterID;
    public float Health;
    public float Armor;
    public float Health_Regen;
    public float Health_Recovery;
    public float Melee_Attack;
    public float Ranged_Attack;
    public float Attack_Rate;
    public float Shield;


}
