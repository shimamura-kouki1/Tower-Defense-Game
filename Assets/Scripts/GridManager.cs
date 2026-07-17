using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Tooltip("グリットの横幅"), SerializeField] public int _width;
    [Tooltip("グリットの縦幅"), SerializeField] public int _height;
    [Tooltip("グリットのサイズ"), SerializeField] public float _cellSize;

    private GridCell[,] _gridCell;

    //平面座標をワールド座標に変化
    public Vector3 GritToWorld(Vector2Int pos)
    {
        return new Vector3(pos.x * _cellSize, 0, pos.y * _cellSize);
    }

    //ワールド座標を平面座標に変化
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / _cellSize);
        int y = Mathf.FloorToInt(worldPos.z / _cellSize);
        return new Vector2Int(x, y);
    }

    //
    public GridCell GetCell(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= _width || pos.y >= _height)
            return null;

        return _gridCell[pos.x, pos.y];
    }

    private void Awake()
    {
        CreatGrid();
    }

    //グリットの配置（生成）
    private void CreatGrid()
    {
        _gridCell = new GridCell[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                //今はグラウンド判定しかない状態
                _gridCell[x, y] = new GridCell { GritPos = new Vector2Int(x, y), CellType = CellType.Ground };
            }

        }
    }

    //ギズモ表示
    private void OnDrawGizmos()
    {
        if (_gridCell == null) return;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var cell = _gridCell[x, y];
                if (cell == null) continue;

                // 色分け
                switch (cell.CellType)
                {
                    case CellType.Ground:
                        Gizmos.color = Color.green;
                        break;

                    case CellType.HighGround:
                        Gizmos.color = Color.blue;
                        break;
                }

                // マスの中心
                Vector3 center = new Vector3(
                    x * _cellSize + _cellSize / 2f,
                    0,
                    y * _cellSize + _cellSize / 2f
                );

                // 小さい立方体で表示
                Gizmos.DrawCube(center, new Vector3(_cellSize, 0.01f, _cellSize));

                // 中心点（デバッグ用）
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(center, 0.05f);
            }
        }

        Gizmos.color = Color.green;
        for (int x = 0; x <= _width; x++)
        {
            Vector3 start = new Vector3(x * _cellSize, 0, 0);
            Vector3 end = new Vector3(x * _cellSize, 0, _height * _cellSize);
            Gizmos.DrawLine(start, end);
        }
        for (int y = 0; y <= _height; y++)
        {
            Vector3 start = new Vector3(0, 0, y * _cellSize);
            Vector3 end = new Vector3(_width * _cellSize, 0, y * _cellSize);
            Gizmos.DrawLine(start, end);
        }
    }

}
