using System.Collections.Generic;
using UnityEngine;

/// <summary>ユニットがグリッド上で向いている方向</summary>
public enum GridDirection
{
    Up,    // +y（基準方向。攻撃パターンはこの向きを基準に定義する）
    Right, // +x
    Down,  // -y
    Left   // -x
}

/// <summary>
/// 攻撃パターン(相対座標リスト)の回転・生成を行うユーティリティ。
/// パターンは常に「Up(前方=+y)」を基準に定義し、実際の向きに応じてRotateで変換する。
/// </summary>
public static class AttackPatternUtility
{
    /// <summary>Up基準で定義された相対座標を、指定した向きに回転させる</summary>
    public static Vector2Int Rotate(Vector2Int offset, GridDirection facing)
    {
        switch (facing)
        {
            case GridDirection.Up: return offset;
            case GridDirection.Right: return new Vector2Int(offset.y, -offset.x);
            case GridDirection.Down: return new Vector2Int(-offset.x, -offset.y);
            case GridDirection.Left: return new Vector2Int(-offset.y, offset.x);
            default: return offset;
        }
    }

    /// <summary>
    /// 前方矩形パターンを生成するヘルパー。
    /// 例: 前方2マス・幅3マス(自分の1マス先〜2マス先、左右1マスずつ)なら
    ///     GenerateForwardRect(forwardMin: 1, forwardMax: 2, width: 3)
    /// </summary>
    public static List<Vector2Int> GenerateForwardRect(int forwardMin, int forwardMax, int width)
    {
        var pattern = new List<Vector2Int>();
        int halfWidth = width / 2;
        int xMin = -halfWidth;
        int xMax = width - halfWidth - 1; // 奇数幅なら中心対称、偶数幅なら右にずれる

        for (int y = forwardMin; y <= forwardMax; y++)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                pattern.Add(new Vector2Int(x, y));
            }
        }
        return pattern;
    }
}