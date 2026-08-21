using System;
using UnityEngine;

public class GridCell
{
    public Vector2Int GritPos;//グリットの座標
    public CellType CellType;//グリットのタイプ
    public GameObject BuildObject;//何がグリットに配置されているか

    /// <summary>
    /// このセルに配置されているUnitBase本体への参照。
    /// 強制退却時にセルを解放するために使う(Player側から設定)。
    /// </summary>
    public UnitBase OccupyingUnit;

    /// <summary>
    /// 敵が通れるか
    /// </summary>
    public bool CanEnemyPass
    {
        get
        {
            return CellType == CellType.Ground;
        }
    }

    public bool CanPlace(UnitType unitType)
    {
        if (BuildObject != null) return false;

        switch (CellType)
        {
            case CellType.Ground:
                return unitType == UnitType.Melee;

            case CellType.HighGround:
                return unitType == UnitType.Ranged;

            default: return false;
        }
    }

    /// <summary>
    /// 配置されていたユニットが強制退却した際に呼ぶ。BuildObjectとOccupyingUnitを両方クリアする。
    /// </summary>
    public void Clear()
    {
        BuildObject = null;
        OccupyingUnit = null;
    }
}

public enum CellType
{
    Ground,
    HighGround
}
