using TMPro;
using UnityEngine;

public class AdditionalCoins : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinsCount;

    public void ChangeText(string text)
    {
        _coinsCount.text = text;
    }
}
