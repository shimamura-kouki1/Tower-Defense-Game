using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("基本情報")]
    [SerializeField] public string unitName = "New Unit";
    [SerializeField] public UnitBase prefab;
    [SerializeField] public UnitType unitType;

    [Header("コスト・耐久（フェーズA 2週目で本格利用）")]
    [SerializeField] public int unitCost;
    [SerializeField] public float maxHP;
    [SerializeField] public float defense;

    [Header("戦闘性能")]
    [SerializeField] public float attackPower;
    [SerializeField, Tooltip("秒。攻撃のクールダウン基礎値")]
    public float attackInterval = 1.0f;
    [SerializeField] public float _attackSpeed;

    [Header("攻撃パターン（Up=前方基準の相対座標リスト）")]
    [SerializeField, Tooltip("自分を(0,0)としたとき攻撃できるマスの相対座標。Up向き基準で定義する")]
    public List<Vector2Int> attackPattern = new List<Vector2Int>();


    //[Header("↓手入力が面倒な場合、ここを設定してから下のContextMenuで自動生成")]
    //[SerializeField] private int _patternForwardMin = 1;
    //[SerializeField] private int _patternForwardMax = 2;
    //[SerializeField] private int _patternWidth = 3;


    [Header("拡張用（遠距離ユニットなどで使用）")]
    public GameObject projectilePrefab;

    //[SerializeField] public int _coolDown;

    //[ContextMenu("前方矩形パターンを生成")]
    //private void GenerateForwardRectPattern()
    //{
    //    attackPattern = AttackPatternUtility.GenerateForwardRect(_patternForwardMin, _patternForwardMax, _patternWidth);
    //}
}
