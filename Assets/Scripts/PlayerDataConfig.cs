using UnityEngine;

[CreateAssetMenu(fileName = "New Player Base Data", menuName = "Create Player Base Data")]
public class PlayerDataConfig : ScriptableObject
{
    public float health;
    public float attackDamage;
    public float attackRate;
    public PlayerType playerType;
}
