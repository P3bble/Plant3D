using UnityEngine;
using TMPro;

public class DiamondManager : MonoBehaviour
{
    public static DiamondManager Instance;
    public int money;
    public TextMeshProUGUI moneyText;

    void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = "$" + money.ToString();
    }
}
