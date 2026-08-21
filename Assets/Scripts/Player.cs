using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform highlight; // 光るオブジェクト

    [Header("選択可能なユニット一覧(同じUnitTypeでも複数種類を登録できる)")]
    [SerializeField] private UnitData[] _availableUnits;

    [Header("現在選択中のユニット(ボタンから変更される)")]
    [SerializeField] private UnitData _currentUnitData;

    [SerializeField] private DPManager _dPManager;

    private void Update()
    {
        HighlightCellUnderMouse();

        if (Input.GetMouseButtonDown(0))
        {
            PlaceTower();
        }
    }

    /// <summary>
    /// UI選択ボタン(UnitSelectButton)から呼ばれる。選択中のUnitDataを切り替えるだけ。
    /// </summary>
    public void SelectUnit(UnitData unitData)
    {
        if (unitData == null || unitData.prefab == null)
        {
            Debug.LogWarning("選択しようとしたUnitDataが不正です(prefab未設定など)。");
            return;
        }
        _currentUnitData = unitData;
    }

    public void HighlightCellUnderMouse()
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
        if (_currentUnitData == null || _currentUnitData.prefab == null)
        {
            Debug.LogWarning("配置するユニットが選択されていません。");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Vector2Int gridPos = gridManager.WorldToGrid(hit.point);
        var cell = gridManager.GetCell(gridPos);
        if (cell == null) return;

        // セルに置けるかどうかは、あくまでUnitData.unitType(近接/遠隔)で判定する
        if (!cell.CanPlace(_currentUnitData.unitType))
        {
            Debug.Log("置けない");
            return;
        }

        Vector3 worldPos = gridManager.GritToWorld(gridPos);
        worldPos += new Vector3(gridManager._cellSize / 2f, 0.01f, gridManager._cellSize / 2f);

        // UnitData.prefabはUnitBase型そのものなので、GetComponent不要でそのままInstantiateできる
        UnitBase unit = Instantiate(_currentUnitData.prefab, worldPos, Quaternion.identity);

        // DPチェック。足りなければ配置を取り消す
        if (!unit.TryDeploy(_dPManager))
        {
            Debug.Log("DP不足のため配置できません");
            Destroy(unit.gameObject);
            return;
        }

        unit.OnPlaced(gridPos);

        cell.BuildObject = unit.gameObject;
        cell.OccupyingUnit = unit;

        // 強制退却時にセルを解放する
        unit.OnStateChanged += (newState) =>
        {
            if (newState == UnitState.ForcedRetreat)
            {
                if (cell.OccupyingUnit == unit) // 別ユニットに既に置き換わっていないか一応確認
                {
                    cell.Clear();
                }
            }
        };
    }
}
