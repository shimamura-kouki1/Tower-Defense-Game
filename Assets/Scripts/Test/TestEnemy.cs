using UnityEngine;

/// <summary>
/// 週3で本実装するまでのテスト用ダミー敵。
/// タグを"Enemy"に設定し、Inspectorで_gridPositionを指定してシーンに置けば
/// ユニットの攻撃ループ(索敵→射程判定→攻撃→ダメージ)を確認できる。
/// </summary>
public class TestEnemy : MonoBehaviour, IDamageable, IGridPositioned
{
    [SerializeField] private float _maxHP = 50f;
    [SerializeField] private Vector2Int _gridPosition;

    private float _currentHP;

    public bool IsDead { get; private set; }
    public Vector2Int GridPosition => _gridPosition;

    private void Awake() => _currentHP = _maxHP;

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        _currentHP -= amount;
        Debug.Log($"{name} took {amount} damage. HP: {_currentHP}");

        if (_currentHP <= 0f)
        {
            IsDead = true;
            Destroy(gameObject);
        }
    }
}
