using System;
using UnityEngine;

public class DPManager : MonoBehaviour
{
    [SerializeField] private int _startDP = 10;
    [SerializeField] private int _maxDP = 99;
    [SerializeField] private int _recoverAmount = 1;
    [SerializeField] private int _recoverFrame = 60;

    private int _currentDP;
    private int _frameCounter;

    public int CurrnetDP => _currentDP;
    public int MaxDP => _maxDP;

    public event Action<int> OnDPChanged;

    /// <summary>
    /// コストが足りるかどうか
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool CanConsume(int amount)
    {
        return _currentDP >= amount;
    }

    /// <summary>
    /// 実際に消費する
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool Consume(int amount)
    {
        if (!CanConsume(amount))
        {
            return false;
        }

        _currentDP -= amount;

        OnDPChanged?.Invoke(_currentDP);

        return true;
    }

    /// <summary>
    /// DPを加算していく
    /// </summary>
    /// <param name="amount"></param>
    public void Add(int amount)
    {
        _currentDP += amount;

        if (_currentDP > _maxDP)
        {
            _currentDP = _maxDP;
        }

        OnDPChanged?.Invoke(_currentDP);
    }

    void Update()
    {
        _frameCounter++;

        if (_frameCounter >= _recoverFrame)
        {
            _frameCounter = 0;
            Add(_recoverAmount);
        }
    }

    private void Start()
    {
        _currentDP = _startDP;
    }
}
