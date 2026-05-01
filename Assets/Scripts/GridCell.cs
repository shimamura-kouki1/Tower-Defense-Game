using UnityEngine;

public class GridCell
{
    public Vector2Int GritPos;//グリットの座標
    public bool CanBuild = true;//グリットに設置できるか
    public GameObject BuildObject;//何がグリットに配置されているか
}
