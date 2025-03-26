using TMPro;
using UnityEngine;

public class Balance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balance;

    private void Start()
    {
        _balance.text = Bank.Instance.Coins.ToString();
    }
}
