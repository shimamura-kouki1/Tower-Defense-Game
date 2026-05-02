using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform highlight; // 光るオブジェクト
    [Tooltip("タワーのprefab"), SerializeField] public GameObject _towerPrefab;

    void Update()
    {
        HighlightCellUnderMouse();

        if (Input.GetMouseButtonDown(0))
        {
            PlaceTower();
        }
    }

    void HighlightCellUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 地面にRayを飛ばす

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // ワールド → グリッド
            Vector2Int gridPos = gridManager.WorldToGrid(hit.point);

            var cell = gridManager.GetCell(gridPos);
            if (cell == null) return;

            // グリッド → ワールド
            Vector3 worldPos = gridManager.GritToWorld(gridPos);

            //マスの中心に合わせる
            worldPos += new Vector3(gridManager._cellSize / 2f, 0.01f, gridManager._cellSize / 2f);

            highlight.position = worldPos;
        }
    }

    private void PlaceTower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2Int gridPos = gridManager.WorldToGrid(hit.point);
            var cell = gridManager.GetCell(gridPos);
            if (cell == null) return;

            if (!cell.CanBuild || cell.BuildObject != null)
            {
                Debug.Log("置けない");
                return;
            }

            Vector3 wolrdPos = gridManager.GritToWorld(gridPos);

            wolrdPos += new Vector3(gridManager._cellSize / 2f, 0.01f, gridManager._cellSize / 2f);

            GameObject tower = Instantiate(_towerPrefab, wolrdPos, Quaternion.identity);

            cell.BuildObject = tower;
        }
    }
}
