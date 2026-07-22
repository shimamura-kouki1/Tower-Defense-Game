/// <summary>ユニットがグリッド上で向いている方向</summary>
public enum GridDirection
{
    Up,    // +y（基準方向。攻撃パターンはこの向きを基準に定義する）
    Right, // +x
    Down,  // -y
    Left   // -x
}