using System;
using UnityEngine;

/// <summary>
/// ユニットの状態。
///   Deployed      : 配置済み・戦闘参加中(被弾でHP減少)
///   ForcedRetreat : HP0による強制退却(演出用の一瞬の状態)
///   Cooldown      : 再配置不可・タイマー進行中
///   Redeployable  : クールダウン明け、再配置可能
/// </summary>
public enum UnitState
{
    Deployed,
    ForcedRetreat,
    Cooldown,
    Redeployable
}

public abstract class UnitBase : MonoBehaviour, IGridPositioned, IDamageable
{
    [SerializeField] protected UnitData _unitData;
    [SerializeField] protected GridDirection _facing = GridDirection.Up;

    protected float _currentHP;
    protected float _coolDownTimer; // 攻撃のクールダウン(既存)
    protected Transform _currentTarget;
    protected Vector2Int _gridPosition;

    // 回転後パターンを保持する実バッファ。配列自体は通常の参照型フィールドなので保持して問題ない。
    // （ref struct であるSpanそのものはフィールドに置けないため、あくまで「配列を持ち、Spanはその都度作る」構成にする）
    [SerializeField] private Vector2Int[] _rotatedPatternBuffer;
    private GridDirection _cachedFacing;
    private bool _patternComputed;

    // ---- 強制退却・クールダウン関連 ----
    private float _retreatCooldownTimer; // 退却クールダウン(攻撃クールダウンとは別物)

    public bool IsDead { get; protected set; }
    public Vector2Int GridPosition => _gridPosition;

    public UnitState State { get; private set; } = UnitState.Deployed;

    /// <summary>状態が変化した時に発火。UI(布陣の穴の表示等)やアニメ制御用</summary>
    public event Action<UnitState> OnStateChanged;

    /// <summary>強制退却が発生した瞬間に発火(演出トリガー用)</summary>
    public event Action OnForcedRetreat;

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Update()
    {
        // 退却中・クールダウン中は戦闘ロジックを止める
        if (State == UnitState.Cooldown)
        {
            _retreatCooldownTimer -= Time.deltaTime;
            if (_retreatCooldownTimer <= 0f)
            {
                SetState(UnitState.Redeployable);
            }
            return;
        }

        if (State != UnitState.Deployed) return;
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

    /// <summary>
    /// 初回配置を試みる。DP消費が成功したらtrue。
    /// Player側でInstantiate直後に呼ぶ想定。
    /// </summary>
    public virtual bool TryDeploy(DPManager dp)
    {
        if (_unitData == null) return false;
        if (!dp.Consume(_unitData.unitCost)) return false;

        _currentHP = _unitData.maxHP;
        IsDead = false;
        SetState(UnitState.Deployed);
        return true;
    }

    /// <summary>
    /// 再配置を試みる。Redeployable状態からのみ呼べる。
    /// ユニットごとの倍率コストが適用される。
    /// 成功したら見た目を復帰させる処理も呼び出し側(Player)で行うこと。
    /// </summary>
    public virtual bool TryRedeploy(DPManager dp, Vector2Int newGridPosition, GridDirection facing = GridDirection.Up)
    {
        if (State != UnitState.Redeployable) return false;
        if (_unitData == null) return false;

        int cost = _unitData.GetRedeployCost();
        if (!dp.Consume(cost)) return false;

        _currentHP = _unitData.maxHP; // 全回復の仕様
        IsDead = false;
        OnPlaced(newGridPosition, facing);
        SetState(UnitState.Deployed);
        return true;
    }

    /// <summary>グリッド配置システムから、設置(または再配置)完了時に呼んでもらう想定のフック</summary>
    public virtual void OnPlaced(Vector2Int gridPosition, GridDirection facing = GridDirection.Up)
    {
        _patternComputed = false;
        _gridPosition = gridPosition;
        _facing = facing;
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

        if (_rotatedPatternBuffer == null || _rotatedPatternBuffer.Length != count)
        {
            _rotatedPatternBuffer = new Vector2Int[count];
            _patternComputed = false;
        }

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
    public virtual void TakeDamage(float amount)
    {
        if (_unitData == null) return;
        if (State != UnitState.Deployed) return; // 戦闘参加中のみ被弾する

        float actualDamage = Mathf.Max(0f, amount - _unitData.defense);
        _currentHP -= actualDamage;

        if (_currentHP <= 0f) ForceRetreat();
    }

    /// <summary>
    /// HP0による強制退却。即Destroyではなく、非表示化してクールダウンへ移行する。
    /// </summary>
    protected virtual void ForceRetreat()
    {
        IsDead = true;
        SetState(UnitState.ForcedRetreat);
        OnForcedRetreat?.Invoke();

        // 見た目を消す(破壊はしない。再配置時に再利用する)
        SetVisualActive(false);

        // 退却演出は一瞬なので、即座にクールダウンへ。
        // 演出時間を挟みたい場合はここをコルーチン化してdelayを入れる。
        _retreatCooldownTimer = _unitData != null ? _unitData.retreatCooldown : 15f;
        SetState(UnitState.Cooldown);
    }

    /// <summary>
    /// 見た目・当たり判定の有効/無効を切り替える。
    /// 退却中はレンダラーとコライダーを止めて、戦闘に一切関与しない状態にする。
    /// </summary>
    protected virtual void SetVisualActive(bool active)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = active;
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = active;
    }

    private void SetState(UnitState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(newState);
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
}