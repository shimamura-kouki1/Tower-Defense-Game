using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] public int _width;
    [SerializeField] public int _height;
    [SerializeField] public float _cellSize;

     private GridCell[,] _gridCell;

    private void Awake()
    {
        Creatgrid();
    }

    private void Creatgrid()
    {
        _gridCell = new GridCell[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _gridCell[x, y] = new GridCell { GritPos = new Vector2Int(x, y), CanBuild = true };
            }

        }
    }

    private void OnDrawGizmos()
    {
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

    public Vector3 GritToWorld(Vector2Int pos)
    {
        return new Vector3(pos.x * _cellSize, 0, pos.y * _cellSize);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / _cellSize);
        int y = Mathf.FloorToInt(worldPos.z / _cellSize);
        return new Vector2Int(x, y);
    }

    public GridCell GetCell(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= _width || pos.y >= _height)
            return null;

        return _gridCell[pos.x, pos.y];
    }

}
