using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [SerializeField] protected UnitData _unitData;

    protected float _currentHP;
    protected float _coolDownTimer;
    protected Transform _currentTarget;

    public bool IsDead { get; private set; }


    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Update()
    {
        //if (IsDead || _unitData == null) return;

        //_coolDownTimer -= Time.deltaTime;

        //if (_currentTarget == null || !IsTargetInRange(_currentTarget))
        //{
        //    _currentTarget = FindTarget();
        //}

        //if (_currentTarget != null && _coolDownTimer <= 0f)
        //{
        //    Attack(_currentTarget);
        //    _coolDownTimer = _unitData.attackInterval;
        //}
    }

    /// <summary>攻撃の中身は各ユニットで実装する（近接・遠距離・範囲攻撃など）</summary>
    //protected abstract void Attack(Transform target);

    /// <summary>グリッド配置システムから、設置完了時に呼んでもらう想定のフック</summary>
    public virtual void OnPlaced(Vector3Int gridPosition)
    {
        // フェーズA 2週目：ここでDP消費・在庫チェックなどを追加予定
    }

    public virtual void Init()
    {
        if (_unitData != null) _currentHP = _unitData._maxHP;
    }

    //Enemyの索敵
    public virtual void SearchEnemies()
    {

    }

    protected virtual bool IsTargetInRange(Transform target)
    {
        if (target == null) return false;
        float distance = Vector3.Distance(transform.position, target.position);
        return true;
        //return distance <= _unitData._attackRange;
    }

    //ダメージを受ける
    public virtual void TakeDamage()
    {

    }

    //HPが減少する処理
    public virtual void HPDecrease()
    {
        TakeDamage();
    }


    //死亡処理
    public virtual void Die()
    {
        IsDead = true;
        // フェーズA 2週目：即Destroyではなく「強制退却」に分岐させる処理をここに追加予定
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (_unitData == null) return;
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, _unitData.attackRange);
    }
}
