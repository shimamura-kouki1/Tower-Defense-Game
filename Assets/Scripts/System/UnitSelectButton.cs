using UnityEngine;

/// <summary>
/// ユニット選択UIのボタンに1つずつアタッチする。
/// InspectorでunitTypeを設定し、ButtonのOnClickイベントに
/// このコンポーネントのOnClick()を登録する。
/// </summary>
public class UnitSelectButton : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private UnitData unitData;

    public void OnClick()
    {
        player.SelectUnit(unitData);
    }
}
