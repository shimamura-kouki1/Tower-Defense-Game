using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Base")]
    public string EnemyName;

    [Header("Stats")]
    public int MaxHP;
    public int MoveSpeed;

    [Header("Attack")]
    public float AttackPower;
    public float AttackRange;
    public float AttackInterval;
}
