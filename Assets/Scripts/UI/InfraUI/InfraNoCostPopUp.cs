using TMPro;
using UnityEngine;

public class InfraNoCostPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _moneyText;

    public void Init(int money)
    {
        _moneyText.text = money.ToString()+"G";
    }
}
