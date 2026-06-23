using System;
using UnityEngine;

public class WalletManager : MonoBehaviour
{

    public float Balance { get; private set; } = 0f;

    public event Action<float> OnBalanceChanged;

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
