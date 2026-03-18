using System;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance { get; private set; }

    public float Balance { get; private set; } = 0f;

    public event Action<float> OnBalanceChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void AddMoney(float amount)
    {
        Balance += amount;

        OnBalanceChanged?.Invoke(Balance);
    }

    public bool TrySpendMoney(float amount)
    {

        if (Balance < amount)
        {
            Debug.LogWarning("Not enough balance to spend.");
            return false;
        }
        Balance -= amount;

        OnBalanceChanged?.Invoke(Balance);
        return true;
    }
}
