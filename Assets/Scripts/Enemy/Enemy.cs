using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected EnemyData _data;

    public virtual void Initialize(EnemyData data)
    {
        _data = data;
    }
}
