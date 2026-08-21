using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public const int TickRate = 60;

    public long CurrentTick { get; private set; }

    [SerializeField] private float _gameSpeed = 1f;
    private float _tickAccumulator;

    private void Update()
    {
        UpdateGameTick();
    }

    private void UpdateGameTick()
    {
        _tickAccumulator += Time.unscaledDeltaTime * _gameSpeed * TickRate;

        while (_tickAccumulator >= 1f)
        {
            _tickAccumulator -= 1f;
            CurrentTick++;
        }
    }
}
