using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("基本情報")]
    [SerializeField] public string unitName = "New Unit";
    [SerializeField] public UnitBase _prefab;

    [Header("ステータス")]
    [SerializeField] public int _unitCost;
    [SerializeField] public float _maxHP;
    [SerializeField] public float _attackSpeed;
    [SerializeField] public float _attackPower;
    [SerializeField] public float _defense; 
    [SerializeField,Tooltip("[0] = 行/[1] = 列")] public int[] _attackRange;
    [SerializeField] public int _coolDown;
    [SerializeField,Tooltip(" 秒。クールダウンの基礎値")] public float AttackInterval = 1.0f;
    [SerializeField] public UnitType _unitType;
}
