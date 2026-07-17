using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [SerializeField] protected UnitData _unitData;

    protected float _currentHP;
    protected float _coolDownTimer;
    protected Transform currentTarget;

    public bool IsDead { get; private set; }


    protected virtual void Awake()
    {
          if(_unitData != null)  _currentHP = _unitData._maxHP;
    }

    /// <summary>グリッド配置システムから、設置完了時に呼んでもらう想定のフック</summary>
    public virtual void OnPlaced(Vector3Int gridPosition)
    {
        // フェーズA 2週目：ここでDP消費・在庫チェックなどを追加予定
    }

    public virtual void Init()
    {

    }

    //Enemyの索敵
    public virtual void SearchEnemies()
    {

    }

    //攻撃
    public virtual void Attack()
    {
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
    }
}
