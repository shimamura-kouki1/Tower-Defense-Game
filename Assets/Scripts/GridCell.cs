using System;
using UnityEngine;

public class GridCell
{
    public Vector2Int GritPos;//グリットの座標
    public CellType CellType;//グリットのタイプ
    public GameObject BuildObject;//何がグリットに配置されているか

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

            case CellType.HightGround:
                return unitType == UnitType.Ranged;

            default: return false;
        }
    }
}

public enum CellType
{
    Ground,
    HightGround
}
