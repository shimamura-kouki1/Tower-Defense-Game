using UnityEngine;

public class MeleeUnit : UnitBase
{
    protected override void Attack(Transform target)
    {
        Debug.Log("攻撃");
        var damageable = target.GetComponent<IDamageable>();
        damageable?.TakeDamage(_unitData.attackPower);
        // TODO: 攻撃モーション・エフェクトはここに追加
    }
}
