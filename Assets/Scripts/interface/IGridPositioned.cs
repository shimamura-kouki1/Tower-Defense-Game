using UnityEngine;

/// <summary>
/// グリッド座標を持つオブジェクトが実装するインターフェース。
/// ユニット・敵の双方で実装することで、射程判定をワールド座標ではなくグリッド座標ベースに統一できる。
/// </summary>
public interface IGridPositioned
{
    Vector2Int GridPosition { get; }
}
