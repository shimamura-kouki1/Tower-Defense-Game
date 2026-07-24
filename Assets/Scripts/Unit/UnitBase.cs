using System;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour,IGridPositioned,IDamageable
{

    [SerializeField] protected UnitData _unitData;
    [SerializeField] protected GridDirection _facing = GridDirection.Up;

    protected float _currentHP;
    protected float _coolDownTimer;
    protected Transform _currentTarget;
    protected Vector2Int _gridPosition;

    // 回転後パターンを保持する実バッファ。配列自体は通常の参照型フィールドなので保持して問題ない。
    // （ref struct であるSpanそのものはフィールドに置けないため、あくまで「配列を持ち、Spanはその都度作る」構成にする）
    [SerializeField] private Vector2Int[] _rotatedPatternBuffer;
    private GridDirection _cachedFacing;
    private bool _patternComputed;

    public bool IsDead { get; private set; }
    public Vector2Int GridPosition => _gridPosition;

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Update()
    {
        if (IsDead || _unitData == null) return;

        _coolDownTimer -= Time.deltaTime;

        if (_coolDownTimer >= 0f) return;

        if (_currentTarget == null || !IsTargetInRange(_currentTarget))
        {
            _currentTarget = FindTarget();
        }

        if (_currentTarget != null && _coolDownTimer <= 0f)
        {
            Attack(_currentTarget);
            _coolDownTimer = _unitData.attackInterval;
        }
    }

    /// <summary>攻撃の中身は各ユニットで実装する（近接・遠距離・範囲攻撃など）</summary>
    protected abstract void Attack(Transform target);

    /// <summary>グリッド配置システムから、設置完了時に呼んでもらう想定のフック</summary>
    public virtual void OnPlaced(Vector2Int gridPosition, GridDirection facing = GridDirection.Up)
    {
        _patternComputed = false;
        _gridPosition = gridPosition;
        _facing = facing;
        // フェーズA 2週目：ここでDP消費・在庫チェックなどを追加予定
    }

    public virtual void Init()
    {
        if (_unitData != null) _currentHP = _unitData.maxHP;
    }

    //Enemyの索敵
    public virtual Transform FindTarget()
    {
        //タグを使用するのはやめたい、どこかから生きてるEnemyのリストを取得したい
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        ReadOnlySpan<Vector2Int> pattern = GetRotatedPattern();

        Transform nearest = null;
        int minDist = int.MaxValue;

        foreach (var enemy in enemies)
        {
            var targetGrid = enemy.GetComponent<IGridPositioned>();
            if (targetGrid == null) continue;

            Vector2Int diff = targetGrid.GridPosition - _gridPosition;
            if (!Contains(pattern, diff)) continue;

            int dist = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }



    /// <summary>対象が現在の攻撃パターン内に入っているか判定</summary>
    protected virtual bool IsTargetInRange(Transform target)
    {
        if (target == null || _unitData == null) return false;

        var targetGrid = target.GetComponent<IGridPositioned>();
        if (targetGrid == null) return false;

        Vector2Int diff = targetGrid.GridPosition - _gridPosition;
        return Contains(GetRotatedPattern(), diff);
    }

    /// <summary>
    /// UnitData.attackPatternを現在の向きに回転させたものを返す。
    /// 向きが変わらない限り配列を使い回すので、毎フレーム呼んでもヒープ確保は発生しない。
    /// </summary>
    protected ReadOnlySpan<Vector2Int> GetRotatedPattern()
    {
        if (_unitData == null || _unitData.attackPattern == null)
            return ReadOnlySpan<Vector2Int>.Empty;

        int count = _unitData.attackPattern.Count;

        // パターン数が変わった場合(データ差し替え等)だけ配列を作り直す
        if (_rotatedPatternBuffer == null || _rotatedPatternBuffer.Length != count)
        {
            _rotatedPatternBuffer = new Vector2Int[count];
            _patternComputed = false;
        }

        // 向きが変わっていなければ再計算不要
        if (!_patternComputed || _cachedFacing != _facing)
        {
            for (int i = 0; i < count; i++)
            {
                _rotatedPatternBuffer[i] = AttackPatternUtility.Rotate(_unitData.attackPattern[i], _facing);
            }
            _cachedFacing = _facing;
            _patternComputed = true;
        }

        return _rotatedPatternBuffer;
    }

    /// <summary>
    /// 小規模コレクション前提の線形探索。
    /// 先輩にHashSetをおすすめされたが、要素数が十数個程度ならHashSetより高速・省メモリらしい。
    /// 要素が増えたらIsTargetInRangeをオーバーライドしてhashSetに変換するといい
    /// </summary>
    private static bool Contains(ReadOnlySpan<Vector2Int> pattern, Vector2Int value)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == value) return true;
        }
        return false;
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="amount"></param>
    public virtual void TakeDamage(float amount)
    {
        if (_unitData == null) return;
        if (IsDead) return;

        float actualDamage = Mathf.Max(0f, amount - _unitData.defense);
        _currentHP -= actualDamage;

        if (_currentHP <= 0f) Die();
    }

    protected virtual void Die()
    {
        IsDead = true;
        // フェーズA 2週目：即Destroyではなく「強制退却」に分岐させる処理をここに追加予定
        Destroy(gameObject);
    }

    /// <summary>配置後に向きだけ変更したい場合に使う（回転砲台など）</summary>
    public virtual void SetFacing(GridDirection facing)
    {
        _facing = facing;
    }

    // デバッグ用：攻撃パターンを可視化。グリッドのXY→ワールドXZに対応していると仮定（1マス=1ワールド単位）。
    // 実際のグリッド軸の対応やセルサイズが違う場合はここを合わせて調整してください。
    private void OnDrawGizmosSelected()
    {
        if (_unitData == null || _unitData.attackPattern == null) return;

        Gizmos.color = Color.red;
        foreach (var offset in _unitData.attackPattern)
        {
            Vector2Int rotated = AttackPatternUtility.Rotate(offset, _facing);
            Vector3 worldOffset = new Vector3(rotated.x, 0f, rotated.y);
            Gizmos.DrawWireCube(transform.position + worldOffset, Vector3.one * 0.9f);
        }
    }

    //HPが減少する処理
    public virtual void HPDecrease()
    {
        //TakeDamage();
    }
}
